using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProbablyHelpful
{
    public static class CustomInspectionRegistry
    {
        public class InspectionPair
        {
            public string ItemTag;
            public CustomInspectionUI Small;
            public CustomInspectionUI Large;
            public Vector3 InitialPosition;
            public IProbablyHelpfulInspectionItem InspectionItem;
        }

        private static readonly List<InspectionPair> pairs =
            new List<InspectionPair>();

        public static void Register(string itemTag, GameObject smallRoot, GameObject largeRoot, IProbablyHelpfulInspectionItem itemDefinition)
        {
            if (smallRoot == null || largeRoot == null)
                throw new ArgumentNullException(
                    "Inspection roots cannot be null."
                );

            if (smallRoot == largeRoot)
                throw new ArgumentException(
                    "Small and large roots must be different GameObjects."
                );

            if (string.IsNullOrEmpty(itemTag))
                throw new ArgumentException("Item tag cannot be empty.");

            if (pairs.Exists(p => p.ItemTag == itemTag))
                throw new InvalidOperationException(
                    $"Inspection already registered: {itemTag}"
                );

            CustomInspectionUI small =
                SetupRoot(smallRoot, false);

            CustomInspectionUI large =
                SetupRoot(largeRoot, true);

            pairs.Add(new InspectionPair
            {
                ItemTag = itemTag,
                Small = small,
                Large = large,
                InitialPosition = large.transform.localPosition,
                InspectionItem = itemDefinition
            });
        }

        public static InspectionPair GetPair(string inspectionTag)
        {
            return pairs.Find(p => p.ItemTag == inspectionTag);
        }

        public static InspectionPair GetPair(GameItem item)
        {
            return pairs.Find(p => item.IsTag(p.ItemTag));
        }

        public static IEnumerable<InspectionPair> GetAllPairs()
        {
            return pairs;
        }

        private static CustomInspectionUI SetupRoot(
            GameObject root,
            bool enlarged)
        {
            root.SetActive(false);
            root.layer = LayerMask.NameToLayer("Ignore Raycast");

            // A UI GameObject must already have a RectTransform.
            // Unity cannot replace a regular Transform in place.
            RectTransform rect =
                root.GetComponent<RectTransform>();

            if (rect == null)
            {
                throw new InvalidOperationException(
                    $"{root.name} requires a RectTransform. " +
                    "Create it using new GameObject(name, typeof(RectTransform))."
                );
            }

            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one * (enlarged ? 2f : 1f);

            CustomInspectionUI ui =
                root.GetComponent<CustomInspectionUI>();

            if (ui == null)
                ui = root.AddComponent<CustomInspectionUI>();

            if (enlarged &&
                root.GetComponent<CounterMoveChild>() == null)
            {
                root.AddComponent<CounterMoveChild>();
            }

            return ui;
        }

        public static void ResetAll()
        {
            foreach (InspectionPair pair in pairs)
            {
                if (pair.Small != null)
                    pair.Small.gameObject.SetActive(false);

                if (pair.Large != null)
                    pair.Large.gameObject.SetActive(false);
            }
        }

        public static void ResetPositions()
        {
            foreach (InspectionPair pair in pairs)
            {
                if (pair.Large != null)
                {
                    pair.Large.transform.localPosition =
                        pair.InitialPosition;
                }
            }
        }

        public static void HandleItem(GameItem gameItem)
        {
            foreach (var pair in pairs)
            {
                bool matches = gameItem.IsTag(pair.ItemTag);

                pair.Small.gameObject.SetActive(matches);
                pair.Large.gameObject.SetActive(matches);

                if (matches && pair.InspectionItem is IProbablyHelpfulInspectionItem inspectionItem)
                {
                    pair.Small.OnInit(gameItem);
                    pair.Large.OnInit(gameItem);
                    inspectionItem.OnInspect(gameItem);
                }
            }
        }
    }
}