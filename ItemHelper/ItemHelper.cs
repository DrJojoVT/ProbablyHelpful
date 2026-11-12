using System;
using System.Collections.Generic;

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
            foreach (Action<GameItem> step in _buildSteps)
                step(item);
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

        public ProbablyHelpfulItem SetValue(int value)
        {
            return Defer(item => item.SetValue(value));
        }

        public ProbablyHelpfulItem SetSolidShape(int width, int height)
        {
            return Defer(item => item.SetShape(new GridShapeBuilder().SetDataFill(width, height)));
        }
    }
}
