using System.Collections;
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

    private List<HeartVisualControl> hearts = new();

    bool state = false;

    // Start is called before the first frame update
    void Start()
    {
        var player = FindFirstObjectByType<Player>();
        if (!player)
        {
            print("Player not found!");
            Destroy(this);
        }

        player.OnHealthChanged += (sender, oldHealth, newHealth) =>
        {
            UpdateHearts(newHealth);
        };


        heartStates.ForEach(x => heartSprites.Add(x.state, x.sprite));

        UpdateHearts(player.GetComponent<IHealth>().CurrentHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            FindAnyObjectByType<Player>().GetComponent<IHealth>().DealDamage(this, -1);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            FindAnyObjectByType<Player>().GetComponent<IHealth>().DealDamage(this, 1);
        }
    }

    private void UpdateHearts(float health)
    {

        var heartCount = (int)health;
        var imageCount = (int)Mathf.Ceil(heartCount / 2.0f);

        var addHeartAmount = imageCount - hearts.Count;
        for (int i = 0; i < addHeartAmount; i++)
            CreateNewHeart();

        foreach (var heart in hearts)
        {
            if(heartCount > 1)
            {
                heart.ChangeHeart(Utils.HeartState.Full);
                heartCount -= 2;
            }
            else if (heartCount == 1)
            {
                heart.ChangeHeart(Utils.HeartState.Half);
                heartCount--;
            }
            else
            {
                heart.ChangeHeart(Utils.HeartState.None);
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
