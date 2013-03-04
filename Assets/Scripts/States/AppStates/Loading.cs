using UnityEngine;
using System.Collections;
using System;
using AIFramework.StateMachine;

public class Loading : State<AppController> 
{
	public Loading(string levelToLoad) 
    {
		this.levelToLoad = levelToLoad;
    }

	AsyncOperation asyncOp;
	string levelToLoad;

	public override void Enter(AppController owner)
	{
		if (Application.loadedLevel != 0)
		{
			PhotonNetwork.isMessageQueueRunning = false;
			Application.LoadLevel("Loading");
		}


		asyncOp = Application.LoadLevelAsync(levelToLoad);
	}

	public override void Execute(AppController owner)
	{
		if (asyncOp == null)
			return;

		owner.loadingBar.sliderValue = asyncOp.progress;
		if(PhotonNetwork.connected)
			owner.PhotonView.RPC("UpdateReadyProgress", PhotonTargets.AllBuffered, asyncOp.progress, PhotonNetwork.player.name);
	}

	public override void Exit(AppController owner)
	{
		
	}
}
