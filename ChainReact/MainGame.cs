using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ChainReact.Core;
using ChainReact.Core.Game;
using ChainReact.Core.Game.Animations;
using ChainReact.Core.Game.Field;
using ChainReact.Core.Game.Objects;
using ChainReact.Input;
using ChainReact.Scenes;
using ChainReact.Utilities;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Audio;
using Sharpex2D.Framework.Audio.WaveOut;
using Sharpex2D.Framework.Rendering;
using Sharpex2D.Framework.Rendering.OpenGL;
using Button = ChainReact.UI.Button;

namespace ChainReact
{
    public class MainGame : Game
    {
        #region Constants
        public const int WabeSize = 64;
        public const float ScalingFactor = 1.5F;
        #endregion

        #region Components
        private ChainReactGame _game;
        private InputManager _input;

        private List<Player> _players;
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

        private string _lastMessage;

        public ChainReactGame ChainReact => _game;

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
        }

        public override void Initialize()
        {
            var gameInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "settings.dat");
            var players = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "players");
            if (!players.Exists) players.Create();
            GameSettings.Instance.Load(gameInfo, players);
            base.Initialize();

        }

        public override void Unload()
        {
            Save();
            base.Unload();
        }

        public void Save()
        {
            var gameInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "settings.dat");
            GameSettings.Instance.Save(gameInfo);
        }

        public override void LoadContent()
        {
            _gameField = ColorTextureConverter.CreateTextureFromColor(64, 64, Color.Gray);
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

            var sound = Content.Load<Sound>("Sounds/ExplosionSound");
            _effect = new SoundEffect(sound);
            ResourceManager.Instance.ImportResource("ExplosionSound", _effect);

            ResourceManager.Instance.ImportResource("Explosion", explosion);

            _players = GameSettings.Instance.Players;

            _game = new ChainReactGame(this, _players, new Vector2(WabeSize, ScalingFactor));

            var fullWabeSizeX = WabeSize * ScalingFactor * _game.Wabes.GetLength(0);
            var fullWabeSizeY = WabeSize * ScalingFactor * _game.Wabes.GetLength(1);
            _wabeBorder = CreateBorderFromColor(64, 64, 1, Color.Olive);
            _gameBorder = CreateBorderFromColor((int)fullWabeSizeX, (int)fullWabeSizeY, 3, Color.White);
            var fieldSize = (int)((WabeSize * ScalingFactor) / 3);
            _fieldBorder = CreateBorderFromColor(fieldSize, fieldSize, 1, Color.Black);

            _input = new InputManager(this);

            _mainMenuScene = new MainMenuScene(this, _input);
            SceneManager.Add(_mainMenuScene);
            SceneManager.ActiveScene = _mainMenuScene;
        }

        public override void Update(GameTime time)
        {
            if (_game.Queue.IsActionQueued)
            {
                var actions = _game.Queue.GetAllActions();
                foreach (var act in actions.Select(x => x.Value).SelectMany(actPair => actPair))
                {
                    act?.Invoke(time);
                }
                return;
            }
            _input.Update(time);
            var menu = _input.Menu.Value;
            if (menu && SceneManager.ActiveScene != _mainMenuScene && !_mainMenuScene.ElementManager.Any(t => t.GetType() == typeof(Button) && ((Button)t).IsClicked))
            {
                SceneManager.ActiveScene = _mainMenuScene;
                return;
            }
            if (SceneManager.ActiveScene != null)
            {
                SceneManager.ActiveScene.OnUpdate(time);
                return;
            }

            if (!_game.GameOver)
            {
                if (_input.Clicked)
                {
                    var wabe = _game.ConvertAbsolutePositionToWabe(_input.Position, WabeSize * ScalingFactor);
                    var field = wabe?.ConvertAbsolutePositionToWabeField(_input.Position, WabeSize * ScalingFactor);
                    if (field == null) return;
                    string error;
                    _game.Set(_game.CurrentPlayer.Id, wabe, field, out error);
                    if (error != null)
                    {
                        _lastMessage = error;
                    }
                }
            }
            if (_game.GameOver && _input != null && _input.Reset.Value)
            {
                ResetGame();
            }
        }

        public override void Draw(SpriteBatch batch, GameTime time)
        {
            var background = ResourceManager.Instance.GetResource<Texture2D>("Background");
            var font = ResourceManager.Instance.GetResource<SpriteFont>("DefaultFont");
            batch.Begin();
            for (var x = 0; x < 11; x++)
            {
                for (var y = 0; y < 8; y++)
                {
                    var tileX = x * WabeSize * ScalingFactor;
                    var tileY = y * WabeSize * ScalingFactor;
                    batch.DrawTexture(background, new Rectangle(tileX, tileY, WabeSize * ScalingFactor, WabeSize * ScalingFactor));
                }
            }

            for (var x = 1; x < 7; x++)
            {
                for (var y = 1; y < 7; y++)
                {
                    var tileX = x * WabeSize * ScalingFactor;
                    var tileY = y * WabeSize * ScalingFactor;
                    batch.DrawTexture(_gameField, new Rectangle(tileX, tileY, WabeSize * ScalingFactor, WabeSize * ScalingFactor));
                }
            }

            const float cut = ((float)WabeSize / 3) * ScalingFactor;
            var fullWabeSizeX = WabeSize * ScalingFactor * _game.Wabes.GetLength(0);
            var fullWabeSizeY = WabeSize * ScalingFactor * _game.Wabes.GetLength(1);
            foreach (var wabe in _game.Wabes)
            {
                var wabeX = (wabe.X + 1) * WabeSize * ScalingFactor;
                var wabeY = (wabe.Y + 1) * WabeSize * ScalingFactor;

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


                if (GameSettings.Instance.WabeLines) batch.DrawTexture(_wabeBorder, new Rectangle(wabeX, wabeY, WabeSize * ScalingFactor, WabeSize * ScalingFactor));
            }
            if (GameSettings.Instance.BorderLines) batch.DrawTexture(_gameBorder, new Rectangle(WabeSize * ScalingFactor, WabeSize * ScalingFactor, fullWabeSizeX, fullWabeSizeY));
            if (!string.IsNullOrEmpty(_lastMessage))
            {
                batch.DrawString(!string.IsNullOrEmpty(_game.Message) ? _game.Message : _lastMessage, font,
                    new Vector2(96, 680), Color.Black);
            }
            else
            {
                if (!string.IsNullOrEmpty(_game.Message))
                {
                    batch.DrawString(_game.Message, font, new Vector2(96, 680), Color.Black);
                }
            }
            if (_game?.CurrentPlayer != null)
            {
                batch.DrawString(_game.CurrentPlayer.Name + $"'s turn ({_game.CurrentPlayer.GetColorString()}) ({_game.CurrentPlayer.Wins} Wins)", font, new Vector2(96, 60), Color.Black);
            }
            if (SceneManager.ActiveScene == null)
            {
                foreach (var wabe in _game.Wabes.Cast<Wabe>().ToList().Where(x => x.AnimationManager.IsRunning).Select(x => x.AnimationManager))
                {
                    wabe.Draw(batch, time);
                }
            }
            SceneManager.ActiveScene?.OnDraw(batch, time);
            batch.End();
        }

        private void ResetGame()
        {
            foreach (var player in _players)
            {
                player.ExecutedFirstPlace = false;
                player.Out = false;
                player.Save();
            }
            _players = GameSettings.Instance.Players;
            foreach (var player in _players)
            {
                player.ExecutedFirstPlace = false;
                player.Out = false;
                player.Save();
            }
            _game = new ChainReactGame(this, _players, new Vector2(WabeSize, ScalingFactor));
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

        private Texture2D CreateBorderFromColor(int width, int heigth, int penLength, Color color)
        {
            var tex = new Texture2D(width, heigth);
            tex.Lock();
            for (var x = 0; x <= width - 1; x++)
            {
                for (var y = 0; y <= heigth - 1; y++)
                    if ((penLength - x) > 0 || (penLength + x) > width - 1)
                        tex[x, y] = color;
            }

            for (var y = 0; y <= heigth - 1; y++)
            {
                for (var x = 0; x <= width - 1; x++)
                    if ((penLength - y) > 0 || (penLength + y) > heigth - 1)
                        tex[x, y] = color;
            }

            tex.Unlock();
            return tex;
        }
    }
}
