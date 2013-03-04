using UnityEngine;
using System.Collections;
using System;
using AIFramework.StateMachine;

public class GameRunning : State<GameController>
{
	static readonly GameRunning instance = new GameRunning();

	public static GameRunning Instance
	{
		get { return instance; }
	}
	public GameRunning()
	{

	}

	public override void Enter(GameController owner)
	{
	}

	public override void Execute(GameController owner)
	{
	}

	public override void Exit(GameController owner)
	{
	}
}