using UnityEngine;
using CorruptedSmileStudio.Spawner;
using System.Collections;
using RAIN.Sensors;
using RAIN.Core;
using System.Linq;

public class Enemy : MonoBehaviour 
{
	[SerializeField]
	private float _health;

	public float health{get; protected set;}

	public PhotonView photonView;
	public Weapon weapon;
	public float meleeDamage;
	private RAINAgent agent;

	void Start()
	{
		photonView = GetComponent<PhotonView>();
		agent = GetComponent<RAINAgent>();
		//Sets itself up for networking
		if (!photonView.isMine)
		{
			DisableAI();
		}

		health = _health;

		InitEnemy();
	}

	protected virtual void InitEnemy()
	{
	}


	public void DisableAI()
	{
		agent.mind.gameObject.SetActive(false);
		agent.enabled = false;
		transform.Find("Path Manager").gameObject.SetActive(false);
		transform.Find("Sensor").gameObject.SetActive(false);
		transform.Find("Obstacle Avoidance Collider").gameObject.SetActive(false);

		foreach(var steerer in GetComponents<Steering>())
		{
			Destroy(steerer);
		}
		
	}

	public void ApplyDamage(float damageAmount)
	{
		health -= damageAmount;

		if(health <= 0)
		{
			GetComponent<SpawnAI>().Remove();
		}
	}

	[RPC]
	public void NetworkApplyDamageToEnemy(float damageAmount, PhotonMessageInfo info)
	{
		if(info.photonView.viewID == this.photonView.viewID)
			ApplyDamage(damageAmount);
	}

	public virtual void Attack()
	{
		animation.Play("Attack");

		
	}
}
