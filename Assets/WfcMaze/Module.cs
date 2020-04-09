using System.Collections.Generic;

namespace WfcMaze
{
    static class ModuleRegistry
    {
        public static int Count => _masks.Count;

        public static DirectionMask GetMask(int i) => _masks[i];

        public static void AddModule(DirectionMask mask) => _masks.Add(mask);

        static List<DirectionMask> _masks = new List<DirectionMask>();
    }
}
