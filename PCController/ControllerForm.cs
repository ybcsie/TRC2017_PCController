using System;
using System.Threading;
using System.Windows.Forms;


namespace PCController
{

    public partial class ControllerForm : Form
    {



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

            ArmData.longbase = 305;
            ArmData.longrate2 = 0.5901;
            ArmData.longrate3 = 1.25655;

            mesPrintln("Ready...");

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

        private volatile bool initRunning = false;

        private string initBtText_TRCConnect;
        private string initBtText_SyntecConnect;


        /*
         * private functions
         * 
         */

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

                panel_Initializer.Enabled = initRunning;

                SyntecClient.refresh();

                label_syntecBusy.Text = SyntecClient.stateStr();

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

        private void linearMOV(double distance, int pointCount)
        {
            while (SyntecClient.readSingleVar(11) != 0)
                Thread.Sleep(10);

            double[] Pos;
            SyntecClient.getPos(out Pos);

            AngleList linearAngleList = RoutPlanning.routplanning(Pos[0], Pos[2], Pos[1], Pos[3], distance, pointCount);

            SyntecClient.writeGVar(12, pointCount);

            Angle currNode = linearAngleList.headAngle;

            if (currNode != null)
                currNode = currNode.nextangle;
            else
            {
                showWarnning("motivation error!");
                return;
            }

            int i = 0;
            while (currNode != null)
            {
                SyntecClient.writeGVar(1011 + 10 * i, (double)currNode.one);
                SyntecClient.writeGVar(1012 + 10 * i, (double)currNode.three);
                SyntecClient.writeGVar(1014 + 10 * i++, (double)currNode.four);

                currNode = currNode.nextangle;
            }

            SyntecClient.writeGVar(11, 1);
        }


