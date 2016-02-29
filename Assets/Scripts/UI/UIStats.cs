using UnityEngine;
using UnityEngine.UI;

namespace BaboUI
{
    public class UIStats : MonoBehaviour
    {
        public GameState gameState;
        public TableStats tablePrefab;
        public Transform tablesContainer;
        public Text roundStatus;
        public StatsTableRow header;

        private TableStats blueTable;
        private TableStats redTable;
        private TableStats bothTable;
        private TableStats specsTable;

        private BaboGameType currentGameType = BaboGameType.NONE;

        public void updateUI() {
            roundStatus.text = BaboUtils.getRoundStatus(gameState.getRoundState());
            if (currentGameType == gameState.getGameType())
                return;
            currentGameType = gameState.getGameType();
            header.setGameType(currentGameType);
            clearTables();
            switch (gameState.getGameType()) {
                case BaboGameType.GAME_TYPE_TDM:
                case BaboGameType.GAME_TYPE_CTF:
                    blueTable = Instantiate(tablePrefab);
                    blueTable.name = "TableBlue";
                    blueTable.players = gameState.players.blue;
                    blueTable.headerBackground.color = BaboUtils.getTeamColor(BaboPlayerTeamID.PLAYER_TEAM_BLUE);
                    blueTable.headerText.text = BaboUtils.getTeamName(BaboPlayerTeamID.PLAYER_TEAM_BLUE);
                    blueTable.gameType = currentGameType;
                    blueTable.transform.SetParent(tablesContainer);
                    BaboUtils.Log("STATS blue {0}", blueTable.players.Count);

                    redTable = Instantiate(tablePrefab);
                    redTable.name = "TableRed";
                    redTable.players = gameState.players.red;
                    redTable.headerBackground.color = BaboUtils.getTeamColor(BaboPlayerTeamID.PLAYER_TEAM_RED);
                    redTable.headerText.text = BaboUtils.getTeamName(BaboPlayerTeamID.PLAYER_TEAM_RED);
                    redTable.gameType = currentGameType;
                    redTable.transform.SetParent(tablesContainer);
                    BaboUtils.Log("STATS red {0}", redTable.players.Count);
                    break;
                case BaboGameType.GAME_TYPE_DM:
                case BaboGameType.GAME_TYPE_SND:
                    bothTable = Instantiate(tablePrefab);
                    bothTable.name = "TableBoth";
                    bothTable.players = gameState.players.both;
                    bothTable.headerBackground.color = BaboUtils.getTeamColor(BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR);
                    bothTable.headerText.text = BaboUtils.getTeamName(BaboPlayerTeamID.PLAYER_TEAM_AUTO_ASSIGN);
                    bothTable.gameType = currentGameType;
                    bothTable.transform.SetParent(tablesContainer);
                    BaboUtils.Log("STATS both {0}", bothTable.players.Count);
                    break;
            }
            specsTable = Instantiate(tablePrefab);
            specsTable.name = "TableSpecs";
            specsTable.players = gameState.players.specs;
            specsTable.headerBackground.color = BaboUtils.getTeamColor(BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR);
            specsTable.headerText.text = BaboUtils.getTeamName(BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR);
            specsTable.gameType = currentGameType;
            specsTable.transform.SetParent(tablesContainer);
            BaboUtils.Log("STATS specs {0}", specsTable.players.Count);
        }

        private void clearTables() {
            while (tablesContainer.childCount > 0) {
                Destroy(tablesContainer.GetChild(0));
            }
        }

        void Update() {
            updateUI();
        }
    }
}
