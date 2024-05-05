using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField]
    private GameObject _enemy;

    private Time deltaTimeNow;

    private float timer = 4f;
    public float spawnInterval = 20f; // Adjust this value to set the interval between enemy spawns

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            InstantiateEnemyServerRpc();
            timer = 0f; // Reset the timer
        }
    }
    [ServerRpc]
    private void InstantiateEnemyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        GameObject enemy = Instantiate(_enemy, transform.position, Quaternion.identity);
        enemy.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);

        // Optionally, you can set the position and rotation of the instantiated enemy
    }
}

    