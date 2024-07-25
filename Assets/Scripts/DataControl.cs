using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataControl : MonoBehaviour
{
    public static DataControl inst;
    private float playerHealth;

    public float playerHealthMax = 6;

    public float deathAnimationPause = 5f;

    private float deathAnimationTime;

    [Header("Scene Change")]
    public float freezeTime = 2;

    private bool sceneLoading = false;
    private bool deathReload = false;

    private bool isShadowWorld = false;

    public bool IsShadowWorld { get { return isShadowWorld; } }

    private Player player;
    private WorldShaderControl shaderControl;

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(gameObject);

            playerHealth = playerHealthMax;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
            Destroy(gameObject);
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        print("scene loaded");
        UnsubscirbeEvents();

        FindObjectsInScene();

        SubscribeEvents();

        sceneLoading = false;

        if (deathReload)
            StartCoroutine(DeathAnimation());
        else
            StartCoroutine(SceneEnter());
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            ChangeScene("SampleScene2");
        }

    }


    public void ChangeScene(string name)
    {
        shaderControl.SceneLeave();
        deathReload = false;

        StartCoroutine(LoadScene(name));
    }

    private void ReloadScene()
    {
        var curScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadSceneAsync(curScene);
        sceneLoading = true;
        deathReload = true;
    }


    IEnumerator LoadScene(string name)
    {
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(name);
        asyncOp.allowSceneActivation = false;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(freezeTime);
        while (!asyncOp.isDone && asyncOp.progress < 0.9f)
        {
            yield return new WaitForSecondsRealtime(0.01f);
        }
        asyncOp.allowSceneActivation = true;
        Time.timeScale = 1;
    }
    IEnumerator DeathAnimation()
    {

        player.DisablePlayerControls(true);
        deathAnimationTime = Time.time;

        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(deathAnimationPause);

        Time.timeScale = 1;

        player.DisablePlayerControls(false);
    }

    IEnumerator SceneEnter()
    {
        player.DisablePlayerControls(true);
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(freezeTime);

        Time.timeScale = 1;
        player.DisablePlayerControls(false);
    }

    private void FindObjectsInScene()
    {
        player = FindObjectOfType<Player>();
        shaderControl = WorldShaderControl.inst;
    }

    private void UnsubscirbeEvents()
    {
        if (player)
        {
            player.OnHealthChanged -= OnPlayerHealthChanged;
            player.OnDeath -= OnPlayerDeath;
        }

        if(shaderControl)
            shaderControl.OnWorldChangeBegin -= OnWorldChange;

    }

    private void SubscribeEvents()
    {
        if (player)
        {
            player.OnHealthChanged += OnPlayerHealthChanged;
            player.OnDeath += OnPlayerDeath;
        }

        if (shaderControl)
        {
            shaderControl.OnWorldChangeBegin += OnWorldChange;
        }
    }

    public bool IsReloadOnDeath()
    {
        return deathReload;
    }


    private void OnWorldChange()
    {

    }

    private void OnWorldChange(bool isShadow) => isShadowWorld = isShadow;

    #region Player
    private void OnPlayerDeath(object source)
    {
        playerHealth = playerHealthMax;
        ReloadScene();
    }

    private void OnPlayerHealthChanged(object source, float oldHealth, float newHealth)
    {
        playerHealth = newHealth;
    }



    // TODO: connect player to data control (so that player health does not reset every scene and is managed from here)
    public void UpdatePlayerHealth(float playerHealth)
    {
        this.playerHealth = playerHealth;
    }

    public void AddPlayerMaxHealth(float healthAmount)
    {
        playerHealthMax += healthAmount;
    }
    
    public float GetCurrentPlayerHealth()
    {
        return playerHealth;
    }
    #endregion
}
