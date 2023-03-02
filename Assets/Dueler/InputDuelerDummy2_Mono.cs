using UnityEngine;

public class InputDuelerDummy2_Mono : MonoBehaviour, IDuelerInput
{
    public Vector2 look;
    public Vector3 move;
    public ButtonRandom jump;
    public ButtonRandom dash;
    public ButtonRandom skill0;
    public ButtonRandom skill1;
    public ButtonRandom skill2;
    public ButtonRandom skill3;
    public Vector2 LookInput => look;
    public Vector3 MoveInput => move;

    public IButtonState JumpInput => jump;


    public IButtonState DashInput => dash;

    public IButtonState[] SkillInputs => throw new System.NotImplementedException();

    public IButtonState GetSkillInput(int num)
    {
        switch (num)
        {
            case 0:
                return skill0;
            case 1:
                return skill1;
            case 2:
                return skill2;
            case 3:
                return skill3;
            default:
                return new DummyButton();
        }
        
    }

    

    [System.Serializable]
    public class ButtonRandom : IButtonState
    {
        public float prob = 0.1f;
        public bool ButtonDown => Lottery(prob);

        public bool Button => false;

        public bool ButtonUp => false;

        public string GetString()
        {
            return "random";
        }

        bool Lottery(float prob) =>Random.value < prob * Time.deltaTime;
    }
}
