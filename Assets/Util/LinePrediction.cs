using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LinePrediction
{
    public static Vector3 LinePredictionWithDelay(Vector3 shotOrigin, Vector3 targetPosition, Vector3 targetVelocity, float shotSpeed, float delayInSec)
    {
        return LinePrediction2(shotOrigin, targetPosition + targetVelocity * delayInSec, targetVelocity, shotSpeed);
    }

    //���`�\���ˌ����ǈ�
    public static Vector3 LinePrediction2(Vector3 shotPosition, Vector3 targetPosition, Vector3 targetVelecity, float bulletSpeed)
    {
        //Unity�̕�����m/s�Ȃ̂�m/flame�ɂ���
        Vector3 v3_Mv = targetVelecity;
        Vector3 v3_Pos = targetPosition - shotPosition;

        float A = (v3_Mv.x * v3_Mv.x + v3_Mv.y * v3_Mv.y + v3_Mv.z * v3_Mv.z) - bulletSpeed * bulletSpeed;
        float B = 2 * (v3_Pos.x * v3_Mv.x + v3_Pos.y * v3_Mv.y + v3_Pos.z * v3_Mv.z);
        float C = (v3_Pos.x * v3_Pos.x + v3_Pos.y * v3_Pos.y + v3_Pos.z * v3_Pos.z);

        //0���֎~
        if (A == 0 && B == 0) return targetPosition;
        if (A == 0) return targetPosition + v3_Mv * (-C / B / 2);

        //�������͂ǂ���������Ȃ��̂Ő�Βl�Ŗ�������
        float D = Mathf.Sqrt(Mathf.Abs(B * B - A * C));
        return targetPosition + v3_Mv * PlusMin((-B - D) / A, (-B + D) / A);
    }
    //�v���X�̍ŏ��l��Ԃ�(�����}�C�i�X�Ȃ�0)
    public static float PlusMin(float a, float b)
    {
        if (a < 0 && b < 0) return 0;
        if (a < 0) return b;
        if (b < 0) return a;
        return a < b ? a : b;
    }
}
