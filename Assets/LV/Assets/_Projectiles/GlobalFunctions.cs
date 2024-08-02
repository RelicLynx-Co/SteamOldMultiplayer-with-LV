using UnityEngine;
using Unity.Netcode;

using System.Collections;
using System.Reflection;
public static class GlobalFunctions {
   

    public struct CollisionInfo
    {
        public Collider collider;
        // public int damage;
        public Vector3 point;
        public float explosionForce;

        public CollisionInfo(Collider collider, Vector3 point, float explosionForce)
        {
            this.collider = collider;
            this.point = point;
            // this.damage = damage;
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
                Vector3 force = pushDirection * explosionForce;
                StartCoroutine(ApplyForceOverTime(playerController, force));
                // playerController.Move(pushDirection * explosionForce * Time.deltaTime);
            }
        }
    }
    private IEnumerator ApplyForceOverTime(CharacterController controller, Vector3 force)
{
    float duration = 0.5f; // Duration to apply the force
    float elapsedTime = 0f;

    while (elapsedTime < duration)
    {
        // Move the player in the push direction
        controller.Move(force * Time.deltaTime);
        elapsedTime += Time.deltaTime;
        yield return null;
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
