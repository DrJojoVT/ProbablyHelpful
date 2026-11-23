using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace ProbablyHelpful
{
    public static class ItemCollection
    {
        // public string ModId;
        // public List<ProbablyHelpfulItem> Items = new List<ProbablyHelpfulItem>();
        public static readonly Dictionary<Type, ProbablyHelpfulItem> RegisteredItems = new Dictionary<Type, ProbablyHelpfulItem>();
        public static List<Action<List<GameItem>>> DirectSellActions = new List<Action<List<GameItem>>>();

        public static T GetItem<T>() where T : ProbablyHelpfulItem
        {
            RegisteredItems.TryGetValue(typeof(T), out var item);
            return (T) item;
        }

        public static List<ProbablyHelpfulItem> GetItemsByModId(string modId)
        {
            return RegisteredItems.Values.Where(item => item.ModId == modId).ToList();
        }

        public static void AddItem(ProbablyHelpfulItem itemHelper)
        {
            // Items.Add(itemHelper);
            RegisteredItems[itemHelper.GetType()] = itemHelper;
        }

        public static void AddItems(params ProbablyHelpfulItem[] items)
        {
            foreach (ProbablyHelpfulItem itemHelper in items)
            {
                AddItem(itemHelper);
            }
        }

        public static void RegisterItems(ModItemDirectory directory, string modId)
        {
            var itemsForMod = GetItemsByModId(modId).ToList();
            foreach (ProbablyHelpfulItem itemHelper in itemsForMod)
            {
                if (!directory.Add(itemHelper.ItemId, () =>
                {
                    GameItem created = itemHelper.Build();
                    LoadedMod mod = ModLoader.Mods.Find(m => m.Manifest.ID == itemHelper.ModId);
                    string modName = mod?.Manifest.Name;
                    ModHelper.SetSource(created, itemHelper.ModId, string.IsNullOrEmpty(modName) ? itemHelper.ModId : modName);
                    return created;
                }))
                    UnityEngine.Debug.LogWarning("[ProbablyHelpful] Failed to register item " + itemHelper.ItemId + " (duplicate id?)");
            }
        }
    
        public static void RegisterOnDirectSell(Action<List<GameItem>> action)
        {
            DirectSellActions.Add(action);
        }
    }

    [HarmonyPatch(typeof(Dialogue))]
    public static class DialoguePatches
    {
        public static int presellCount;
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Dialogue.ExecuteEndAction))]
        public static bool ExecuteEndActionPrefix(Dialogue __instance)
        {
            presellCount = EmporiumEntry.Instance.GetAllNonOwnedItem().Count;
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Dialogue.ExecuteEndAction))] // Replace SomeMethod with the actual method name
        public static void ExecuteEndActionPostfix(Dialogue __instance)
        {
            var postsellCount = EmporiumEntry.Instance.GetAllNonOwnedItem().Count;
            if (presellCount < postsellCount) {
                var sells = EmporiumEntry.Instance.GetAllNonOwnedItem();
                foreach (var action in ItemCollection.DirectSellActions)
                {
                    action.Invoke(sells);
                }
            }
        }
    }
}
