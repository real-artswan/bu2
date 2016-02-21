using System;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class UIManager : MonoBehaviour
{

    public GameObject mainMenu;
    public GameObject gameMenu;
    public GameObject HUD;
    public GameObject statsTable;
    public GlobalGameVariables gameVars;
    public Text gameMenuInfo;
    public NetConnection connection;
    public GameObject resumeButton;
    public GameState gameState;
    public GameObject mainWeapons;

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
                statsTable.SetActive(true);
            }
            if (CrossPlatformInputManager.GetButtonUp("Stats")) {
                statsTable.SetActive(false);
            }
        }
    }

    public void setMainWeapon(Toggle sender) {
        if (sender.isOn && (gameState.thisPlayer != null))
            gameState.thisPlayer.setWeaponType((BaboWeapon)Enum.Parse(typeof(BaboWeapon), sender.name));
    }

    public void askAssignTeam(Button sender) {
        BaboPlayerTeamID team = (BaboPlayerTeamID)Enum.Parse(typeof(BaboPlayerTeamID), sender.name);
        gameState.assignTeam(team);
        hideMenus();
    }

    public void showMainMenu() {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        HUD.SetActive(false);
        gameMenu.SetActive(false);
        mainMenu.SetActive(true);
        resumeButton.SetActive(connection.connected);
    }
    public void showGameMenu() {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        HUD.SetActive(true);
        gameMenu.SetActive(true);
        Transform wToggle = null;
        if (gameState.thisPlayer != null)
            mainWeapons.transform.FindChild(gameState.thisPlayer.getWeaponType().ToString());
        if (wToggle != null) {
            wToggle.gameObject.GetComponent<Toggle>().isOn = true;
        }
        mainMenu.SetActive(false);
    }

    public void hideMenus() {
        Texture2D cur = gameVars.cursorTarget;
        Cursor.SetCursor(cur, new Vector2(cur.width / 2, cur.height / 2), CursorMode.Auto);
        HUD.SetActive(true);
        gameMenu.SetActive(false);
        mainMenu.SetActive(false);
    }

    public void closeApplication() {
        Application.Quit();
    }
}
