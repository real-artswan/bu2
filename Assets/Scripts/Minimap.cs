using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class Minimap : MonoBehaviour
{
    public float minimapMargins = 10;
    public GameState gameState;
    public Texture2D blueFlagTexture;
    public Texture2D redFlagTexture;

    private RawImage minimapControl;

    public void updateControlSize() {
        minimapControl.texture = gameState.map.minimap.mainTexture;
        minimapControl.SetNativeSize();
        RectTransform rt = minimapControl.gameObject.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(rt.rect.width / 2 + minimapMargins, rt.rect.height / 2 + minimapMargins);
    }

    void Start() {
        minimapControl = GetComponent<RawImage>();
    }

    void Update() {
        if (!gameState.map.mapCreated || !gameState.hud.gameObject.activeSelf)
            return;
        minimapControl.texture = drawLiveMinimap(gameState.map.minimap);
    }

    private void drawFlag(Vector3 flagPosition, Material mat) {
        mat.SetPass(0);
        float halfH = mat.mainTexture.height / 2f;
        float halfW = mat.mainTexture.width / 2f;
        Vector3 pFlagCenter = new Vector3(
            flagPosition.x + gameState.map.wShift,
            gameState.map.mapHeight - flagPosition.z - gameState.map.hShift)
            * gameState.map.minimapScaleFactor;
        Matrix4x4 mFlagPos = Matrix4x4.TRS(pFlagCenter, Quaternion.identity, new Vector3(1, 1) / gameState.map.minimapScaleFactor * 1.5f);
        GL.MultMatrix(mFlagPos);
        GL.Begin(GL.QUADS);
        {
            GL.TexCoord3(0, 1, 0);
            GL.Vertex3(-halfW, -halfH, 0);
            GL.TexCoord3(1, 1, 0);
            GL.Vertex3(halfW, -halfH, 0);
            GL.TexCoord3(1, 0, 0);
            GL.Vertex3(halfW, halfH, 0);
            GL.TexCoord3(0, 0, 0);
            GL.Vertex3(-halfW, halfH, 0);
        }
        GL.End();
    }

    private void drawFlagPod(Vector3 podPosition, Color c) {
        Vector3 pPodCenter = new Vector3(
                podPosition.x + gameState.map.wShift,
                gameState.map.mapHeight - podPosition.z - gameState.map.hShift)
                * gameState.map.minimapScaleFactor;
        Matrix4x4 mFlagPos = Matrix4x4.TRS(pPodCenter, Quaternion.identity, new Vector3(1, 1) * gameState.map.minimapScaleFactor);
        GL.MultMatrix(mFlagPos);
        GL.Begin(GL.QUADS);
        {
            GL.Color(c);
            GL.Vertex3(-0.5f, -0.5f, 0);
            GL.Vertex3(0.5f, -0.5f, 0);
            GL.Vertex3(0.5f, 0.5f, 0);
            GL.Vertex3(-0.5f, 0.5f, 0);
        }
        GL.End();
    }

    private void drawDynamicStuff(int width, int height) {
        BaboPlayerTeamID myTeam = BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR;
        if ((gameState.thisPlayer != null) && ((gameState.thisPlayer.teamID == BaboPlayerTeamID.PLAYER_TEAM_BLUE) || (gameState.thisPlayer.teamID == BaboPlayerTeamID.PLAYER_TEAM_RED)))
            myTeam = gameState.thisPlayer.teamID;

        Material mat = new Material(Shader.Find("Sprites/Default"));
        GL.PushMatrix();
        {
            mat.SetPass(0);
            GL.LoadPixelMatrix(0, width, height, 0);
            foreach (PlayerState player in gameState.players.Values) {
                if ((player == gameState.thisPlayer) || (player.status != BaboPlayerStatus.PLAYER_STATUS_ALIVE) ||
                    ((player.teamID != BaboPlayerTeamID.PLAYER_TEAM_BLUE) && (player.teamID != BaboPlayerTeamID.PLAYER_TEAM_RED)))
                    continue;
                if ((myTeam == BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR) || (myTeam == player.teamID)) {
                    Vector3 pCenter = new Vector3(
                        player.currentCF.position.x + gameState.map.wShift,
                        gameState.map.mapHeight - player.currentCF.position.z - gameState.map.hShift)
                        * gameState.map.minimapScaleFactor;
                    float angle = 180 - BaboUtils.SignedAngle(player.currentCF.mousePosOnMap - player.currentCF.position, Vector3.forward);
                    Matrix4x4 mPlayerPos = Matrix4x4.TRS(pCenter, Quaternion.Euler(0, 0, angle), new Vector3(1, 1));
                    GL.MultMatrix(mPlayerPos);
                    GL.Begin(GL.TRIANGLES);
                    {
                        GL.Color(BaboUtils.getTeamColor(player.teamID));
                        GL.Vertex3(0, 3, 0);
                        GL.Vertex3(3.5f, -6, 0);
                        GL.Vertex3(-3.5f, -6, 0);
                    }
                    GL.End();
                }
            }
            if ((gameState.thisPlayer != null) && (gameState.thisPlayer.teamID != BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR)) {
                float enemyMarkerR = gameState.map.minimapScaleFactor / 1.5f;
                foreach (PlayerState player in gameState.players.Values) {
                    if ((player == gameState.thisPlayer) || (player.status != BaboPlayerStatus.PLAYER_STATUS_ALIVE) ||
                        ((player.teamID != BaboPlayerTeamID.PLAYER_TEAM_BLUE) && (player.teamID != BaboPlayerTeamID.PLAYER_TEAM_RED)))
                        continue;

                    if ((myTeam != player.teamID) && (player.firedShowDelay > 0)) {
                        Vector3 pEnemyCenter = new Vector3(
                            player.currentCF.position.x + gameState.map.wShift,
                            gameState.map.mapHeight - player.currentCF.position.z - gameState.map.hShift)
                            * gameState.map.minimapScaleFactor;
                        Matrix4x4 mEnemyPos = Matrix4x4.TRS(pEnemyCenter, Quaternion.identity, new Vector3(1, 1));
                        GL.MultMatrix(mEnemyPos);
                        GL.Begin(GL.LINES);
                        {
                            GL.Color(BaboUtils.getTeamColor(player.teamID, player.firedShowDelay / 2f));
                            for (int i = 0; i < 30; ++i) {
                                float a = i / 30f;
                                float tmpAngle = a * Mathf.PI * 2;
                                GL.Vertex3(0, 0, 0);
                                GL.Vertex3(Mathf.Cos(tmpAngle) * enemyMarkerR, Mathf.Sin(tmpAngle) * enemyMarkerR, 0);
                            }
                        }
                        GL.End();
                    }
                }
            }
            if (gameState.getGameType() == BaboGameType.GAME_TYPE_CTF) {
                //flag pods
                drawFlagPod(gameState.map.blueFlagPod.transform.position, BaboUtils.getTeamColor(BaboPlayerTeamID.PLAYER_TEAM_BLUE));
                drawFlagPod(gameState.map.redFlagPod.transform.position, BaboUtils.getTeamColor(BaboPlayerTeamID.PLAYER_TEAM_RED));
                //flags
                mat.mainTexture = blueFlagTexture;
                drawFlag(gameState.blueFlag.transform.position, mat);
                mat.mainTexture = redFlagTexture;
                drawFlag(gameState.redFlag.transform.position, mat);
            }
        }
        GL.PopMatrix();
    }

    private Texture2D drawLiveMinimap(Material minimap) {
        int width = minimap.mainTexture.width;
        int height = minimap.mainTexture.height;
        // get a temporary RenderTexture //
        RenderTexture renderTexture = RenderTexture.GetTemporary(width, height);

        // set the RenderTexture as global target (that means GL too)
        RenderTexture.active = renderTexture;

        // clear GL //
        GL.Clear(false, true, new Color(1, 1, 1, 0));
        //copy static minimap to temporary texture
        Graphics.Blit(minimap.mainTexture, renderTexture);
        // render GL immediately to the active render texture //
        drawDynamicStuff(minimap.mainTexture.width, minimap.mainTexture.height);

        // read the active RenderTexture into a new Texture2D //
        Texture2D newTexture = new Texture2D(width, height);
        newTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        // apply pixels and compress //
        bool applyMipsmaps = false;
        newTexture.Apply(applyMipsmaps);
        bool highQuality = true;
        newTexture.Compress(highQuality);

        // clean up after the party //
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(renderTexture);

        // return the goods //
        return newTexture;
    }
}