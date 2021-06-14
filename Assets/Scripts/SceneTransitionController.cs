using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitionController : MonoBehaviour
{
    public static SceneTransitionController instance;

    public string SceneToLoad;
    private Image transitionImage;
    public float transitionSpeed = 2f;
    private bool shouldReveal;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(transform.root.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        transitionImage = GetComponent<Image>();
        shouldReveal = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldReveal)
        {
            transitionImage.material.SetFloat("_Cutoff", Mathf.MoveTowards(transitionImage.material.GetFloat("_Cutoff"), 1.1f, transitionSpeed * Time.deltaTime));
        }
        else
        {
            //LoadScene(SceneToLoad);
            transitionImage.material.SetFloat("_Cutoff", Mathf.MoveTowards(transitionImage.material.GetFloat("_Cutoff"), -0.1f, transitionSpeed * Time.deltaTime));
            if (transitionImage.material.GetFloat("_Cutoff") == -0.1f)
            {
                SceneManager.LoadScene(SceneToLoad);
            }
        }
    }
    void OnApplicationQuit()
    {
        transitionImage.material.SetFloat("_Cutoff", -0.1f);
    }

    public void LoadScene(string sceneToLoad)
    {
        SceneToLoad = sceneToLoad;
        shouldReveal = false;
    }
}
