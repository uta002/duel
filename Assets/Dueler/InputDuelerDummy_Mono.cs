using UnityEngine;

public class InputDuelerDummy_Mono : MonoBehaviour, IDuelerInput
{
    public Vector2 LookInput => Vector2.zero;
    public Vector3 MoveInput => Vector3.zero;

    public IButtonState JumpInput => new DummyButton();

    public IButtonState DashInput => new DummyButton();


    public IButtonState GetSkillInput(int num) => new DummyButton();

    IButtonState[] skillInputs;
    public IButtonState[] SkillInputs
    {
        get
        {
            if (skillInputs == null)
            {
                skillInputs = new IButtonState[] { new DummyButton(), new DummyButton(), new DummyButton(), new DummyButton(), new DummyButton(), new DummyButton() };
            }
            return skillInputs;
        }
    }

}

public class InputDuelerDummy : IDuelerInput
{
    public Vector2 LookInput => Vector2.zero;
    public Vector3 MoveInput => Vector3.zero;

    public IButtonState JumpInput => new DummyButton();

    public IButtonState DashInput => new DummyButton();

    public IButtonState GetSkillInput(int num) => new DummyButton();

    IButtonState[] skillInputs;
    public IButtonState[] SkillInputs
    {
        get
        {
            if (skillInputs == null)
            {
                skillInputs = new IButtonState[] { new DummyButton(), new DummyButton(), new DummyButton(), new DummyButton(), new DummyButton(), new DummyButton() };
            }
            return skillInputs;
        }
    }

}
