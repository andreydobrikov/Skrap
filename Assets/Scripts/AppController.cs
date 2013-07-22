using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AIFramework.StateMachine;

public class AppController : MonoBehaviour 
{
	private StateMachine<AppController> _stateMachine;

	public StateMachine<AppController> StateMachine
	{
		get { return _stateMachine; }
	}

	private PhotonView _photonView;

	public PhotonView PhotonView
	{
		get
		{
			if (_photonView == null)
				_photonView = gameObject.GetComponent<PhotonView>();
			return _photonView;
		}
	}

	private UISlider _loadingBar;

	public UISlider loadingBar
	{
		get
		{
			if (_loadingBar == null)
				_loadingBar = GameObject.Find("Progress Bar").GetComponent<UISlider>();
			return _loadingBar;
		}
	}

	public List<UISlider> playerLoadingBars = new List<UISlider>();
	
	void Awake()
	{
		//if(Application.internetReachability == NetworkReachability.NotReachable)
		{
			PhotonNetwork.offlineMode = true;	
		}
	}
		
	
	void Start () 
	{
		DontDestroyOnLoad(gameObject);
		_stateMachine = new StateMachine<AppController>(this);
		StateMachine.SetCurrentStateAndFireEnter(new Loading("MainMenu"));

	}

	void Update()
	{
		StateMachine.Update();
	}

	void OnLevelWasLoaded(int levelIndex)
	{
		Debug.Log("LOADED LEVEL: "+  levelIndex);
		if (levelIndex == 0 && PhotonNetwork.connected)
		{
			PhotonNetwork.isMessageQueueRunning = true;
			UITable table = GameObject.Find("LoadingTable").GetComponent<UITable>();
			foreach (var player in PhotonNetwork.playerList)
			{
				GameObject loadingStatusGO = (GameObject)GameObject.Instantiate((GameObject)Resources.Load("LoadingScreen_Player"));
				loadingStatusGO.transform.parent = table.transform;
				loadingStatusGO.transform.Find("Label").GetComponent<UILabel>().text = player.name;
				table.Reposition();
				playerLoadingBars.Add(loadingStatusGO.transform.Find("Progress Bar(Networked)").GetComponent<UISlider>());
			}
		}
		else if (levelIndex == 1)
		{
			StateMachine.ChangeState(FrontMenu.Instance);
		}
		else
		{
			StateMachine.ChangeState(InGame.Instance);
		}
	}

	[RPC]
	public void UpdateReadyProgress(float progress, string playerName)
	{
		Debug.Log("+++++----- Updating loading bar over network!!");
		var specifiedPlayersLoadingBar = playerLoadingBars.Find(e => e.transform.Find("Label").GetComponent<UILabel>().text == playerName);
		specifiedPlayersLoadingBar.sliderValue = progress;
	}
}
