using System;
using System.Collections.Generic;
using System.Linq;
using ChainReact.Core;
using ChainReact.Input;
using ChainReact.UI;
using ChainReact.UI.Base;
using ChainReact.UI.Types;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Content;
using Sharpex2D.Framework.Rendering;

namespace ChainReact.Components
{
    public class SettingsComponent : DrawableGameComponent
    {
        private readonly Game _game;
        private readonly InputManager _input;

        #region Player-Checkboxes
        private Checkbox _playerOne;
        private Checkbox _playerTwo;
        private Checkbox _playerThree;
        private Checkbox _playerFour;
        #endregion

        private Checkbox _fieldLines;
        private Checkbox _wabeLines;
        private Checkbox _borderLines;

        private Label _header;
        private Label _errorLabel;
        private Button _backButton;

        private readonly List<Control> _registeredControls = new List<Control>();

        private readonly Coverage _blueCoverage = new Coverage(new Color(0, 71, 171, 255));

        public SettingsComponent(Game game, InputManager input) : base(game)
        {
            _game = game;
            _input = input;
            LoadContent(_game.Content);
        }

        public override void Update(GameTime gameTime)
        {
            if (_input.Menu)
            {
                Visible = false;
            }

            foreach (var control in _registeredControls)
            {
                control.Update(gameTime);
            }
           
            if (_input.Clicked)
            {
                foreach (
                    var clickable in
                        _registeredControls.Where(control => control.IsHovered && control.Visible && control.Enabled && control is IClickableControl).Cast<IClickableControl>())
                {
                    clickable.Clicked(gameTime);
                }
                foreach (
                    var checkable in
                        _registeredControls.Where(control => control.IsHovered && control.Visible && control.Enabled && control is ICheckableControl).Cast<ICheckableControl>())
                {
                    checkable.Checked(gameTime);
                }
            }
            if (!ResourceManager.SoundAvailable)
            {
                _errorLabel.Text = "No available audio devices where found. " + Environment.NewLine + "Restart the game if you plugged in an audio device.";
            }
            _errorLabel.Visible = !string.IsNullOrEmpty(_errorLabel.Text);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _blueCoverage.DrawField(_game, spriteBatch);
            foreach (var control in _registeredControls)
            {
                control.Draw(spriteBatch, gameTime);
            }
        }

