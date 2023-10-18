using System;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

public class DishongTowerChallenge
{
    public class DishongTowerChallenge_Init : IModApi
    {
        public void InitMod(Mod _modInstance)
        {
            Log.Out(" Loading Patch: " + this.GetType().ToString());

            // Reduce extra logging stuff
            Application.SetStackTraceLogType(UnityEngine.LogType.Log, StackTraceLogType.None);
            Application.SetStackTraceLogType(UnityEngine.LogType.Warning, StackTraceLogType.None);

            var harmony = new HarmonyLib.Harmony(GetType().ToString());
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(XUiC_NewContinueGame))]
    [HarmonyPatch("startGame")]
    [HarmonyPatch(new Type[] {  })]
    public class Patch_XUiC_NewContinueGame_startGame
    {
        static bool Prefix(XUiC_NewContinueGame __instance)
        {
            GamePrefs.Set(EnumGamePrefs.XPMultiplier, 100);
            GamePrefs.Set(EnumGamePrefs.LootAbundance, 100);
            GamePrefs.Set(EnumGamePrefs.BlockDamagePlayer, 100);
            GamePrefs.Set(EnumGamePrefs.BlockDamageAI, 100);
            GamePrefs.Set(EnumGamePrefs.BlockDamageAIBM, 100);
            GamePrefs.Set(EnumGamePrefs.PlayerKillingMode, 2);
            GamePrefs.Set(EnumGamePrefs.AirDropFrequency, 0);
            GamePrefs.Set(EnumGamePrefs.EnemySpawnMode, true);

            return true;
        }
    }
}

