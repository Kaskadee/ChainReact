namespace ChainReact.UI
{
    // TODO Implement Textbox
    public class Textbox
    {
        #region Old
        //private Texture2D _background;
        //private ElementManager _manager;

        //public Textbox(Game game, ElementManager elementManager, string text, string font, Color col, Rectangle bounds) : base(game, elementManager)
        //{
        //    _manager = elementManager;
        //    Text = text;
        //    Font = ResourceManager.Instance.GetResource<SpriteFont>(font);
        //    Color = col;
        //    Bounds = bounds;
        //    _background = ColorTextureConverter.CreateTextureFromColor((int)Bounds.Width, (int)Bounds.Height, Color.DarkGray);
        //}

        //public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        //{
        //    spriteBatch.DrawTexture(_background, Bounds, Color.White);
        //    spriteBatch.DrawString(Text, Font, Position, Color);
        //    base.Draw(spriteBatch, gameTime);
        //}

        //public override void Update(GameTime gameTime)
        //{
        //    _manager.SetFocus(this);
        //    base.Update(gameTime);
        //}

        //public override void InputStateReceived(InputState inputState)
        //{
        //    if (inputState.Is<KeyboardState>())
        //    {
        //        var keyboardState = (KeyboardState) inputState.State;
        //        if (keyboardState.IsPressed(Keys.Back))
        //        {
        //            Text = Text.Substring(0, Text.Length - 1);
        //            return;
        //        }
        //        foreach (var refer in keyboardState.AllPressed().Where(x => x.Value).Select(x => x.Key))
        //        {
        //            Text = Text + Enum.GetName(typeof(Keys), refer);
        //        }

        //    }
        //    base.InputStateReceived(inputState);
        //}
    #endregion
    }
}
