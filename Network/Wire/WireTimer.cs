using Industrica.ClassBase;
using Industrica.Save;
using Industrica.UI.UIData;
using System.Collections.Generic;
using System.Linq;

namespace Industrica.Network.Wire
{
    public class WireTimer : BaseMachine
    {
        public WirePort input, timer, output;
        public WirePortUIData inputUIData, timerUIData, outputUIData;
        public Queue<DataUpdate> updates = new();
        private SaveData save;

        public void SetPorts(WirePort input, WirePort timer, WirePort output)
        {
            this.input = input;
            this.timer = timer;
            this.output = output;

            inputUIData = input.gameObject.EnsureComponent<WirePortUIData>().WithPort(input);
            timerUIData = timer.gameObject.EnsureComponent<WirePortUIData>().WithPort(timer);
            outputUIData = output.gameObject.EnsureComponent<WirePortUIData>().WithPort(output);
        }

        public override void Start()
        {
            base.Start();

            input.OnCharge += OnCharge;
            timerUIData.OnUpdate += OnTimerUpdate;
            save = new SaveData(this);

            OnTimerUpdate();
        }

        public void OnDestroy()
        {
            save.Invalidate();
        }

        public void Update()
        {
            ConsumeEnergyPerSecond(EnergyUsage, out _);

            if (updates.Count == 0)
            {
                return;
            }

            updates.ForEach(update => update.Update());
            if (updates.Peek().Done())
            {
                output.SetElectricity(updates.Dequeue().value);
            }
        }

        private void OnCharge()
        {
            updates.Enqueue(new DataUpdate(timer.value + OutputDelay, input.value));
        }

        private void OnTimerUpdate()
        {
            // lol
            timerUIData.text.text += "s";
        }

        public const float OutputDelay = 0.15f;
        public const float EnergyUsage = 1f;

        public class DataUpdate
        {
            public float remainingDelay;
            public int value;

            public DataUpdate(float remainingDelay, int value)
            {
                this.remainingDelay = remainingDelay;
                this.value = value;
            }

            public void Update()
            {
                remainingDelay -= DayNightCycle.main.deltaTime;
            }

            public bool Done()
            {
                return remainingDelay <= 0f;
            }
        }

        public class SaveData : ComponentSaveData<SaveData, WireTimer>
        {
            public Queue<DataUpdate> updates;

            public SaveData(WireTimer component) : base(component) { }

            public override SaveSystem.SaveData<SaveData> SaveStorage => SaveSystem.Instance.wireTimerSaveData;

            public override void CopyFromStorage(SaveData data)
            {
                updates = data.updates;
            }

            public override void Load()
            {
                Component.updates = updates;
            }

            public override void Save()
            {
                updates = new(Component.updates);
            }

            public override bool IncludeInSave()
            {
                return updates.Any();
            }
        }
    }
}
