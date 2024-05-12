using UnityEngine;
using Unity.Netcode;


public class GlobalFunctions : NetworkBehaviour {
   

    public struct CollisionInfo
    {
        public Collider collider;
        public int damage;
        public Vector3 point;
        public float explosionForce;

        public CollisionInfo(Collider collider, Vector3 point, float explosionForce, int damage)
        {
            this.collider = collider;
            this.point = point;
            this.damage = damage;
            this.explosionForce = explosionForce;
        }
    }

    public static void PlayerPushingCollision(CollisionInfo collisionInfo)
    {
        if (collisionInfo.collider != null)
        {
            Collider collider = collisionInfo.collider;
            Vector3 point = collisionInfo.point;
            float explosionForce = collisionInfo.explosionForce;

            CharacterController playerController = collider.GetComponent<CharacterController>();
            if (playerController != null)
            {
                // Calculate the direction away from the collision point
                Vector3 pushDirection = (playerController.transform.position - point).normalized;
                // Apply force to move the player away from the collision point
                playerController.Move(pushDirection * explosionForce * Time.deltaTime);
            }
        }
    }
    public static void ApplyCollisionNetworkRigid(CollisionInfo collisionInfo, ulong netObjectId = 0)
    {
            Collider collider = collisionInfo.collider;
            Vector3 point = collisionInfo.point;
            float explosionForce = collisionInfo.explosionForce;
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
                    Vector3 direction = rb.transform.position - point;

                    // Apply force based on the collision velocity and direction
                    rb.AddForce(direction.normalized * explosionForce, ForceMode.Impulse);
                }
            }
    }
}
