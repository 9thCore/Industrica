using Industrica.ClassBase;
using Industrica.Item.Generic;
using Industrica.Register.EcoTarget;
using Industrica.Save;
using Industrica.Utility;
using Industrica.World.OreVein;
using System.Collections;
using UnityEngine;
using UWE;

namespace Industrica.Machine.Mining
{
    public class CoreSampleDrill : BaseMachineExternal
    {
        public GenericHandTarget handTarget;
        public Collider[] colliders;

        private float drillTimeRemaining = DrillTimeRequired;
        private TechType coreSample = TechType.None;
        private bool running = false;
        private SaveData data;
        private bool pickingUp = false;
        private bool validPlacement = false;

        public override float PowerRelaySearchRange => 20f;

        public CoreSampleDrill WithHandTarget(GenericHandTarget handTarget)
        {
            this.handTarget = handTarget;
            handTarget.enabled = false;
            return this;
        }

        public void GatherColliders()
        {
            colliders = GetComponentsInChildren<Collider>(true);
        }

        public override void Start()
        {
            base.Start();

            handTarget.onHandClick = new();
            handTarget.onHandClick.AddListener(OnHandClick);
            handTarget.onHandHover = new();
            handTarget.onHandHover.AddListener(OnHandHover);

            if (TryGetOreVein(transform.position, out AbstractOreVein oreVein)
                && Vector3.SqrMagnitude(transform.position - oreVein.transform.position) <= oreVein.RangeSquared)
            {
                coreSample = oreVein.CoreSampleTechType;
            } else
            {
                coreSample = ItemsBasic.CoreSampleEmpty.TechType;
            }

            data = new SaveData(this);
        }

        public void Update()
        {
            if (!running
                || !validPlacement
                || !StillDrilling())
            {
                return;
            }

            ConsumeEnergyPerSecond(EnergyUsagePerSecond, out _);
            if (!IsPowered())
            {
                return;
            }

            drillTimeRemaining -= DayNightCycle.main.deltaTime;
            if (!StillDrilling()
                && !TryAddSample())
            {
                ErrorMessage.AddMessage("Ready_IndustricaCoreSampleDrill".Translate());
            }
        }

        public void OnDestroy()
        {
            if (data == null)
            {
                return;
            }

            data.Invalidate();
        }

        public void Reset()
        {
            drillTimeRemaining = DrillTimeRequired;
        }

        public bool StillDrilling()
        {
            return drillTimeRemaining > 0f;
        }

        public bool TryAddSample()
        {
            if (pickingUp
                || Vector3.SqrMagnitude(Player.main.transform.position - transform.position) >= MaxPickupRangeSquared)
            {
                return false;
            }

            running = false;
            pickingUp = true;
            CoroutineHost.StartCoroutine(AddSampleAsync());
            return true;
        }

        private IEnumerator AddSampleAsync()
        {
            if (coreSample == TechType.None)
            {
                pickingUp = false;
                Reset();
                yield break;
            }

            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(coreSample, false);
            yield return task;

            GameObject result = task.GetResult();
            if (result == null
                || !result.TryGetComponent(out Pickupable pickupable))
            {
                pickingUp = false;
                Reset();
                yield break;
            }

            if (!Inventory.main.HasRoomFor(pickupable))
            {
                ErrorMessage.AddMessage("InventoryFull".Translate());
                yield break;
            }

            GameObject instance = GameObject.Instantiate(result);
            Inventory.main.ForcePickup(instance.GetComponent<Pickupable>());
            Player.main.PlayGrab();

            Reset();
            pickingUp = false;
        }

        public void OnHandHover(HandTargetEventData data)
        {
            if (!validPlacement)
            {
                HandReticle.main.SetIcon(HandReticle.IconType.HandDeny);
                HandReticle.main.SetText(HandReticle.TextType.Hand, "Invalid_IndustricaCoreSampleDrill", true);
                HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "InvalidSubscript_IndustricaCoreSampleDrill", true);
                return;
            }

