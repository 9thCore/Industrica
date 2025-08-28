using Industrica.ClassBase;
using Industrica.Operation;
using Industrica.UI;
using Industrica.UI.UIData;
using System.Collections;
using UnityEngine;

namespace Industrica.Network.Wire
{
    public class WireLogicGate : BaseMachine
    {
        private readonly OperationWrapper operationWrapper = new();
        public GenericHandTarget target;
        public WirePort input1, input2, output;
        public WirePortUIData input1UIData, input2UIData, outputUIData;
        public OperationWrapperUIData operationUI;

        public WireLogicGate WithTarget(GenericHandTarget target)
        {
            this.target = target;
            return this;
        }

        public void SetPorts(WirePort input1, WirePort input2, WirePort output)
        {
            this.input1 = input1;
            this.input2 = input2;
            this.output = output;

            input1UIData = input1.gameObject.EnsureComponent<WirePortUIData>().WithPort(input1);
            input2UIData = input2.gameObject.EnsureComponent<WirePortUIData>().WithPort(input2);
            outputUIData = output.gameObject.EnsureComponent<WirePortUIData>().WithPort(output);

            operationUI = gameObject.EnsureComponent<OperationWrapperUIData>();
        }

        public override void Start()
        {
            base.Start();

            operationUI.Setup(operationWrapper);

            input1.OnCharge += OnChange;
            input2.OnCharge += OnChange;
            operationWrapper.OnChange += OnChange;

            target.onHandHover = new();
            target.onHandHover.AddListener(OnHover);
            target.onHandClick = new();
            target.onHandClick.AddListener(OnClick);

            operationWrapper.CreateSave(GetComponent<UniqueIdentifier>().Id);
        }

        public void Update()
        {
            ConsumeEnergyPerSecond(EnergyUsage, out _);
        }

        public void OnDestroy()
        {
            operationWrapper.InvalidateSave();
        }

        public void OnChange()
        {
            int current = operationWrapper.Apply(input1.value, input2.value);
            StartCoroutine(ChargeDelayed(current));
        }

        public IEnumerator ChargeDelayed(int current)
        {
            yield return new WaitForSeconds(OutputDelay);
            output.SetElectricity(current);
        }

        public void OnHover(HandTargetEventData data)
        {
            HandReticle.main.SetIcon(HandReticle.IconType.Hand);
            HandReticle.main.SetText(HandReticle.TextType.Hand, "IndustricaOperator_CycleOperation", true, GameInput.Button.LeftHand);

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

            operationWrapper.CycleOperation();
        }

        public const float OutputDelay = 1f / 15;
        public const float EnergyUsage = 0.1f;
    }
}
