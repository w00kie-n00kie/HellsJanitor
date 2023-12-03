using System;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

public class HJStopVehiclesPickup_Init : IModApi
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

//[HarmonyPatch(typeof(EntityPlayerLocal))]
//[HarmonyPatch("OnUpdateEntity")]
//[HarmonyPatch(new Type[] { })]
//public class Patch_EntityPlayerLocal_OnUpdateEntity
//{
//    static bool Prefix(EntityPlayerLocal __instance)
//    {
//        if (__instance.bag != null)
//        {
//            var array = __instance.bag.GetSlots();

//            for (int i = 0; i <= array.Length - 1; i++)
//            {
//                if (!array[i].IsEmpty())
//                {
//                    ItemClass itemClass = array[i].itemValue.ItemClass;

//                    if (itemClass.GetItemName() == "vehicleMinibikePlaceable"
//                        || itemClass.GetItemName() == "vehicleMotorcyclePlaceable"
//                        || itemClass.GetItemName() == "vehicle4x4TruckPlaceable"
//                        || itemClass.GetItemName() == "vehicleGyrocopterPlaceable")
//                    {
//                        Log.Out("Patch_EntityPlayerLocal_OnUpdateEntity : " + itemClass.GetItemName());

//                        __instance.Jumping = false;

//                        return false;
//                    }
//                }
//            }
//        }

//        if (__instance.inventory != null)
//        {
//            var array = __instance.inventory.GetSlots();

//            for (int i = 0; i <= array.Length - 1; i++)
//            {
//                if (!array[i].IsEmpty())
//                {
//                    ItemClass itemClass = array[i].itemValue.ItemClass;

//                    if (itemClass.GetItemName() == "vehicleMinibikePlaceable"
//                        || itemClass.GetItemName() == "vehicleMotorcyclePlaceable"
//                        || itemClass.GetItemName() == "vehicle4x4TruckPlaceable"
//                        || itemClass.GetItemName() == "vehicleGyrocopterPlaceable")
//                    {
//                        __instance.Jumping = false;

//                        return false;
//                    }
//                }
//            }
//        }

//        return true;
//    }
//}

[HarmonyPatch(typeof(EntityPlayerLocal))]
[HarmonyPatch("GetSpeedModifier")]
[HarmonyPatch(new Type[] { })]
public class Patch_EntityPlayerLocal_GetSpeedModifier
{
    static bool Prefix(EntityPlayerLocal __instance, ref float __result)
    {
        if (__instance.bag != null)
        {
            var array = __instance.bag.GetSlots();

            for (int i = 0; i <= array.Length - 1; i++)
            {
                if (!array[i].IsEmpty())
                {
                    ItemClass itemClass = array[i].itemValue.ItemClass;

                    if (itemClass.GetItemName() == "vehicleMinibikePlaceable"
                        || itemClass.GetItemName() == "vehicleMotorcyclePlaceable"
                        || itemClass.GetItemName() == "vehicle4x4TruckPlaceable"
                        || itemClass.GetItemName() == "vehicleGyrocopterPlaceable")
                    {
                        // Log.Out("Patch_EntityPlayerLocal_OnUpdateEntity : " + itemClass.GetItemName());

                        __result = 0f;

                        return false;
                    }
                }
            }
        }

        if (__instance.inventory != null)
        {
           var array = __instance.inventory.GetSlots();

            for (int i = 0; i <= array.Length - 1; i++)
            {
                if (!array[i].IsEmpty())
                {
                    ItemClass itemClass = array[i].itemValue.ItemClass;

                    if (itemClass.GetItemName() == "vehicleMinibikePlaceable"
                        || itemClass.GetItemName() == "vehicleMotorcyclePlaceable"
                        || itemClass.GetItemName() == "vehicle4x4TruckPlaceable"
                        || itemClass.GetItemName() == "vehicleGyrocopterPlaceable")
                    {
                        __result = 0f;

                        return false;
                    }
                }
            }
        }

        return true;
    }
}