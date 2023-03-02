using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncObject_Mono : MonoBehaviour
{
    public List<SyncObject_Mono> SyncObjects => syncObjects;
    List<SyncObject_Mono> syncObjects = new List<SyncObject_Mono>();

    public int SyncObjID { get; private set; }
    public int OwnerID { get; private set; }

    public bool Equals(int syncObjID, int ownerID) => syncObjID == SyncObjID && ownerID == OwnerID;

    public void Init(int syncID, int ownerID)
    {
        SyncObjID = syncID;
        OwnerID = ownerID;

        syncObjects.Add(this);
    }

    public void OnDestroy()
    {
        syncObjects.Remove(this);
    }
}
