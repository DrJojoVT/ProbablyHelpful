using System;
using System.Collections.Generic;

namespace ProbablyHelpful
{
    public partial class ProbablyHelpfulItem
    {
        public ProbablyHelpfulItem ModifySpriteFromInventory(Func<GameInventory, List<ItemSpriteModifier>> modifierFunc)
        {
            return Defer(item =>
            {
                GameInventory inventory = GraphUtils.FindChildType<GameInventory>(item);
                if (inventory == null)
                {
                    UnityEngine.Debug.LogWarning("[ProbablyHelpful] ModifySpriteFromInventory on '" + item.identifier + "': no inventory found. Call it after SetContainer.");
                    return;
                }
                Func<GameItem, List<ItemSpriteModifier>> previous = item.getItemSpriteModifierForItemFunc;
                item.getItemSpriteModifierForItemFunc = self =>
                {
                    var modifiers = new List<ItemSpriteModifier>();
                    List<ItemSpriteModifier> fromPrevious = previous?.Invoke(self);
                    if (fromPrevious != null)
                        modifiers.AddRange(fromPrevious);
                    List<ItemSpriteModifier> fromInventory = modifierFunc(inventory);
                    if (fromInventory != null)
                        modifiers.AddRange(fromInventory);
                    return modifiers;
                };
                Action<GameItem, GameInventory, SlotMarker> refresh = (_, _, _) => item.Validate();
                inventory.onSlotAddItemEndFunc += refresh;
                inventory.onSlotRemoveItemEndFunc += refresh;
            });
        }
    }
}
