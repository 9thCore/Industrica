using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Industrica.Buildable.Electrical;
using Industrica.Buildable.Pump;
using Industrica.Item.Tool;
using Industrica.Network;
using Industrica.Network.Physical.Item;
using Industrica.Network.Systems;
using Industrica.Network.Wire;
using Industrica.Operation;
using Industrica.Patch.Vanilla;
using Industrica.Patch.Vanilla.Build;
using Industrica.Save;
using Nautilus.Handlers;
using System.Reflection;

namespace Industrica
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("com.9thcore.modularilybased")]
    public class Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger { get; private set; }

        private static Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

        private void Awake()
        {
            // set project-scoped logger instance
            Logger = base.Logger;

            // Initialize custom prefabs
            InitializePrefabs();
            InitializeLanguage();
            InitializeSave();
            InitializePatches();

            Logger.LogInfo($"Successfully loaded [{PluginInfo.PLUGIN_NAME} {PluginInfo.PLUGIN_VERSION}]! Happy engineering!");
        }

        private void InitializeLanguage()
        {
            LanguageHandler.RegisterLocalizationFolder("Localization");
        }

        private void InitializeSave()
        {
            SaveSystem.Register();
        }

        private void InitializePrefabs()
        {
            BaseModOperations.Register();

            PlacedItemTransferPipe.Register();
            ItemTransportPipe.Register();
            ItemPhysicalNetwork.RegisterPrefab();
            BuildableItemPump.Register();
            PlacedWire.Register();
            ItemWireTool.Register();
            BuildableElectricLever.Register();
            BuildableElectricOperator.Register();
            BuildableElectricSplitter.Register();
            BuildableElectricTimer.Register();

            VanillaPatch.Patch();
        }

        private void InitializePatches()
        {
            PatchBioReactor.PatchMethod(harmony);
            PatchNuclearReactor.PatchMethod(harmony);
        }
    }
}