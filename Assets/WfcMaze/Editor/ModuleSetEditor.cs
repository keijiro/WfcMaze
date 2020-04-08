using UnityEngine;
using UnityEditor;

namespace WfcMaze.Editor
{
    class ModuleSetEditor
    {
        [MenuItem("Assets/Create/Module Set")]
        static void CreateModuleSet()
        {
            ProjectWindowUtil.CreateAsset(
                ScriptableObject.CreateInstance<ModuleSet>(),
                "New Module Set.asset"
            );
        }
    }
}
