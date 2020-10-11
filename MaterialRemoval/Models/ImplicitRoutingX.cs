using g3;

namespace MaterialRemoval.Models
{
    public class ImplicitRoutingX : ImplicitAxAlignedRouting
    {
        protected override int DirKey => 0;

        #region ctor

        public ImplicitRoutingX(double length, double radius, int toolId, bool posDirection) : base(length, radius, toolId, posDirection ? 1.0 : -1.0)
        {
        }

        #endregion

        #region overridden

        protected override Vector2d GetPlaneComponent(ref Vector3d v) => v.yz;

        protected override AxisAlignedBox3d ToolPositionBox(ref Vector3d position)
        {
            var center = position;

            center.x += (_length / 2.0) * _axisComponent;

            return new AxisAlignedBox3d(center, _length / 2.0, _radius, _radius);
        }

        #endregion
    }
}
