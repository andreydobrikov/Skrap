using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Core;
using RAIN.Belief;
using RAIN.Action;

public class BuzzSawChase : Action
{
    public BuzzSawChase()
    {
        actionName = "BuzzSawChase";
    }

	SteerForPursuit pursuit;
	Vehicle thisVehicle;

    public override ActionResult Start(Agent agent, float deltaTime)
    {
		Debug.Log("START WAS CALLED!");
		thisVehicle = agent.Avatar.GetComponent<AutonomousVehicle>();
		//Enable the pursuit steering.
		pursuit = agent.Avatar.GetComponent<SteerForPursuit>();
		if(pursuit == null)
			pursuit = agent.Avatar.AddComponent<SteerForPursuit>();
		else
			pursuit.enabled = true;
		pursuit.Weight = 14.0f;

		//Enable the alignment steering (this reinforceses the 'swarm' feeling of buzzsaws)
		var alignment = agent.Avatar.GetComponent<SteerForAlignment>();
		if(alignment == null)
			alignment = agent.Avatar.AddComponent<SteerForAlignment>();
		alignment.enabled = true;

		thisVehicle.RefreshSteeringList();

		pursuit.Quarry = agent.actionContext.GetContextItem<GameObject>("target").transform;

		return ActionResult.RUNNING;
    }

    public override ActionResult Execute(Agent agent, float deltaTime)
    {
		if(pursuit.ReportedArrival)
		{
			Debug.Log("PURSUIT REPORTED SUCCESS!");
			return ActionResult.SUCCESS;
		}

		return ActionResult.RUNNING;
    }

    public override ActionResult Stop(Agent agent, float deltaTime)
    {
		Debug.Log("STOP WAS CALLED!");
		agent.Avatar.GetComponent<SteerForPursuit>().enabled = false;
		agent.Avatar.GetComponent<SteerForAlignment>().enabled = false;
		
		return ActionResult.SUCCESS;
    }
}