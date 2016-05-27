using ChainReact.Core.Utilities;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Input;
using Sharpex2D.Framework.UI;

namespace ChainReact.Input.Devices
{
    public class MouseDevice
    {
        public Vector2 Position { get; private set; }
        public bool Clicked { get; private set; }

        private bool _previousClicked;

        public InputState State { get; private set; }

        public void Update(GameTime time)
        {
            var state = Mouse.GetState();
            State = new InputState(state);
            Position = state.Position;
            if (state.IsPressed(MouseButtons.Left) && !_previousClicked)
            {
                _previousClicked = true;
                Clicked = true;
            }
            else if (state.IsPressed(MouseButtons.Left) && _previousClicked)
            {
                Clicked = false;
            }
            else if (!state.IsPressed(MouseButtons.Left) && _previousClicked)
            {
                Clicked = false;
                _previousClicked = false;
            }
            else
            {
                Clicked = false;
            }
        }
    }
}
