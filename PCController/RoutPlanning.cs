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

        public static double ratio;
        public static double distance;

        public static double[,] coordinate;

        public static double[,] measureangle = null;

        public static double[] Z_chamberA =
            {
            107.2,//0
            111.8,//1
            115.8,//2
            119.8,//3
            123.8,//4
            127.8,//5
            131.8,//6
            135.8,//7
            139.8,//8
            143.8,//9
            147.8,//10
            151.8,//11
            155.8,//12
            159.8,//13
            163.8,//14
            167.8,//15
            171.8,//16
            175.8,//17
            179.8,//18
            183.8,//19
            187.8,//20
            191.8,//21
            195.8,//22
            199.8,//23
            203.8//24
        };

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
        public static int pointCount;

        private static bool location=false;

        private static MotorAngle calcu(MotorAngle origangle, double distance, int pointsnum)
        {
            pointCount = pointsnum;

            double pi = 3.1415926;
            int count = 0;

            double arm1long = ArmData.longbase;
            double arm2long = arm1long * (ArmData.longrate2);
            double arm3long = arm1long * (ArmData.longrate3);
            double angle1;
            double angle2;
            double angle3;
            double angle4;
            //Program.form.showWarnning(string.Format("arm1long = {0:f3}, arm2long = {1:f3},arm3long = {2:f3}", arm1long, arm2long, arm3long));

            if (arm1long == 0 || arm2long == 0 || arm3long == 0)
            {
                Program.form.showWarnning(string.Format("arm1long = {0:f3}, arm2long = {1:f3},arm3long = {2:f3}", arm1long, arm2long, arm3long));
                return null;
            }

            if (origangle.motor3angle > 0)
            {
                angle1 = (180 - (origangle.motor1angle)) * pi / 180;
                angle2 = (origangle.motor2angle) * pi / 180;
                angle3 = (180 - (origangle.motor3angle)) * pi / 180;
                angle4 = (180 + (origangle.motor4angle)) * pi / 180;
            }
            else
            {
                angle1 = (origangle.motor1angle) * pi / 180;
                angle2 = (origangle.motor2angle) * pi / 180;
                angle3 = (180 + (origangle.motor3angle)) * pi / 180;
                angle4 = (180 - (origangle.motor4angle)) * pi / 180;
            }

            Program.form.mesPrintln(String.Format("initial angle of 1,2,3,4:{0:f},{1:f},{2:f},{3:f}", origangle.motor1angle, origangle.motor2angle, origangle.motor3angle, origangle.motor4angle));

            double tmpangle1, tmpangle7, tmpangle8, tmpangle4, tmpangle2, tmpangle9, tmpangle6;
            double tmpline1;//origangle2,arm1,arm2

            double movelong =0;
            double longestmove = 0;

            double afterline1 = 0, afterangle1 = 0, afterangle3 = 0, afterangle4 = 0;


            //判斷是否超過極限，若超過極限，則伸長至極限
            tmpline1 = Math.Pow(arm1long, 2) + Math.Pow(arm2long, 2) - 2 * arm1long * arm2long * Math.Cos(angle3);
            tmpline1 = Math.Sqrt(tmpline1);
            tmpangle7 = (Math.Pow(tmpline1, 2) + Math.Pow(arm2long, 2) - Math.Pow(arm1long, 2)) / (2 * arm2long * tmpline1);
            tmpangle7 = Math.Acos(tmpangle7);
            if (pi - tmpangle7 >= angle4)
            {
                tmpangle6 = (tmpline1 * Math.Sin(tmpangle7 + angle4)) / (arm1long + arm2long);
                tmpangle6 = Math.Asin(tmpangle6);
                tmpangle6 = pi - tmpangle7 - tmpangle6 - angle4;
                if (tmpangle6 <0.000001)
                {
                    longestmove = arm1long + arm2long - tmpline1;
                }
                else
                {
                    longestmove = (arm1long + arm2long) * Math.Sin(tmpangle6) / Math.Sin(tmpangle7 + angle4);
                }
                Program.form.mesPrintln(String.Format("tmpline:{0:f},tmpangle7:{1:f},tmpangle6:{2:f},longestmove:{3:f}",tmpline1 ,tmpangle7,tmpangle6,longestmove));
            }
            else
            {
                tmpangle6 = (tmpline1 * Math.Sin(2*pi - tmpangle7 - angle4)) / (arm1long + arm2long);
                tmpangle6 = Math.Asin(tmpangle6);
                tmpangle6 = pi - tmpangle6 - (2 * pi - tmpangle7 - angle4);
                if (tmpangle6 < 0.000001)
                {
                    longestmove = arm1long + arm2long - tmpline1;
                }
                else
                {
                    longestmove = (arm1long + arm2long) * Math.Sin(tmpangle6) / Math.Sin(2 * pi - tmpangle7 - angle4);
                }
                Program.form.mesPrintln(String.Format("tmpline:{0:f},tmpangle7:{1:f},tmpangle6:{2:f},longestmove:{3:f}", tmpline1, tmpangle7, tmpangle6, longestmove));
            }

            if (distance > longestmove)
            {
                Program.form.mesPrintln(string.Format("移動距離超出極限，自動重設距離為極限點 arm1={0:f3},arm2={1:f3},arm3={2:f3} longest = {3:f3}", arm1long, arm2long, arm3long, longestmove));
                distance = longestmove;
            }


            movelong = distance / pointsnum;

            tmpangle6 = 0;
            tmpangle7 = 0;
            tmpline1 = 0;

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

                if (tmpline1 == 0)
                {
                    Program.form.showWarnning(string.Format("tmpline1 = {0:f3}", tmpline1));
                    angleFileWriter.Close();
                    return null;
                }


                tmpangle8 = (Math.Pow(tmpline1, 2) + Math.Pow(arm1long, 2) - Math.Pow(arm2long, 2)) / (2 * arm1long * tmpline1);
                tmpangle8 = Math.Acos(tmpangle8);

                tmpangle7 = (Math.Pow(tmpline1, 2) + Math.Pow(arm2long, 2) - Math.Pow(arm1long, 2)) / (2 * arm2long * tmpline1);
                tmpangle7 = Math.Acos(tmpangle7);
                if (pi-tmpangle7>=angle4)
                {
                    tmpangle4 = angle4 + tmpangle7;
                }
                else
                {
                    tmpangle4 = 2*pi - angle4 - tmpangle7;
                }

                afterline1 = Math.Pow(tmpline1, 2) + Math.Pow(movelong, 2) - 2 * tmpline1 * movelong * Math.Cos(tmpangle4);
                afterline1 = Math.Sqrt(afterline1);

                afterangle3 = (Math.Pow(arm2long, 2) + Math.Pow(arm1long, 2) - Math.Pow(afterline1, 2)) / (2 * arm1long * arm2long);
                afterangle3 = Math.Acos(afterangle3);


                if (afterline1 == 0)
                {
                    Program.form.showWarnning(string.Format("afterline1 = {0:f3}", afterline1));
                    angleFileWriter.Close();
                    return null;
                }

                tmpangle9 = (Math.Pow(afterline1, 2) + Math.Pow(arm1long, 2) - Math.Pow(arm2long, 2)) / (2 * arm1long * afterline1);
                tmpangle9 = Math.Acos(tmpangle9);

                tmpangle6 = (Math.Pow(tmpline1, 2) + Math.Pow(afterline1, 2) - Math.Pow(movelong, 2)) / (2 * tmpline1 * afterline1);
                if(tmpangle6>1 && tmpangle6 < 1.0001)
                {
                    tmpangle6 = 1;
                }


                tmpangle6 = Math.Acos(tmpangle6);

                if (pi - tmpangle7 >= angle4)
                {
                    afterangle1 = angle1 - (tmpangle8 - tmpangle9 - tmpangle6);//
                    afterangle4 = pi - (pi - tmpangle4 - tmpangle6) - (pi - afterangle3 - tmpangle9);
                }
                else
                {
                    afterangle1 = angle1 - (tmpangle8 - tmpangle9 + tmpangle6);//
                    afterangle4 = pi + (pi - tmpangle4 - tmpangle6) - (pi - afterangle3 - tmpangle9);
                }
                //Program.form.mesPrintln(String.Format("angle1:{0:f} ,angle3:{1:f} ,angle4:{2:f}", afterangle1 * 180 / pi, afterangle3 * 180 / pi, afterangle4 * 180 / pi));


                if (afterangle1 == double.NaN || afterangle3 == double.NaN)
                {
                    Program.form.showWarnning(string.Format("afterangle1 = {0:f3},afterangle3 = {1:f3}", afterangle1, afterangle3));
                    angleFileWriter.Close();
                    return null;
                }

                if (origangle.motor3angle > 0)
                {
                    angleFileWriter.WriteLine("{0:f12},{1:f12}", 180 - (afterangle1 * 180 / pi), 180 - (afterangle3 * 180 / pi));
                }
                else
                {
                    angleFileWriter.WriteLine("{0:f12},{1:f12}", (afterangle1 * 180 / pi), (afterangle3 * 180 / pi)-180);
                }
                angle1 = afterangle1;
                angle3 = afterangle3;
                angle4 = afterangle4;
            }

            angleFileWriter.Close();

            MotorAngle afterangle = new MotorAngle(afterangle1, angle2, afterangle3, afterangle4);


            return afterangle;
        }


        private static MotorAngle back(MotorAngle origangle, double distance, int pointsnum)
        {
            pointCount = pointsnum;

            double pi = 3.1415926;
            int count = 0;

            double arm1long = ArmData.longbase;
            double arm2long = arm1long * (ArmData.longrate2);
            double arm3long = arm1long * (ArmData.longrate3);
            double angle1;
            double angle2;
            double angle3;
            double angle4;
            //Program.form.showWarnning(string.Format("arm1long = {0:f3}, arm2long = {1:f3},arm3long = {2:f3}", arm1long, arm2long, arm3long));

            if (arm1long == 0 || arm2long == 0 || arm3long == 0)
            {
                Program.form.showWarnning(string.Format("arm1long = {0:f3}, arm2long = {1:f3},arm3long = {2:f3}", arm1long, arm2long, arm3long));
                return null;
            }

            if (origangle.motor3angle > 0)
            {
                angle1 = (180 - (origangle.motor1angle)) * pi / 180;
                angle2 = (origangle.motor2angle) * pi / 180;
                angle3 = (180 - (origangle.motor3angle)) * pi / 180;
                angle4 = (180 + (origangle.motor4angle)) * pi / 180;
            }
            else
            {
                angle1 = (origangle.motor1angle) * pi / 180;
                angle2 = (origangle.motor2angle) * pi / 180;
                angle3 = (180 + (origangle.motor3angle)) * pi / 180;
                angle4 = (180 - (origangle.motor4angle)) * pi / 180;
            }

            Program.form.mesPrintln(String.Format("initial angle of 1,2,3,4:{0:f},{1:f},{2:f},{3:f}", origangle.motor1angle, origangle.motor2angle, origangle.motor3angle, origangle.motor4angle));

            double tmpangle1, tmpangle7, tmpangle8, tmpangle4, tmpangle2, tmpangle9, tmpangle6;
            double tmpline1;//origangle2,arm1,arm2

            double movelong = distance / pointsnum *(-1);

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

                if (tmpline1 == 0)
                {
                    Program.form.showWarnning(string.Format("tmpline1 = {0:f3}", tmpline1));
                    angleFileWriter.Close();
                    return null;
                }


                tmpangle8 = (Math.Pow(tmpline1, 2) + Math.Pow(arm1long, 2) - Math.Pow(arm2long, 2)) / (2 * arm1long * tmpline1);
                tmpangle8 = Math.Acos(tmpangle8);

                tmpangle7 = (Math.Pow(tmpline1, 2) + Math.Pow(arm2long, 2) - Math.Pow(arm1long, 2)) / (2 * arm2long * tmpline1);
                tmpangle7 = Math.Acos(tmpangle7);

                if (pi - tmpangle7 >= angle4)
                {
                    tmpangle4 = pi - angle4 - tmpangle7;
                }
                else
                {
                    tmpangle4 = angle4 + tmpangle7 -pi;
                }

                afterline1 = Math.Pow(tmpline1, 2) + Math.Pow(movelong, 2) - 2 * tmpline1 * movelong * Math.Cos(tmpangle4);
                afterline1 = Math.Sqrt(afterline1);

                afterangle3 = (Math.Pow(arm2long, 2) + Math.Pow(arm1long, 2) - Math.Pow(afterline1, 2)) / (2 * arm1long * arm2long);
                afterangle3 = Math.Acos(afterangle3);


                if (afterline1 == 0)
                {
                    Program.form.showWarnning(string.Format("afterline1 = {0:f3}", afterline1));
                    angleFileWriter.Close();
                    return null;
                }

                tmpangle9 = (Math.Pow(afterline1, 2) + Math.Pow(arm1long, 2) - Math.Pow(arm2long, 2)) / (2 * arm1long * afterline1);
                tmpangle9 = Math.Acos(tmpangle9);

                tmpangle6 = (Math.Pow(tmpline1, 2) + Math.Pow(afterline1, 2) - Math.Pow(movelong, 2)) / (2 * tmpline1 * afterline1);
                if (tmpangle6 > 1 && tmpangle6 < 1.0001)
                {
                    tmpangle6 = 1;
                }


                tmpangle6 = Math.Acos(tmpangle6);

                if (pi - tmpangle7 >= angle4)
                {
                    afterangle1 = angle1 - (tmpangle8 + tmpangle6) + tmpangle9;//
                    afterangle4 = pi - (pi - angle4 - tmpangle7) - (pi - afterangle3 - tmpangle9) - tmpangle6;
                }
                else
                {
                    afterangle1 = angle1 - tmpangle8 + tmpangle9 + tmpangle6;//
                    afterangle4 = 2 * pi - (pi - afterangle3 - tmpangle9) - (pi - tmpangle6 - tmpangle4);
                }
                //Program.form.mesPrintln(String.Format("angle1:{0:f} ,angle3:{1:f} ,angle4:{2:f}", afterangle1 * 180 / pi, afterangle3 * 180 / pi, afterangle4 * 180 / pi));


                if (afterangle1 == double.NaN || afterangle3 == double.NaN)
                {
                    Program.form.showWarnning(string.Format("afterangle1 = {0:f3},afterangle3 = {1:f3}", afterangle1, afterangle3));
                    angleFileWriter.Close();
                    return null;
                }

                if (origangle.motor3angle > 0)
                {
                    angleFileWriter.WriteLine("{0:f12},{1:f12}", 180 - (afterangle1 * 180 / pi), 180 - (afterangle3 * 180 / pi));
                }
                else
                {
                    angleFileWriter.WriteLine("{0:f12},{1:f12}", (afterangle1 * 180 / pi), (afterangle3 * 180 / pi) - 180);
                }
                angle1 = afterangle1;
                angle3 = afterangle3;
                angle4 = afterangle4;
            }

            angleFileWriter.Close();

            MotorAngle afterangle = new MotorAngle(afterangle1, angle2, afterangle3, afterangle4);

            return afterangle;
        }


        public static void Initialize()
        {
            ArmData.coordinate = new double[10, 4];

            /*
            the first layer(highter one) are 0~3(left to right);second layer(lower one) are 4~7(left to right)
            */
            //int[,] coordinate = new int[10, 4];
            int i;
            int count = 0;
            //double[,] measureangle = new double[10, 4];
            double[,] realcd = new double[10, 3];
            double[] centerx = new double[4];
            double[] centery = new double[4];
            double avcx, avcy;
            double[,] reference = new double[10, 2];
            double armlong1 = ArmData.longbase;
            double armlong2 = armlong1 * ArmData.longrate2;
            double armlong3 = armlong1 * ArmData.longrate3;
            const double pi = 3.1415926;
            Program.form.mesPrintln(String.Format("長度 1:{0:f} 2: {1:f} 3:{2:f}", ArmData.distance, ArmData.ratio, armlong3));
            //Program.form.mesPrintln(String.Format(".... {0:f}  {1:f}  {2:f}", armlong1, armlong2, armlong3));


            //calculate the real(x,y)coordinate(realcd) of each platform
            if (ArmData.measureangle != null)
            {
                for (i = 0; i < 10; i++)
                {

                    //Program.form.mesPrintln(String.Format("measureangle{0:f}  0:{1:f} 1:{2:f} 2:{3:f}", i, ArmData.measureangle[i, 0], ArmData.measureangle[i, 1], ArmData.measureangle[i, 2]));
                    if (ArmData.measureangle[i, 0] != 0)
                    {
                        Program.form.mesPrintln(String.Format("measureangle{0:f}  0:{1:f} 2:{2:f} 3:{3:f} ", i, ArmData.measureangle[i, 0], ArmData.measureangle[i, 2], ArmData.measureangle[i, 3]));
                        realcd[i, 0] = (armlong1 * Math.Cos(ArmData.measureangle[i, 0]*pi/180)) + (armlong2 * Math.Cos((ArmData.measureangle[i, 0] + ArmData.measureangle[i, 2]) * pi / 180)) + ((armlong3-20) * Math.Cos((ArmData.measureangle[i, 0] + ArmData.measureangle[i, 2] + ArmData.measureangle[i, 3]) * pi / 180));
                        Program.form.mesPrintln(String.Format("0: {0:f} 2: {1:f}  3: {2:f} ", (armlong1 * Math.Cos(ArmData.measureangle[i, 0]*pi / 180)), (armlong2 * Math.Cos((ArmData.measureangle[i, 0] + ArmData.measureangle[i, 2])*pi/180)), ((armlong3-20) * Math.Cos((ArmData.measureangle[i, 0] + ArmData.measureangle[i, 2] + ArmData.measureangle[i, 3])*pi / 180))));
                        realcd[i, 1] = (armlong1 * Math.Sin(ArmData.measureangle[i, 0] * pi / 180)) + (armlong2 * Math.Sin((ArmData.measureangle[i, 0] + ArmData.measureangle[i, 2]) * pi / 180)) + ((armlong3-20) * Math.Sin((ArmData.measureangle[i, 0] + ArmData.measureangle[i, 2] + ArmData.measureangle[i, 3]) * pi / 180));
                        Program.form.mesPrintln(String.Format("realcd{0:f}  x:{1:f} y:{2:f} ", i, realcd[i, 0], realcd[i, 1]));
                    }
                    else
                    {
                        realcd[i, 0] = -1;
                        realcd[i, 1] = -1;
                    }

                }
            }
            else
            {
                Program.form.mesPrintln("use defalt angle");
                for (i = 0; i < 10; i++)
                {
                    realcd[i, 0] = -1;
                    realcd[i, 1] = -1;
                }
                
                realcd[1, 0] = -458.4725;
                realcd[1, 1] = 631.033255;

                realcd[2, 0] = 0;
                realcd[2, 1] = 780;
                realcd[3, 0] = 458.4725;
                realcd[3, 1] = 631.033255;

                //need to deleted
            }
            for (i=0;i<4;i++)
            {
                centerx[i] = 0;
            }

            //calculate the center of cycle
            double cx, cy, dx, dy, l, h, tdx, tdy;

            if (ArmData.measureangle == null)
            {

                for (i = 0; i < 4; i++)
                {
                    if (realcd[i, 0] != -1 && realcd[i + 1, 0] != -1)
                    {
                        cx = (realcd[i, 0] + realcd[i + 1, 0]) / 2;
                        cy = (realcd[i, 1] + realcd[i + 1, 1]) / 2;
                        if (realcd[i + 1, 0] - realcd[i, 0] < 0)
                        {
                            dx = realcd[i, 0] - realcd[i + 1, 0];
                            dy = realcd[i, 1] - realcd[i + 1, 1];
                        }
                        else
                        {
                            dx = realcd[i + 1, 0] - realcd[i, 0];
                            dy = realcd[i + 1, 1] - realcd[i, 1];
                        }
                        l = Math.Sqrt(dx * dx + dy * dy);
                        h = Math.Sqrt(ArmData.ratio * ArmData.ratio - (l / 2) * (l / 2));
                        tdx = (-1) * h * dy / l;
                        tdy = h * dx / l;
                        centerx[i] = cx - tdx;
                        centery[i] = cy - tdy;
                        count++;
                    }
                }
                avcx = (centerx[0] + centerx[1] + centerx[2] + centerx[3]) / count;
                avcy = (centery[0] + centery[1] + centery[2] + centery[3]) / count;
                Program.form.mesPrintln(String.Format("中心位置 x:{0:f} y: {1:f} count:{2:d}", avcx, avcy, count));
                //calculate center

                for (i = 0; i < 10; i++)
                {
                    if (realcd[i, 0] == -1)
                    {
                        if (i < 5)
                        {
                            realcd[i, 0] = avcx + ((realcd[2, 0] - avcx) * Math.Cos(-1 * 36 * (i - 2) * pi / 180)) - ((realcd[2, 1] - avcy) * Math.Sin(-1 * 36 * (i - 2) * pi / 180));
                            realcd[i, 1] = avcy + ((realcd[2, 0] - avcx) * Math.Sin(-1 * 36 * (i - 2) * pi / 180)) + ((realcd[2, 1] - avcy) * Math.Cos(-1 * 36 * (i - 2) * pi / 180));
                            //Program.form.mesPrintln(String.Format("..... i:{0:d} x:{1:f} y: {2:f}", i,realcd[i,0], realcd[i,1]));
                        }
                        else
                        {
                            realcd[i, 0] = realcd[i - 5, 0];
                            realcd[i, 1] = realcd[i - 5, 1];
                        }
                    }
                }
            }
            else
            {
                for (i = 0; i < 4; i++)
                {
                    if (realcd[i+5, 0] != -1 && realcd[i+5 + 1, 0] != -1)
                    {
                        cx = (realcd[i+5, 0] + realcd[i+5 + 1, 0]) / 2;
                        cy = (realcd[i+5, 1] + realcd[i+5 + 1, 1]) / 2;
                        if (realcd[i+5 + 1, 0] - realcd[i+5, 0] < 0)
                        {
                            dx = realcd[i+5, 0] - realcd[i+5 + 1, 0];
                            dy = realcd[i+5, 1] - realcd[i+5 + 1, 1];
                        }
                        else
                        {
                            dx = realcd[i+5 + 1, 0] - realcd[i+5, 0];
                            dy = realcd[i+5 + 1, 1] - realcd[i+5, 1];
                        }
                        l = Math.Sqrt(dx * dx + dy * dy);
                        h = Math.Sqrt(ArmData.ratio * ArmData.ratio - (l / 2) * (l / 2));
                        tdx = (-1) * h * dy / l;
                        tdy = h * dx / l;
                        centerx[i] = cx - tdx;
                        centery[i] = cy - tdy;
                        count++;
                    }
                }
                avcx = (centerx[0] + centerx[1] + centerx[2] + centerx[3]) / count;
                avcy = (centery[0] + centery[1] + centery[2] + centery[3]) / count;
                Program.form.mesPrintln(String.Format("中心位置 x:{0:f} y: {1:f} count:{2:d}", avcx, avcy, count));
                //calculate center

                for (i = 9; i >=0 ; i--)
                {
                    if (realcd[i, 0] == -1)
                    {
                        if (i > 4)
                        {
                            realcd[i, 0] = avcx + ((realcd[7, 0] - avcx) * Math.Cos(-1 * 36 * (i - 7) * pi / 180)) - ((realcd[7, 1] - avcy) * Math.Sin(-1 * 36 * (i - 7) * pi / 180));
                            realcd[i, 1] = avcy + ((realcd[7, 0] - avcx) * Math.Sin(-1 * 36 * (i - 7) * pi / 180)) + ((realcd[7, 1] - avcy) * Math.Cos(-1 * 36 * (i - 7) * pi / 180));
                            //Program.form.mesPrintln(String.Format("..... i:{0:d} x:{1:f} y: {2:f}", i,realcd[i,0], realcd[i,1]));
                        }
                        else
                        {
                            realcd[i, 0] = realcd[i + 5, 0];
                            realcd[i, 1] = realcd[i + 5, 1];
                        }
                    }
                }
            }

            double tmplong1 = 0;//original pointer to forth modor
            double tmplong2 = 0;//avcenter to reference
            double tmpangle1 = 0;//tmplong and y axis;
            double tmpangle2 = 0;//tmplong and (avcenter to reference)
            double judgevector = 0;

            for (i = 0; i < 10; i++)
            {
                reference[i, 0] = avcx + (realcd[i, 0] - avcx) * ((ArmData.ratio - ArmData.distance - armlong3) / ArmData.ratio);
                reference[i, 1] = avcy + (realcd[i, 1] - avcy) * ((ArmData.ratio - ArmData.distance - armlong3) / ArmData.ratio);
                //printf("\n%f %f %f\n",reference[i][0],reference[i][1],(reference[i][0]-avcx)*(reference[i][0]-avcx)+(reference[i][1]-avcy)*(reference[i][1]-avcy));
                Program.form.mesPrintln(string.Format("各平台直線進入點  x:{0:f} y:{1:f}", reference[i, 0], reference[i, 1]));
            }

            for (i = 0; i < 10; i++)
            {
                if (reference[i, 0] <= 0.001)
                {
                    tmplong1 = Math.Sqrt(reference[i, 0] * reference[i, 0] + reference[i, 1] * reference[i, 1]);
                    tmpangle1 = ((-1) * reference[i, 0] * 1) / (tmplong1);
                    tmpangle1 = (Math.Acos(tmpangle1)) * 180 / pi;
                    ArmData.coordinate[i, 0] = (tmplong1 * tmplong1 + armlong1 * armlong1 - armlong2 * armlong2) / (2 * tmplong1 * armlong1);
                    ArmData.coordinate[i, 0] = (Math.Acos(ArmData.coordinate[i, 0])) * 180 / pi;
                    ArmData.coordinate[i, 0] = 180 - (tmpangle1 + ArmData.coordinate[i, 0]);
                    if(i<5)
                    {
                        ArmData.coordinate[i, 1] = 121.5;
                    }
                    else
                    {
                        ArmData.coordinate[i, 1] = 16.5;
                    }
                    ArmData.coordinate[i, 2] = (armlong1 * armlong1 + armlong2 * armlong2 - tmplong1 * tmplong1) / (2 * armlong1 * armlong2);
                    ArmData.coordinate[i, 2] = 180 - ((Math.Acos(ArmData.coordinate[i, 2])) * 180 / pi);
                    ArmData.coordinate[i, 3] = (armlong2 * armlong2 + tmplong1 * tmplong1 - armlong1 * armlong1) / (2 * tmplong1 * armlong2);
                    ArmData.coordinate[i, 3] = (Math.Acos(ArmData.coordinate[i, 3])) * 180 / pi;
                    tmplong2 = Math.Sqrt((reference[i, 0] - avcx) * (reference[i, 0] - avcx) + (reference[i, 1] - avcy) * (reference[i, 1] - avcy));
                    tmpangle2 = (reference[i, 0] * (reference[i, 0] - avcx) + reference[i, 1] * (reference[i, 1] - avcy)) / (tmplong2 * tmplong1);
                    if (tmpangle2 >= 1)
                    {
                        tmpangle2 = 0;
                    }
                    else
                    {
                        tmpangle2 = (Math.Acos(tmpangle2)) * 180 / pi;
                    }

                    judgevector = avcx * reference[i, 1] - avcy * reference[i, 0];
                    if (judgevector<=0)
                    {
                        ArmData.coordinate[i, 3] = (-1) * (tmpangle2 + ArmData.coordinate[i, 3]);
                    }
                    else
                    {
                        ArmData.coordinate[i, 3] = (-1) * (ArmData.coordinate[i, 3] - tmpangle2);
                    }
                    /*
                    coordinate[i + 5, 0] = coordinate[i, 0];
                    coordinate[i + 5, 1] = 20;
                    coordinate[i + 5, 2] = coordinate[i, 2];
                    coordinate[i + 5, 3] = coordinate[i, 3];
                    */
                }
                else
                {
                    tmplong1 = Math.Sqrt(reference[i, 0] * reference[i, 0] + reference[i, 1] * reference[i, 1]);
                    tmpangle1 = ( reference[i, 0] * 1) / (tmplong1);
                    tmpangle1 = (Math.Acos(tmpangle1)) * 180 / pi;
                    ArmData.coordinate[i, 0] = (tmplong1 * tmplong1 + armlong1 * armlong1 - armlong2 * armlong2) / (2 * tmplong1 * armlong1);
                    ArmData.coordinate[i, 0] = (Math.Acos(ArmData.coordinate[i, 0])) * 180 / pi;
                    ArmData.coordinate[i, 0] = (tmpangle1 + ArmData.coordinate[i, 0]);

                    if (i < 5)
                    {
                        ArmData.coordinate[i, 1] = 121.5;
                    }
                    else
                    {
                        ArmData.coordinate[i, 1] = 16.5;
                    }
                    ArmData.coordinate[i, 2] = (armlong1 * armlong1 + armlong2 * armlong2 - tmplong1 * tmplong1) / (2 * armlong1 * armlong2);
                    ArmData.coordinate[i, 2] = ((Math.Acos(ArmData.coordinate[i, 2])) * 180 / pi)-180;
                    ArmData.coordinate[i, 3] = (armlong2 * armlong2 + tmplong1 * tmplong1 - armlong1 * armlong1) / (2 * tmplong1 * armlong2);
                    ArmData.coordinate[i, 3] = (Math.Acos(ArmData.coordinate[i, 3])) * 180 / pi;
                    tmplong2 = Math.Sqrt((reference[i, 0] - avcx) * (reference[i, 0] - avcx) + (reference[i, 1] - avcy) * (reference[i, 1] - avcy));
                    tmpangle2 = (reference[i, 0] * (reference[i, 0] - avcx) + reference[i, 1] * (reference[i, 1] - avcy)) / (tmplong2 * tmplong1);
                    if (tmpangle2>=1 )
                    {
                        tmpangle2 = 0;
                    }
                    else
                    {
                        tmpangle2 = (Math.Acos(tmpangle2)) * 180 / pi;
                    }
                    judgevector = avcx * reference[i, 1] - avcy * reference[i, 0];
                    if (judgevector >0)
                    {
                        //Program.form.mesPrintln(string.Format(".....點  3:{0:f},{1:f}", tmpangle2, ArmData.coordinate[i, 3]));
                        ArmData.coordinate[i, 3] = (1) * (tmpangle2 + ArmData.coordinate[i, 3]);
                        
                    }
                    else
                    {
                        //Program.form.mesPrintln(string.Format(".....點  3:{0:f},{1:f}", tmpangle2, ArmData.coordinate[i, 3]));
                        ArmData.coordinate[i, 3] = (1) * (ArmData.coordinate[i, 3] - tmpangle2);

                    }
                    /*
                    coordinate[i + 5, 0] = coordinate[i, 0];
                    coordinate[i + 5, 1] = coordinate[i, 1];
                    coordinate[i + 5, 2] = coordinate[i, 2];
                    coordinate[i + 5, 3] = coordinate[i, 3];
                    */
                }
                Program.form.mesPrintln(string.Format("各平台直線進入初始四軸角度 1axis:{0:f} 2axis:{1:f} 3axis:{2:f} 4axis:{3:f} \n", ArmData.coordinate[i, 0], ArmData.coordinate[i, 1], ArmData.coordinate[i, 2], ArmData.coordinate[i, 3]));
            }


        }


        public static void checkcassette(double[,] checkcassette)
        {
            int i = 0;
            double num = 130;

            for (i = 5; i >= 0; i--)//0最高   5最低
            {
                checkcassette[0, i] = num ;
                checkcassette[0, i] = num ;
                num = num + 0.48;
            }
            /*
            checkcassette[0, 0] = ;
            checkcassette[0, 1] = ;
            checkcassette[0, 2] = ;
            checkcassette[0, 3] = ;
            checkcassette[0, 4] = ;
            checkcassette[0, 5] = ;
            */
            checkcassette[1, 0] = checkcassette[0, 0];
            checkcassette[1, 1] = checkcassette[0, 1];
            checkcassette[1, 2] = checkcassette[0, 2];
            checkcassette[1, 3] = checkcassette[0, 3];
            checkcassette[1, 4] = checkcassette[0, 4];
            checkcassette[1, 5] = checkcassette[0, 5];
        }

        public static AngleList routplanning(double angle1, double angle2, double angle3, double angle4, double distance, int pointsnum)
        {

            MotorAngle origmotor = new MotorAngle(angle1, angle2, angle3, angle4);

            if (distance > 0)
            {
                calcu(origmotor, distance, pointsnum);
            }
            else
            {
                back(origmotor, distance, pointsnum);
            }

            decimal[,] data = new decimal[100, 2];


            StreamReader angleFileReader = new StreamReader("moveangle.txt");


            for (int linenum = 0; !angleFileReader.EndOfStream; linenum++)
            {
                string[] strSplit = angleFileReader.ReadLine().Split(',');

                
                data[linenum, 0] = Convert.ToDecimal(strSplit[0]);
                data[linenum, 1] = Convert.ToDecimal(strSplit[1]);
                
            }
            /*
            angle1 = 10;
            angle2 = 10;
            angle3 = 10;
            angle4 = 10;
            */
            angleFileReader.Close();


            StreamWriter angleFileWriter = new StreamWriter("moveangle.txt");

            AngleList list = new AngleList((decimal)angle1, (decimal)angle2,(decimal)angle3, (decimal)angle4);
            Angle currnode = list.headAngle;

            decimal tmpangle = (decimal)angle4;


            for (int i = 0; i < pointsnum; i++)
            {
                if (i == 0)
                {
                    tmpangle -= ((data[i, 0] - (decimal)angle1) + (data[i, 1] - (decimal)angle3));
                }
                else
                {
                    tmpangle -= ((data[i, 0] - data[i - 1, 0]) + (data[i, 1] - data[i - 1, 1]));
                }

                angleFileWriter.WriteLine("{0:f12},{1:f12},{2:f12}", data[i, 0], data[i, 1],  tmpangle);

                currnode.creatNextNode(data[i, 0], (decimal)angle2,data[i, 1], tmpangle);
                currnode = currnode.nextangle;

            }


            angleFileWriter.Close();

            return list;

        }

    }

}
