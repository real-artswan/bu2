using UnityEngine;
using UnityEngine.UI;

namespace BaboUI
{
    public class StatsTableRow : MonoBehaviour
    {
        public Text PlayerName, Kills, Death, Damage, Return, Caps, Ping;
        private BaboGameType gameType;

        public void bePlayer(PlayerState player) {
            GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
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

        public void beSubHeader(BaboPlayerTeamID team) {
            clearFields();
            GetComponent<Image>().color = BaboUtils.getTeamColor(team);
            PlayerName.text = BaboUtils.getTeamName(team);
        }

        private void clearFields() {
            PlayerName.text = "";
            Kills.text = "";
            Death.text = "";
            Damage.text = "";
            Return.text = "";
            Caps.text = "";
            Ping.text = "";
        }

        public void beHeader() {
            GetComponent<Image>().color = new Color(0, 0, 0, 128);
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

        internal void setGameType(BaboGameType baboGameType) {
            gameType = baboGameType;
        }
    }
}
