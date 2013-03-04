using UnityEngine;
using System.Collections;
using System.Linq;

public class NetworkPlayer : NetworkObject
{
	public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		base.OnPhotonSerializeView(stream, info);
	}
}