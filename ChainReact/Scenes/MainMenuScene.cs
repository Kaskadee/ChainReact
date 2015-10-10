using System;
using System.Diagnostics;
using System.Linq;
using ChainReact.Controls;
using ChainReact.Controls.Base;
using ChainReact.Controls.Base.Interfaces;
using ChainReact.Core.Utilities;
using ChainReact.Input;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Content;
using Sharpex2D.Framework.Rendering;
using Sharpex2D.Framework.UI;

namespace ChainReact.Scenes
{
    public class MainMenuScene : Scene
    {
        private readonly Game _game;
        private readonly InputManager _input;

        private ButtonControl _continuebutton;
        private ButtonControl _settingsButton;

        public MainMenuScene(Game game, InputManager input)
        {
            _game = game;
            _input = input;
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            LoadContent(game.Content);
        }

        public override void OnUpdate(GameTime gameTime)
        {
            ElementManager.Update(gameTime);
            if (_input.Clicked.Value)
            {
                foreach (
                    var clickable in
                        ElementManager.ToArray().Where(control => control is Control && ((Control)control).IsHovered && control is IClickableControl).Cast<IClickableControl>())
                {
                    clickable.OnClick(gameTime);
                }
                foreach (
                    var checkable in
                        ElementManager.ToArray().Where(control => control is Control && ((Control)control).IsHovered && control is ICheckableControl).Cast<ICheckableControl>())
                {
                    checkable.OnCheckedChanged(gameTime);
                }
            }
        }

        public override void OnDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            BlackField.DrawField(_game, spriteBatch, 0.5f);
            _continuebutton.Draw(spriteBatch, gameTime);
            _settingsButton.Draw(spriteBatch, gameTime);
        }

        public void LoadContent(ContentManager content)
        {
            _continuebutton = new ContinueButton(_game, ElementManager, "Textures/ButtonMenu", "Textures/ButtonMenuHovered", "Fonts/ButtonFont")
            {
                Bounds = new Rectangle(250, 200, 250, 50),
                Text = "Continue"
            };
            _settingsButton = new SettingsButton(_game, ElementManager, "Textures/ButtonSettings", "Textures/ButtonSettingsHovered", "Fonts/ButtonFont", _input)
            {
                Bounds = new Rectangle(250, 275, 250, 50),
                Text = "Settings"
            };
            ElementManager.AddRootElement(_continuebutton);
            ElementManager.AddRootElement(_settingsButton);
        }

        public override void OnSceneDeactivated()
        {
            foreach (var button in ElementManager.ToArray().Where(control => control.GetType() == typeof(ButtonControl)).Cast<ButtonControl>())
            {
                button.Update(null);
            }
            base.OnSceneDeactivated();
        }
    }
}
