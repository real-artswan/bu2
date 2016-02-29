using System;
using System.Collections;
using System.Collections.Generic;
using Wintellect.PowerCollections;

namespace Utils
{
    public class PlayersManager : IEnumerable<PlayerState>, IDisposable
    {
        public class PlayersKillsComparer : IComparer<PlayerState>
        {
            public int Compare(PlayerState x, PlayerState y) {
                return y.kills - x.kills;
            }
        }

        public class SortedPlayers : OrderedBag<PlayerState>
        {
            public SortedPlayers() : base(new PlayersKillsComparer()) { }

        };

        internal readonly SortedPlayers blue = new SortedPlayers();
        internal readonly SortedPlayers red = new SortedPlayers();
        internal readonly SortedPlayers both = new SortedPlayers();
        internal readonly SortedPlayers specs = new SortedPlayers();
        private Dictionary<byte, PlayerState> players = new Dictionary<byte, PlayerState>();
        private bool ignoreDestroy = false;

        public PlayersManager() {
            PlayerState.OnTeamChanged += onPlayerTeamChanged;
            PlayerState.OnPlayerCreated += onPlayerCreated;
            PlayerState.OnPlayerDestroyed += onPlayerDestroyed;
        }

        public void Dispose() {
            PlayerState.OnTeamChanged -= onPlayerTeamChanged;
            PlayerState.OnPlayerCreated -= onPlayerCreated;
            PlayerState.OnPlayerDestroyed -= onPlayerDestroyed;
        }

        private void onPlayerDestroyed(PlayerState player) {
            if (ignoreDestroy)
                return;
            BaboUtils.Log("MANAGER Player destroyed {0} {1}", player.ToString(), player.getTeamID());
            players.Remove(player.playerID);
            blue.Remove(player);
            red.Remove(player);
            both.Remove(player);
            int sc = specs.Count;
            specs.Remove(player);
            if (sc == specs.Count) {
                foreach (PlayerState p in specs) {
                    BaboUtils.Log(p.ToString() + "; ");
                }
            }
        }

        private void onPlayerCreated(PlayerState player) {
            BaboUtils.Log("MANAGER Player created {0} {1}", player.ToString(), player.getTeamID());
            players.Add(player.playerID, player);
            sortPlayer(player);
        }

        private void onPlayerTeamChanged(PlayerState player, BaboPlayerTeamID prevTeam) {
            BaboUtils.Log("MANAGER team changed player {0} from team {1} to team {2}", player, prevTeam, player.getTeamID());
            switch (prevTeam) {
                case BaboPlayerTeamID.PLAYER_TEAM_BLUE:
                    blue.Remove(player);
                    both.Remove(player);
                    break;
                case BaboPlayerTeamID.PLAYER_TEAM_RED:
                    red.Remove(player);
                    both.Remove(player);
                    break;
                default:
                    BaboUtils.Log("removed from specs");
                    specs.Remove(player);
                    BaboUtils.Log("specs count {0}", specs.Count);
                    break;
            }
            sortPlayer(player);
        }

        private void sortPlayer(PlayerState player) {
            switch (player.getTeamID()) {
                case BaboPlayerTeamID.PLAYER_TEAM_BLUE:
                    blue.Add(player);
                    both.Add(player);
                    break;
                case BaboPlayerTeamID.PLAYER_TEAM_RED:
                    red.Add(player);
                    both.Add(player);
                    break;
                default:
                    specs.Add(player);
                    break;
            }
        }

        public void destroyAll() {
            ignoreDestroy = true;
            try {
                foreach (PlayerState ps in players.Values) {
                    ps.destroy();
                }
                players.Clear();
                blue.Clear();
                red.Clear();
                both.Clear();
                specs.Clear();
            }
            finally {
                ignoreDestroy = false;
            }
        }

        public bool tryGetPlayer(byte id, out PlayerState player) {
            return players.TryGetValue(id, out player);
        }

        public void resetAll() {
            foreach (PlayerState ps in players.Values) {
                ps.reset();
            }
        }

        public bool contains(byte id) {
            return players.ContainsKey(id);
        }

        public IEnumerator<PlayerState> GetEnumerator() {
            return players.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