        public override void LoadContent(ContentManager manager)
        {
            var font = ResourceManager.GetResource<SpriteFont>("ButtonFont");
            ResourceManager.LoadResource<Texture2D>(_game, "Checkbox", "Textures/CheckboxChecked");
            ResourceManager.LoadResource<Texture2D>(_game, "CheckboxUnchecked", "Textures/CheckboxUnchecked");

            #region Player Checkboxes Init
            _playerOne = new Checkbox(_game, "Checkbox", "CheckboxUnchecked", "Enable Player 1 (Green) (Wins: 0)", "ButtonFont")
            {
                Position = new Vector2(25, 60),
                Size = new Rectangle(0, 0, 32, 32),
                Color = Color.White,
                Tag = "Player1",
                IsChecked = true
            };
            _playerTwo = new Checkbox(_game, "Checkbox", "CheckboxUnchecked", "Enable Player 2 (Red) (Wins: 0)", "ButtonFont")
            {
                Position = new Vector2(25, 110),
                Size = new Rectangle(0, 0, 32, 32),
                Color = Color.White,
                Tag = "Player2",
                IsChecked = true
            };
            _playerThree = new Checkbox(_game, "Checkbox", "CheckboxUnchecked", "Enable Player 3 (Blue) (Wins: 0)", "ButtonFont")
            {
                Position = new Vector2(25, 160),
                Size = new Rectangle(0, 0, 32, 32),
                Color = Color.White,
                Enabled = true,
                Tag = "Player3"
            };
            _playerFour = new Checkbox(_game, "Checkbox", "CheckboxUnchecked", "Enable Player 4 (Orange) (Wins: 0)", "ButtonFont")
            {
                Position = new Vector2(25, 210),
                Size = new Rectangle(0, 0, 32, 32),
                Color = Color.White,
                Enabled = true,
                Tag = "Player4"
            };
            #endregion

            _fieldLines = new Checkbox(_game, "Checkbox", "CheckboxUnchecked", "Enable Field Lines", "ButtonFont")
            {
                Position = new Vector2(25, 310),
                Size = new Rectangle(0, 0, 32, 32),
                Color = Color.White,
                Enabled = true,
                Tag = "FieldLines"
            };

            _wabeLines = new Checkbox(_game, "Checkbox", "CheckboxUnchecked", "Enable Wabe Lines", "ButtonFont")
            {
                Position = new Vector2(25, 360),
                Size = new Rectangle(0, 0, 32, 32),
                Color = Color.White,
                Enabled = true,
                Tag = "WabeLines"
            };

            _borderLines = new Checkbox(_game, "Checkbox", "CheckboxUnchecked", "Enable Border Lines", "ButtonFont")
            {
                Position = new Vector2(25, 410),
                Size = new Rectangle(0, 0, 32, 32),
                Color = Color.White,
                Enabled = true,
                Tag = "BorderLines"
            };

            _backButton = new Button(_game, "ButtonExit", "ButtonExitHovered", "ButtonFont")
            {
                Position = new Vector2(25, 700),
                Size = new Rectangle(0, 0, 275, 50),
                Enabled = true,
                Color = Color.White,
                Text = "Return to main menu",
            };
            _backButton.OnClick += (s, e) =>
            {
                //var fadeInOut = new FadeInOutTransition(Color.Black, 800f, 600f);
                //_game.SceneManager.ChangeWithTransition(_mainMenuScene, fadeInOut);
                Visible = false;
            };

            _errorLabel = new Label(_game, "", "DefaultFont", Color.Red)
            {
                Visible = false,
                Enabled = true,
                Position = new Vector2(25, 625),
                Size = new Rectangle(0, 0, 1, 1),
            };

            _playerOne.OnCheckedChanged += PlayersChanged;
            _playerTwo.OnCheckedChanged += PlayersChanged;
            _playerThree.OnCheckedChanged += PlayersChanged;
            _playerFour.OnCheckedChanged += PlayersChanged;

            _fieldLines.OnCheckedChanged += (sender, args) => GameSettings.Instance.FieldLines = _fieldLines.IsChecked;
            _wabeLines.OnCheckedChanged += (sender, args) => GameSettings.Instance.WabeLines = _wabeLines.IsChecked;
            _borderLines.OnCheckedChanged +=
                (sender, args) => GameSettings.Instance.BorderLines = _borderLines.IsChecked;

            var headerTex = "--- Settings -";
            do
            {
                headerTex += "-";
            } while (font.MeasureString(headerTex).X < (_game.Window.ClientSize.X - 60));

            _header = new Label(_game, headerTex, "ButtonFont", Color.White)
            {
                Position = new Vector2(25, 25),
                Size = new Rectangle(0, 0, 1, 1),
                Enabled = true
            };

            // Apply Settings
            _borderLines.IsChecked = GameSettings.Instance.BorderLines;
            _fieldLines.IsChecked = GameSettings.Instance.FieldLines;
            _wabeLines.IsChecked = GameSettings.Instance.WabeLines;

            _playerOne.IsChecked = GameSettings.Instance.AvailablePlayers.First(t => t.Name == _playerOne.Tag).Enabled;
            _playerTwo.IsChecked = GameSettings.Instance.AvailablePlayers.First(t => t.Name == _playerTwo.Tag).Enabled;
            _playerThree.IsChecked = GameSettings.Instance.AvailablePlayers.First(t => t.Name == _playerThree.Tag).Enabled;
            _playerFour.IsChecked = GameSettings.Instance.AvailablePlayers.First(t => t.Name == _playerFour.Tag).Enabled;

            _registeredControls.Add(_playerOne);
            _registeredControls.Add(_playerTwo);
            _registeredControls.Add(_playerThree);
            _registeredControls.Add(_playerFour);

            _registeredControls.Add(_fieldLines);
            _registeredControls.Add(_wabeLines);
            _registeredControls.Add(_borderLines);
            _registeredControls.Add(_backButton);
            _registeredControls.Add(_errorLabel);
            _registeredControls.Add(_header);



            DisableEnabledCheckboxes();

        }

        private void DisableEnabledCheckboxes()
        {
            var checkboxList = new List<Checkbox> { _playerOne, _playerTwo, _playerThree, _playerFour };
            checkboxList.RemoveAll(c => !c.IsChecked);
            if (checkboxList.Count <= 2)
            {
                foreach (var c in checkboxList)
                {
                    c.Enabled = false;
                }
            }
            else
            {
                foreach (var c in checkboxList)
                {
                    c.Enabled = true;
                }
            }
        }

        private void PlayersChanged(object sender, EventArgs e)
        {
            var checkboxList = new List<Checkbox> { _playerOne, _playerTwo, _playerThree, _playerFour };
            foreach (var player in GameSettings.Instance.AvailablePlayers)
            {
                player.Enabled = false;
            }
            checkboxList.RemoveAll(c => !c.IsChecked);
            var playerList = checkboxList.Select(c => c.Tag).ToList();
            if (playerList.Count <= 2)
            {
                foreach (var c in checkboxList)
                {
                    c.Enabled = false;
                }
            }
            else
            {
                foreach (var c in checkboxList)
                {
                    c.Enabled = true;
                }
            }
            var tmp = GameSettings.Instance.AvailablePlayers;
            foreach (var p in playerList.Select(name => tmp.FirstOrDefault(t => t.Name == name)))
            {
                p.Enabled = true;
            }
            GameSettings.Instance.AvailablePlayers = tmp;
        }
    }
}
