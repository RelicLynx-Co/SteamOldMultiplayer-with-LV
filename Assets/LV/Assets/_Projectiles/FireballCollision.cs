using UnityEngine;
using Unity.Netcode;

public class FireballCollision : NetworkBehaviour
{
    public float explosionForce = 100f;
    public float explosionRadius = 3f;
    private NetworkSpawnManager _spawnManager;
    private void Start()
    {
        _spawnManager = NetworkManager.Singleton.SpawnManager;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            PushObjects(collision.contacts[0].point);
            Destroy(gameObject);
        }
    }

    private void PushObjects(Vector3 explosionPosition)
    {
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);

        foreach (Collider collider in colliders)
        {
            string layerName = LayerMask.LayerToName(collider.gameObject.layer);
            if (layerName != "Enemy" && layerName != "Player")
            { continue;}

            Rigidbody rb = collider.attachedRigidbody;

            if (rb != null)
            {
                Vector3 direction = collider.transform.position - explosionPosition;

                // Apply explosion force to the object locally
                rb.AddForce(direction.normalized * explosionForce, ForceMode.Impulse);

                // Check if the object is networked
                NetworkObject netObject = collider.GetComponent<NetworkObject>();
                if (netObject != null && (netObject.IsPlayerObject || netObject.IsOwnedByServer))
                {
                    // If the object is networked, replicate the force to clients
                    ApplyForceServerRpc(netObject.NetworkObjectId, direction.normalized * explosionForce);
                }
            }
        }
    }

    [ServerRpc]
    private void ApplyForceServerRpc(ulong networkObjectId, Vector3 force)
{
    if (_spawnManager == null)
    {
        _spawnManager = NetworkManager.Singleton.SpawnManager;
    }

    NetworkObject netObj = _spawnManager.SpawnedObjects[networkObjectId];
        Debug.Log("NetworkID: " + networkObjectId);

        if (netObj != null)
    {
        Rigidbody rb = netObj.GetComponent<Rigidbody>();
        UnityEngine.AI.NavMeshAgent navMeshAgent = netObj.GetComponent<UnityEngine.AI.NavMeshAgent>();

        if (rb != null)
        {

                if (navMeshAgent != null)
            {
                navMeshAgent.enabled = false;
            }
                rb.AddForce(force, ForceMode.Impulse);
            if (navMeshAgent != null)
            {
                Invoke(nameof(EnableNavMeshAgent), 0.5f);
            }
        }
    }
}

private void EnableNavMeshAgent()
{
    UnityEngine.AI.NavMeshAgent navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    if (navMeshAgent != null)
    {
        navMeshAgent.enabled = true;
    }
}
}
