using UnityEngine;
using System.Collections;
using System;

public class PlayerState {

	public string playerName = "";
	public readonly byte playerID = 0;
	public Int32 netID = 0;
	public string ip = "";
	internal short ping = 0;
	internal BaboPlayerTeamID teamID = BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR;
	internal BaboPlayerStatus status = BaboPlayerStatus.PLAYER_STATUS_LOADING;
	internal BaboMainWeapon mainWeapon = BaboMainWeapon.WEAPON_NO;

	public readonly PlayerController playerController = null;
	public PlayerState(byte playerID, PlayerController playerController = null) {
		this.playerID = playerID;
		this.playerController = playerController;
	}
}
