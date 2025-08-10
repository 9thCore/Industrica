using Industrica.ClassBase;
using System.Collections;
using UnityEngine;
using UWE;

namespace Industrica.Network.Wire
{
    public class WireSplitter : BaseMachine
    {
        public WirePort input, output1, output2;

        public void SetPorts(WirePort input, WirePort output1, WirePort output2)
        {
            this.input = input;
            this.output1 = output1;
            this.output2 = output2;
        }

        public override void Start()
        {
            base.Start();
            input.OnCharge += OnCharge;
        }

        public void Update()
        {
            ConsumeEnergyPerSecond(EnergyUsage, out _);
        }

        private void OnCharge()
        {
            CoroutineHost.StartCoroutine(ChargeLate());
        }

        private IEnumerator ChargeLate()
        {
            yield return new WaitForSeconds(OutputDelay);
            output1.SetElectricity(input.value / 2);
            output2.SetElectricity(input.value / 2 + input.value % 2);
        }

        public const float OutputDelay = WireLogicGate.OutputDelay;
        public const float EnergyUsage = 0.02f;
    }
}
