using ChainReact.Controls.Base;
using ChainReact.Input;
using ChainReact.Scenes;
using Sharpex2D.Framework;
using Sharpex2D.Framework.Rendering;
using Sharpex2D.Framework.UI;

namespace ChainReact.Controls
{
    public class SettingsButton : ButtonControl
    {
        private readonly Game _game;
        private readonly Scene _settingsScene;

        public SettingsButton(Game game, ElementManager elementManager, string button, string hovered, string font, InputManager input) : base(game, elementManager, button, hovered, font)
        {
            _game = game;
            _settingsScene = new SettingsScene(game, input);
        }

        public SettingsButton(Game game, ElementManager elementManager, Texture2D button, Texture2D hovered, SpriteFont font, InputManager input) : base(game, elementManager, button, hovered, font)
        {
            _game = game;
            _settingsScene = new SettingsScene(game, input);
        }

        public override void OnClick(GameTime time)
        {
            base.OnClick(time);
            _game.SceneManager.Add(_settingsScene);
            _game.SceneManager.ActiveScene = _settingsScene;
        }
    }
}
