using System;
using System.Threading;
using System.Windows.Forms;


namespace PCController
{
    
    public partial class ControllerForm : Form
    {


        public static int[] wafernum = new int[6];
        public static int[] wafernumB = new int[6];
        /*
         * constructors
         * 
         */

        public ControllerForm()
        {
            InitializeComponent();

            initBtText_TRCConnect = bt_TRCConnect.Text;
            initBtText_SyntecConnect = bt_SyntecConnect.Text;


            tB_SyntecIP.Text = SyntecClient.DEFAULT_IP;
            tB_fileName.Text = SyntecClient.DEFAULT_NCFILENAME;

            tB_TRCIP.Text = TRCClient.DEFAULT_IP;
            tB_TRCPort.Text = TRCClient.DEFAULT_PORT.ToString();

            //test
            //bt_test.Enabled = false;

            timer300ms = new System.Windows.Forms.Timer();
            timer300ms.Interval = 300;
            timer300ms.Tick += new EventHandler(timer300ms_Tick);
            timer300ms.Enabled = true;

        }


        /*
         * public functions
         * 
         */


        public void mesPrint(string str)
        {
            while (messLock) ;
            messLock = true;
            messOut += str;
            messModified = true;
            messLock = false;
        }

        public void mesPrintln(string str)
        {
            mesPrint(str + "\r\n");

        }

        public void showWarnning(string str)
        {
            MessageBox.Show(str, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }


        /*
         * private variables
         * 
         */

        private System.Windows.Forms.Timer timer300ms;

        private string messOut = "";
        private volatile bool messModified = false;
        private volatile bool messLock = false;

        private volatile bool isMoverBtClicked = false;

        private string initBtText_TRCConnect;
        private string initBtText_SyntecConnect;

 
        /*
         * private functions
         * 
         */
        private void ControllerForm_Load(object sender, EventArgs e)
        {
            //initial Arm Data
            ArmData.longbase = 305;
            ArmData.longrate2 = 179 / ArmData.longbase;
            ArmData.longrate3 = 340 / ArmData.longbase;

            ArmData.distance = 260;

            ArmData.ratio = 780;

            //initialize coordinate
            RoutPlanning.Initialize();

            mesPrintln("Ready...");

        }

        private void ControllerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ThreadsController.abortAllThreads();
            TRCClient.disconnect();
            SyntecClient.disconnect();
        }



        private void timer300ms_Tick(object sender, EventArgs e)
        {
            refreshMess();

            if (SyntecClient.isConnected())
            {
                bt_SyntecConnect.Text = "Connected";

                panelTestFunc.Enabled = true;
                panelJOG.Enabled = true;
                panel_Initialize.Enabled = true;
                bt_cycleReset.Enabled = true;
                bt_servoSW.Enabled = true;

                panel_Initializer.Enabled = Initializer.moverRunning;
                panel_movStage.Enabled = !Initializer.moverRunning & Initializer.isMovStageAllowed;

                SyntecClient.refresh();

                label_syntecBusy.Text = SyntecClient.stateStr();

                cB_initModeOn.Checked = SyntecClient.readReg(25) == 0 ? false : true;
                //num_JOGSpeed.Value = SyntecClient.readReg(17);

                if (SyntecClient.Mach != null)
                    label_pos.Text =
                        string.Format("C1 = {0:f3} ({1:f3})   ", SyntecClient.Mach[0], SyntecClient.Pos[0]) +
                        string.Format("C2 = {0:f3} ({1:f3})   ", SyntecClient.Mach[1], SyntecClient.Pos[1]) +
                        string.Format("C3 = {0:f3} ({1:f3})   ", SyntecClient.Mach[2], SyntecClient.Pos[2]) +
                        string.Format("C4 = {0:f3} ({1:f3})   ", SyntecClient.Mach[3], SyntecClient.Pos[3]);
                else
                    label_pos.Text = "Unable to get position";

            }
            else
            {
                bt_SyntecConnect.Text = initBtText_SyntecConnect;
                bt_SyntecConnect.Enabled = true;

                panelTestFunc.Enabled = false;
                panelJOG.Enabled = false;
                panel_Initialize.Enabled = false;
                panel_Initializer.Enabled = false;
                bt_cycleReset.Enabled = false;
                bt_servoSW.Enabled = false;

            }


            if (TRCClient.isConnected())
            {
                bt_TRCConnect.Text = "Connected";
            }
            else
            {
                bt_TRCConnect.Text = initBtText_TRCConnect;
                bt_TRCConnect.Enabled = true;

            }

        }



