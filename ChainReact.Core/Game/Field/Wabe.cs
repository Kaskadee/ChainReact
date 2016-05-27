using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Xml.Serialization;
using ChainReact.Core.Game.Animations;
using ChainReact.Core.Game.Layout;
using ChainReact.Core.Game.Objects;
using Newtonsoft.Json;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Audio;

namespace ChainReact.Core.Game.Field
{
    public class Wabe
    {
        private readonly float _size;
        private int _id;
        private int _poweredSpheres;
        private readonly ChainReactGame _game;

        [JsonIgnore]
        public ExplosionManager AnimationManager { get; }

        public Player Owner { get; set; }
        public WabeLayout Layout { get; }
        public WabeType Type { get; }

        public int X { get; }
        public int Y { get; }

        public WabeField[] Fields { get; set; }

        public int SphereCount { get; }

        public int PoweredSpheres
        {
            get { return _poweredSpheres; }
            set
            {
                if (SphereCount < value)
                    throw new IndexOutOfRangeException($"Unable to power {value} spheres. There're only {SphereCount} spheres in this wabe.");
                if (SphereCount == value)
                {
                    Explode(null);
                }
                else
                {
                    _poweredSpheres = value;
                }
            }
        }

        private readonly bool _skipAnimation;

        /// <summary>
        /// Initializes a new instance of the <see cref="Wabe"/> class.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="type">The wabe type.</param>
        /// <param name="x">The x position.</param>
        /// <param name="y">The y position.</param>
        /// <param name="size">The size of the wabe (Size, ScalingFactor).</param>
        /// <param name="resourceName">The resource name of the explosion sound</param>
        public Wabe(ChainReactGame game, WabeType type, int x, int y, float size, bool skipExplodeAnimation)
        {
            _skipAnimation = skipExplodeAnimation;
            _game = game;
            Type = type;
            Fields = new WabeField[9];
            X = x;
            Y = y;
            
            Layout = new WabeLayout(this, new Vector2(0, 5));
            switch (Type)
            {
                case WabeType.FourWabe:
                    SphereCount = 4;
                    for (var i = 0; i <= 8; i++)
                    {
                        Fields[i] = new WabeField(Layout.Fields[i], i);
                    }
                    break;
                case WabeType.ThreeWabe:
                    SphereCount = 3;
                    for (var i = 0; i <= 8; i++)
                    {
                        Fields[i] = new WabeField(Layout.Fields[i], i);
                    }
                    break;
                case WabeType.TwoWabe:
                    SphereCount = 2;
                    for (var i = 0; i <= 8; i++)
                    {
                        Fields[i] = new WabeField(Layout.Fields[i], i);
                    }
                    break;
            }
            _size = size;
            var sound = ResourceManager.TryGetResource<SoundEffect>("ExplosionSoundEffect");
            AnimationManager = new ExplosionManager(new List<Explosion>(), 3, sound)
            {
                AbsolutePosition = GetPositionOfWabeCenter(),
                IsRelative = true
            };         
            PopulateExplosionManager();
        }

        private Wabe()
        {
            Fields = new WabeField[9];
            var sound = ResourceManager.TryGetResource<SoundEffect>("ExplosionSoundEffect");
            AnimationManager = new ExplosionManager(new List<Explosion>(), 3, sound)
            {
                AbsolutePosition = GetPositionOfWabeCenter(),
                IsRelative = true
            };
            PopulateExplosionManager();
        }

        private void Explode(GameTime time)
        {
            if (AnimationManager.AllFinished || _skipAnimation)
            {
                if (_game.Queue.GetAllActions().ContainsKey(_id) && !_skipAnimation)
                {
                    _game.Queue.Remove(_id);
                    AnimationManager.Reset();
                    PopulateExplosionManager();
                }
                var nearWabes = _game.GameMap.GetNearWabes(X, Y).OrderBy(w => w.X + w.Y);
                var poweredFields = Fields.ToList().Where(w => w.Type == WabeFieldType.Powered).OrderBy(w => w.Id);
                var results = new Dictionary<Wabe, WabeField>();
                _poweredSpheres = 0;
                var owner = Owner;
                Owner = null;
                for (var i = 0; i < nearWabes.Count(); i++)
                {
                    var nearWabe = nearWabes.ElementAt(i);
                    var poweredField = poweredFields.ElementAt(i);
                    var x = 1;
                    var y = 1;
                    switch (poweredField.Id)
                    {
                        case 1:
                            x = 1;
                            y = 2;
                            break;
                        case 3:
                            x = 2;
                            y = 1;
                            break;
                        case 5:
                            x = 0;
                            y = 1;
                            break;
                        case 7:
                            x = 1;
                            y = 0;
                            break;
                    }
                    var field = nearWabe.ConvertVector2ToWabeField(new Vector2(x, y));
                    if (field.Type != WabeFieldType.Unpowered)
                        field = nearWabe.GetNearestPowerableField(field);
                    results.Add(nearWabe, field);
                }
                foreach (var field in Fields.Where(f => f.Type == WabeFieldType.Powered))
                {
                    field.Type = WabeFieldType.Unpowered;
                }
                foreach (var wabePair in results)
                {
                    var wabe = wabePair.Key;
                    var field = wabePair.Value;
                    wabe.Set(owner, field);
                }
                _game.CheckWin();
            }
            else
            {
                if (!AnimationManager.IsRunning && !_skipAnimation)
                {
                    string error;
                    AnimationManager.Start(out error);
                    ResourceManager.LastSoundError = error;
                    var actionList = new List<Action<GameTime>> { Explode, AnimationManager.Update };
                    _id = _game.Queue.Add(actionList);
                }
            }
        }

