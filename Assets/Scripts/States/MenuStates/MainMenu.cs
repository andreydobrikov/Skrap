using UnityEngine;
using System.Collections;
using AIFramework.StateMachine;

public sealed class MainMenu : State<MenuController>
{
    static readonly MainMenu instance = new MainMenu();

    public static MainMenu Instance
    {
        get { return instance; }
    }

    private MainMenu() 
    { 

    }

    public override void Enter(MenuController owner)
    {
		owner.TweenMenuIn(owner.frontMenuPanel);
        
		foreach (Transform button in owner.frontMenuPanel.transform)
        {
            UIEventListener.Get(button.gameObject).onClick += owner.HandleFrontMenuClick;
        }
    }


    public override void Execute(MenuController owner)
    {

    }


    public override void Exit(MenuController owner)
    {
        owner.TweenMenuOut(owner.frontMenuPanel);

        foreach (Transform button in owner.frontMenuPanel.transform)
        {
            var listener = button.GetComponent<UIEventListener>();
            if (listener)
                UIEventListener.Get(button.gameObject).onClick -= owner.HandleFrontMenuClick;
        }
    }
}