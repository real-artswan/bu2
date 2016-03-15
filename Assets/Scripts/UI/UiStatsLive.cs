using UnityEngine;

public class UiStatsLive : MonoBehaviour
{
    public int minWidth = 800;
    public GameState gameState;

    public GUIStyle styleRoundState;
    public GUIStyle styleCell;
    public GUIStyle styleFirstColumn;

    private GUIStyle subHeaderStyle;
    private Color backColor = new Color(0, 0, 0, 0.5f);

    void Awake() {
        subHeaderStyle = new GUIStyle(styleFirstColumn);
        subHeaderStyle.fixedWidth = 0;
    }

    void OnGUI() {
        //GUI.skin.font.material.color = Color.white;
        float contentWidth = Mathf.Max(minWidth, Screen.width / 2f);
        float contentLeft = Screen.width / 2f - contentWidth / 2f;
        GUIStyle style = new GUIStyle();
        style.stretchWidth = true;
        GUILayout.BeginArea(new Rect(contentLeft, 0, contentWidth, Screen.height), style);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        //GUI.color = Color.black;
        GUI.backgroundColor = Color.black;
        GUI.contentColor = BaboUtils.getInverseColor(GUI.backgroundColor);
        //GUI.contentColor = Color.white;
        //GUILayout.Box(new GUIContent(l10n.getRoundStatus(gameState.getRoundState())), roundStateStyle);
        GUILayout.Box(new GUIContent(l10n.getRoundStatus(gameState.getRoundState())), styleRoundState);
        //GUILayout.Space(spacing);
        drawHeader();
        switch (gameState.getGameType()) {
            case BaboGameType.GAME_TYPE_CTF:
            case BaboGameType.GAME_TYPE_TDM:
                GUI.backgroundColor = BaboUtils.getTeamColor(BaboPlayerTeamID.PLAYER_TEAM_RED);
                GUI.contentColor = BaboUtils.getInverseColor(GUI.backgroundColor);
                drawSubHeader(l10n.redTeam);
                GUI.backgroundColor = backColor;
                GUI.contentColor = Color.white;
                gameState.playersManager.red.Sort(Comparison);
                foreach (PlayerState player in gameState.playersManager.red) {
                    drawRow(player);
                }
                GUI.backgroundColor = BaboUtils.getTeamColor(BaboPlayerTeamID.PLAYER_TEAM_BLUE);
                GUI.contentColor = BaboUtils.getInverseColor(GUI.backgroundColor);
                drawSubHeader(l10n.blueTeam);
                GUI.backgroundColor = backColor;
                GUI.contentColor = Color.white;
                gameState.playersManager.blue.Sort(Comparison);
                foreach (PlayerState player in gameState.playersManager.blue) {
                    drawRow(player);
                }
                break;
            default:
                GUI.backgroundColor = BaboUtils.getTeamColor(BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR);
                GUI.contentColor = BaboUtils.getInverseColor(GUI.backgroundColor);
                drawSubHeader(l10n.freeForAll);
                GUI.backgroundColor = backColor;
                GUI.contentColor = Color.white;
                gameState.playersManager.both.Sort(Comparison);
                foreach (PlayerState player in gameState.playersManager.both) {
                    drawRow(player);
                }
                break;
        }
        //spectators
        GUI.backgroundColor = BaboUtils.getTeamColor(BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR, 0.5f);
        GUI.contentColor = BaboUtils.getInverseColor(GUI.backgroundColor);
        drawSubHeader(l10n.specTeam);
        GUI.backgroundColor = backColor;
        GUI.contentColor = Color.white;
        gameState.playersManager.specs.Sort(Comparison);
        foreach (PlayerState player in gameState.playersManager.specs) {
            drawRow(player);
        }
        GUI.backgroundColor = Color.black;

        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private int Comparison(PlayerState x, PlayerState y) {
        int res = y.kills - x.kills;
        if (res == 0)
            res = y.damage - x.damage;
        if (res == 0)
            res = y.playerID - x.playerID;
        return res;
    }

    void drawRow(PlayerState player) {
        GUILayout.BeginHorizontal();
        GUILayout.Label(player.playerName, styleFirstColumn);
        GUILayout.Label(player.kills.ToString(), styleCell);
        GUILayout.Label(player.deaths.ToString(), styleCell);
        GUILayout.Label(player.damage.ToString(), styleCell);
        GUILayout.Label(player.returns.ToString(), styleCell);
        GUILayout.Label(player.score.ToString(), styleCell);
        GUILayout.Label(player.ping.ToString(), styleCell);
        GUILayout.EndHorizontal();
    }
    void drawHeader() {
        GUILayout.BeginHorizontal();
        GUILayout.Label(l10n.playerName, styleFirstColumn);
        GUILayout.Label(l10n.kills, styleCell);
        GUILayout.Label(l10n.death, styleCell);
        GUILayout.Label(l10n.damage, styleCell);
        GUILayout.Label(l10n.retrns, styleCell);
        GUILayout.Label(l10n.caps, styleCell);
        GUILayout.Label(l10n.ping, styleCell);
        GUILayout.EndHorizontal();
    }

    void drawSubHeader(string title) {
        GUILayout.BeginHorizontal();
        GUILayout.Label(title, subHeaderStyle);
        GUILayout.EndHorizontal();
    }
}