using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene_Mono : MonoBehaviour
{
    public void LoadScene(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }

    public void LoadCustomizeScene() => LoadScene("CustomizeScene");
    public void LoadOnlineSelectScene() => LoadScene("OnlineSelectScene");
    public void LoadMenuScene() => LoadScene("MenuScene");
    public void LoadSampleScene() => LoadScene("SampleScene");
}
