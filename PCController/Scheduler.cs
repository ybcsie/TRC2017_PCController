using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCController
{
    class Scheduler
    {
        public static int[] missiontime=new int[3];
        public static int[,] missionstate=new int[5,3];
        public static int[] missionnum=new int[3];
        public static int armtime;


        public static int savenum = 0;
        public static int[,] finalpath1=new int[50000,100];
        public static int[,] finalpath2 = new int[50000, 100];
        public static int[,] finalpath3 = new int[50000, 100];
        public static int[,] finalpath4 = new int[50000, 100];
        public static int[,] finalpath5 = new int[50000, 100];
        public static int[,] finalpath6 = new int[50000, 100];
        public static int[,] finaltime = new int[50000, 100]; 
        public static int x, y, wx, wy;


        public static void ScheduleFunction(int[,] scheduleing,int[] missiontime0, int[,] missionstate0, int[] missionnum0, int armtime0)
        {
            int alltime;
            int[,] statetime=new int[5,3];//0 cassette A,1 A,2 B
            int[,] statepriority=new int[5,3];
            int[] statewafernum=new int[5];
            int[] path=new int[6];
            int count;
            int ans;
            char[,] pathchar = new char[24, 2];

            x = 0;
            y = 0;
            wx = 0;
            wy = 0;

            int i = 0, j = 0;
            alltime = 0;
            count = 0;
            armtime = armtime0;

            for (i = 0; i < 50000; i++)
            {
                for (j = 0; j < 100; j++)
                {
                    finalpath1[i,j] = 0;
                    finalpath2[i,j] = 0;
                    finalpath3[i,j] = 0;
                    finalpath4[i,j] = 0;
                    finalpath5[i,j] = 0;
                    finalpath6[i,j] = 0;
                    finaltime[i,j] = 0;
                }
            }

            for (i=0;i<3;i++)
            {
                missiontime[i] = missiontime0[i];
                missionnum[i] = missionnum0[i];
            }

            for (i = 0; i < 5; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    missionstate[i, j] = missionstate0[i, j];
                    statepriority[i,j] = 0;
                    statetime[i,j] = 0;
                }
            }
            for (i = 0; i < 5; i++)
            {
                statewafernum[i] = 0;
            }
            statewafernum[0] = 6;
            for (i = 0; i < 6; i++)
            {
                path[i] = 0;
            }

            scheduling(alltime, statetime, statepriority, count, path, statewafernum);

            Program.form.mesPrintln(String.Format("total road {0:d}\n", savenum));

            x = 0;
            y = 0;

            ans = findthefast();
           /* 
                for(i=0;i<100;i++){
                for(j=0;j<50;j++){
                    if(finaltime[j,i]!=0){
                        Program.form.mesPrint(String.Format("{0:d} ", finaltime[j,i]));
                    }
                }
                if(finaltime[j,i]!=0)
                    Program.form.mesPrintln("");
            }
            */
            Program.form.mesPrintln(String.Format("path: {0:d} {1:d} {2:d} {3:d} {4:d} {5:d} totaltime:{6:d}\n", finalpath1[x,y], finalpath2[x, y], finalpath3[x, y], finalpath4[x, y], finalpath5[x, y], finalpath6[x, y], finaltime[x,y]));

            for (i = 3;i>=0; i--)
            {
                scheduleing[i, 1] = finalpath1[x, y] % 10;
                finalpath1[x, y] = finalpath1[x, y] / 10;
                scheduleing[i, 0] = finalpath1[x, y] % 10;
                finalpath1[x, y] = finalpath1[x, y] / 10;
                scheduleing[4+i, 1] = finalpath2[x, y] % 10;
                finalpath2[x, y] = finalpath2[x, y] / 10;
                scheduleing[4+i, 0] = finalpath2[x, y] % 10;
                finalpath2[x, y] = finalpath2[x, y] / 10;
                scheduleing[8+i, 1] = finalpath3[x, y] % 10;
                finalpath3[x, y] = finalpath3[x, y] / 10;
                scheduleing[8+i, 0] = finalpath3[x, y] % 10;
                finalpath3[x, y] = finalpath3[x, y] / 10;
                scheduleing[12+i, 1] = finalpath4[x, y] % 10;
                finalpath4[x, y] = finalpath4[x, y] / 10;
                scheduleing[12+i, 0] = finalpath4[x, y] % 10;
                finalpath4[x, y] = finalpath4[x, y] / 10;
                scheduleing[16+i, 1] = finalpath5[x, y] % 10;
                finalpath5[x, y] = finalpath5[x, y] / 10;
                scheduleing[16+i, 0] = finalpath5[x, y] % 10;
                finalpath5[x, y] = finalpath5[x, y] / 10;
                scheduleing[20+i, 1] = finalpath6[x, y] % 10;
                finalpath6[x, y] = finalpath6[x, y] / 10;
                scheduleing[20+i, 0] = finalpath6[x, y] % 10;
                finalpath6[x, y] = finalpath6[x, y] / 10;
            }

            int xline, yline,k;
            xline = 0;
            yline = 0;
            

            for (i = 0; i < 24; i++)
            {
                for (j = 0; j < 2; j++)
                {
                    if (scheduleing[i, j] !=0 && scheduleing[i, j] !=7)
                    {
                        scheduleing[i, j] = scheduleing[i, j] - 1;
                        for (k = 0; k < 3; k++)
                        {
                            if (scheduleing[i, j] - missionnum[k] >= 0)
                            {
                                scheduleing[i, j] = scheduleing[i, j] - missionnum[k];
                                xline++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        scheduleing[i, j] = missionstate[xline + 1, scheduleing[i, j]];//轉成ABCDEF
                        xline = 0;

                        if(scheduleing[i, j] == 1)
                        {
                            scheduleing[i, j] = 1;
                            pathchar[i, j] = 'A';
                        }
                        else if(scheduleing[i, j] == 2)
                        {
                            scheduleing[i, j] = 2;
                            pathchar[i, j] = 'B';
                        }
                        else if (scheduleing[i, j] == 3)
                        {
                            scheduleing[i, j] = 7;
                            pathchar[i, j] = 'C';
                        }
                        else if (scheduleing[i, j] == 4)
                        {
                            scheduleing[i, j] = 3;
                            pathchar[i, j] = 'D';
                        }
                        else if (scheduleing[i, j] == 5)
                        {
                            scheduleing[i, j] = 8;
                            pathchar[i, j] = 'E';
                        }
                        else if (scheduleing[i, j] == 6)
                        {
                            scheduleing[i, j] = 9;
                            pathchar[i, j] = 'F';
                        }

                    }
                    else if(scheduleing[i, j] == 0)
                    {
                        scheduleing[i, j] = 4;
                        pathchar[i,j] = 'S';
                    }
                    else if(scheduleing[i, j] == 7)
                    {
                        scheduleing[i, j] = 5;
                        pathchar[i, j] = 'Z';
                    }

                }
                Program.form.mesPrintln(string.Format("PATH = {0:c},{1:c}", pathchar[i,0], pathchar[i, 1]));

            }
            

        }

        public static int scheduling(int alltime0, int[,] statetime0, int[,] statepriority0, int count0, int[] path0, int[] statewafernum0)
        {
            int alltime1, alltime2, alltime3, alltime4;
            int[,] statetime1 = new int[5, 3], statetime2 = new int[5, 3], statetime3 = new int[5, 3], statetime4 = new int[5, 3];//0 cassette A,1 A,2 B
            int[,] statepriority1 = new int[5, 3], statepriority2 = new int[5, 3], statepriority3 = new int[5, 3], statepriority4 = new int[5, 3];
            int[] statewafernum1 = new int[5], statewafernum2 = new int[5], statewafernum3 = new int[5], statewafernum4 = new int[5];
            int count1, count2, count3, count4;
            int[] path1 = new int[6], path2 = new int[6], path3 = new int[6], path4 = new int[6];
            int from1, from2, from3, from4, target1, target2, target3, target4;
            int tmp;

            int i = 0, j = 0, judge = 0;
            /*
                if(savenum>50)
                return 0;
            */

            alltime1 = alltime0;
            alltime2 = alltime0;
            alltime3 = alltime0;
            alltime4 = alltime0;
            for (i = 0; i < 5; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    statetime1[i, j] = statetime0[i, j];
                    statepriority1[i, j] = statepriority0[i, j];
                    statetime2[i, j] = statetime0[i, j];
                    statepriority2[i, j] = statepriority0[i, j];
                    statetime3[i, j] = statetime0[i, j];
                    statepriority3[i, j] = statepriority0[i, j];
                    statetime4[i, j] = statetime0[i, j];
                    statepriority4[i, j] = statepriority0[i, j];
                    //printf("shit %d\n",statepriority0[i][j]);
                }
                //printf("shit %d\n",statepriority0[i][j]);
            }
            for (i = 0; i < 6; i++)
            {
                path1[i] = path0[i];
                path2[i] = path0[i];
                path3[i] = path0[i];
                path4[i] = path0[i];
            }
            for (i = 0; i < 5; i++)
            {
                statewafernum1[i] = statewafernum0[i];
                statewafernum2[i] = statewafernum0[i];
                statewafernum3[i] = statewafernum0[i];
                statewafernum4[i] = statewafernum0[i];
            }
            alltime1 = alltime0;
            alltime2 = alltime0;
            alltime3 = alltime0;
            alltime4 = alltime0;
            count1 = count0;
            count2 = count0;
            count3 = count0;
            count4 = count0;

            if (statewafernum1[0] > 0 && statewafernum1[1] < missionnum[0])
            {
                statewafernum1[0]--;
                statewafernum1[1]++;
                from1 = 0;

                //尋找目標
                judge = 0;
                target1 = -1;
                for (i = 0; i < missionnum[0]; i++)
                {
                    if (statepriority1[1, i] == 0 && judge == 0)
                    {
                        statepriority1[1, i] = 1;
                        statetime1[1, i] = missiontime[0];//紀錄工作時間
                        judge = 1;
                        target1 = i;
                    }
                    else if (statepriority1[1, i] != 0)
                    {
                        statepriority1[1, i] = statepriority1[1, i] + 1;
                    }
                }
                if (judge != 1)
                {
                    Program.form.mesPrintln("no target 0");
                }

                alltime1 = alltime1 + armtime;//動到alltime所有都要減時間
                for (i = 0; i < 5; i++)
                {
                    for (j = 0; j < 3; j++)
                    {
                        if (statetime1[i, j] > 0)
                        {
                            if (i != 1 || j != target1)
                            {
                                statetime1[i, j] = statetime1[i, j] - armtime;
                                if (statetime1[i, j] < 0)
                                {
                                    statetime1[i, j] = 0;
                                }
                            }
                        }
                    }
                }


                judge = 0;

                for (i = 0; i < 6 && judge == 0; i++)
                {
                    if (path1[i] < 999999)
                    {
                        path1[i] = path1[i] * 100 + (from1) * 10 + (target1 + 1);
                        judge = 1;
                    }
                }
                count1++;

                scheduling(alltime1, statetime1, statepriority1, count1, path1, statewafernum1);
            }

            i = 0;
            j = 0;
            judge = 0;


            if (statewafernum2[1] > 0 && statewafernum2[2] < missionnum[1])
            {
                //手閉時間
                //printf("mission 2:%d   Wafer2:%d   priori:%d\n",missionnum[1],statewafernum2[2],statepriority2[2][0]);

                statewafernum2[1]--;
                statewafernum2[2]++;

                //找優先權&來源
                tmp = 0;
                from2 = (-1);
                for (i = 0; i < missionnum[0]; i++)
                {
                    //printf("statepriority 1 %d  tmp:%d\n",statepriority2[1][i],tmp);
                    if (statepriority2[1, i] > tmp)
                    {
                        tmp = statepriority2[1, i];
                        from2 = i;
                        //printf("find\n");
                    }
                    //printf("statepriority 1 %d = %d from2:%d tmp:%d \n",i,statepriority2[1][i],from2,tmp);
                }
                if (from2 == (-1))
                {
                    Program.form.mesPrintln("no from 1");

                }

                //開始拿走
                statepriority2[1, from2] = 0;//優先全規0
                                             //printf("fuck from:%d  prio:%d\n",from2,statepriority2[1][from2]);
                                             //紀錄等待from好的時間
                alltime2 = alltime2 + statetime2[1, from2];
                for (i = 0; i < 5; i++)
                {
                    for (j = 0; j < 3; j++)
                    {
                        if (statetime2[i, j] > 0)
                        {
                            statetime2[i, j] = statetime2[i, j] - statetime2[1, from2];
                            if (statetime2[i, j] < 0)
                            {
                                statetime2[i, j] = 0;
                            }
                        }
                    }
                }

                judge = 0;
                target2 = -1;
                for (i = 0; i < missionnum[1]; i++)
                {
                    //printf("prio %d\n",statepriority2[2][i]);
                    if (statepriority2[2, i] == 0 && judge == 0)
                    {
                        statepriority2[2, i] = 1;
                        statetime2[2, i] = missiontime[1];//紀錄工作時間
                        judge = 1;
                        target2 = i;
                    }
                    else if (statepriority2[2, i] != 0)
                    {
                        statepriority2[2, i] = statepriority2[2, i] + 1;
                    }
                }
                if (judge != 1)
                {
                    Program.form.mesPrintln("no target 1");

                }
                alltime2 = alltime2 + armtime;//動到alltime所有都要減時間
                for (i = 0; i < 5; i++)
                {
                    for (j = 0; j < 3; j++)
                    {
                        if (statetime2[i, j] > 0)
                        {
                            if (i != 2 || j != target2)
                            {
                                statetime2[i, j] = statetime2[i, j] - armtime;
                                if (statetime2[i, j] < 0)
                                {
                                    statetime2[i, j] = 0;
                                }
                            }
                        }
                    }
                }

                judge = 0;
                for (i = 0; i < 6 && judge == 0; i++)
                {
                    if (path2[i] < 999999)
                    {
                        path2[i] = path2[i] * 100 + (from2 + 1) * 10 + (target2 + missionnum[0] + 1);
                        judge = 1;
                    }
                }
                count2++;

                scheduling(alltime2, statetime2, statepriority2, count2, path2, statewafernum2);
            }


            if (statewafernum3[2] > 0 && statewafernum3[3] < missionnum[2])
            {
                //手閉時間

                //printf("state2 wafernum:%d\n",statewafernum3[2]);
                statewafernum3[2]--;
                statewafernum3[3]++;

                //找優先權&來源
                tmp = 0;
                from3 = (-1);
                for (i = 0; i < missionnum[1]; i++)
                {
                    //printf("statepriority 1 %d = %d\n",i,statepriority3[1][i]);
                    if (statepriority3[2, i] > tmp)
                    {
                        tmp = statepriority3[2, i];
                        from3 = i;
                    }
                }
                if (from3 == (-1))
                {
                    Program.form.mesPrintln("no from 2");

                }


                //開始拿走
                statepriority3[2, from3] = 0;//優先全規0
                                             //printf("from3:%d\n",from3);
                                             //紀錄等待from好的時間
                alltime3 = alltime3 + statetime3[2, from3];
                for (i = 0; i < 5; i++)
                {
                    for (j = 0; j < 3; j++)
                    {
                        if (statetime3[i, j] > 0)
                        {
                            statetime3[i, j] = statetime3[i, j] - statetime3[2, from3];
                            if (statetime3[i, j] < 0)
                            {
                                statetime3[i, j] = 0;
                            }
                        }
                    }
                }

                judge = 0;
                target3 = -1;
                for (i = 0; i < missionnum[2]; i++)
                {
                    if (statepriority3[3, i] == 0 && judge == 0)
                    {
                        statepriority3[3, i] = 1;
                        statetime3[3, i] = missiontime[1];//紀錄工作時間
                        judge = 1;
                        target3 = i;
                    }
                    else if (statepriority3[3, i] != 0)
                    {
                        statepriority3[3, i] = statepriority3[3, i] + 1;
                    }
                }
                if (judge != 1)
                {
                    Program.form.mesPrintln("no target 2");

                }

                alltime3 = alltime3 + armtime;//動到alltime所有都要減時間
                for (i = 0; i < 5; i++)
                {
                    for (j = 0; j < 3; j++)
                    {
                        if (statetime3[i, j] > 0)
                        {
                            if (i != 3 || j != target3)
                            {
                                statetime3[i, j] = statetime3[i, j] - armtime;
                                if (statetime3[i, j] < 0)
                                {
                                    statetime3[i, j] = 0;
                                }
                            }
                        }
                    }
                }


                judge = 0;

                for (i = 0; i < 6 && judge == 0; i++)
                {
                    if (path3[i] < 999999)
                    {
                        path3[i] = path3[i] * 100 + (from3 + missionnum[0] + 1) * 10 + (target3 + missionnum[1] + missionnum[0] + 1);
                        judge = 1;
                    }
                }
                count3++;


                scheduling(alltime3, statetime3, statepriority3, count3, path3, statewafernum3);
            }


            if (statewafernum4[3] > 0 && statewafernum4[4] < 6)
            {
                //手閉時間
                statewafernum4[3]--;
                statewafernum4[4]++;

                //找優先權&來源
                tmp = 0;
                from4 = (-1);
                for (i = 0; i < missionnum[2]; i++)
                {
                    if (statepriority4[3, i] > tmp)
                    {
                        tmp = statepriority4[3, i];
                        from4 = i;
                    }
                }
                if (from4 == (-1))
                {
                    Program.form.mesPrintln("no from 3");

                }

                //開始拿走
                statepriority4[3, from4] = 0;//優先全規0
                                             //紀錄等待from好的時間
                alltime4 = alltime4 + statetime4[3, from4];
                for (i = 0; i < 5; i++)
                {
                    for (j = 0; j < 3; j++)
                    {
                        if (statetime4[i, j] > 0)
                        {
                            statetime4[i, j] = statetime4[i, j] - statetime4[3, from4];
                            if (statetime4[i, j] < 0)
                            {
                                statetime4[i, j] = 0;
                            }
                        }
                    }
                }
                target4 = 7;
                alltime4 = alltime4 + armtime;//動到alltime所有都要減時間
                for (i = 0; i < 5; i++)
                {
                    for (j = 0; j < 3; j++)
                    {
                        if (statetime4[i, j] > 0)
                        {
                            statetime4[i, j] = statetime4[i, j] - armtime;
                            if (statetime4[i, j] < 0)
                            {
                                statetime4[i, j] = 0;
                            }
                        }
                    }
                }


                judge = 0;
                for (i = 0; i < 6 && judge == 0; i++)
                {
                    if (path4[i] < 999999)
                    {
                        path4[i] = path4[i] * 100 + (from4 + missionnum[0] + missionnum[1] + 1) * 10 + (target4);
                        judge = 1;
                    }
                }
                count4++;
                if (statewafernum4[4] == 6)
                {
                    /////write data;
                    x = savenum % 50000;
                    y = savenum / 50000;

                    finaltime[x, y] = alltime4;

                    finalpath1[x, y] = path4[0];
                    finalpath2[x, y] = path4[1];
                    finalpath3[x, y] = path4[2];
                    finalpath4[x, y] = path4[3];
                    finalpath5[x, y] = path4[4];
                    finalpath6[x, y] = path4[5];

                    savenum++;


                    //printf("find a road %d\n",alltime4);
                }
                else
                {

                    scheduling(alltime4, statetime4, statepriority4, count4, path4, statewafernum4);
                }
            }
            //printf("total time %d\n",savenum);
            return 0;
            
        }

        public static int findthefast()
        {

            int i = 0, j = 0;
            int target = 999999;

            for (i = 0; i < 50000; i++)
            {
                for (j = 0; j < 100; j++)
                {
                    if (finaltime[i,j] != 0)
                    {
                        if (finaltime[i,j] < target)
                        {
                            target = finaltime[i,j];
                            x = i;
                            y = j;
                        }
                        if (finalpath1[i,j] == 1133447 && finalpath2[i,j] == 1133447 && finalpath3[i,j] == 1133447 && finalpath4[i,j] == 1133447 && finalpath5[i,j] == 1133447 && finalpath6[i,j] == 1133447)
                        {
                            wx = i;
                            wy = j;
                        }
                    }
                }
            }
            return target;
        }

    }
}
