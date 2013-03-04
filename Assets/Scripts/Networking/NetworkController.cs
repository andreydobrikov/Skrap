using UnityEngine;
using System.Collections;

public class NetworkController : MonoBehaviour 
{
	NetworkPlayerProperties playerPropController;
	public PhotonView photonView;

	static NetworkController instance;

	public static NetworkController Instance
	{
		get { return instance; }
	}

	void Start () 
	{
		DontDestroyOnLoad(this.gameObject);
		instance = this;
		photonView = GetComponent<PhotonView>();
		playerPropController = GetComponent<NetworkPlayerProperties>();
		if(!PhotonNetwork.connected)
		{
			PhotonNetwork.ConnectUsingSettings("1.0");
		}
	}

	[RPC]
	public void LoadGame()
	{
		var appcontroller = GameObject.Find("AppController").GetComponent<AppController>();
		appcontroller.StateMachine.ChangeState(new Loading((string)PhotonNetwork.room.customProperties["level"]));
	}



	#region Photon Messages

	void OnConnectedToPhoton()
	{
		playerPropController.InitPlayerProperties();
	}


	#endregion
}
