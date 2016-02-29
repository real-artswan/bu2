using Tacticsoft;
using UnityEngine;
using UnityEngine.UI;

namespace BaboUI
{
    public class StatsTableRow : TableViewCell
    {
        public static Color OddColor = new Color(0.5f, 0.5f, 0.5f, 1);
        public static Color EvenColor = new Color(0.5f, 0.7f, 0.7f, 1);
        public static Color HeaderColor = new Color(0, 0, 0, 1);

        public Text PlayerName, Kills, Death, Damage, Return, Caps, Ping;
        private BaboGameType gameType;
        private PlayerState player = null;
        private Image background;

        void Awake() {
            background = GetComponent<Image>();
        }

        void Update() {
            if (player == null)
                updateAsHeader();
            else
                updateAsPlayer();
        }

        private void updateAsPlayer() {
            if ((player.status == BaboPlayerStatus.PLAYER_STATUS_DEAD) && (player.getTeamID() != BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR))
                PlayerName.text = string.Format("{0} {1}", l10n.playerDead, player.playerName);
            else
                PlayerName.text = player.playerName;
            Kills.text = player.kills.ToString();
            Death.text = player.deaths.ToString();
            Damage.text = player.damage.ToString();
            switch (gameType) {
                case BaboGameType.GAME_TYPE_CTF:
                case BaboGameType.GAME_TYPE_TDM:
                    Return.text = player.returns.ToString();
                    Caps.text = player.score.ToString();
                    break;
                case BaboGameType.GAME_TYPE_DM:
                case BaboGameType.GAME_TYPE_SND:
                    Return.text = "";
                    Caps.text = "";
                    break;
            }
            Ping.text = player.ping.ToString();
        }

        private void updateAsHeader() {
            PlayerName.text = l10n.playerName;
            Kills.text = l10n.kills;
            Death.text = l10n.death;
            Damage.text = l10n.damage;
            switch (gameType) {
                case BaboGameType.GAME_TYPE_CTF:
                case BaboGameType.GAME_TYPE_TDM:
                    Return.text = l10n.retrns;
                    Caps.text = l10n.caps;
                    break;
                case BaboGameType.GAME_TYPE_DM:
                case BaboGameType.GAME_TYPE_SND:
                    Return.text = "";
                    Caps.text = "";
                    break;
            }
            Ping.text = l10n.ping;
        }

        public void bePlayer(PlayerState player, int index) {
            if (index % 2 == 0)
                background.color = EvenColor;
            else
                background.color = OddColor;
            this.player = player;
        }

        public void beHeader() {
            background.color = HeaderColor;
            this.player = null;
        }

        internal void setGameType(BaboGameType baboGameType) {
            gameType = baboGameType;
        }
    }
}
