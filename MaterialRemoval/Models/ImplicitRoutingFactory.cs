using g3;
using System;

namespace MaterialRemoval.Models
{
    public static class ImplicitRoutingFactory
    {

        public static ImplicitRouting Create(int toolId, Vector3d direction, double length, double radius)
        {
            ImplicitRouting result = null;

            if (IsZero(direction.x) && IsZero(direction.y))
            {
                result = new ImplicitRoutingZ(length, radius, toolId, direction.z > 0);
            }
            else if (IsZero(direction.z) && IsZero(direction.y))
            {
                result = new ImplicitRoutingX(length, radius, toolId, direction.x > 0);
            }
            else if (IsZero(direction.x) && IsZero(direction.z))
            {
                result = new ImplicitRoutingY(length, radius, toolId, direction.y > 0);
            }
            else
            {
                throw new NotImplementedException();
            }

            return result;
        }

        static bool IsZero(double value) => ImplicitFactoryHelper.IsZero(value);
    }
}
