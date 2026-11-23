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
        void CustomizeContainer() { }
        public GameInventory CreateContainer(int inventoryWidth, int inventoryHeight, bool allowAllItems = false)
        {
            if (this is not ProbablyHelpfulItem itemHelper) return default;
            GameItem item = itemHelper.GameItem;
            var (window, inventory) = DirectoryUtils.CreateInventoryWindow(inventoryWidth, inventoryHeight, true);
            item.SetContentWindow(window);
            if (allowAllItems)
                ContainerHelper.InitContainerItem(inventory, item);
            else
                ContainerHelper.InitContainerItem(inventory, item, AllowedTags, AllowedIds);
	        
            item.EnableTag(ContainerHelper.CONTAINER_TAG);
            item.EnableTag(ContainerHelper.CONTAINER_TOOLTIP_TAG);
            return inventory;
        }

        public GameInventory[] CreateMultiContainer(params (int width, int height)[] sections)
        {
            if (this is not ProbablyHelpfulItem itemHelper) return default;
            GameItem item = itemHelper.GameItem;
            var inventories = sections.Select(s => new GameGridInventory(s.width, s.height).SetMachineryStyle()).ToArray();

            var window = MachineHelper.CreateMachineryWindow(canMove: true,
                MachineHelper.Panel(null, MachineryStack.Horizontal(inventories)));
            item.SetContentWindow(window);

            for (int i = 0; i < sections.Length; i++)
            {
                ContainerHelper.InitContainerItem(inventories[i], item);
            }
            item.EnableTag(ContainerHelper.CONTAINER_TAG);
            return inventories;
        }

        public GameInventory[] CreateMultiContainer(params (int width, int height, List<string> allowedTags, List<string> allowedIds)[] sections)
        {
            if (this is not ProbablyHelpfulItem itemHelper) return default;
            GameItem item = itemHelper.GameItem;
            var inventories = sections.Select(s => new GameGridInventory(s.width, s.height).SetMachineryStyle()).ToArray();

            var window = MachineHelper.CreateMachineryWindow(canMove: true,
                MachineHelper.Panel(null, MachineryStack.Horizontal(inventories)));
            item.SetContentWindow(window);

            for (int i = 0; i < sections.Length; i++)
            {
                ContainerHelper.InitContainerItem(inventories[i], item, sections[i].allowedTags, sections[i].allowedIds);
            }
            item.EnableTag(ContainerHelper.CONTAINER_TAG);
            return inventories;
        }

        public GameInventory[] CreateMultiContainerVertical(params (int width, int height)[] sections)
        {
            if (this is not ProbablyHelpfulItem itemHelper) return default;
            GameItem item = itemHelper.GameItem;
            var inventories = sections.Select(s => new GameGridInventory(s.width, s.height).SetMachineryStyle()).ToArray();

            var window = MachineHelper.CreateMachineryWindow(canMove: true,
                MachineHelper.Panel(null, MachineryStack.Vertical(inventories)));
            item.SetContentWindow(window);

            for (int i = 0; i < sections.Length; i++)
            {
                ContainerHelper.InitContainerItem(inventories[i], item);
            }
            item.EnableTag(ContainerHelper.CONTAINER_TAG);
            return inventories;
        }

        public GameInventory[] CreateMultiContainerVertical(params (int width, int height, List<string> allowedTags, List<string> allowedIds)[] sections)
        {
            if (this is not ProbablyHelpfulItem itemHelper) return default;
            GameItem item = itemHelper.GameItem;
            var inventories = sections.Select(s => new GameGridInventory(s.width, s.height).SetMachineryStyle()).ToArray();

            var window = MachineHelper.CreateMachineryWindow(canMove: true,
                MachineHelper.Panel(null, MachineryStack.Vertical(inventories)));
            item.SetContentWindow(window);

            for (int i = 0; i < sections.Length; i++)
            {
                ContainerHelper.InitContainerItem(inventories[i], item, sections[i].allowedTags, sections[i].allowedIds);
            }
            item.EnableTag(ContainerHelper.CONTAINER_TAG);
            return inventories;
        }

        public void ContainerOpenSound(string soundId)
        {
            if (this is not ProbablyHelpfulItem itemHelper) return;
            GameItem item = itemHelper.GameItem;
            item.soundContainerOpen = soundId;
        }

        public void ContainerCloseSound(string soundId)
        {
            if (this is not ProbablyHelpfulItem itemHelper) return;
            GameItem item = itemHelper.GameItem;
            item.soundContainerClose = soundId;
        }

        public void SetAsPouch()
        {
            if (this is not ProbablyHelpfulItem itemHelper) return;
            itemHelper.Tag.Add(ContainerHelper.POUCH_TAG);
        }

        public void SetAsBackpack()
        {
            if (this is not ProbablyHelpfulItem itemHelper) return;
            itemHelper.Tag.Add(ContainerHelper.BACKPACK_TAG);
        }

        public void StorableInBackpack()
        {
            if (this is not ProbablyHelpfulItem itemHelper) return;
            GameItem item = itemHelper.GameItem;
            StorableInContainers();
            itemHelper.Tag.Add(ContainerHelper.NESTABLE_DEVICE_TAG);
        }

        public void SetAsPrecontainer()
        {
            if (this is not ProbablyHelpfulItem itemHelper) return;
            GameItem item = itemHelper.GameItem;
            item.DisableTag(ContainerHelper.CONTAINER_TAG);
            item.EnableTag(ContainerHelper.PRE_CONTAINER_TAG);

        }

        public void StorableInContainers()
        {
            if (this is not ProbablyHelpfulItem itemHelper) return;
            itemHelper.Tag.Add(ContainerHelper.STORABLE_PACKAGE_TAG);
        }

        public void ContainerAllowedItems(params string[] ids)
        {
            AllowedIds.AddRange(ids);
        }

        public void ContainerAllowedTags(params string[] ids)
        {
            AllowedTags.AddRange(ids);
        }

        public void ModifySpriteFromInventory(Func<GameInventory, List<ItemSpriteModifier>> modifierFunc)
        {
            if (this is not ProbablyHelpfulItem itemHelper) return;
            GameItem item = itemHelper.GameItem;
            GameInventory inventory = GraphUtils.FindChildType<GameInventory>(item);
            if (inventory == null)
            {
                UnityEngine.Debug.LogWarning("[ProbablyHelpful] ModifySpriteFromInventory on '" + item.identifier + "': no inventory found. Call it after SetContainer.");
                return;
            }
            ModifySpriteFromInventory(inventory, modifierFunc);
        }

        public void ModifySpriteFromInventory(GameInventory inventory, Func<GameInventory, List<ItemSpriteModifier>> modifierFunc)
        {
            if (this is not ProbablyHelpfulItem itemHelper) return;
            GameItem item = itemHelper.GameItem;
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
    }
}
