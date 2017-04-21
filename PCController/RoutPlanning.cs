using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCController
{
    class ArmData
    {
        public static double longbase;
        public static double longrate2;
        public static double longrate3;
    }

    class MotorAngle
    {
        public MotorAngle(double motor1angle_in, double motor2angle_in, double motor3angle_in, double motor4angle_in)
        {
            motor1angle = motor1angle_in;
            motor2angle = motor2angle_in;
            motor3angle = motor3angle_in;
            motor4angle = motor4angle_in;
        }

        public double motor1angle, motor2angle, motor3angle, motor4angle;
    }

    class AngleList
    {
        public Angle headAngle;
        public AngleList(decimal one_in, decimal two_in, decimal three_in, decimal four_in)
        {
            headAngle = new Angle(one_in, two_in, three_in, four_in);
        }
    }

    class Angle
    {
        public decimal one, two, three, four;
        public Angle nextangle;
        public Angle(decimal one_in, decimal two_in, decimal three_in, decimal four_in)
        {
            one = one_in;
            two = two_in;
            three = three_in;
            four = four_in;

            nextangle = null;
        }
        public void creatNextNode(decimal one_in, decimal two_in, decimal three_in, decimal four_in)
        {
            nextangle = new Angle(one_in, two_in, three_in, four_in);
        }
    }


    class RoutPlanning
    {

        private static MotorAngle calcu(MotorAngle origangle, double distance, int pointsnum)
        {

            double pi = 3.1415926;
            int count = 0;

            double arm1long = ArmData.longbase;
            double arm2long = arm1long * (ArmData.longrate2);
            double arm3long = arm1long * (ArmData.longrate3);

            double angle1 = (origangle.motor1angle) * pi / 180;
            double angle2 = (origangle.motor2angle) * pi / 180;
            double angle3 = (origangle.motor3angle) * pi / 180;
            double angle4 = (origangle.motor4angle) * pi / 180;

            Program.form.mesPrintln(String.Format("initial angle of 1,3,4:{0:f},{1:f},{2:f}", origangle.motor1angle, origangle.motor3angle, origangle.motor4angle));

            double tmpangle1, tmpangle7, tmpangle8, tmpangle4, tmpangle2, tmpangle9, tmpangle6;
            double tmpline1;//origangle2,arm1,arm2

            double movelong = distance / pointsnum;

            double afterline1 = 0, afterangle1 = 0, afterangle3 = 0, afterangle4 = 0;

            StreamWriter angleFileWriter = new StreamWriter("moveangle.txt");
            /*
                //check if the distance is able to move
                tmpline1=pow(arm1long,2)+pow(arm2long,2)-2*arm1long*arm2long*cos(angle3);
                tmpline1=Math.Sqrt(tmpline1);
                if(distance>(arm1long+arm2long)-tmpline1){
                printf("the distance is too long,the longest distance is %f\n",tmpline1);
                }
                //
            */
            for (count = 0; count < pointsnum; count++)
            {

                tmpline1 = Math.Pow(arm1long, 2) + Math.Pow(arm2long, 2) - 2 * arm1long * arm2long * Math.Cos(angle3);
                tmpline1 = Math.Sqrt(tmpline1);

                tmpangle8 = (Math.Pow(tmpline1, 2) + Math.Pow(arm1long, 2) - Math.Pow(arm2long, 2)) / (2 * arm1long * tmpline1);
                tmpangle8 = Math.Acos(tmpangle8);

                tmpangle7 = (Math.Pow(tmpline1, 2) + Math.Pow(arm2long, 2) - Math.Pow(arm1long, 2)) / (2 * arm2long * tmpline1);
                tmpangle7 = Math.Acos(tmpangle7);

                tmpangle4 = angle4 + tmpangle7;

                afterline1 = Math.Pow(tmpline1, 2) + Math.Pow(movelong, 2) - 2 * tmpline1 * movelong * Math.Cos(tmpangle4);
                afterline1 = Math.Sqrt(afterline1);

                afterangle3 = (Math.Pow(arm2long, 2) + Math.Pow(arm1long, 2) - Math.Pow(afterline1, 2)) / (2 * arm1long * arm2long);
                afterangle3 = Math.Acos(afterangle3);

                tmpangle9 = (Math.Pow(afterline1, 2) + Math.Pow(arm1long, 2) - Math.Pow(arm2long, 2)) / (2 * arm1long * afterline1);
                tmpangle9 = Math.Acos(tmpangle9);

                tmpangle6 = (Math.Pow(tmpline1, 2) + Math.Pow(afterline1, 2) - Math.Pow(movelong, 2)) / (2 * tmpline1 * afterline1);
                tmpangle6 = Math.Acos(tmpangle6);

                afterangle1 = angle1 - (tmpangle8 - tmpangle9 - tmpangle6);//

                afterangle4 = pi - (pi - tmpangle4 - tmpangle6) - (pi - afterangle3 - tmpangle9);

                //Program.form.mesPrintln(String.Format("angle1:{0:f} ,angle3:{1:f} ,angle4:{2:f}", afterangle1 * 180 / pi, afterangle3 * 180 / pi, afterangle4 * 180 / pi));

                angleFileWriter.WriteLine("{0:f12},{1:f12}", afterangle1 * 180 / pi, afterangle3 * 180 / pi);

                angle1 = afterangle1;
                angle3 = afterangle3;
                angle4 = afterangle4;
            }

            angleFileWriter.Close();

            MotorAngle afterangle = new MotorAngle(afterangle1, angle2, afterangle3, afterangle4);


            return afterangle;
        }

        public static void Initialize(int[,] coordinate)
        {

            /*
            the first layer(highter one) are 0~3(left to right);second layer(lower one) are 4~7(left to right)
            */
            int i;
            for (i = 0; i < 5; i++)
            {
                coordinate[i, 0] = 760;//ridio
                coordinate[i, 1] = (i + 1) * 30;//angle
                coordinate[i, 2] = 0;//z
                coordinate[i + 5, 0] = 760;//ridio
                coordinate[i + 5, 1] = (i + 1) * 30;//angle
                coordinate[i + 5, 2] = 0;//z
                                         //printf("coordinate %d:%d,%d,%d",i,coordinate[i][0],coordinate[i][1],coordinate[i][2]);
            }
        }

        public static AngleList routplanning(double armbaselong, double arm2rate, double arm3rate, double angle1, double angle2, double angle3, double angle4, double distance, int pointsnum)
        {

            MotorAngle origmotor = new MotorAngle(angle1, angle2, angle3, angle4);

            ArmData.longbase = armbaselong;
            ArmData.longrate2 = arm2rate;
            ArmData.longrate3 = arm3rate;

            calcu(origmotor, distance, pointsnum);

            decimal[,] data = new decimal[100, 2];


            StreamReader angleFileReader = new StreamReader("moveangle.txt");


            for (int linenum = 0; !angleFileReader.EndOfStream; linenum++)
            {
                string[] strSplit = angleFileReader.ReadLine().Split(',');

                data[linenum, 0] = Convert.ToDecimal(strSplit[0]);
                data[linenum, 1] = Convert.ToDecimal(strSplit[1]);
            }

            angleFileReader.Close();


            StreamWriter angleFileWriter = new StreamWriter("moveangle.txt");

            AngleList list = new AngleList(90 - (decimal)angle1, (decimal)angle2, 180 - (decimal)angle3, 180 - (decimal)angle4);
            Angle currnode = list.headAngle;

            decimal tmpangle = (decimal)angle4;


            for (int i = 0; i < pointsnum; i++)
            {
                if (i == 0)
                {
                    tmpangle += ((data[i, 0] - (decimal)angle1) + (data[i, 1] - (decimal)angle3));
                }
                else
                {
                    tmpangle += ((data[i, 0] - data[i - 1, 0]) + (data[i, 1] - data[i - 1, 1]));
                }

                angleFileWriter.WriteLine("{0:f12},{1:f12},{2:f12}", 90 - data[i, 0], 180 - data[i, 1], 180 - tmpangle);

                currnode.creatNextNode(90 - data[i, 0], (decimal)angle2, 180 - data[i, 1], 180 - tmpangle);
                currnode = currnode.nextangle;

            }


            angleFileWriter.Close();

            return list;

        }

    }

}
