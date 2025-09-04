using System.Collections.Generic;
using UnityEngine;
using static VFXParticlesPool;

namespace Industrica.Utility
{
    public class UICustomContainerHandler : MonoBehaviour
    {
        public static UICustomContainerHandler Instance;
        public static uGUI_InventoryTab Tab => Instance.tab;
        public static uGUI_ItemsContainer Torpedo1 => Tab.torpedoStorage[0];
        public static uGUI_ItemsContainer Torpedo2 => Tab.torpedoStorage[1];
        public static uGUI_ItemsContainer Torpedo3 => Tab.torpedoStorage[2];
        public static uGUI_ItemsContainer Torpedo4 => Tab.torpedoStorage[3];

        private uGUI_InventoryTab tab;

        public static void MoveContainerUI(uGUI_ItemsContainer container, Vector2 position)
        {
            if (Instance == null)
            {
                return;
            }

            DataHolder holder = container.gameObject.EnsureComponent<DataHolder>();
            holder.FetchTransform();
            holder.UpdatePosition(position);
            Instance.holders.Enqueue(holder);
        }

        public void Revert()
        {
            while (holders.Count > 0)
            {
                holders.Dequeue().Revert();
            }
        }

        public void DoUpdate()
        {
            foreach (uGUI_ItemsContainer container in Tab.torpedoStorage)
            {
                container.DoUpdate();
            }
        }

        private void Start()
        {
            Instance = this;
            tab = GetComponent<uGUI_InventoryTab>();
        }

        private readonly Queue<DataHolder> holders = new();
        private class DataHolder : MonoBehaviour
        {
            private RectTransform rectTransform;
            public Vector2 oldAnchoredPosition;

            public void FetchTransform()
            {
                rectTransform = GetComponent<RectTransform>();
            }

            public void UpdatePosition(Vector2 position)
            {
                oldAnchoredPosition = rectTransform.anchoredPosition;
                rectTransform.anchoredPosition = position;
            }

            public void Revert()
            {
                rectTransform.anchoredPosition = oldAnchoredPosition;
            }
        }
    }
}
