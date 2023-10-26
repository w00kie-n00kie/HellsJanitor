using System;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

public class DishongPickupWorkstations
{
    public class DishongPickupWorkstations_Init : IModApi
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

    [HarmonyPatch(typeof(BlockWorkstation))]
    [HarmonyPatch("GetBlockActivationCommands")]
    [HarmonyPatch(new Type[] { typeof(WorldBase), typeof(BlockValue), typeof(int), typeof(Vector3i), typeof(EntityAlive) })]
    public class Patch_BlockWorkstation_GetBlockActivationCommands
    {
        static bool Prefix(BlockWorkstation __instance, ref BlockActivationCommand[] __result, WorldBase _world, int _clrIdx, Vector3i _blockPos, float ___TakeDelay)
        {
            var tileEntityWorkstation = (TileEntityWorkstation)_world.GetTileEntity(_clrIdx, _blockPos);

            if (tileEntityWorkstation != null 
                && tileEntityWorkstation.IsPlayerPlaced)
            {
                if (___TakeDelay > 0f)
                {
                    __result = new BlockActivationCommand[]
                    {
                        new BlockActivationCommand("open", "campfire", true, false),
                        new BlockActivationCommand("take", "hand", true, false)
                    };

                    return false;
                }
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(BlockDewCollector))]
    [HarmonyPatch("GetBlockActivationCommands")]
    [HarmonyPatch(new Type[] { typeof(WorldBase), typeof(BlockValue), typeof(int), typeof(Vector3i), typeof(EntityAlive) })]
    public class Patch_BlockDewCollector_GetBlockActivationCommands
    {
        static bool Prefix(BlockDewCollector __instance, ref BlockActivationCommand[] __result, float ___TakeDelay)
        {
            if (___TakeDelay > 0f)
            {
                __result = new BlockActivationCommand[]
                {
                    new BlockActivationCommand("Search", "search", true, false),
                    new BlockActivationCommand("take", "hand", true, false)
                };

                return false;
            } 

            return true;
        }
    }
}

