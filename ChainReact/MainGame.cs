using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using ChainReact.Core;
using Microsoft.Xna.Framework.Audio;
using ChainReact.Components;

namespace ChainReact
{
	public class MainGame : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private SingleplayerGame _gameComponent;
		private InputManager _input;
		private FpsCounterComponent _frameCounter;

		private RasterizerState _rasterizer;

		public MainGame ()
		{
			_graphics = new GraphicsDeviceManager (this);
			_graphics.PreferredBackBufferHeight = 768;
			_graphics.PreferredBackBufferWidth = 768;
			_graphics.PreferMultiSampling = true;
			_graphics.GraphicsProfile = GraphicsProfile.HiDef;
			_graphics.SynchronizeWithVerticalRetrace = true;
			_graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;

			_graphics.PreparingDeviceSettings += (sender, e) => {
				e.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount = 16;
			};

			_rasterizer = new RasterizerState () { MultiSampleAntiAlias = true };

			this.Window.AllowUserResizing = false;
			this.Window.Title = "ChainReact v0.1 - Linux (MonoGame)";
			this.IsMouseVisible = true;
			Content.RootDirectory = "Content";
		}
			
		protected override void Initialize ()
		{
			_input = new InputManager (this);
			_frameCounter = new FpsCounterComponent ();
			base.Initialize ();
		}

		protected override void LoadContent ()
		{
			_spriteBatch = new SpriteBatch (GraphicsDevice);

			ResourceManager.LoadTexture (GraphicsDevice, "Background", "Content/Textures/Background.png");
			ResourceManager.LoadTexture (GraphicsDevice, "Unpowered", "Content/Textures/Unpowered.png");
			ResourceManager.LoadTexture (GraphicsDevice, "Powered", "Content/Textures/Powered.png");
			ResourceManager.LoadTexture (GraphicsDevice, "Unowned", "Content/Textures/Default.png");
			ResourceManager.LoadTexture (GraphicsDevice, "Explosion", "Content/Textures/Explosion.png");
			ResourceManager.LoadTexture (GraphicsDevice, "ButtonMenu", "Content/Textures/ButtonMenu.png");
			ResourceManager.LoadTexture (GraphicsDevice, "ButtonMenuHovered", "Content/Textures/ButtonMenuHovered.png");
			ResourceManager.LoadTexture (GraphicsDevice, "ButtonSettings", "Content/Textures/ButtonSettings.png");
			ResourceManager.LoadTexture (GraphicsDevice, "ButtonSettingsHovered", "Content/Textures/ButtonMenuHovered.png");
			ResourceManager.LoadTexture (GraphicsDevice, "ButtonExit", "Content/Textures/ButtonExit.png");
			ResourceManager.LoadTexture (GraphicsDevice, "ButtonExitHovered", "Content/Textures/ButtonExitHovered.png");
			ResourceManager.LoadTexture (GraphicsDevice, "ButtonHowToPlay", "Content/Textures/ButtonHTP.png");
			ResourceManager.LoadTexture (GraphicsDevice, "ButtonHowToPlayHovered", "Content/Textures/ButtonHTPHovered.png");
			ResourceManager.LoadResource<SpriteFont> (Content, "DefaultFont", "Fonts/Default");
			ResourceManager.LoadResource<SpriteFont> (Content, "BoldFont", "Fonts/DefaultBold");
			ResourceManager.LoadResource<SpriteFont> (Content, "ButtonFont", "Fonts/Button");
			ResourceManager.LoadResource<SpriteFont> (Content, "SpecialFont", "Fonts/Special");
			ResourceManager.LoadResource<SoundEffect> (Content, "ExplosionSound", "Sounds/ExplosionSound");

			_gameComponent = new SingleplayerGame (this, _input, new Rectangle(768, 768, 0, 0));
		}

		protected override void Update (GameTime gameTime)
		{
			if (Keyboard.GetState ().IsKeyDown (Keys.Escape))
				Exit ();
			_input.Update (gameTime);
			_gameComponent.Update (gameTime);

			base.Update (gameTime);
		}

		protected override void Draw (GameTime gameTime)
		{
			_frameCounter.Update (gameTime);
			var font = ResourceManager.GetResource<SpriteFont> ("DefaultFont");
			var fpsCount = Math.Round (_frameCounter.CurrentFramesPerSecond, 2);
			var fps = string.Format ("FPS: {0}", fpsCount);

			_graphics.GraphicsDevice.Clear (Color.CornflowerBlue);

			_spriteBatch.Begin (SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend, rasterizerState: _rasterizer);
			_gameComponent.Draw (_spriteBatch, gameTime);
			_spriteBatch.DrawString (font, fps, new Vector2 (1, 1), Color.DimGray);
			_spriteBatch.End ();

			base.Draw (gameTime);
		}
	}
}

