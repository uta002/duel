using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PhotonUtil3D
{
    public static float MilliSec2Sec(int milliSec) => milliSec * 0.001f;
    public static int Sec2MilliSec(float sec) => (int)(sec * 1000);

    public static Vector3 Direction2Degree(Vector3 direction)
    {
        return Quaternion.LookRotation(direction).eulerAngles;
    }

    public static Vector3 Degree2Direction(Vector3 degree) => Degree2Direction(degree.x, degree.y);

    public static Vector3 Degree2Direction(float x, float y)
    {
        return Quaternion.Euler(x, y, 0f) * Vector3.forward;
    }

    public static float StandardDivision(float ave, float sigma)
    {
        return sigma * Mathf.Sqrt(-2.0f * Mathf.Log(Random.value)) * Mathf.Cos(2.0f * Mathf.PI * Random.value) + ave;
    }

    public static float ElapsedTime(int timestamp)
    {
        return MilliSec2Sec(unchecked(PhotonNetwork.ServerTimestamp - timestamp));
    }

    public static float ElapsedTime(int startTime, int endTime)
    {
        return MilliSec2Sec(unchecked(endTime - startTime));
    }

}
