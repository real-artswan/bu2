using UnityEngine;
using System.Collections;

public class BaboFlagsState
{
	public enum FlagState {
		RETURNED = -2,
		ABANDONED = -1,
		CAPTURED = 0,
		STOLEN = 1
	}
	public Vector3 redPos;
	public Vector3 bluePos;
	public FlagState redState = FlagState.RETURNED;
	public FlagState blueState = FlagState.RETURNED;

	public FlagState stateByID(int id)
	{
		switch (id)
		{
			case 0:
				return blueState;
			case 1:
				return redState;
		}
		return FlagState.RETURNED;
	}

	public Vector3 posByID(int id)
	{
		switch (id)
		{
			case 0:
				return bluePos;
			case 1:
				return redPos;
		}
		return Vector3.zero;
	}
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

public enum BaboWeapon
{
	WEAPON_NO = -1,
	WEAPON_SMG = 0,
	WEAPON_SHOTGUN = 1,
	WEAPON_SNIPER = 2,
	WEAPON_DUAL_MACHINE_GUN = 3,
	WEAPON_CHAIN_GUN = 4,
	WEAPON_BAZOOKA = 5,
	WEAPON_PHOTON_RIFLE = 6,
	WEAPON_FLAME_THROWER = 7,
	WEAPON_GRENADE = 8,
	WEAPON_COCKTAIL_MOLOTOV = 9,
	KNIVES = 10, 
	NUKE = 11, 
	SHIELD = 12,
	MINIBOT = 13,
	WEAPON_MINIBOT_WEAPON = 100
}

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

public enum BaboProjectileType
{
	PROJECTILE_DIRECT = 1,
	PROJECTILE_ROCKET = 2,
	PROJECTILE_GRENADE = 3,
	PROJECTILE_LIFE_PACK = 4,
	PROJECTILE_DROPED_WEAPON = 5,
	PROJECTILE_DROPED_GRENADE = 6,
	PROJECTILE_COCKTAIL_MOLOTOV = 7,
	PROJECTILE_FLAME = 8,
	PROJECTILE_GIB = 9,
	PROJECTILE_NONE = 10,
	PROJECTILE_PHOTON = 11
}

public enum BaboGrabbableItem
{
	ITEM_LIFE_PACK = 1,
	ITEM_WEAPON = 2,
	ITEM_GRENADE = 3
}