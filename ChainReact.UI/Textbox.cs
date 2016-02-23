using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ChainReact.Core;
using ChainReact.Core.Utilities;
using ChainReact.UI.Base;
using ChainReact.UI.Types;
using Microsoft.SqlServer.Server;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Input;
using Sharpex2D.Framework.Rendering;
using Sharpex2D.Framework.UI;

namespace ChainReact.UI
{
    public class Textbox : Control, IInputControl
    {
        private Trigger _keyPressedTrigger = new Trigger(false);

        public bool KeyPressed { get; private set; }
        public string PreviousText { get; private set; }
        public Func<string, string> DefaultHandle { get; }

        public Textbox(Game game, ElementManager elementManager, Color textColor, string font, string text = "") : base(game, elementManager)
        {
            Color = textColor;
            Font = ResourceManager.Instance.GetResource<SpriteFont>(font);
            Text = text;
            PreviousText = text;
            CanGetFocus = true;
            DefaultHandle = DefaultInputHandle;
        }

        public override void Update(GameTime gameTime)
        {
            UpdateTextFromKeyboardInput(DefaultHandle);
            var state = Keyboard.GetState();
            var keys = state.GetPressedKeys();
            if (keys.Count() < 1)
            {
                _keyPressedTrigger.SetToFalse();
                KeyPressed = false;
            }
            if (!Enabled)
            {
                SetAppearance(Theme.Disabled);
            }
            else if (HasFocus)
            {
                SetAppearance(Theme.Focused);
            }
            else
            {
                SetAppearance(Theme.Normal);
            }
            base.Update(gameTime);
        }

        private void SetAppearance(Theme theme)
        {

        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!Visible) return;
            spriteBatch.DrawString(Text, Font, Position, Color);
            base.Draw(spriteBatch, gameTime);
        }

        public void UpdateTextFromKeyboardInput(Func<string, string> handleInput)
        {
            if (Enabled && HasFocus && KeyPressed && _keyPressedTrigger.Value)
            {
                Text = handleInput(Text);
            }
            else
            {
                KeyPressed = false;
            }
        }

        private string DefaultInputHandle(string arg)
        {
            var newValue = arg;
            var state = Keyboard.GetState();
            var keys = state.GetPressedKeys();
            newValue = KeysExtension.GetStringValue(newValue, keys);
            return newValue;
        }

        private bool _flag;

        public void KeyPress(char chr)
        {
            if (_flag && chr != 0)
            {
                KeyPressed = false;
                _keyPressedTrigger.SetToFalse();
                return;
            }
            if (_flag && chr == 0)
            {
                KeyPressed = false;
                _keyPressedTrigger.SetToFalse();
                _flag = false;
                return;
            }
            if (!_flag && chr != 0)
            {
                _keyPressedTrigger.Set(chr != 0);
                KeyPressed = _keyPressedTrigger.GetValue();
                _flag = true;
            }
            if (!_flag && chr == 0)
            {
                KeyPressed = false;
                _keyPressedTrigger.SetToFalse();
            }
        }
    }

    public enum Theme
    {
        Disabled,
        Focused,
        Normal
    }
}
