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
		agent.Avatar.GetComponent<Enemy>().Attack();
		
        return ActionResult.RUNNING;
    }

    public override ActionResult Execute(Agent agent, float deltaTime)
    {
        return ActionResult.SUCCESS;
    }

    public override ActionResult Stop(Agent agent, float deltaTime)
    {
		var damage = agent.Avatar.GetComponent<Enemy>().meleeDamage;
		agent.actionContext.GetContextItem<GameObject>("target").GetComponent<PhotonView>().RPC("NetworkApplyDamageToPlayer", PhotonTargets.All, damage);

		return ActionResult.SUCCESS;
    }
}