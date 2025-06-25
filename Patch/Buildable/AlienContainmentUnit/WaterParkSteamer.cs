using Industrica.Buildable.Stackable;
using Industrica.Utility;
using UnityEngine;

namespace Industrica.Patch.Buildable.AlienContainmentUnit
{
    public class WaterParkSteamer : MonoBehaviour
    {
        private readonly AdjacentModuleSearch<WaterParkGeometry> lookup = new(Base.Direction.Above);

        private BaseNuclearReactor reactor;
        private WaterParkGeometry geometry;
        private WaterParkSteamer above, below;
        private WaterPark park;
        private bool wasOverheatingLastFrame;
        private bool directReactorConnection = false;
        private SmoothValue steam = new(initialDuration: 4f);
        private ParticleSystem system;
        private MeshRenderer renderer;

        public void Start()
        {
            park = geometry.GetModule();

            lookup.Link(this);
            lookup.FoundModule += FoundModule;

            steam.OnFinish += OnFinish;

            system = gameObject.GetComponentInChildren<ParticleSystem>();
            renderer = gameObject.GetComponentInChildren<MeshRenderer>();
        }

        public float GetHeat()
        {
            return steam.value;
        }

        public bool Overheating()
        {
            if (directReactorConnection)
            {
                return reactor != null && reactor.producingPower;
            }

            if (below == null)
            {
                return false;
            }

            return below.Overheating();
        }

        public void Update()
        {
            UpdateHeat();
            steam.Update();

            if (steam.IsChanging())
            {
                UpdateGeometry(steam.value);
            }

            if (!Overheating())
            {
                return;
            }

            lookup.CheckForModule();

            // Heat up player, but don't if they're currently exiting the park
            // because you can't exit fast enough to escape death
            if (PlayerCinematicController.cinematicModeCount == 0
                && Player.main.currentWaterPark == park)
            {
                Player.main.temperatureDamage.StartLavaDamage();
            }
        }

        private void OnFinish(float value)
        {
            UpdateGeometry(value);
        }

        private void UpdateHeat()
        {
            bool overheatingThisFrame = Overheating();
            if (overheatingThisFrame == wasOverheatingLastFrame)
            {
                return;
            }
            wasOverheatingLastFrame = overheatingThisFrame;

            steam.SetTarget(overheatingThisFrame ? 1f : 0f);
        }

        public void UpdateGeometry(float heat)
        {
            UpdateParticles(heat);
            UpdateGlass(heat);
        }

        private void UpdateParticles(float heat)
        {
            system.startSize = Mathf.Lerp(0.07f, 0.3f, heat);
        }

        private void UpdateGlass(float heat)
        {
            float alpha = Mathf.Lerp(0.31f, 0.85f, heat);
            renderer.material.color = renderer.material.color.WithAlpha(alpha);
        }

        public void FoundModule(WaterParkGeometry geometry)
        {
            WaterParkSteamer steamer = Attach(geometry);
            steamer.SetReactor(reactor, false);
            above = steamer;
            steamer.below = this;
        }

        public void SetReactor(BaseNuclearReactor reactor, bool directReactorConnection)
        {
            this.reactor = reactor;
            this.directReactorConnection = directReactorConnection;
            above?.SetReactor(reactor, false);
        }

        public static WaterParkSteamer Attach(WaterParkGeometry module)
        {
            if (module == null)
            {
                return null;
            }

            WaterParkSteamer steamer = module.gameObject.EnsureComponent<WaterParkSteamer>();
            steamer.geometry = module;

            return steamer;
        }
    }
}
