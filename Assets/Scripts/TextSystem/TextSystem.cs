using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TextSystem : MonoBehaviour
{
    public delegate void OnTextShownEventHandler();
    public delegate void OnTextHiddenEventHandler();
    public event OnTextShownEventHandler OnTextShown;
    public event OnTextHiddenEventHandler OnTextHidden;

    public static TextSystem inst;



    public TextHandler stories;

    public TextMeshProUGUI textDisplay;

    private Transform textSource;
    private Vector2 textOffset;
    private Vector3 originPos;

    private bool isFollowingSource = false;
    private Utils.TextBubbleEffects textEffect;

    private bool textHasDuration = false;
    private float textSetTime;
    private float textDuration;

    public int maxDisplayChars = 30;
    private string remainingText;
    private bool lastText = true;

    private bool showKeyIndicator = false;

    private Player player;

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        if (!player)
            Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {

            DisplayText(player.transform, new Vector2(0, .5f), "Player", 0);

        }


        if (textHasDuration && textDuration + textSetTime < Time.time)
        {
            if (lastText)
            {
                HideTextBubble();
            }
            else
                ScrollTextForward();
        }


        if (isFollowingSource)
            BubbleFollowUpdate(textSource.position);
        else
            BubbleFollowUpdate(originPos);

    }

    public void ScrollTextForward()
    {
        int characterAmount = 0;
        if (remainingText.Length < maxDisplayChars)
            characterAmount = remainingText.Length;
        else
            characterAmount = maxDisplayChars - 1 - remainingText.Substring(0, maxDisplayChars).Reverse().ToList().IndexOf(' ');

        textDisplay.text = remainingText.Substring(0, characterAmount);
        remainingText = remainingText.Remove(0, characterAmount);


        if (remainingText.Length == 0)
            lastText = true;
        else
            lastText = false;

        textSetTime = Time.time;
    }

    private void HideTextBubble()
    {
        textDisplay.enabled = false;
        if (showKeyIndicator)
            KeyIndicatorControl.inst.HideIndicator();

        OnTextHidden?.Invoke();
    }
    private void ShowTextBubble()
    {
        textDisplay.enabled = true;
        if (showKeyIndicator)
            KeyIndicatorControl.inst.ShowIndicator(Utils.Iteraction.Interact, gameObject, new Vector2(textDisplay.rectTransform.sizeDelta.x / 2, textDisplay.rectTransform.sizeDelta.y / 2));

        player.DisablePlayerControls(false);
        OnTextShown?.Invoke();
    }

    public void DisplayText(Transform source, Vector2 textOffset, string sourceName, int textID)
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


        this.textOffset = textOffset;
        this.showKeyIndicator = resultObject.showKeyIndicator;
        this.remainingText = resultObject.text;


        // Set if text should follow it's source
        isFollowingSource = resultObject.followSource;
        if (isFollowingSource)
        {
            textSource = source;
        }
        else
            originPos = source.position;

        player.DisablePlayerControls(resultObject.lockPlayerControls);

        ScrollTextForward();
        ShowTextBubble();
    }

    public void HideText() => HideTextBubble();

    private void BubbleFollowUpdate(Vector3 source)
    {
        // some smoothing function here
        var destinationPos = source;

        destinationPos += new Vector3(textOffset.x, textOffset.y, 0);

        destinationPos += new Vector3(0, textDisplay.rectTransform.sizeDelta.y, 0);


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
        transform.position = destinationPos;
    }
}
