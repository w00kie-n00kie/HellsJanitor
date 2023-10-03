using System;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

public class DishongGameOptionSleeperRespawn
{
    //public class DishongGameOptionSleeperRespawn_Init : IModApi
    //{
    //    public void InitMod(Mod _modInstance)
    //    {
    //        Log.Out(" Loading Patch: " + this.GetType().ToString());

    //        // Reduce extra logging stuff
    //        Application.SetStackTraceLogType(UnityEngine.LogType.Log, StackTraceLogType.None);
    //        Application.SetStackTraceLogType(UnityEngine.LogType.Warning, StackTraceLogType.None);

    //        var harmony = new HarmonyLib.Harmony(GetType().ToString());
    //        harmony.PatchAll(Assembly.GetExecutingAssembly());
    //    }
    //}

    [HarmonyPatch(typeof(SleeperVolume))]
    [HarmonyPatch("SetRespawnTime")]
    [HarmonyPatch(new Type[] { typeof(World) })]
    public class Patch_SleeperVolume_SetRespawnTime
    {
        static bool Prefix(SleeperVolume __instance, World _world, ref ulong ___respawnTime)
        {
            int num = WMMGameOptions.GetInt("CustomSleeperRespawnDays");
            if (num <= 0)
            {
                num = 30;
            }
            ___respawnTime = _world.worldTime + (ulong)(num * 24000);
            __instance.wasCleared = true;

            return false;
        }
    }
}

