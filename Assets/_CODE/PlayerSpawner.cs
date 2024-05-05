using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : NetworkBehaviour
{
	[SerializeField]
	private GameObject _player;
	[SerializeField]
	private CinemachineFreeLook _playerFreeLook;

	private bool isStarted = false;

	private void Start()
	{
		DontDestroyOnLoad(this.gameObject);
	}

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += NetworkSpawn;
	}

	private void NetworkSpawn( string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut )
	{
		if ( isStarted ) { return; }

		if(IsHost && sceneName == "GamePlay" ){
			foreach (var clientID in clientsCompleted ){
				GameObject player = Instantiate( _player );
				player.GetComponent<NetworkObject>().SpawnAsPlayerObject( clientID, true );
			}
			isStarted = true;
		}
	}
}