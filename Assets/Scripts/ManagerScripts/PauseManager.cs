using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    //GameManager GM;
    public static PauseManager pauseManager;
    public static bool IsGamePaused = false;
    [SerializeField] string sceneToLoad;
    [SerializeField] private GameObject pauseMenuUI, resultsMenuUI;
    [SerializeField] private string[] missionResults;
    public TextMeshProUGUI[] pointsText;

    [SerializeField] private GameObject[] skillList;
    [SerializeField] private Sprite[] spriteList;
    [SerializeField] private Image eyeCon;
    private void Start()
    {
        pauseManager = this;
        //GM = GameManager.instance;
    }
    public void PauseButtonPressed()
    {
        if (IsGamePaused)
            Resume();
        else
            Pause();
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        IsGamePaused = false;
    }
    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale=0f;
        IsGamePaused = true;
    }
    public void LoadMenu()
    {
        //IsGamePaused = false;
        //Time.timeScale = 1f;
        //Destroy(Mission.instance.gameObject);
        //MusicManager.instance.StopMusic();
        //GM.RestoreCheckpointStart();
        //SceneTransitionController.instance.LoadScene(sceneToLoad);
    }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
