using FMOD;
using Industrica.UI;
using Industrica.UI.SpriteDataFix;
using System;
using System.Drawing;
using TMPro;
using UnityEngine;
using static Atlas;

namespace Industrica.Utility
{
    public static class UIUtil
    {
        public static Canvas CreateCanvas(this GameObject prefab, Vector2? size = null)
        {
            Canvas canvas = prefab.CreateChild(nameof(Canvas))
                .EnsureComponent<Canvas>()
                .WithScale(x: 0.002f, y: 0.002f);

            RectTransform transform = canvas.GetComponent<RectTransform>();

            if (size != null)
            {
                transform.offsetMin = -size.Value / 2f;
                transform.offsetMax = size.Value / 2f;
            }

            if (prefab.TryGetComponent(out Constructable _))
            {
                prefab.EnsureComponent<ConstructableCanvas>().WithCanvas(canvas);
            }

            return canvas;
        }

        public static GameObject CreateUIElement(this Canvas canvas, Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null)
        {
            return canvas.gameObject.CreateChild($"UIElement{canvas.transform.childCount}", position: position, rotation: rotation, scale: scale);
        }
        
        public static Canvas WithText(this Canvas canvas, Action<TextMeshProUGUI> callback)
        {
            TextMeshProUGUI component = canvas.CreateUIElement().EnsureComponent<TextMeshProUGUI>();
            callback(component);
            return canvas;
        }

        public static Canvas WithBackgroundIcon(this Canvas canvas, CraftData.BackgroundType type, float radius, Vector2 size, Action<uGUI_Icon> callback)
        {
            return WithIcon<BackgroundSpriteDataFix>(canvas, size, fixer => fixer.WithType(type).WithRadius(radius), callback);
        }

        public static Canvas WithIcon<T>(this Canvas canvas, Vector2 size, Action<T> initFixer, Action<uGUI_Icon> callback) where T : BaseSpriteDataFix, new()
        {
            canvas.CreateUIElement()
                .EnsureComponentChained(out T dataFixer)
                .EnsureComponentChained(out uGUI_Icon icon);

            dataFixer.icon = icon;
            initFixer(dataFixer);
            dataFixer.Init(size);

            callback(dataFixer.icon);

            return canvas;
        }
    }
}
