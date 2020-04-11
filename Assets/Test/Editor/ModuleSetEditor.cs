using UnityEngine;
using UnityEditor;

class ModuleSetEditor
{
    [MenuItem("Assets/Create/WfcMaze/Module Set")]
    static void CreateModuleSet()
    {
        var mset = ScriptableObject.CreateInstance<ModuleSet>();
        ProjectWindowUtil.CreateAsset(mset, "New Module Set.asset");
    }
}
