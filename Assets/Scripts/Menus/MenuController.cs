using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using AIFramework.StateMachine;

public class MenuController : MonoBehaviour 
{
	private StateMachine<MenuController> _stateMachine;
	
	public StateMachine<MenuController> StateMachine
	{
		get{return this._stateMachine;}
	}

	NetworkPlayerProperties playerPropsController;
	
	public GameObject frontMenuPanel;
	public GameObject[] findGamePanels;
	public GameObject[] createGamePanels;
	public GameObject createGamePopUpPanel;
	
	private Vector3 tweenOutPos;
	private AppController appController;

	void Awake()
	{
		_stateMachine = new StateMachine<MenuController>(this);
		if (GameObject.Find("AppController") == null)
			Instantiate((GameObject)Resources.Load("AppController")).name = "AppController";
	}

	void Start()
	{
		playerPropsController = GameObject.Find("NetworkMessages").GetComponent<NetworkPlayerProperties>();
		tweenOutPos = new Vector3(-1250f,0f,1.7f);
		StateMachine.SetCurrentState(MainMenu.Instance);
		StateMachine.ChangeState(LoginMenu.Instance);
		appController = GameObject.Find("AppController").GetComponent<AppController>();
	}
	
	void Update()
	{
        //Keep the menu's state machine updated every frame
		StateMachine.Update();
	}

	#region LoginMenu

	public GameObject loginMenuPanel;

	public UIInput loginUsernameInput;
	public UIInput loginPasswordInput;
	public GameObject incorrectDetailsLabel;

	public void HandleTextBoxSubmit(GameObject go)
	{
		Debug.Log("++++----" + go.name);
		if(go.name == loginUsernameInput.name)
		{
			loginUsernameInput.selected = false;
			loginPasswordInput.selected = true;
			UIInput.current = loginPasswordInput;
		}
		else if(go.name == loginPasswordInput.name)
		{
			Login (loginUsernameInput.text, loginPasswordInput.text);
		}
	}

	public void HandleLoginClick(GameObject go)
	{
		if(go.name == "LoginButton")
			Login (loginUsernameInput.text, loginPasswordInput.text);
	}


	void Login(string userName, string password)
	{
		//Replace with ruby query?
		if(!string.IsNullOrEmpty(userName))
		{
			playerPropsController.SetPlayerName(userName, true);
			StateMachine.ChangeState(MainMenu.Instance);
		}
		else
		{
			DisplayIncorrectDetailsWarning();
		}
		
	}

	void DisplayIncorrectDetailsWarning()
	{
		incorrectDetailsLabel.GetComponent<UILabel>().alpha = 1.0f;
		Invoke("HideIncorrectDetailsWarning", 1.0f);
	}

	void HideIncorrectDetailsWarning()
	{
		incorrectDetailsLabel.GetComponent<UILabel>().alpha = 0.0f;
		
	}

	#endregion

    #region FrontMenu
	UITable serverListTable;

	void InitFrontMenu()
	{

	}

    public void HandleFrontMenuClick(GameObject go)
	{
		switch(go.name)
		{
            case "StartGame":
                StateMachine.ChangeState(CreateGameMenu.Instance);
				
			    break;
		
	    	case "Search":
				StateMachine.ChangeState(FindGameMenu.Instance);
		    	break;
		
		    case "Skills":
			
			    break;
		
		    case "Seetings":
				
			    break;
		
		    case "Quit":
			    Application.Quit();
			    break;
            
            default:
                Debug.LogError("Unknown button name recieved: " + go.name);
                break;
		}
	}

    #endregion

    #region CreateGame & JoinGame

    UIInput serverNameInput;
    UIPopupList noPlayersInput;
    UIPopupList difficultyInput;
    UIInput passwordInput;
    UIPopupList levelInput;
    UITable playerList;

	List<UILabel> playerNamesList = new List<UILabel>();

    public GameObject startGameButton;
    public GameObject hostGameButton;
    public GameObject playerListPlayerPrefab;

    public void InitCreateGame()
    {
        serverNameInput = GameObject.Find("Input-ServerName").GetComponent<UIInput>();
        noPlayersInput = GameObject.Find("Input-NoPlayers").GetComponent<UIPopupList>();
        difficultyInput = GameObject.Find("Input-Difficulty").GetComponent<UIPopupList>();
        passwordInput = GameObject.Find("Input-Password").GetComponent<UIInput>();
        levelInput = GameObject.Find("Input-Level").GetComponent<UIPopupList>();
        playerList = GameObject.Find("PlayerList").GetComponent<UITable>();
    }

	public void EnableInputChangeListeners()
	{
        noPlayersInput.onSelectionChange = NetworkRoomProperties.UpdateNoPlayers;
		difficultyInput.onSelectionChange = NetworkRoomProperties.UpdateDifficulty;
		levelInput.onSelectionChange = NetworkRoomProperties.UpdateLevel;
	}

