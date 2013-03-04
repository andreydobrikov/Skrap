using UnityEngine;
using System.Collections;
using AIFramework.StateMachine;

public sealed class FindGameMenu : State<MenuController> 
{
	static readonly FindGameMenu instance = new FindGameMenu();

	public static FindGameMenu Instance
	{
		get { return instance; }
	}

	private FindGameMenu() 
	{ 

	}
	public override void Enter(MenuController owner)
	{
		if(!PhotonNetwork.connected)
			PhotonNetwork.ConnectUsingSettings("v0.1");
		owner.TweenMenuIn(owner.findGamePanels);

		foreach (GameObject panel in owner.findGamePanels)
		{
			foreach (Transform child in panel.transform)
			{
				UIEventListener.Get(child.gameObject).onClick += owner.HandleFindGameMenuClick;
			}
		}

	}

	public override void Execute(MenuController owner)
	{
 		
	}

	public override void Exit(MenuController owner)
	{
		owner.TweenMenuOut(owner.findGamePanels);

		foreach (GameObject panel in owner.findGamePanels)
		{
			foreach (Transform child in panel.transform)
			{
				UIEventListener.Get(child.gameObject).onClick -= owner.HandleFindGameMenuClick;
			}
		}
		owner.RemoveAllFromServerList();
	}
}
