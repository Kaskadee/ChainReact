using System.Collections.Generic;
using System.Linq;
using ChainReact.Core.Utilities;
using ChainReact.Input.Devices;
using ChainReact.Utilities;
using Sharpex2D.Framework;
using Sharpex2D.Framework.UI;

namespace ChainReact.Input
{
    public class InputManager : IInputController, IUpdateable
    {
        public List<IInputController> Controllers { get; set; }

        public int Priority { get; }
        public Vector2 Position { get; private set; }
        public Trigger Clicked { get; }
        public Trigger Reset { get; }
        public Trigger Menu { get; }

        public InputState State { get; }

        public InputManager()
        {
            Priority = 0;
            State = null;
            Controllers = new List<IInputController> { new MouseDevice(1), new KeyboardDevice(2) };
            Clicked = new Trigger(false);
            Reset = new Trigger(false);
            Menu = new Trigger(false);
        }

        public void Update(GameTime gameTime)
        {
            Position = Vector2.Zero;
            Clicked.SetToFalse();
            Reset.SetToFalse();
            foreach (var controller in Controllers.OrderByDescending(c => c.Priority))
            {
                controller.Update(gameTime);
                if (Position.Equals(Vector2.Zero))
                {
                    Position = controller.Position;
                }
                Clicked.Set(controller.Clicked.Value);
                Reset.Set(controller.Reset.Value);
                Menu.Set(controller.Menu.Value);
            }
        }
    }
}
