using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviourPunCallbacks
{
    IDuelerInput input;

    [SerializeField] Transform target;
    //[SerializeField] float sensitivity = 1f;
    [SerializeField] float elevation = 90f;
    [SerializeField] float depression = -90f;

    Vector3 angle;

    public void Init(IDuelerInput input)
    {
        this.input = input;
        angle = target.localEulerAngles;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            angle.y += input.LookInput.x * SaveData.Instance.sensitivity;
            angle.x += -input.LookInput.y * SaveData.Instance.sensitivity;

            angle.x = Mathf.Clamp(angle.x, depression, elevation);

            target.localEulerAngles = angle;
        }
    }
}
