using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LinePrediction
{
    public static Vector3 LinePredictionWithDelay(Vector3 shotOrigin, Vector3 targetPosition, Vector3 targetVelocity, float shotSpeed, float delayInSec)
    {
        return LinePrediction2(shotOrigin, targetPosition + targetVelocity * delayInSec, targetVelocity, shotSpeed);
    }

    //線形予測射撃改良案
    public static Vector3 LinePrediction2(Vector3 shotPosition, Vector3 targetPosition, Vector3 targetVelecity, float bulletSpeed)
    {
        //Unityの物理はm/sなのでm/flameにする
        Vector3 v3_Mv = targetVelecity;
        Vector3 v3_Pos = targetPosition - shotPosition;

        float A = (v3_Mv.x * v3_Mv.x + v3_Mv.y * v3_Mv.y + v3_Mv.z * v3_Mv.z) - bulletSpeed * bulletSpeed;
        float B = 2 * (v3_Pos.x * v3_Mv.x + v3_Pos.y * v3_Mv.y + v3_Pos.z * v3_Mv.z);
        float C = (v3_Pos.x * v3_Pos.x + v3_Pos.y * v3_Pos.y + v3_Pos.z * v3_Pos.z);

        //0割禁止
        if (A == 0 && B == 0) return targetPosition;
        if (A == 0) return targetPosition + v3_Mv * (-C / B / 2);

        //虚数解はどうせ当たらないので絶対値で無視した
        float D = Mathf.Sqrt(Mathf.Abs(B * B - A * C));
        return targetPosition + v3_Mv * PlusMin((-B - D) / A, (-B + D) / A);
    }
    //プラスの最小値を返す(両方マイナスなら0)
    public static float PlusMin(float a, float b)
    {
        if (a < 0 && b < 0) return 0;
        if (a < 0) return b;
        if (b < 0) return a;
        return a < b ? a : b;
    }
}
