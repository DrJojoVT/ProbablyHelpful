using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using UnityEngine;

namespace ProbablyHelpful
{
    // Extend Methods
    public partial class ProbablyHelpfulItem
    {
    
        public ProbablyHelpfulItem CanSelfActivate(Func<GameItem, GameInventory, SlotMarker, bool> canActivate)
        {
            return Defer(item =>
            {
                item.mayActivateSlotItemFunc += canActivate;
                item.canActivateSlotItemFunc += canActivate;
                if (this is IProbablyHelpfulContainer)
                {
                    RestrictActivationToContextMenu(item);
                }
            });
        }

        public void RestrictActivationToContextMenu(GameItem item)
        {
            Func<GameItem, GameInventory, SlotMarker, bool> inner = item.mayActivateSlotItemFunc;
            if (inner == null)
                return;
            ProbablyHelpful.Logger.Log("Pilk RestrictActivationToContextMenu");
            item.mayActivateSlotItemFunc = (activated, inventory, slot) =>
                IsContextMenuTargeting(activated) &&
                inner.InvokeAllReduce(activated, inventory, slot, InvokeFuncExtensions.BoolReduceOR);
        }

        public ProbablyHelpfulItem CanSelfActivate(bool canActivate)
        {
            return CanSelfActivate((_,_,_) => canActivate);
        }

        public ProbablyHelpfulItem OnSelfActivate(Action<GameItem> activate)
        {
            return OnSelfActivate((item, _, _) => activate(item));
        }

        public ProbablyHelpfulItem OnSelfActivate(Action<GameItem, GameInventory> activate)
        {
            return OnSelfActivate((item, inventory, _) => activate(item, inventory));
        }

        public ProbablyHelpfulItem OnSelfActivate(Action<GameItem, GameInventory, SlotMarker> activate)
        {
            return Defer(item =>
            {
                item.onActivateSlotItemFunc += activate;
            });
        }

        private static readonly FieldInfo ContextMenuCurrentItemField =
            typeof(ItemContextHandler).GetField("currentItem", BindingFlags.Instance | BindingFlags.NonPublic);

        public static bool IsContextMenuTargeting(GameItem item)
        {
            ItemContextHandler handler = ItemContextHandler.current;
            if (handler == null || ContextMenuCurrentItemField == null) return false;
            return ReferenceEquals(ContextMenuCurrentItemField.GetValue(handler), item);
        }
    }
}