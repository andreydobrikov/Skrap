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
	}


	public void DisableAI()
	{
		agent.mind.gameObject.SetActive(false);
		agent.enabled = false;
		transform.Find("Path Manager").gameObject.SetActive(false);
		transform.Find("Sensor").gameObject.SetActive(false);
		transform.Find("Obstacle Avoidance Collider").gameObject.SetActive(false);
		transform.Find("NeighbourSensor").gameObject.SetActive(false);

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
	public void NetworkApplyDamage(float damageAmount)
	{
		ApplyDamage(damageAmount);
	}
}
