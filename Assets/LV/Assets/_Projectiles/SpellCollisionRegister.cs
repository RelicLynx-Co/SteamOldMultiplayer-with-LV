using UnityEngine;
using Unity.Netcode;

public class SpellCollisionRegister : NetworkBehaviour
{
    public float explosionForce = 100f;
    public float explosionRadius = 3f;

    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            CheckCollisions(collision.contacts[0].point, collision.relativeVelocity);
            Destroy(gameObject);
        }
    }

    private void CheckCollisions(Vector3 point, Vector3 velocity)
    {
        Collider[] colliders = Physics.OverlapSphere(point, explosionRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                CharacterController playerController = collider.GetComponent<CharacterController>();
                if (playerController != null)
                {
                    // Calculate the direction away from the collision point
                    Vector3 pushDirection = (playerController.transform.position - point).normalized;
                    // Apply force to move the player away from the collision point
                    playerController.Move(pushDirection * explosionForce * Time.deltaTime);
                }
            }
            else
            {

                NetworkObject netObject = collider.GetComponent<NetworkObject>();
                if (netObject != null)
                {
                    ApplyCollisionServerRpc(point, velocity, netObject.NetworkObjectId);
                }
                else
                {
                    ApplyCollision(point, velocity);
                }
            }


        }
    }

    [ServerRpc]
    private void ApplyCollisionServerRpc(Vector3 collisionPoint, Vector3 collisionVelocity, ulong netObjectId)
    {
        ApplyCollisionClientRpc(collisionPoint, collisionVelocity, netObjectId);
    }

    [ClientRpc]
    private void ApplyCollisionClientRpc(Vector3 collisionPoint, Vector3 collisionVelocity, ulong netObjectId)
    {
        ApplyCollision(collisionPoint, collisionVelocity, netObjectId);
    }
    private void ApplyCollision(Vector3 collisionPoint, Vector3 collisionVelocity, ulong netObjectId = 0)
    {
        if (IsServer)
        {
            NetworkObject netObject = null;
            // Retrieve the networked object using the netObjectId
            if (netObjectId != 0)
                netObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[netObjectId];
            if (netObjectId != 0 && netObject != null)
            {
                Rigidbody rb = netObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Calculate the direction from the collision point to the object's center
                    Vector3 direction = rb.transform.position - collisionPoint;

                    // Apply force based on the collision velocity and direction
                    rb.AddForce(direction.normalized * explosionForce, ForceMode.Impulse);
                }
            }

        }
    }


}
