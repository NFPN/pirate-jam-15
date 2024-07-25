using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryTrigger : MonoBehaviour
{
    public string storyName;
    public int id;
    public bool isOnPlayer;
    public Vector2 offset;
    public bool isDestroyedAfterText;

    [Header("true => Save data required")]
    public bool resetOnSceneEnter;

    private bool canTrigger = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && canTrigger)
        {
            canTrigger = false;
            TextSystem.inst.DisplayText(isOnPlayer ? null : gameObject, offset, storyName, id, isDestroyedAfterText ? DestroyObject : null);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && canTrigger)
        {
            canTrigger = false; 
            TextSystem.inst.DisplayText(isOnPlayer ? null : gameObject, offset, storyName, id, isDestroyedAfterText ? DestroyObject : null);
        }
    }

    private void DestroyObject()
    {
        if (!resetOnSceneEnter)
            DataControl.inst.AddUsedObject(gameObject);
        
        canTrigger = false;
        Destroy(gameObject);   
    }
}
