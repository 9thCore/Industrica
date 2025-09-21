using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Industrica.Patch.Vanilla;
using Industrica.Register;
using Industrica.Register.Equipment;
using Industrica.Save;
using Industrica.Utility;
using Nautilus.Handlers;

namespace Industrica
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("com.9thcore.modularilybased")]
    public class Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger { get; private set; }

        private void Awake()
        {
            // set project-scoped logger instance
            Logger = base.Logger;

            // Initialize custom prefabs
            InitializeEquipment();
            InitializeLanguage();
            InitializeSave();

            ItemRegistry.Register();
            BuildableRegistry.Register();
            MiscRegistry.Register();
            OreVeinRegistry.Register();
            VanillaPatch.Register();

            WaitScreenHandler.RegisterEarlyAsyncLoadTask("Industrica", RecipeRegistry.Register);
            WaitScreenHandler.RegisterLateAsyncLoadTask("Industrica", DelayedStart.EnableObjects);

            new Harmony(PluginInfo.PLUGIN_GUID).PatchAll();

            Logger.LogInfo($"Successfully loaded [{PluginInfo.PLUGIN_NAME} {PluginInfo.PLUGIN_VERSION}]! Happy engineering!");
        }

        private void InitializeLanguage()
        {
            LanguageHandler.RegisterLocalizationFolder("Localization/Buildable");
            LanguageHandler.RegisterLocalizationFolder("Localization/Equipment");
            LanguageHandler.RegisterLocalizationFolder("Localization/Item");
            LanguageHandler.RegisterLocalizationFolder("Localization/Loading");
            LanguageHandler.RegisterLocalizationFolder("Localization/OreVein");
            LanguageHandler.RegisterLocalizationFolder("Localization/Port/Generic");
            LanguageHandler.RegisterLocalizationFolder("Localization/Port/Item");
            LanguageHandler.RegisterLocalizationFolder("Localization/Port/Wire");
            LanguageHandler.RegisterLocalizationFolder("Localization/Recipe");
            LanguageHandler.RegisterLocalizationFolder("Localization/UI");
        }
        
        private void InitializeSave()
        {
            SaveSystem.Register();
            OreVeinSaveSystem.Register();
        }

        private void InitializeEquipment()
        {
            FilterEquipment.RegisterSlots();
            GenericNoFilterEquipment.RegisterSlots();
        }
    }
}