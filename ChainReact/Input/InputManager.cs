using System.Collections.Generic;
using System.Linq;
using ChainReact.Core.Utilities;
using ChainReact.Input.Devices;
using Sharpex2D.Framework;
using Sharpex2D.Framework.UI;

namespace ChainReact.Input
{
    public class InputManager :  IUpdateable
    {
        public Vector2 Position { get; private set; }
        public bool Clicked { get; private set; }
        public bool Reset { get; private set; }
        public bool Menu { get; private set; }
        public bool Refresh { get; private set; }

        private readonly MouseDevice _mouseDevice;
        private readonly KeyboardDevice _keyboardDevice;

        public InputManager()
        {
            _mouseDevice = new MouseDevice();
            _keyboardDevice = new KeyboardDevice();
        }

        public void Update(GameTime gameTime)
        {
            // Update mouse
            _mouseDevice.Update(gameTime);
            Position = _mouseDevice.Position;
            Clicked = _mouseDevice.Clicked;

            // Update keyboard
            _keyboardDevice.Update(gameTime);
            Reset = _keyboardDevice.Reset;
            Menu = _keyboardDevice.Menu;
            Refresh = _keyboardDevice.Refresh;
        }
    }
}
