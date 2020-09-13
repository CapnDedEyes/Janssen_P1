using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody projectile;

    [SerializeField] float firingSpeed;

    private void Awake()
    {
        projectile = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        projectile.AddForce(transform.forward * firingSpeed);
    }
}