        private void auto()
        {
            ArmData.distance = 270;
            //need modify
            double[,] cassettezaxis = new double[2, 6];//0是cassetteA,1是cassetteB
            int[,] scheduleing = new int[100, 2];
            int[] missiontime0 = new int[3];
            int[,] missionstate0 = new int[5, 3];
            int[] missionnum0 = new int[3];
            int checklargestate = 0;
            int[,] lackstate = new int[26, 6];
            int[] savestate = new int[9];
            int[,] missiontmp0 = new int[5, 3];
            int armtime0;
            int i = 0;
            int j = 0;

            for (i = 0; i < 3; i++)
            {
                missiontime0[i] = 0;
            }

            const int pointsnum = 40;

            AngleList[] go = new AngleList[10];


            //roudplaining

            for (i = 0; i < 10; i++)
            {
                //Program.form.mesPrintln(string.Format(" 1axis:{0:f} 2axis:{1:f} 3axis:{2:f} 4axis:{3:f} \n", coordinate[i, 0], coordinate[i, 1], coordinate[i, 2], coordinate[i, 3]));
                go[i] = RoutPlanning.routplanning(ArmData.coordinate[i, 0], ArmData.coordinate[i, 1], ArmData.coordinate[i, 2], ArmData.coordinate[i, 3], ArmData.distance, pointsnum);
            }


            //end plaining 

            //Nccode generator

            NCGen.generator(go);

            Thread.Sleep(500);

            mesPrintln("NCGen: NC Code generation is done.");

            SyntecClient.cycleReset();

            SyntecClient.uploadNCFile(SyntecClient.NCFileName.MAIN_JOB);

            SyntecClient.cycleStart();
            RoutPlanning.checkcassette(cassettezaxis);
            //TRCClient.handShake();
            TRCClient.init1();
            //>>>>>>>>>>>>>>>>>任務內容
            missiontime0[0] = TRCClient.record_time[0, 0];
            missiontime0[1] = TRCClient.record_time[0, 1];
            missiontime0[2] = TRCClient.record_time[0, 2];
            Program.form.mesPrintln(string.Format("missiontime0 = {0:d},{1:d},{2:d}", missiontime0[0], missiontime0[1], missiontime0[2]));
            for (i = 0; i < 5; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    missiontmp0[i, j] = 0;
                }
            }
            for (j = 0; j < 6; j++)
            {
                for (i = 0; i < 3; i++)
                {
                    if (TRCClient.record_stage[j, i] > 100)
                    {
                        missionnum0[i] = 3;
                    }
                    else if (TRCClient.record_stage[j, i] > 10)
                    {
                        missionnum0[i] = 2;
                    }
                    else
                    {
                        missionnum0[i] = 1;
                    }
                }
                if ((missionnum0[0] + missionnum0[2] + missionnum0[1]) == 6)
                {
                    checklargestate = j;
                    j = 6;
                }
            }
            for (i = 0; i < 25; i++)
            {
                for (j = 0; j < 6; j++)
                {
                    lackstate[i, j] = 0;
                }
            }
            Program.form.mesPrintln(string.Format("m num = {0:d},{1:d},{2:d}", missionnum0[0], missionnum0[1], missionnum0[2]));
            for (i = 1; i < 4; i++)
            {
                //Program.form.mesPrintln(string.Format("r stage = {0:d}", TRCClient.record_stage[0, i - 1]));
                if (TRCClient.record_stage[checklargestate, i - 1] > 100)
                {
                    savestate[i - 1] = TRCClient.record_stage[checklargestate, i - 1];
                    missionstate0[i, 2] = TRCClient.record_stage[checklargestate, i - 1] % 10;
                    TRCClient.record_stage[checklargestate, i - 1] = TRCClient.record_stage[checklargestate, i - 1] / 10;
                    missionstate0[i, 1] = TRCClient.record_stage[checklargestate, i - 1] % 10;
                    TRCClient.record_stage[checklargestate, i - 1] = TRCClient.record_stage[checklargestate, i - 1] / 10;
                    missionstate0[i, 0] = TRCClient.record_stage[checklargestate, i - 1] % 10;
                    Program.form.mesPrintln(string.Format("m stage = {0:d},{1:d},{2:d}", missionstate0[i, 0], missionstate0[i, 1], missionstate0[i, 2]));
                }
                else if (TRCClient.record_stage[checklargestate, i - 1] > 10)
                {
                    savestate[i - 1] = TRCClient.record_stage[checklargestate, i - 1];
                    missionstate0[i, 1] = TRCClient.record_stage[checklargestate, i - 1] % 10;
                    TRCClient.record_stage[checklargestate, i - 1] = TRCClient.record_stage[checklargestate, i - 1] / 10;
                    missionstate0[i, 0] = TRCClient.record_stage[checklargestate, i - 1] % 10;
                    Program.form.mesPrintln(string.Format("m stage = {0:d},{1:d}", missionstate0[i, 0], missionstate0[i, 1]));
                }
                else
                {
                    savestate[i - 1] = TRCClient.record_stage[checklargestate, i - 1];
                    missionstate0[i, 0] = TRCClient.record_stage[checklargestate, i - 1] % 10;
                    Program.form.mesPrintln(string.Format("m stage = {0:d}", missionstate0[i, 0]));
                }
            }
            int k, l, m, n, x, z, a, b;
            n = 0;
            for (j = 0; j < 6; j++)
            {
                if (j != checklargestate)
                {
                    if (savestate[0] != TRCClient.record_stage[j, 0] || savestate[1] != TRCClient.record_stage[j, 1] || savestate[2] != TRCClient.record_stage[j, 2])
                    {
                        Program.form.mesPrintln(string.Format("fuck {0:d} = {1:d},{2:d},{3:d}", j, TRCClient.record_stage[j, 0], TRCClient.record_stage[j, 1], TRCClient.record_stage[j, 2]));
                        for (i = 1; i < 4; i++)
                        {
                            if (TRCClient.record_stage[j, i - 1] > 100)
                            {
                                missiontmp0[i, 2] = TRCClient.record_stage[j, i - 1] % 10;
                                TRCClient.record_stage[j, i - 1] = TRCClient.record_stage[j, i - 1] / 10;
                                missiontmp0[i, 1] = TRCClient.record_stage[j, i - 1] % 10;
                                TRCClient.record_stage[j, i - 1] = TRCClient.record_stage[j, i - 1] / 10;
                                missiontmp0[i, 0] = TRCClient.record_stage[j, i - 1] % 10;
                                Program.form.mesPrintln(string.Format("m stage {0:d} = {1:d},{2:d},{3:d}", j, missiontmp0[i, 0], missiontmp0[i, 1], missiontmp0[i, 2]));
                            }
                            else if (TRCClient.record_stage[j, i - 1] > 10)
                            {
                                missiontmp0[i, 1] = TRCClient.record_stage[j, i - 1] % 10;
                                TRCClient.record_stage[j, i - 1] = TRCClient.record_stage[j, i - 1] / 10;
                                missiontmp0[i, 0] = TRCClient.record_stage[j, i - 1] % 10;
                                Program.form.mesPrintln(string.Format("m stage {0:d}= {1:d},{2:d}", j, missiontmp0[i, 0], missiontmp0[i, 1]));
                            }
                            else
                            {
                                missiontmp0[i, 0] = TRCClient.record_stage[j, i - 1] % 10;
                                Program.form.mesPrintln(string.Format("m stage{0:d} = {1:d}", j, missiontmp0[i, 0]));
                            }

                        }
                        x = 0;
                        for (k = 1; k < 4; k++)
                        {
                            for (l = 0; l < 3; l++)
                            {
                                if (missionstate0[k, l] != 0)
                                {
                                    for (m = 0; m < 3 && n == 0; m++)
                                    {
                                        if (missionstate0[k, l] == missiontmp0[k, m])
                                        {
                                            n = 1;

                                        }
                                    }
                                    if (n == 0)
                                    {
                                        lackstate[TRCClient.record_wafer[j], x] = l + 1;
                                        for (a = 0; a < k - 1; a++)
                                        {
                                            lackstate[TRCClient.record_wafer[j], x] = lackstate[TRCClient.record_wafer[j], x] + missionnum0[a];
                                        }

                                        x++;
                                    }
                                    else
                                    {
                                        n = 0;
                                    }
                                }
                            }

                            Program.form.mesPrintln(string.Format("lack{0:d} = {1:d} {2:d} {3:d}  ", TRCClient.record_wafer[j], lackstate[TRCClient.record_wafer[j], 0], lackstate[TRCClient.record_wafer[j], 1], lackstate[TRCClient.record_wafer[j], 2]));
                        }

                    }
                }



                for (k = 0; k < 5; k++)
                {
                    for (l = 0; l < 3; l++)
                    {
                        missiontmp0[k, l] = 0;
                    }
                }
            }
            /*
            missionstate0[1, 0] = 1; missionstate0[1, 1] = 4;
            missionstate0[2, 0] = 2;
            missionstate0[3, 0] = 5; missionstate0[3, 1] = 3; missionstate0[3, 2] = 6;
            */
            armtime0 = 8;

