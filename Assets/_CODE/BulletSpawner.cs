using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletSpawner : NetworkBehaviour
{
[SerializeField] private List<GameObject>Projectiles;

	// [SerializeField]
	// private GameObject buleet;
	// [SerializeField]
	// private GameObject spell1;
	// [SerializeField]
	// private GameObject spell2;
	[SerializeField]	private Transform initialTransform;

	private void Update()
	{
		if(!IsOwner)
		return;
		if ( Input.GetKeyDown( KeyCode.LeftControl ))
		{
			SpwanBulletServerRPC(0, initialTransform.position, initialTransform.rotation );
		}
	}

	[ServerRpc]
	private void SpwanBulletServerRPC(int ProjectilesId, Vector3 position, Quaternion rotation, ServerRpcParams serverRpcParams = default )
	{
		GameObject inst_Bullet = Instantiate( Projectiles[ProjectilesId], position, rotation );
		//inst_Bullet.GetComponent<NetworkObject>().SpawnWithOwnership( serverRpcParams.Receive.SenderClientId );
		inst_Bullet.GetComponent<NetworkObject>().SpawnWithOwnership( serverRpcParams.Receive.SenderClientId );
		//inst_Bullet.GetComponent<NetworkObject>().Spawn();
	}
}