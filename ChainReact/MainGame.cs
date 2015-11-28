﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using ChainReact.Core;
using ChainReact.Core.Game;
using ChainReact.Core.Game.Field;
using ChainReact.Core.Game.Objects;
using ChainReact.Core.Server;
using ChainReact.Core.Utilities;
using ChainReact.Input;
using ChainReact.Scenes;
using ChainReact.Utilities;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Audio;
using Sharpex2D.Framework.Audio.WaveOut;
using Sharpex2D.Framework.Content;
using Sharpex2D.Framework.Network;
using Sharpex2D.Framework.Rendering;
using Sharpex2D.Framework.Rendering.OpenGL;
using Button = ChainReact.UI.Button;
using TextureUtilities = ChainReact.Utilities.TextureUtilities;

namespace ChainReact
{
    public class MainGame : Game
    {
        #region Network
        private byte _statusCode;

        private Map _map;
        private Server _internalServer;
        private NetworkPeer _clientPeer;

        private Dictionary<string, Action<string>> _registeredCommands;
        #endregion

        #region Components
        private InputManager _input;
        #endregion

        #region Textures
        private Texture2D _gameBorder;
        private Texture2D _fieldBorder;
        private Texture2D _wabeBorder;
        private Texture2D _gameField;

        private SoundEffect _effect;
        #endregion

        #region Scenes
        private MainMenuScene _mainMenuScene;
        #endregion

        #region Game Values
        private string _currentPlayer;
        private string _currentPlayerColor;
        private string _winner;

        private string _lastMessage;
        private bool _gameOver;
        #endregion

        public bool IsReady => _statusCode == 200;

        public override void Setup(LaunchParameters launchParameters)
        {
            if (launchParameters.KeyAvailable("Debugger") && launchParameters["Debugger"] == "Enabled")
            {
                Debugger.Launch();
            }
            GraphicsManager = new GLGraphicsManager
            {
                PreferredBackBufferHeight = 768,
                PreferredBackBufferWidth = 768
            };
            var window = Get<GameWindow>();
            window.Title = "ChainReact - Development Build";
            Content.RootPath = "Content";
            SoundManager = new WaveOutSoundManager();
            ResourceManager.Instance.SoundManager = SoundManager;
        }

