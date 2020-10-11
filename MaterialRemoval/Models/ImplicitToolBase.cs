using g3;
using MaterialRemoval.Enums;

namespace MaterialRemoval.Models
{
    public abstract class ImplicitToolBase : BoundedImplicitFunction3d, IAlongDirectionSemplidicable, IStepIndexed
    {
        protected double _radius;
        protected double _length;
        protected Vector3d _position;

        public int Index { get; set; } = -1;

        public ImplicitToolBase(Vector3d position, double length, double radius)
        {
            _position = position;
            _length = length;
            _radius = radius;
        }

        public abstract AxisAlignedBox3d Bounds();
        public abstract double Value(ref Vector3d pt);

        protected abstract bool IsComparable(ImplicitToolBase tool);
        protected abstract int CheckParallel(Vector3d v);

        public AlongDirectionSemplificationCheckResult Check(ImplicitToolBase tool)
        {
            var result = AlongDirectionSemplificationCheckResult.None;

            if(IsComparable(tool))
            {
                var d = tool._position - _position;
                var p = CheckParallel(d);
                
                if (p > 0) result = AlongDirectionSemplificationCheckResult.GoOn;
                else if (p < 0) result = AlongDirectionSemplificationCheckResult.BackOff;
            }

            return result;
        }

        public bool IsCloseTo(ImplicitToolBase tool, double tolerace = 0.1, double minToolRadius = 10.0, double radiusRate = 0.01)
        {
            bool result = false;
            var d = tool._position - _position;
            var dist = d.Length;

            if(dist <= tolerace)
            {
                result = true;
            }
            else if(_radius > minToolRadius)
            {
                var r = _radius * radiusRate;

                if((dist <= r) && (CheckParallel(d) == 0))
                {
                    result = true;
                }
            }

            return result;
        }
    }
}