            //<<<<<<<<<<<<<<<<<<
            //scheduleing
            

            if(missiontime0[0]== missiontime0[1] && missiontime0[1]== missiontime0[2])
            {
                scheduleing[0, 0] = 4;
                scheduleing[0, 1] = 1;
                scheduleing[1, 0] = 1;
                scheduleing[1, 1] = 2;
                scheduleing[2, 0] = 2;
                scheduleing[2, 1] = 7;
                scheduleing[3, 0] = 7;
                scheduleing[3, 1] = 3;
                scheduleing[4, 0] = 3;
                scheduleing[4, 1] = 8;
                scheduleing[5, 0] = 8;
                scheduleing[5, 1] = 9;
                scheduleing[6, 0] = 9;
                scheduleing[6, 1] = 5;
                wafernum[0] = 0;
                wafernum[1] = 0;
                wafernum[2] = 0;
                wafernum[3] = 0;
                wafernum[4] = 0;
                wafernum[5] = TRCClient.record_wafer[0];
                wafernumB[0] = 0;
                wafernumB[1] = 0;
                wafernumB[2] = 0;
                wafernumB[3] = 0;
                wafernumB[4] = 0;
                wafernumB[5] = TRCClient.record_dst[0];
            }
            else
            {
                wafernum[0] = TRCClient.record_wafer[0];
                wafernum[1] = TRCClient.record_wafer[1];
                wafernum[2] = TRCClient.record_wafer[2];
                wafernum[3] = TRCClient.record_wafer[3];
                wafernum[4] = TRCClient.record_wafer[4];
                wafernum[5] = TRCClient.record_wafer[5];
                wafernumB[0] = TRCClient.record_dst[0];
                wafernumB[1] = TRCClient.record_dst[1];
                wafernumB[2] = TRCClient.record_dst[2];
                wafernumB[3] = TRCClient.record_dst[3];
                wafernumB[4] = TRCClient.record_dst[4];
                wafernumB[5] = TRCClient.record_dst[5];
                int tmp, tmpB;
                for (i = 0; i < 6; i++)
                {
                    tmp = wafernum[0];
                    for (j = 1; j < (6 - i); j++)
                    {
                        tmp = wafernum[j - 1];
                        tmpB = wafernumB[j - 1];
                        if (tmp < wafernum[j])
                        {
                            wafernum[j - 1] = wafernum[j];
                            wafernumB[j - 1] = wafernumB[j];
                            wafernum[j] = tmp;
                            wafernumB[j] = tmpB;
                        }
                    }
                }
                Scheduler.ScheduleFunction(scheduleing, missiontime0, missionstate0, missionnum0, armtime0, lackstate, wafernum);
            }

