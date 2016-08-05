using System;
using Microsoft.Xna.Framework;
using System.IO;
using ChainReact.Core.Client;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using ChainReact.Core.Game.Objects;
using ChainReact.Extensions;
using ChainReact.Core.Game;
using ChainReact.Core;
using ChainReact.Utilities;
using ChainReact.Core.Game.Field;
using System.Linq;

namespace ChainReact.Components
{
	public class SingleplayerGame : DrawableGameComponent
	{
		public ClientIdentity LocalIdentity { get; private set; }

		#region Game Resources
		private Texture2D _background;
		private Texture2D _gameColorBack; 
		private Texture2D _gameBorder;
		private Texture2D _wabeBorder;
		private Texture2D _fieldBorder;

		private SpriteFont _font;

		private IEnumerable<Player> _players;
		#endregion

		private readonly FileInfo _gameSettings = new FileInfo("settings.dat");
		private readonly InputManager _input;

		private string _message;
		private Rectangle _gameSize;

		private ChainReactGame _game;

		private MainGame _mainGame;

		public SingleplayerGame (Game game, InputManager input, Rectangle gameSize) : base (game)
		{
			_mainGame = (MainGame)game;
			_input = input;
			_gameSize = gameSize;
			LocalIdentity = IdentityExtensions.LoadIdentity ("identity.dat");
			_players = GameSettings.Instance.Players.EnabledPlayers; 
			LoadContent ();
		}

		protected override void LoadContent ()
		{
			_game = new ChainReactGame (false, true);
			_gameColorBack = TextureUtilities.ConvertFromColor (_mainGame.GraphicsDevice, Color.DimGray);

			var fullWabeSizeX = ChainReactGame.WabeSize * _game.GameMap.Wabes.GetLength(0);
			var fullWabeSizeY = ChainReactGame.WabeSize * _game.GameMap.Wabes.GetLength(1);
			var fieldSize = (int)((ChainReactGame.WabeSize) / 3);
			_wabeBorder = TextureUtilities.CreateBorderFromColor(_mainGame.GraphicsDevice, 64, 64, 1, Color.Olive);
			_gameBorder = TextureUtilities.CreateBorderFromColor(_mainGame.GraphicsDevice, (int)fullWabeSizeX, (int)fullWabeSizeY, 3, Color.White);
			_fieldBorder = TextureUtilities.CreateBorderFromColor(_mainGame.GraphicsDevice, fieldSize, fieldSize, 1, Color.Black);

			_background = ResourceManager.GetResource<Texture2D>("Background");
			_font = ResourceManager.GetResource<SpriteFont>("DefaultFont");

			_game.Initialize(_players);

			base.LoadContent ();
		}

		public override void Update (GameTime gameTime)
		{
			if (_game != null && _game.Queue.IsActionQueued)
			{
				var actions = _game.Queue.GetAllActions();
				foreach (var act in actions.Select(x => x.Value).SelectMany(actPair => actPair))
				{
					act?.Invoke(gameTime);
				}
				return;
			}
			if (_game != null && !_game.GameOver)
			{
				if (_input.Clicked)
				{
					var wabe = _game.GameMap.AbsoluteToWabe(_input.MousePosition.ToVector2());
					var field = wabe?.ConvertAbsolutePositionToWabeField(_input.MousePosition.ToVector2(), ChainReactGame.WabeSize);
					if (field == null) return;
					string error;
					_game.Set(_game.CurrentPlayer.Id, wabe, field, out error);
					if (error != null)
					{
						_message = error;
					}
				}
			}
			else if (_input != null && _input.Reset)
			{
				ResetGame();
			}
			base.Update (gameTime);
		}

