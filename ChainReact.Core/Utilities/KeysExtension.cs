using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpex2D.Framework.Input;

namespace ChainReact.Core.Utilities
{
    public static class KeysExtension
    {
        public static string GetStringValue(string str, Keys key)
        {
            var value = (int) key;
            var character = (char) value;
            return ReplaceSpecialCharacter(str, character);
        }

        public static string GetStringValue(string str, Keys[] keys)
        {
            var result = str;
            foreach (var key in keys)
            {
                var value = (int) key;
                var character = (char) value;
                ReplaceSpecialCharacter(result, character);
            }
            return result;
        }

        public static string ReplaceSpecialCharacter(string str, char chr)
        {
            switch (chr)
            {
                case (char)8:
                    return str.Remove(str.Length - 1);
            }
            return str;
        }
    }
}
