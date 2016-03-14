using UnityEngine;

public class UiStatsLive : MonoBehaviour
{
    public int minWidth = 800;
    public BaboGameType gameType;
    public PlayersManager players;
    public Font defaultFont;

    public GUIStyle roundStateStyle;
    public GUIStyle labelStyle;
    public GUIStyle labelStylePlayerName;

    private GUIStyle subHeaderStyle;
    private Color backColor = new Color(0, 0, 0, 0.5f);
    void Awake() {
        subHeaderStyle = new GUIStyle(labelStylePlayerName);
        subHeaderStyle.fixedWidth = 0;

    }

    void OnGUI() {
        if (defaultFont != null)
            GUI.skin.font = defaultFont;
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
        GUILayout.Box(new GUIContent(l10n.getRoundStatus(BaboRoundState.GAME_PLAYING)), roundStateStyle);
        //GUILayout.Space(spacing);
        drawHeader();
        switch (gameType) {
            case BaboGameType.GAME_TYPE_CTF:
            case BaboGameType.GAME_TYPE_TDM:
                GUI.backgroundColor = BaboUtils.getTeamColor(BaboPlayerTeamID.PLAYER_TEAM_RED);
                GUI.contentColor = BaboUtils.getInverseColor(GUI.backgroundColor);
                drawSubHeader(l10n.redTeam);
                GUI.backgroundColor = backColor;
                GUI.contentColor = Color.white;
                players.red.Sort(Comparison);
                foreach (PlayerState player in players.red) {
                    drawRow(player);
                }
                GUI.backgroundColor = BaboUtils.getTeamColor(BaboPlayerTeamID.PLAYER_TEAM_BLUE);
                GUI.contentColor = BaboUtils.getInverseColor(GUI.backgroundColor);
                drawSubHeader(l10n.blueTeam);
                GUI.backgroundColor = backColor;
                GUI.contentColor = Color.white;
                players.blue.Sort(Comparison);
                foreach (PlayerState player in players.blue) {
                    drawRow(player);
                }
                break;
            default:
                GUI.backgroundColor = BaboUtils.getTeamColor(BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR);
                GUI.contentColor = BaboUtils.getInverseColor(GUI.backgroundColor);
                drawSubHeader(l10n.freeForAll);
                GUI.backgroundColor = backColor;
                GUI.contentColor = Color.white;
                players.both.Sort(Comparison);
                foreach (PlayerState player in players.both) {
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
        players.specs.Sort(Comparison);
        foreach (PlayerState player in players.specs) {
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
        GUILayout.Label(player.playerName, labelStylePlayerName);
        GUILayout.Label(player.kills.ToString(), labelStyle);
        GUILayout.Label(player.deaths.ToString(), labelStyle);
        GUILayout.Label(player.damage.ToString(), labelStyle);
        GUILayout.Label(player.returns.ToString(), labelStyle);
        GUILayout.Label(player.score.ToString(), labelStyle);
        GUILayout.Label(player.ping.ToString(), labelStyle);
        GUILayout.EndHorizontal();
    }
    void drawHeader() {
        GUILayout.BeginHorizontal();
        GUILayout.Label(l10n.playerName, labelStylePlayerName);
        GUILayout.Label(l10n.kills, labelStyle);
        GUILayout.Label(l10n.death, labelStyle);
        GUILayout.Label(l10n.damage, labelStyle);
        GUILayout.Label(l10n.retrns, labelStyle);
        GUILayout.Label(l10n.caps, labelStyle);
        GUILayout.Label(l10n.ping, labelStyle);
        GUILayout.EndHorizontal();
    }

    void drawSubHeader(string title) {
        GUILayout.BeginHorizontal();
        GUILayout.Label(title, subHeaderStyle);
        GUILayout.EndHorizontal();
    }
}