using System.Collections.Generic;
using System.Linq;
using ChainReact.Core;
using ChainReact.Input;
using ChainReact.UI;
using ChainReact.UI.Base;
using ChainReact.UI.Types;
using ChainReact.Utilities;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Content;
using Sharpex2D.Framework.Rendering;

namespace ChainReact.Scenes
{
    public class SettingsScene : Scene
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

        private readonly Coverage _orangeCoverage = new Coverage(new Color(0, 71, 171, 255));

        public SettingsScene(Game game, InputManager input)
        {
            _game = game;
            _input = input;
            LoadContent();
        }

        public override void OnUpdate(GameTime gameTime)
        {
            foreach (var control in ElementManager.ToArray())
            {
                control.Update(gameTime);
            }
           
            if (_input.Clicked.Value)
            {
                foreach (
                    var clickable in
                        ElementManager.ToArray().Where(control => control is Control && ((Control)control).IsHovered && control is IClickableControl).Cast<IClickableControl>())
                {
                    clickable.Clicked(gameTime);
                }
                foreach (
                    var checkable in
                        ElementManager.ToArray().Where(control => control is Control && ((Control)control).IsHovered && control is ICheckableControl).Cast<ICheckableControl>())
                {
                    checkable.Checked(gameTime);
                }
            }
        }

        public override void OnDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _orangeCoverage.DrawField(_game, spriteBatch, 1.0f);
            foreach (var control in ElementManager.ToArray())
            {
                control.Draw(spriteBatch, gameTime);
            }
        }

        public void LoadContent()
        {
            var font = ResourceManager.Instance.GetResource<SpriteFont>("ButtonFont");
            ResourceManager.Instance.LoadResource<Texture2D>(_game, "Checkbox", "Textures/CheckboxChecked");
            ResourceManager.Instance.LoadResource<Texture2D>(_game, "CheckboxUnchecked", "Textures/CheckboxUnchecked");

            #region Player Checkboxes Init
            _playerOne = new Checkbox(_game, ElementManager, "Checkbox", "CheckboxUnchecked", "Enable Player 1 (Green) (Wins: 0)", "ButtonFont")
            {
                Bounds = new Rectangle(25, 60, 32, 32),
                Color = Color.White,
                Enabled = false,
                Tag = "Player1",
                IsChecked = true
            };
            _playerTwo = new Checkbox(_game, ElementManager, "Checkbox", "CheckboxUnchecked", "Enable Player 2 (Red) (Wins: 0)", "ButtonFont")
            {
                Bounds = new Rectangle(25, 110, 32, 32),
                Color = Color.White,
                Enabled = false,
                Tag = "Player2",
                IsChecked = true
            };
            _playerThree = new Checkbox(_game, ElementManager, "Checkbox", "CheckboxUnchecked", "Enable Player 3 (Blue) (Wins: 0)", "ButtonFont")
            {
                Bounds = new Rectangle(25, 160, 32, 32),
                Color = Color.White,
                Enabled = true,
                Tag = "Player3"
            };
            _playerFour = new Checkbox(_game, ElementManager, "Checkbox", "CheckboxUnchecked", "Enable Player 4 (Orange) (Wins: 0)", "ButtonFont")
            {
                Bounds = new Rectangle(25, 210, 32, 32),
                Color = Color.White,
                Enabled = true,
                Tag = "Player4"
            };
            #endregion

            _fieldLines = new Checkbox(_game, ElementManager, "Checkbox", "CheckboxUnchecked", "Enable Field Lines", "ButtonFont")
            {
                Bounds = new Rectangle(25, 310, 32, 32),
                Color = Color.White,
                Enabled = true,
                Tag = "FieldLines"
            };

            _wabeLines = new Checkbox(_game, ElementManager, "Checkbox", "CheckboxUnchecked", "Enable Wabe Lines", "ButtonFont")
            {
                Bounds = new Rectangle(25, 360, 32, 32),
                Color = Color.White,
                Enabled = true,
                Tag = "WabeLines"
            };

            _borderLines = new Checkbox(_game, ElementManager, "Checkbox", "CheckboxUnchecked", "Enable Border Lines", "ButtonFont")
            {
                Bounds = new Rectangle(25, 410, 32, 32),
                Color = Color.White,
                Enabled = true,
                Tag = "BorderLines"
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

            var lblHeader = new Label(_game, ElementManager, headerTex, "ButtonFont", Color.White)
            {
                Bounds = new Rectangle(25, 25, 1, 1),
                Enabled = true
            };

            // Apply Settings
            _borderLines.IsChecked = GameSettings.Instance.BorderLines;
            _fieldLines.IsChecked = GameSettings.Instance.FieldLines;
            _wabeLines.IsChecked = GameSettings.Instance.WabeLines;

            //ElementManager.AddRootElement(_testCheckbox);
        }

        private void PlayersChanged(object sender, System.EventArgs e)
        {
            var checkboxList = new List<Checkbox> { _playerOne, _playerTwo, _playerThree, _playerFour };
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
        }
    }
}
