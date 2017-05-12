using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCommunicationLibraryExample
{
    class Robot
    {
        [DllImport("..\\..\\..\\Release\\AURACommunicationLibraryDLL.dll", EntryPoint = "InitialiseComm")]
        public static extern void InitialiseComm();

        [DllImport("..\\..\\..\\Release\\AURACommunicationLibraryDLL.dll", EntryPoint = "CommunicateData", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CommunicateData();

        [DllImport("..\\..\\..\\Release\\AURACommunicationLibraryDLL.dll", EntryPoint = "GetData", CallingConvention = CallingConvention.Cdecl)]
        public static extern double getData(int robotNumber, int direction);

        [DllImport("..\\..\\..\\Release\\AURACommunicationLibraryDLL.dll", EntryPoint = "SetRobot", CallingConvention = CallingConvention.Cdecl)]
        public static extern int setRobot(int robotNumber, float leftVelocity, float rightVelocity);


        public Point location { get; set; }

        public double angle { get; set; }

        public int id { get; set; }

        public void driveToPoint(Point p)
        {
            const float straightP = 1.0f;
            const float turnP = 1.5f;

            var angleToTarget = location.angleTo(p);

            var angleError = angle - angleToTarget;
            var dist = location.distanceTo(p);
            
            if (Math.Abs(angleError) < Math.PI / 4)
            {
                setRobot(this.id, (float)(dist * straightP), (float)(-angleError * turnP));
            }
            else
            {
                setRobot(this.id, 0.0f, (float)(-angleError * turnP));
            }
        }

        public Robot(int robotID)
        {
            location = new Point(0, 0);
            angle = 0;
            id = robotID;
        }
    }
}
