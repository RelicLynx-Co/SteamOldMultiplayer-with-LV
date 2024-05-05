using Unity.Netcode;
using UnityEngine;

namespace _root_namespace
{
	public class PlayerDummyNetworkSync : NetworkBehaviour
	{
		private readonly NetworkVariable<PlayerNetworkData> _netState = new(writePerm: NetworkVariableWritePermission.Owner);
		private Vector3 _vel;
		private float _rotVel;
		[SerializeField] private float _cheapInterpolation = 0.1f;
		[SerializeField] private Vector3 respawnPosition = new Vector3(0f, 10f, 0f); // Example respawn position

		private void Update()
		{
			if (IsOwner)
			{
				_netState.Value = new PlayerNetworkData
				{
					Position = transform.position,
					Rotation = transform.rotation.eulerAngles
				};
			}
			else
			{
				// hill-billy interpolation
				transform.position = Vector3.SmoothDamp(transform.position, _netState.Value.Position, ref _vel, _cheapInterpolation);
				transform.rotation = Quaternion.Euler(
					0,
					Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, _netState.Value.Rotation.y, ref _rotVel, _cheapInterpolation),
					0);
			}

			// Check if the player falls below -200 on the Y-axis
			if (transform.position.y < -150f)
			{
				RespawnPlayer();
			}
		}

		private void RespawnPlayer()
		{
			if (IsOwner)
			{
				// Create a local copy of the PlayerNetworkData struct
				PlayerNetworkData newData = _netState.Value;

				// Set the position of the local copy to the respawn position
				newData.Position = respawnPosition;

				// Assign the modified local copy back to the Value property
				_netState.Value = newData;

				// Set the player's position to the respawn position
				transform.position = respawnPosition;
			}
		}


		private struct PlayerNetworkData : INetworkSerializable
		{
			private float _x, _z;
			private short _yRot;

			internal Vector3 Position
			{
				get => new Vector3(_x, 0, _z);
				set
				{
					_x = value.x;
					_z = value.z;
				}
			}

			internal Vector3 Rotation
			{
				get => new Vector3(0, _yRot, 0);
				set => _yRot = (short)value.y;
			}

			public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
			{
				serializer.SerializeValue(ref _x);
				serializer.SerializeValue(ref _z);
				serializer.SerializeValue(ref _yRot);
			}
		}
	}
}
