namespace Wfc
{
    struct State
    {
        //       -1 : Not determined
        // 0 -  n-1 : Module with Pose 0
        // n - 2n-2 : Module with Pose 1
        // ...

        public static int Count = Geometry.PoseCount * ModuleRegistry.Count;

        public bool IsDetermined => _value >= 0;
        public int Index => _value % ModuleRegistry.Count;
        public Pose Pose => (Pose)(_value / ModuleRegistry.Count);

        public static State Undetermined => new State { _value = -1 };

        public static State Encoded(int value) => new State { _value = value };

        public static State IndexPose(int index, Pose pose)
          => new State { _value = (int)pose * ModuleRegistry.Count + index };

        int _value;
    }
}
