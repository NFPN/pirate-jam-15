using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TextSystem : MonoBehaviour
{
    public static TextSystem inst;

    public TextHandler stories;

    public TextMeshProUGUI textDisplay;

    private Transform textSource;
    private Vector3 originPos;
    private bool isFollowingSource = false;
    private Utils.TextBubbleEffects textEffect;

    private bool textHasDuration = false;
    private float textSetTime;
    private float textDuration;

    public int maxDisplayChars = 30;
    private bool lastText;


    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(this);
    }

    public void DisplayText()
    {

    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            var player = FindObjectOfType<Player>();
            if (player)
            {
                DisplayText(player.transform, "Player", 0);
            }
        }

        if (textHasDuration && textDuration + textSetTime < Time.time)
            HideTextBubble();


        if (isFollowingSource)
            BubbleFollowUpdate(textSource.position);
        else
            BubbleFollowUpdate(originPos);

    }

    private void HideTextBubble()
    {
        textDisplay.enabled = false;
    }
    private void ShowTextBubble()
    {
        textDisplay.enabled = true;
    }

    public void DisplayText(Transform source, string sourceName, int textID)
    {
        var objectTexts = stories.resources.Find(x => x.Name.ToLower() == sourceName.ToLower());
        if (objectTexts == null)
            return;
        var resultObject = objectTexts.texts.Find(x => x.ID == textID);

        if (resultObject == null)
            return;

        // Set for how long the text is visible
        textHasDuration = resultObject.hasDuration;
        textSetTime = Time.time;
        textDuration = resultObject.textDuration;

        // Add effect
        textEffect = resultObject.effect;

        textDisplay.text = resultObject.text;


        // Set if text should follow it's source
        isFollowingSource = resultObject.followSource;
        if (isFollowingSource)
        {
            textSource = source;
        }
        else
            originPos = source.position;


        ShowTextBubble();
    }

    public void HideText() => HideTextBubble();

    private void BubbleFollowUpdate(Vector3 source)
    {
        // some smoothing function here
        var destinationPos = source;
        destinationPos.y += textDisplay.rectTransform.sizeDelta.y;


        switch (textEffect)
        {
            case Utils.TextBubbleEffects.None:
                break;
            case Utils.TextBubbleEffects.SinUpDown:
                destinationPos.y += Mathf.Sin(Time.time * 2) * 0.2f;
                break;
            default:
                break;
        }
        textDisplay.transform.position = destinationPos;
    }
}
