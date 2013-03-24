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
	Radar thisRadar;

    public override ActionResult Start(Agent agent, float deltaTime)
    {
		if(thisVehicle == null)
			thisVehicle = agent.Avatar.GetComponent<AutonomousVehicle>();
		if(thisRadar == null)
			thisRadar = agent.Avatar.GetComponent<Radar>();
		//Enable the pursuit steering.
		if(pursuit == null)
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

		var target = agent.actionContext.GetContextItem<GameObject>("target").transform;
		pursuit.Quarry = target;

		//Let other buzzsaws nearby know that we found a target.
		if (thisRadar.Detected != null)
		{
			foreach (var otherVehicle in thisRadar.Detected)
			{
				var otherAgent = otherVehicle.GetComponent<RAINAgent>();
				if (otherAgent.mind.actionContext.GetContextItem<GameObject>("target") == null)
					otherAgent.mind.actionContext.SetContextItem<GameObject>("target", target.gameObject);
			}
		}

		return ActionResult.RUNNING;
    }

    public override ActionResult Execute(Agent agent, float deltaTime)
    {
		agent.actionContext.SetContextItem<float>("distance", Vector3.Distance(agent.Avatar.transform.position, pursuit.Quarry.position));
		if(pursuit.ReportedArrival)
		{
			Debug.Log("PURSUIT REPORTED SUCCESS!");
			return ActionResult.SUCCESS;
		}
		if (agent.actionContext.GetContextItem<GameObject>("target") == null)
		{
			Debug.Log("PURSUIT REPORTED FAILURE!");
			return ActionResult.FAILURE;
		}

		return ActionResult.RUNNING;
    }

    public override ActionResult Stop(Agent agent, float deltaTime)
    {
		Debug.Log("STOP WAS CALLED!");
		agent.actionContext.SetContextItem<GameObject>("target", null);
		pursuit.Quarry = null;
		agent.Avatar.GetComponent<SteerForPursuit>().enabled = false;
		agent.Avatar.GetComponent<SteerForAlignment>().enabled = false;
		
		return ActionResult.SUCCESS;
    }
}