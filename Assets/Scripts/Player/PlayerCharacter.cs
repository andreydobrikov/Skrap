using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerCharacter : MonoBehaviour 
{
	private int m_level;
	private int m_xp;
	private int m_xpNeeded;
	[SerializeField]
	protected Weapon primaryWeapon;
	[SerializeField]
	protected Weapon secondaryWeapon;
	
	public int level
	{
		get { return m_level; }
		set { m_level = value; }
	}

	public int Xp 
	{
		get { return this.m_xp; }
		set { m_xp = value; }
	}

	public int XpNeeded 
	{
		get { return this.m_xpNeeded; }
		set { m_xpNeeded = value; }
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
				enemyObj.GetComponent<Enemy>().photonView.RPC("NetworkApplyDamage", PhotonTargets.All, primaryWeapon.damage);
			}
			else
			{
				var enemyObj = firstHit.Transform.gameObject;
				enemyObj.GetComponent<Enemy>().photonView.RPC("NetworkApplyDamage", PhotonTargets.All, primaryWeapon.damage);
			}
		}


		
	}

	void Start()
	{
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
	
}