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
            port.OnCharge += Update;
            return this;
        }

        public override void SetText(TextMeshProUGUI text)
        {
            base.SetText(text);
            Update();
        }

        public void Update()
        {
            text.text = port.value.ToString("D2");
        }
    }
}
