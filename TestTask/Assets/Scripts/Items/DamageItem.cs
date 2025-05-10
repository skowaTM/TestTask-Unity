using UnityEngine;

public class DamageItem : Item
{
    [SerializeField, Range(1f, 3f)] private float damage = 1f;


    protected override void ItemPickup(Player player)
    {
        base.ItemPickup(player);

        player.TookDamage(damage);
    }
}
