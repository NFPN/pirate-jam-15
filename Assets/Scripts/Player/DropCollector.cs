using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCollector : MonoBehaviour
{

    public float distanceToPlayer = 0.3f;
    private InventoryControl inventory;

    private void Start()
    {
        inventory = InventoryControl.inst;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.TryGetComponent<Drop>(out var drop);
        if (!drop)
            return;
        drop.CollectDrop(this, distanceToPlayer);

    }

    public void ProcessDrop(Drop drop)
    {
        if (drop.dropItem == Utils.Items.Shard)
            inventory.AddShards(1);
        else
            inventory.AddItem(drop.dropItem);

        drop.DestroyDrop();
    }
}
