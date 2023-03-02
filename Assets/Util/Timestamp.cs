using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timestamp
{
    protected int timestamp;

    public Timestamp()
    {

    }
    public Timestamp(int timestamp)
    {
        this.timestamp = timestamp;
    }

    public float TimeElapsed => Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - timestamp) / 1000f);
    public bool CheckTimeElapsedBetween(float min, float max) => min <= TimeElapsed && TimeElapsed <= max;

    public void Init(int timestamp)
    {
        this.timestamp = timestamp;
    }

    public void Init()
    {
        timestamp = PhotonNetwork.ServerTimestamp;
    }
}
