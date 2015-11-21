using System.Collections.Generic;
using System.Linq;
using ChainReact.Core.Game.Objects;

namespace ChainReact.Core.Utilities
{
    public static class EnumerableExt
    {
        public static Player NextOfPlayer(this IList<Player> players, Player item)
        {
            var list = players.Where(t => !t.Out).ToList();
            return list[(list.IndexOf(item) + 1) == list.Count ? 0 : (list.IndexOf(item) + 1)];
        }

        public static T NextOf<T>(this IList<T> list, T item)
        {
            return list[(list.IndexOf(item) + 1) == list.Count ? 0 : (list.IndexOf(item) + 1)];
        }
    }
}
