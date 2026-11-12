using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProbablyHelpful
{
    public partial class ProbablyHelpfulItem
    {
        public static bool IsEnabled = false;
        private readonly List<Action<GameItem>> _buildSteps = new List<Action<GameItem>>();
        public string _itemId;
        private string _modId;
        public ProbablyHelpfulTagCollection Tag;

        public string ItemId => _itemId;
        public string ModId => _modId;

        public ProbablyHelpfulItem(string modId, string itemId)
        {
            if (itemId.Contains(".")) {
                _itemId = itemId;
            } else {
                _itemId = modId + "." + itemId;
            }
            _modId = modId;
            Tag = new ProbablyHelpfulTagCollection(this);
            // Containers
            if (this is IProbablyHelpfulContainer container)
            {
                Defer((item) => container.CustomizeContainer(item));
            }
        }

        internal ProbablyHelpfulItem Defer(Action<GameItem> step)
        {
            _buildSteps.Add(step);
            return this;
        }

        public GameItem Build()
        {
            GameItem item = ItemDirectory.CreateEmptyItem(_itemId);
            item.SetSpriteAndShapeFromMod(_modId, _itemId);
            int count = _buildSteps.Count;
            try 
            {
                for (int i = 0; i < _buildSteps.Count; i++)
                    _buildSteps[i](item);
            }
            finally
            {
                if (_buildSteps.Count > count)
                    _buildSteps.RemoveRange(count, _buildSteps.Count - count);
            }
            return item;
        }

        public ProbablyHelpfulItem SetSprite(string spriteKey)
        {
            return Defer(item => item.SetSpriteAndShapeFromMod(_modId, spriteKey));
        }

        public ProbablyHelpfulItem SetName(string name)
        {
            return Defer(item => item.name = name);
        }

        public ProbablyHelpfulItem SetDescription(string desc)
        {
            return Defer(item => item.shortDescription = desc);
        }

        public ProbablyHelpfulItem SetLocalizedNameAndDescription()
        {
            return Defer(item =>
            {
                item.name = ModHelper.GetLocalized(_modId, _itemId + "_name");
                item.shortDescription = ModHelper.GetLocalized(_modId, _itemId + "_description");
            });
        }

        public ProbablyHelpfulItem SetValue(int value)
        {
            return Defer(item => item.SetValue(value));
        }

        public ProbablyHelpfulItem SetSolidShape(int width, int height)
        {
            return Defer(item => item.SetShape(new GridShapeBuilder().SetDataFill(width, height)));
        }

        public void SetItemActivateOnTarget(string itemId, Func<GameItem, GameItem, bool> canExecute, Action<GameItem, GameItem> execute)
        {
            SetItemActivateOnTarget((target, dragged) => dragged.identifier == itemId, canExecute, execute);
        }

        public void SetItemActivateOnTarget(Func<GameItem, GameItem, bool> isValidTarget, Func<GameItem, GameItem, bool> canExecute, Action<GameItem, GameItem> execute)
        {
            Defer(item =>
            {
                item.mayThisTargetItemFunc += isValidTarget;
                item.canThisTargetItemFunc += (dragged, target) =>
                    isValidTarget(dragged, target) && canExecute(dragged, target);
                item.onThisTargetItemFunc += execute;
            });
        }
    }
}
