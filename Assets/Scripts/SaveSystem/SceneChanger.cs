using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SceneChanger : MonoBehaviour
{
    public string teleporterName;
    public string sceneName;
    public string targetTeleport;

    public bool isLocalTeleport = false;
    public bool canTeleport = true;

    private bool ignoreEnter = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canTeleport)
            return;

        if (collision.CompareTag("Player"))
        {
            if (ignoreEnter)
            {
                ignoreEnter = false;
                return;
            }
            if (isLocalTeleport)
                DataControl.inst.TeleportPlayerToLocation(targetTeleport);
            else
                DataControl.inst.ChangeScene(sceneName, targetTeleport);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!canTeleport)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            if (ignoreEnter)
            {
                ignoreEnter = false;
                return;
            }
            if (isLocalTeleport)
                DataControl.inst.TeleportPlayerToLocation(targetTeleport);
            else
                DataControl.inst.ChangeScene(sceneName, targetTeleport);
        }
    }

    public void IgnoreFirstEnter()
    {
        ignoreEnter = true;
    }
}