        private void auto()
        {
            const double ratio = 780;

            //need modify

            const double distance = 200;
            //need modify
            double[,] coordinate = new double[10, 4];
            double[,] cassettezaxis = new double[2, 6];//0是cassetteA,1是cassetteB
            int[,] scheduleing = new int[100, 2];
            const int pointsnum = 30;
            AngleList[] go = new AngleList[10];

            //initialize coordinate
            RoutPlanning.Initialize(coordinate, distance, ratio);
            RoutPlanning.checkcassette(cassettezaxis);
            //scheduleing
            Scheduler.ScheduleFunction(scheduleing);


            //roudplaining
            int i = 0;
            for (i = 0; i < 10; i++)
            {
                //Program.form.mesPrintln(string.Format(" 1axis:{0:f} 2axis:{1:f} 3axis:{2:f} 4axis:{3:f} \n", coordinate[i, 0], coordinate[i, 1], coordinate[i, 2], coordinate[i, 3]));
                go[i] = RoutPlanning.routplanning(coordinate[i, 0], coordinate[i, 1], coordinate[i, 2], coordinate[i, 3], distance+20, pointsnum);
            }


            //printf("fuck %LF %LF\n", go[0]->one, go[0]->nextangle->one);

            //end plaining 

            //Nccode generator

            NCGen.generator(go);

            Thread.Sleep(500);

            mesPrintln("NCGen: NC Code generation is done.");


            //Thread startcontrol = new Thread();
            
            ThreadsController.addThreadAndStartByFunc(() =>
            {
              ControllerForm.controlwhile(scheduleing, coordinate, cassettezaxis);
            });
            

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


        private static void controlwhile(int[,] scheduleing, double[,] coordinate, double[,] cassettezaxis)
        {
            int WaferonHand = 0, WaferinCassettA = 6, WaferinCassettB = 0;
            int[] WaferonChamber = new int[10];//1 for A,2 for B,3 for D,7 for C,8 for E,9 for F
            int step = 0, i = 0;
            double oversignal = 0;
            char[] correspondChambername = new char[10];
            int director = 0,predirector=1;

            Program.form.mesPrintln(string.Format("fuck on {0:d}", scheduleing[5, 0]));

            correspondChambername[1] = 'A';
            correspondChambername[2] = 'B';
            correspondChambername[3] = 'D';
            correspondChambername[7] = 'C';
            correspondChambername[8] = 'E';
            correspondChambername[9] = 'F';

            for (i = 0; i < 10; i++)
            {
                WaferonChamber[i] = 0;
            }
            SyntecClient.writeGVar(5, 0);
            SyntecClient.writeGVar(6, 0);
            //與server交握
            while (scheduleing[step, 0] != 0)
            {
                if (scheduleing[step, 0]==1 || scheduleing[step, 0]==2 || scheduleing[step, 0] ==7 || scheduleing[step, 0] == 8 || scheduleing[step, 0] == 3)
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
                SyntecClient.writeGVar(2, 0); //設定@2，抓為0，亦即吸盤吸

                if (scheduleing[step, 0] != 4)
                {
                    SyntecClient.writeGVar(3, coordinate[scheduleing[step, 0] - 1, 1] - 8);//設定@3,Z軸伸長時高度(coordinate[scheduleing[step,0],1])
                    SyntecClient.writeGVar(4, coordinate[scheduleing[step, 0] - 1, 1] + 8);//設定@4,Z軸收回時高度(coordinate[scheduleing[step,0],1])
                }
                else
                {
                    SyntecClient.writeGVar(3, cassettezaxis[0, WaferinCassettA - 1] - 8);//設定@3,Z軸伸長至cassetteA時高度
                    SyntecClient.writeGVar(4, cassettezaxis[0, WaferinCassettA - 1] + 2.4);//設定@4,Z軸收回時高度
                }

                SyntecClient.writeGVar(1, scheduleing[step,0]);//設定@1為scheduleing[i,0]
                //控制器進行第一動作
                Thread.Sleep(3000);
                Program.form.mesPrintln("write 5 1");
                SyntecClient.writeGVar(5, 1);//發送動作許可請求，接受後寫@5為1
                //控制器進行接下動作


                Program.form.mesPrintln(String.Format("Wait for grab {0:d}", scheduleing[step, 0]));

                oversignal = SyntecClient.readReg(50);
                while (oversignal == 0)
                {//while接收控制器回傳動作結束@11=1;
                    Thread.Sleep(500);
                    oversignal = SyntecClient.readReg(50);
                }
                //Program.form.mesPrintln("hihihi");
                if (scheduleing[step, 0] == 4)
                {
                    WaferonHand = WaferinCassettA;
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

                //發送動作許可請求
                SyntecClient.writeGVar(2, 1);//設定@2，放為1，意即吸盤不吸
                if (scheduleing[step, 1] != 5)
                {
                    SyntecClient.writeGVar(3, coordinate[scheduleing[step, 1] - 1, 1] + 12);//設定@3,Z軸伸長放時高度(coordinate[scheduleing[step,1],2])
                    SyntecClient.writeGVar(4, coordinate[scheduleing[step, 1] - 1, 1] - 8);//設定@4,Z軸縮回時高度(coordinate[scheduleing[step,1],2])
                }
                else
                {
                    SyntecClient.writeGVar(3, cassettezaxis[1, WaferinCassettB] + 2.4);//設定@3,Z軸伸長至cassetteB放時高度
                    SyntecClient.writeGVar(4, cassettezaxis[1, WaferinCassettB] - 8);//設定@4,Z軸縮回時高度
                }
                SyntecClient.writeGVar(1, scheduleing[step, 1]);//設定@1為scheduleing[i,1]
                                                                //控制器進行動作
                Program.form.mesPrintln(String.Format("Wait for put {0:d}", scheduleing[step, 1]));
                //控制器進行第一動作
                Thread.Sleep(3000);
                Program.form.mesPrintln("write 5 1");
                SyntecClient.writeGVar(5, 1);//發送動作許可請求，接受後寫@5為1
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
                }
                else
                {
                    if (WaferonChamber[scheduleing[step, 1]] != 0)
                    {
                        Program.form.showWarnning(string.Format("there is Wafer on {0:d}", scheduleing[step, 1]));
                    }
                    WaferonChamber[scheduleing[step, 1]] = WaferonHand;
                    WaferonHand = 0;
                }
                SyntecClient.writeReg(50, 0);//@11=0
                                             //發送執行結束許可

                step = step + 1;
                Thread.Sleep(500);

            }
            Program.form.showWarnning("mission end");
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
            mesPrintln("Starting TRC communication...");
            ThreadsController.addThreadAndStartByFunc(TRCClient.communicate);

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

        private void bt_movForward_Click(object sender, EventArgs e)
        {
            if (!SyntecClient.isBusy())
                return;
            ThreadsController.addThreadAndStartByFunc(() =>
            {

                if (checkBox_precise.Checked)
                    linearMOV(1, 2);
                else
                    linearMOV(20, 2);
            });
        }

        private void bt_startInit_Click(object sender, EventArgs e)
        {
            ThreadsController.addThreadAndStartByFunc(() =>
            {
                SyntecClient.writeReg(25, 0);//cancle init mode
                SyntecClient.writeReg(17, 150);//JOG Speed

                SyntecClient.cycleReset();

                mesPrintln("Initializer: adjusting the angle of each axis");

                //C1
                while (SyntecClient.Pos[0] < 0)
                {
                    mesPrintln("Initializer: JOG C1...");
                    SyntecClient.JOG(1, SyntecClient.JOGMode.POSITIVE);
                    Thread.Sleep(300);
                }

                SyntecClient.JOG(1, SyntecClient.JOGMode.STOP);

                mesPrintln("Initializer: C1 OK!");

                //C2
                while (SyntecClient.Pos[1] < 0)
                {
                    mesPrintln("Initializer: JOG C2...");
                    SyntecClient.JOG(2, SyntecClient.JOGMode.POSITIVE);
                    Thread.Sleep(300);
                }

                SyntecClient.JOG(2, SyntecClient.JOGMode.STOP);

                mesPrintln("Initializer: C2 OK!");

                //C3
                while (SyntecClient.Pos[2] < 0)
                {
                    mesPrintln("Initializer: JOG C3...");
                    SyntecClient.JOG(3, SyntecClient.JOGMode.POSITIVE);
                    Thread.Sleep(300);
                }

                SyntecClient.JOG(3, SyntecClient.JOGMode.STOP);

                mesPrintln("Initializer: C3 OK!");

                //C4
                while (SyntecClient.Pos[3] < 0)
                {
                    mesPrintln("Initializer: JOG C4...");
                    SyntecClient.JOG(4, SyntecClient.JOGMode.POSITIVE);
                    Thread.Sleep(300);
                }

                SyntecClient.JOG(4, SyntecClient.JOGMode.STOP);

                mesPrintln("Initializer: C4 OK!");


                mesPrintln("Initializer: Starting initializer...");

                runInitAndWait(1);



                //origin finder

                double[] org = new double[4];
                double[] tmpPos = new double[2];

                double[] pos;


                SyntecClient.writeReg(17, 20);//JOG Speed

                //init C3
                mesPrintln("Initializer: Init C3...");

                SyntecClient.JOG(3, SyntecClient.JOGMode.POSITIVE);

                while (SyntecClient.readIBit(333))
                    Thread.Sleep(500);

                SyntecClient.JOG(3, SyntecClient.JOGMode.STOP);


                SyntecClient.writeReg(25, 1);

                SyntecClient.JOG(3, SyntecClient.JOGMode.NEGATIVE);

                while (!SyntecClient.readIBit(333))
                    Thread.Sleep(500);

                SyntecClient.JOG(3, SyntecClient.JOGMode.STOP);

                SyntecClient.getPos(out pos);

                org[2] = pos[2];

                //init C1
                mesPrintln("Initializer: Init C1...");

                SyntecClient.JOG(1, SyntecClient.JOGMode.NEGATIVE);

                while (!SyntecClient.readIBit(331))
                    Thread.Sleep(500);

                SyntecClient.JOG(1, SyntecClient.JOGMode.STOP);

                SyntecClient.getPos(out pos);
                tmpPos[0] = pos[0];

                SyntecClient.getPos(out pos);
                tmpPos[1] = pos[0];

                org[0] = (tmpPos[0] + tmpPos[1]) / 2;

                /*
                //init C2
                mesPrintln("Initializer: Init C2...");

                SyntecClient.JOG(2, SyntecClient.JOGMode.NEGATIVE);

                while (!SyntecClient.readIBit(332))
                    Thread.Sleep(500);

                SyntecClient.JOG(2, SyntecClient.JOGMode.STOP);

                SyntecClient.getPos(out pos);
                tmpPos[0] = pos[1];

                SyntecClient.getPos(out pos);
                tmpPos[1] = pos[1];

                org[1] = (tmpPos[0] + tmpPos[1]) / 2;
                */

                SyntecClient.writeReg(25, 0);

                SyntecClient.writeGVar(1010, 10);
                SyntecClient.writeGVar(1020, 10);
                SyntecClient.writeGVar(1040, 10);


                initRunning = false;
                mesPrintln("Initializer: Done!");

            });
        }
        private void runInitAndWait(int step)
        {
            SyntecClient.cycleReset();

            NCGen.genInitNC(step);

            SyntecClient.uploadNCFile(SyntecClient.NCFileName.INITIALIZER);

            SyntecClient.cycleStart();
            initRunning = true;

            while (!SyntecClient.isBusy())
                Thread.Sleep(800);

            while (!SyntecClient.isIdle())
                Thread.Sleep(800);

            Thread.Sleep(800);
        }

        private void bt_thetaP_Click(object sender, EventArgs e)
        {
            if (!SyntecClient.isBusy())
                return;

            ThreadsController.addThreadAndStartByFunc(() =>
            {
                while (SyntecClient.readSingleVar(11) != 0)
                    Thread.Sleep(10);


                if (checkBox_precise.Checked)
                    SyntecClient.writeGVar(11, 5);
                else
                    SyntecClient.writeGVar(11, 2);
            });
        }

        private void bt_thetaN_Click(object sender, EventArgs e)
        {
            if (!SyntecClient.isBusy())
                return;

            ThreadsController.addThreadAndStartByFunc(() =>
            {


                while (SyntecClient.readSingleVar(11) != 0)
                    Thread.Sleep(10);


                if (checkBox_precise.Checked)
                    SyntecClient.writeGVar(11, 6);
                else
                    SyntecClient.writeGVar(11, 3);
            });
        }

        private void bt_test_Click(object sender, EventArgs e)
        {
            SyntecClient.writeReg(17, 150);//JOG Speed

        }

        private void bt_start_Click(object sender, EventArgs e)
        {
            //controlwhile(scheduleing, coordinate, cassettezaxis);
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
