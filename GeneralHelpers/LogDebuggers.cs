using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Reflection;

namespace ProbablyHelpful
{
    public partial class ProbablyHelpfulHelper
    {
        public static void LogItem(ModLog logger, GameItem item)
        {
            if (logger == null) return;
            if (item == null)
            {
                logger.Log("==========");
                logger.Log("Item: <null>");
                logger.Log("==========");
                return;
            }

            logger.Log("==========");
            logger.Log($"Item: {item.name} {item.identifier}");

            // Best-effort tag dump.
            // GameItem stores tags in an internal structure that may differ between builds;
            // instead of relying on internal fields, enumerate known tag constants and test them.
            DumpKnownTags(logger, item, typeof(ContainerHelper), "ContainerHelper");
            DumpKnownTags(logger, item, typeof(TypeHelper), "TypeHelper");

            // Fallback: attempt to reflect internal tag dictionaries (may yield nothing).
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            foreach (var field in item.GetType().GetFields(flags))
            {
                if (!field.Name.Contains("tag", StringComparison.OrdinalIgnoreCase)) continue;
                if (!typeof(IDictionary).IsAssignableFrom(field.FieldType)) continue;
                if (field.GetValue(item) is not IDictionary dict) continue;
                logger.Log($"Tag Dict: {field.Name}");
                foreach (DictionaryEntry entry in dict)
                {
                    if (entry.Key is not string key || entry.Value == null)
                        continue;

                    var isEnabled = entry.Value.GetType().GetMethod("IsEnabled", flags);
                    if (isEnabled != null && (bool)isEnabled.Invoke(entry.Value, null) == true)
                        logger.Log($"  + {key}");
                }
            }
            logger.Log("==========");
        }

        private static void DumpKnownTags(ModLog logger, GameItem item, Type tagType, string label)
        {
            try
            {
                var tags = tagType
                    .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
                    .Select(f => (string)f.GetRawConstantValue())
                    .Distinct()
                    .ToList();

                var enabled = new List<string>();
                foreach (var tag in tags)
                {
                    if (string.IsNullOrEmpty(tag)) continue;
                    if (item.IsTag(tag)) enabled.Add(tag);
                }

                if (enabled.Count > 0)
                {
                    logger.Log($"Tags ({label}):");
                    foreach (var tag in enabled)
                        logger.Log($"  + {tag}");
                }
            }
            catch (Exception ex)
            {
                logger.Log($"LogItem: failed dumping {label} tags: {ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}