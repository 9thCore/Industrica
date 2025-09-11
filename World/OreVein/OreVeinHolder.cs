using System.Collections.Generic;
using UnityEngine;

namespace Industrica.World.OreVein
{
    public class OreVeinHolder : MonoBehaviour
    {
        private static OreVeinHolder instance;
        public static OreVeinHolder Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject singleton = new GameObject($"Industrica{nameof(OreVeinHolder)}");
                    instance = singleton.AddComponent<OreVeinHolder>();
                }

                return instance;
            }
        }

        public readonly HashSet<AbstractOreVein> AllOreVeins = new();
    }
}
