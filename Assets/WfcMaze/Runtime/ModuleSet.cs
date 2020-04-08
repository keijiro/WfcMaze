using UnityEngine;

namespace WfcMaze
{
    public class ModuleSet : ScriptableObject
    {
        [System.Serializable]
        public struct WallFlags
        {
            public bool Left;
            public bool Right;
            public bool Down;
            public bool Up;
            public bool Back;
            public bool Front;
        }

        [System.Serializable]
        public struct Module
        {
            public Mesh Mesh;
            public WallFlags WallFlags;
        }

        public Module[] modules;
    }
}
