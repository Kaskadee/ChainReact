using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using ChainReact.Components;
using ChainReact.Core;
using ChainReact.Core.Client;
using ChainReact.Input;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Audio;
using Sharpex2D.Framework.Audio.WaveOut;
using Sharpex2D.Framework.Content;
using Sharpex2D.Framework.Rendering;
using Sharpex2D.Framework.Rendering.OpenGL;

namespace ChainReact
{
    public class MainGame : Game
    {
        #region Components
        private InputManager _input;
        #endregion

        #region Game Components

        public List<GameComponent> GameComponents { get; } = new List<GameComponent>();

        private SingleplayerComponent _singleplayerGame;
        private MainMenuComponent _mainMenuComponent;
        #endregion

        public ClientIdentity Identity => _singleplayerGame?.LocalIdentity;
        public int DrawPriority { get; private set; }

        public override void Setup(LaunchParameters launchParameters)
        {
            if (launchParameters.KeyAvailable("Debugger") && launchParameters["Debugger"] == "Enabled")
            {
                Debugger.Launch();
            }
            GraphicsManager = new GLGraphicsManager
            {
                PreferredBackBufferHeight = 720,
                PreferredBackBufferWidth = 720
            };
            var window = Get<GameWindow>();
            window.Title = "ChainReact - Development Build";
            Content.RootPath = "Content";
            SoundManager = new WaveOutSoundManager();
            ResourceManager.SoundManager = SoundManager;
        }

        public override void Initialize()
        {
            _input = new InputManager();
            DrawPriority = 3;
            base.Initialize();
        }

        public override void LoadContent()
        {
            ResourceManager.LoadResource<Texture2D>(this, "ExplosionTexture", "Textures/Explosion");
            ResourceManager.LoadResource<Texture2D>(this, "Background", "Textures/Background");
            ResourceManager.LoadResource<Texture2D>(this, "Unpowered", "Textures/Unpowered");
            ResourceManager.LoadResource<Texture2D>(this, "Powered", "Textures/Powered");
            ResourceManager.LoadResource<Texture2D>(this, "Unowned", "Textures/Default");
            ResourceManager.LoadResource<Texture2D>(this, "ButtonMenu", "Textures/ButtonMenu");
            ResourceManager.LoadResource<Texture2D>(this, "ButtonMenuHovered", "Textures/ButtonMenuHovered");
            ResourceManager.LoadResource<Texture2D>(this, "ButtonSettings", "Textures/ButtonSettings");
            ResourceManager.LoadResource<Texture2D>(this, "ButtonSettingsHovered", "Textures/ButtonMenuHovered");
            ResourceManager.LoadResource<SpriteFont>(this, "ButtonFont", "Fonts/ButtonFont");
            ResourceManager.LoadResource<SpriteFont>(this, "DefaultFont", "Fonts/Default");
            ResourceManager.LoadResource<Texture2D>(this, "ButtonExit", "Textures/ButtonExit");
            ResourceManager.LoadResource<Texture2D>(this, "ButtonExitHovered", "Textures/ButtonExitHovered");
            ResourceManager.LoadResource<Texture2D>(this, "ButtonHowToPlay", "Textures/ButtonHTP");
            ResourceManager.LoadResource<Texture2D>(this, "ButtonHowToPlayHovered", "Textures/ButtonHTPHovered");
            ResourceManager.LoadResource<TextFile>(this, "HowToPlay", "Others/howtoplay");
            ResourceManager.LoadResource<SpriteFont>(this, "WinnerFont", "Fonts/WinnerFont");
            ResourceManager.LoadResource<SpriteFont>(this, "BoldFont", "Fonts/Bold");
            ResourceManager.LoadResource<Sound>(this, "ExplosionSound", "Sounds/ExplosionSound");

            _singleplayerGame = new SingleplayerComponent(this, _input, Get<GameWindow>().ClientSize) { DrawOrder = 1, UpdateOrder = 1, Visible = true, Enabled = true};
            _mainMenuComponent = new MainMenuComponent(this, _input) { DrawOrder = 2, UpdateOrder = 2, Visible = true, Enabled = true };

            GameComponents.Add(_singleplayerGame);
            GameComponents.Add(_mainMenuComponent);
            
            var controlFromHandle = Control.FromHandle(GameWindow.Default.Handle);
            var form = controlFromHandle as FrmLoading;
            form?.DestroyControls();
        }

        public void RegisterNewComponent(DrawableGameComponent component)
        {
            GameComponents.Add(component);
        }

        public void JoinGame()
        {
            foreach (var component in GameComponents.Cast<DrawableGameComponent>())
            {
                component.Enabled = false;
                component.Visible = false;
            }

            var multiplayer = (DrawableGameComponent)GameComponents.First(t => t.GetType() == typeof (MultiplayerComponent));
            multiplayer.Enabled = true;
            multiplayer.Visible = true;
        }

        public override void Update(GameTime time)
        {
            _input.Update(time);

            var updateComponent = GameComponents.Where(t => t.Enabled).OrderByDescending(t => t.UpdateOrder).First();
            updateComponent.Update(time);
        }

        public override void Draw(SpriteBatch batch, GameTime time)
        {
            foreach (var component in GameComponents.Cast<DrawableGameComponent>().OrderBy(t => t.UpdateOrder).Where(t => t.Visible))
            {
                component.Draw(batch, time);
            }
            SceneManager.Draw(batch, time);
            batch.End();
        }
    }
}
