using UnityEngine;
using System.Collections;

public class NetworkPlayerProperties : MonoBehaviour 
{
	PhotonView photonView;

	void Start () 
	{
		photonView = GetComponent<PhotonView>();
	}

	public void InitPlayerProperties()
	{
		var playerProps = new Hashtable();

		playerProps.Add("isReady", false);

		PhotonNetwork.player.SetCustomProperties(playerProps);
	}

	void ResetPlayerProperties()
	{
		InitPlayerProperties();
	}

	public void SetPlayerReady(bool isReady)
	{
		var playerProps = new Hashtable();
		playerProps.Add("isReady", isReady);

		PhotonNetwork.player.SetCustomProperties(playerProps);

		if(PhotonNetwork.room != null)
		{
			photonView.RPC("UpdateReady", PhotonTargets.AllBuffered, PhotonNetwork.player.name, isReady);
		}
	}

	public void SetPlayerName(string name, bool isdev)
	{
		if(isdev)
			PhotonNetwork.player.name = "(DEV) " + name;
		else
			PhotonNetwork.player.name = name;
	}


	#region RPCs

	[RPC]
	void UpdateReady(string playerName, bool isReady)
	{
		GameObject.Find("MenuController").GetComponent<MenuController>().UpdatePlayerReady(playerName, isReady);
	}

	#endregion
}
