using UnityEngine;
using System.Collections;
using AIFramework.StateMachine;

public class JoinedGameMenu : State<MenuController>
{
	static readonly JoinedGameMenu instance = new JoinedGameMenu();

	public static JoinedGameMenu Instance
	{
		get { return instance; }
	}

	private JoinedGameMenu() 
	{
		startGameButton = GameObject.Find("StartGameButton");
		hostGameButton = GameObject.Find("HostGameButton");
	}

	private GameObject startGameButton;
	private GameObject hostGameButton;

	public override void Enter(MenuController owner)
	{
		CreateGameMenu.Instance.CacheButtons();
		owner.TweenMenuIn(owner.createGamePanels);

		foreach (GameObject panel in owner.createGamePanels)
		{
			foreach (Transform child in panel.transform)
			{
				UIEventListener.Get(child.gameObject).onClick += owner.HandleCreateGameMenuClick;
			}
		}

		if(startGameButton != null)
			startGameButton.SetActive(false);
		if(hostGameButton != null)
			hostGameButton.SetActive(false);
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
		owner.ClearPlayerListUI();
		PhotonNetwork.LeaveRoom();
	}
}
