using Industrica.Utility;
using TMPro;
using UnityEngine;

namespace Industrica.UI.UIData
{
    public class UITranslator : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public string key;

        public void Setup(TextMeshProUGUI text, string key)
        {
            this.text = text;
            this.key = key;
        }

        public void Start()
        {
            UpdateText();
        }

        public void OnEnable()
        {
            Language.main.onLanguageChanged += UpdateText;
        }

        public void OnDisable()
        {
            Language.main.onLanguageChanged -= UpdateText;
        }

        public void UpdateText()
        {
            text.text = key.Translate();
        }
    }
}
