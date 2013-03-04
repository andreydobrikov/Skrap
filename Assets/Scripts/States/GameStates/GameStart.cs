using UnityEngine;
using System.Collections;
using System;
using AIFramework.StateMachine;

public class GameStart : State<GameController>
{
	static readonly GameStart instance = new GameStart();

	public static GameStart Instance
	{
		get { return instance; }
	}
	public GameStart()
	{

	}

	public override void Enter(GameController owner)
	{
		var playerClone = PhotonNetwork.Instantiate("Player", new Vector3(0,10,0), Quaternion.identity, 0);
		var cameraSystemClone = (GameObject)GameObject.Instantiate((GameObject)Resources.Load("Camera Target"));
		//This is our player, so turn on it's controls and create the camera system.
		playerClone.AddComponent<Rigidbody>();

		cameraSystemClone.transform.parent = playerClone.transform;
		cameraSystemClone.transform.localPosition = new Vector3(0f, 1.1f, 0f);
		playerClone.GetComponent<CharacterControls>().enabled = true;
		playerClone.GetComponent<PlayerCharacter>().enabled = true;
		owner.StartCoroutine(owner.CountdownToGameStart());
	}

	public override void Execute(GameController owner)
	{

	}

	public override void Exit(GameController owner)
	{
		if(PhotonNetwork.isMasterClient)
			owner.spawnController.EnableSpawner(owner.spawnController.spawnID);
	}
}