using Unity.Netcode;
using UnityEngine;

public class NetworkHealthState : NetworkBehaviour
{

	[HideInInspector]
	public NetworkVariable<int> healthPoint = new NetworkVariable<int>( 100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server );
	//public NetworkVariable<int> healthPoint = new NetworkVariable<int>( 100 );

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		healthPoint.Value = 100;
	}

	[ServerRpc]
	public void TakeDamageServerRPC( int damage )
	{
		if ( healthPoint.Value <= 0 )
			return; // No need to process if the player is already dead

		healthPoint.Value -= damage; // Subtract damage from health

		if ( healthPoint.Value <= 0 )
		{
			// Handle player death here, like disabling the player object
			Debug.Log( "Player Died!" );
		}
	}







}