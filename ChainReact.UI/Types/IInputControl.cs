using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChainReact.UI.Types
{
    public interface IInputControl
    {
        bool KeyPressed { get; }
        void KeyPress(char chr);
    }
}
