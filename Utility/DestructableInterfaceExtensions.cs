using Industrica.ClassBase;

namespace Industrica.Utility
{
    public static class DestructableInterfaceExtensions
    {
        public static bool IsAlive(this IDestroyable instance)
        {
            return instance != null && instance.IsInstanceAlive;
        }

        public static bool IsDestroyed(this IDestroyable instance)
        {
            return !instance.IsAlive();
        }
    }
}
