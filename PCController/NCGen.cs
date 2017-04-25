using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCController
{
    class NCGen
    {
        private static StreamWriter ncfile;

        public static void generator(AngleList[] go, int[,] scheduling)
        {
            const int complete = 11, access = 10, touch = 12, grab = 13, communication = 322;//O pin
            decimal[,] savedata = new decimal[100, 4];
            int i, number, count;
            decimal afterz;

            int tesknum = 0;

            ncfile = new StreamWriter("mainjob.txt");

            ncfile.WriteLine("%%@MACRO");
            ncfile.WriteLine("#1510 := @115221;");
            ncfile.WriteLine("MOVJ C1=0.0 C2=0.0 C3=0.0 C4=0.0 FJ20;");


            for (i = 0; scheduling[i, 0] != 0; i++)
            {
                //catch
                number = scheduling[i, 0];
                count = 0;

                waitaccess(access, communication);

                Angle tmp = go[number - 1].headAngle;

                while (tmp != null)
                {
                    ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3={2:f3} C4={3:f3} FJ10 PL8;", tmp.one, tmp.three,tmp.two ,tmp.four);

                    savedata[count, 0] = tmp.one;
                    savedata[count, 1] = tmp.two;
                    savedata[count, 2] = tmp.three;
                    savedata[count, 3] = tmp.four;

                    tmp = tmp.nextangle;
                    count++;
                }

                count--;

                afterz = suck(touch);

                while (count != 0)
                {
                    ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3={2:f3} C4={3:f3} FJ10 PL8;", savedata[count, 0], savedata[count, 2], afterz, savedata[count, 3]);
                    count--;
                }
                ncfile.WriteLine("WAIT();");
                //sendcomplete(complete, communication);


                //put
                number = scheduling[i, 1];
                count = 0;

                waitaccess(access, communication);

                tmp = go[number - 1].headAngle;

                while (tmp != null)
                {
                    ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3={2:f3} C4={3:f3} FJ10 PL8;", tmp.one, tmp.three, tmp.two, tmp.four);

                    savedata[count, 0] = tmp.one;
                    savedata[count, 1] = tmp.two;
                    savedata[count, 2] = tmp.three;
                    savedata[count, 3] = tmp.four;

                    tmp = tmp.nextangle;
                    count++;
                }

                count--;
                //afterz = put(touch);
                while (count != 0)
                {
                    ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3={2:f3} C4={3:f3} FJ10 PL8;", savedata[count, 0], savedata[count, 2], afterz, savedata[count, 3]);
                    count--;
                }
                ncfile.WriteLine("WAIT();");
                /*

                sendcomplete(complete, communication);
                */
            }

            ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3={2:f3} C4={3:f3} FJ10 PL8;",0,0, 0, 0);
            ncfile.WriteLine("M30;");
            ncfile.Close();


        }

        public static string getMovCode()
        {
            string output = "";
            int gvarNO = 100;

            for (int i = 0; i < RoutPlanning.pointCount; i++)
            {
                output += string.Format("MOVJ C1=@{0:d} C2=@{1:d} C3=@{2:d} C4=@{3:d} FJ10 PL5;\r\n", gvarNO++, gvarNO++, gvarNO++, gvarNO++);
            }

            return output;
        }

        private static int waitaccess(int access, int communication)
        {
            ncfile.WriteLine("SETDO({0:d},1);", communication);
            ncfile.WriteLine("WHILE (@{0:d}=0) DO", access);
            ncfile.WriteLine("SLEEP();");
            ncfile.WriteLine("END_WHILE");
            ncfile.WriteLine("SETDO({0:d},0);", access);

            return 1;

        }
        private static decimal suck(int touch)
        {
            /*
            ncfile.WriteLine("WHILE (READDO({0:f3})=0) DO", touch);
            ncfile.WriteLine("SLEEP();");
            ncfile.WriteLine("END_WHILE;");
            ncfile.WriteLine("SETDO({0:d},0);", touch);
            */
            return 0;

        }

        private static decimal put(int touch)
        {
            /*
            ncfile.WriteLine("WHILE (READDO({0:d})=0) DO", touch);
            ncfile.WriteLine("SLEEP();");
            ncfile.WriteLine("END_WHILE;");
            ncfile.WriteLine("SETDO({0:d},0);", touch);
            */
            return 0;

        }

        private static int sendcomplete(int complete, int communication)
        {
            ncfile.WriteLine("SETDO({0:d},1);", communication);
            ncfile.WriteLine("WHILE (READDO({0:d})=0) DO", complete);
            ncfile.WriteLine("SLEEP();");
            ncfile.WriteLine("END_WHILE;");
            ncfile.WriteLine("SETDO({0:d},0);", complete);

            return 0;

        }

    }
}
