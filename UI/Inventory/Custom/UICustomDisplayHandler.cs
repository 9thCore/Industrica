using Industrica.UI.Inventory.Custom.Info;
using Industrica.Utility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Industrica.UI.Inventory.Custom
{
    public class UICustomDisplayHandler : MonoBehaviour
    {
        public static UICustomDisplayHandler Instance;

        public GameObject displayParent;

        private int registeredCount = 0;
        private readonly HashSet<DisplayInfo> display = new();

        public void Start()
        {
            Instance = this;
            displayParent = gameObject.CreateChild($"Industrica{nameof(UICustomDisplayHandler)}Parent");
        }

        public void Update()
        {
            foreach (DisplayInfo info in display)
            {
                info.Update();
            }
        }

        public void Show(DisplayInfo info)
        {
            info.SetActive(true);
            display.Add(info);
        }

        public void Hide(DisplayInfo info)
        {
            info.SetActive(false);
            display.Remove(info);
        }

        public void Revert()
        {
            foreach (DisplayInfo info in display)
            {
                info.SetActive(false);
            }

            display.Clear();
        }

        public GameObject GetNextChild()
        {
            return displayParent.CreateChild($"Display{registeredCount++}");
        }
    }
}
