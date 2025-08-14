using Industrica.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Industrica.Network
{
    public class MultiTool : PlayerTool
    {
        private static readonly List<Type> types = new();
        public static void RegisterConnectionTool<T>() where T : ConnectionToolBase
        {
            types.Add(typeof(T));
        }

        public override string animToolName => TechType.Builder.AsString(true);

        public List<ConnectionToolBase> connections;

        private int selectedConnection;
        public int SelectedConnection
        {
            get => selectedConnection;
            set {
                int clamped = math.clamp(value, 0, connections.Count);
                if (selectedConnection == clamped)
                {
                    return;
                }

                Selected.gameObject.SetActive(false);
                selectedConnection = clamped;

                if (HasPower())
                {
                    Selected.gameObject.SetActive(true);
                }
            }
        }

        public Material barMaterial;
        public GameObject stretchedPart, endCap;
        public ConnectionToolBase Selected => connections[selectedConnection];

        public void Setup(Material barMaterial)
        {
            this.barMaterial = barMaterial;
            connections = new();

            types
                .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ConnectionToolBase)))
                .ForEach(InitialiseType);

            if (!connections.Any())
            {
                Plugin.Logger.LogError($"{nameof(MultiTool)}: could not find any non-abstract subclasses of {nameof(ConnectionTool<Port>)}?? Disabling...");
                enabled = false;
            }
        }

        private void InitialiseType(Type type)
        {
            GameObject child = gameObject.CreateChild(type.Name);
            child.SetActive(false);
            ConnectionToolBase component = child.EnsureComponent(type) as ConnectionToolBase;
            component.InitialiseComponent(this);
            connections.Add(component);
        }

        public void Start()
        {
            Selected.gameObject.SetActive(true);
            energyMixin.onPoweredChanged += OnPowerChange;
        }

        private void OnPowerChange(bool powered)
        {
            Selected.gameObject.SetActive(powered);
        }

        public void Cycle()
        {
            SelectedConnection = (SelectedConnection + 1) % connections.Count;
        }

        public bool HasPower()
        {
            return energyMixin.charge > 0f;
        }

        public override bool OnLeftHandDown()
        {
            if (!HasPower())
            {
                return false;
            }

            return Selected.OnLeftHandDown();
        }

        public override bool DoesOverrideHand()
        {
            if (!HasPower())
            {
                return false;
            }

            return Selected.DoesOverrideHand();
        }

        public override bool OnRightHandDown()
        {
            if (!HasPower())
            {
                return false;
            }

            if (Selected.HoveringOccupiedConnection)
            {
                return true;
            }

            if (Selected.Placing)
            {
                Selected.Reset();
                return true;
            }

            Cycle();
            return true;
        }

        public void Update()
        {
            if (HasPower())
            {
                HandReticle.main.SetText(HandReticle.TextType.Use, Selected.UseText, true, GameInput.Button.RightHand);
            }

            float percent = energyMixin.capacity == 0f ? 0f : energyMixin.charge / energyMixin.capacity;
            barMaterial.SetFloat(ShaderPropertyID._Amount, percent);

            if (Selected.Placing
                && GameModeUtils.RequiresPower())
            {
                energyMixin.ConsumeEnergy(EnergyUsagePerSecond * DayNightCycle.main.deltaTime);
            }
        }

        public const float EnergyUsagePerSecond = 100f / 120f;
    }
}
