using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Ammo : MonoBehaviour
{
	public string ammoResourceName;
	public float velocity;
	public int amount;


	public delegate void CollisionEventHandler(List<RewinderHitboxHit> hits);
	public event  CollisionEventHandler OnHit;

	protected virtual void Start()
	{
		
	}

	protected void FireOnHit(List<RewinderHitboxHit> hits)
	{
		if(OnHit != null)
			OnHit(hits);
	}

}