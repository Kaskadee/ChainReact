using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChainReact.Extensions
{
    public static class ExceptionExtensions
    {
        public static string ToShortString(this Exception ex)
        {
            var fullString = ex.StackTrace;
            var splitted = fullString.Split('\n');
            var lines = splitted.Length;
            var count = splitted.Length;
            var shortString = string.Empty;
            for (var i = 0; i < 3; i++)
            {
                if (lines >= i)
                {
                    shortString += splitted[i];
                    count--;
                }
            }
            if (count > 0)
                shortString += "... and " + count + " more";
            return shortString;
        }
    }
}
