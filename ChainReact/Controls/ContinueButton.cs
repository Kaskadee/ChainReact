using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChainReact.Controls.Base;
using ChainReact.Scenes;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Rendering;
using Sharpex2D.Framework.UI;

namespace ChainReact.Controls
{
    public class ContinueButton : ButtonControl
    {
        private readonly Game _game;

        public ContinueButton(Game game, ElementManager elementManager, string button, string hovered, string font)
            : base(game, elementManager, button, hovered, font)
        {
            _game = game;
        }

        public ContinueButton(Game game, ElementManager elementManager, Texture2D button, Texture2D hovered,
            SpriteFont font) : base(game, elementManager, button, hovered, font)
        {
            _game = game;
        }

        public override void OnClick(GameTime time)
        {
            base.OnClick(time);
            _game.SceneManager.ActiveScene = null;
        }
    }
}
