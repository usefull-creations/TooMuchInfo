using HarmonyLib;
using GorillaNetworking;

namespace TooMuchInfo
{
    [HarmonyPatch(typeof(GorillaComputer))]
    public class CustomComputerPatches
    {
        public static int currentPage = 0;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void StartPostfix(GorillaComputer __instance)
        {
            __instance.OrderList.RemoveAll(x => x.State == GorillaComputer.ComputerState.Troop || x.State == GorillaComputer.ComputerState.Redemption);

            __instance.OrderList.Add(new GorillaComputer.StateOrderItem(GorillaComputer.ComputerState.Troop, "TMI"));
        }

        static string GetToggleText(int optionIndex, string name, bool value)
        {
            return $"OPTION {optionIndex + 1}: {name} [{(value ? "ON" : "OFF")}]\n\n";
        }

        [HarmonyPatch("TroopScreen")]
        [HarmonyPrefix]
        static bool TroopScreenPrefix(GorillaComputer __instance)
        {
            __instance.screenText.Set("TOO MUCH INFO (PRESS ENTER FOR NEXT PAGE)\n\n");
            
            if (currentPage == 0)
            {
                __instance.screenText.Append(GetToggleText(0, "SHOW DATE", Plugin.ShowCreationDate));
                __instance.screenText.Append(GetToggleText(1, "SHOW COLOR", Plugin.ShowColor));
                __instance.screenText.Append(GetToggleText(2, "SHOW PLATFORM", Plugin.ShowPlatform));
            }
            else if (currentPage == 1)
            {
                __instance.screenText.Append(GetToggleText(0, "SHOW COSMETICS", Plugin.ShowCosmetics));
                __instance.screenText.Append(GetToggleText(1, "SHOW MODS", Plugin.ShowMods));
                __instance.screenText.Append(GetToggleText(2, "SHOW TAGGED", Plugin.ShowTagged));
            }
            else if (currentPage == 2)
            {
                __instance.screenText.Append(GetToggleText(0, "SHOW FPS", Plugin.ShowFPS));
                __instance.screenText.Append(GetToggleText(1, "TURN SETTINGS", Plugin.ShowTurnSettings));
                __instance.screenText.Append(GetToggleText(2, "FRIEND NAMES", Plugin.ShowFriendNames));
            }
            
            return false; // Skip original
        }

        [HarmonyPatch("ProcessTroopState")]
        [HarmonyPrefix]
        static bool ProcessTroopStatePrefix(GorillaKeyboardBindings buttonPressed)
        {
            if (buttonPressed == GorillaKeyboardBindings.enter)
            {
                currentPage++;
                if (currentPage > 2) currentPage = 0;
            }
            else if (buttonPressed == GorillaKeyboardBindings.option1)
            {
                if (currentPage == 0) Plugin.ShowCreationDate = !Plugin.ShowCreationDate;
                else if (currentPage == 1) Plugin.ShowCosmetics = !Plugin.ShowCosmetics;
                else if (currentPage == 2) Plugin.ShowFPS = !Plugin.ShowFPS;
            }
            else if (buttonPressed == GorillaKeyboardBindings.option2)
            {
                if (currentPage == 0) Plugin.ShowColor = !Plugin.ShowColor;
                else if (currentPage == 1) Plugin.ShowMods = !Plugin.ShowMods;
                else if (currentPage == 2) Plugin.ShowTurnSettings = !Plugin.ShowTurnSettings;
            }
            else if (buttonPressed == GorillaKeyboardBindings.option3)
            {
                if (currentPage == 0) Plugin.ShowPlatform = !Plugin.ShowPlatform;
                else if (currentPage == 1) Plugin.ShowTagged = !Plugin.ShowTagged;
                else if (currentPage == 2) Plugin.ShowFriendNames = !Plugin.ShowFriendNames;
            }
            
            return false; // Skip original
        }

        [HarmonyPatch("SupportScreen")]
        [HarmonyPrefix]
        static bool SupportScreenPrefix(GorillaComputer __instance)
        {
            __instance.screenText.Set("SUPPORT\n\nPLAYER ID  ");
            
            if (PlayFabAuthenticator.instance != null)
                __instance.screenText.Append(PlayFabAuthenticator.instance.GetPlayFabPlayerId());
            else
                __instance.screenText.Append("UNKNOWN");
                
            __instance.screenText.Append("\n\n\ndiscord.gg/poopoovr\n\nRemade by poopooVR");
            return false;
        }
    }
}
