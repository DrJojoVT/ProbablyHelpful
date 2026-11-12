using System.Collections.Generic;

namespace ProbablyHelpful
{
    public class ItemCollection
    {
        private readonly List<ProbablyHelpfulItem> _items = new List<ProbablyHelpfulItem>();
        public ItemCollection SetItems(params ProbablyHelpfulItem[] items)
        {
            var collection = new ItemCollection();
            if (items != null)
                collection._items.AddRange(items);
            ModHook.OnModItemDirectoryInit += collection.RegisterItems;
            return collection;
        }

        public void RegisterItems(ModItemDirectory directory)
        {
            foreach (ProbablyHelpfulItem item in _items)
            {
                ProbablyHelpfulItem captured = item;
                if (!directory.Add(captured.ItemId, () =>
                {
                    GameItem created = captured.Build();
                    LoadedMod mod = ModLoader.Mods.Find(m => m.Manifest.ID == captured.ModId);
                    string modName = mod?.Manifest.Name;
                    ModHelper.SetSource(created, captured.ModId, string.IsNullOrEmpty(modName) ? captured.ModId : modName);
                    return created;
                }))
                    UnityEngine.Debug.LogWarning("[ProbablyHelpful] Failed to register item " + captured.ItemId + " (duplicate id?)");
            }
        }
    }
}
