using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Test_Mono : MonoBehaviour
{
    [SerializeField] AI_Input_Random ai_Input;
}

[System.Serializable]
public class AI_Input_Random : IDuelerInput
{
    public ParryInput parry;

    /*
    public ButtonRandom skill1;
    public ButtonRandom skill2;
    public ButtonRandom skill3;
    public ButtonRandom skill4;
    public ButtonRandom skill5;
    */
    public DummyButton d = new DummyButton();

    public Vector2 LookInput => Vector2.zero;

    public Vector3 MoveInput => Vector3.zero;
    //public Vector3 MoveInput => GetMoveInput();

    public IButtonState JumpInput => new DummyButton();

    public IButtonState DashInput => new DummyButton();

    IButtonState[] skillInputs;
    public IButtonState[] SkillInputs
    {
        get
        {
            if (skillInputs == null)
            {
                MySubject<Blade_Notification>.AddObserver(parry);
                skillInputs = new IButtonState[] { parry, d, d, d, d, d };
                //skillInputs = new IButtonState[] { parry, skill1, skill2, skill3, skill4, skill5 };
            }
            return skillInputs;
        }
    }
    Vector3 currentMoveInput;
    Vector3 GetMoveInput()
    {
        return currentMoveInput;
    }
}

[System.Serializable]
public class ParryInput : IButtonState, IMyObservable<Blade_Notification>
{
    [SerializeField] float reactionTime = 0.03f;
    [SerializeField] float parryStateTime = 0.1f;
    int reactionTimeInMS => PhotonUtil3D.Sec2MilliSec(reactionTime);
    int parryStateTimeMS => PhotonUtil3D.Sec2MilliSec(parryStateTime);
    float nextPressTime;
    bool press;

    public bool ButtonDown
    {
        get
        {
            // Debug.Log(nextPressTime <= PhotonNetwork.ServerTimestamp);
            if (press && nextPressTime <= PhotonNetwork.ServerTimestamp)
            {
                press = false;
                return true;
            }
            return false;
        }
    }

    public bool Button => false;

    public bool ButtonUp => false;

    public string GetString()
    {
        return "auto";
    }

    public void Notify(Blade_Notification data)
    {
        if (data.Parryable)
        {
            if (data.ReachTime - data.ActivatedTime > reactionTimeInMS)
            {
                press = true;
                nextPressTime = data.ReachTime - parryStateTimeMS;
                // Debug.Log("nextPresstime" + nextPressTime);
                // Debug.Log(PhotonUtil3D.MilliSec2Sec(nextPressTime - PhotonNetwork.ServerTimestamp));
            }
        }
    }
}

[System.Serializable]
public class RandomButtonInput : IButtonState
{
    [SerializeField] float ave = 4f;
    [SerializeField] float sigma = 1f;

    public bool ButtonDown => Lottery();

    public bool Button => false;

    public bool ButtonUp => false;

    public string GetString()
    {
        return "random";
    }
    float CurrentTime => Time.time;
    void SetNextPressTime()
    {
        nextPressTime = CurrentTime +  PhotonUtil3D.StandardDivision(ave, sigma);
    }
    float nextPressTime;
    bool Lottery() {
        return Random.value  < Time.deltaTime;
    }
}

