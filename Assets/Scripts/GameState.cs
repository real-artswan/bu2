using System.Collections.Generic;
using UnityEngine;
using BaboNetwork;
using UnityEngine.UI;
using System.Collections;
using System;

public class GameState: MonoBehaviour
{
	public NetConnection connection;
	public GlobalGameVariables gameVars;
	public GlobalServerVariables serverVars;
    public UIManager uiManager;
	public CanvasHUD hud;
	public Map map;
	public PlayerController thisPlayer;

	internal Dictionary<byte, PlayerState> players = new Dictionary<byte, PlayerState>();
	internal Dictionary<int, ProjectileState> projectiles = new Dictionary<int, ProjectileState>();
    internal float gameTimeLeft;
	internal float roundTimeLeft;
    //internal float roundTimeLeft;
    internal List<string> chatMessages = new List<string>();
    internal List<string> eventMessages = new List<string>();
    internal bool isConnected;
    internal bool gotGameState;
    internal int mapSeed;
    internal short blueWin;
    internal short redWin;
	internal BaboGameType _gameType;
	internal BaboGameType getGameType(){
		return _gameType;
	}
	internal bool needToShutDown = false;
	internal BaboRoundState roundState = BaboRoundState.GAME_DONT_SHOW;
    internal BaboMainWeapon nextWeapon = BaboMainWeapon.WEAPON_SMG;
    internal int serverFrameID = 0;

    internal void setGameType(BaboGameType gameType) {
		_gameType = gameType;

        serverFrameID = 0;

        hud.updateHudElementsVisibility();
        map.flagsState.redState = BaboFlagsState.FlagState.INITIAL;
        map.flagsState.blueState = BaboFlagsState.FlagState.INITIAL;
        
        List<byte> keys = new List<byte>(players.Keys);
        foreach (byte id in keys)
        {
            if ((thisPlayer.playerState != null) && (thisPlayer.playerState.playerID == id))
            {
                players[id] = new PlayerState(id);
                thisPlayer.playerState = players[id];
            }
            else
                players[id] = new PlayerState(id);
        }
	}

	
	void Start() {
        serverFrameID = 0;
    }

	void Update() {
		if (gameVars.showPing && (thisPlayer.playerState != null))
			hud.pingGraph.setPing (thisPlayer.playerState.ping);
		int gtl = (int)gameTimeLeft + 1;
		int rtl = (int)roundTimeLeft + 1;
		hud.gameTimer.text = String.Format("{0:d2}:{1:d2}", gtl / 60, gtl % 60);
		hud.roundTimer.text = String.Format("{0:d2}:{1:d2}", rtl / 60, rtl % 60);
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
        teamRequest.playerID = thisPlayer.playerState.playerID;
        teamRequest.teamRequested = (sbyte)team;
        connection.packetsToSend.Enqueue(new BaboRawPacket(teamRequest));
    }
}
