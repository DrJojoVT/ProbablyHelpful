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
    }
}