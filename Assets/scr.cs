using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr: MonoBehaviour
{
    public Transform[] spawnloc;
    public GameObject spawnprefab;
    public GameObject spawnclone;
    void spawn()
    {
        spawnclone = Instantiate(spawnprefab, spawnloc[0].transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
    }  

    
}
