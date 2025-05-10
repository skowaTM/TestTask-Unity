using UnityEngine;

public class BoosterItem : Item
{
    [SerializeField, Range(1f, 5f)] private float multiplier = 3f;
    [SerializeField, Range(1f, 5f)] private float duration = 3f;


    protected override void ItemPickup(Player player)
    {
        player.TookBooster(multiplier, duration);

        base.ItemPickup(player);
    }
}
