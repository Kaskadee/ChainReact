using System;
using System.Linq;
using ChainReact.Input;
using ChainReact.Input.Devices;
using ChainReact.UI;
using ChainReact.UI.Base;
using ChainReact.UI.Types;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Input;
using Sharpex2D.Framework.Rendering;

namespace ChainReact.Scenes
{
    public class MainMenuScene : Scene
    {
        private readonly Game _game;
        private readonly InputManager _input;
        private SettingsScene _settingsScene;
        private HowToPlayScene _howToPlayScene;

        private Button _continuebutton;
        private Button _howToPlay;
        private Button _settingsButton;
        private Button _exitButton;

        private readonly Coverage _blackCoverage = new Coverage(Color.Black);

        public MainMenuScene(Game game, InputManager input)
        {
            _game = game;
            _input = input;
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            LoadContent();
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
                    clickable.Clicked(gameTime);
                }
                foreach (
                    var checkable in
                        ElementManager.ToArray().Where(control => control is Control && ((Control)control).IsHovered && control is ICheckableControl).Cast<ICheckableControl>())
                {
                    checkable.Checked(gameTime);
                }
                
                ElementManager.SetInputState(_input.Controllers.First(t => t.GetType() == typeof(KeyboardDevice)).State);
               
            }
            foreach (
                    var inputable in
                        ElementManager.ToArray()
                            .Where(
                                control =>
                                    control is Control && ((Control)control).HasFocus && control is IInputControl)
                            .Cast<IInputControl>())
            {
                var state = Keyboard.GetState();
                var keys = state.GetPressedKeys();
                var chr = (keys.Count() < 1) ? (char)0 : (char)keys.FirstOrDefault();
                inputable.KeyPress(chr);
            }
        }

        public override void OnDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _blackCoverage.DrawField(_game, spriteBatch, 128);
            _continuebutton.Draw(spriteBatch, gameTime);
            _settingsButton.Draw(spriteBatch, gameTime);
            _exitButton.Draw(spriteBatch, gameTime);
            _howToPlay.Draw(spriteBatch, gameTime);
        }

        public void LoadContent()
        {
            _settingsScene = new SettingsScene(_game, _input, this);
            _howToPlayScene = new HowToPlayScene(_game, _input, this);
            _game.SceneManager.Add(_settingsScene);
            _continuebutton = new Button(_game, ElementManager, "ButtonMenu", "ButtonMenuHovered", "ButtonFont")
            {
                Bounds = new Rectangle(250, 200, 250, 50),
                Text = "Continue"
            };
            _continuebutton.OnClick += ContinueClick;
            _howToPlay = new Button(_game, ElementManager, "ButtonHowToPlay", "ButtonHowToPlayHovered", "ButtonFont")
            {
                Bounds = new Rectangle(250, 275, 250, 50),
                Text = "How to play"
            };
            _howToPlay.OnClick += HowToPlay;
            _settingsButton = new Button(_game, ElementManager, "ButtonSettings", "ButtonSettingsHovered", "ButtonFont")
            {
                Bounds = new Rectangle(250, 350, 250, 50),
                Text = "Settings"
            };
            _settingsButton.OnClick += SettingsClick;
            _exitButton = new Button(_game, ElementManager, "ButtonExit", "ButtonExitHovered", "ButtonFont")
            {
                Bounds = new Rectangle(250, 425, 250, 50),
                Text = "Exit"
            };
            _exitButton.OnClick += (sender, args) => _game.Exit();
        }

        private void HowToPlay(object sender, EventArgs e)
        {
            var fadeInOut = new FadeInOutTransition(Color.Black, 800f, 600f);
            _game.SceneManager.ChangeWithTransition(_howToPlayScene, fadeInOut);
            OnSceneDeactivated();
        }

        private void SettingsClick(object sender, EventArgs e)
        {
            var fadeInOut = new FadeInOutTransition(Color.Black, 800f, 600f);
            _game.SceneManager.ChangeWithTransition(_settingsScene, fadeInOut);
            OnSceneDeactivated();
        }

        private void ContinueClick(object sender, EventArgs e)
        {
            var fadeInOut = new FadeInOutTransition(Color.Black, 800f, 600f);
            _game.SceneManager.ChangeWithTransition(null, fadeInOut);
            OnSceneDeactivated();
        }

        public override void OnSceneDeactivated()
        {
            foreach (var button in ElementManager.ToArray().Where(control => control.GetType() == typeof(Button)).Cast<Button>())
            {
                button.IsClicked = false;
            }
            base.OnSceneDeactivated();
        }
    }
}
