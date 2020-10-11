using g3;
using MaterialRemoval.Extensions;
using System;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;

namespace MaterialRemoval.Models
{
    public static class ImplicitToolFactory
    {
        public static ImplicitToolBase Create(Point3D position, Vector3D direction, double length, double radius)
        {
            return Create(position.ToVector3d(), direction.ToVector3d(), length, radius);
        }

        public static ImplicitToolBase Create(Vector3d position, Vector3d direction, double length, double radius)
        {
            ImplicitToolBase result = null;

            if(IsZero(direction.x) && IsZero(direction.y))
            {
                result = new ImplicitToolZ(position, length, radius, direction.z > 0);
            }
            else if (IsZero(direction.z) && IsZero(direction.y))
            {
                result = new ImplicitToolX(position, length, radius, direction.x > 0);
            }
            else if (IsZero(direction.x) && IsZero(direction.z))
            {
                result = new ImplicitToolY(position, length, radius, direction.y > 0);
            }
            else
            {
                throw new NotImplementedException();
            }

            return result;
        }

        private static bool IsZero(double value) => ImplicitFactoryHelper.IsZero(value);

    }
}
