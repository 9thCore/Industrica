using Industrica.ClassBase;
using Industrica.Item.Generic;
using Industrica.Register.EcoTarget;
using Industrica.Save;
using Industrica.World.OreVein;
using System.Collections;
using UnityEngine;
using UWE;

namespace Industrica.Machine.Mining
{
    public class Drill : BaseMachineExternal
    {
        public StorageContainer storageContainer;
        public GenericHandTarget handTarget;
        public Collider[] colliders;

        private float drillTimeRemaining = DrillTimeRequired;
        private TechType resource = TechType.None;
        private SaveData data;
        private bool validPlacement = false;

        public override float PowerRelaySearchRange => 20f;

        public Drill WithStorageContainer(StorageContainer storageContainer)
        {
            this.storageContainer = storageContainer;
            return this;
        }

        public Drill WithHandTarget(GenericHandTarget handTarget)
        {
            this.handTarget = handTarget;
            return this;
        }

        public void GatherColliders()
        {
            colliders = GetComponentsInChildren<Collider>(true);
        }

        public override void Start()
        {
            base.Start();

            storageContainer.isValidHandTarget = false;

            handTarget.onHandClick = new();
            handTarget.onHandClick.AddListener(OnHandClick);
            handTarget.onHandHover = new();
            handTarget.onHandHover.AddListener(OnHandHover);

            if (TryGetOreVein(transform.position, out AbstractOreVein oreVein)
                && Vector3.SqrMagnitude(transform.position - oreVein.transform.position) <= oreVein.RangeSquared)
            {
                resource = oreVein.ResourceTechType;
            }
            else
            {
                resource = ItemsBasic.OreVeinResourceEmpty.TechType;
            }

            data = new SaveData(this);
        }

        public void Update()
        {
            if (!validPlacement
                || !HasSpace())
            {
                return;
            }

            ConsumeEnergyPerSecond(EnergyUsagePerSecond, out _);
            if (!IsPowered())
            {
                return;
            }

            drillTimeRemaining -= DayNightCycle.main.deltaTime;
            if (drillTimeRemaining <= 0f)
            {
                TryAddItem();
                Reset();
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

        public void TryAddItem()
        {
            if (resource == TechType.None)
            {
                return;
            }

            CoroutineHost.StartCoroutine(TryAddItemAsync());
        }

        private IEnumerator TryAddItemAsync()
        {
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(resource, false);
            yield return task;

            GameObject result = task.GetResult();
            if (result == null
                || !result.TryGetComponent(out Pickupable pickupable))
            {
                yield break;
            }

            if (!storageContainer.container.HasRoomFor(pickupable))
            {
                yield break;
            }

            GameObject instance = UWE.Utils.InstantiateDeactivated(result);
            storageContainer.container.AddItem(instance.GetComponent<Pickupable>());
        }

        public void Reset()
        {
            drillTimeRemaining = DrillTimeRequired;
        }

        public bool HasSpace()
        {
            return storageContainer.container.HasRoomFor(resource);
        }

        public void OnHandClick(HandTargetEventData data)
        {
            if (!validPlacement)
            {
                return;
            }

            storageContainer.OnHandClick(Player.main.guiHand);
        }

        public void OnHandHover(HandTargetEventData data)
        {
            if (!validPlacement)
            {
                HandReticle.main.SetIcon(HandReticle.IconType.HandDeny);
                HandReticle.main.SetText(HandReticle.TextType.Hand, "Invalid_IndustricaDrill", true);
                HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "InvalidSubscript_IndustricaDrill", true);
                return;
            }

            if (HasSpace())
            {
                HandReticle.main.SetIcon(HandReticle.IconType.Progress, 1.5f);
                HandReticle.main.SetProgress((DrillTimeRequired - drillTimeRemaining) / DrillTimeRequired);
                HandReticle.main.SetText(HandReticle.TextType.Hand, "Working_IndustricaDrill", true, GameInput.Button.LeftHand);
                if (!IsPowered())
                {
                    HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "unpowered", true);
                }
            }
            else
            {
                HandReticle.main.SetIcon(HandReticle.IconType.Hand);
                HandReticle.main.SetText(HandReticle.TextType.Hand, "Full_IndustricaDrill", true, GameInput.Button.LeftHand);
                HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "FullSubscript_IndustricaDrill", true);
            }
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
        public const float DrillTimeRequired = 30f;
        public const float EnergyUsagePerSecond = 2f;

        public class SaveData : ComponentSaveData<SaveData, Drill>
        {
            public float drillTimeRemaining = 0f;
            public TechType resource = TechType.None;
            public bool validPlacement = false;

            public SaveData(Drill component) : base(component) { }

            public override SaveSystem.SaveData<SaveData> SaveStorage => SaveSystem.Instance.drillSaveData;

            public override void CopyFromStorage(SaveData data)
            {
                drillTimeRemaining = data.drillTimeRemaining;
                resource = data.resource;
                validPlacement = data.validPlacement;
            }

            public override void Load()
            {
                Component.drillTimeRemaining = drillTimeRemaining;
                Component.resource = resource;
                Component.validPlacement = validPlacement;
            }

            public override void Save()
            {
                drillTimeRemaining = Component.drillTimeRemaining;
                resource = Component.resource;
                validPlacement = Component.validPlacement;
            }

            public override void Initialise()
            {
                Component.colliders.ForEach(collider => collider.enabled = false);
                Component.validPlacement = CheckValidPlacement(Component.transform.position + Component.transform.up * 0.1f, -Component.transform.up);
                Component.colliders.ForEach(collider => collider.enabled = true);
            }
        }
    }
}
