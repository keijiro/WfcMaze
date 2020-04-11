using System.Linq;
using Unity.Mathematics;

namespace Wfc
{
    // Axis enums
    public enum Axis { None, X, Y, Z }

    // Direction enums
    // Axis [X/Y/Z] + Sign [P/N]
    public enum Direction { XN, XP, YN, YP, ZN, ZP }

    // Pose enums
    public enum Pose { XP_YP_ZP, XP_ZP_YN, XP_YN_ZN, XP_ZN_YP,
                       YP_XN_ZP, YP_ZP_XN, YP_XP_ZN, YP_ZN_XN,
                       XN_YN_ZP, XN_ZP_YN, XN_YP_ZN, XN_ZN_YN,
                       YN_XN_ZP, YN_XP_ZN, YN_XN_ZN, YN_ZN_XP,
                       ZP_YP_XN, ZP_XN_YN, ZP_YN_XP, ZP_XP_YP,
                       ZN_YP_XP, ZN_XP_YN, ZN_YN_XN, ZN_XN_YP }

    // Geometry utilities
    public static class Geometry
    {
        #region Count of enums

        public const int AxisCount = 4;
        public const int DirectionCount = 6;
        public const int PoseCount = 3 * 8;

        #endregion

        #region Direction/Pose helpers

        public static int3 ToVector(this Axis a)
        {
            switch (a)
            {
                case Axis.X : return math.int3(1, 0, 0);
                case Axis.Y : return math.int3(0, 1, 0);
                case Axis.Z : return math.int3(0, 0, 1);
            }
            return 0;
        }

        public static int3 ToVector(this Direction d)
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

        public static quaternion ToRotation(this Pose pose)
          => _poseRotation[(int)pose];

        public static Axis ToAxis(int3 v)
        {
            if (v.x != 0) return Axis.X;
            if (v.y != 0) return Axis.Y;
            if (v.z != 0) return Axis.Z;
            return Axis.None;
        }

        public static Axis ToAxis(float3 v)
          => ToAxis((int3)math.round(v));

        public static Direction ToDirection(int3 v)
        {
            if (v.x < 0) return Direction.XN;
            if (v.x > 0) return Direction.XP;
            if (v.y < 0) return Direction.YN;
            if (v.y > 0) return Direction.YP;
            if (v.z < 0) return Direction.ZN;
            return Direction.ZP;
        }

        public static Direction ToDirection(float3 v)
          => ToDirection((int3)math.round(v));

        public static Axis GetRotated(this Axis axis, Pose pose)
          => _axisPoseTable[(int)pose * AxisCount + (int)axis];

        public static Direction GetRotated(this Direction dir, Pose pose)
          => _dirPoseTable[(int)pose * DirectionCount + (int)dir];

        #endregion

        #region Look up tables and initializers

        static quaternion[] _poseRotation
          = Enumerable.Range(0, PoseCount)
            .Select(i => CalculatePoseRotation((Pose)i))
            .ToArray();

        static Axis[] _axisPoseTable
          = Enumerable.Range(0, PoseCount * AxisCount)
            .Select(i => CalculateRotatedAxis(
                           (Axis)(i % AxisCount),
                           (Pose)(i / AxisCount)))
            .ToArray();

        static Direction[] _dirPoseTable
          = Enumerable.Range(0, PoseCount * DirectionCount)
            .Select(i => CalculateRotatedDirection(
                           (Direction)(i % DirectionCount),
                           (Pose     )(i / DirectionCount)))
            .ToArray();

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

        static Axis CalculateRotatedAxis(Axis axis, Pose pose)
          => ToAxis(math.mul(CalculatePoseRotation(pose), axis.ToVector()));

        static Direction CalculateRotatedDirection(Direction dir, Pose pose)
          => ToDirection(math.mul(CalculatePoseRotation(pose), dir.ToVector()));

        #endregion
    }
}
