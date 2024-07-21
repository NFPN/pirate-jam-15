using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TextData", menuName = "ScriptableObjects/TextScriptableObject", order = 1)]
public class TextHandler : ScriptableObject
{
    public List<TextObject> resources;
}

[System.Serializable]
public class TextObject
{
    public string Name;
    public List<TextLine> texts;
}

[System.Serializable]
public class TextLine
{
    public int ID;
    public string text;
    public bool followSource;
    public bool hasDuration = true;
    public bool showKeyIndicator = false;
    public bool lockPlayerControls = false;
    public float textDuration = 5;
    public Utils.TextBubbleEffects effect;
}
