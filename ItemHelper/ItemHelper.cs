using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProbablyHelpful
{
    public abstract partial class ProbablyHelpfulItem
    {
        private readonly List<Action<GameItem>> _buildSteps = new List<Action<GameItem>>();
        public ProbablyHelpfulTagCollection Tag;
        public abstract string ItemId { get; }
        public abstract string ModId { get; }
        private GameItem _gameItem;
        public GameItem GameItem => _gameItem;
        public CustomInspectionUI inspectionUISmall;
        public CustomInspectionUI inspectionUILarge;

        public ProbablyHelpfulItem()
        {
            Tag = new ProbablyHelpfulTagCollection(this);
        }
        public virtual void Customize() { }

        public GameItem Build()
        {
            GameItem item = _gameItem = ItemDirectory.CreateEmptyItem(ItemId);
            item.SetSpriteAndShapeFromMod(ModId, ItemId);
            Customize();
            if (this is IProbablyHelpfulContainer container)
            {
                container.CustomizeContainer();
            }
            if (this is IProbablyHelpfulInspectionItem inspectionItem)
            {
                inspectionItem.InitializeInspection();
            }
            return item;
        }

        public void SetSprite(string spriteKey)
        {
            GameItem.SetSpriteAndShapeFromMod(ModId, spriteKey);
        }

        public void SetName(string name)
        {
            GameItem.name = name;
        }

        public void SetDescription(string desc)
        {
            GameItem.shortDescription = desc;
        }

        public void SetLocalizedNameAndDescription()
        {
            GameItem.name = ModHelper.GetLocalized(ModId, ItemId + "_name");
            GameItem.shortDescription = ModHelper.GetLocalized(ModId, ItemId + "_description");
        }

        public void SetValue(int value)
        {
            GameItem.SetValue(value);
        }

        public void SetShape(GridShapeBuilder shape)
        {
            GameItem.SetShape(shape);
            if (GameItem is GameItemElement element)
            {
                element.background.image.alphaHitTestMinimumThreshold = 0f;
                element.fakeBackground.image.alphaHitTestMinimumThreshold = 0f;
            }
        }

        public void SetSolidShape(int width, int height)
        {
            SetShape(new GridShapeBuilder().SetDataFill(width, height));
        }

        public void Revalidate(GameItem item)
        {
            item.ValidateShapeState();
        }
        public static void ClearTags(GameItem item)
        {
            foreach (string tag in item.state.dict.Keys.ToList())
            item.DisableTag(tag, skipValidate: true);
        }
    }
}
