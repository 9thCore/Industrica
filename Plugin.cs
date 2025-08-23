using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Industrica.Buildable.Electrical;
using Industrica.Buildable.Pump;
using Industrica.Buildable.Storage;
using Industrica.Item.Filter;
using Industrica.Item.Tool;
using Industrica.Network;
using Industrica.Network.Physical.Item;
using Industrica.Network.Systems;
using Industrica.Network.Wire;
using Industrica.Operation;
using Industrica.Patch.Vanilla;
using Industrica.Patch.Vanilla.Build;
using Industrica.Register.Equipment;
using Industrica.Save;
using Industrica.Utility;
using Nautilus.Handlers;
using Nautilus.Utility;

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
            InitializePrefabs();
            InitializeLanguage();
            InitializeSave();

            new Harmony(PluginInfo.PLUGIN_GUID).PatchAll();

            Logger.LogInfo($"Successfully loaded [{PluginInfo.PLUGIN_NAME} {PluginInfo.PLUGIN_VERSION}]! Happy engineering!");
        }

        private void InitializeLanguage()
        {
            LanguageHandler.RegisterLocalizationFolder("Localization/Buildable");
            LanguageHandler.RegisterLocalizationFolder("Localization/Equipment");
            LanguageHandler.RegisterLocalizationFolder("Localization/Item");
            LanguageHandler.RegisterLocalizationFolder("Localization/Loading");
            LanguageHandler.RegisterLocalizationFolder("Localization/Port/Generic");
            LanguageHandler.RegisterLocalizationFolder("Localization/Port/Item");
            LanguageHandler.RegisterLocalizationFolder("Localization/Port/Wire");
        }
        
        private void InitializeSave()
        {
            SaveSystem.Register();
        }

        private void InitializePrefabs()
        {
            BaseModOperations.Register();

            PlacedItemTransferPipe.Register();
            ItemPhysicalNetwork.RegisterPrefab();
            BuildableItemPump.Register();
            PlacedWire.Register();
            BuildableElectricLever.Register();
            BuildableElectricOperator.Register();
            BuildableElectricSplitter.Register();
            BuildableElectricTimer.Register();
            ItemMultiTool.Register();
            BaseModConnectionTools.Register();
            ItemTechTypeFilter.Register();
            BuildableFilterLocker.Register();

            VanillaPatch.Register();
        }

        private void InitializeEquipment()
        {
            FilterEquipment.RegisterSlots();
        }
    }
}