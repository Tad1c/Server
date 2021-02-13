using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePhysics : MonoBehaviour
{
    public float forceValue = 50f;

    void Start()
    {
        GetComponent<Rigidbody>().AddForce(new Vector3(0f, 0f, 1f) * forceValue);
    }

}