        public void Set()
        {
            Set(Owner);
        }

        public void Set(Player owner)
        {
            var field = Fields.First(i => i.Type == WabeFieldType.Unpowered);
            if (field != null) field.Type = WabeFieldType.Powered;
            if (Owner != owner) Owner = owner;
            if (_game.CheckWin())
            {
                return;
            }
            PoweredSpheres++;
        }

        public void Set(Player owner, WabeField field)
        {
            if (field == null)
                return;
            if (field.Type != WabeFieldType.Unpowered)
            {
                field = Fields.First(i => i.Type == WabeFieldType.Unpowered);
            }
            if (field != null) field.Type = WabeFieldType.Powered;
            if (Owner != owner) Owner = owner;
            if (_game.CheckWin())
            {
                return;
            }
            PoweredSpheres++;
        }

        private void PopulateExplosionManager()
        {
            var powerableWabeFields = Fields.Where(f => f.Type == WabeFieldType.Powered || f.Type == WabeFieldType.Unpowered).ToList();
            for (var i = powerableWabeFields.Count - 1; i >= 0; i--)
            {

                var wabeField = powerableWabeFields[i];
                var dict = new Dictionary<int, Vector2>
                {
                    {1, new Vector2(0, -32) },
                    {3, new Vector2(-32, 0) },
                    {5, new Vector2(32, 0) },
                    {7, new Vector2(0, 32) }
                };
                Vector2 vect;
                var success = dict.TryGetValue(wabeField.Id, out vect);
                if (!success) throw new InvalidOperationException("Can't get position of animation from wabefield");
                AnimationManager.CreateNew(new Rectangle(vect.X, vect.Y, 32, 32), true);
            }
        }

        public Vector2 GetPositionOfWabeCenter()
        {
            var thirdsize = _size / 3;
            var x = X;
            var y = Y;
            var absoluteX = (x * _size) + _size;
            var absoluteY = (y * _size) + _size;
            return new Vector2(absoluteX + thirdsize, absoluteY + thirdsize);
        }

        public WabeField ConvertAbsolutePositionToWabeField(Vector2 position, float wabesize)
        {
            if (Math.Abs(position.X) < 1 || Math.Abs(position.Y) < 1) return null;
            var x = (position.X / wabesize) - 1;
            var y = (position.Y / wabesize) - 1;
            if (x < 0 || y < 0) return null;
            var subtractX = Math.Floor(x);
            var subtractY = Math.Floor(y);
            var relativeX = x - subtractX;
            var relativeY = y - subtractY;
            var indexX = (relativeX < 0.4F) ? 0 : (relativeX < 0.7F) ? 1 : 2;
            var indexY = (relativeY < 0.4F) ? 0 : (relativeY < 0.7F) ? 1 : 2;
            var vector = new Vector2(indexX, indexY);
            var field = ConvertVector2ToWabeField(vector);
            return field.Type == WabeFieldType.Unpowered ? field : GetNearestPowerableField(field);
        }

        private WabeField GetNearestPowerableField(WabeField field)
        {
            var lowestValue = Fields.Length;
            WabeField wabeResult = null;
            foreach (var nearField in Fields.ToList())
            {
                if (nearField.Type == WabeFieldType.Unpowered)
                {
                    var val = Math.Abs((nearField.Id - field.Id));
                    var row = field.Id / 3;
                    var rowNear = nearField.Id / 3;
                    if (val < lowestValue || val == lowestValue && rowNear == row)
                    {
                        lowestValue = val;
                        wabeResult = nearField;
                    }
                }
            }
            return wabeResult != null ? Fields[wabeResult.Id] : null;
        }

        public WabeField ConvertVector2ToWabeField(Vector2 position)
        {
            var x = (int)position.X;
            var y = (int)position.Y;
            if (x < 0 || x > 2)
                throw new IndexOutOfRangeException(nameof(x));
            if (y < 0 || y > 2)
                throw new IndexOutOfRangeException(nameof(y));
            switch (y)
            {
                case 0:
                    return Fields[x];
                case 1:
                    return Fields[x + 3];
                case 2:
                    return Fields[x + 6];
                default:
                    return null;
            }
        }

        public Vector2 ConvertWabeFieldToVector2(WabeField field)
        {
            var id = Array.IndexOf(Fields, field);
            var x = 0;
            var y = 0;
            switch (id)
            {
                case 0:
                case 3:
                case 6:
                    x = 0;
                    break;
                case 1:
                case 4:
                case 7:
                    x = 1;
                    break;
                case 2:
                case 5:
                case 8:
                    x = 2;
                    break;
            }
            switch (id)
            {
                case 0:
                case 1:
                case 2:
                    y = 0;
                    break;
                case 3:
                case 4:
                case 5:
                    y = 1;
                    break;
                case 6:
                case 7:
                case 8:
                    y = 2;
                    break;
            }
            return new Vector2(x, y);
        }
    }

    public enum WabeType
    {
        TwoWabe,
        ThreeWabe,
        FourWabe
    }
}
