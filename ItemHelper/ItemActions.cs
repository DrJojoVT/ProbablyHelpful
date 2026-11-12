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
    
        public ProbablyHelpfulItem CanSelfActivate(Func<GameItem, GameInventory, SlotMarker, bool> canActivate)
        {
            return Defer(item =>
            {
                item.mayActivateSlotItemFunc += canActivate;
                item.canActivateSlotItemFunc += canActivate;
            });
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
    }
}