		public void Draw(SpriteBatch batch, GameTime time)
		{
			var wabeSize = (int)ChainReactGame.WabeSize;
			// Draw wood background
			for (var x = 0; x < 11; x++)
			{
				for (var y = 0; y < 8; y++)
				{
					var tileX = x * wabeSize;
					var tileY = y * wabeSize;
					batch.Draw(_background, new Rectangle(tileX, tileY, wabeSize, wabeSize), Color.White);
				}
			}

			// Draw gray game field
			for (var x = 1; x < 7; x++)
			{
				for (var y = 1; y < 7; y++)
				{
					var tileX = x * wabeSize;
					var tileY = y * wabeSize;
					batch.Draw(_gameColorBack, new Rectangle(tileX, tileY, wabeSize, wabeSize), Color.White);
				}
			}

			// Draw wabes
		 	int cut = (wabeSize / 3);
			var fullWabeSizeX = wabeSize * _game.GameMap.Wabes.GetLength(0);
			var fullWabeSizeY = wabeSize * _game.GameMap.Wabes.GetLength(1);
			foreach (var wabe in _game.GameMap.Wabes)
			{
				var wabeX = (wabe.X + 1) * wabeSize;
				var wabeY = (wabe.Y + 1) * wabeSize;

				for (var x = 0; x <= 2; x++)
				{
					for (var y = 0; y <= 2; y++)
					{
						var field = wabe.ConvertVector2ToWabeField(new Vector2(x, y));
						var texture = SelectTextureFromField(field);
						var mutltiplicatorX = (int)cut * x;
						var mutltiplicatorY = (int)cut * y;
						if (field.Type == WabeFieldType.Center)
						{
							var color = wabe.Owner?.Color ?? Color.White;
							batch.Draw(texture, new Rectangle((wabeX) + mutltiplicatorX, (wabeY) + mutltiplicatorY, cut, cut), color);
						}
						else if (field.Type == WabeFieldType.Unused)
						{
							var color = wabe.Owner?.Color ?? Color.LightGray;
							batch.Draw(texture, new Rectangle((wabeX) + mutltiplicatorX, (wabeY) + mutltiplicatorY, cut, cut), color);
						}
						else
						{
							batch.Draw(texture,
								new Rectangle((wabeX) + mutltiplicatorX, (wabeY) + mutltiplicatorY, cut, cut), Color.White);
						}
						var posX = mutltiplicatorX + wabeX;
						var posY = mutltiplicatorY + wabeY;
						// Draw field border
						if (GameSettings.Instance.FieldLines)
							batch.Draw(_fieldBorder, new Rectangle(posX, posY, cut, cut), Color.White);
					}
				}
				// Draw wabe border
				if (GameSettings.Instance.WabeLines)
					batch.Draw(_wabeBorder, new Rectangle(wabeX, wabeY, wabeSize, wabeSize), Color.White);
			}

			// Draw game border
			if (GameSettings.Instance.BorderLines)
				batch.Draw(_gameBorder, new Rectangle(wabeSize, wabeSize, fullWabeSizeX, fullWabeSizeY), Color.White);

			// Draw messages
			if (!string.IsNullOrEmpty(_message))
			{
				var text = !string.IsNullOrEmpty (_game.Message) ? _game.Message : _message;
				batch.DrawString (_font, text, new Vector2 (96, 680), Color.Black);
			}
			else
			{
				if (!string.IsNullOrEmpty(_game.Message))
				{
					batch.DrawString(_font, _game.Message, new Vector2(96, 680), Color.Black);
				}
			}

			if (_game?.CurrentPlayer != null)
			{
				batch.DrawString(_font, _game.CurrentPlayer.Name + $"'s turn ({_game.CurrentPlayer.GetColorString()}) ({_game.CurrentPlayer.Wins} Wins)", new Vector2(96, 60), Color.Black);
			}

			foreach (var wabe in _game.GameMap.Wabes.Cast<Wabe>().ToList().Where(x => x.AnimationManager.IsRunning).Select(x => x.AnimationManager))
			{
				wabe.Draw(batch, time);
			}

			if (_game.GameOver && _game.Winner != null)
			{
				var winFont = ResourceManager.GetResource<SpriteFont>("SpecialFont");
				var midX = _gameSize.X / 2;
				var midY = _gameSize.Y / 2;
				var msg1 = _game.Winner.Name + " has won the game!";
				var msg2 = "Press R to restart the game!";
				var sizeMsg1 = winFont.MeasureString(msg1);
				var sizeMsg2 = winFont.MeasureString(msg2);
				var pos1 = new Vector2(midX - (sizeMsg1.X / 2), midY - (sizeMsg1.Y / 2) - 50);
				var pos2 = new Vector2(midX - (sizeMsg2.X / 2), midY - (sizeMsg2.Y / 2) + 20);
				batch.DrawString(winFont, msg1, pos1, Color.Crimson);
				batch.DrawString(winFont, msg2, pos2, Color.Crimson);
			 }
		}

		private void ResetGame()
		{
			_players = GameSettings.Instance.Players.EnabledPlayers;
			_game = new ChainReactGame(false, false);
			_game.Initialize(_players);
		}

		public void SaveSettings()
		{
			GameSettings.Instance.Save(_gameSettings);
		}

		protected override void UnloadContent()
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

