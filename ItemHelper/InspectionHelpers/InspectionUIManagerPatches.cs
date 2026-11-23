using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace ProbablyHelpful
{
    [HarmonyPatch(typeof(InspectionUIManager))]
    public static class InspectionUIManagerPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(InspectionUIManager.OnInspectItem))]
        public static void OnInspectItemPostfix(GameItem gameItem)
        {
            CustomInspectionRegistry.HandleItem(gameItem);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(InspectionUIManager.ResetUI))]
        public static void ResetUIPostfix()
        {
            CustomInspectionRegistry.ResetAll();
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(InspectionUIManager.OpenUI))]
        public static void OpenUIPostfix()
        {
            CustomInspectionRegistry.ResetPositions();
        }
    }
}