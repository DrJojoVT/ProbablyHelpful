using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace ProbablyHelpful
{
    public partial class ProbablyHelpfulHelper
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
    
        public static List<Sprite> GetModSprites(string modId,string prefix, params GameItem[] items)
        {
            var sprites = new List<Sprite>();
            foreach (var item in items)
            {
                var sprite = CustomUIManager.Instance.GetModSprite(modId, prefix + item.identifier, true);
                if (sprite != null) sprites.Add(sprite);
            }
            return sprites;
        }
        public static bool MoveItemTo(GameItem item, GameItem target)
        {
            GameInventory inventory = GraphUtils.FindChildType<GameInventory>(target);
            return MoveItemTo(item, inventory);
        }
	    public static bool MoveItemTo(GameItem item, GameInventory inventory)
	    {
	        if (item == null || inventory == null)
	        {
	            return false;
	        }
	        SlotMarker slotMarker = inventory.TryFindOneValidInventorySlot(item);
	        if (slotMarker == null)
	        {
	            ProbablyHelpful.Logger.Warning($"MoveItemTo: no valid slot for '{item.identifier}' in inventory '{inventory.identifier}'.");
	            return false;
	        }

	        if (!slotMarker.IsValid())
	        {
	            ProbablyHelpful.Logger.Warning($"MoveItemTo: invalid slot for '{item.identifier}' in inventory '{inventory.identifier}'.");
	            return false;
	        }

	        // Use the same placement path as GraphUtils so we don't depend on grid-shape APIs.
	        GraphUtils.TryAcceptAll(inventory, item);
	        bool placed = item.parentInventory == inventory;
	        return placed;
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

        public static IEnumerable<GameInventory> GetRootInventories()
        {
            EmporiumEntry entry = EmporiumEntry.Instance;
            if (entry == null) yield break;
            GameInventory[] roots =
            {
                entry.invElement,
                entry.frontInvinvElement,
                entry.backInvinvElement,
                entry.backInvinvElementCounter,
                entry.showcaseElement,
                entry.hiddenElement,
                entry.soldElement,
                entry.hirelingInv,
                entry.trashInvElement,
                entry.trashcanInvElement,
                entry.docInvElement,
                entry.drainInvElement,
                entry.faucetElement,
                entry.cassettePlayerElement,
                entry.vendingMachineElement,
                entry.vendingFountainElement,
                entry.swapBufferElement,
                entry.bazarLeftinvElement,
                entry.responseInventory,
                entry.responseInventoryClosable,
                entry.afterhourInventory,
                entry.afterhourPocketSlotInvLeft,
                entry.afterhourPocketSlotInvBackpack,
                entry.afterhourPocketSlotInvRight,
            };
            foreach (GameInventory root in roots)
                if (root != null) yield return root;
        }

        public static List<GameItem> FindAllItems(Func<GameItem, bool> filter = null)
        {
            var found = new HashSet<GameItem>();
            foreach (GameInventory root in GetRootInventories())
                foreach (GameItem item in GraphUtils.FindAllChildrenType<GameItem>(root, filter))
                    found.Add(item);
            return found.ToList();
        }

        public static void ForEachItem(Action<GameItem> action, Func<GameItem, bool> filter = null)
        {
            foreach (GameItem item in FindAllItems(filter))
                action(item);
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

        public static int AddSellContainer<T>(List<GameItem> validItems) 
            where T : ProbablyHelpfulItem, IProbablyHelpfulContainer
        {
            ProbablyHelpfulItem itemHelper = ItemCollection.GetItem<T>();
            GameItem container = ModHelper.SpawnItem(itemHelper.ItemId);
            int total = 0;
            foreach (var item in validItems)
            {
                ModHelper.SetItemOwnerShip(item, true);
                var slot = container.TryFindOneValidInventorySlot(item);
                var n = slot?.TryAcceptOnce() ?? 0;
                if (n == 0)
                {
                    // Item not accepted into the container
                    ModHelper.SetItemOwnerShip(item, false);
                    continue;
                }
                total += n;
            }
            ModHelper.SetItemOwnerShip(container, false);
            ModHelper.MoveItemToBackCounter(container);
            ModHelper.MoveItemToFrontCounter(container);
            return total;
        }
    }
}
