using System.Linq;
using Unity.Mathematics;

namespace Wfc
{
    // Direction enums
    // Axis [X/Y/Z] + Sign [P/N]
    enum Direction { XN, XP, YN, YP, ZN, ZP }

    // Pose enums
    enum Pose { XP_YP_ZP, XP_ZP_YN, XP_YN_ZN, XP_ZN_YP,
                YP_XN_ZP, YP_ZP_XN, YP_XP_ZN, YP_ZN_XN,
                XN_YN_ZP, XN_ZP_YN, XN_YP_ZN, XN_ZN_YN,
                YN_XN_ZP, YN_XP_ZN, YN_XN_ZN, YN_ZN_XP,
                ZP_YP_XN, ZP_XN_YN, ZP_YN_XP, ZP_XP_YP,
                ZN_YP_XP, ZN_XP_YN, ZN_YN_XN, ZN_XN_YP }

    static class Geometry
    {
        public const int DirectionCount = 6;
        public const int PoseCount = 3 * 8;

        public static int3 ToVector(Direction d)
        {
            switch (d)
            {
                case Direction.XN: return math.int3(-1, 0, 0);
                case Direction.XP: return math.int3( 1, 0, 0);
                case Direction.YN: return math.int3(0, -1, 0);
                case Direction.YP: return math.int3(0,  1, 0);
                case Direction.ZN: return math.int3(0, 0, -1);
                case Direction.ZP: return math.int3(0, 0,  1);
            }
            return 0; // Error
        }

        public static Direction ToDirection(int3 v)
        {
            if (v.x < 0) return Direction.XN;
            if (v.x > 0) return Direction.XP;
            if (v.y < 0) return Direction.YN;
            if (v.y > 0) return Direction.YP;
            if (v.z < 0) return Direction.ZN;
            return Direction.ZP;
        }

        public static quaternion ToRotation(Pose pose)
          => _poseRotation[(int)pose];

        public static Direction Rotate(Direction d, Pose p)
          => _directionTable[(int)p * DirectionCount + (int)d];

        static quaternion[] _poseRotation
          = Enumerable.Range(0, PoseCount)
            .Select(i => CalculatePoseRotation((Pose)i))
            .ToArray();

        static Direction[] _directionTable
          = Enumerable.Range(0, PoseCount * DirectionCount)
            .Select(i => RotatedDirection((Pose     )(i / DirectionCount),
                                          (Direction)(i % DirectionCount)))
            .ToArray();

        static Direction RotatedDirection(Pose pose, Direction dir)
          => ToDirection((int3)math.round(
                 math.mul(CalculatePoseRotation(pose), ToVector(dir))));

        static quaternion CalculatePoseRotation(Pose pose)
        {
            var hpi = math.PI / 2;

            var i = (int)pose;
            var rx = quaternion.RotateX((i & 3) * hpi);

            if (i < 4 * 4)
                return math.mul(quaternion.RotateZ((i / 4) * hpi), rx);
            else if (i < 5 * 4)
                return math.mul(quaternion.RotateY(+hpi), rx);
            else
                return math.mul(quaternion.RotateY(-hpi), rx);
        }
    }
}