	public void DisableInputChangeListeners()
	{
		noPlayersInput.onSelectionChange = null;
		difficultyInput.onSelectionChange = null;
		levelInput.onSelectionChange = null;
	}

	public void HandleCreateGameMenuClick(GameObject go)
	{
		switch(go.name)
		{
            case "StartGameButton":
				if(AreAllPlayersReady())
				{
					NetworkController.Instance.photonView.RPC("LoadGame", PhotonTargets.All);
				}
                break;
            case "HostGameButton":
				//Disable input and display popup.
			    foreach (GameObject panel in createGamePanels)
		        {
		            foreach (Transform child in panel.transform)
		            {
		                UIEventListener.Get(child.gameObject).onClick -= HandleCreateGameMenuClick;
		            }
		        }
				FadeMenuIn(createGamePopUpPanel);
                CreateServer(serverNameInput.label.text, Convert.ToInt32(noPlayersInput.selection), difficultyInput.selection, passwordInput.label.text, levelInput.selection);
				
                DisableInputChangeListeners();
                break;
		    case "Back":
			    StateMachine.ChangeState(MainMenu.Instance);
			    break;
			case "ReadyButton":
				playerPropsController.SetPlayerReady(!(bool)PhotonNetwork.player.customProperties["isReady"]);
				break;			
		    default:
			    Debug.LogError("Unknown button name recieved: " + go.name);
			    break;
		}
	}

    void CreateServer(string serverName, int players, string difficulty, string password, string level)
    {
        Hashtable customRoomProps = new Hashtable();
        string[] publicProps = new string[]{"haspassword", "level", "difficulty"};

        customRoomProps.Add("password", password);

		customRoomProps.Add("difficulty", difficulty);

        if(!string.IsNullOrEmpty(password))
          customRoomProps.Add("haspassword", true);
        else
            customRoomProps.Add("haspassword", false);

        customRoomProps.Add("level", level);

        PhotonNetwork.CreateRoom(serverName, true, true, players, customRoomProps, publicProps);

		TurnOffInputColliders();
    }

	void TurnOffInputColliders()
	{
		serverNameInput.collider.enabled = false;
		passwordInput.collider.enabled = false;
	}

	void TurnOnInputColliders()
	{
		serverNameInput.collider.enabled = true;
		passwordInput.collider.enabled = true;
	}

    void UpdatePlayerListUI(string playerName, bool addPlayer)
    {
		if (playerList == null)
			playerList = GameObject.Find("PlayerList").GetComponent<UITable>();
        //Fancy Linq magic: Basically just checks if the playername given exists in the players list generated above.
		bool playerAlreadyExists = playerNamesList.Exists(e => e.text == playerName);

        if (addPlayer && !playerAlreadyExists)
        {
            GameObject playerLabel = (GameObject)Instantiate(playerListPlayerPrefab);
            playerLabel.transform.parent = playerList.transform;
            playerLabel.transform.Find("PlayerName").GetComponent<UILabel>().text = playerName;
			playerLabel.transform.Find("PlayerName").GetComponent<UILabel>().depth = 12;
            playerList.Reposition();
			var fixZPos = playerLabel.transform.localPosition;
			fixZPos.z = 0f;
			playerLabel.transform.localPosition = fixZPos;
			//Populate our list of player objects.
			playerNamesList.Add(playerLabel.transform.Find("PlayerName").GetComponent<UILabel>());
        }
        else if (!addPlayer && playerAlreadyExists)
        {
			//More Linq magic: It finds the entry in players that matches playerName
			var playerLabelObj = playerNamesList.Find(e => e.text == playerName);
			playerNamesList.Remove(playerLabelObj);
            Destroy(playerLabelObj.transform.parent.gameObject);
            playerList.Reposition();
        }
    }

	public void ClearPlayerListUI()
	{
		if(playerList == null)
			playerList = GameObject.Find("PlayerList").GetComponent<UITable>();
		var players = playerList.GetComponentsInChildren<UILabel>();
		foreach (var player in players)
		{
			Destroy(player.transform.parent.gameObject);
		}
	}

	public void UpdatePlayerReady(string playerName, bool isReady)
	{
		//Find the player, then grab the readytext object for them.
		var playerLabelObj = playerNamesList.Find(e => e.text == playerName).transform.parent.Find("ReadyText").GetComponent<UILabel>();
		if(isReady)
		{
			playerLabelObj.text = "Ready!";
			playerLabelObj.color = Color.green;
		}
		else
		{
			playerLabelObj.text = "NotReady!";
			playerLabelObj.color = Color.red;
		}
	}

	bool AreAllPlayersReady()
	{
		var allPlayers = PhotonNetwork.playerList;
		return allPlayers.All(e => (bool)e.customProperties["isReady"]);
	}

    #endregion

	#region FindGame

	public GameObject serverEntryPrefab;
	public UITable serverTable;
	private string selectedServerName;

