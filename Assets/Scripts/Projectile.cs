using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;

    public float damage = 1f; // Set dynamically by dice


    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }
}
