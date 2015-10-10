using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpex2D.Framework;

namespace ChainReact.Controls.Base.Interfaces
{
    public interface ICheckableControl
    {
        bool Checked { get; }
        void OnCheckedChanged(GameTime time);
    }
}
