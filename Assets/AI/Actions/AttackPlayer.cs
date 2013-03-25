using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Core;
using RAIN.Belief;
using RAIN.Action;

public class AttackPlayer : Action
{
    public AttackPlayer()
    {
        actionName = "AttackPlayer";
    }

    public override ActionResult Start(Agent agent, float deltaTime)
    {
		Debug.Log("ATTACKING THE PLAYER!!!!!");
		agent.Avatar.GetComponent<Enemy>().Attack();
		
        return ActionResult.RUNNING;
    }

    public override ActionResult Execute(Agent agent, float deltaTime)
    {	
		  return ActionResult.SUCCESS;
    }

    public override ActionResult Stop(Agent agent, float deltaTime)
    {
		var target = agent.actionContext.GetContextItem<GameObject>("target");
		if (target != null)
		{
			var damage = agent.Avatar.GetComponent<Enemy>().meleeDamage;
			target.GetComponent<PhotonView>().RPC("NetworkApplyDamageToPlayer", PhotonTargets.All, damage);
			Debug.Log("We attacked the player. We applied " + damage + " to the player " + agent.actionContext.GetContextItem<GameObject>("target").name);
			return ActionResult.SUCCESS;
		}
		else
			return ActionResult.FAILURE;
    }
}