using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace ProbablyHelpful
{
    public class ProbablyHelpfulHelper
    {
        public static List<ItemSpriteModifier> GetItemSpriteModifiersFromInventory(Func<GameItem, int, ItemSpriteModifier> callback, GameInventory inventory, int startIndex = 0, int maxItems = int.MaxValue)
        {
            var modifiers = new List<ItemSpriteModifier>();
            for (var i = startIndex; i < startIndex + maxItems; i++)
            {
                GameItem item = inventory.childItems.ElementAtOrDefault(i);
                if (item == null) break;
                modifiers.Add(callback.Invoke(item, i));
            }

            return modifiers;
        }

        public static List<Vector2> GetVectorPatter79(int ox, int oy, int w, int h, int rows, int cols)
        {
            List<Vector2> vectors = new List<Vector2>();
            for (var i = 0; i < rows * cols; i++)
            {
                int x = i % cols;
                int dx = ox + x * w;
                int y = i / cols;
                int dy = -oy - y * h;
                var pos = new Vector2(dx,dy);
                vectors.Add(pos);
            }
            return vectors;
        }
    
        public static bool MoveItemTo(GameItem item, GameInventory inventory)
        {
            SlotMarker slotMarker = inventory.TryFindOneValidInventorySlot(item);
            if (slotMarker == null || !slotMarker.IsValid() || slotMarker.AcceptUnchecked() < 1) return false;
            inventory.TryInventorySlot(item, slotMarker.itemGridShape);
            if (inventory.TryInventorySlot(item, slotMarker.itemGridShape) != null) return true;
            return false;
        }

        public static void MoveContents(GameInventory source, GameInventory destination)
        {
            if (source == null || destination == null || source == destination) return;
            foreach (GameItem content in new List<GameItem>(source.childItems))
            {
                GraphUtils.TryAcceptAll(destination, content);
            }
        }

        public static void MoveContents(GameItem source, GameInventory destination)
        {
            GameInventory inventory1 = GraphUtils.FindChildType<GameInventory>(source);
            MoveContents(inventory1, destination);
        }

        public static void MoveContents(GameItem source, GameItem destination)
        {
            GameInventory inventory1 = GraphUtils.FindChildType<GameInventory>(source);
            GameInventory inventory2 = GraphUtils.FindChildType<GameInventory>(destination);
            MoveContents(inventory1, inventory2);
        }

        public static bool TryCreateItem(string itemId, GameInventory inventory, out GameItem item)
        {
            item = ModHelper.SpawnItem(itemId);
            if (inventory == null) {
                item.Destroy();
                return false;
            }
            if (item == null) return false;
            if (MoveItemTo(item, inventory)) return true;
            item.Destroy();
            return false;
        }
    }
}