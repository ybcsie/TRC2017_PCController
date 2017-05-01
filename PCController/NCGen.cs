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
            const int complete = 11, access = 10, suck=320, communication = 6;//O pin
            decimal[,] savedata = new decimal[100, 4];
            int i, number, count;
            double afterz=0;
            int[] cassetteA = new int[6]; 
            int[] cassetteB = new int[6];//紀錄六片wafer在cassette中的位置
            int outnum = 0;//已從cassetteA取出之wafer數
            int putnum = 0;//已放入cassetteB之wafer數
            double casetspacing = 6.4;//mm

            startGenNC(SyntecClient.NCFileName.MAIN_JOB);

            ncfile.WriteLine("MOVJ C1=0.0 C2=0.0 C3=0.0 C4=0.0 FJ20;");

            //測試用，隨意指定wafer所在cassette位置
            for (i = 0; i < 6; i++)
            {
                cassetteA[i] = 4 * i + 4;
                cassetteB[5 - i] = 4 * i + 4;
            }
            //

            for (i = 0; scheduling[i, 0] != 0; i++)
            {
                //catch
                number = scheduling[i, 0];
                count = 0;

                waitaccess(access, communication);//發送執行許可，並等待許可

                if (number == 4)//計算要從cassetteA拿出第幾片Wafer，用以抓取wafer確認位置
                {
                    outnum++;
                }

                Angle tmp = go[number - 1].headAngle;

                while (tmp != null)
                {
                    ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3={2:f3} C4={3:f3} FJ10 PL5;", tmp.one, tmp.three, tmp.two, tmp.four);

                    savedata[count, 0] = tmp.one;
                    savedata[count, 1] = tmp.two;
                    savedata[count, 2] = tmp.three;
                    savedata[count, 3] = tmp.four;

                    tmp = tmp.nextangle;
                    count++;
                }

                count--;
                Obitcontrol(suck, 1);
                afterz = movezaxis(cassetteA[outnum], (double)savedata[count, 1]);
                ncfile.WriteLine("WAIT();");

                while (count != 0)
                {
                    ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3={2:f3} C4={3:f3} FJ10 PL5;", savedata[count, 0], savedata[count, 2], afterz, savedata[count, 3]);
                    count--;
                }
                ncfile.WriteLine("WAIT();");

                //sendcomplete(complete, communication);//發送完成任務訊號，並等待確認



                number = scheduling[i, 1];
                count = 0;

                waitaccess(access, communication);

                tmp = go[number - 1].headAngle;

                if (number == 5)
                {
                    putnum++;
                }

                while (tmp != null)
                {
                    ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3={2:f3} C4={3:f3} FJ10 PL5;", tmp.one, tmp.three, tmp.two, tmp.four);

                    savedata[count, 0] = tmp.one;
                    savedata[count, 1] = tmp.two;
                    savedata[count, 2] = tmp.three;
                    savedata[count, 3] = tmp.four;

                    tmp = tmp.nextangle;
                    count++;
                }

                count--;
                Obitcontrol(suck, 0);
                afterz = movezaxis(cassetteB[putnum],(double)savedata[count,1]);
                ncfile.WriteLine("WAIT();");

                while (count != 0)
                {
                    ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3={2:f3} C4={3:f3} FJ10 PL5;", savedata[count, 0], savedata[count, 2], afterz, savedata[count, 3]);
                    count--;
                }
                ncfile.WriteLine("WAIT();");

                //sendcomplete(complete, communication);

            }

            endGenNC();

        }


        private static void startGenNC(string filename)
        {
            ncfile = new StreamWriter(filename);

            ncfile.WriteLine("%%@MACRO");
            ncfile.WriteLine("#1510 := @115221;");

        }
        private static void endGenNC()
        {
            ncfile.WriteLine("M30;");
            ncfile.Close();
        }

        public static void genInitNC()
        {
            startGenNC("initializer.txt");

            ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C4={2:f3} FJ10;", 30, 60, -70);

            ncfile.WriteLine("WAIT();");

            ncfile.WriteLine("@10 := 1;");
            ncfile.WriteLine("WHILE (@10 = 1) DO");

            ncfile.WriteLine("IF (@11 = 1) THEN");
            //ncfile.WriteLine("WAIT();");
            ncfile.WriteLine(getMovCode());
            ncfile.WriteLine("WAIT();");
            ncfile.WriteLine("@11 := 0;");
            ncfile.WriteLine("END_IF;"); 

            ncfile.WriteLine("SLEEP();");
            ncfile.WriteLine("END_WHILE;");

            endGenNC();
        }

        public static void genObitSetterNC(int addr, bool val)
        {
            startGenNC(SyntecClient.NCFileName.OBIT_SETTER);
            ncfile.WriteLine("SETDO({0:d},{1:d});", addr, val ? 1 : 0);
            endGenNC();
        }

        private static string getMovCode()
        {
            string output = "";
            int gvarNO = 100;

            for (int i = 0; i < 10; i++)
            {
                output += string.Format("MOVJ C1=@{0:d} C2=@{1:d} C3=@{2:d} C4=@{3:d} FJ10 PL5;\r\n", gvarNO++, gvarNO++, gvarNO++, gvarNO++);
            }

            return output;
        }

        private static void Obitcontrol(int pin, int command)//command: 0 for close,1 for open
        {
            ncfile.WriteLine("SETDO({0:d},{1:d});",pin,command);
        }

        private static double movezaxis(double expvalue,double nowvalue)
        {
            /*
            if (expvalue < nowvalue)
            {
                while (expvalue <= nowvalue)
                {
                    ncfile.WriteLine("MOVJ C3={0:f3};", nowvalue-0.05);
                    nowvalue = nowvalue - 0.05;
                }
            }
            else
            {
                while (expvalue > nowvalue)
                {
                    ncfile.WriteLine("MOVJ C3={0:f3};", nowvalue + 0.05);
                    nowvalue = nowvalue + 0.05;
                }
            }
            return nowvalue;
            */
            return 0;
        }

        private static void waitaccess(int access, int communication)
        {
            ncfile.WriteLine("@{0:d}=1;", communication);
            ncfile.WriteLine("WHILE (@{0:d}=0) DO", access);
            ncfile.WriteLine("SLEEP();");
            ncfile.WriteLine("END_WHILE");
            ncfile.WriteLine("@{0:d}=0;", access);

        }

        private static int sendcomplete(int complete, int communication)
        {
            ncfile.WriteLine("@{0:d}=1;", communication);
            ncfile.WriteLine("WHILE (@{0:d}=0) DO", complete);
            ncfile.WriteLine("SLEEP();");
            ncfile.WriteLine("END_WHILE;");
            ncfile.WriteLine("@{0:d}=0;", complete);

            return 0;

        }

    }
}
