using Unity.Netcode;
using UnityEngine;

public class bulletScript : NetworkBehaviour
{
	private float speed = 20f;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();

		GetComponent<Rigidbody>().velocity = transform.forward * speed;// * Time.deltaTime;
	}

	private void OnCollisionEnter( Collision collision )
	{
		Debug.Log( $" [OnCollisionEnter] > Bullet collider.name: {collision.collider.name}" );

		Debug.Log( $" [OnTriggerEnter] > IsOwner: {IsOwner} IsServer: {IsServer}" );

		if ( !IsServer ) // Ensure this runs only on the server
		{
			Debug.Log( "Collisinon NOT SERVER I AM" );
			return;
		}

		var hitPlayer = collision.gameObject.GetComponent<NetworkHealthState>(); // Assume PlayerHealth is the script that manages health
		if ( hitPlayer != null )
		{
			hitPlayer.TakeDamageServerRPC( 10 ); // Example: applying 10 damage
		}

		Destroy( gameObject ); // Destroy the bullet on hit
	}

	private void OnTriggerEnter( Collider other )
	{

		Debug.Log( $" [OnTriggerEnter] > Bullet collider.name: {other.name}" );
		//if( other.name == "PlayerArmature(Clone)" )
		if ( other.name == "PlayerArmature(Clone)" && GetComponent<NetworkObject>().OwnerClientId != other.GetComponent<NetworkObject>().OwnerClientId )
		{
			// not server Ownerd :D
			//if ( !IsServer ) // Ensure this runs only on the server
			//{
			//	Debug.Log( "Trigger NOT SERVER I AM" );
			//	return;
			//}

			var hitPlayer = other.gameObject.GetComponent<NetworkHealthState>(); // Assume PlayerHealth is the script that manages health
			if ( hitPlayer != null )
			{
				Debug.Log( "administer Pain" );
				hitPlayer.TakeDamageServerRPC( 10 ); // Example: applying 10 damage
			}
			else
			{

				Debug.Log( "administer Pain FAILED" );

			}


			Destroy( gameObject ); // Destroy the bullet on hit
								   //if ( other.GetComponent<bulletScript>() && GetComponent<NetworkObject>().OwnerClientId != other.GetComponent<NetworkObject>().OwnerClientId )
								   //{
								   //	other.GetComponent<NetworkHealthState>().healthPoint.Value -= 10;
								   //}
		}
	}
}