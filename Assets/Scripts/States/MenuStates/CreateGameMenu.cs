using UnityEngine;
using System.Collections;
using AIFramework.StateMachine;
using CorruptedSmileStudio.INI;

public sealed class CreateGameMenu : State<MenuController>
{
    static readonly CreateGameMenu instance = new CreateGameMenu();

    public static CreateGameMenu Instance
    {
        get { return instance; }
    }

    private CreateGameMenu() 
    {
		LoadLevelListFromIniFile();
		CacheButtons();
    }

	INIUnity levelList;
    private GameObject startGameButton;
	private GameObject hostGameButton;

	//This is used when a player joins a game, then exits and tries to create one.
	public void CacheButtons()
	{
		startGameButton = GameObject.Find("StartGameButton");
		hostGameButton = GameObject.Find("HostGameButton");
	}

	void LoadLevelListFromIniFile()
	{
		try
        {
            levelList = INIFile.Read(Application.dataPath + "/Config/LevelList.ini").ToINIUnity();
        }
        catch (System.IO.FileNotFoundException ex)
        {
            INIFile.Write(ex.FileName, levelList);
        }
	}

    public override void Enter(MenuController owner)
    {
		PopulateLevelList();
		if(!PhotonNetwork.connected)
			PhotonNetwork.ConnectUsingSettings("v0.1");
        owner.TweenMenuIn(owner.createGamePanels);

        foreach (GameObject panel in owner.createGamePanels)
        {
            foreach (Transform child in panel.transform)
            {
                UIEventListener.Get(child.gameObject).onClick += owner.HandleCreateGameMenuClick;
            }
        }

		startGameButton.SetActive(false);
		hostGameButton.SetActive(true);

        owner.InitCreateGame();
    }


    public override void Execute(MenuController owner)
    {

    }


    public override void Exit(MenuController owner)
    {
        owner.TweenMenuOut(owner.createGamePanels);

        foreach (GameObject panel in owner.createGamePanels)
        {
            foreach (Transform child in panel.transform)
            {
                UIEventListener.Get(child.gameObject).onClick -= owner.HandleCreateGameMenuClick;
            }
        }
		if (PhotonNetwork.room != null && PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.room.open = false;
			PhotonNetwork.room.visible = false;

			foreach (var player in PhotonNetwork.playerList)
			{
				PhotonNetwork.CloseConnection(player);
				PhotonNetwork.LeaveRoom();
			}
		}
		owner.ClearPlayerListUI();
    }

	void PopulateLevelList()
	{
		var popUpList = GameObject.Find("Input-Level").GetComponent<UIPopupList>();
		
		var levels = levelList.elements;
		foreach(var entry in levels)
		{
			var stringsplit = entry.Key.Split('.');
			foreach(var substring in stringsplit)
			{
				if(substring == "levelName")
					popUpList.items.Add(entry.Value);
			}
		}
		popUpList.selection = popUpList.items[0];
	}
}
