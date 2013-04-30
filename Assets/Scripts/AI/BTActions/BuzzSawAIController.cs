using UnityEngine;
using System.Collections;
using BLTree = Behave.Runtime.Tree;
using Behave.Runtime;
using Behave;

public class BuzzSawAIController : BAStandardAI_BuzzSaw 
{
	BLTree _tree;
	GameObject currentTarget;
	GameObject[] allTargetsInRange;

	public float targetMaxDistance = 25.0f;


	IEnumerator Start()
	{
		_tree = BLStandardAI.InstantiateTree(BLStandardAI.TreeType.Enemies_BuzzSaw, this);

		while (Application.isPlaying && _tree != null)
		{
			AIUpdate();
			yield return new WaitForSeconds(1 / _tree.Frequency);
		}
	}

	void AIUpdate()
	{
		if (_tree.Tick() != BehaveResult.Running)
		{
			Debug.Log("Resetting tree!");
			_tree.Reset();
		}
	}

	public static BehaveResult UnhandlesID(int id)
	{
		if (BLStandardAI.IsAction(id))
		{
			Debug.LogError("Unhandled Action: " + ((BLStandardAI.ActionType)id).ToString());
		}
		else if (BLStandardAI.IsDecorator(id))
		{
			Debug.LogError("Unhandled decorator: " + ((BLStandardAI.DecoratorType)id).ToString());
		}
		else
		{
			Debug.LogError("Invalid ID: " + id);
		}
		return BehaveResult.Success;
	}

	public override BehaveResult Tick(BLTree sender, bool init)
	{
		return UnhandlesID(sender.ActiveID);
	}

	public override void Reset(BLTree sender)
	{
		
	}

	public override int SelectTopPriority(BLTree sender, params int[] ids)
	{
		return 0;
	}



	public override BehaveResult TickHasNoTargetAction(BLTree sender, string stringParameter, float floatParameter, IAgent agent, object data)
	{
		if (currentTarget == null)
		{
			return BehaveResult.Success;
		}
		else if (Vector3.Distance(transform.position, currentTarget.transform.position) > targetMaxDistance)
		{
			return BehaveResult.Success;
		}

		return BehaveResult.Failure;
	}

	public override BehaveResult TickGetAllTargetsAction(BLTree sender, string stringParameter, float floatParameter, IAgent agent, object data)
	{
		allTargetsInRange = new GameObject[0];
		//Use the radar on the vehicle to scan for 'player' or 'enemy' tagged objects. (Is there a Neighbours variable as part of the radar system?)

		return BehaveResult.Failure;
	}

	public override BehaveResult TickPickATargetAction(BLTree sender, string stringParameter, float floatParameter, IAgent agent, object data)
	{
		//Pick the closest target from allTargetsInRange that has the tag 'player'.

		return base.TickPickATargetAction(sender, stringParameter, floatParameter, agent, data);
	}

	public override BehaveResult TickHasTargetAction(BLTree sender, string stringParameter, float floatParameter, IAgent agent, object data)
	{
		if (currentTarget != null && Vector3.Distance(transform.position, currentTarget.transform.position) < targetMaxDistance)
			return BehaveResult.Success;

		return BehaveResult.Failure;
	}



	public override BehaveResult TickAddPursuitAction(BLTree sender, string stringParameter, float floatParameter, IAgent agent, object data)
	{
		return base.TickAddPursuitAction(sender, stringParameter, floatParameter, agent, data);
	}


	public override BehaveResult TickIsWithinAttackRangeAction(BLTree sender, string stringParameter, float floatParameter, IAgent agent, object data)
	{
		return base.TickIsWithinAttackRangeAction(sender, stringParameter, floatParameter, agent, data);
	}

	public override BehaveResult TickAttackAction(BLTree sender, string stringParameter, float floatParameter, IAgent agent, object data)
	{
		return base.TickAttackAction(sender, stringParameter, floatParameter, agent, data);
	}

	public override BehaveResult TickIsNotWithinAttackRangeAction(BLTree sender, string stringParameter, float floatParameter, IAgent agent, object data)
	{
		return base.TickIsNotWithinAttackRangeAction(sender, stringParameter, floatParameter, agent, data);
	}

	public override BehaveResult TickStopAttackAction(BLTree sender, string stringParameter, float floatParameter, IAgent agent, object data)
	{
		return base.TickStopAttackAction(sender, stringParameter, floatParameter, agent, data);
	}



}