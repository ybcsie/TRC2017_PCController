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


            bt_upload.Enabled = false;
            bt_cycStart.Enabled = false;
            bt_writeGVar.Enabled = false;
            bt_readGVar.Enabled = false;

            //test
            //bt_test.Enabled = false;

            timer300ms = new System.Windows.Forms.Timer();
            timer300ms.Interval = 300;
            timer300ms.Tick += new EventHandler(timer300ms_Tick);
            timer300ms.Enabled = true;

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
                bt_upload.Enabled = true;
                bt_cycStart.Enabled = true;
                bt_writeGVar.Enabled = true;
                bt_readGVar.Enabled = true;

                label_syntecBusy.Text = SyntecClient.isBusy() ? "Busy" : "Idle";

                label_pos.Text = SyntecClient.Rel[0].ToString() + " " + SyntecClient.Rel[1].ToString() + " " + SyntecClient.Rel[2].ToString() + " " + SyntecClient.Rel[3].ToString();

            }
            else
            {
                bt_SyntecConnect.Text = initBtText_SyntecConnect;
                bt_SyntecConnect.Enabled = true;
                bt_upload.Enabled = false;
                bt_cycStart.Enabled = false;
                bt_writeGVar.Enabled = false;
                bt_readGVar.Enabled = false;
            }


            if(TRCClient.isConnected())
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
            const double armbaselong = 320;
            const double arm2rate = 0.53125;
            const double arm3rate = 0.9375;

            //need modify
            const double tmp1 = 28.1748;
            const double tmp2 = 89.1047;
            const double tmp3 = 62.72038;
            const double distance = 130;
            //need modify

            const int pointsnum = 10;



            int[,] coordinate = new int[10, 3];
            int[,] scheduleing = new int[100, 2];
            AngleList[] go = new AngleList[10];

            //initialize coordinate
            RoutPlanning.Initialize(coordinate);

            //scheduleing
            Scheduler.ScheduleFunction(scheduleing);


            //roudplaining
            go[0] = RoutPlanning.routplanning(armbaselong, arm2rate, arm3rate, coordinate[0, 1] + tmp1, coordinate[0, 2], tmp2, 170 - tmp3, distance, pointsnum);
            go[1] = RoutPlanning.routplanning(armbaselong, arm2rate, arm3rate, coordinate[1, 1] + tmp1, coordinate[1, 2], tmp2, 170 - tmp3, distance, pointsnum);
            go[2] = RoutPlanning.routplanning(armbaselong, arm2rate, arm3rate, coordinate[2, 1] + tmp1, coordinate[2, 2], tmp2, 170 - tmp3, distance, pointsnum);
            go[3] = RoutPlanning.routplanning(armbaselong, arm2rate, arm3rate, coordinate[3, 1] + tmp1, coordinate[3, 2], tmp2, 170 - tmp3, distance, pointsnum);
            go[4] = RoutPlanning.routplanning(armbaselong, arm2rate, arm3rate, coordinate[4, 1] + tmp1, coordinate[4, 2], tmp2, 170 - tmp3, distance, pointsnum);
            go[5] = RoutPlanning.routplanning(armbaselong, arm2rate, arm3rate, coordinate[5, 1] + tmp1, coordinate[5, 2], tmp2, 170 - tmp3, distance, pointsnum);
            go[6] = RoutPlanning.routplanning(armbaselong, arm2rate, arm3rate, coordinate[6, 1] + tmp1, coordinate[6, 2], tmp2, 170 - tmp3, distance, pointsnum);
            go[7] = RoutPlanning.routplanning(armbaselong, arm2rate, arm3rate, coordinate[7, 1] + tmp1, coordinate[7, 2], tmp2, 170 - tmp3, distance, pointsnum);
            go[8] = RoutPlanning.routplanning(armbaselong, arm2rate, arm3rate, coordinate[8, 1] + tmp1, coordinate[8, 2], tmp2, 170 - tmp3, distance, pointsnum);
            go[9] = RoutPlanning.routplanning(armbaselong, arm2rate, arm3rate, coordinate[9, 1] + tmp1, coordinate[9, 2], tmp2, 170 - tmp3, distance, pointsnum);

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
            tB_mesPrint.Text = messOut;
            tB_mesPrint.SelectionStart = tB_mesPrint.TextLength;
            tB_mesPrint.ScrollToCaret();
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
            SyntecClient.cycleStart();
            Thread.Sleep(300);
            setState("Cycle started!");

        }


        private void bt_test_Click(object sender, EventArgs e)
        {
            /*
            syntec.writeSingleVar(10, 0);
            setState(syntec.readSingleVar(10).ToString());
            */
            //syntec.setPos();

            /*
            ThreadsController.addThreadAndStartByFunc(TRCClient.connect);
            Thread.Sleep(200);
            ThreadsController.addThreadAndStartByFunc(TRCClient.communicate);
            */

            SyntecClient.disconnect();

        }

        private void bt_writeGVar_Click(object sender, EventArgs e)
        {
            int no = Convert.ToInt32(num_gvarNo.Value);
            double val = Convert.ToDouble(num_gvarValue.Value);

            SyntecClient.writeSingleVar(no, val);

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

        }

        private void bt_TRCCommStart_Click(object sender, EventArgs e)
        {
            mesPrintln("Starting TRC communication...");
            ThreadsController.addThreadAndStartByFunc(TRCClient.communicate);

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
