using System.Collections.Generic;

namespace WfcMaze
{
    static class ModuleRegistry
    {
        public static int Count => _modules.Count;

        public static Connectivity GetConnectivity(int i) => _modules[i];

        public static void AddModule(Connectivity conn) => _modules.Add(conn);

        static List<Connectivity> _modules = new List<Connectivity>();
    }
}
