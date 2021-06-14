using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Respawn : MonoBehaviour
{
    public void RespawnPlayer()
    {
        //if (Mission.instance != null)
        //    Mission.instance.OnPlayerContinue();
        StartCoroutine(RespawnWait());
    }
    private IEnumerator RespawnWait()
    {
        yield return new WaitForSeconds(2f);
        //if (Checkpoint.currentlyActiveCheckpoint == null)
        //{
            SceneTransitionController.instance.LoadScene(SceneManager.GetActiveScene().name);/* SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);*/
        //}
        //else
        //{
        //    //transform.position = gm.lastCheckpointPos;
        //    SceneTransitionController.instance.LoadScene(SceneManager.GetActiveScene().name);
        //    //if (PlayerRespawnedFromCheckpoint)
        //    //{

        //    //}
        //}
    }
}
