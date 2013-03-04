using UnityEngine;
using System.Collections;
using System;
using AIFramework.StateMachine;

public class InGame : State<AppController>
{
	static readonly InGame instance = new InGame();

    public static InGame Instance
    {
        get { return instance; }
    }
	public InGame() 
    {
		
    }

	public override void Enter(AppController owner)
	{

	}

	public override void Execute(AppController owner)
	{

	}

	public override void Exit(AppController owner)
	{
		
	}
}