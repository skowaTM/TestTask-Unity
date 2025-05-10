using UnityEngine;

public class NoteItem : Item
{
    [SerializeField, TextArea]
    private string noteText = "";


    protected override void ItemPickup(Player player)
    {
        if (noteText == "")
        {
            noteText = $"some text: bla-bla\nand some number: {UnityEngine.Random.Range(1, 100)}";
        }
        player.TookNote(noteText);

        base.ItemPickup(player);
    }
}
