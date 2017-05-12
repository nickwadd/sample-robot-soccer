using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCommunicationLibraryExample
{
    class Program
    {
        [DllImport("..\\..\\..\\Release\\AURACommunicationLibraryDLL.dll", EntryPoint = "InitialiseComm")]
        public static extern void InitialiseComm();

        [DllImport("..\\..\\..\\Release\\AURACommunicationLibraryDLL.dll", EntryPoint = "CommunicateData", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CommunicateData();

        [DllImport("..\\..\\..\\Release\\AURACommunicationLibraryDLL.dll", EntryPoint = "GetData", CallingConvention = CallingConvention.Cdecl)]
        public static extern double getData(int robotNumber, int direction);

        [DllImport("..\\..\\..\\Release\\AURACommunicationLibraryDLL.dll", EntryPoint = "SetRobot", CallingConvention = CallingConvention.Cdecl)]
        public static extern int setRobot(int robotNumber, float leftVelocity, float rightVelocity);

        static void Main(string[] args)
        {
            InitialiseComm();

            List<Robot> robots = new List<Robot>();

            Point ball = new Point(0,0);
            for (int i = 0; i < 5; i++)
            {
                robots.Add(new Robot(i));
            }
            while (true)
            {
                CommunicateData();


                ball.x = getData(10, 0);
                ball.y = getData(10, 1);

                foreach (var robot in robots)
                {
                    robot.location.x = getData(robot.id, 0);
                    robot.location.y = getData(robot.id, 1);
                    robot.angle = getData(robot.id, 2);

                    robot.driveToPoint(ball);
                }

                

                
                //Print the location and heading of robot 0 (labelled robot 1 in RS program)
                //data 0-4 are my team robots, data 5-9 are opponents and data 10 is ball. Opponent robot and ball only have x and y coordinates (0 and 1) where team robots have a bearing (2)
                //Console.WriteLine("Robot 0 is at location: {0}, {1} and has heading {2}", getData(0, 0), getData(0, 1), getData(0, 2));



                /*
                const float straightP = 1.0f;
                const float turnP = 1.5f;
                
                var robotAngle = getData(0, 2);

                Point robot = new Point(getData(0, 0), getData(0, 1));
                Point target = new Point(1.1, 0.9);

                var angleToTarget = robot.angleTo(target);
                
                var angleError = robotAngle - angleToTarget;
                var dist = robot.distanceTo(target);

                //Console.WriteLine("{0}", angleToTarget);

                //Set the linear and angular velocity (rad/s) of robot 0
                //setRobot(0, 0.2f, -0.1f);

                if(Math.Abs(angleError) < Math.PI/4)
                {
                    setRobot(0, (float)(dist * straightP), (float)(-angleError * turnP));
                }
                else
                {
                    setRobot(0, 0.0f, (float)(-angleError * turnP));
                }

                */
            }
        }
    }
}
