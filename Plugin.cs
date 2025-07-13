using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Industrica.Buildable.ConnectableTest;
using Industrica.Buildable.SteamReactor;
using Industrica.Item.Network;
using Industrica.Item.Network.Placed;
using Industrica.Network.Systems;
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

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        private void Awake()
        {
            // set project-scoped logger instance
            Logger = base.Logger;

            // Initialize custom prefabs
            InitializePrefabs();
            InitializeLanguage();
            InitializeSave();

            // register harmony patches, if there are any
            Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
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
            SteamReactor.Register();
            PlacedItemTransferPipe.Register();
            ItemTransferPipe.Register();
            ConnectorTest.Register();
            ItemPhysicalNetwork.RegisterPrefab();
        }
    }
}