using UnityEngine;
using UnityEngine.UI;

public class UI_CandidateSkill_Mono : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Button button;
    public Button Button => button;
    public void Init(Sprite icon)
    {
        this.icon.sprite = icon;
    }
}