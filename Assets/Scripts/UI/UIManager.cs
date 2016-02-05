using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject gameMenu;
    public GameObject HUD;
    public GlobalGameVariables gameVars;
    public Text gameMenuInfo;
    public NetConnection connection;
    public GameObject resumeButton;

    void Start()
    {
        Cursor.visible = true;
        resumeButton.SetActive(connection.connected);
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