	public void HandleFindGameMenuClick(GameObject go)
	{
		switch (go.name)
		{
			case "JoinGameButton":
				JoinServer();

				break;

			case "RefreshButton":
				RefreshServerList();
				break;

			case "Back":
				StateMachine.RevertToPreviousState();
				break;

			default:
				Debug.LogError("Unknown button name recieved: " + go.name);
				break;
		}
	}

	void RefreshServerList()
	{
		RemoveAllFromServerList();
		RoomInfo[] rooms = PhotonNetwork.GetRoomList();

		if (rooms != null && rooms.Length > 0)
		{
			foreach (var room in rooms)
			{
				var serverEntryGO = (GameObject)Instantiate(serverEntryPrefab);
				//Set the labels to the info from the roominfo.
				serverEntryGO.GetComponent<UIServerButton>().serverName = room.name;
				serverEntryGO.transform.Find("ServerName").GetComponent<UILabel>().text = room.name;
				serverEntryGO.transform.Find("NoOfPlayers").GetComponent<UILabel>().text = room.maxPlayers + "/" + room.playerCount;
				serverEntryGO.GetComponent<UIServerButton>().onServerSelectionChange = OnServerSelectionChange;

				serverEntryGO.transform.parent = serverTable.transform;
				serverEntryGO.GetComponent<UIServerButton>().radioButtonRoot = serverTable.transform;

				var fixZPosition = serverEntryGO.transform.localPosition;
				fixZPosition.z = -100f;
				serverEntryGO.transform.localPosition = fixZPosition;
				//Something is wrong with NGUI. I have to call Reposition and set repositionNow to true for it to do anything for some reason.
				serverTable.Reposition();
				serverTable.repositionNow = true;
			}
		}
		serverTable.Reposition();
	}

	public void RemoveAllFromServerList()
	{
		foreach (Transform child in serverTable.transform)
		{
			if (child != serverTable.transform)
			{
				Destroy(child.gameObject);
				serverTable.Reposition();
			}
		}
	}

	private void OnServerSelectionChange(bool state, string serverName)
	{
		if (!state)
		{
			selectedServerName = null;
		}
		else
		{
			selectedServerName = serverName;
		}
	}

	public void DoubleClickServer(UIServerButton buttonClicked)
	{
		selectedServerName = buttonClicked.serverName;
		JoinServer();
	}

	void JoinServer()
	{
		PhotonNetwork.JoinRoom(selectedServerName);
	}

	#endregion


	#region NetworkMessages

    void OnCreatedRoom()
    {
		if(StateMachine.CurrentState != CreateGameMenu.Instance)
			StateMachine.ChangeState(CreateGameMenu.Instance);
		foreach (GameObject panel in createGamePanels)
        {
            foreach (Transform child in panel.transform)
            {
                UIEventListener.Get(child.gameObject).onClick += HandleCreateGameMenuClick;
            }
        }
		FadeMenuOut(createGamePopUpPanel);
        EnableInputChangeListeners();
        hostGameButton.SetActive(false);
        startGameButton.SetActive(true);
    }

    void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        UpdatePlayerListUI(newPlayer.name, true);
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer oldPlayer)
    {
        UpdatePlayerListUI(oldPlayer.name, false);
    }

	void OnJoinedRoom()
	{
		if (!PhotonNetwork.isMasterClient)
		{
			StateMachine.ChangeState(JoinedGameMenu.Instance);
		}
		foreach (var player in PhotonNetwork.playerList)
		{
			UpdatePlayerListUI(player.name, true);
		}
		playerPropsController.SetPlayerReady(false);
		
	}

	void OnLeftRoom()
	{
		UpdatePlayerListUI(PhotonNetwork.player.name, false);
	}

	void OnPhotonCreateRoomFailed()
	{
		_stateMachine.ChangeState(MainMenu.Instance);
	}

    #endregion

	#region Tweens

	public void TweenMenuIn(GameObject[] uiElementsToTween)
	{
		foreach(var element in uiElementsToTween)
		{
			Debug.Log("Running tween on: " + element.name);
			TweenPosition.Begin(element, 1.0f, Vector3.zero);
		}
	}
	
	public void TweenMenuIn(GameObject uiElementToTween)
	{
		TweenPosition.Begin(uiElementToTween, 1.0f, Vector3.zero);
	}
	
	public void TweenMenuOut(GameObject[] uiElementsToTween)
	{
		foreach(var element in uiElementsToTween)
		{
			TweenPosition.Begin(element, 1.0f, tweenOutPos);
		}
	}
		
	public void TweenMenuOut(GameObject uiElementToTween)
	{
		TweenPosition.Begin(uiElementToTween, 1.0f, tweenOutPos);
    }

	public void FadeMenuIn(GameObject uiElementToTween)
	{
		TweenAlpha.Begin(uiElementToTween, 0.2f, 1.0f);
	}

	public void FadeMenuOut(GameObject uiElementToTween)
	{
		TweenAlpha.Begin(uiElementToTween, 0.2f, 0.0f);
	}
    #endregion
}
