using UnityEngine;
using System.Collections;
using AIFramework.StateMachine;

public sealed class FrontMenu : State<AppController> 
{
	static readonly FrontMenu instance = new FrontMenu();

    public static FrontMenu Instance
    {
        get { return instance; }
    }

	private FrontMenu() 
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
