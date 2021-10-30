using System.Collections.Generic;
using System.Linq;

namespace Wfc {

public static class ModuleRegistry
{
    #region Public members

    public static int Count
      => _modules.Length;

    public static Connectivity GetConnectivity(int i)
      => _modules[i];

    public static void Reset(IEnumerable<Connectivity> e)
      => _modules = e.ToArray();

    #endregion

    #region Private members

    static Connectivity[] _modules;

    #endregion
}

} //namespace Wfc
