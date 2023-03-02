using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScene : MonoBehaviour
{
    public string onlineSelectScene;
    public string customizeScene;
    public void OnlinePressed()
    {
        SceneManager.LoadScene(onlineSelectScene);
    }

    public void CustomizePressed()
    {
        SceneManager.LoadScene(customizeScene);
    }
}
