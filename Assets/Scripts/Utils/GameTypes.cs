using UnityEngine;

public enum BaboTeamColor
{
    BLUE = 0,
    RED = 1
}

public class BaboFlagsState
{
    public class BaboFlagState
    {

        public Vector3 position = Vector3.zero;
        public FlagStateID state = FlagStateID.ON_POD;

        public void reset() {
            state = FlagStateID.ON_POD;
            position = Vector3.zero;
        }
    }

    private BaboFlagState[] flagsStates = new BaboFlagState[2] { new BaboFlagState(), new BaboFlagState() };

    public BaboFlagState this[BaboTeamColor color]
    {
        get { return flagsStates[(int)color]; }
    }
    public void reset() {
        foreach (BaboFlagState fs in flagsStates)
            fs.reset();
    }
}

public enum FlagStateID
{
    ON_POD = -2,
    ON_FLOOR = -1,
    //>=0 means flag captured == player's ID
}

public enum BaboPlayerTeamID
{
    PLAYER_TEAM_SPECTATOR = -1,
    PLAYER_TEAM_BLUE = BaboTeamColor.BLUE,
    PLAYER_TEAM_RED = BaboTeamColor.RED,
    PLAYER_TEAM_AUTO_ASSIGN = 2
}

public enum BaboPlayerStatus
{
    PLAYER_STATUS_ALIVE = 0,
    PLAYER_STATUS_DEAD = 1,
    UNUSED_PLAYER_STATUS_LOADING = 2
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

public enum BaboGameType
{
    NONE = -1,
    GAME_TYPE_DM,
    GAME_TYPE_TDM,
    GAME_TYPE_CTF,
    GAME_TYPE_SND
};

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
}

public enum BaboGrabbableItem
{
    ITEM_LIFE_PACK = 1,
    ITEM_WEAPON = 2,
    ITEM_GRENADE = 3
}