            Program.form.mesPrintln(string.Format("Wnum = {0:d},{1:d},{2:d},{3:d},{4:d},{5:d}", wafernum[0], wafernum[1], wafernum[2], wafernum[3], wafernum[4], wafernum[5]));


            /*
            for(i=0; i < 24; i++)
            {
                    Program.form.mesPrintln(string.Format("PATH = {0:d},{1:d}", scheduleing[i, 0], scheduleing[i, 1]));
                
            }
            */


            //roudplaining
            /*
            for (i = 0; i < 10; i++)
            {
                //Program.form.mesPrintln(string.Format(" 1axis:{0:f} 2axis:{1:f} 3axis:{2:f} 4axis:{3:f} \n", coordinate[i, 0], coordinate[i, 1], coordinate[i, 2], coordinate[i, 3]));
                go[i] = RoutPlanning.routplanning(ArmData.coordinate[i, 0], ArmData.coordinate[i, 1], ArmData.coordinate[i, 2], ArmData.coordinate[i, 3], ArmData.distance, pointsnum);
            }


            //end plaining 

            //Nccode generator

            NCGen.generator(go);

            Thread.Sleep(500);

            mesPrintln("NCGen: NC Code generation is done.");

            SyntecClient.cycleReset();

            SyntecClient.uploadNCFile(SyntecClient.NCFileName.MAIN_JOB);

            SyntecClient.cycleStart();
            */

            controlwhile(scheduleing, cassettezaxis);
            

        }

        private void test()
        {
            double oversignal = 0;
            while (oversignal == 0)
            {//while接收控制器回傳動作結束@11=1;
                oversignal = SyntecClient.readSingleVar(11);//@11

            }
            Program.form.mesPrintln("hihihi");
        }


