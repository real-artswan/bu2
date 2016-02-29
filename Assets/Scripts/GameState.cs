using BaboNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class GameState : MonoBehaviour
{
    public GameObject spectator;
    public GameObject flagModel;
    public GameObject baboModel;
    public ParticleSystem explosionModel;
    public ParticleSystem flameFlowPrefab;
    public ParticleSystem shootFirePrefab;
    public ParticleSystem stopSmokePrefab;
    public Trail bulletTrailPrefab;
    public Trail photonTrailPrefab;
    public NetConnection connection;
    public GlobalGameVariables gameVars;
    public GlobalServerVariables serverVars;
    public UIManager uiManager;
    public CanvasHUD hud;
    public Map map;
    public Minimap minimap;

    internal PlayerState thisPlayer;
    internal Voting voting;
    internal PlayersManager players;
    private List<ProjectileState> projectiles;
    internal BaboFlagsState flagsState;

    internal List<string> chatMessages;
    internal List<string> eventMessages;
    //internal bool gotGameState = false;
    internal int mapSeed = 0; //?? what is this
    internal bool needToShutDown = false;
    internal int serverFrameID = 0;
    internal bool isAdmin = false;
    internal float gameTimeLeft = 0;
    internal float roundTimeLeft = 0;
    internal float autoBalanceTimer = 0;
    internal short blueTeamScore = 0;
    internal short redTeamScore = 0;
    internal float viewShake = 0;

    internal GameObject blueFlag;
    internal GameObject redFlag;

    private BaboRoundState _roundState = BaboRoundState.GAME_DONT_SHOW;
    internal BaboRoundState getRoundState() {
        return _roundState;
    }
    private BaboGameType _gameType = BaboGameType.GAME_TYPE_DM;

    internal BaboGameType getGameType() {
        return _gameType;
    }

    private void reset() {
        //serverFrameID = 0;
        autoBalanceTimer = 0;
        blueTeamScore = 0;
        redTeamScore = 0;
        viewShake = 0;
        roundTimeLeft = 0;
        _roundState = BaboRoundState.GAME_DONT_SHOW;
        voting.reset();
        flagsState.reset();
        players.resetAll();
        /*foreach (Trail trail in trails)
            trail.destroy();
        trails.Clear();*/
        foreach (ProjectileState projectile in projectiles)
            projectile.destroy();
        projectiles.Clear();
        hud.updateHudElementsVisibility();
    }

    void Awake() {
        voting = new Voting();
        players = new PlayersManager();
        projectiles = new List<ProjectileState>();
        flagsState = new BaboFlagsState();

        chatMessages = new List<string>();
        eventMessages = new List<string>();

        blueFlag = Instantiate(flagModel);
        Renderer r = blueFlag.transform.FindChild("Cloth").GetComponent<Renderer>();
        r.material = new Material(r.material);
        r.material.color = BaboUtils.getTeamColor(BaboPlayerTeamID.PLAYER_TEAM_BLUE);
        blueFlag.SetActive(false);

        redFlag = Instantiate(flagModel);
        r = redFlag.transform.FindChild("Cloth").GetComponent<Renderer>();
        r.material = new Material(r.material);
        r.material.color = BaboUtils.getTeamColor(BaboPlayerTeamID.PLAYER_TEAM_RED);
        redFlag.SetActive(false);

        gameObject.SetActive(false);
    }

    private void updateFlagPosition(BaboFlagsState.BaboFlagState state, Transform flagTransform, Vector3 initPos) {
        switch (state.state) {
            case FlagStateID.ON_POD:
                flagTransform.position = initPos;
                break;
            case FlagStateID.ON_FLOOR:
                flagTransform.position = state.position;
                break;
            default:
                PlayerState player;
                if (players.tryGetPlayer((byte)state.state, out player))
                    flagTransform.position = player.currentCF.position;
                else {
                    if (Debug.isDebugBuild)
                        Debug.LogFormat("Can not find flagger player ID {0}", (byte)state.state);
                }
                break;
        }
    }

    void Update() {
        if (needToShutDown) {
            connection.disconnect();
        }
        //flags
        if ((_gameType == BaboGameType.GAME_TYPE_CTF) && (map.mapCreated)) {
            updateFlagPosition(flagsState[BaboTeamColor.BLUE], blueFlag.transform, map.blueFlagPod.transform.position);
            updateFlagPosition(flagsState[BaboTeamColor.RED], redFlag.transform, map.redFlagPod.transform.position);
        }
        //draw trails
        /*int i = 0;
        while (i < trails.Count) {
            Trail trail = trails[i];
            if (trail = null)
                trails.Remove(trail);
            else
                i++;
        } */
        //draw projectiles
        foreach (ProjectileState projectile in projectiles) {
            projectile.update();
        }
        //update UI
        if (hud.gameObject.activeSelf) {
            //hud
            if ((gameVars.showPing) && (thisPlayer != null)) {
                hud.pingGraph.setPing(thisPlayer.ping);
                hud.health.value = thisPlayer.life * 100;
                hud.nades.text = thisPlayer.nades.ToString();
                hud.molotovs.text = thisPlayer.molotovs.ToString();
            }
            int gtl = (int)gameTimeLeft + 1;
            int rtl = (int)roundTimeLeft + 1;
            hud.gameTimer.text = String.Format("{0:d2}:{1:d2}", gtl / 60, gtl % 60);
            hud.roundTimer.text = String.Format("{0:d2}:{1:d2}", rtl / 60, rtl % 60);
            int max_score = 0;
            switch (getGameType()) {
                case BaboGameType.GAME_TYPE_CTF:
                    max_score = serverVars.sv_winLimit;
                    break;
                case BaboGameType.GAME_TYPE_TDM:
                    max_score = serverVars.sv_scoreLimit;
                    break;
            }
            if (max_score > 0) {
                hud.blueTeamScore.text = String.Format("{0}/{1}", blueTeamScore, max_score);
                hud.redTeamScore.text = String.Format("{0}/{1}", redTeamScore, max_score);
            }
        }
    }

    public void startGame() {
        gameTimeLeft = 0;
        gameObject.SetActive(true);

        uiManager.showGameMenu();
        hud.gameObject.SetActive(true);
        needToShutDown = false;
    }

    public void closeGame() {
        //close connection
        connection.disconnect();
        //clear map
        map.clearMap();
        //disable self
        gameObject.SetActive(false);
        //enable main menu
        uiManager.showMainMenu();
        //clean all
        players.destroyAll();
        blueFlag.SetActive(false);
        redFlag.SetActive(false);
        reset();
    }

    public void updateMenuInfo() {
        uiManager.gameMenuInfo.text =
                String.Format(l10n.menuGameInfo, _gameType.ToString(), "Server name", map.mapName, map.authorName, "Rules of this game"); ;
    }

    public void thisPlayerAskTeam(BaboPlayerTeamID team) {
        if ((thisPlayer == null) || (thisPlayer.getTeamID() == team))
            return;
        net_clsv_svcl_team_request teamRequest;
        teamRequest.playerID = thisPlayer.playerID;
        teamRequest.teamRequested = (sbyte)team;
        connection.packetsToSend.Enqueue(new BaboRawPacket(teamRequest));
    }

    internal void thisPlayerTeamAssigned() {
        updateSpectatorActivity(true);
    }

    internal void setRoundState(BaboRoundState newState) {
        if (_roundState == newState)
            return;
        _roundState = newState;

        uiManager.lockShowStats(newState != BaboRoundState.GAME_PLAYING);
        updateSpectatorActivity(true);
    }

    public void updateSpectatorActivity(bool allowSpecView) {
        if (!allowSpecView) {
            spectator.SetActive(false);
            return;
        }
        spectator.SetActive(
            (getRoundState() == BaboRoundState.GAME_PLAYING) && (
                (thisPlayer == null) || (thisPlayer.getTeamID() == BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR)
                )
            );
    }

    internal void setGameType(BaboGameType gameType) {
        _gameType = gameType;
        bool flagsVisible = _gameType == BaboGameType.GAME_TYPE_CTF;
        if (map.blueFlagPod != null)
            map.blueFlagPod.SetActive(flagsVisible);
        if (map.redFlagPod != null)
            map.redFlagPod.SetActive(flagsVisible);
        blueFlag.SetActive(flagsVisible);
        redFlag.SetActive(flagsVisible);

        reset();
    }

    internal void spawnExplosion(Vector3 position, Vector3 normal, float radius) {
        //if (Debug.isDebugBuild)
        //Debug.LogFormat("Explosion at {0}, normal {1}, radius {2}", position.ToString(), normal.ToString(), radius);
        ParticleSystem expl = Instantiate(explosionModel, position, Quaternion.LookRotation(normal)) as ParticleSystem;
        expl.transform.localScale = expl.transform.localScale * radius;
        expl.Play();
        gameVars.explosionMarkPrefab.createExplosionMark(position, radius, gameVars.terrainMarksLifeTime);
    }

    internal void spawnImpact(Vector3 position1, Vector3 position2, BaboWeapon shootWeapon, BaboPlayerTeamID teamID, byte nuzzleID) {

        //if (Debug.isDebugBuild)
        //  Debug.LogFormat("Shoot: {0}->{1}, id {3}", position1.ToString(), position2.ToString(), nuzzleID);

        float dmg = serverVars.weaponsVars.getWeapon(shootWeapon).damage;
        Color trailColor = BaboUtils.getTeamColor(teamID);
        /*if (Debug.isDebugBuild)
            Debug.LogFormat("Impact from team {0} with color {1}", teamID.ToString(), trailColor.ToString());*/
        switch (shootWeapon) {
            case BaboWeapon.WEAPON_FLAME_THROWER:
                //spawn fire
                ParticleSystem flame = Instantiate(flameFlowPrefab, position1, Quaternion.identity) as ParticleSystem;
                flame.gameObject.transform.LookAt(position2);
                flame.startLifetime = (position2 - position1).magnitude / 10;
                flame.Play();
                return;
            case BaboWeapon.WEAPON_PHOTON_RIFLE:
                dmg = 2.0f;
                Trail trail = Instantiate<Trail>(photonTrailPrefab);
                trail.spawnTrail(position1, position2, dmg, trailColor, dmg * 4, 0);
                for (int i = 2; i < 5; i++)
                    Instantiate<Trail>(photonTrailPrefab).spawnTrail(position1, position2, dmg / (float)Math.Pow(2, i), trailColor, dmg, 1);
                break;
            default:
                Instantiate<Trail>(bulletTrailPrefab).spawnTrail(position1, position2, dmg, trailColor, dmg * 4, 0);
                //spawn shot's glow and smoke
                ParticleSystem shoot = Instantiate(shootFirePrefab, position1, Quaternion.identity) as ParticleSystem;
                shoot.gameObject.transform.LookAt(position2);
                //shoot.transform.localScale = shoot.transform.localScale;
                shoot.Play();

                ParticleSystem smoke = Instantiate(stopSmokePrefab, position2, Quaternion.identity) as ParticleSystem;
                smoke.gameObject.transform.LookAt(position1);
                //shoot.transform.localScale = shoot.transform.localScale;
                smoke.Play();
                break;
        }
    }

    internal void AddProjectile(ProjectileState projectile) {
        switch (projectile.typeID) {
            case BaboProjectileType.PROJECTILE_DIRECT:
            case BaboProjectileType.PROJECTILE_ROCKET:
            case BaboProjectileType.PROJECTILE_GRENADE:
            case BaboProjectileType.PROJECTILE_COCKTAIL_MOLOTOV:
                PlayerState ps;
                if (players.tryGetPlayer(projectile.playerID, out ps))
                    ps.firedShowDelay = 2;
                break;
        }
        projectiles.Add(projectile);
    }

    internal void DeleteProjectile(int projectileID) {
        ProjectileState ps = projectiles.Find(p => p.uniqueID == projectileID);
        if (ps == null)
            return;
        projectiles.Remove(ps);
        ps.destroy();
    }

    internal void StickProjectile(short projectileID, byte playerID) {
        //projectileID here is not ID but index in list of projectiles (omg)
        if (projectileID >= projectiles.Count)
            return;
        //ok, lets pray it is right projectile
        projectiles[projectileID].stickToPlayer = playerID;
    }
}
