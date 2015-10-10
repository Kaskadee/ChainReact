using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ChainReact.Controls;
using ChainReact.Controls.Base;
using ChainReact.Core.Game;
using ChainReact.Core.Game.Animations;
using ChainReact.Core.Game.Field;
using ChainReact.Core.Game.Objects;
using ChainReact.Core.Utilities;
using ChainReact.Input;
using ChainReact.Scenes;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Audio.WaveOut;
using Sharpex2D.Framework.Rendering;
using Sharpex2D.Framework.Rendering.OpenGL;

namespace ChainReact
{
    public class MainGame : Game
    {
        #region Constants
        public const int WabeSize = 64;
        public const float ScalingFactor = 1.5F;
        #endregion

        #region Components
        private ChainReactGame _game;
        private InputManager _input;
        #endregion

        #region Textures
        private Texture2D _background;

        private Texture2D _unusedFields;
        private Texture2D _spheresUnpowered;
        private Texture2D _spheresPowered;
        private Texture2D _unowned;

        private Texture2D _playerTemplate;

        private Texture2D _gameBorder;
        private Texture2D _wabeBorder;

        private Texture2D[] _splittedExplosion;
        private Texture2D _explosion;
        private AnimatedSpriteSheet _explosionAnimation;
        private MultiAnimation _testAnimation;
        #endregion

        #region Fonts
        private SpriteFont _default;
        #endregion

        #region Scenes

        private MainMenuScene _mainMenuScene;
        #endregion

        private string _lastMessage;

        public override void Setup(LaunchParameters launchParameters)
        {
            GraphicsManager = new OpenGLGraphicsManager
            {
                PreferredBackBufferHeight = 768,
                PreferredBackBufferWidth = 768
            };
            var window = Get<GameWindow>();
            window.Title = "ChainReact - Development Build";
            Content.RootPath = "Content";
            SoundManager = new WaveOutSoundManager();
        }

        public override void LoadContent()
        {
            _background = Content.Load<Texture2D>("Textures/Background");
            _unusedFields = Content.Load<Texture2D>("Textures/Unused");
            _spheresUnpowered = Content.Load<Texture2D>("Textures/Unpowered");
            _spheresPowered = Content.Load<Texture2D>("Textures/Powered");
            _unowned = Content.Load<Texture2D>("Textures/Default");

            _explosion = Content.Load<Texture2D>("Textures/Explosion"); 
            AssignExplosionAnimation();
            _playerTemplate = Content.Load<Texture2D>("Textures/Default");

            // TODO Add player configuration
           
            var players = new List<Player> { new Player(1, "Player1", Color.Green), new Player(2, "Player2", Color.Red) };
            var twoExplosion = new MultiAnimation(this,
                new List<Animation>
                {
                    new Animation(_explosionAnimation, new Rectangle(32, 0, 32, 32)),
                    new Animation(_explosionAnimation, new Rectangle(0, 32, 32, 32))
                }, 3);
            var threeExplosion = new MultiAnimation(this,
                new List<Animation>
                {
                    new Animation(_explosionAnimation, new Rectangle(32, 0, 32, 32)),
                    new Animation(_explosionAnimation, new Rectangle(0, 32, 32, 32)),
                    new Animation(_explosionAnimation, new Rectangle(-32, 0, 32, 32))
                }, 3);
            var fourExplosion = new MultiAnimation(this,
               new List<Animation>
               {
                    new Animation(_explosionAnimation, new Rectangle(32, 0, 32, 32)),
                    new Animation(_explosionAnimation, new Rectangle(0, 32, 32, 32)),
                    new Animation(_explosionAnimation, new Rectangle(-32, 0, 32, 32)),
                    new Animation(_explosionAnimation, new Rectangle(0, -32, 32, 32))
               }, 3);
            var multi = new MultiAnimation(this,
                new List<Animation> {new Animation(_explosionAnimation, new Rectangle(32, 0, 32, 32))}, 3);
            _testAnimation = multi;
            _game = new ChainReactGame(this, new List<MultiAnimation>() { twoExplosion, threeExplosion, fourExplosion }, players, new Vector2(WabeSize, ScalingFactor));

            var fullWabeSizeX = WabeSize * ScalingFactor * _game.Wabes.GetLength(0);
            var fullWabeSizeY = WabeSize * ScalingFactor * _game.Wabes.GetLength(1);
            _wabeBorder = CreateBorderFromColor(64, 64, 1, Color.Olive);
            _gameBorder = CreateBorderFromColor((int)fullWabeSizeX, (int)fullWabeSizeY, 3, Color.White);

            _input = new InputManager(this);
            _default = Content.Load<SpriteFont>("Fonts/Default.xcf");

            _mainMenuScene = new MainMenuScene(this, _input);
            SceneManager.Add(_mainMenuScene);
            SceneManager.ActiveScene = _mainMenuScene;
        }

        private void AssignExplosionAnimation()
        {
            _explosionAnimation = new AnimatedSpriteSheet(_explosion);
            for (var i = 0; i < 12; i++)
            {
                var x = 134 * i;
                var kf = new Keyframe(new Rectangle(x, 0, 134, 134), 100f);
                _explosionAnimation.Add(kf);
            }
            _explosionAnimation.Rectangle = new Rectangle(0, 0, 134, 134);
            _explosionAnimation.AutoUpdate = true;
        }

