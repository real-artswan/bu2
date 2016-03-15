using UnityEngine;

//[RequireComponent(typeof(RawImage))]
public class Minimap : MonoBehaviour
{
    internal BaboFlagsState flagsState;
    public Vector2 margins = new Vector2(10, 10);
    public Map map;
    public Texture2D blueFlagTexture;
    public Texture2D redFlagTexture;
    internal bool canDraw() {
        return (map.mapCreated && this.gameObject.activeSelf);
    }
    private Material spritesMaterial;
    private float flagHalf;
    private PlayersManager playersManager;

    void Awake() {
        spritesMaterial = new Material(Shader.Find("Sprites/Default"));
        flagHalf = blueFlagTexture.width / 2f;
        GameObject go = GameObject.FindWithTag("PlayersManager");
        if (go != null)
            playersManager = go.GetComponent<PlayersManager>();

    }

    private Vector3 mapCoordToMiniCoord(Vector3 mapPos) {
        return (new Vector2(
            mapPos.x + map.wShift,
            mapPos.z + map.hShift)
            * map.minimapScaleFactor) + margins;
    }

    private void drawFlag(Vector3 flagPosition) {
        spritesMaterial.SetPass(0);
        Matrix4x4 mFlagPos = Matrix4x4.TRS(flagPosition, Quaternion.identity,
            new Vector3(1, 1) / map.minimapScaleFactor * 1.5f);
        GL.MultMatrix(mFlagPos);
        GL.Begin(GL.QUADS);
        {
            GL.TexCoord3(0, 1, 0);
            GL.Vertex3(-flagHalf, flagHalf, 0);
            GL.TexCoord3(1, 1, 0);
            GL.Vertex3(flagHalf, flagHalf, 0);
            GL.TexCoord3(1, 0, 0);
            GL.Vertex3(flagHalf, -flagHalf, 0);
            GL.TexCoord3(0, 0, 0);
            GL.Vertex3(-flagHalf, -flagHalf, 0);
        }
        GL.End();
    }

    private void drawFlagPod(Vector3 podPosition, Color c) {
        Matrix4x4 mFlagPos = Matrix4x4.TRS(podPosition, Quaternion.identity, new Vector3(1, 1) * map.minimapScaleFactor);
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
        if (playersManager == null)
            return;
        BaboPlayerTeamID myTeam = BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR;
        if ((playersManager.thisPlayer != null) && ((playersManager.thisPlayer.getTeamID() == BaboPlayerTeamID.PLAYER_TEAM_BLUE)
            || (playersManager.thisPlayer.getTeamID() == BaboPlayerTeamID.PLAYER_TEAM_RED)))
            myTeam = playersManager.thisPlayer.getTeamID();
        spritesMaterial.mainTexture = null;
        GL.PushMatrix();
        {
            GL.LoadPixelMatrix();
            map.minimapMaterial.SetPass(0);
            GL.Begin(GL.QUADS);
            {
                GL.TexCoord3(0, 0, 0);
                GL.Vertex3(margins.x, margins.y, 0);
                GL.TexCoord3(0, 1, 0);
                GL.Vertex3(margins.x, height + margins.y, 0);
                GL.TexCoord3(1, 1, 0);
                GL.Vertex3(width + margins.x, height + margins.y, 0);
                GL.TexCoord3(1, 0, 0);
                GL.Vertex3(width + margins.x, margins.y, 0);
            }
            GL.End();

            spritesMaterial.SetPass(0);

            foreach (PlayerState player in playersManager) {
                if ((player == playersManager.thisPlayer)
                    || (player.status != BaboPlayerStatus.PLAYER_STATUS_ALIVE)
                    || ((player.getTeamID() != BaboPlayerTeamID.PLAYER_TEAM_BLUE)
                    && (player.getTeamID() != BaboPlayerTeamID.PLAYER_TEAM_RED)))
                    continue;
                if ((myTeam == BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR)
                    || (myTeam == player.getTeamID())) {
                    Vector3 pCenter = mapCoordToMiniCoord(player.currentCF.position);
                    float angle = BaboUtils.SignedAngle(player.currentCF.mousePosOnMap - player.currentCF.position, Vector3.forward);
                    Matrix4x4 mPlayerPos = Matrix4x4.TRS(pCenter, Quaternion.Euler(0, 0, angle), new Vector3(1, 1));
                    GL.MultMatrix(mPlayerPos);
                    GL.Begin(GL.TRIANGLES);
                    {
                        GL.Color(BaboUtils.getTeamColor(player.getTeamID()));
                        GL.Vertex3(0, 3, 0);
                        GL.Vertex3(3.5f, -6, 0);
                        GL.Vertex3(-3.5f, -6, 0);
                    }
                    GL.End();
                }
            }
            if ((playersManager.thisPlayer != null) && (playersManager.thisPlayer.getTeamID() != BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR)) {
                float enemyMarkerR = map.minimapScaleFactor / 1.5f;
                foreach (PlayerState player in playersManager) {
                    if ((player == playersManager.thisPlayer) || (player.status != BaboPlayerStatus.PLAYER_STATUS_ALIVE) ||
                        ((player.getTeamID() != BaboPlayerTeamID.PLAYER_TEAM_BLUE) && (player.getTeamID() != BaboPlayerTeamID.PLAYER_TEAM_RED)))
                        continue;

                    if ((myTeam != player.getTeamID()) && (player.firedShowDelay > 0)) {
                        Vector3 pEnemyCenter = mapCoordToMiniCoord(player.currentCF.position);
                        Matrix4x4 mEnemyPos = Matrix4x4.TRS(pEnemyCenter, Quaternion.identity, new Vector3(1, 1));
                        GL.MultMatrix(mEnemyPos);
                        GL.Begin(GL.LINES);
                        {
                            GL.Color(BaboUtils.getTeamColor(player.getTeamID(), player.firedShowDelay / 2f));
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
            if (flagsState != null) {
                //flag pods
                drawFlagPod(mapCoordToMiniCoord(map.blueFlagPod.transform.position),
                    BaboUtils.getTeamColor(BaboPlayerTeamID.PLAYER_TEAM_BLUE));
                drawFlagPod(mapCoordToMiniCoord(map.redFlagPod.transform.position),
                    BaboUtils.getTeamColor(BaboPlayerTeamID.PLAYER_TEAM_RED));
                //flags
                spritesMaterial.mainTexture = blueFlagTexture;
                drawFlag(mapCoordToMiniCoord(flagsState[BaboTeamColor.BLUE].position));
                spritesMaterial.mainTexture = redFlagTexture;
                drawFlag(mapCoordToMiniCoord(flagsState[BaboTeamColor.RED].position));
            }
        }
        GL.PopMatrix();
    }

    public void drawLiveMinimap() {
        int width = map.minimapMaterial.mainTexture.width;
        int height = map.minimapMaterial.mainTexture.height;
        //GL.Clear(false, true, new Color(1, 1, 1, 0));
        drawDynamicStuff(width, height);
    }
}