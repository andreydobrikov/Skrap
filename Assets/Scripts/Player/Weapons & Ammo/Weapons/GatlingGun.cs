using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GatlingGun : Weapon 
{
	PhotonView photonView;

	public override void Start()
	{
		photonView = transform.root.GetComponent<PhotonView>();
		Debug.Log(transform.root.name);
	} 

	public override void Fire()
	{
		PhotonNetwork.Instantiate(this.ammo.ammoResourceName, this.bulletOrigin.position, bulletOrigin.transform.rotation, 0);

		if (photonView.isMine)
		{
			Ray r = new Ray(bulletOrigin.position, bulletOrigin.forward);

			List<RewinderHitboxHit> hits = new List<RewinderHitboxHit>();
			Debug.Log("Local Time: " + TimeSpan.FromSeconds(Time.time).TotalMilliseconds + " Photon time: " + (float)PhotonNetwork.time + " Latest snapshot time: " + RewinderSnapshot.Latest.Time);
			RewinderSnapshot.Raycast(Time.time, r.origin, r.direction, out hits);
			Debug.DrawRay(r.origin, r.direction, Color.green, 30.0f);

			if (hits.Count > 0)
			{
				FireOnHit(hits);
				//RewinderSnapshot.Recycle(hits);	
			}
		}
	}

	public override void Reload()
	{
		base.Reload();
	}
}
