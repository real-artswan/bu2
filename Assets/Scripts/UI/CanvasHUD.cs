﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CanvasHUD : MonoBehaviour {
	public GlobalGameVariables gameVariables;
	public GameState gameState;
	public Text roundTimer;
	public Text blueTeamScore;
	public Text redTeamScore;
	public Text centerMessage;
	public Text topCounter;
	public GameObject nades;
	public GameObject molotovs;
	public Slider health;
	public Slider heatWpn;
	public GameObject fpsCounter;
	public GameObject pingGraph;

	public void updateHudElementsVisibility(){
		//game settings depending
		fpsCounter.SetActive(gameVariables.showFPS);
		pingGraph.SetActive(gameVariables.showPing);
		//game state depending
		if (centerMessage.text == "")
			centerMessage.gameObject.SetActive(false);
		switch (gameState.getGameType()) {
			case BaboGameType.GAME_TYPE_DM:
				topCounter.gameObject.SetActive(
                    (gameState.thisPlayer.playerState != null) &&
                    (gameState.thisPlayer.playerState.teamID != BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR));
				blueTeamScore.gameObject.SetActive(false);
				redTeamScore.gameObject.SetActive(false);
				break;
			case BaboGameType.GAME_TYPE_TDM:
			case BaboGameType.GAME_TYPE_CTF:
			case BaboGameType.GAME_TYPE_SND:
				topCounter.gameObject.SetActive(false);
				blueTeamScore.gameObject.SetActive(true);
				redTeamScore.gameObject.SetActive(true);
				break;
		}
		if ((gameState.thisPlayer.playerState != null) && (gameState.thisPlayer.playerState.teamID != BaboPlayerTeamID.PLAYER_TEAM_SPECTATOR)) {
			nades.gameObject.SetActive(true);
			molotovs.gameObject.SetActive(true);
			health.gameObject.SetActive(true);
			heatWpn.gameObject.SetActive(gameState.thisPlayer.playerState.mainWeapon == BaboMainWeapon.WEAPON_CHAIN_GUN);
		}
		else
		{
			nades.gameObject.SetActive(false);
			molotovs.gameObject.SetActive(false);
			health.gameObject.SetActive(false);
			heatWpn.gameObject.SetActive(false);
		}
	}
}
