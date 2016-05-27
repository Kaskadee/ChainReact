using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ChainReact.Core;
using ChainReact.Core.Client;
using ChainReact.Core.Game;
using ChainReact.Core.Game.Field;
using ChainReact.Core.Game.Objects;
using ChainReact.Core.Utilities;
using ChainReact.Input;
using ChainReact.Properties;
using ChainReact.Utilities;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Audio;
using Sharpex2D.Framework.Content;
using Sharpex2D.Framework.Rendering;

namespace ChainReact.Components
{
    public class SingleplayerComponent : DrawableGameComponent
    {
        private readonly FileInfo _gameSettingsFile = new FileInfo("settings.dat");
        private readonly DirectoryInfo _playerDirectory = new DirectoryInfo("Players");

        public ClientIdentity LocalIdentity { get; private set; }

        #region Game Resources

        private Texture2D _gameAreaTexture;
        private Texture2D _gameBorder;
        private Texture2D _wabeBorder;
        private Texture2D _fieldBorder;
        private Texture2D _background;

        private SpriteFont _font;

        private IEnumerable<Player> _players;
        #endregion

        private ChainReactGame _game;
        private readonly InputManager _input;
        private string _lastMessage;

        private Vector2 _clientSize;

        public SingleplayerComponent(Game game, InputManager input, Vector2 clientSize) : base(game)
        {
            _input = input;
            _clientSize = clientSize;
            LoadIdentity();
            if (!_playerDirectory.Exists) _playerDirectory.Create();
            GameSettings.Instance.Load(_gameSettingsFile, _playerDirectory);
            LoadContent(game.Content);
        }

