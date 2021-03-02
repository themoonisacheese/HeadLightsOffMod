using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityModManagerNet;
using UnityEngine;
using System.Reflection;

namespace HeadLightsOffMod
{
    public class Mod
    {
        public static UnityModManager.ModEntry mod; // despite usual coding best practises, leaving your mod fields as public is best, enabling other modders to interact with your mod.
        public static bool enabled;
        static bool Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = new Harmony(modEntry.Info.Id); //register a new harmony patch
            harmony.PatchAll(Assembly.GetExecutingAssembly()); //apply all of our patches

            mod = modEntry;
            modEntry.OnToggle = onToggleEnabled; //function to call when

            return true;
        }

        static bool onToggleEnabled(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;
            return true;
        }




        [HarmonyPatch(typeof(TheDarkKnightManager))]//target TheDarkNightManager Class
        [HarmonyPatch("Start")] //target its Start() method
        static class Mod_Patch
        {                
            //Postfix will run after Start() has returned
            static void Postfix(List<Light> __PlayerHeadLights) //despite PlayerHeadLights not being an argument in Start(), we can request it to be here.
            {
                if (!Mod.enabled)
                    return;

                try // if your code errors out, it doesn't crash the game instantly. however, the function you are patching may not run, and that may cause crashes or instability. thus, best practice is to do anything that might be unsafe in a try catch block.
                {
                    foreach (Light light in __PlayerHeadLights)
                    {
                        light.enabled = false;
                    }
                }
                catch(Exception e)
                {
                    Mod.mod.Logger.Error(e.ToString()); //log errors. logs can be found in the log tab in UMM, by clicking the "show detailed logs" button
                }
            }
        }


    }
}
