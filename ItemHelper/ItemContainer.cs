using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ProbablyHelpful
{
    
    public interface IProbablyHelpfulContainer
    {
        private static readonly ConditionalWeakTable<IProbablyHelpfulContainer, List<string>> _allowedIds = new();
        private static readonly ConditionalWeakTable<IProbablyHelpfulContainer, List<string>> _allowedTags = new();
        List<string> AllowedIds => _allowedIds.GetOrCreateValue(this);
        List<string> AllowedTags => _allowedTags.GetOrCreateValue(this);
        void CustomizeContainer(GameItem item) { }
    }

    public static class ProbablyHelpfulContainerExtensions
    {
        public static GameInventory CreateContainer<T>(this T containerItem, GameItem item, int inventoryWidth, int inventoryHeight, bool allowAllItems = false, bool containerTags = true)
            where T : ProbablyHelpfulItem, IProbablyHelpfulContainer
        {
            var (window, inventory) = DirectoryUtils.CreateInventoryWindow(inventoryWidth, inventoryHeight, true);
            item.SetContentWindow(window);
            if (allowAllItems)
                ContainerHelper.InitContainerItem(inventory, item);
            else
                ContainerHelper.InitContainerItem(inventory, item, containerItem.AllowedTags, containerItem.AllowedIds);
	        
            item.EnableTag(ContainerHelper.CONTAINER_TAG);
            item.EnableTag(ContainerHelper.CONTAINER_TOOLTIP_TAG);
            return inventory;
        }

        public static GameInventory[] CreateMultiContainerVertical<T>(this T containerItem, GameItem item, params 
            (int width, int height, List<string> allowedTags, List<string> allowedIds)[] sections) 
            where T : ProbablyHelpfulItem, IProbablyHelpfulContainer
        {
            var inventories = sections.Select(s => new GameGridInventory(s.width, s.height).SetMachineryStyle()).ToArray();

            var window = MachineHelper.CreateMachineryWindow(canMove: true,
                MachineHelper.Panel(null, MachineryStack.Vertical(inventories))); //, role: "wallet"
            item.SetContentWindow(window);
            
            for (int i = 0; i < sections.Length; i++)
            {
                ContainerHelper.InitContainerItem(inventories[i], item, sections[i].allowedTags, sections[i].allowedIds);
            }
            item.EnableTag(ContainerHelper.CONTAINER_TAG);
            return inventories;
        }

        public static void ContainerOpenSound<T>(this T containerItem, GameItem item, string soundId)
            where T : ProbablyHelpfulItem, IProbablyHelpfulContainer
        {
            item.soundContainerOpen = soundId;
        }

        public static void ContainerCloseSound<T>(this T containerItem, GameItem item, string soundId)
            where T : ProbablyHelpfulItem, IProbablyHelpfulContainer
        {
            item.soundContainerClose = soundId;
        }

        public static void SetAsPouch<T>(this T containerItem, GameItem item)
            where T : ProbablyHelpfulItem, IProbablyHelpfulContainer
        {
            containerItem.Tag.Add(ContainerHelper.POUCH_TAG);
        }

        public static void SetAsBackpack<T>(this T containerItem, GameItem item)
            where T : ProbablyHelpfulItem, IProbablyHelpfulContainer
        {
            containerItem.Tag.Add(ContainerHelper.BACKPACK_TAG);
        }

        public static void StorableInBackpack<T>(this T containerItem, GameItem item)
            where T : ProbablyHelpfulItem, IProbablyHelpfulContainer
        {
            containerItem.StorableInContainers(item);
            containerItem.Tag.Add(ContainerHelper.NESTABLE_DEVICE_TAG);
        }

        public static void SetAsPrecontainer<T>(this T containerItem, GameItem item)
        {
            item.DisableTag(ContainerHelper.CONTAINER_TAG);
            item.EnableTag(ContainerHelper.PRE_CONTAINER_TAG);

        }

        public static void StorableInContainers<T>(this T containerItem, GameItem item)
            where T : ProbablyHelpfulItem, IProbablyHelpfulContainer
        {
            containerItem.Tag.Add(ContainerHelper.STORABLE_PACKAGE_TAG);
        }

        public static void ContainerAllowedItems<T>(this T containerItem, params string[] ids)
            where T : ProbablyHelpfulItem, IProbablyHelpfulContainer
        {
            containerItem.AllowedIds.AddRange(ids);
        }

        public static void ContainerAllowedTags<T>(this T containerItem, params string[] ids)
            where T : ProbablyHelpfulItem, IProbablyHelpfulContainer
        {
            containerItem.AllowedTags.AddRange(ids);
        }

        public static void ModifySpriteFromInventory<T>(this T containerItem, GameItem item, Func<GameInventory, List<ItemSpriteModifier>> modifierFunc)
            where T : ProbablyHelpfulItem, IProbablyHelpfulContainer
        {
            GameInventory inventory = GraphUtils.FindChildType<GameInventory>(item);
            if (inventory == null)
            {
                UnityEngine.Debug.LogWarning("[ProbablyHelpful] ModifySpriteFromInventory on '" + item.identifier + "': no inventory found. Call it after SetContainer.");
                return;
            }
            containerItem.ModifySpriteFromInventory(item, inventory, modifierFunc);
        }

        public static void ModifySpriteFromInventory<T>(this T containerItem, GameItem item, GameInventory inventory, Func<GameInventory, List<ItemSpriteModifier>> modifierFunc)
            where T : ProbablyHelpfulItem, IProbablyHelpfulContainer
        {
            Func<GameItem, List<ItemSpriteModifier>> previous = item.getItemSpriteModifierForItemFunc;
            item.getItemSpriteModifierForItemFunc = self =>
            {
                var modifiers = new List<ItemSpriteModifier>();
                if (previous != null)
                {
                    modifiers.AddRange(previous(self));
                }

                if (modifierFunc != null)
                {
                    var extraModifiers = modifierFunc(inventory);
                    if (extraModifiers != null)
                    {
                        modifiers.AddRange(extraModifiers);
                    }
                }

                return modifiers;
            };
            Action<GameItem, GameInventory, SlotMarker> refresh = (_, _, _) => item.Validate();
            inventory.onSlotAddItemEndFunc += refresh;
            inventory.onSlotRemoveItemEndFunc += refresh;
        }

        public static void ModifySpriteFromInventories<T>(this T containerItem, GameItem item, Func<GameInventory, List<ItemSpriteModifier>> modifierFunc, params GameInventory[] inventories)
            where T : ProbablyHelpfulItem, IProbablyHelpfulContainer
        {
            foreach (var inventory in inventories)
            {
                containerItem.ModifySpriteFromInventory(item, inventory, modifierFunc);
            }
        }
    }
}
