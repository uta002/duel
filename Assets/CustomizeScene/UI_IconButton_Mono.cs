using UnityEngine;
using UnityEngine.UI;

public class UI_IconButton_Mono : MonoBehaviour
{
    public Button button;
    public Image icon;

    public void Init(Sprite icon) => this.icon.sprite = icon;
}
