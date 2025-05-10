using System;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected virtual void ItemPickup(Player player)
    {
        MyLogger.Log($"Player picked up a '{this.GetType().Name}'");

        GameManager.Instance?.PopItemFromList(gameObject);
        Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            ItemPickup(player);
        }
    }
}
