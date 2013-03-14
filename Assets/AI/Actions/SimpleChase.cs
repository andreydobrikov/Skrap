using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Core;
using RAIN.Belief;
using RAIN.Action;


public class SimpleChase : Action 
{
	public SimpleChase()
    {
        actionName = "SimpleChase";
    }

	SteerForPursuit pursuit;
	Vehicle thisVehicle;
	

	public override ActionResult Start (Agent agent, float deltaTime)
	{
		Debug.Log("Adding chase AI!");
		thisVehicle = agent.Avatar.GetComponent<AutonomousVehicle>();
		
		pursuit = agent.Avatar.GetComponent<SteerForPursuit>();
		if(pursuit == null)
			pursuit = agent.Avatar.AddComponent<SteerForPursuit>();
		else
			pursuit.enabled = true;
		pursuit.Weight = 14.0f;

		pursuit.Quarry = agent.actionContext.GetContextItem<GameObject>("target").transform;
		
		thisVehicle.RefreshSteeringList();
		return ActionResult.RUNNING;
		
	}

	public override ActionResult Execute (Agent agent, float deltaTime)
	{
		var distance = Vector3.Distance(agent.Avatar.transform.position, pursuit.Quarry.position);
		agent.actionContext.SetContextItem<float>("distance", distance);
		if(pursuit.ReportedArrival)
		{
			Debug.Log("PURSUIT REPORTED SUCCESS!");
			return ActionResult.SUCCESS;
		}

		return ActionResult.RUNNING;
		
	}

	public override ActionResult Stop (Agent agent, float deltaTime)
	{
		Debug.Log("removed chase AI!");
		agent.Avatar.GetComponent<SteerForPursuit>().enabled = false;
		
		return ActionResult.SUCCESS;

	}
}
