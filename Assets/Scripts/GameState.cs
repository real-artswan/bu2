using System.Collections.Generic;
using UnityEngine;
using BaboNetwork;
using UnityEngine.UI;
using System.Collections;
using System;

public class GameState: MonoBehaviour
{
    public GameObject explosionModel;
	public NetConnection connection;
	public GlobalGameVariables gameVars;
	public GlobalServerVariables serverVars;
    public UIManager uiManager;
	public CanvasHUD hud;
	public Map map;
	public PlayerState thisPlayer;


    internal Voting voting = new Voting();
	internal Dictionary<byte, PlayerState> players = new Dictionary<byte, PlayerState>();
	internal Dictionary<int, ProjectileState> projectiles = new Dictionary<int, ProjectileState>();
    internal float gameTimeLeft = 0;
	internal float roundTimeLeft = 0;
    internal List<string> chatMessages = new List<string>();
    internal List<string> eventMessages = new List<string>();
    internal bool gotGameState = false;
    internal int mapSeed = 0; //?? what is this
    internal short blueWin = 0;
    internal short redWin = 0;
	internal BaboGameType _gameType = BaboGameType.GAME_TYPE_DM;
	internal BaboGameType getGameType(){
		return _gameType;
	}
	internal bool needToShutDown = false;
	internal BaboRoundState roundState = BaboRoundState.GAME_DONT_SHOW;
    internal BaboWeapon nextWeapon = BaboWeapon.WEAPON_SMG;
    internal int serverFrameID = 0;
    internal bool isAdmin = false;
    internal float autoBalanceTimer = 0;
    internal short blueTeamScore = 0;
    internal short redTeamScore = 0;

    internal void setGameType(BaboGameType gameType) {
		_gameType = gameType;

        serverFrameID = 0;

        hud.updateHudElementsVisibility();
        map.flagsState.redState = BaboFlagsState.FlagState.RETURNED;
        map.flagsState.blueState = BaboFlagsState.FlagState.RETURNED;
        
        List<byte> keys = new List<byte>(players.Keys);
        foreach (byte id in keys)
        {
            if ((thisPlayer != null) && (thisPlayer.playerID == id))
            {
                players[id] = new PlayerState(id);
                thisPlayer = players[id];
            }
            else
                players[id] = new PlayerState(id);
        }
	}

	
	void Start() {
        
    }

	void Update() {
        if (hud.gameObject.activeSelf) {
            if ((gameVars.showPing) && (thisPlayer != null))
            {
                hud.pingGraph.setPing(thisPlayer.ping);
                hud.health.value = thisPlayer.life * 100;
                hud.nades.text = thisPlayer.nades.ToString();
                hud.molotovs.text = thisPlayer.molotovs.ToString();
            }
            int gtl = (int)gameTimeLeft + 1;
            int rtl = (int)roundTimeLeft + 1;
            hud.gameTimer.text = String.Format("{0:d2}:{1:d2}", gtl / 60, gtl % 60);
            hud.roundTimer.text = String.Format("{0:d2}:{1:d2}", rtl / 60, rtl % 60);
            hud.blueTeamScore.text = blueTeamScore.ToString();
            hud.redTeamScore.text = redTeamScore.ToString();
        }
    }

	void FixedUpdate() {
        if (needToShutDown)
        {
            connection.disconnect();
        }
        //repaint state

    }

	public void startGame() {
        thisPlayer.transform.position = new Vector3(0, 100, 0);
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
	}

    public void updateMenuInfo()
    {
        uiManager.gameMenuInfo.text =
                String.Format(l10n.menuGameInfo, _gameType.ToString(), "Server name", map.mapName, map.authorName, "Rules of this game"); ;
    }

    public void assignTeam(BaboPlayerTeamID team)
    {
        net_clsv_svcl_team_request teamRequest;
        teamRequest.playerID = thisPlayer.playerID;
        teamRequest.teamRequested = (sbyte)team;
        connection.packetsToSend.Enqueue(new BaboRawPacket(teamRequest));
    }

    internal void spawnExplosion(Vector3 position, Vector3 normal, float radius)
    {
        ParticleSystem expl = Instantiate(explosionModel, position, Quaternion.identity) as ParticleSystem;
        expl.gameObject.AddComponent<TimedDestroy>().delay = expl.startLifetime;
    }
}
