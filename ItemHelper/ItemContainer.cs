using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace ProbablyHelpful
{
    // Extend Methods
    public partial class ProbablyHelpfulItem
    {
        private int _inventoryWidth;
        private int _inventoryHeight;
        private List<string> whitelistItemIds = new List<string>();
        private List<string> whitelistItemTags = new List<string>();
        public ProbablyHelpfulItem SetContainer(int inventoryWidth, int inventoryHeight)
        {
            CreateItemContainerInventory(inventoryWidth, inventoryHeight);
            return this;
        }

        public void CreateItemContainerInventory(int inventoryWidth, int inventoryHeight)
        {
            _inventoryWidth = inventoryWidth;
            _inventoryHeight = inventoryHeight;
            if (inventoryWidth <= 0 || inventoryHeight <= 0)
                throw new InvalidOperationException("Container inventory dimensions must be positive.");

            Defer(item =>
            {
                var (window, inventory) = DirectoryUtils.CreateInventoryWindow(inventoryWidth, inventoryHeight, true);
                item.SetContentWindow(window);
                ContainerHelper.InitContainerItem(inventory, item, whitelistItemTags, whitelistItemIds);
            });
            Tag.Add(ContainerHelper.CONTAINER_TAG);
        }

        public ProbablyHelpfulItem ContainerAllowedItems(params string[] ids)
        {
            whitelistItemIds.AddRange(ids);
            return this;
        }

        public ProbablyHelpfulItem ContainerAllowedTags(params string[] ids)
        {
            whitelistItemTags.AddRange(ids);
            return this;
        }

        public ProbablyHelpfulItem ContainerOpenSound(string soundId)
        {
            return Defer(item => item.soundContainerOpen = soundId);
        }

        public ProbablyHelpfulItem ContainerCloseSound(string soundId)
        {
            return Defer(item => item.soundContainerClose = soundId);
        }

        public ProbablyHelpfulItem SetAsPouch()
        {
            return Tag.Add(ContainerHelper.POUCH_TAG);
        }

        public ProbablyHelpfulItem SetAsBackpack()
        {
            return Tag.Add(ContainerHelper.BACKPACK_TAG);
        }

        public ProbablyHelpfulItem StorableInBackpack()
        {
            StorableInContainers();
            return Tag.Add(ContainerHelper.NESTABLE_DEVICE_TAG);
        }

        public ProbablyHelpfulItem StorableInContainers()
        {
            return Tag.Add(ContainerHelper.STORABLE_PACKAGE_TAG);
        }
        
    }
}