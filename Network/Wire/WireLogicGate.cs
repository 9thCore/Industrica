using Industrica.ClassBase;
using Industrica.Save;
using Industrica.UI.UIData;
using Industrica.Utility;
using System.Collections;
using UnityEngine;

namespace Industrica.Network.Wire
{
    public class WireLogicGate : BaseMachine, IRelayPowerChangeListener
    {
        private readonly OperationWrapper operationWrapper = new();
        public GenericHandTarget target;
        public WirePort input1, input2, output;
        public WirePortUIData input1UIData, input2UIData, outputUIData;
        public OperationWrapperUIData operationUI;
        public Canvas canvas;
        public SaveData save;

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

            save = new(this);
        }

        public void Update()
        {
            ConsumeEnergyPerSecond(EnergyUsage, out _);
        }

        public void OnDestroy()
        {
            save.Invalidate();
        }

        public void PowerUpEvent(PowerRelay relay)
        {
            canvas.gameObject.SetActive(true);
        }

        public void PowerDownEvent(PowerRelay relay)
        {
            canvas.gameObject.SetActive(false);
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

            if (!powerRelay.IsPowered())
            {
                HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "unpowered", true);
            }
        }

        public void OnClick(HandTargetEventData data)
        {
            if (!powerRelay.IsPowered())
            {
                return;
            }

            operationWrapper.CycleOperation();
        }

        public const float OutputDelay = 1f / 15;
        public const float EnergyUsage = 0.1f;

        public class SaveData : ComponentSaveData<SaveData, WireLogicGate>
        {
            public OperationWrapper.Type operation;

            public SaveData(WireLogicGate component) : base(component) { }

            public override SaveSystem.SaveData<SaveData> SaveStorage => SaveSystem.Instance.wireLogicGateData;

            public override void CopyFromStorage(SaveData data)
            {
                operation = data.operation;
            }

            public override void Load()
            {
                Component.operationWrapper.Set(operation);
            }

            public override void Save()
            {
                operation = Component.operationWrapper.type;
            }

            public override bool IncludeInSave()
            {
                return operation != default;
            }
        }
    }
}
