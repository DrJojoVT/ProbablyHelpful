using System;
using System.Collections.Generic;
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
        public static GameInventory CreateContainer<T>(this T containerItem, GameItem item, int inventoryWidth, int inventoryHeight)
            where T : ProbablyHelpfulItem, IProbablyHelpfulContainer
        {
            var (window, inventory) = DirectoryUtils.CreateInventoryWindow(inventoryWidth, inventoryHeight, true);
            item.SetContentWindow(window);
            ContainerHelper.InitContainerItem(inventory, item, containerItem.AllowedTags, containerItem.AllowedIds);
            return inventory;
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
        }
    }

    public partial class ProbablyHelpfulItem
    {
        

        // Runs inside Build (never defers: _buildSteps is being enumerated there).
        // private void InitializeContainerInterface(GameItem item)
        // {
        //     if (!(this is IProbablyHelpfulContainer container))
        //         return;
        //     if (container.InventoryWidth <= 0 || container.InventoryHeight <= 0)
        //         throw new InvalidOperationException("Container inventory dimensions must be positive.");

        //     if (container.AllowedItemIds != null)
        //         whitelistItemIds.AddRange(container.AllowedItemIds);
        //     if (container.AllowedItemTags != null)
        //         whitelistItemTags.AddRange(container.AllowedItemTags);

        //     GameInventory inventory = CreateContainer(item, container.InventoryWidth, container.InventoryHeight);
        //     item.ModifyTag(ContainerHelper.CONTAINER_TAG, state => state.SetBool(true), true);
        //     item.EnableTag(ContainerHelper.CONTAINER_TAG);
        //     if (container.OpenSoundId != null)
        //         item.soundContainerOpen = container.OpenSoundId;
        //     if (container.CloseSoundId != null)
        //         item.soundContainerClose = container.CloseSoundId;

        //     container.CustomizeContainer(item, inventory);
        // }

    
    }
}
