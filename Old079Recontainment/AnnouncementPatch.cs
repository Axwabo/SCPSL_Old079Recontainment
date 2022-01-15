using HarmonyLib;

namespace Old079Recontainment {
    [HarmonyPatch(typeof(Recontainer079),"UpdateStatus")]
    internal static class AnnouncementPatch {
        private static void Postfix(int engagedGenerators) {
            
        }
    }
}