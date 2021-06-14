using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    private AudioManager audio;

    private void Start()
    {
        audio = AudioManager.instance;
    }

    public void ButtonHover()
    {
        audio.PlaySound("ButtonHover");
    }
    
    public void ButtonClick()
    {
        audio.PlaySound("ButtonClick");
        SceneManager.LoadScene("GameScene");
    }
}
