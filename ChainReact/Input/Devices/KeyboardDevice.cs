using ChainReact.Core.Utilities;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Input;
using Sharpex2D.Framework.UI;

namespace ChainReact.Input.Devices
{
    public class KeyboardDevice
    {
        public Vector2 Position { get; private set; }
        public bool Reset { get; private set; }
        public bool Menu { get; private set; }
        public bool Refresh { get; private set; }

        private bool _previousReset;
        private bool _previousMenu;
        private bool _previousRefresh;

        public InputState State { get; private set; }

        public void Update(GameTime time)
        {
            var state = Keyboard.GetState();
            State = new InputState(state);

            // Escape
            if (state.IsPressed(Keys.Escape) && !_previousMenu)
            {
                _previousMenu = true;
                Menu = true;
            }
            else if (state.IsPressed(Keys.Escape) && _previousMenu)
            {
                Menu = false;
            }
            else if (!state.IsPressed(Keys.Escape) && _previousMenu)
            {
                Menu = false;
                _previousMenu = false;
            }
            else
            {
                Menu = false;
            }

            // Reset
            if (state.IsPressed(Keys.R) && !_previousReset)
            {
                _previousReset = true;
                Reset = true;
            }
            else if (state.IsPressed(Keys.R) && _previousReset)
            {
                Reset = false;
            }
            else if (!state.IsPressed(Keys.R) && _previousReset)
            {
                Reset = false;
                _previousReset = false;
            }
            else
            {
                Reset = false;
            }

            // Refresh
            if (state.IsPressed(Keys.F5) && !_previousRefresh)
            {
                _previousRefresh = true;
                Refresh = true;
            }
            else if (state.IsPressed(Keys.F5) && _previousRefresh)
            {
                Refresh = false;
            }
            else if (!state.IsPressed(Keys.F5) && _previousRefresh)
            {
                Refresh = false;
                _previousRefresh = false;
            }
            else
            {
                Refresh = false;
            }
        }
    }
}
