using Industrica.Buildable.Stackable;
using Industrica.ClassBase;
using Industrica.Patch.Buildable.AlienContainmentUnit;

namespace Industrica.Buildable.SteamReactor
{
    public class SteamReactorBehaviour : BaseGenerator
    {
        // Provide a 1.5x boost to nuclear power generation
        public const float PowerPerSecond = BaseNuclearReactor.powerPerSecond / 2;

        private readonly AdjacentModuleSearch<WaterParkGeometry> lookup = new(Base.Direction.Below);

        public WaterParkSteamer waterPark;
        private GenericHandTarget handTarget;

        public override bool CanGenerate => Constructed && waterPark != null && waterPark.Overheating();
        public override float MaxPower => 500f;

        public new void Start()
        {
            base.Start();

            lookup.Link(this);
            lookup.FoundModule += FoundModule;

            handTarget = gameObject.GetComponentInChildren<GenericHandTarget>();
            handTarget.onHandHover.AddListener(OnHover);
        }

        public void OnHover(HandTargetEventData data)
        {
            HandReticle hand = HandReticle.main;
            hand.SetText(HandReticle.TextType.Hand, "IndustricaSteamReactor", false, GameInput.Button.None);


            string subscript = Language.main.GetFormat("Use_IndustricaSteamReactor", PowerInt, MaxPowerInt);
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
    }
}
