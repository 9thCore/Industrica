using Nautilus.Utility;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Industrica.Utility
{
    public static class PathUtil
    {
        public static readonly string AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static readonly string AssetPath = Path.Combine(AssemblyPath, "Assets");
        public static readonly string SpritePath = Path.Combine(AssetPath, "Sprite");
        public static readonly string TexturePath = Path.Combine(AssetPath, "Texture");

        public static Sprite GetImage(string path)
        {
            string fullPath = Path.Combine(SpritePath, $"{path}.png");
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException();
            }
            return ImageUtils.LoadSpriteFromFile(fullPath);
        }

        public static Texture GetTexture(string path)
        {
            string fullPath = Path.Combine(TexturePath, $"{path}.png");
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException();
            }
            return ImageUtils.LoadTextureFromFile(fullPath);
        }
    }
}
