using System.Collections.Generic;
using UnityEngine;
using BaboNetwork;
using UnityEngine.UI;
using System.Collections;
using System;

public class GameState: MonoBehaviour
{
	public GameObject baboModel;
	public ParticleSystem explosionModel;
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
	internal List<Trail> trails = new List<Trail>();
    internal float gameTimeLeft = 0;
	internal float roundTimeLeft = 0;
    internal List<string> chatMessages = new List<string>();
    internal List<string> eventMessages = new List<string>();
    internal bool gotGameState = false;
    internal int mapSeed = 0; //?? what is this
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
	internal float viewShake = 0;

    internal void setGameType(BaboGameType gameType) {
		_gameType = gameType;

        serverFrameID = 0;

        hud.updateHudElementsVisibility();
        map.flagsState.redState = BaboFlagsState.FlagState.RETURNED;
        map.flagsState.blueState = BaboFlagsState.FlagState.RETURNED;
        
		foreach (PlayerState ps in players.Values)
        {
			ps.reset();
        }
	}


	void Start() {
		gameObject.SetActive(false);
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

	void FixedUpdate() {
        if (needToShutDown)
        {
            connection.disconnect();
        }
        //repaint state

    }

	public void startGame() {
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
		Debug.LogFormat("nade at {0}, normal {1}, radius {2}", position.ToString(), normal.ToString(), radius);
		ParticleSystem expl = Instantiate(explosionModel, position, Quaternion.LookRotation(normal)) as ParticleSystem;
		expl.transform.localScale = expl.transform.localScale * radius;
		expl.Play();
    }

	internal void spawnImpact(Vector3 position1, Vector3 position2, Vector3 normal, BaboWeapon shootWeapon, float damage, BaboPlayerTeamID teamID) {
		Color trailColor = Color.black;
		switch (shootWeapon){
			case BaboWeapon.WEAPON_FLAME_THROWER:
				//spawn fire
				return;
			case BaboWeapon.WEAPON_PHOTON_RIFLE:
				switch (teamID) {
					case BaboPlayerTeamID.PLAYER_TEAM_BLUE:
						trailColor = new Color(0.25f, 0.25f, 0.9f, 1);
						break;
					case BaboPlayerTeamID.PLAYER_TEAM_RED:
						trailColor = new Color(0.9f, 0.25f, 0.25f, 1);
						break;
				}
				float dmg = 2.0f;
				trails.Add(new Trail(position1, position2, dmg, trailColor, dmg * 4, 0));
				for (int i = 2; i < 5; i++)
					trails.Add(new Trail(position1, position2, dmg / (float)Math.Pow(2, i), trailColor, dmg, 1));
				break;
			default:
				switch (teamID) {
					case BaboPlayerTeamID.PLAYER_TEAM_BLUE:
						trailColor = new Color(0.5f, 0.5f, 0.9f, 1);
						break;
					case BaboPlayerTeamID.PLAYER_TEAM_RED:
						trailColor = new Color(0.9f, 0.5f, 0.5f, 1);
						break;
				}
				trails.Add(new Trail(position1, position2, damage, trailColor, damage * 4, 0));
				break;
		}
		//spawn shot's glow and smoke
	}
}