        public override void Update(GameTime time)
        {
            if (_game.Queue.IsActionQueued)
            {
                var actions = _game.Queue.GetAllActions();
                foreach (var act in actions.Select(x => x.Value).SelectMany(actPair => actPair))
                {
                    act?.Invoke(time);
                }
                return;
            }
            _input.Update(time);
            var menu = _input.Menu.Value;
            if (menu && SceneManager.ActiveScene != _mainMenuScene && !_mainMenuScene.ElementManager.Any(t => t.GetType() == typeof(ButtonControl) && ((ButtonControl)t).Clicked))
            {
                SceneManager.ActiveScene = _mainMenuScene;
                return;
            }
            if (SceneManager.ActiveScene != null)
            {
                SceneManager.ActiveScene.OnUpdate(time);
                return;
            }

            if (!_game.GameOver)
            {
                if (_input.Clicked)
                {
                    var wabe = _game.ConvertAbsolutePositionToWabe(_input.Position, WabeSize * ScalingFactor);
                    var field = wabe?.ConvertAbsolutePositionToWabeField(_input.Position, WabeSize * ScalingFactor);
                    if (field == null) return;
                    string error;
                    _game.Set(_game.CurrentPlayer.Id, wabe, field, out error);
                    if (error != null)
                    {
                        _lastMessage = error;
                    }
                }
            }
            if (_game.GameOver && _input != null && _input.Reset.Value)
            {
                _input = null;
                LoadContent();
            }
        }

        public override void Draw(SpriteBatch batch, GameTime time)
        {
            batch.Begin();
            for (var x = 0; x < 11; x++)
            {
                for (var y = 0; y < 8; y++)
                {
                    var tileX = x * WabeSize * ScalingFactor;
                    var tileY = y * WabeSize * ScalingFactor;
                    batch.DrawTexture(_background, new Rectangle(tileX, tileY, WabeSize * ScalingFactor, WabeSize * ScalingFactor));
                }
            }
            const float cut = ((float)WabeSize / 3) * ScalingFactor;
            var fullWabeSizeX = WabeSize * ScalingFactor * _game.Wabes.GetLength(0);
            var fullWabeSizeY = WabeSize * ScalingFactor * _game.Wabes.GetLength(1);
            foreach (var wabe in _game.Wabes)
            {
                var wabeX = (wabe.X + 1) * WabeSize * ScalingFactor;
                var wabeY = (wabe.Y + 1) * WabeSize * ScalingFactor;

                for (var x = 0; x <= 2; x++)
                {
                    for (var y = 0; y <= 2; y++)
                    {
                        var field = wabe.ConvertVector2ToWabeField(new Vector2(x, y));
                        var texture = SelectTextureFromField(wabe.Owner, field);
                        var mutltiplicatorX = cut * x;
                        var mutltiplicatorY = cut * y;
                        if (field.Type == WabeFieldType.Center)
                        {
                            var opacity = (wabe.Owner != null) ? 0.8F : 1.0F;
                            var color = wabe.Owner?.Color ?? Color.White;
                            batch.DrawTexture(texture,
                                     new Rectangle((wabeX) + mutltiplicatorX, (wabeY) + mutltiplicatorY, cut, cut), color, opacity);
                        }
                        else
                        {
                            batch.DrawTexture(texture,
                                new Rectangle((wabeX) + mutltiplicatorX, (wabeY) + mutltiplicatorY, cut, cut));
                        }

                        //batch.DrawTexture(_fieldBorder, new Rectangle((wabeX) + mutltiplicatorX, (wabeY) + mutltiplicatorY, cut, cut));
                    }
                }


                batch.DrawTexture(_wabeBorder, new Rectangle(wabeX, wabeY, WabeSize * ScalingFactor, WabeSize * ScalingFactor));
            }
            batch.DrawTexture(_gameBorder, new Rectangle(WabeSize * ScalingFactor, WabeSize * ScalingFactor, fullWabeSizeX, fullWabeSizeY));
            if (!string.IsNullOrEmpty(_lastMessage))
            {
                batch.DrawString(!string.IsNullOrEmpty(_game.Message) ? _game.Message : _lastMessage, _default,
                    new Vector2(0, 700), Color.Black);
            }
            else
            {
                if (!string.IsNullOrEmpty(_game.Message))
                {
                    batch.DrawString(_game.Message, _default, new Vector2(0, 700), Color.Black);
                }
            }
            if (_game?.CurrentPlayer != null)
            {
                batch.DrawString(_game.CurrentPlayer.Name + "'s turn", _default, new Vector2(96, 50), Color.Black);
            }
            if (SceneManager.ActiveScene == null)
            {
                foreach (var wabe in _game.Wabes.Cast<Wabe>().ToList().Where(x => x.Animation.IsRunning).Select(x => x.Animation))
                {
                    wabe.Draw(batch, time);
                }
            }
            SceneManager.ActiveScene?.OnDraw(batch, time);
            batch.End();
        }

        private Texture2D SelectTextureFromField(Player player, WabeField field)
        {
            switch (field.Type)
            {
                case WabeFieldType.Unused:
                    return _unusedFields;
                case WabeFieldType.Unpowered:
                    return _spheresUnpowered;
                case WabeFieldType.Powered:
                    return _spheresPowered;
                case WabeFieldType.Center:
                    return player == null ? _unowned : _playerTemplate;
                default:
                    return null;
            }
        }

        private Texture2D CreateBorderFromColor(int width, int heigth, int penLength, Color color)
        {
            var tex = new Texture2D(width, heigth);
            tex.Lock();
            for (var x = 0; x <= width - 1; x++)
            {
                for(var y = 0; y <= heigth - 1; y++)
                    if ((penLength - x) > 0 || (penLength + x) > width - 1)
                        tex[x, y] = color;
            }

            for (var y = 0; y <= heigth - 1; y++)
            {
                for (var x = 0; x <= width - 1; x++)
                    if ((penLength - y) > 0 || (penLength + y) > heigth - 1)
                        tex[x, y] = color;
            }

            tex.Unlock();
            return tex;
        }
    }
}
