using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubespawner : MonoBehaviour
{
    ObjectPooler objectPooler;
    private void Start()
    {
        objectPooler = ObjectPooler.Instance;
    }
    void FixedUpdate()
    {
        objectPooler.SpawnFromPool("cube", transform.position, Quaternion.identity);
    }
}
