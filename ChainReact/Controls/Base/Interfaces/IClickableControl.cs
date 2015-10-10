using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Sharpex2D.Framework;

namespace ChainReact.Controls.Base.Interfaces
{
    public interface IClickableControl
    {
        bool Clicked { get; }
        void OnClick(GameTime time);
    }
}
