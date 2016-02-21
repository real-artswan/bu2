using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BaboUI
{
    public class StatsTable : MonoBehaviour
    {
        public GameState gameState;
        public StatsTableRow rowPrefab;
        public Text roundStatus;
        public Transform content;

        private int currentRow = 0;

        void Update() {
            roundStatus.text = BaboUtils.getRoundStatus(gameState.getRoundState());

            currentRow = 0;

            List<PlayerState> specs = new List<PlayerState>();
            switch (gameState.getGameType()) {
                case BaboGameType.GAME_TYPE_TDM:
                case BaboGameType.GAME_TYPE_CTF:
                    updateRowsCount(gameState.players.Count + 4);
                    getNextRow().beHeader();
                    List<PlayerState> red = new List<PlayerState>();
                    List<PlayerState> blue = new List<PlayerState>();
                    foreach (PlayerState p in gameState.players.Values) {
                        switch (p.teamID) {
                            case BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR:
                                specs.Add(p);
                                break;
                            case BaboPlayerTeamID.PLAYER_TEAM_BLUE:
                                blue.Add(p);
                                break;
                            case BaboPlayerTeamID.PLAYER_TEAM_RED:
                                red.Add(p);
                                break;
                        }
                    }
                    getNextRow().beSubHeader(BaboPlayerTeamID.PLAYER_TEAM_BLUE);
                    blue.Sort((x, y) => x.kills - y.kills);
                    addPlayersRows(blue);
                    getNextRow().beSubHeader(BaboPlayerTeamID.PLAYER_TEAM_RED);
                    red.Sort((x, y) => x.kills - y.kills);
                    addPlayersRows(red);
                    break;
                case BaboGameType.GAME_TYPE_DM:
                case BaboGameType.GAME_TYPE_SND:
                    updateRowsCount(gameState.players.Count + 2);
                    getNextRow().beHeader();
                    List<PlayerState> all = new List<PlayerState>();
                    foreach (PlayerState p in gameState.players.Values) {
                        if (p.teamID == BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR)
                            specs.Add(p);
                        else
                            all.Add(p);
                    }
                    all.Sort((x, y) => x.kills - y.kills);
                    addPlayersRows(all);
                    break;
            }
            getNextRow().beSubHeader(BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR);
            specs.Sort((x, y) => x.kills - y.kills);
            addPlayersRows(specs);
        }

        private void addPlayersRows(List<PlayerState> players) {
            foreach (PlayerState p in players) {
                getNextRow().bePlayer(p);
            }
        }

        private StatsTableRow getNextRow() {
            StatsTableRow row = content.GetChild(currentRow++).GetComponent<StatsTableRow>();
            row.setGameType(gameState.getGameType());
            return row;
        }

        private void deleteLastRow() {
            Destroy(content.GetChild(content.childCount - 1));
        }

        private void createRow() {
            StatsTableRow row = Instantiate<StatsTableRow>(rowPrefab);
            row.gameObject.transform.SetParent(content);
        }

        private void updateRowsCount(int newCount) {
            while (getCount() != newCount) {
                if (getCount() < newCount)
                    createRow();
                else
                    deleteLastRow();
            }
        }

        private int getCount() {
            return content.childCount;
        }
    }
}
