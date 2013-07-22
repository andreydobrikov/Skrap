using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerCharacter : MonoBehaviour 
{	
	private int _level;
	private int _xp;
	private int _xpNeeded;

	[SerializeField]
	protected Weapon primaryWeapon;
	[SerializeField]
	protected Weapon secondaryWeapon;
	[SerializeField]
	private float _health;

	public PhotonView photonView;

	public int level
	{
		get { return _level; }
		set { _level = value; }
	}

	public int Xp 
	{
		get { return this._xp; }
		set { _xp = value; }
	}

	public int XpNeeded 
	{
		get { return this._xpNeeded; }
		set { _xpNeeded = value; }
	}

	float fireRateTimer;

	void OnHit(List<RewinderHitboxHit> hits)
	{
		var firstHit = hits.FirstOrDefault();
		/* Keeping this for easy debugging
		foreach (var hit in hits)
		{
			Debug.Log("We hit: " + hit.Hitbox.Transform.gameObject.name);
			if (hit.Hitbox.Transform.parent != null)
				Debug.Log("Hits parent is: " + hit.Hitbox.Transform.transform.parent.gameObject.name);
		}
		 */
		RewinderSnapshot.Recycle(hits);

		if(firstHit.Transform.gameObject.tag == "Enemy")
		{
			//Check for a parent object, if one is found. Get it's Enemy script and apply damage
			if(firstHit.Transform.parent != null)
			{
				var enemyObj = firstHit.Transform.parent.gameObject;
				enemyObj.GetComponent<Enemy>().photonView.RPC("NetworkApplyDamageToEnemy", PhotonTargets.All, primaryWeapon.damage);
			}
			else
			{
				var enemyObj = firstHit.Transform.gameObject;
				enemyObj.GetComponent<Enemy>().photonView.RPC("NetworkApplyDamageToEnemy", PhotonTargets.All, primaryWeapon.damage);
			}
		}
	}

	void Start()
	{
		photonView = GetComponent<PhotonView>();
		primaryWeapon.OnHit += OnHit;
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.Mouse0))
		{
			FirePrimaryWeapon();
		}
	}


	void FirePrimaryWeapon()
	{
		if (Time.time > fireRateTimer)
		{
			fireRateTimer = Time.time + primaryWeapon.fireRate;
			primaryWeapon.Fire();
		}
	}

	void FireSecondaryWeapon()
	{

	}

	void Ability()
	{
		
	}

	public void ApplyDamage(float damageAmount)
	{
		_health -= damageAmount;

		if(_health <= 0)
		{
			//Kill player.
		}
		Debug.Log("Player recieved damage. Health is now: " + _health);
	}
	
	[RPC]
	public void NetworkApplyDamageToPlayer(float damageAmount, PhotonMessageInfo info)
	{
		Debug.Log("+++--- Applying damage to player: " + this.photonView.viewID + " info ower id: " + info.photonView.owner.ID + " this owner id: " + this.photonView.owner.ID);
		if(info.photonView.owner.ID == this.photonView.owner.ID)
			ApplyDamage(damageAmount);
	}
	
}