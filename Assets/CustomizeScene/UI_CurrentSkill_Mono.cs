using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_CurrentSkill_Mono : MonoBehaviour
{
    public int SkillID { get; set; }
    [SerializeField] Image icon;
    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI buttonText;
    public Button Button => button;
    [SerializeField] GameObject selectFrame;

    public void Init(int skillId, Sprite icon, string buttonText)
    {
        this.icon.sprite = icon;
        SkillID = skillId;
        this.buttonText.text = buttonText;
    }

    public void Deselect()
    {
        selectFrame.SetActive(false);
    }

    public void Select()
    {
        selectFrame.SetActive(true);
    }
}
