using UnityEngine;

public class ModuleSet : ScriptableObject
{
    [System.Serializable]
    public struct Connectivity
    {
        public Wfc.Axis Left;
        public Wfc.Axis Right;
        public Wfc.Axis Bottom;
        public Wfc.Axis Top;
        public Wfc.Axis Back;
        public Wfc.Axis Front;
    }

    [System.Serializable]
    public struct Module
    {
        public Mesh Mesh;
        public Connectivity Connectivity;
    }

    public Module[] modules;
}
