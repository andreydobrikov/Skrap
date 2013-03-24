using UnityEngine;
using CorruptedSmileStudio.Spawner;
using System.Collections;
using System.Linq;

public class Enemy : MonoBehaviour 
{
	[SerializeField]
	private float _health;

	public float health{get; protected set;}

	public PhotonView photonView;
	public Weapon weapon;
	public float meleeDamage;
	protected Animator animator;
	protected AutonomousVehicle vehicle;

	void Start()
	{
		vehicle = GetComponent<AutonomousVehicle>();
		photonView = GetComponent<PhotonView>();
		//Sets itself up for networking
		if (!photonView.isMine)
		{
			DisableAI();
		}

		health = _health;
		animator = transform.Find("Graphics/mine_bot").GetComponent<Animator>();
		Debug.Log(animator);
		InitEnemy();
	}

	protected virtual void InitEnemy()
	{
	}

	protected virtual void UpdateAnimation()
	{
	}


	public void DisableAI()
	{
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

	}
}
