using System;
using System.Text;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using TMPro;

namespace ProbablyHelpful
{
    
    public class CustomInspectionUI : MonoBehaviour
    {
        public void OnInit(GameItem item)
        {
            DisableAll();

            foreach (var group in spriteGroups)
            {
                if (group.Value.Count > 0)
                {
                    group.Value[0].SetActive(true);
                }
            }
        }
        private readonly Dictionary<string, List<GameObject>> spriteGroups = new Dictionary<string, List<GameObject>>();
        private readonly Dictionary<string, TextMeshProUGUI> textFields = new();

        public void AddSprite(string group, Sprite sprite, Vector2 position, Vector2 size)
        {
            if (!spriteGroups.TryGetValue(group, out var objects))
            {
                objects = new List<GameObject>();
                spriteGroups[group] = objects;
            }

            GameObject spriteObject = new GameObject(
                $"{group}_{objects.Count}",
                typeof(RectTransform),
                typeof(Image)
            );

            spriteObject.transform.SetParent(transform, false);

            RectTransform rect =
                spriteObject.GetComponent<RectTransform>();

            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            Image image = spriteObject.GetComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = true;
            image.raycastTarget = false;

            spriteObject.SetActive(false);

            objects.Add(spriteObject);
        }
        public void AddText(string textName, Vector2 position, Vector2 size, float fontSize = 24f
            , FontLoader.FontRole fontRole = FontLoader.FontRole.Noto_Sans, TextAlignmentOptions alignment = TextAlignmentOptions.Center, Color? color = null)
        {
            if (textFields.ContainsKey(textName))
            {
                Debug.LogWarning($"Inspection text already exists: {textName}");
                return;
            }

            GameObject textObject = new GameObject(
                $"Text_{textName}",
                typeof(RectTransform),
                typeof(TextMeshProUGUI)
            );

            textObject.transform.SetParent(transform, false);

            RectTransform rect = textObject.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            TextMeshProUGUI tmp = textObject.GetComponent<TextMeshProUGUI>();

            tmp.text = "";
            tmp.fontSize = fontSize;
            tmp.alignment = alignment;
            tmp.color = color ?? Color.black;
            tmp.enableWordWrapping = false;
            tmp.raycastTarget = false;

            // Use the game's localization-aware font system.
            var locale = UnityEngine.Localization.Settings
                .LocalizationSettings.SelectedLocale;

            tmp.font = FontLoader.Instance.GetFontByRole(fontRole, locale);

            textFields.Add(textName, tmp);
        }

        public void SetSpriteGroup(string groupName, int spriteIndex)
        {
            if (!spriteGroups.TryGetValue(groupName, out var sprites))
            {
                Debug.LogWarning($"Inspection group not found: {groupName}");
                return;
            }

            for (int i = 0; i < sprites.Count; i++)
            {
                sprites[i].SetActive(i == spriteIndex);
            }
        }
        public void SetText(string textName, string value)
        {
            if (!textFields.TryGetValue(textName, out var tmp))
            {
                Debug.LogWarning($"Inspection text not found: {textName}");
                return;
            }

            tmp.SetText(value ?? string.Empty);
        }

        public void DisableAll()
        {
            foreach (var group in spriteGroups.Values)
            {
                foreach (GameObject obj in group)
                    obj.SetActive(false);
            }
        }
    }

    public class ItemInspectionSpriteObject
    {
        private static string InspectionKey = "_InspectionUI";
        private static string MaskKey = "Mask";
        public string InspectionTag;
        public GameObject InspectionObject;
        public GameObject MaskObject;
        public Action<GameItem> OnInitializeCallback;
        public ItemInspectionSpriteObject(ItemSpriteModifier modifier, string inspectionTag, string sortingLayer = "", Action<GameItem> onInitialize = null)
        {
            InspectionTag = inspectionTag;
            InspectionObject = CreateGameObject(modifier, sortingLayer);
            InspectionObject.gameObject.transform.parent.name = InspectionKey;
            MaskObject = CreateGameObject(modifier, sortingLayer);
            MaskObject.gameObject.transform.parent.name = MaskKey;
            OnInitializeCallback = onInitialize ?? (_ => { });
        }

        public void OnInitialize(GameItem item)
        {
            OnInitializeCallback(item);
        }

        private GameObject CreateGameObject(ItemSpriteModifier modifier, string sortingLayer = "")
        {
            var go = new GameObject();
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = modifier.sprite;
            sr.color = modifier.color;
            sr.sortingLayerName = sortingLayer;
            sr.sortingOrder = modifier.priority;
            return go;
        }

