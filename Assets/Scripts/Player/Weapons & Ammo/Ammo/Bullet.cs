using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : Ammo
{
	private PhotonView photonView;

	protected override void Start()
	{
		photonView = GetComponent<PhotonView>();
		Invoke("DestroyBullet", 10.0f);

		rigidbody.AddForce(transform.forward * velocity, ForceMode.Impulse);
	}

	void DestroyBullet()
	{
		if (photonView.isMine)
			PhotonNetwork.Destroy(gameObject);
	}
}
