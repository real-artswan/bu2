using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityStandardAssets.CrossPlatformInput;

public class UIManager : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject gameMenu;
    public GameObject HUD;
    public GlobalGameVariables gameVars;
    public Text gameMenuInfo;
    public NetConnection connection;
    public GameObject resumeButton;
    public GameState gameState;
    public GameObject mainWeapons;

    void Start()
    {
        Cursor.visible = true;
        resumeButton.SetActive(connection.connected);
    }

    void Update()
    {
        if (!mainMenu.activeSelf && CrossPlatformInputManager.GetButton("Cancel"))
        {
            if (gameMenu.activeSelf)
                hideMenus();
            else
                showGameMenu();
        }
    }

    public void setMainWeapon(Toggle sender)
    {
        if (sender.isOn)
            gameState.nextWeapon = (BaboMainWeapon)Enum.Parse(typeof(BaboMainWeapon), sender.name);
    }

    public void askAssignTeam(Button sender)
    {
        BaboPlayerTeamID team = (BaboPlayerTeamID)Enum.Parse(typeof(BaboPlayerTeamID), sender.name);
        gameState.assignTeam(team);
        hideMenus();
    }

    public void showMainMenu()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        HUD.SetActive(false);
        gameMenu.SetActive(false);
        mainMenu.SetActive(true);
        resumeButton.SetActive(connection.connected);
    }
    public void showGameMenu()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        HUD.SetActive(true);
        gameMenu.SetActive(true);
        Transform wToggle = mainWeapons.transform.FindChild(gameState.nextWeapon.ToString());
        if (wToggle != null)
        {
            wToggle.gameObject.GetComponent<Toggle>().isOn = true;
        }
        mainMenu.SetActive(false);
    }

    public void hideMenus()
    {
        Texture2D cur = gameVars.cursorTarget;
        Cursor.SetCursor(cur, new Vector2(cur.width / 2, cur.height / 2), CursorMode.Auto);
        HUD.SetActive(true);
        gameMenu.SetActive(false);
        mainMenu.SetActive(false);
    }

    public void closeApplication()
    {
        Application.Quit();
    }

    void OnEnable()
    {

    }

    void OnDisable()
    {

    }
}
