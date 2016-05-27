using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using ChainReact.Core;
using ChainReact.Core.Networking.Tcp;
using ChainReact.Input;
using ChainReact.UI.Types;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Content;
using Sharpex2D.Framework.Input;
using Sharpex2D.Framework.Rendering;
using Button = ChainReact.UI.Button;
using Control = ChainReact.UI.Base.Control;

namespace ChainReact.Components
{
    public class MultiplayerHostComponent : DrawableGameComponent
    {
        private readonly MainGame _game;
        private readonly InputManager _input;

        private Button _hostGameButton;
        private Button _joinGameButton;
        private readonly Random _random = new Random();

        private readonly List<Control> _registeredControls = new List<Control>();

        private readonly Coverage _blackCoverage = new Coverage(Color.Black);

        public MultiplayerHostComponent(MainGame game, InputManager input) : base(game)
        {
            _game = game;
            _input = input;
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            LoadContent(_game.Content);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var control in _registeredControls)
            {
                control.Update(gameTime);
            }

            if (_input.Clicked)
            {
                foreach (
                    var clickable in
                        _registeredControls.Where(control => control.IsHovered && control is IClickableControl).Cast<IClickableControl>())
                {
                    clickable.Clicked(gameTime);
                }
                foreach (
                    var checkable in
                        _registeredControls.Where(control => control.IsHovered && control is ICheckableControl).Cast<ICheckableControl>())
                {
                    checkable.Checked(gameTime);
                }
            }

            foreach (var inputable in _registeredControls.Where(control => control.HasFocus && control is IInputControl).Cast<IInputControl>())
            {
                var state = Keyboard.GetState();
                var keys = state.GetPressedKeys();
                var chr = (keys.Count() < 1) ? (char)0 : (char)keys.FirstOrDefault();
                inputable.KeyPress(chr);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _blackCoverage.DrawField(_game, spriteBatch, 128);
            _hostGameButton.Draw(spriteBatch, gameTime);
            _joinGameButton.Draw(spriteBatch, gameTime);
        }

        public override void LoadContent(ContentManager content)
        {
            _hostGameButton = new Button(_game, "ButtonMenu", "ButtonMenuHovered", "ButtonFont")
            {
                Position = new Vector2(250, 125),
                Size = new Rectangle(0, 0, 500, 50),
                Text = "Host a local multiplayer game"
            };
            _joinGameButton = new Button(_game, "ButtonMenu", "ButtonMenuHovered", "ButtonFont")
            {
                Position = new Vector2(250, 200),
                Size = new Rectangle(0, 0, 500, 50),
                Text = "Join a local multiplayer game"
            };
            _hostGameButton.OnClick += HostGame;
            _joinGameButton.OnClick += JoinGame;

            _registeredControls.Add(_hostGameButton);
            _registeredControls.Add(_joinGameButton);
        }

        private void JoinGame(object sender, EventArgs e)
        {
            Application.Run(new FrmJoinGame());
            var multiplayerComponent = new MultiplayerComponent(_game, _input, _game.Get<GameWindow>().ClientSize, new IPEndPoint(IPAddress.Loopback, GameSettings.DefaultPortServer));
            _game.RegisterNewComponent(multiplayerComponent);
            Visible = false;
            Enabled = false;
            _game.JoinGame();
        }

        private void HostGame(object sender, EventArgs e)
        {
            
            var server = new ChainReactServer(_game.Identity.Id);
            server.Start();
           var multiplayerComponent = new MultiplayerComponent(_game, _input, _game.Get<GameWindow>().ClientSize, new IPEndPoint(IPAddress.Loopback, GameSettings.DefaultPortServer));
            _game.RegisterNewComponent(multiplayerComponent);
            Visible = false;
            Enabled = false;
            GameSettings.Instance.Address = IPAddress.Loopback;
            _game.JoinGame();
            //_game.JoinGame(_random.Next(25000, 60000));
        }

        public override void UnloadContent()
        {
            foreach (var button in _registeredControls.Where(control => control.GetType() == typeof(Button)).Cast<Button>())
            {
                button.IsClicked = false;
            }
            base.UnloadContent();
        }
    }
}
