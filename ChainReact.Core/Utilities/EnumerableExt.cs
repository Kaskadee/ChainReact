using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChainReact.Core.Utilities
{
    public static class EnumerableExt
    {
        public static T NextOf<T>(this IList<T> list, T item)
        {
            return list[(list.IndexOf(item) + 1) == list.Count ? 0 : (list.IndexOf(item) + 1)];
        }
    }
}
