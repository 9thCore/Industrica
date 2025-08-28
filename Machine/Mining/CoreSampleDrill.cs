using Industrica.ClassBase;
using Industrica.Save;
using Industrica.Utility;
using System.Collections;
using UnityEngine;
using UWE;

namespace Industrica.Machine.Mining
{
    public class CoreSampleDrill : BaseMachineExternal
    {
        public GenericHandTarget handTarget;

        private float drillTimeRemaining = DrillTimeRequired;
        private SaveData data;

        public override float PowerRelaySearchRange => 20f;

        public CoreSampleDrill WithHandTarget(GenericHandTarget handTarget)
        {
            this.handTarget = handTarget;
            return this;
        }

        public override void Start()
        {
            base.Start();

            handTarget.onHandClick = new();
            handTarget.onHandClick.AddListener(OnHandClick);
            handTarget.onHandHover = new();
            handTarget.onHandHover.AddListener(OnHandHover);

            data = new SaveData(this);
        }

        public void Update()
        {
            if (!StillDrilling())
            {
                return;
            }

            ConsumeEnergyPerSecond(EnergyUsagePerSecond, out _);
            if (!IsPowered())
            {
                return;
            }

            drillTimeRemaining -= DayNightCycle.main.deltaTime;
            if (drillTimeRemaining <= 0f
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
            if (Vector3.SqrMagnitude(Player.main.transform.position - transform.position) >= MaxPickupRangeSquared)
            {
                return false;
            }

            CoroutineHost.StartCoroutine(AddSampleAsync());
            Reset();
            return true;
        }

        private static IEnumerator AddSampleAsync()
        {
            yield break;
        }

        public void OnHandHover(HandTargetEventData data)
        {
            if (StillDrilling())
            {
                HandReticle.main.SetIcon(HandReticle.IconType.Progress, 1.5f);
                HandReticle.main.SetProgress((DrillTimeRequired - drillTimeRemaining) / DrillTimeRequired);
                HandReticle.main.SetText(HandReticle.TextType.Hand, "Working_IndustricaCoreSampleDrill", true);
            } else
            {
                HandReticle.main.SetIcon(HandReticle.IconType.Hand);
                HandReticle.main.SetText(HandReticle.TextType.Hand, "Finished_IndustricaCoreSampleDrill", true, GameInput.Button.LeftHand);
            }
        }

        public void OnHandClick(HandTargetEventData data)
        {
            if (StillDrilling())
            {
                return;
            }

            TryAddSample();
        }

        public const float MaxPickupRange = 5f;
        public const float MaxPickupRangeSquared = MaxPickupRange * MaxPickupRange;
        public const float DrillTimeRequired = 5f;
        public const float EnergyUsagePerSecond = 10f / 3f;

        public class SaveData : ComponentSaveData<SaveData, CoreSampleDrill>
        {
            public float drillTimeRemaining = 0f;

            public SaveData(CoreSampleDrill component) : base(component) { }

            public override SaveSystem.SaveData<SaveData> SaveStorage => SaveSystem.Instance.coreSampleDrillSaveData;

            public override void CopyFromStorage(SaveData data)
            {
                drillTimeRemaining = data.drillTimeRemaining;
            }

            public override void Load()
            {
                Component.drillTimeRemaining = drillTimeRemaining;
            }

            public override void Save()
            {
                drillTimeRemaining = Component.drillTimeRemaining;
            }
        }
    }
}
