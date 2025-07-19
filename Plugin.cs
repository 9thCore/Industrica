using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Industrica.Buildable.Pump;
using Industrica.Item.Network;
using Industrica.Item.Network.Placed;
using Industrica.Network.Systems;
using Industrica.Patch.Vanilla;
using Industrica.Save;
using Nautilus.Handlers;

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
            PlacedItemTransferPipe.Register();
            ItemTransportPipe.Register();
            ItemPhysicalNetwork.RegisterPrefab();
            BuildableItemPump.Register();

            VanillaPatch.Patch(harmony);
        }
    }
}