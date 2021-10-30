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

        public static explicit operator Wfc.Connectivity(Connectivity c)
          => new Wfc.Connectivity(c.Left, c.Right,
                                  c.Bottom, c.Top,
                                  c.Back, c.Front);
    }

    [System.Serializable]
    public struct Module
    {
        public Mesh Mesh;
        public Connectivity Connectivity;
    }

    public Module[] modules;
}
