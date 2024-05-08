using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class SpelllMovement : MonoBehaviour
{
    public float fireballSpeed = 5f; // Speed of the fireball
    public float selfDestructDelay = 5f;
    private Rigidbody rb;
    private Vector3 initialDirection;

    void Start()
{
    rb = GetComponent<Rigidbody>();
    initialDirection = rb.transform.forward.normalized;
    if (rb == null)
        Debug.LogError("NetworkRigidbody not found on Fireball GameObject");

    StartCoroutine(SelfDestructCoroutine());
}

    void Update()
    {
        if (rb != null)
        {
            rb.velocity = initialDirection * fireballSpeed;
        }
        else
        {
            Debug.LogError("NetworkRigidbody not found!");
        }
    }
    private IEnumerator SelfDestructCoroutine()
    {
        yield return new WaitForSeconds(selfDestructDelay);
        Destroy(gameObject);
    }
}
