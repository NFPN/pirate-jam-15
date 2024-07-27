using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioInitialize : MonoBehaviour
{

    public float artificialDelay = 3;
    private bool loadingScene = false;
    // Update is called once per frame
    void Update()
    {
        if (loadingScene)
            return;

        if (FMODUnity.RuntimeManager.HasBankLoaded("Master"))
        {
            loadingScene = true;
            StartCoroutine(LoadScene());
        }
    }

    private IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(artificialDelay);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
