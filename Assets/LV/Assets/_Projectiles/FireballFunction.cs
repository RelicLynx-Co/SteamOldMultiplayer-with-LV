using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class FireballFunction : NetworkBehaviour
{
    [SerializeField] private Transform initialTransform;
    [SerializeField] private List<GameObject> Projectiles;
    private int selectedSpell =404;
    private void Update()
    {
        if (!IsOwner)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            selectedSpell = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedSpell = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedSpell = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedSpell = 4;
        }
        if (selectedSpell!=404)
        {
            RequestFireServerRpc(transform.position, transform.rotation);
            FireSpell(transform.position,transform.rotation);
        }
    }
    [ServerRpc]
    private void RequestFireServerRpc(Vector3 dir, Quaternion rotation)
    {
        RequestFireClientRpc(dir,rotation);
    }

    [ClientRpc]
    private void RequestFireClientRpc(Vector3 dir, Quaternion rotation)
    {
        if (!IsOwner) { FireSpell(dir,rotation); }
    }
    public void FireSpell(Vector3 dir, Quaternion rotation)
    {
        float enemyHeight = GetComponent<Collider>().bounds.size.y;
        Vector3 shootFrom  = dir + rotation * Vector3.forward * 2 + Vector3.up * (enemyHeight / 2f);
        var projectile = Instantiate(Projectiles[selectedSpell], shootFrom, rotation);
        projectile.GetComponent<NetworkObject>().SpawnWithOwnership(NetworkManager.Singleton.LocalClientId);
        selectedSpell = 404;
    }
}
