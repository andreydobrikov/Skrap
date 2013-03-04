using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
	public float damage;
	public float fireRate;
	public float maxAmmo;
	public Transform bulletOrigin;
	public Ammo ammo;

	public delegate void CollisionEventHandler(List<RewinderHitboxHit> hits);
	public event CollisionEventHandler OnHit;

	public virtual void Fire()
	{
		
	}

	public virtual void Reload()
	{

	}

	public virtual void Start()
	{

	}

	protected void FireOnHit(List<RewinderHitboxHit> hits)
	{
		if (OnHit != null)
			OnHit(hits);
	}
}