        public override void Initialize()
        {
            var gameInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "settings.dat");
            var players = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "Players");
            if (!players.Exists) players.Create();
            GameSettings.Instance.Load(gameInfo, players);
            _internalServer = new Server(ServerMode.Internal, NetworkPeer.Protocol.Tcp);
            base.Initialize();

        }

        public override void LoadContent()
        {
            _gameField = TextureUtilities.CreateTextureFromColor(64, 64, Color.Gray);
            var explosion = Content.Load<Texture2D>("Textures/Explosion");
            ResourceManager.Instance.LoadResource<Texture2D>(this, "Background", "Textures/Background");
            ResourceManager.Instance.LoadResource<Texture2D>(this, "Unpowered", "Textures/Unpowered");
            ResourceManager.Instance.LoadResource<Texture2D>(this, "Powered", "Textures/Powered");
            ResourceManager.Instance.LoadResource<Texture2D>(this, "Unowned", "Textures/Default");
            ResourceManager.Instance.LoadResource<Texture2D>(this, "ButtonMenu", "Textures/ButtonMenu");
            ResourceManager.Instance.LoadResource<Texture2D>(this, "ButtonMenuHovered", "Textures/ButtonMenuHovered");
            ResourceManager.Instance.LoadResource<Texture2D>(this, "ButtonSettings", "Textures/ButtonSettings");
            ResourceManager.Instance.LoadResource<Texture2D>(this, "ButtonSettingsHovered", "Textures/ButtonMenuHovered");
            ResourceManager.Instance.LoadResource<SpriteFont>(this, "ButtonFont", "Fonts/ButtonFont");
            ResourceManager.Instance.LoadResource<SpriteFont>(this, "DefaultFont", "Fonts/Default");
            ResourceManager.Instance.LoadResource<Texture2D>(this, "ButtonExit", "Textures/ButtonExit");
            ResourceManager.Instance.LoadResource<Texture2D>(this, "ButtonExitHovered", "Textures/ButtonExitHovered");
            ResourceManager.Instance.LoadResource<Texture2D>(this, "ButtonHowToPlay", "Textures/ButtonHTP");
            ResourceManager.Instance.LoadResource<Texture2D>(this, "ButtonHowToPlayHovered", "Textures/ButtonHTPHovered");
            ResourceManager.Instance.LoadResource<TextFile>(this, "HowToPlay", "Others/howtoplay");
            ResourceManager.Instance.LoadResource<SpriteFont>(this, "WinnerFont", "Fonts/WinnerFont");

            if (ResourceManager.Instance.SoundAvailable)
            {
                var sound = Content.Load<Sound>("Sounds/ExplosionSound");
                _effect = new SoundEffect(sound);
                ResourceManager.Instance.ImportResource("ExplosionSound", _effect);
            }

            ResourceManager.Instance.ImportResource("Explosion", explosion);

            _registeredCommands = new Dictionary<string, Action<string>>();
            var defaultCommands = new DefaultCommands(this);
            defaultCommands.RegisterCommands();

            _clientPeer = new NetworkPeer(NetworkPeer.Protocol.Tcp) { MaxReceiveBuffer = 1000000 };
            _clientPeer.Connect(new IPEndPoint(IPAddress.Loopback, 22794));
            _clientPeer.MessageArrived += MessageReceived;
            _clientPeer.Send(new OutgoingMessage(Encoding.UTF8.GetBytes("request")));


            var fullWabeSizeX = ChainReactGame.FullSize * Map.DefaultLengthX;
            var fullWabeSizeY = ChainReactGame.FullSize * Map.DefaultLengthY;
            _wabeBorder = TextureUtilities.CreateBorderFromColor(64, 64, 1, Color.Olive);
            _gameBorder = TextureUtilities.CreateBorderFromColor((int)fullWabeSizeX, (int)fullWabeSizeY, 3, Color.White);
            var fieldSize = (int)((ChainReactGame.FullSize) / 3);
            _fieldBorder = TextureUtilities.CreateBorderFromColor(fieldSize, fieldSize, 1, Color.Black);

            _input = new InputManager();
            _mainMenuScene = new MainMenuScene(this, _input);

            var fadeInOut = new FadeInOutTransition(Color.Black, 0f, 800f);
            SceneManager.ChangeWithTransition(_mainMenuScene, fadeInOut);
            SceneManager.Add(_mainMenuScene);
            SceneManager.ChangeWithTransition(_mainMenuScene, fadeInOut);
            var controlFromHandle = Control.FromHandle(GameWindow.Default.Handle);
            var form = controlFromHandle as FrmLoading;
            form?.DestroyControls();
        }

        public override void Update(GameTime time)
        {
            //if (_ready)
            //{
            //    var message = "set:1:1";
            //    _clientPeer.Send(new OutgoingMessage(Encoding.UTF8.GetBytes(message)));
            //    var message2 = "set:1:2";
            //    _clientPeer.Send(new OutgoingMessage(Encoding.UTF8.GetBytes(message2)));
            //    _ready = false;
            //}
            //if (_game.Queue.IsActionQueued)
            //{
            //    var actions = _game.Queue.GetAllActions();
            //    foreach (var act in actions.Select(x => x.Value).SelectMany(actPair => actPair))
            //    {
            //        act?.Invoke(time);
            //    }
            //    return;
            //}
            _input.Update(time);
            var menu = _input.Menu.Value;
            if (menu && SceneManager.ActiveScene != _mainMenuScene && !_mainMenuScene.ElementManager.Any(t => t.GetType() == typeof(Button) && ((Button)t).IsClicked))
            {
                var fadeInOut = new FadeInOutTransition(Color.Black, 1000f, 800f);
                SceneManager.ChangeWithTransition(_mainMenuScene, fadeInOut);
                return;
            }
            SceneManager.Update(time);

            if (!_gameOver)
            {
                if (_input.Clicked)
                {
                    var wabe = _map.AbsoluteToWabe(_input.Position);
                    var field = wabe?.ConvertAbsolutePositionToWabeField(_input.Position, ChainReactGame.FullSize);
                    if (field == null) return;
                    Set(wabe.X, wabe.Y, field.Id);
                }
            }
        }

        public override void Draw(SpriteBatch batch, GameTime time)
        {
            if (_map == null) return;
            var background = ResourceManager.Instance.GetResource<Texture2D>("Background");
            var font = ResourceManager.Instance.GetResource<SpriteFont>("DefaultFont");
            batch.Begin();
            for (var x = 0; x < 11; x++)
            {
                for (var y = 0; y < 8; y++)
                {
                    var tileX = x * ChainReactGame.FullSize;
                    var tileY = y * ChainReactGame.FullSize;
                    batch.DrawTexture(background, new Rectangle(tileX, tileY, ChainReactGame.FullSize, ChainReactGame.FullSize));
                }
            }

            for (var x = 1; x < 7; x++)
            {
                for (var y = 1; y < 7; y++)
                {
                    var tileX = x * ChainReactGame.FullSize;
                    var tileY = y * ChainReactGame.FullSize;
                    batch.DrawTexture(_gameField, new Rectangle(tileX, tileY, ChainReactGame.FullSize, ChainReactGame.FullSize));
                }
            }

            const float cut = (ChainReactGame.WabeSize / 3) * ChainReactGame.ScalingFactor;
            var fullWabeSizeX = ChainReactGame.FullSize * _map.GetLengthX();
            var fullWabeSizeY = ChainReactGame.FullSize * _map.GetLengthY();
            foreach (var wabe in _map.Wabes)
            {
                var wabeX = (wabe.X + 1) * ChainReactGame.FullSize;
                var wabeY = (wabe.Y + 1) * ChainReactGame.FullSize;

                for (var x = 0; x <= 2; x++)
                {
                    for (var y = 0; y <= 2; y++)
                    {
                        var field = wabe.ConvertVector2ToWabeField(new Vector2(x, y));
                        var texture = SelectTextureFromField(field);
                        var mutltiplicatorX = cut * x;
                        var mutltiplicatorY = cut * y;
                        if (field.Type == WabeFieldType.Center)
                        {
                            var color = wabe.Owner?.Color ?? Color.White;
                            color.A = (wabe.Owner != null) ? (byte)205 : (byte)255;
                            batch.DrawTexture(texture, new Rectangle((wabeX) + mutltiplicatorX, (wabeY) + mutltiplicatorY, cut, cut), color);
                        }
                        else if (field.Type == WabeFieldType.Unused)
                        {
                            var color = wabe.Owner?.Color ?? Color.LightGray;
                            color.A = 128;
                            batch.DrawTexture(texture, new Rectangle((wabeX) + mutltiplicatorX, (wabeY) + mutltiplicatorY, cut, cut), color);
                        }
                        else
                        {
                            batch.DrawTexture(texture,
                                new Rectangle((wabeX) + mutltiplicatorX, (wabeY) + mutltiplicatorY, cut, cut));
                        }
                        var posX = mutltiplicatorX + wabeX;
                        var posY = mutltiplicatorY + wabeY;
                        if (GameSettings.Instance.FieldLines) batch.DrawTexture(_fieldBorder, new Rectangle(posX, posY, cut, cut));
                    }
                }


                if (GameSettings.Instance.WabeLines) batch.DrawTexture(_wabeBorder, new Rectangle(wabeX, wabeY, ChainReactGame.FullSize, ChainReactGame.FullSize));
            }
            if (GameSettings.Instance.BorderLines) batch.DrawTexture(_gameBorder, new Rectangle(ChainReactGame.FullSize, ChainReactGame.FullSize, fullWabeSizeX, fullWabeSizeY));
            if (!string.IsNullOrEmpty(_lastMessage))
            {
                batch.DrawString(_lastMessage, font, new Vector2(96, 680), Color.Black);
            }

            if (!string.IsNullOrEmpty(ResourceManager.Instance.LastSoundError))
            {
                batch.DrawString("Failed to play a sound: " + ResourceManager.Instance.LastSoundError, font, new Vector2(96, 720), Color.Red);
            }
            if (!string.IsNullOrEmpty(_currentPlayer))
            {
                batch.DrawString(_currentPlayer + $"'s turn ({_currentPlayerColor})", font, new Vector2(96, 60), Color.Black);
            }
            if (SceneManager.ActiveScene == null)
            {
                //foreach (var wabe in _map.ToList().Where(x => x.AnimationManager.IsRunning).Select(x => x.AnimationManager))
                //{
                //    wabe.Draw(batch, time);
                //}
            }
            if (_gameOver && !string.IsNullOrEmpty(_winner))
            {
                var winFont = ResourceManager.Instance.GetResource<SpriteFont>("WinnerFont");
                var midX = Get<GameWindow>().ClientSize.X / 2;
                var midY = Get<GameWindow>().ClientSize.Y / 2;
                var msg1 = _winner + " has won the game!";
                var msg2 = "Press R to restart the game!";
                var sizeMsg1 = winFont.MeasureString(msg1);
                var sizeMsg2 = winFont.MeasureString(msg2);
                var pos1 = new Vector2(midX - (sizeMsg1.X / 2), midY - (sizeMsg1.Y / 2) - 50);
                var pos2 = new Vector2(midX - (sizeMsg2.X / 2), midY - (sizeMsg2.Y / 2) + 20);
                batch.DrawString(msg1, winFont, pos1, Color.Crimson);
                batch.DrawString(msg2, winFont, pos2, Color.Crimson);
            }
            SceneManager.Draw(batch, time);
            batch.End();
        }

        public override void Unload()
        {
            var gameInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "settings.dat");
            GameSettings.Instance.Save(gameInfo);

            _internalServer.Shutdown();
            base.Unload();
        }

        private Texture2D SelectTextureFromField(WabeField field)
        {
            switch (field.Type)
            {
                case WabeFieldType.Unpowered:
                    return ResourceManager.Instance.GetResource<Texture2D>("Unpowered");
                case WabeFieldType.Powered:
                    return ResourceManager.Instance.GetResource<Texture2D>("Powered");
                case WabeFieldType.Unused:
                case WabeFieldType.Center:
                    return ResourceManager.Instance.GetResource<Texture2D>("Unowned");
                default:
                    return null;
            }
        }

        private void MessageReceived(object sender, IncomingMessageEventArgs e)
        {
            var isStatusCode = e.Message.Data.Length == 1;
            if (isStatusCode)
            {
                _statusCode = e.Message.Data[0];
            }
            else
            {
                var msg = Encoding.UTF8.GetString(e.Message.Data);
                var splitted = msg.Split(':');
                var command = splitted[0];
                if (_registeredCommands.ContainsKey(command))
                {
                    var val = _registeredCommands[command];
                    val?.Invoke(msg);
                }
                else
                {
                    Console.WriteLine($@"The command ""{command}"" is not registered!");
                }
            }
        }

        private void Set(int x, int y, int fieldId)
        {
            _clientPeer.Send(new OutgoingMessage(Encoding.UTF8.GetBytes("request")));
            var messageString = $"set:{x}:{y}:{fieldId}";
            var message = new OutgoingMessage(Encoding.UTF8.GetBytes(messageString));
            _clientPeer.Send(message);
        }

        public void Send(string msg)
        {
            var outgoing = new OutgoingMessage(Encoding.UTF8.GetBytes(msg));
            _clientPeer.Send(outgoing);
        }

        public void RegisterCommand(string command, Action<string> act)
        {
            if(_registeredCommands.ContainsKey(command))
                throw new InvalidOperationException(command + " is already registered!");
            _registeredCommands.Add(command, act);
        }

        public void SetMap(Map map)
        {
            _map = map;
        }

        public void SetMessage(string message)
        {
            _lastMessage = message;
        }

        public void SetGameover(bool gameOver)
        {
            _gameOver = gameOver;
        }

        public void SetCurrentPlayer(string name, string color)
        {
            _currentPlayer = name;
            _currentPlayerColor = color;
        }

        public void SetWinner(string name)
        {
            _winner = name;
        }
    }
}
