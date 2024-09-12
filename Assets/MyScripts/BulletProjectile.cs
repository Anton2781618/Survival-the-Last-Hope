using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody bulletRigidbody;
    [SerializeField] private float speed = 40f;

    private void Start() 
    {

        bulletRigidbody.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other) 
    {
        Destroy(gameObject);
    }
}
