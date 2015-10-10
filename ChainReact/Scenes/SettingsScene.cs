using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChainReact.Controls;
using ChainReact.Controls.Base;
using ChainReact.Controls.Base.Interfaces;
using ChainReact.Input;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Content;
using Sharpex2D.Framework.Input;
using Sharpex2D.Framework.Rendering;
using Sharpex2D.Framework.UI;

namespace ChainReact.Scenes
{
    public class SettingsScene : Scene
    {
        private readonly Game _game;
        private readonly InputManager _input;
        private CheckboxControl _testCheckbox;

        public SettingsScene(Game game, InputManager input)
        {
            _game = game;
            _input = input;
            LoadContent(game.Content);
        }

        public override void OnUpdate(GameTime gameTime)
        {
            foreach (var control in ElementManager.ToArray())
            {
                control.Update(gameTime);
            }
           
            if (_input.Clicked.Value)
            {
                foreach (
                    var clickable in
                        ElementManager.ToArray().Where(control => control is Control && ((Control)control).IsHovered && control is IClickableControl).Cast<IClickableControl>())
                {
                    clickable.OnClick(gameTime);
                }
                foreach (
                    var checkable in
                        ElementManager.ToArray().Where(control => control is Control && ((Control)control).IsHovered && control is ICheckableControl).Cast<ICheckableControl>())
                {
                    checkable.OnCheckedChanged(gameTime);
                }
            }
        }

        public override void OnDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            BlackField.DrawField(_game, spriteBatch, 0.5f);
            _testCheckbox.Draw(spriteBatch, gameTime);
        }

        public void LoadContent(ContentManager content)
        {
            _testCheckbox = new TestCheckbox(_game, ElementManager, "Textures/CheckboxChecked", "Textures/CheckboxUnchecked", "Check Me :)", "Fonts/ButtonFont")
            {
                Bounds = new Rectangle(350, 350, 32, 32),
                Color = Color.White
            };
            ElementManager.AddRootElement(_testCheckbox);
        }
    }
}
