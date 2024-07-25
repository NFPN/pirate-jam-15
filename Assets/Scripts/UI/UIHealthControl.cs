using System.Collections.Generic;
using UnityEngine;

public class UIHealthControl : MonoBehaviour
{
    [System.Serializable]
    public struct HeartState
    {
        public Utils.HeartState state;
        public Sprite sprite;
    }

    public GameObject heartPrefab;
    public Transform healthBar;

    public List<HeartState> heartStates;

    private Dictionary<Utils.HeartState, Sprite> heartSprites = new();

    private readonly List<HeartVisualControl> hearts = new();

    private IHealth playerHealth;

    //private bool state = false;

    // Start is called before the first frame update
    private void Start()
    {
        var player = FindFirstObjectByType<Player>();
        if (!player)
        {
            print("Player not found!");
            Destroy(this);
        }

        player.OnHealthChanged += (sender, oldHealth, newHealth) =>
        {
            if (hearts.Count == 0)
                UpdateHearts(newHealth, false);
            else
                UpdateHearts(newHealth);
        };

        heartStates.ForEach(x => heartSprites.Add(x.state, x.sprite));

        playerHealth = player.GetComponent<IHealth>();

        WorldShaderControl.inst.OnWorldChangeBegin += OnChangeToShadow;
    }

    private void OnChangeToShadow(bool state)
    {
        UpdateHearts(playerHealth.CurrentHealth, true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            playerHealth.DealDamage(this, -1);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            playerHealth.DealDamage(this, 1);
        }
    }

    private void UpdateHearts(float health, bool doAnimation = true)
    {
        var heartCount = (int)health;
        var imageCount = (int)Mathf.Ceil(heartCount / 2.0f);

        var addHeartAmount = imageCount - hearts.Count;
        for (int i = 0; i < addHeartAmount; i++)
            CreateNewHeart();

        foreach (var heart in hearts)
        {
            if (heartCount > 1)
            {
                if (WorldShaderControl.inst.IsShadowWorld)
                    heart.ChangeHeart(Utils.HeartState.FullShadow, doAnimation);
                else
                    heart.ChangeHeart(Utils.HeartState.Full, doAnimation);
                heartCount -= 2;
            }
            else if (heartCount == 1)
            {
                if (WorldShaderControl.inst.IsShadowWorld)
                    heart.ChangeHeart(Utils.HeartState.HalfShadow, doAnimation);
                else
                    heart.ChangeHeart(Utils.HeartState.Half, doAnimation);
                heartCount--;
            }
            else
            {
                heart.ChangeHeart(Utils.HeartState.None, doAnimation);
            }
        }
    }

    private void CreateNewHeart()
    {
        var heart = Instantiate(heartPrefab, healthBar);
        var hControl = heart.GetComponent<HeartVisualControl>();
        hControl.SetSprites(heartSprites);
        hearts.Add(hControl);
    }
}
