using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataControl : MonoBehaviour
{
    public Action OnLoaded;
    public Action OnLeaveScene;
    public Action OnActiveSceneChange;

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

    public bool IsShadowWorld
    { get { return isShadowWorld; } }

    public string SceneName { get; private set; }

    public List<SceneChanger> sceneChangers;

    // private Dictionary<int, (string scene, Vector3 position)> itemData;

    // scene and unique id
    private Dictionary<string, List<string>> objectData = new();

    private Player player;
    private WorldShaderControl shaderControl;
    private Vector3 enterLocation;

    private string targetTeleport;

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(gameObject);

            playerHealth = playerHealthMax;

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }
        else
            Destroy(gameObject);

        //TODO: change to the proper index in the build
        if (SceneManager.GetActiveScene().name.ToLower().Contains("begin"))
            isShadowWorld = true;
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        OnActiveSceneChange?.Invoke();
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        SceneName = SceneManager.GetActiveScene().name;

        if (!objectData.ContainsKey(SceneName))
            objectData.Add(SceneName, new());

        if (objectData.ContainsKey(SceneName))
            DestroySceneObjects(objectData[SceneName]);

        UnsubscirbeEvents();

        FindObjectsInScene();

        SubscribeEvents();

        sceneChangers = FindObjectsByType<SceneChanger>(FindObjectsSortMode.None).ToList();

        print("hi");
        if (targetTeleport != null)
            TeleportPlayerToLocation(targetTeleport);

        if (deathReload)
            StartCoroutine(DeathAnimation());
        else
            StartCoroutine(SceneEnter());

        OnLoaded?.Invoke();
    }

    // Start is called before the first frame update
    private void Start()
    {
        OnLoaded?.Invoke();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ChangeScene("SampleScene2", "testTeleport");
        }
    }

    public void TeleportPlayerToLocation(string targetTP)
    {
        if (sceneChangers.Count == 0)
            return;
        var location = sceneChangers.First(x => x.teleporterName == targetTP);
        location.IgnoreFirstEnter();
        if (location != null)
            player.Teleport(location.transform.position);
    }

    public void AddUsedObject(GameObject obj)
    {
        obj.TryGetComponent<ObjectSaveData>(out var data);
        if (!data)
            return;

        if (!objectData[SceneName].Contains(data.ID))
            objectData[SceneName].Add(data.ID);
    }

    public void ChangeScene(string name, string targetTeleport = null)
    {
        if (sceneLoading)
            return;
        if (SceneManager.GetSceneByName(name) == null)
        {
            print($"scene {name} not found");
            return;
        }
        OnLeaveScene?.Invoke();
        sceneLoading = true;
        if (shaderControl)
            shaderControl.SceneLeave();
        deathReload = false;
        this.targetTeleport = targetTeleport;
        StartCoroutine(LoadScene(name));
    }

    public void ChangeScene(string name)
    {
        if (sceneLoading)
            return;
        if (SceneManager.GetSceneByName(name) == null)
        {
            print($"scene {name} not found");
            return;
        }
        OnLeaveScene?.Invoke();
        sceneLoading = true;
        if (shaderControl)
            shaderControl.SceneLeave();
        deathReload = false;
        this.targetTeleport = null;
        StartCoroutine(LoadScene(name));
    }

    private void DestroySceneObjects(List<string> objectIDs)
    {
        var trackedObjects = FindObjectsOfType<ObjectSaveData>();
        var objectsToDestroy = trackedObjects.Where(x => objectIDs.Contains(x.ID));

        objectsToDestroy.ToList().ForEach(x => Destroy(x.gameObject));
    }

    private void ReloadScene()
    {
        var curScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadSceneAsync(curScene);
        OnLeaveScene?.Invoke();
        sceneLoading = true;
        deathReload = true;

        if (targetTeleport != null)
            TeleportPlayerToLocation(targetTeleport);
    }

    private IEnumerator LoadScene(string name)
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

    private IEnumerator DeathAnimation()
    {
        if (player)
            player.DisablePlayerControls(true);
        deathAnimationTime = Time.time;

        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(deathAnimationPause);

        Time.timeScale = 1;

        if (player)
            player.DisablePlayerControls(false);

        sceneLoading = false;
    }

    private IEnumerator SceneEnter()
    {
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(freezeTime);

        Time.timeScale = 1;

        sceneLoading = false;
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

        if (shaderControl)
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

    #endregion Player
}
