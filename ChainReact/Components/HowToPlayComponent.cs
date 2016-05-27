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
    public sealed class HowToPlayComponent : DrawableGameComponent
    {
        private readonly Game _game;
        private readonly Coverage _coverage;

        private Label _labelRules;
        private readonly InputManager _input;
        private Button _backButton;

        private FadeInOutTransition _transition;
        private bool _transist;

        private readonly List<Control> _registeredControls = new List<Control>(); 

        public HowToPlayComponent(Game game, InputManager input) : base(game)
        {
            _game = game;
            _input = input;
            _coverage = new Coverage(Color.Black);
            LoadContent(_game.Content);
        }

        public override void Update(GameTime gameTime)
        {
            if (_transist)
            {
                _transition.Update(gameTime);
                return;
            }
            foreach (var control in _registeredControls.ToArray())
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
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_transist)
            {
                _transition.Draw(spriteBatch, gameTime);
                return;
            }
            _coverage.DrawField(_game, spriteBatch);
            foreach (var control in _registeredControls.ToArray())
            {
                control.Draw(spriteBatch, gameTime);
            }
            base.Draw(spriteBatch, gameTime);
        }

        public override void LoadContent(ContentManager content)
        {
            var text = ResourceManager.GetResource<TextFile>("HowToPlay");

            _labelRules = new Label(_game, text.Text, "DefaultFont", Color.White)
            {
                Visible = true,
                Enabled = true,
                Position = new Vector2(25, 25),
                Size = new Rectangle(0, 0, 1, 1)
            };

            _backButton = new Button(_game, "ButtonExit", "ButtonExitHovered", "ButtonFont")
            {
                Text = "Return to main menu",
                Enabled = true,
                Position = new Vector2(25, 700),
                Size = new Rectangle(0, 0, 275, 50)
            };

            _registeredControls.Add(_labelRules);
            _registeredControls.Add(_backButton);

            _backButton.OnClick += ReturnToMainMenu;
        }

        private void ReturnToMainMenu(object sender, EventArgs e)
        {
            _transist = true;
            _transition = new FadeInOutTransition(Color.Black, 800f, 600f);
            _transition.TransitionCompleted += TransitionOnTransitionCompleted;
        }

        private void TransitionOnTransitionCompleted(object sender, EventArgs eventArgs)
        {
            _transist = false;
            _transition.TransitionCompleted -= TransitionOnTransitionCompleted;
            Visible = false;
        }
    }
}
