using Industrica.Buildable.Stackable;
using Industrica.ClassBase;
using Industrica.Patch.Buildable.AlienContainmentUnit;
using Industrica.Save;

namespace Industrica.Buildable.SteamReactor
{
    public class SteamReactorBehaviour : BaseGenerator
    {
        // Provide a 1.5x boost to nuclear power generation
        public const float PowerPerSecond = BaseNuclearReactor.powerPerSecond / 2;

        private readonly AdjacentModuleSearch<WaterParkGeometry> lookup = new(Base.Direction.Below);

        public WaterParkSteamer waterPark;
        private GenericHandTarget handTarget;

        public SaveData save;

        public override bool CanGenerate => Constructed && waterPark != null && waterPark.Overheating();
        public override float MaxPower => 500f;

        public override void Start()
        {
            base.Start();

            lookup.Link(this);
            lookup.FoundModule += FoundModule;

            handTarget = gameObject.GetComponentInChildren<GenericHandTarget>();
            handTarget.onHandHover.AddListener(OnHover);

            save = new(this);
        }

        public void OnHover(HandTargetEventData data)
        {
            HandReticle hand = HandReticle.main;
            hand.SetText(HandReticle.TextType.Hand, "IndustricaSteamReactor", false, GameInput.Button.None);

            string subscript = Language.main.GetFormat($"Use_IndustricaSteamReactor_{(CanGenerate ? "Enabled" : "Disabled")}", PowerInt, MaxPowerInt);
            hand.SetText(HandReticle.TextType.HandSubscript, subscript, true, GameInput.Button.None);
        }

        public void Update()
        {
            if (Constructed)
            {
                lookup.CheckForModule();
            }

            GeneratePowerPerSecond(PowerPerSecond, false, out _);
        }

        public void FoundModule(WaterParkGeometry geometry)
        {
            if (!geometry.TryGetComponent(out WaterParkSteamer steamer))
            {
                lookup.Invalidate();
                return;
            }

            waterPark = steamer;
        }

        public class SaveData : ComponentSaveData<SteamReactorBehaviour>
        {
            public float power;

            public SaveData(SteamReactorBehaviour component) : base(component) { }

            public override void Load()
            {
                TryLoad(SaveSystem.Instance.steamReactorSaveData);
                Component.SetPower(power);
            }

            public override void Save()
            {
                power = Component.Power;
            }

            public override void CopyFromStorage(AbstractSaveData other)
            {
                if (other is not SaveData data)
                {
                    return;
                }

                power = data.power;
            }
        }
    }
}
