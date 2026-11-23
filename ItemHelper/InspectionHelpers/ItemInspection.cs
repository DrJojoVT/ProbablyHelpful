

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProbablyHelpful
{
    public interface IProbablyHelpfulInspectionItem
    {
        string InspectionTag => "";
        void CustomizeInspection() { }

        void InitializeInspection()
        {
            if (this is not ProbablyHelpfulItem itemHelper) return;
            var item = itemHelper.GameItem;
            item.EnableTag("INSPECTABLE_TAG");
            if (!string.IsNullOrEmpty(InspectionTag))
                item.EnableTag(InspectionTag);
            SetupInspection();
            CustomizeInspection();
        }

        void OnInspect(GameItem item) { }

        void SetSpriteGroup(string groupName, int spriteIndex)
        {
            var pair = CustomInspectionRegistry.GetPair(InspectionTag);

            if (pair == null)
                return;

            pair.Small.SetSpriteGroup(groupName, spriteIndex);
            pair.Large.SetSpriteGroup(groupName, spriteIndex);
        }

        void SetupInspection()
        {
            if (this is not ProbablyHelpfulItem itemHelper) return;

            var existing = CustomInspectionRegistry.GetPair(InspectionTag);
            if (existing != null)
            {
                itemHelper.inspectionUISmall = existing.Small;
                itemHelper.inspectionUILarge = existing.Large;
                return;
            }

            Transform smallParent = InspectionUIManager.Instance.cigaretteUISmall.transform.parent;
            Transform largeParent = InspectionUIManager.Instance.cigaretteUILarge.transform.parent;

            GameObject gameRootSmall = new GameObject(
                $"_{InspectionTag}Inspection",
                typeof(RectTransform),
                typeof(CustomInspectionUI)
            );

            GameObject gameRootLarge = new GameObject(
                $"_{InspectionTag}InspectionLarge",
                typeof(RectTransform),
                typeof(CustomInspectionUI),
                typeof(CounterMoveChild)
            );

            // Parent first
            gameRootSmall.transform.SetParent(smallParent, false);
            gameRootLarge.transform.SetParent(largeParent, false);

            // Copy RectTransform configuration
            CopyRectTransform(
                InspectionUIManager.Instance.cigaretteUISmall.GetComponent<RectTransform>(),
                gameRootSmall.GetComponent<RectTransform>()
            );

            CopyRectTransform(
                InspectionUIManager.Instance.cigaretteUILarge.GetComponent<RectTransform>(),
                gameRootLarge.GetComponent<RectTransform>()
            );

            // Configure CounterMoveChild
            var cmc = gameRootLarge.GetComponent<CounterMoveChild>();

            AccessTools.Field(
                typeof(CounterMoveChild),
                "movementMultiplier"
            ).SetValue(cmc, 1.5f);

            gameRootSmall.transform.SetSiblingIndex(
                InspectionUIManager.Instance.cigaretteUISmall.transform.GetSiblingIndex()
            );

            gameRootLarge.transform.SetSiblingIndex(
                InspectionUIManager.Instance.cigaretteUILarge.transform.GetSiblingIndex()
            );

            itemHelper.inspectionUISmall = gameRootSmall.GetComponent<CustomInspectionUI>();
            itemHelper.inspectionUILarge = gameRootLarge.GetComponent<CustomInspectionUI>();

            CustomInspectionRegistry.Register(
                InspectionTag,
                gameRootSmall,
                gameRootLarge,
                this
            );
        }

        private static void CopyRectTransform(RectTransform source, RectTransform destination)
        {
            destination.anchorMin = source.anchorMin;
            destination.anchorMax = source.anchorMax;
            destination.pivot = source.pivot;

            destination.sizeDelta = source.sizeDelta;
            destination.anchoredPosition = source.anchoredPosition;

            destination.localRotation = source.localRotation;
            destination.localScale = source.localScale;
            destination.localPosition = source.localPosition;
        }
        void AddSprite(string groupName, string modId, string spriteKey)
        {
            AddSprite(groupName, CustomUIManager.Instance.GetModSprite(modId, spriteKey, true), 0, 0);
        }
        void AddSprite(string groupName, string modId, string spriteKey, int x, int y)
        {
            AddSprite(groupName, CustomUIManager.Instance.GetModSprite(modId, spriteKey, true), x, y);
        }
        void AddSprite(string groupName, Sprite sprite, int x, int y)
        {
            AddSprite(groupName, sprite, new Vector2(x, y));
        }
        void AddSprite(string groupName, Sprite sprite, Vector2 position)
        {
            if (this is ProbablyHelpfulItem itemHelper)
            {
                Vector2 size = new Vector2(sprite.rect.width, sprite.rect.height);
                itemHelper.inspectionUISmall.AddSprite(groupName, sprite, position, size);
                itemHelper.inspectionUILarge.AddSprite(groupName, sprite, position, size);
            }
        }

        void AddText(string textName,int x,int y,int width,int height,float fontSize = 24f
            ,FontLoader.FontRole fontRole = FontLoader.FontRole.Noto_Sans,TextAlignmentOptions alignment = TextAlignmentOptions.Center,Color? color = null)
        {
            var position = new Vector2(x, y);
            var size = new Vector2(width, height);
            AddText(textName, position, size, fontSize, fontRole, alignment,color);
        }

        void AddText(string textName,Vector2 position,Vector2 size,float fontSize = 24f,
            FontLoader.FontRole fontRole = FontLoader.FontRole.Noto_Sans,TextAlignmentOptions alignment = TextAlignmentOptions.Center,Color? color = null)
        {
            if (this is not ProbablyHelpfulItem itemHelper) return;
            itemHelper.inspectionUISmall.AddText(textName,position,size,fontSize,fontRole,alignment,color);
            itemHelper.inspectionUILarge.AddText(textName,position,size,fontSize,fontRole,alignment,color);
        }
    
        void SetText(string textName, string text)
        {
            if (this is not ProbablyHelpfulItem itemHelper) return;
            itemHelper.inspectionUISmall.SetText(textName, text);
            itemHelper.inspectionUILarge.SetText(textName, text);
        }
    }
}
