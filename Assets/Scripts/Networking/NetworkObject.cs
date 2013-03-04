using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(PhotonView))]

public class NetworkObject : MonoBehaviour
{
	Vector3 correctPos;
	Quaternion correctRot;
	PhotonView photonView;
	RewinderSnapshotCreator snapshotCreator;
	RewinderHitboxGroup hitboxGroup;
	public float movementSmoothing = 6.0f;

	void Update()
	{
		//Do we need to smooth out the movement as well?
		if (!photonView.isMine)
		{
			transform.position = Vector3.Lerp(transform.position, this.correctPos, Time.deltaTime * movementSmoothing);
			transform.rotation = Quaternion.Lerp(transform.rotation, this.correctRot, Time.deltaTime * movementSmoothing);
		}
	}

	void Start()
	{
		photonView = GetComponent<PhotonView>();
		snapshotCreator = GetComponent<RewinderSnapshotCreator>();
		if(photonView.isMine)
		{
			hitboxGroup = GetComponent<RewinderHitboxGroup>();
			RewinderSnapshot.RegisterGroup(hitboxGroup);
			if(snapshotCreator != null)
				snapshotCreator.NewSnapshot += HandleNewSnapshot;
		}
		else
		{
			if(snapshotCreator != null)
				snapshotCreator.enabled = false;
		}

	}

	private void HandleNewSnapshot(RewinderSnapshot obj)
	{
		if (photonView.isMine)
		{
			RewinderHitboxGroupSnapshot group = obj.Groups.Where(e => e.Group == hitboxGroup).First();//obj.Groups.First();
			correctPos = group.Position;
			correctRot = group.Rotation;
		}
	}

	public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			// We own this player: send the others our Snapshot data.
			stream.SendNext(correctPos);
			stream.SendNext(correctRot);
		}
		else
		{
			// Network player, receive data
			this.correctPos = (Vector3)stream.ReceiveNext();
			this.correctRot = (Quaternion)stream.ReceiveNext();
		}
	}
}