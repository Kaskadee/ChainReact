using ChainReact.Utilities;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Input;
using Sharpex2D.Framework.UI;

namespace ChainReact.Input.Devices
{
    public class MouseDevice : IInputController
    {
        public int Priority { get; }
        public Vector2 Position { get; private set; }
        public Trigger Clicked { get; }
        public Trigger Reset { get; }
        public Trigger Menu { get; }

        public InputState State { get; private set; }

        public MouseDevice(int priority)
        {
            Priority = priority;
            Clicked = new Trigger(false);
            Reset = new Trigger(false);
            Menu = new Trigger(false);
        }

        public void Update(GameTime time)
        {
            var state = Mouse.GetState();
            State = new InputState(state);
            Position = state.Position;
            Clicked.Value = state.IsPressed(MouseButtons.Left);
        }
    }
}
