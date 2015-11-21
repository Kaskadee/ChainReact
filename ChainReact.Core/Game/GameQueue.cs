using System;
using System.Collections.Generic;
using System.Linq;
using Sharpex2D.Framework;

namespace ChainReact.Core.Game
{
    public class GameQueue
    {
        private readonly Dictionary<int, List<Action<GameTime>>> _queuedActions = new Dictionary<int, List<Action<GameTime>>>();

        public bool IsActionQueued => _queuedActions.Count > 0;

        /// <summary>
        /// Adds an action to the queue and returns the id
        /// </summary>
        public int Add(List<Action<GameTime>> act)
        {
            var id = GetNextAvailableId();
            _queuedActions.Add(id, act);
            return id;
        }

        /// <summary>
        /// Removes the specified action from the queue.
        /// </summary>
        public void Remove(int id)
        {
            _queuedActions.Remove(id);
        }

        /// <summary>
        /// Gets all actions.
        /// </summary>
        public Dictionary<int, List<Action<GameTime>>> GetAllActions()
        {
            var orginal = _queuedActions;
            var copy = orginal.ToDictionary(originalPair => originalPair.Key, originalPair => originalPair.Value);
            return copy;
        }

        private int GetNextAvailableId()
        {
            var id = 0;
            while(_queuedActions.ContainsKey(id))
            {
                id++;
            }
            return id;
        }
    }
}