        private void LoadIdentity()
        {
            if (!File.Exists("identity.dat"))
            {
                Application.Run(new FrmCreateIdentity());
                if (!File.Exists("identity.dat"))
                {
                    MessageBox.Show(Resources.IdentityNotCreated, Resources.ErrorHeader,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(-1);
                }
            }
            try
            {
                using (var sr = new StreamReader("identity.dat"))
                {
                    var json = sr.ReadToEnd();
                    var identity = ClientIdentity.Deserialize(json);
                    LocalIdentity = identity;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(Resources.IdentityAccessDenied + ex.Message);
                Environment.Exit(-1);
            }

        }

        public override void LoadContent(ContentManager content)
        {
            _game = new ChainReactGame(false, true);

            if (ResourceManager.SoundAvailable)
            {
                var sound = ResourceManager.GetResource<Sound>("ExplosionSound");
                var effect = new SoundEffect(sound);
                ResourceManager.ImportResource("ExplosionSoundEffect", effect);
            }
            _players = GameSettings.Instance.Players;

            _gameAreaTexture = ColorTextureConverter.CreateTextureFromColor(64, 64, Color.Gray);

            var fullWabeSizeX = ChainReactGame.WabeSize * _game.GameMap.Wabes.GetLength(0);
            var fullWabeSizeY = ChainReactGame.WabeSize * _game.GameMap.Wabes.GetLength(1);
            var fieldSize = (int)((ChainReactGame.WabeSize) / 3);
            _wabeBorder = TextureUtilities.CreateBorderFromColor(64, 64, 1, Color.Olive);
            _gameBorder = TextureUtilities.CreateBorderFromColor((int)fullWabeSizeX, (int)fullWabeSizeY, 3, Color.White);
            _fieldBorder = TextureUtilities.CreateBorderFromColor(fieldSize, fieldSize, 1, Color.Black);

            _background = ResourceManager.GetResource<Texture2D>("Background");
            _font = ResourceManager.GetResource<SpriteFont>("DefaultFont");


            _game.Initialize(_players);
        }

        public override void Update(GameTime gameTime)
        {
            if (_game.Queue.IsActionQueued)
            {
                var actions = _game.Queue.GetAllActions();
                foreach (var act in actions.Select(x => x.Value).SelectMany(actPair => actPair))
                {
                    act?.Invoke(gameTime);
                }
                return;
            }
            if (!_game.GameOver)
            {
                if (_input.Clicked)
                {
                    var wabe = _game.GameMap.AbsoluteToWabe(_input.Position);
                    var field = wabe?.ConvertAbsolutePositionToWabeField(_input.Position, ChainReactGame.WabeSize);
                    if (field == null) return;
                    string error;
                    _game.Set(_game.CurrentPlayer.Id, wabe, field, out error);
                    if (error != null)
                    {
                        _lastMessage = error;
                    }
                }
            }
            else if (_input != null && _input.Reset)
            {
                ResetGame();
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch batch, GameTime time)
        {
            batch.Begin();
            // Draw wood background
            for (var x = 0; x < 11; x++)
            {
                for (var y = 0; y < 8; y++)
                {
                    var tileX = x * ChainReactGame.WabeSize;
                    var tileY = y * ChainReactGame.WabeSize;
                    batch.DrawTexture(_background, new Rectangle(tileX, tileY, ChainReactGame.WabeSize, ChainReactGame.WabeSize));
                }
            }

            // Draw gray game field
            for (var x = 1; x < 7; x++)
            {
                for (var y = 1; y < 7; y++)
                {
                    var tileX = x * ChainReactGame.WabeSize;
                    var tileY = y * ChainReactGame.WabeSize;
                    batch.DrawTexture(_gameAreaTexture, new Rectangle(tileX, tileY, ChainReactGame.WabeSize, ChainReactGame.WabeSize));
                }
            }

            // Draw wabes
            const float cut = (ChainReactGame.WabeSize / 3);
            var fullWabeSizeX = ChainReactGame.WabeSize * _game.GameMap.Wabes.GetLength(0);
            var fullWabeSizeY = ChainReactGame.WabeSize * _game.GameMap.Wabes.GetLength(1);
                foreach (var wabe in _game.GameMap.Wabes)
                {
                    var wabeX = (wabe.X + 1) * ChainReactGame.WabeSize;
                    var wabeY = (wabe.Y + 1) * ChainReactGame.WabeSize;

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
                            // Draw field border
                            if (GameSettings.Instance.FieldLines)
                                batch.DrawTexture(_fieldBorder, new Rectangle(posX, posY, cut, cut));
                        }
                    }
                    // Draw wabe border
                    if (GameSettings.Instance.WabeLines)
                        batch.DrawTexture(_wabeBorder, new Rectangle(wabeX, wabeY, ChainReactGame.WabeSize, ChainReactGame.WabeSize));
                }

            // Draw game border
            if (GameSettings.Instance.BorderLines)
                batch.DrawTexture(_gameBorder, new Rectangle(ChainReactGame.WabeSize, ChainReactGame.WabeSize, fullWabeSizeX, fullWabeSizeY));

            // Draw messages
            if (!string.IsNullOrEmpty(_lastMessage))
            {
                batch.DrawString(!string.IsNullOrEmpty(_game.Message) ? _game.Message : _lastMessage, _font,
                    new Vector2(96, 680), Color.Black);
            }
            else
            {
                if (!string.IsNullOrEmpty(_game.Message))
                {
                    batch.DrawString(_game.Message, _font, new Vector2(96, 680), Color.Black);
                }
            }

            if (!string.IsNullOrEmpty(ResourceManager.LastSoundError))
            {
                batch.DrawString("Failed to play a sound: " + ResourceManager.LastSoundError, _font, new Vector2(96, 720), Color.Red);
            }

            if (_game?.CurrentPlayer != null)
            {
                batch.DrawString(_game.CurrentPlayer.Name + $"'s turn ({_game.CurrentPlayer.GetColorString()}) ({_game.CurrentPlayer.Wins} Wins)", _font, new Vector2(96, 60), Color.Black);
            }

            foreach (var wabe in _game.GameMap.Wabes.Cast<Wabe>().ToList().Where(x => x.AnimationManager.IsRunning).Select(x => x.AnimationManager))
            {
                wabe.Draw(batch, time);
            }

            if (_game.GameOver && _game.Winner != null)
            {
                var winFont = ResourceManager.GetResource<SpriteFont>("WinnerFont");
                var midX = _clientSize.X / 2;
                var midY = _clientSize.Y / 2;
                var msg1 = _game.Winner.Name + " has won the game!";
                var msg2 = "Press R to restart the game!";
                var sizeMsg1 = winFont.MeasureString(msg1);
                var sizeMsg2 = winFont.MeasureString(msg2);
                var pos1 = new Vector2(midX - (sizeMsg1.X / 2), midY - (sizeMsg1.Y / 2) - 50);
                var pos2 = new Vector2(midX - (sizeMsg2.X / 2), midY - (sizeMsg2.Y / 2) + 20);
                batch.DrawString(msg1, winFont, pos1, Color.Crimson);
                batch.DrawString(msg2, winFont, pos2, Color.Crimson);
            }
            batch.End();
            base.Draw(batch, time);
        }

        private void ResetGame()
        {
            _players = GameSettings.Instance.Players;
            _game = new ChainReactGame(false, false);
            _game.Initialize(_players);
        }

        public void SaveSettings()
        {
            GameSettings.Instance.Save(_gameSettingsFile);
        }

        public override void UnloadContent()
        {
            SaveSettings();
            base.UnloadContent();
        }

        private Texture2D SelectTextureFromField(WabeField field)
        {
            switch (field.Type)
            {
                case WabeFieldType.Unpowered:
                    return ResourceManager.GetResource<Texture2D>("Unpowered");
                case WabeFieldType.Powered:
                    return ResourceManager.GetResource<Texture2D>("Powered");
                case WabeFieldType.Unused:
                case WabeFieldType.Center:
                    return ResourceManager.GetResource<Texture2D>("Unowned");
                default:
                    return null;
            }
        }
    }
}
