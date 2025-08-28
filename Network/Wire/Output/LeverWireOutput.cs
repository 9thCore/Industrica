using Industrica.ClassBase;
using Industrica.ClassBase.Addons.Machine;
using Industrica.Utility;
using UnityEngine;

namespace Industrica.Network.Wire.Output
{
    public class LeverWireOutput : BaseMachine, IRelayPowerChangeListener
    {
        private readonly SmoothValue indicatorTransition = new(0f, 0.125f);

        public Renderer indicator1, indicator2;
        public GenericHandTarget target;
        public WirePort output;
        public int Value
        {
            get => output.value;
            set => output.SetElectricity(value);
        }

        public LeverWireOutput WithHandTarget(GenericHandTarget target)
        {
            this.target = target;
            return this;
        }

        public LeverWireOutput WithRenderers(Renderer indicator1, Renderer indicator2)
        {
            this.indicator1 = indicator1;
            this.indicator2 = indicator2;
            return this;
        }

        public void SetPort(WirePort output)
        {
            this.output = output;
        }

        public override void Start()
        {
            base.Start();

            target.onHandHover = new HandTargetEvent();
            target.onHandHover.AddListener(OnHover);
            target.onHandClick = new HandTargetEvent();
            target.onHandClick.AddListener(OnClick);

            indicatorTransition.OnFinish += Animate;
            output.OnCharge += OnCharge;
        }

        private void OnCharge()
        {
            indicatorTransition.SetTarget(Value == LeverOff ? 0f : 1f);
        }

        public void Update()
        {
            ConsumeEnergy();
            UpdateIndicator();
        }

        public void ConsumeEnergy()
        {
            if (Value < LeverOn)
            {
                return;
            }

            ConsumeEnergyPerSecond(EnergyUsage, out _);
        }

        public void UpdateIndicator()
        {
            indicatorTransition.Update();
            if (indicatorTransition.IsChanging())
            {
                Animate(indicatorTransition.value);
            }
        }

        public void Animate(float value)
        {
            Color color = Color.Lerp(ColorOff, ColorOn, value);
            indicator1.material.color = color;
            indicator2.material.color = color;
        }

        public void OnHover(HandTargetEventData data)
        {
            HandReticle.main.SetIcon(HandReticle.IconType.Hand);
            HandReticle.main.SetText(HandReticle.TextType.Hand, "IndustricaLever_Toggle", true, GameInput.Button.LeftHand);

            if (!IsPowered())
            {
                HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "unpowered", true);
            }
        }

        public void OnClick(HandTargetEventData data)
        {
            if (!IsPowered())
            {
                return;
            }

            if (Value != LeverOff)
            {
                Value = LeverOff;
                return;
            }

            Value = LeverOn;
        }

        public void PowerUpEvent(PowerRelay relay)
        {
            
        }

        public void PowerDownEvent(PowerRelay relay)
        {
            Value = 0;
        }

        public static readonly Color ColorOn = Color.red;
        public static readonly Color ColorOff = Color.Lerp(ColorOn, Color.black, 0.5f);
        public const float EnergyUsage = 0.05f;
        public const int LeverOff = WirePort.WireDefault;
        public const int LeverOn = LeverOff + 1;
    }
}
