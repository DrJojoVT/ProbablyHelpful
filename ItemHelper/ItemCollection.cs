using System.Collections.Generic;

namespace ProbablyHelpful
{
    public class ItemCollection
    {
        public string ModId;
        public List<ProbablyHelpfulItem> Items;
        public ItemCollection(string modId)
        {
            ModId = modId;
            Items = new List<ProbablyHelpfulItem>();
        }

        public ItemCollection AddItem(ProbablyHelpfulItem itemHelper)
        {
            Items.Add(itemHelper);
            return this;
        }

        public ItemCollection AddItems(params ProbablyHelpfulItem[] items)
        {
            foreach (ProbablyHelpfulItem itemHelper in items)
            {
                AddItem(itemHelper);
            }
            return this;
        }

        public void RegisterItems(ModItemDirectory directory)
        {
            foreach (ProbablyHelpfulItem itemHelper in Items)
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
    }
}
