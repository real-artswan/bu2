using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersManager : MonoBehaviour, IEnumerable<PlayerState>
{
    public static PlayersManager findSelf() {
        GameObject go = GameObject.FindWithTag("PlayersManager");
        if (go != null)
            return go.GetComponent<PlayersManager>();
        else
            return null;
    }

    public byte getUniqueID() {
        byte id;
        do {
            id = (byte)Random.Range(byte.MinValue, byte.MaxValue);
        }
        while (players.ContainsKey(id));
        return id;
    }

    internal PlayerState thisPlayer;

    internal readonly List<PlayerState> blue = new List<PlayerState>();
    internal readonly List<PlayerState> red = new List<PlayerState>();
    internal readonly List<PlayerState> both = new List<PlayerState>();
    internal readonly List<PlayerState> specs = new List<PlayerState>();
    private Dictionary<byte, PlayerState> players = new Dictionary<byte, PlayerState>();
    private bool ignoreDestroy = false;

    private List<PlayerState>[] lists = new List<PlayerState>[4];

    void Awake() {
        lists[0] = blue;
        lists[1] = red;
        lists[2] = both;
        lists[3] = specs;
        PlayerState.OnTeamChanged += onPlayerTeamChanged;
        PlayerState.OnPlayerCreated += onPlayerCreated;
        PlayerState.OnPlayerDestroyed += onPlayerDestroyed;
    }

    void OnDestroy() {
        PlayerState.OnTeamChanged -= onPlayerTeamChanged;
        PlayerState.OnPlayerCreated -= onPlayerCreated;
        PlayerState.OnPlayerDestroyed -= onPlayerDestroyed;
    }

    private void onPlayerDestroyed(PlayerState player) {
        if (ignoreDestroy)
            return;
        BaboUtils.Log("MANAGER Player destroyed {0} {1}", player.ToString(), player.getTeamID());
        players.Remove(player.playerID);
        for (int i = 0; i < lists.Length; i++) {
            lists[i].Remove(player);
        }
        BaboUtils.Log("destroy specs {0}", specs.Count);
    }

    private void onPlayerCreated(PlayerState player) {
        BaboUtils.Log("MANAGER Player created {0} {1}", player.ToString(), player.getTeamID());
        players.Add(player.playerID, player);
        sortPlayer(player);
    }

    private void onPlayerTeamChanged(PlayerState player) {
        BaboUtils.Log("MANAGER team changed by player {0} to team {1}", player, player.getTeamID());
        for (int i = 0; i < lists.Length; i++) {
            lists[i].Remove(player);
        }
        BaboUtils.Log("change specs {0}", specs.Count);
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
        BaboUtils.Log("sort specs {0}", specs.Count);
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
