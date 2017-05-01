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
            ArmData.longrate2 = 0.5868852;
            ArmData.longrate3 = 1.154918032;

            mesPrintln("Ready...");

        }

        ~ControllerForm()
        {
            ThreadsController.abortAllThreads();
            TRCClient.disconnect();
            SyntecClient.disconnect();
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
        private bool messModified = false;
        private bool messLock = false;

        private string initBtText_TRCConnect;
        private string initBtText_SyntecConnect;

        /*
         * private functions
         * 
         */

        private void timer300ms_Tick(object sender, EventArgs e)
        {
            refreshMess();

            if (SyntecClient.isConnected())
            {
                bt_SyntecConnect.Text = "Connected";

                panelTestFunc.Enabled = true;
                panelJOG.Enabled = true;

                label_syntecBusy.Text = SyntecClient.isBusy() ? "Busy" : SyntecClient.isPaused() ? "Paused" : "Idle";

                if (SyntecClient.Rel != null)
                    label_pos.Text = string.Format("C1 = {0:f3}   C2 = {1:f3}   C3 = {2:f3}   C4 = {3:f3}", SyntecClient.Mach[0], SyntecClient.Mach[1], SyntecClient.Mach[2], SyntecClient.Mach[3]);
                else
                    label_pos.Text = "Unable to get position";

            }
            else
            {
                bt_SyntecConnect.Text = initBtText_SyntecConnect;
                bt_SyntecConnect.Enabled = true;

                panelTestFunc.Enabled = false;
                panelJOG.Enabled = false;

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

        private void linearMOV()
        {
            if (SyntecClient.readSingleVar(11) == 1)
                return;


            AngleList linearAngleList = RoutPlanning.routplanning(SyntecClient.Mach[0], SyntecClient.Mach[2], SyntecClient.Mach[1], SyntecClient.Mach[3], 50, 10);

            Angle currNode = linearAngleList.headAngle;

            int i = 0;
            while (currNode != null)
            {
                SyntecClient.writeGVar(100 + i++, (double)currNode.one);
                SyntecClient.writeGVar(100 + i++, (double)currNode.three);
                SyntecClient.writeGVar(100 + i++, (double)currNode.two);
                SyntecClient.writeGVar(100 + i++, (double)currNode.four);

                currNode = currNode.nextangle;
            }

            SyntecClient.writeGVar(11, 1);
        }


        private void auto()
        {
            const double ratio = 780;

            //need modify
 
            const double distance = 180;
            //need modify

            const int pointsnum = 50;



            double[,] coordinate = new double[10, 4];
            int[,] scheduleing = new int[100, 2];
            AngleList[] go = new AngleList[10];

            //initialize coordinate
            RoutPlanning.Initialize(coordinate, distance, ratio);

            //scheduleing
            Scheduler.ScheduleFunction(scheduleing);


            //roudplaining
            int i = 0;
            for (i = 0; i < 10; i++)
            {
                //Program.form.mesPrintln(string.Format(" 1axis:{0:f} 2axis:{1:f} 3axis:{2:f} 4axis:{3:f} \n", coordinate[i, 0], coordinate[i, 1], coordinate[i, 2], coordinate[i, 3]));
                go[i] = RoutPlanning.routplanning(coordinate[i, 0], coordinate[i, 1], coordinate[i, 2], coordinate[i, 3], distance, pointsnum);
            }

            //printf("fuck %LF %LF\n", go[0]->one, go[0]->nextangle->one);

            //end plaining 

            //Nccode generator
            NCGen.generator(go, scheduleing);

            Thread.Sleep(1000);
            mesPrintln("NCGen: NC Code generation is done.");


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
            TRCClient.connect(tB_TRCIP.Text,Convert.ToInt32(tB_TRCPort.Text));

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


        private void bt_test_Click(object sender, EventArgs e)
        {
            linearMOV();


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
            auto();
            NCGen.genInitNC();

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

        private const int MAX_COUNT = 10;
        private static int threadsCount = 0;
        private static Thread[] threads = null;

    }
}
