using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TestDuelerSpawner_Mono : MonoBehaviour
{
    public string prefabName;
    public KeyCode keyCode;

    void Update()
    {
        if (Input.GetKeyDown(keyCode))
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        float range = 5f;
        var pos = new Vector3(Random.Range(-range, range), 5f, Random.Range(-range, range));

        PhotonNetwork.Instantiate(prefabName, pos, Quaternion.identity);
    }
}
