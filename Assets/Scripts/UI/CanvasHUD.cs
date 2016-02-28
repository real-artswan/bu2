using UnityEngine;
using UnityEngine.UI;

public class CanvasHUD : MonoBehaviour
{
    public GlobalGameVariables gameVariables;
    public GameState gameState;
    public Text gameTimer;
    public Text roundTimer;
    public Text blueTeamScore;
    public Text redTeamScore;
    public Text centerMessage;
    public Text topCounter;
    public Text nades;
    public Text molotovs;
    public Slider health;
    public Slider heatWpn;
    public GameObject fpsCounter;
    public PingGraph pingGraph;

    private void setFlagsActive(bool active) {
        blueTeamScore.gameObject.transform.parent.parent.gameObject.SetActive(active);
    }

    public void updateHudElementsVisibility() {
        //game settings depending
        roundTimer.gameObject.SetActive(false);
        fpsCounter.SetActive(gameVariables.showFPS);
        pingGraph.gameObject.SetActive(gameVariables.showPing);
        //game state depending
        if (centerMessage.text == "")
            centerMessage.gameObject.SetActive(false);
        switch (gameState.getGameType()) {
            case BaboGameType.GAME_TYPE_DM:
                topCounter.gameObject.SetActive(
                    (gameState.thisPlayer != null) &&
                    (gameState.thisPlayer.getTeamID() != BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR));
                setFlagsActive(false);
                break;
            case BaboGameType.GAME_TYPE_TDM:
            case BaboGameType.GAME_TYPE_CTF:
            case BaboGameType.GAME_TYPE_SND:
                topCounter.gameObject.SetActive(false);
                setFlagsActive(true);
                break;
        }
        if ((gameState.thisPlayer != null) && (gameState.thisPlayer.getTeamID() != BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR)) {
            nades.gameObject.transform.parent.gameObject.SetActive(true);
            molotovs.gameObject.transform.parent.gameObject.SetActive(true);
            health.gameObject.SetActive(true);
            heatWpn.gameObject.SetActive(gameState.thisPlayer.getWeaponType() == BaboWeapon.WEAPON_CHAIN_GUN);
        }
        else {
            nades.gameObject.transform.parent.gameObject.SetActive(false);
            molotovs.gameObject.transform.parent.gameObject.SetActive(false);
            health.gameObject.SetActive(false);
            heatWpn.gameObject.SetActive(false);
        }
    }
}
