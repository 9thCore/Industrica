using Industrica.Network.Wire;
using TMPro;

namespace Industrica.UI.UIData
{
    public class WirePortUIData : BackedTextUIData
    {
        public WirePort port;

        public WirePortUIData WithPort(WirePort port)
        {
            this.port = port;
            return this;
        }

        public override void SetText(TextMeshProUGUI text)
        {
            base.SetText(text);
            OnCharge();
        }

        public void Start()
        {
            port.OnCharge += OnCharge;
        }

        public void OnCharge()
        {
            text.text = port.value.ToString("D2");
            InvokeUpdate();
        }
    }
}
