using System.Collections.Generic;

namespace Wfc
{
    public static class ModuleRegistry
    {
        #region Public members

        public static int Count => _modules.Count;

        public static Connectivity GetConnectivity(int i) => _modules[i];

        public static void AddModule(Connectivity conn) => _modules.Add(conn);

        #endregion

        #region Private members

        static List<Connectivity> _modules = new List<Connectivity>();

        #endregion
    }
}
