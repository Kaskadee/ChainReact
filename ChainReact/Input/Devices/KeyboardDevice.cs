using ChainReact.Utilities;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Input;

namespace ChainReact.Input.Devices
{
    public class KeyboardDevice : IInputController
    {
        public int Priority { get; }

        public Vector2 Position { get; private set; }
        public Trigger Clicked { get; }
        public Trigger Reset { get; }
        public Trigger Menu { get; }

        public KeyboardDevice(int priority)
        {
            Priority = priority;
            Clicked = new Trigger(false);
            Reset = new Trigger(false);
            Menu = new Trigger(false);
        }

        public void Update(GameTime time)
        {
            var state = Keyboard.GetState();
            Position = Vector2.Zero;

            Reset.Value = state.IsPressed(Keys.R);
            Menu.Value = state.IsPressed(Keys.Escape);
        }
    }
}
