using UnityEngine;
using System.Collections;

public class BaboFlagsState
{
	public enum FlagState {
		DROP = -1
	}
	public Vector3 redPos;
	public Vector3 bluePos;
	public FlagState redState;
	public FlagState blueState;
}

public enum BaboPlayerTeamID
{
	PLAYER_TEAM_SPECTATOR = -1,
	PLAYER_TEAM_BLUE = 0,
	PLAYER_TEAM_RED = 1,
	PLAYER_TEAM_AUTO_ASSIGN = 2
}

public enum BaboPlayerStatus
{
	PLAYER_STATUS_ALIVE = 0,
	PLAYER_STATUS_DEAD = 1,
	PLAYER_STATUS_LOADING = 2
}

public enum BaboMainWeapon
{
	WEAPON_NO = -1,
	WEAPON_SMG,
	WEAPON_SHOTGUN,
	WEAPON_SNIPER,
	WEAPON_DUAL_MACHINE_GUN,
	WEAPON_CHAIN_GUN,
	WEAPON_BAZOOKA,
	WEAPON_PHOTON_RIFLE,
	    
};
public enum BaboSecondaryWeapon {KNIVES, SHIELD, NUKE, MINIBOT};
public enum BaboGameType { GAME_TYPE_DM, GAME_TYPE_TDM, GAME_TYPE_CTF, GAME_TYPE_SND };

public enum BaboRoundState 
{
	GAME_PLAYING = -1,
	GAME_BLUE_WIN = 0,
	GAME_RED_WIN = 1,
	GAME_DRAW = 2,
	GAME_DONT_SHOW = 3,
	GAME_MAP_CHANGE = 4
}