using System;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

public class DishongLootBags
{
    public class DishongLootBags_Init : IModApi
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

    [HarmonyPatch(typeof(EntityAlive))]
    [HarmonyPatch("dropItemOnDeath")]
    [HarmonyPatch(new Type[] { })]
    public class Patch_EntityAlive_dropItemOnDeath
    {
        static void Postfix(EntityAlive __instance)
        {
            var entityClass = EntityClass.list[__instance.entityClass];
            if(entityClass == null)
            {
                return;
            }

            var lootDropEntityClass = entityClass.Properties.GetString("DishongLootDropEntityClass");
            if(string.IsNullOrEmpty(lootDropEntityClass))
            {
                return;
            }

            var lootDropEntityClassId = EntityClass.FromString(lootDropEntityClass);
            if(lootDropEntityClassId == -1)
            {
                return;
            }

            var lootDropProb = entityClass.Properties.GetFloat("DishongLootDropProb");

            if (__instance.entityId > 0 &&
                lootDropProb != 0f && 
                lootDropProb > __instance.rand.RandomFloat)
            {
                var vector = __instance.GetPosition();
                vector.y += 0.9f;
                
                EntityLootContainer entityLootContainer = EntityFactory.CreateEntity(lootDropEntityClassId, vector, Vector3.zero) as EntityLootContainer;

                //AccessTools.Field(typeof(EntityLootContainer), "forceInventoryCreate").SetValue(entityLootContainer, true);
                //entityLootContainer.OnUpdateEntity();

                //if (entityLootContainer.lootContainer != null)
                //{
                //    var entityThatKilledMe = AccessTools.Field(typeof(EntityAlive), "entityThatKilledMe").GetValue(__instance) as EntityAlive;

                //    if(entityThatKilledMe != null)
                //    {
                //        GameManager.Instance.lootManager.LootContainerOpened(entityLootContainer.lootContainer, entityThatKilledMe.entityId, entityThatKilledMe.EntityTags);

                //        Log.Out("Patch_EntityAlive_dropItemOnDeath : IsEmpty =  " + entityLootContainer.lootContainer.IsEmpty());
                //    }
                //}

                __instance.world.SpawnEntityInWorld(entityLootContainer);
                entityLootContainer.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
                Audio.Manager.BroadcastPlay(vector, "zpack_spawn", 0f);       
            }
        }
    }
}

