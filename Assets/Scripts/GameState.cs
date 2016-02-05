using System.Collections.Generic;
using UnityEngine;
using BaboNetwork;
using UnityEngine.UI;
using System.Collections;
using System;

public class GameState: MonoBehaviour
{
	public NetConnection connection;
	public GlobalGameVariables gameVars;
	public GlobalServerVariables serverVars;
    public UIManager uiManager;
	public CanvasHUD hud;
	public Map map;
	public PlayerController thisPlayer;

	internal Dictionary<byte, PlayerState> players = new Dictionary<byte, PlayerState>();
    internal float gameTimeLeft;
    //internal float roundTimeLeft;
    internal List<string> chatMessages = new List<string>();
    internal List<string> eventMessages = new List<string>();
    internal bool isConnected;
    internal bool gotGameState;
    internal int mapSeed;
    internal short blueWin;
    internal short redWin;
	internal BaboGameType _gameType;
	internal BaboGameType getGameType(){
		return _gameType;
	}
	internal bool needToShutDown = false;
	internal BaboRoundState roundState = BaboRoundState.GAME_DONT_SHOW;

	internal void setGameType(BaboGameType gameType) {
		_gameType = gameType;
		hud.updateHudElementsVisibility();
	}

	BaboNetPacketProcessor packetsProcessor;
	private void addPacketToSend(BaboRawPacket packet) {
        //Debug.Log(DateTime.Now.ToString() + "->" + ((BaboPacketTypeID)packet.typeID).ToString());
		connection.packetsToSend.Enqueue(packet);
	}

	void Start() {
		packetsProcessor = new BaboNetPacketProcessor(this, new AddPacketCallback(addPacketToSend));
	}

	void FixedUpdate() {
        if ((connection.connected) && (connection.recievedPackets.Count > 0))
        {
            BaboRawPacket packet = connection.recievedPackets.Dequeue();
            packetsProcessor.processPacket(packet); //update game state
            /*if (packet.typeID == 106)
                Debug.Log(DateTime.Now.ToString() + " <PING> " + connection.recievedPackets.Count.ToString());*/
        }

		if (needToShutDown) {
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
}
