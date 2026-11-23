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
    public abstract partial class ProbablyHelpfulItem
    {
    
        public void CanSelfActivate(Func<GameItem, GameInventory, SlotMarker, bool> canActivate)
        {
            GameItem.mayActivateSlotItemFunc += canActivate;
            GameItem.canActivateSlotItemFunc += canActivate;
            if (this is IProbablyHelpfulContainer)
            {
                RestrictActivationToContextMenu(GameItem);
            }
        }

        public void RestrictActivationToContextMenu(GameItem item)
        {
            Func<GameItem, GameInventory, SlotMarker, bool> inner = item.mayActivateSlotItemFunc;
            if (inner == null)
                return;
            item.mayActivateSlotItemFunc = (activated, inventory, slot) =>
                IsContextMenuTargeting(activated) &&
                inner.InvokeAllReduce(activated, inventory, slot, InvokeFuncExtensions.BoolReduceOR);
        }

        public void CanSelfActivate(bool canActivate)
        {
            CanSelfActivate((_,_,_) => canActivate);
        }

        public void OnSelfActivate(Action<GameItem> activate)
        {
            OnSelfActivate((item, _, _) => activate(item));
        }

        public void OnSelfActivate(Action<GameItem, GameInventory> activate)
        {
            OnSelfActivate((item, inventory, _) => activate(item, inventory));
        }

        public void OnSelfActivate(Action<GameItem, GameInventory, SlotMarker> activate)
        {
            GameItem.onActivateSlotItemFunc += activate;
        }

        private static readonly FieldInfo ContextMenuCurrentItemField =
            typeof(ItemContextHandler).GetField("currentItem", BindingFlags.Instance | BindingFlags.NonPublic);

        public static bool IsContextMenuTargeting(GameItem item)
        {
            ItemContextHandler handler = ItemContextHandler.current;
            if (handler == null || ContextMenuCurrentItemField == null) return false;
            return ReferenceEquals(ContextMenuCurrentItemField.GetValue(handler), item);
        }

        public void SetItemActivateOnTarget(string itemId, Func<GameItem, GameItem, bool> canExecute, Action<GameItem, GameItem> execute)
        {
            SetItemActivateOnTarget((target, dragged) => dragged.identifier == itemId, canExecute, execute);
        }
        public void SetItemActivateOnTarget(Func<GameItem, GameItem, bool> isValidTarget, Func<GameItem, GameItem, bool> canExecute, Action<GameItem, GameItem> execute)
        {
            GameItem.mayThisTargetItemFunc += isValidTarget;
            GameItem.canThisTargetItemFunc += (dragged, target) =>
                isValidTarget(dragged, target) && canExecute(dragged, target);
            GameItem.onThisTargetItemFunc += execute;
        }
    }
}