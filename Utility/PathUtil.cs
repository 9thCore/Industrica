using Nautilus.Utility;
using System.IO;
using System.Reflection;

namespace Industrica.Utility
{
    public static class PathUtil
    {
        private static string _assemblyPath;
        public static string AssemblyPath => _assemblyPath ??= Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private static string _assetPath;
        public static string AssetPath => _assetPath ??= Path.Combine(AssemblyPath, "Assets");

        public static Atlas.Sprite GetImage(string path)
        {
            string fullPath = Path.Combine(AssetPath, $"{path}.png");
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException();
            }
            return ImageUtils.LoadSpriteFromFile(fullPath);
        }
    }
}
