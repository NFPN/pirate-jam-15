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
            
        };


        heartStates.ForEach(x => heartSprites.Add(x.state, x.sprite));

        


        var heart = Instantiate(heartPrefab, healthBar);
        var hControl = heart.GetComponent<HeartVisualControl>();
        hControl.SetSprites(heartSprites);
        hearts.Add(hControl);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            state = !state;
            hearts[0].ChangeHeart(Utils.HeartState.Full);
        }   
    }
}
