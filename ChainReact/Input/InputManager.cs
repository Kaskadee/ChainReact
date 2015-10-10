using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChainReact.Input.Devices;
using ChainReact.Utilities;
using Sharpex2D.Framework;

namespace ChainReact.Input
{
    public class InputManager : IInputController, IUpdateable
    {
        private Game _game;

        public List<IInputController> Controllers { get; set; }

        public int Priority { get; }
        public Vector2 Position { get; private set; }
        public Trigger Clicked { get; }
        public Trigger Reset { get; }
        public Trigger Menu { get; }

        public InputManager(Game game)
        {
            Priority = 0;
            _game = game;
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