        public void SetActive(bool isActive)
        {
            if (InspectionObject != null)
                InspectionObject.SetActive(isActive);
            if (MaskObject != null)
                MaskObject.SetActive(isActive);
        }
    }
}


// // ===================
// public static class GameObjectDumper
// {
//     // New method: Returns an array of strings, each safely under maxChunkSize
//     public static string[] DumpToStringArray(this GameObject go, int maxChunkSize = 7500, bool includeChildren = true)
//     {
//         string fullDump = go.DumpToString(includeChildren);
//         return ChunkByLines(fullDump, maxChunkSize);
//     }

//     // Helper to split the string cleanly at line breaks without breaking words
//     private static string[] ChunkByLines(string text, int maxChunkSize)
//     {
//         if (string.IsNullOrEmpty(text)) return new string[0];

//         List<string> chunks = new List<string>();
//         StringBuilder currentChunk = new StringBuilder();
//         string[] lines = text.Split('\n');

//         foreach (string rawLine in lines)
//         {
//             string line = rawLine.TrimEnd('\r');

//             // If adding the next line exceeds the limit, save the current chunk and start a new one
//             if (currentChunk.Length + line.Length + 1 > maxChunkSize && currentChunk.Length > 0)
//             {
//                 chunks.Add(currentChunk.ToString());
//                 currentChunk.Clear();
//             }

//             // Fallback in case a single line itself is longer than maxChunkSize
//             if (line.Length > maxChunkSize)
//             {
//                 for (int i = 0; i < line.Length; i += maxChunkSize)
//                 {
//                     int len = Mathf.Min(maxChunkSize, line.Length - i);
//                     chunks.Add(line.Substring(i, len));
//                 }
//             }
//             else
//             {
//                 currentChunk.AppendLine(line);
//             }
//         }

//         if (currentChunk.Length > 0)
//         {
//             chunks.Add(currentChunk.ToString());
//         }

//         return chunks.ToArray();
//     }

//     // --- Keep your existing DumpToString and DumpComponentMembers methods below ---
//     public static string DumpToString(this GameObject go, bool includeChildren = true, int indent = 0)
//     {
//         if (go == null) return "GameObject is null.";

//         StringBuilder sb = new StringBuilder();
//         string pad = new string(' ', indent * 4);

//         sb.AppendLine($"{pad}=== GameObject: {go.name} ===");
//         sb.AppendLine($"{pad}Active Self: {go.activeSelf} | Active In Hierarchy: {go.activeInHierarchy}");
//         sb.AppendLine($"{pad}Tag: {go.tag} | Layer: {LayerMask.LayerToName(go.layer)} ({go.layer})");

//         Transform t = go.transform;
//         sb.AppendLine($"{pad}Position: {t.position} (Local: {t.localPosition})");
//         sb.AppendLine($"{pad}Rotation: {t.eulerAngles} (Local: {t.localEulerAngles})");
//         sb.AppendLine($"{pad}Scale: {t.localScale}");

//         sb.AppendLine($"{pad}Components:");
//         Component[] components = go.GetComponents<Component>();

//         foreach (Component comp in components)
//         {
//             if (comp == null)
//             {
//                 sb.AppendLine($"{pad}  - [Missing Script]");
//                 continue;
//             }

//             sb.AppendLine($"{pad}  - {comp.GetType().Name}");
//             if (comp is Transform) continue;

//             DumpComponentMembers(comp, sb, pad + "      ");
//         }

//         if (includeChildren && t.childCount > 0)
//         {
//             sb.AppendLine($"{pad}Children ({t.childCount}):");
//             foreach (Transform child in t)
//             {
//                 sb.Append(child.gameObject.DumpToString(true, indent + 1));
//             }
//         }

//         return sb.ToString();
//     }

//     private static void DumpComponentMembers(Component comp, StringBuilder sb, string pad)
//     {
//         var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
//         var type = comp.GetType();

//         foreach (FieldInfo field in type.GetFields(flags))
//         {
//             try
//             {
//                 object value = field.GetValue(comp);
//                 sb.AppendLine($"{pad}{field.Name}: {value ?? "null"}");
//             }
//             catch { }
//         }

//         foreach (PropertyInfo prop in type.GetProperties(flags))
//         {
//             if (!prop.CanRead) continue;
//             try
//             {
//                 object value = prop.GetValue(comp, null);
//                 sb.AppendLine($"{pad}{prop.Name}: {value ?? "null"}");
//             }
//             catch { }
//         }
//     }
// }