        private void controlwhile(int[,] scheduleing, double[,] cassettezaxis)
        {
            int WaferonHand = 0, WaferinCassettA = 6, WaferinCassettB = 0;
            int[] WaferonChamber = new int[10];//1 for A,2 for B,3 for D,7 for C,8 for E,9 for F
            int step = 0, i = 0,tmp=0;
            double oversignal = 0;
            int[] correspondChambername = new int[10];
            int director = 0,predirector=0;

            Program.form.mesPrintln(string.Format("fuck on {0:d}", scheduleing[5, 0]));

            correspondChambername[1] = 1;
            correspondChambername[2] = 2;
            correspondChambername[3] = 4;
            correspondChambername[4] = 0;
            correspondChambername[5] = 7;
            correspondChambername[7] = 3;
            correspondChambername[8] = 5;
            correspondChambername[9] = 6;

            for (i = 0; i < 10; i++)
            {
                WaferonChamber[i] = 0;
            }
            SyntecClient.writeGVar(5, 0);
            SyntecClient.writeGVar(6, 0);
            SyntecClient.writeReg(50, 0);//@11=0
            oversignal = SyntecClient.readReg(50);
            while (oversignal == 0)
            {//while接收控制器回傳動作結束@11=1;
                Thread.Sleep(500);
                Program.form.mesPrintln("等待開始");
                oversignal = SyntecClient.readReg(50);
            }
            SyntecClient.writeReg(50, 0);//@11=0

            while (scheduleing[step, 0] != 0)
            {
                if (scheduleing[step, 0] == 1 || scheduleing[step, 0] == 2 || scheduleing[step, 0] == 7 || scheduleing[step, 0] == 8 || scheduleing[step, 0] == 3)
                {
                    director = 1;
                }
                else
                {
                    director = 0;
                }
                if (director != predirector)
                {
                    SyntecClient.writeGVar(6, 1);
                }
                else
                {
                    SyntecClient.writeGVar(6, 0);
                }
                predirector = director;
                director = 0;

                SyntecClient.writeReg(50, 0);//@100050=0
                SyntecClient.writeGVar(2, 1); //設定@2，抓為0，亦即吸盤吸
                
                if (scheduleing[step, 0] != 4)
                {
                    SyntecClient.writeGVar(3, ArmData.coordinate[scheduleing[step, 0] - 1, 1] - 8);//設定@3,Z軸伸長時高度(coordinate[scheduleing[step,0],1])
                    SyntecClient.writeGVar(4, ArmData.coordinate[scheduleing[step, 0] - 1, 1] + 8);//設定@4,Z軸收回時高度(coordinate[scheduleing[step,0],1])

                }
                else
                {
                    SyntecClient.writeGVar(3, ArmData.WaferZs_IO[WaferinCassettA - 1,0]);//設定@3,Z軸伸長至cassetteA時高度
                    SyntecClient.writeGVar(4, ArmData.WaferZs_IO[WaferinCassettA - 1, 1]);//設定@4,Z軸收回時高度
                }

                SyntecClient.writeGVar(1, scheduleing[step, 0]);//設定@1為scheduleing[i,0]
                //控制器進行第一動作
                Thread.Sleep(3000);
                //Program.form.mesPrintln("write 5 1");
                if (scheduleing[step, 0] == 4) {
                    while (TRCClient.sentEvent(0, correspondChambername[scheduleing[step, 0]], wafernum[WaferinCassettA - 1], wafernum[WaferinCassettA - 1]) != 1)
                    {
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    while (TRCClient.sentEvent(0, correspondChambername[scheduleing[step, 0]], WaferonChamber[scheduleing[step, 0]],0) != 1)
                    {
                        Thread.Sleep(1000);
                    }
                }

                
                //控制器進行接下動作


                Program.form.mesPrintln(String.Format("Wait for grab {0:d}", scheduleing[step, 0]));
                SyntecClient.writeGVar(5, 1);//發送動作許可請求，接受後寫@5為1
                Program.form.mesPrintln("write 5 1");
                oversignal = SyntecClient.readReg(50);
                while (oversignal == 0)
                {//while接收控制器回傳動作結束@11=1;
                    Thread.Sleep(500);
                    oversignal = SyntecClient.readReg(50);
                }
                //Program.form.mesPrintln("hihihi");
                if (scheduleing[step, 0] == 4)
                {
                    WaferonHand = wafernum[WaferinCassettA-1];
                    WaferinCassettA--;
                }
                else
                {
                    if (WaferonChamber[scheduleing[step, 0]] == 0)
                    {
                        Program.form.showWarnning(string.Format("there is no Wafer on {0:d}", scheduleing[step, 0]));
                    }
                    WaferonHand = WaferonChamber[scheduleing[step, 0]];
                    WaferonChamber[scheduleing[step, 0]] = 0;
                }


                if (scheduleing[step, 1] == 1 || scheduleing[step, 1] == 2 || scheduleing[step, 1] == 7 || scheduleing[step, 1] == 8 || scheduleing[step, 1] == 3)
                {
                    director = 1;
                }
                else
                {
                    director = 0;
                }
                if (director != predirector)
                {
                    SyntecClient.writeGVar(6, 1);
                }
                else
                {
                    SyntecClient.writeGVar(6, 0);
                }
                predirector = director;
                director = 0;
                SyntecClient.writeReg(50, 0);//@11=0
                //發送執行結束許可
                if (scheduleing[step, 0] == 4)
                {
                    TRCClient.sentEvent(1, correspondChambername[scheduleing[step, 0]], WaferonHand, wafernum[WaferinCassettA]);
                }
                else
                {
                    TRCClient.sentEvent(1, correspondChambername[scheduleing[step, 0]], WaferonHand, 0);
                }
                Thread.Sleep(3500);


                SyntecClient.writeGVar(2, 0);//設定@2，放為1，意即吸盤不吸
                if (scheduleing[step, 1] != 5)
                {
                    SyntecClient.writeGVar(3, ArmData.coordinate[scheduleing[step, 1] - 1, 1] + 12);//設定@3,Z軸伸長放時高度(coordinate[scheduleing[step,1],2])
                    SyntecClient.writeGVar(4, ArmData.coordinate[scheduleing[step, 1] - 1, 1] - 4);//設定@4,Z軸縮回時高度(coordinate[scheduleing[step,1],2])

                }
                else
                {
                    SyntecClient.writeGVar(3, cassettezaxis[1, WaferinCassettB] + 2);//設定@3,Z軸伸長至cassetteB放時高度
                    SyntecClient.writeGVar(4, cassettezaxis[1, WaferinCassettB] - 4);//設定@4,Z軸縮回時高度
                }
                SyntecClient.writeGVar(1, scheduleing[step, 1]);//設定@1為scheduleing[i,1]
                                                                //控制器進行動作
                Program.form.mesPrintln(String.Format("Wait for put {0:d}", scheduleing[step, 1]));
                //控制器進行第一動作
                Thread.Sleep(3000);

                

                if (scheduleing[step, 1] == 5)//發送動作許可請求，接受後寫@5為1
                {
                    while (TRCClient.sentEvent(2, correspondChambername[scheduleing[step, 1]], WaferonHand, wafernum[5-WaferinCassettB]) != 1)
                    {
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    while (TRCClient.sentEvent(2, correspondChambername[scheduleing[step, 1]], WaferonHand, 0) != 1)
                    {
                        Thread.Sleep(1000);
                    }
                }
                Program.form.mesPrintln("write 5 1");
                SyntecClient.writeGVar(5, 1);
                //控制器進行接下動作
                oversignal = SyntecClient.readReg(50);
                while (oversignal == 0)
                {//while接收控制器回傳動作結束@100050=1;
                    Thread.Sleep(500);
                    oversignal = SyntecClient.readReg(50);//@100050
                }
                if (scheduleing[step, 1] == 5)
                {
                    WaferinCassettB++;
                    tmp = WaferonHand;
                    WaferonHand = 0;
                }
                else
                {
                    if (WaferonChamber[scheduleing[step, 1]] != 0)
                    {
                        Program.form.showWarnning(string.Format("there is Wafer on {0:d}", scheduleing[step, 1]));
                    }
                    WaferonChamber[scheduleing[step, 1]] = WaferonHand;
                    tmp = WaferonHand;
                    WaferonHand = 0;
                }
                SyntecClient.writeReg(50, 0);//@100050=0                             
                if (scheduleing[step, 1] == 5)//發送執行結束許可
                {
                    TRCClient.sentEvent(3, correspondChambername[scheduleing[step, 1]], tmp, wafernumB[5 - WaferinCassettB + 1]);
                }
                else
                {
                    TRCClient.sentEvent(3, correspondChambername[scheduleing[step, 1]], tmp, 0);
                }
                step = step + 1;
                Thread.Sleep(1500);

            }
            Program.form.showWarnning("mission end");
            TRCClient.end();
        }

        private void setState(string state)
        {
            label_state.Text = state;
            mesPrintln("Local: " + state);
        }
        private void refreshMess()
        {
            if (!messModified)
                return;

            tB_mesPrint.Text = messOut;
            tB_mesPrint.SelectionStart = tB_mesPrint.TextLength;
            tB_mesPrint.ScrollToCaret();
            messModified = false;
        }


        private void bt_SyntecConnect_Click(object sender, EventArgs e)
        {
            bt_SyntecConnect.Enabled = false;
            bt_SyntecConnect.Text = "Connecting...";
            SyntecClient.connect(tB_SyntecIP.Text);

        }


        private void bt_TRCConnect_Click(object sender, EventArgs e)
        {
            bt_TRCConnect.Enabled = false;
            bt_TRCConnect.Text = "Connecting...";
            TRCClient.connect(tB_TRCIP.Text, Convert.ToInt32(tB_TRCPort.Text));

        }


        private void bt_upload_Click(object sender, EventArgs e)
        {
            setState("Uploading...");
            if (SyntecClient.uploadNCFile(tB_fileName.Text) == SyntecClient.SUCCESSFUL)
                setState("Upload successful");

        }

        private void bt_cycStart_Click(object sender, EventArgs e)
        {
            if (SyntecClient.isBusy())
            {
                SyntecClient.cycleStop();
                setState("Cycle stopped!");
            }
            else
            {
                SyntecClient.cycleStart();
                setState("Cycle started!");
            }

        }



        private void bt_writeGVar_Click(object sender, EventArgs e)
        {
            int no = Convert.ToInt32(num_gvarNo.Value);
            double val = Convert.ToDouble(num_gvarValue.Value);

            SyntecClient.writeGVar(no, val);

            setState("Write global " + no.ToString() + " to value " + val.ToString() + " successful!");

        }

        private void bt_readGVar_Click(object sender, EventArgs e)
        {
            int no = Convert.ToInt32(num_gvarNo.Value);
            double val = SyntecClient.readSingleVar(no);

            setState("Read global " + no.ToString() + ": value = " + val.ToString());

        }


        private void bt_genNC_Click(object sender, EventArgs e)
        {
            ThreadsController.addThreadAndStartByFunc(auto);

        }

        private void bt_TRCCommStart_Click(object sender, EventArgs e)
        {
            mesPrintln("Starting TRC communication task2...");
            ThreadsController.addThreadAndStartByFunc(TRCClient.init);
        }

        private void bt_setOrigin_Click(object sender, EventArgs e)
        {
            SyntecClient.setOrigin();
        }

        private void bt_JOG1P_MouseDown(object sender, MouseEventArgs e)
        {
            SyntecClient.JOG(1, SyntecClient.JOGMode.POSITIVE);
        }

        private void bt_JOG1P_MouseUp(object sender, MouseEventArgs e)
        {
            SyntecClient.JOG(1, SyntecClient.JOGMode.STOP);
        }

        private void bt_JOG2P_MouseDown(object sender, MouseEventArgs e)
        {
            SyntecClient.JOG(2, SyntecClient.JOGMode.POSITIVE);
        }

        private void bt_JOG2P_MouseUp(object sender, MouseEventArgs e)
        {
            SyntecClient.JOG(2, SyntecClient.JOGMode.STOP);
        }

        private void bt_JOG3P_MouseDown(object sender, MouseEventArgs e)
        {
            SyntecClient.JOG(3, SyntecClient.JOGMode.POSITIVE);
        }

        private void bt_JOG3P_MouseUp(object sender, MouseEventArgs e)
        {
            SyntecClient.JOG(3, SyntecClient.JOGMode.STOP);
        }

        private void bt_JOG4P_MouseDown(object sender, MouseEventArgs e)
        {
            SyntecClient.JOG(4, SyntecClient.JOGMode.POSITIVE);
        }

        private void bt_JOG4P_MouseUp(object sender, MouseEventArgs e)
        {
            SyntecClient.JOG(4, SyntecClient.JOGMode.STOP);
        }

        private void bt_JOG1N_MouseDown(object sender, MouseEventArgs e)
        {
            SyntecClient.JOG(1, SyntecClient.JOGMode.NEGATIVE);
        }

        private void bt_JOG1N_MouseUp(object sender, MouseEventArgs e)
        {
            SyntecClient.JOG(1, SyntecClient.JOGMode.STOP);
        }

        private void bt_JOG2N_MouseDown(object sender, MouseEventArgs e)
        {
            SyntecClient.JOG(2, SyntecClient.JOGMode.NEGATIVE);
        }

        private void bt_JOG2N_MouseUp(object sender, MouseEventArgs e)
        {
            SyntecClient.JOG(2, SyntecClient.JOGMode.STOP);
        }

        private void bt_JOG3N_MouseDown(object sender, MouseEventArgs e)
        {
            SyntecClient.JOG(3, SyntecClient.JOGMode.NEGATIVE);
        }

        private void bt_JOG3N_MouseUp(object sender, MouseEventArgs e)
        {
            SyntecClient.JOG(3, SyntecClient.JOGMode.STOP);
        }

        private void bt_JOG4N_MouseDown(object sender, MouseEventArgs e)
        {
            SyntecClient.JOG(4, SyntecClient.JOGMode.NEGATIVE);
        }

        private void bt_JOG4N_MouseUp(object sender, MouseEventArgs e)
        {
            SyntecClient.JOG(4, SyntecClient.JOGMode.STOP);
        }

        private void bt_writeO_Click(object sender, EventArgs e)
        {
            ThreadsController.addThreadAndStartByFunc(() =>
            {
                SyntecClient.writeOBit(Convert.ToInt32(num_obitNo.Value), rBt_obitT.Checked);
                while (SyntecClient.isBusy())
                {
                    Thread.Sleep(50);
                }

                mesPrintln("Obit setting successed.");
            });
        }

        private void bt_servoSW_Click(object sender, EventArgs e)
        {
            SyntecClient.motorServoSwitch();
        }

        private void bt_cycleReset_Click(object sender, EventArgs e)
        {
            SyntecClient.cycleReset();
        }


        private void bt_originSetter_Click(object sender, EventArgs e)
        {
            ThreadsController.addThreadAndStartByFunc(Initializer.originSetter);
        }


        private void bt_startInit_Click(object sender, EventArgs e)
        {
            ThreadsController.addThreadAndStartByFunc(Initializer.setup);

        }

        private void bt_movToStage_Click(object sender, EventArgs e)
        {
            ThreadsController.addThreadAndStartByFunc(() =>
            {
                int stageIndex = -1;

                if (rBt_initStage6.Checked)
                    stageIndex = 6;

                else if (rBt_initStage7.Checked)
                    stageIndex = 7;

                else if (rBt_initStage8.Checked)
                    stageIndex = 8;

                Initializer.movToStage(stageIndex);

            });
        }

        private void bt_getPoint_Click(object sender, EventArgs e)
        {
            ThreadsController.addThreadAndStartByFunc(Initializer.getPoint);

        }

        private void bt_movForward_Click(object sender, EventArgs e)
        {
            if (isMoverBtClicked)
                return;

            if (!SyntecClient.isBusy())
                return;

            isMoverBtClicked = true;

            ThreadsController.addThreadAndStartByFunc(() =>
            {

                if (checkBox_precise.Checked)
                    Initializer.linearMOV(1, 5);
                else
                    Initializer.linearMOV(280, 35);

                isMoverBtClicked = false;
            });
        }

        private void bt_movBackward_Click(object sender, EventArgs e)
        {
            if (isMoverBtClicked)
                return;

            if (!SyntecClient.isBusy())
                return;

            isMoverBtClicked = true;

            ThreadsController.addThreadAndStartByFunc(() =>
            {
                Initializer.linearMOV(-1, 5);

                isMoverBtClicked = false;
            });

        }

        private void bt_thetaP_Click(object sender, EventArgs e)
        {
            if (isMoverBtClicked)
                return;

            if (!SyntecClient.isBusy())
                return;

            isMoverBtClicked = true;

            ThreadsController.addThreadAndStartByFunc(() =>
            {
                Initializer.thetaP(checkBox_precise.Checked);
                isMoverBtClicked = false;

            });
        }

        private void bt_thetaN_Click(object sender, EventArgs e)
        {
            if (isMoverBtClicked)
                return;

            if (!SyntecClient.isBusy())
                return;

            isMoverBtClicked = true;

            ThreadsController.addThreadAndStartByFunc(() =>
            {
                Initializer.thetaN(checkBox_precise.Checked);
                isMoverBtClicked = false;

            });
        }

        private void bt_test_Click(object sender, EventArgs e)
        {
            ThreadsController.addThreadAndStartByFunc(Initializer.catchZInit);

        }

        private void bt_start_Click(object sender, EventArgs e)
        {
            ThreadsController.addThreadAndStartByFunc(() =>
            {
                Initializer.home(false);
                auto();
            });

        }


        private void cB_initModeOn_MouseClick(object sender, MouseEventArgs e)
        {
            SyntecClient.writeReg(25, SyntecClient.readReg(25) == 0 ? 1 : 0);
        }

        private void num_JOGSpeed_ValueChanged(object sender, EventArgs e)
        {
            SyntecClient.setJOGSpeed((int)num_JOGSpeed.Value);
        }

        private void bt_comT1_Click(object sender, EventArgs e)
        {
            //
            mesPrintln("Starting TRC communication task1...");
            ThreadsController.addThreadAndStartByFunc(TRCClient.init1);

        }

        private void bt_initCoo_Click(object sender, EventArgs e)
        {
            RoutPlanning.Initialize();
        }

        private void bt_zCatch_Click(object sender, EventArgs e)
        {
            ThreadsController.addThreadAndStartByFunc(() =>
            {
                Initializer.catchZ((int)num_initSlotID.Value);
            });

        }

        private void bt_ZP_MouseDown(object sender, MouseEventArgs e)
        {
            SyntecClient.setJOGSpeed(cB_zPrecise.Checked ? 10 : 100);
            SyntecClient.JOG(3, SyntecClient.JOGMode.POSITIVE);

        }

        private void bt_ZN_MouseDown(object sender, MouseEventArgs e)
        {
            SyntecClient.setJOGSpeed(cB_zPrecise.Checked ? 10 : 100);
            SyntecClient.JOG(3, SyntecClient.JOGMode.NEGATIVE);

        }

        private void bt_ZP_MouseUp(object sender, MouseEventArgs e)
        {
            SyntecClient.JOG(3, SyntecClient.JOGMode.STOP);

        }

        private void bt_ZN_MouseUp(object sender, MouseEventArgs e)
        {
            SyntecClient.JOG(3, SyntecClient.JOGMode.STOP);

        }

        private void bt_prepareGetWafer_Click(object sender, EventArgs e)
        {
            ThreadsController.addThreadAndStartByFunc(Initializer.catchZInit);

        }

        private void bt_goIn_Click(object sender, EventArgs e)
        {
            ThreadsController.addThreadAndStartByFunc(()=>
            {
                Initializer.getWaferTester((int)num_initSlotID.Value);
            });
            
        }

        private void bt_setZin_Click(object sender, EventArgs e)
        {
            double[] pos;
            SyntecClient.getPos(out pos);

            ArmData.WaferZs_IO[(int)num_initSlotID.Value, 0] = pos[2];
            ArmData.writeAngle();
        }

        private void bt_setZout_Click(object sender, EventArgs e)
        {
            double[] pos;
            SyntecClient.getPos(out pos);

            ArmData.WaferZs_IO[(int)num_initSlotID.Value, 1] = pos[2];
            ArmData.writeAngle();

        }

        private void bt_SuckSW_Click(object sender, EventArgs e)
        {
            ThreadsController.addThreadAndStartByFunc(() =>
            {
                SyntecClient.writeOBit(320, !SyntecClient.readOBit(320));

                while (SyntecClient.isBusy())
                {
                    Thread.Sleep(50);
                }

                mesPrintln("Suck SW Successfully.");
            });


        }

        private void bt_manualCatch_Click(object sender, EventArgs e)
        {
            ThreadsController.addThreadAndStartByFunc(Initializer.manualCatch);
        }

        private void bt_readInit_Click(object sender, EventArgs e)
        {
            ThreadsController.addThreadAndStartByFunc(()=>
            {
                Initializer.readInitPoint();
                mesPrintln("Done!");
            });
        }

        private void bt_readZangle_Click(object sender, EventArgs e)
        {
            ThreadsController.addThreadAndStartByFunc(() =>
            {
                ArmData.readAngle();
                mesPrintln("Done!");

            });

        }
    }









    class ThreadsController
    {
        public static int addThreadAndStartByFunc(ThreadStart func)
        {
            if (threads == null)
                threads = new Thread[MAX_COUNT];

            if (threadsCount == MAX_COUNT)
            {
                Program.form.showWarnning("Thread Controller: Threads count > MAX");
                return -1; //return error
            }

            threads[threadsCount] = new Thread(func);
            threads[threadsCount++].Start();

            return 0; //return success

        }

        public static void abortAllThreads()
        {
            if (threads == null)
                return;

            foreach (Thread currThread in threads)
            {
                if (currThread != null)
                    currThread.Abort();
            }
        }

        private const int MAX_COUNT = 500;
        private static int threadsCount = 0;
        private static Thread[] threads = null;

    }
}
