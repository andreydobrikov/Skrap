using UnityEngine;
using System.Collections;
using AIFramework.StateMachine;

public class LoginMenu : State<MenuController>
{
	static readonly LoginMenu instance = new LoginMenu();

	public static LoginMenu Instance
	{
		get { return instance; }
	}

	private LoginMenu() 
	{

	}

	public override void Enter (MenuController owner)
	{
		owner.TweenMenuIn(owner.loginMenuPanel);
		foreach (Transform button in owner.loginMenuPanel.transform)
        {
            UIEventListener.Get(button.gameObject).onClick += owner.HandleLoginClick;
			UIEventListener.Get(button.gameObject).onSubmit += owner.HandleTextBoxSubmit;
        }
	}

	public override void Execute (MenuController owner)
	{

	}

	public override void Exit (MenuController owner)
	{
		owner.TweenMenuOut(owner.loginMenuPanel);
		foreach (Transform button in owner.loginMenuPanel.transform)
        {
            UIEventListener.Get(button.gameObject).onClick -= owner.HandleLoginClick;
			UIEventListener.Get(button.gameObject).onSubmit -= owner.HandleTextBoxSubmit;
        }
	}
}