            if (!running)
            {
                HandReticle.main.SetIcon(HandReticle.IconType.Hand);
                HandReticle.main.SetText(HandReticle.TextType.Hand, "Activate_IndustricaCoreSampleDrill", true, GameInput.Button.LeftHand);
                HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "ActivateSubscript_IndustricaCoreSampleDrill", true);
                return;
            }

            if (StillDrilling())
            {
                HandReticle.main.SetIcon(HandReticle.IconType.Progress, 1.5f);
                HandReticle.main.SetProgress((DrillTimeRequired - drillTimeRemaining) / DrillTimeRequired);
                HandReticle.main.SetText(HandReticle.TextType.Hand, "Working_IndustricaCoreSampleDrill", true);
                if (!IsPowered())
                {
                    HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "unpowered", true);
                }
            } else
            {
                HandReticle.main.SetIcon(HandReticle.IconType.Hand);
                HandReticle.main.SetText(HandReticle.TextType.Hand, "Finished_IndustricaCoreSampleDrill", true, GameInput.Button.LeftHand);
            }
        }

        public void OnHandClick(HandTargetEventData data)
        {
            if (!validPlacement)
            {
                return;
            }

            if (!running)
            {
                SetState(true);
                return;
            }

            if (StillDrilling())
            {
                return;
            }

            TryAddSample();
        }

        private void SetState(bool state)
        {
            running = state;
        }

        public static bool TryGetOreVein(Vector3 position, out AbstractOreVein oreVein)
        {
            IEcoTarget target = EcoRegionManager.main.FindNearestTarget(OreVeinEcoTarget.EcoTargetType, position);
            if (target != null
                && target.GetGameObject().TryGetComponent(out oreVein))
            {
                return true;
            }

            oreVein = default;
            return false;
        }

        public static bool CheckValidPlacement(Vector3 position, Vector3 direction)
        {
            return Physics.Raycast(position, direction, out RaycastHit info, RaycastDistance, Builder.placeLayerMask, QueryTriggerInteraction.Ignore)
                && info.transform.gameObject.layer == LayerID.TerrainCollider;
        }

        public const float RaycastDistance = 0.5f;
        public const float MaxPickupRange = 5f;
        public const float MaxPickupRangeSquared = MaxPickupRange * MaxPickupRange;
        public const float DrillTimeRequired = 10f;
        public const float EnergyUsagePerSecond = 0.5f;

        public class SaveData : ComponentSaveData<SaveData, CoreSampleDrill>
        {
            public float drillTimeRemaining = 0f;
            public TechType coreSample = TechType.None;
            public bool running = false;
            public bool validPlacement = false;

            public SaveData(CoreSampleDrill component) : base(component) { }

            public override SaveSystem.SaveData<SaveData> SaveStorage => SaveSystem.Instance.coreSampleDrillSaveData;

            public override void CopyFromStorage(SaveData data)
            {
                drillTimeRemaining = data.drillTimeRemaining;
                coreSample = data.coreSample;
                running = data.running;
                validPlacement = data.validPlacement;
            }

            public override void Load()
            {
                Component.drillTimeRemaining = drillTimeRemaining;
                Component.coreSample = coreSample;
                Component.running = running;
                Component.validPlacement = validPlacement;
                Component.handTarget.enabled = true;
            }

            public override void Save()
            {
                drillTimeRemaining = Component.drillTimeRemaining;
                coreSample = Component.coreSample;
                running = Component.running;
                validPlacement = Component.validPlacement;
            }

            public override void Initialise()
            {
                Component.colliders.ForEach(collider => collider.enabled = false);
                Component.validPlacement = CheckValidPlacement(Component.transform.position + Component.transform.up * 0.1f, -Component.transform.up);
                Component.colliders.ForEach(collider => collider.enabled = true);

                Component.handTarget.enabled = true;
            }
        }
    }
}
