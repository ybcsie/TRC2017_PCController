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

        public static void generator(AngleList[] go)
        {
            const int complete = 100050, access = 5, suck=320, communication = 1,GraborPut=2 ,beginzaxis=3 ,afterzaxis=4 ,changedirector=6;//
            const double topangle =225;
            decimal[,] savedata = new decimal[100, 4];
            int i, number, count;
            double afterz=0;
            int[] cassetteA = new int[6]; 
            int[] cassetteB = new int[6];//紀錄六片wafer在cassette中的位置
            int outnum = 0;//已從cassetteA取出之wafer數
            int putnum = 0;//已放入cassetteB之wafer數
            double casetspacing = 6.4;//mm
            double lowerdefaltz = 20;

            startGenNC(SyntecClient.NCFileName.MAIN_JOB);
            ncfile.WriteLine("@{0:d}:=0;", complete);
            ncfile.WriteLine("WAIT();");
            //ncfile.WriteLine("MOVJ C1=90.0 C2=0.0 C4=0.00 FJ20 PL10;");
            ncfile.WriteLine("MOVJ C1=82.735 C2=174.890 C4=-150.000 FJ20 PL10;");
            ncfile.WriteLine("MOVJ C2=170.0 C4=-150.0 FJ20 PL10;");
            ncfile.WriteLine("MOVJ C2=170.0 C4=150.0 FJ20 PL10;");
            ncfile.WriteLine("MOVJ C2=-170.0 C4=150.0 FJ20 PL10;");
            ncfile.WriteLine("MOVJ C1=85.735 C2=-148.067 C4=116.332 FJ20 PL10;");
            ncfile.WriteLine("WAIT();");
            ncfile.WriteLine("MOVJ C3=121.0 FJ20 PL10;");
            ncfile.WriteLine("WAIT();");
            ncfile.WriteLine("@5:=0;");
            ncfile.WriteLine("@4:=0;");
            ncfile.WriteLine("@2:=0;");
            ncfile.WriteLine("@3:=0;");
            ncfile.WriteLine("@1:=0;");
            ncfile.WriteLine("@{0:d}:=1;",complete);
            NCmain(communication);

            for (i = 0; i<10; i++)
            {
                count = 0;
                Angle tmp = go[i].headAngle;
                ncfile.WriteLine("N{0:d};",(i+11));
                ncfile.WriteLine("@1:=0;");
                //ncfile.WriteLine("@{0:d}:=0;", complete);
                while (tmp != null)
                {
                    
                    if(tmp.four>0 && tmp.four > 150)
                    {
                        tmp.four = 150;
                    }
                    else if (tmp.four<0 && tmp.four<-150)
                    {
                        tmp.four = -150;
                    }
                    
                    if (count == 0)
                    {
                        ncfile.WriteLine("IF (@{0:d} = 1) THEN", changedirector);
                        ncfile.WriteLine("SETDO({0:d},{1:d});", changedirector, 0);
                        if (tmp.four > 0)
                        {
                            //ncfile.WriteLine("MOVJ C4=45.0 FJ20 PL10;");
                            ncfile.WriteLine("MOVJ C2=170.0 C4=-150.0 FJ20 PL10;");
                            ncfile.WriteLine("MOVJ C2=170.0 C4=150.0 FJ20 PL10;");
                            ncfile.WriteLine("MOVJ C2=-170.0 C4=150.0 FJ20 PL10;");
                            ncfile.WriteLine("WAIT();");
                            if (i < 6)
                            {
                                ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3=@{2:d} C4={3:f3} FJ20 PL10;", tmp.one, tmp.three, beginzaxis, tmp.four);
                            }
                            else
                            {
                                ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3={2:f3} C4={3:f3} FJ20 PL10;", tmp.one, tmp.three, lowerdefaltz, tmp.four);
                            }
                            //ncfile.WriteLine("MOVJ C4={0:f3} FJ20 PL10;", tmp.four);
                            ncfile.WriteLine("WAIT();");
                        }
                        else
                        {
                            ncfile.WriteLine("MOVJ C2=-170.0 C4=150.0 FJ20 PL10;");
                            ncfile.WriteLine("MOVJ C2=-170.0 C4=-150.0 FJ20 PL10;");
                            ncfile.WriteLine("MOVJ C2=170.0 C4=-150.0 FJ20 PL10;");
                            ncfile.WriteLine("WAIT();");
                            if (i < 6)
                            {
                                ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3=@{2:d} C4={3:f3} FJ20 PL10;", tmp.one, tmp.three, beginzaxis, tmp.four);
                            }
                            else
                            {
                                ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3={2:f3} C4={3:f3} FJ20 PL10;", tmp.one, tmp.three, lowerdefaltz, tmp.four);
                            }
                            //ncfile.WriteLine("MOVJ C4={0:f3} FJ20 PL10;", tmp.four);
                            ncfile.WriteLine("WAIT();");
                        }
                        ncfile.WriteLine("ELSEIF (@{0:d} = 0) THEN", changedirector);
                        if (i < 6)
                        {
                            ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3=@{2:d} C4={3:f3} FJ20 PL10;", tmp.one, tmp.three, beginzaxis, tmp.four);
                        }
                        else
                        {
                            ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3={2:f3} C4={3:f3} FJ20 PL10;", tmp.one, tmp.three, lowerdefaltz, tmp.four);
                        }
                        ncfile.WriteLine("END_IF;");
                    }
                    else
                    {
                        if (i > 5 && count <15)
                        {
                            ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3={2:f3} C4={3:f3} FJ20 PL10;", tmp.one, tmp.three, lowerdefaltz, tmp.four);
                        }
                        else
                        {
                            ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3=@{2:d} C4={3:f3} FJ20 PL10;", tmp.one, tmp.three, beginzaxis, tmp.four);
                        }
                    }
                    //ncfile.WriteLine("MOVJ C2={1:f3} FJ30 PL5;", tmp.one, tmp.three, tmp.two, tmp.four);
                    savedata[count, 0] = tmp.one;
                    savedata[count, 1] = tmp.two;
                    savedata[count, 2] = tmp.three;
                    savedata[count, 3] = tmp.four;
                    if (count == 0)
                    {
                        ncfile.WriteLine("WAIT();");
                        ncfile.WriteLine("WHILE(@{0:d} = 0) DO",access);
                        ncfile.WriteLine("SLEEP();");
                        ncfile.WriteLine("END_WHILE");
                        ncfile.WriteLine("@5:=0;");
                        ncfile.WriteLine("WAIT();");
                    }

                    tmp = tmp.nextangle;
                    count++;
                }
                //ncfile.WriteLine("WAIT();");
                count--;
                ncfile.WriteLine("WAIT();");
                Obitcontrol(suck, GraborPut);
                ncfile.WriteLine("WAIT();");
                if (i == 3 || i==4)
                {
                    ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3=@{2:d} C4={3:f3} FJ1 PL10;", savedata[count, 0], savedata[count, 2], afterzaxis, savedata[count, 3]);
                }
                else
                {
                    ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3=@{2:d} C4={3:f3} FJ10 PL10;", savedata[count, 0], savedata[count, 2], afterzaxis, savedata[count, 3]);
                }
                
                //ncfile.WriteLine("MOVJ C2={1:f3} FJ30 PL5;", savedata[count, 0], savedata[count, 2], zaxis, savedata[count, 3]);
                ncfile.WriteLine("WAIT();");

                while (count != -1)
                {
                    if (i > 5 && count < 15)
                    {
                        ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3={2:f3} C4={3:f3} FJ20 PL10;", savedata[count, 0], savedata[count, 2], lowerdefaltz, savedata[count, 3]);
                    }
                    else
                    {
                        ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3=@{2:d} C4={3:f3} FJ20 PL10;", savedata[count, 0], savedata[count, 2], afterzaxis, savedata[count, 3]);
                    }
                    //ncfile.WriteLine("MOVJ C2={1:f3} FJ30 PL5;", savedata[count, 0], savedata[count, 2], zaxis, savedata[count, 3]);
                    count--;
                }
                ncfile.WriteLine("WAIT();");
                ncfile.WriteLine("@{0:d}:=1;", complete);
                ncfile.WriteLine("WAIT();");
                ncfile.WriteLine("GOTO 1;");

                //sendcomplete(complete, communication);//發送完成任務訊號，並等待確認

                if (i==4 || i == 8)//若是第6第10平台則不需生成NCcode
                {
                    i++;
                }

            }


            //
            /*
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
                    ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3={2:f3} C4={3:f3} FJ20 PL5;", tmp.one, tmp.three, tmp.two, tmp.four);

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
                    ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3={2:f3} C4={3:f3} FJ20 PL5;", savedata[count, 0], savedata[count, 2], afterz, savedata[count, 3]);
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
                    ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3={2:f3} C4={3:f3} FJ20 PL5;", tmp.one, tmp.three, tmp.two, tmp.four);

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
                    ncfile.WriteLine("MOVJ C1={0:f3} C2={1:f3} C3={2:f3} C4={3:f3} FJ20 PL5;", savedata[count, 0], savedata[count, 2], afterz, savedata[count, 3]);
                    count--;
                }
                ncfile.WriteLine("WAIT();");

                //sendcomplete(complete, communication);

            }
            */
            endGenNC();

        }

        private static void NCmain(int communication)
        {
            ncfile.WriteLine("N1;");
            ncfile.WriteLine("WHILE (@{0:d}=0) DO", communication);
            ncfile.WriteLine("SLEEP();");
            ncfile.WriteLine("END_WHILE");
            ncfile.WriteLine("IF (@{0:d} = 1) THEN",communication);
            ncfile.WriteLine("GOTO 11;", communication);
            ncfile.WriteLine("ELSEIF (@{0:d} = 2) THEN", communication);
            ncfile.WriteLine("GOTO 12;", communication);
            ncfile.WriteLine("ELSEIF (@{0:d} = 3) THEN", communication);
            ncfile.WriteLine("GOTO 13;", communication);
            ncfile.WriteLine("ELSEIF (@{0:d} = 4) THEN", communication);
            ncfile.WriteLine("GOTO 14;", communication);
            ncfile.WriteLine("ELSEIF (@{0:d} = 5) THEN", communication);
            ncfile.WriteLine("GOTO 15;", communication);
            ncfile.WriteLine("ELSEIF (@{0:d} = 7) THEN", communication);
            ncfile.WriteLine("GOTO 17;", communication);
            ncfile.WriteLine("ELSEIF (@{0:d} = 8) THEN", communication);
            ncfile.WriteLine("GOTO 18;", communication);
            ncfile.WriteLine("ELSEIF (@{0:d} = 9) THEN", communication);
            ncfile.WriteLine("GOTO 19;", communication);
            ncfile.WriteLine("ELSE");
            ncfile.WriteLine("GOTO 1;");
            ncfile.WriteLine("END_IF;");
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



        private static string[] initNCLines =
        {
            //1
            "G91 MOVJ C2=@1002 FJ5;",
            //2
            @"
G91 MOVJ C1=10. C2=90. C3=20. FJ20;
G91 MOVJ C1=10. C2=20. C4=-20. FJ20;
G90 MOVJ C1=120. FJ10;
",
            //3
            "G91 MOVJ C4=@1004 FJ5;",
            //4
            "G91 MOVJ C4=30. FJ10;",
            //5 home C1 at left
            @"
G91 MOVJ C3=@1003 FJ20;
G91 MOVJ C2=@1002 C4=@1004 FJ20;
G91 MOVJ C2=-180. C4=-10. FJ20;
G91 MOVJ C2=-120. C4=150. FJ20;
G91 MOVJ C4=90. FJ20;
G90 MOVJ C1=70. FJ10;
",
            //6 home all positive
            @"
G91 MOVJ C3=@1003 FJ20;
G91 MOVJ C2=@1002 C4=@1004 FJ20;
G91 MOVJ C4=90. FJ20;
G91 MOVJ C2=-60. C4=90. FJ20;
G90 MOVJ C1=70. FJ10;
",
            //7
            @"
G91 MOVJ C1=@1001 C2=@1002 C3=@1003 C4=@1004 FJ10;
#11:=700;
#12:=800;
#14:=900;

G91;
FOR #1:=1 TO @699 BY 1 DO
MOVJ C1=@[#11] C2=@[#12] C4=@[#14] PL10 FJ10;

#11:=#11+1;
#12:=#12+1;
#14:=#14+1;

END_FOR;

MOVJ C3=-20. FJ10;

",
            //8
            @"
G91 MOVJ C3=@1003 FJ20;
G91 MOVJ C1=@1001 C2=@1002 C4=@1004 FJ10;
#11:=1600;
#12:=1700;
#14:=1800;

G91;
FOR #1:=1 TO @1500 BY 1 DO
MOVJ C1=@[#11] C2=@[#12] C4=@[#14] PL10 FJ10;

#11:=#11+1;
#12:=#12+1;
#14:=#14+1;

END_FOR;
",
            //9
            @"
@11=0;
"

//N1
+ @"
N1;
SLEEP();

IF (@11 = 1) THEN
GOTO 2;
END_IF;

IF (@11 = 2) THEN
GOTO 3;
END_IF;

IF (@11 = 3) THEN
GOTO 4;
END_IF;

IF (@11 = 4) THEN
GOTO 5;
END_IF;

IF (@11 = 5) THEN
GOTO 6;
END_IF;

IF (@11 = 6) THEN
GOTO 7;
END_IF;

IF (@11 = 7) THEN
GOTO 8;
END_IF;

GOTO 1;
"

//N2 linear mov
+@"
N2;
#11:=1011;
#12:=1012;
#13:=1013;
#14:=1014;

G91;
FOR #1:=1 TO @12 BY 1 DO
MOVJ C1=@[#11] C2=@[#12] C4=@[#14] PL10 FJ10;

#11:=#11+10;
#12:=#12+10;
#14:=#14+10;

END_FOR;
WAIT();
@11 := 0;
GOTO 1;

"


//N3 theta +
+ @"
N3;
G91 MOVJ C1=2. FJ5;
WAIT();
@11 := 0;
GOTO 1;

"

//N4 theta -
+ @"
N4;
G91 MOVJ C1=-2. FJ5;
WAIT();
@11 := 0;
GOTO 1;


"

//N5 move out
+ @"
N5;
#11:=700+@699-1;
#12:=800+@699-1;
#14:=900+@699-1;

G91;
MOVJ C3=20. FJ10;

FOR #1:=1 TO @699 BY 1 DO
MOVJ C1=-@[#11] C2=-@[#12] C4=-@[#14] PL10 FJ10;

#11:=#11-1;
#12:=#12-1;
#14:=#14-1;

END_FOR;
WAIT();
@11 := 0;
GOTO 8;
"

//N6 theta + precise
+ @"
N6;
G91 MOVJ C1=0.2 FJ5;
WAIT();
@11 := 0;
GOTO 1;
"

//N7 theta - precise
+ @"
N7;
G91 MOVJ C1=-0.2 FJ5;
WAIT();
@11 := 0;
GOTO 1;
"

//N8
+ @"
N8;

",
            //10
            @"
G91 MOVJ C3=@1003 FJ5;
"
        };

        public static void genInitNC(int step)
        {
            startGenNC("initializer.txt");

            ncfile.WriteLine(initNCLines[step - 1]);

            endGenNC();
        }

        public static void genObitSetterNC(int addr, bool val)
        {
            startGenNC(SyntecClient.NCFileName.OBIT_SETTER);
            ncfile.WriteLine("SETDO({0:d},{1:d});", addr, val ? 1 : 0);
            endGenNC();
        }


        /*
        private static string getSearchCode(int pointCount)
        {
            string output = "";
            int gvarNO = 100;

            output += "FOR #1:=0 TO 100 BY 10 DO\r\n";
            output += "MOVJ C1 =@[#1011+#1] C2=@[#1012+#1] C4=@[#1014+#1];\r\n";
            output += "WAIT();\r\n";

            output += "#11:=#11+1;\r\n";
            output += "#12:=#12+1;\r\n";
            output += "#13:=#13+1;\r\n";
            output += "#14:=#14+1;\r\n";

            output += "IF(READDI(320) = 1) THEN\r\n";

            output += "GOTO 01;\r\n";

            output += "END_IF\r\n";

            END_FOR;
"
            for (int i = 0; i < pointCount; i++)
            {
                output += string.Format("MOVJ C1=@{0:d} C2=@{1:d} C3=@{2:d} C4=@{3:d} FJ3 PL5;\r\n", gvarNO++, gvarNO++, gvarNO++, gvarNO++);
            }

            return output;

        }
*/

        private static void Obitcontrol(int Opin, int GraborPut)//command: 0 for close,1 for open
        {
            ncfile.WriteLine("IF (@{0:d} = 0) THEN", GraborPut);
            ncfile.WriteLine("SETDO({0:d},{1:d});",Opin,0);
            ncfile.WriteLine("ELSEIF (@{0:d} = 1) THEN", GraborPut);
            ncfile.WriteLine("SETDO({0:d},{1:d});", Opin, 1);
            ncfile.WriteLine("END_IF;");
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
