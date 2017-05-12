using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCommunicationLibraryExample
{
    class Point
    {
        public double x { get; set; }
        public double y { get; set; }

        public double angleTo(Point p)
        {
            const double TOL = 0.00001;

            if (Math.Abs(p.x - this.x) < TOL)
            {
                if(this.y > p.y)
                {
                    return -Math.PI / 2;
                }
                else
                {
                    return Math.PI / 2;
                }
            }

            if (this.x > p.x)
            {
                if (this.y > p.y)
                {
                    return Math.Atan((p.y - this.y) / (p.x - this.x)) - Math.PI;
                }
                else
                {
                    return Math.Atan((p.y - this.y) / (p.x - this.x)) + Math.PI;
                }
            }
            else
            {
                return Math.Atan((p.y - this.y) / (p.x - this.x));
            }
        }

        public double distanceTo(Point p)
        {
            return Math.Sqrt((this.x - p.x)* (this.x - p.x) + (this.y - p.y) * (this.y - p.y));
        }

        public Point(double pX, double pY)
        {
            x = pX;
            y = pY;
        }
    }
}
