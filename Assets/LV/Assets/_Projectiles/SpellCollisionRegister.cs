using UnityEngine;
using Unity.Netcode;
using System;
using System.Reflection;
public class SpellCollisionRegister : NetworkBehaviour

{
    [Header("Spell Parameters")]
    public float explosionForce = 100f;
    public float explosionRadius = 3f;
    [Header("Enemy Collision Actions")]
    [SerializeField] private string enemyCollisionFunctionName;
    [Header("Player Collision Actions")]
    [SerializeField] private string playerCollisionFunctionName;

private GlobalFunctions.CollisionInfo collisionInfo;
    
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
                 collisionInfo = new GlobalFunctions.CollisionInfo(collider, point, explosionForce);
            if (collider.CompareTag("Player"))
            {
                if (!string.IsNullOrEmpty(playerCollisionFunctionName))
                {
                    MethodInfo method = typeof(GlobalFunctions).GetMethod(playerCollisionFunctionName);
                    if (method != null)
                    {
                        method.Invoke(null, new object[] { collisionInfo });
                    }
                }
            }
            else if(collider.CompareTag("Enemy"))
            {

                NetworkObject netObject = collider.GetComponent<NetworkObject>();
                if (netObject != null)
                {
                    ApplyCollisionServerRpc( netObject.NetworkObjectId);
                }
                else
                {
                     MethodInfo method = typeof(GlobalFunctions).GetMethod(enemyCollisionFunctionName);
                    if (method != null)
                    {
                        method.Invoke(null, new object[] { collisionInfo, netObject.NetworkObjectId });
                    }
                }
            }
            else {
               		Debug.Log( collider.gameObject.tag);

            }


        }
    }

    [ServerRpc]
    private void ApplyCollisionServerRpc(ulong netObjectId)
    {
        ApplyCollisionClientRpc(netObjectId);
    }

    [ClientRpc]
    private void ApplyCollisionClientRpc( ulong netObjectId)
    {
         MethodInfo method = typeof(GlobalFunctions).GetMethod(enemyCollisionFunctionName);
                    if (method != null)
                    {
                        method.Invoke(null, new object[] { collisionInfo, netObjectId});
                    }
    }
    


}
