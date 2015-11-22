using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ChainReact.Core;
using ChainReact.Input;
using ChainReact.UI;
using ChainReact.UI.Base;
using ChainReact.UI.Types;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Content;
using Sharpex2D.Framework.Rendering;
using Sharpex2D.Framework.UI;

namespace ChainReact.Scenes
{
    public sealed class HowToPlayScene : Scene
    {
        private readonly Game _game;
        private readonly Scene _mainMenuScene;
        private readonly Coverage _coverage;

        private Label _labelRules;
        private readonly InputManager _input;
        private Button _backButton;

        public HowToPlayScene(Game game, InputManager input, Scene mainMenuScene)
        {
            _game = game;
            _input = input;
            _mainMenuScene = mainMenuScene;
            _coverage = new Coverage(Color.Black);
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
                        ElementManager.ToArray().Where(control => control is Control && ((Control)control).IsHovered && ((Control)control).Visible && ((Control)control).Enabled && control is IClickableControl).Cast<IClickableControl>())
                {
                    clickable.Clicked(gameTime);
                }
                foreach (
                    var checkable in
                        ElementManager.ToArray().Where(control => control is Control && ((Control)control).IsHovered && ((Control)control).Visible && ((Control)control).Enabled && control is ICheckableControl).Cast<ICheckableControl>())
                {
                    checkable.Checked(gameTime);
                }
            }
            base.OnUpdate(gameTime);
        }

        public override void OnDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _coverage.DrawField(_game, spriteBatch);
            foreach (var control in ElementManager.ToArray())
            {
                control.Draw(spriteBatch, gameTime);
            }
            base.OnDraw(spriteBatch, gameTime);
        }

        public void LoadContent()
        {
            var text = ResourceManager.Instance.GetResource<TextFile>("HowToPlay");

            _labelRules = new Label(_game, ElementManager, text.Text, "DefaultFont", Color.White)
            {
                Visible = true,
                Enabled = true,
                Bounds = new Rectangle(25, 25, 1, 1)
            };

            _backButton = new Button(_game, ElementManager, "ButtonExit", "ButtonExitHovered", "ButtonFont")
            {
                Text = "Return to main menu",
                Enabled = true,
                Bounds = new Rectangle(25, 700, 275, 50)
            };

            _backButton.OnClick += ReturnToMainMenu;
        }

        private void ReturnToMainMenu(object sender, EventArgs e)
        {
            var fadeInOut = new FadeInOutTransition(Color.Black, 800f, 600f);
            _game.SceneManager.ChangeWithTransition(_mainMenuScene, fadeInOut);
            OnSceneDeactivated();
        }
    }
}
