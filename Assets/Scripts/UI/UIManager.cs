﻿using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject gameMenu;
    public CanvasHUD HUD;
    public UiStatsLive uiStats;
    public GlobalGameVariables gameVars;
    public Text gameMenuInfo;
    public NetConnection connection;
    public GameObject resumeButton;
    public GameState gameState;
    public GameObject mainWeapons;

    private bool lockStats = false;

    void Start() {
        Cursor.visible = true;
        resumeButton.SetActive(connection.connected);
    }

    void Update() {
        if (!mainMenu.activeSelf) {
            if (CrossPlatformInputManager.GetButtonUp("Cancel")) {
                if (gameMenu.activeSelf)
                    hideMenus();
                else
                    showGameMenu();
            }
            if (CrossPlatformInputManager.GetButtonDown("Stats")) {
                showStats(true);
            }
            if (!lockStats && CrossPlatformInputManager.GetButtonUp("Stats")) {
                showStats(false);
            }
        }
    }

    public void showStats(bool show) {
        uiStats.gameObject.SetActive(show);
        gameState.updateSpectatorActivity(!show);
    }

    public void lockShowStats(bool lockStats) {
        this.lockStats = lockStats;
        uiStats.gameObject.SetActive(lockStats);
        gameState.updateSpectatorActivity(!lockStats);
    }

    public void showMainMenu() {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        HUD.gameObject.SetActive(false);
        gameMenu.SetActive(false);
        mainMenu.SetActive(true);
        resumeButton.SetActive(connection.connected);
        gameState.updateSpectatorActivity(false);
    }
    public void showGameMenu() {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        HUD.gameObject.SetActive(true);
        gameMenu.SetActive(true);
        Transform wToggle = null;
        if (gameState.playersManager.thisPlayer != null)
            mainWeapons.transform.FindChild(gameState.playersManager.thisPlayer.getWeaponType().ToString());
        if (wToggle != null) {
            wToggle.gameObject.GetComponent<Toggle>().isOn = true;
        }
        mainMenu.SetActive(false);

        gameState.updateSpectatorActivity(true);
    }

    public void hideMenus() {
        Texture2D cur = gameVars.cursorTarget;
        Cursor.SetCursor(cur, new Vector2(cur.width / 2, cur.height / 2), CursorMode.Auto);
        HUD.gameObject.SetActive(true);
        gameMenu.SetActive(false);
        mainMenu.SetActive(false);

        gameState.updateSpectatorActivity(true);
    }

    public void closeApplication() {
        Application.Quit();
    }

    public void setMainWeapon(Toggle sender) {
        if (sender.isOn && (gameState.playersManager.thisPlayer != null))
            gameState.playersManager.thisPlayer.setWeaponType((BaboWeapon)Enum.Parse(typeof(BaboWeapon), sender.name));
    }

    public void setSecobdaryWeapon(Toggle sender) {
        if (sender.isOn && (gameState.playersManager.thisPlayer != null))
            gameState.playersManager.thisPlayer.setWeapon2Type((BaboWeapon)Enum.Parse(typeof(BaboWeapon), sender.name));
    }

    public void askAssignTeam(Button sender) {
        BaboPlayerTeamID team = (BaboPlayerTeamID)Enum.Parse(typeof(BaboPlayerTeamID), sender.name);
        gameState.thisPlayerAskTeam(team);
        hideMenus();
    }
}
