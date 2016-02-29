using Tacticsoft;
using UnityEngine;
using UnityEngine.UI;

namespace BaboUI
{
    public class TableStats : MonoBehaviour, ITableViewDataSource
    {
        public Text headerText;
        public Image headerBackground;
        public StatsTableRow rowPrefab;
        public TableView table;

        internal BaboGameType gameType = BaboGameType.GAME_TYPE_DM;
        internal Utils.PlayersManager.SortedPlayers players;

        public TableViewCell GetCellForRowInTableView(TableView tableView, int row) {
            StatsTableRow rowObj = Instantiate(rowPrefab) as StatsTableRow;
            rowObj.bePlayer((PlayerState)players[row], row);
            rowObj.setGameType(gameType);
            return rowObj;
        }

        public float GetHeightForRowInTableView(TableView tableView, int row) {
            return (rowPrefab.transform as RectTransform).rect.height;
        }

        public int GetNumberOfRowsForTableView(TableView tableView) {
            return players.Count;
        }

        void Awake() {
            table.dataSource = this;
        }

        void Update() {
            if (players.Count == table.Count)
                return;
            table.ReloadData();
        }
    }
}