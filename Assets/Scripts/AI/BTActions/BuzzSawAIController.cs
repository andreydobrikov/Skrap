using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BLTree = Behave.Runtime.Tree;
using Behave.Runtime;
using Behave;

public class BuzzSawAIController : BAStandardAI_BuzzSaw 
{
	BLTree _tree;
	GameObject currentTarget;
	GameObject[] allTargetsInRange;
	Enemy enemyObject;

	float attackRateTimer;

	public float targetMaxDistance = 25.0f;
	public float attackRange = 1.5f;


	IEnumerator Start()
	{
		enemyObject = GetComponent<Enemy>();

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
		List<GameObject> detectedObjects = new List<GameObject>();
		//Use the radar on the vehicle to scan for 'player' tagged objects.
		if (enemyObject.vehicle.Radar.Detected == null)
		{
			enemyObject.vehicle.Radar.OnUpdateRadar(null);
		}
		

		foreach (var obj in enemyObject.vehicle.Radar.Detected)
		{
			detectedObjects.Add(obj.gameObject);
		}

		allTargetsInRange = detectedObjects.Where(e => e.tag == "Player").ToArray();

		Debug.Log("We detected all targets, we found this many: " + allTargetsInRange.Length);

		return BehaveResult.Success;
	}

	public override BehaveResult TickPickATargetAction(BLTree sender, string stringParameter, float floatParameter, IAgent agent, object data)
	{
		//At the moment this just picks the closest target from allTargetsInRange. Later we will create a 'threat' variable that can be checked.
		var nearestDistanceSqr = Mathf.Infinity;
		Transform nearestObj = null;
 
		// loop through each tagged object, remembering nearest one found
		foreach (GameObject obj in allTargetsInRange) 
		{
			var objectPos = obj.transform.position;
			var distanceSqr = (objectPos - transform.position).sqrMagnitude;
 
			if (distanceSqr < nearestDistanceSqr) 
			{
				nearestObj = obj.transform;
				nearestDistanceSqr = distanceSqr;
			}
		}

		if (nearestObj == null)
			return BehaveResult.Failure;

		currentTarget = nearestObj.gameObject;

		Debug.Log("We set the current target to: " + currentTarget.gameObject);

		if (currentTarget != null)
		{
			return BehaveResult.Success;
		}
		else
		{
			return BehaveResult.Failure;
		}
	}

	public override BehaveResult TickHasTargetAction(BLTree sender, string stringParameter, float floatParameter, IAgent agent, object data)
	{
		if (currentTarget != null && Vector3.Distance(transform.position, currentTarget.transform.position) < targetMaxDistance)
			return BehaveResult.Success;

		return BehaveResult.Failure;
	}



	public override BehaveResult TickAddPursuitAction(BLTree sender, string stringParameter, float floatParameter, IAgent agent, object data)
	{
		var pursuit = gameObject.GetComponent<SteerForPursuit>();
		if (pursuit == null)
		{
			pursuit = gameObject.AddComponent<SteerForPursuit>();
			enemyObject.vehicle.RefreshSteeringList();
		}
		if (pursuit.enabled == false)
			pursuit.enabled = true;

		pursuit.Weight = 15.0f;
		pursuit.Quarry = currentTarget.transform;

		return BehaveResult.Success;

	}


	public override BehaveResult TickIsWithinAttackRangeAction(BLTree sender, string stringParameter, float floatParameter, IAgent agent, object data)
	{
		if (currentTarget == null)
		{
			Debug.Log("current target was null. Returning that the target is NOT within attack range");
			return BehaveResult.Failure;
		}

		if (Vector3.Distance(transform.position, currentTarget.transform.position) > attackRange)
		{
			Debug.Log("TARGET IS NOT IN ATTACK RANGE!");
			return BehaveResult.Failure;
		}
		else
		{
			Debug.Log("SUCCESS! Target IS in attack range!");
			return BehaveResult.Success;
		}
	}

	public override BehaveResult TickAttackAction(BLTree sender, string stringParameter, float floatParameter, IAgent agent, object data)
	{
		if (Time.time > attackRateTimer)
		{
			attackRateTimer = Time.time + enemyObject.timeBetweenAttacks;
			enemyObject.Attack();
		}
		return BehaveResult.Success;
	}

	public override BehaveResult TickIsNotWithinAttackRangeAction(BLTree sender, string stringParameter, float floatParameter, IAgent agent, object data)
	{
		if (currentTarget == null)
		{
			Debug.Log("current target was null. Returning that the target is NOT within attack range");
			return BehaveResult.Success;
		}

		if (Vector3.Distance(transform.position, currentTarget.transform.position) > attackRange)
		{
			Debug.Log("Target is NOT within attack range");
			return BehaveResult.Success;
		}
		else
		{
			return BehaveResult.Failure;
		}
	}

	public override BehaveResult TickStopAttackAction(BLTree sender, string stringParameter, float floatParameter, IAgent agent, object data)
	{
		enemyObject.FinishAttack();
		return BehaveResult.Success;
	}



}