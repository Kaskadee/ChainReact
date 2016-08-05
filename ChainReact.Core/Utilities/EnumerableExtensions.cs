using System;
using ChainReact.Core.Game.Objects;
using System.Collections.Generic;
using System.Linq;

namespace ChainReact.Core.Utilities
{
	public static class EnumerableExtensions
	{
		public static Player NextOfPlayer(this IList<Player> players, Player item, Dictionary<Player, bool> isOut )
		{
			var list = players.Where(t => !isOut[t]).ToList();
			return list[(list.IndexOf(item) + 1) == list.Count ? 0 : (list.IndexOf(item) + 1)];
		}

		public static T NextOf<T>(this IList<T> list, T item)
		{
			return list[(list.IndexOf(item) + 1) == list.Count ? 0 : (list.IndexOf(item) + 1)];
		}
	}
}

