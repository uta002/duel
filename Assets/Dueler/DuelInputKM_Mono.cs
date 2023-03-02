using UnityEngine;

public class DuelInputKM_Mono : MonoBehaviour, IDuelerInput {
    public KeybordButton jump;
    public KeybordButton dash;

    public KeybordButton attack0;
    public KeybordButton attack1;
    public KeybordButton attack2;
    public KeybordButton attack3;
    public KeybordButton attack4;
    public KeybordButton attack5;

    IButtonState[] attackButtons;
    private void Awake()
    {
        attackButtons = new IButtonState[]{ attack0, attack1, attack2, attack3, attack4, attack5 };
    }

    public Vector2 LookInput => new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    public Vector3 MoveInput => new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

    public IButtonState JumpInput => jump;

    public IButtonState DashInput => dash;

    public IButtonState[] SkillInputs => attackButtons;

    public IButtonState GetSkillInput(int num) => attackButtons[num % attackButtons.Length];

}

public interface IButtonState
{
    bool ButtonDown { get; }
    bool Button { get; }
    bool ButtonUp { get; }

    string GetString();
}

[System.Serializable]
public class DummyButton : IButtonState
{
    public bool ButtonDown => false;

    public bool Button => false;

    public bool ButtonUp => false;

    public string GetString()
    {
        return "Dummy";
    }
}
[System.Serializable]
public class KeybordButton : IButtonState
{
    [SerializeField] KeyCode keyCode;

    public KeybordButton(KeyCode keyCode)
    {
        this.keyCode = keyCode;
    }

    public bool ButtonDown => Input.GetKeyDown(keyCode);

    public bool Button => Input.GetKey(keyCode);

    public bool ButtonUp => Input.GetKeyUp(keyCode);

    public string GetString()
    {
        return keyCode.ToString();
    }
}
[System.Serializable]
public class MouseButton : IButtonState
{
    [SerializeField] int buttonNum;

    public MouseButton(int buttonNum)
    {
        this.buttonNum = buttonNum;
    }

    public bool ButtonDown => Input.GetMouseButtonDown(buttonNum);

    public bool Button => Input.GetMouseButton(buttonNum);

    public bool ButtonUp => Input.GetMouseButtonUp(buttonNum);

    public string GetString()
    {
        return "M" + buttonNum;
    }
}
