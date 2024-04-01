using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticles : MonoBehaviour
{

    [SerializeField] private LayerMask layer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (layer == (layer | (1 << collision.gameObject.layer)))
            Destroy(collision.gameObject);
    }
}
