using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;


using Syntec.Remote;


namespace PCController
{
    class SyntecClient
    {
        public const short SUCCESSFUL = (short)SyntecRemoteCNC.ErrorCode.NormalTermination;
        public const string DEFAULT_IP = "192.168.1.110";
        public const string DEFAULT_NCFILENAME = NCFileName.MAIN_JOB;

        public static string[] AxisName = null, Unit = null;
        public static float[] Mach = null, Abs = null, Rel = null, Dist = null;

        public static class ControlMode
        {
            public const int AUTO = 2;
            public const int JOG = 4;
            public const int SET_ORIGIN = 7;

        }

        public static class JOGMode
        {
            public const int STOP = 0;
            public const int POSITIVE = 1;
            public const int NEGATIVE = 2;

        }


        //
        /*public functions*/
        //

        public static void connect(string ip)
        {
            remoteIP = ip;

            disconnect();

            cnc = new SyntecRemoteCNC(remoteIP);
        }


        public static void disconnect()
        {
            if (cnc != null)
            {
                cnc.Close();
                cnc = null;
            }

            //Remove Unused Process
            Process[] process = Process.GetProcessesByName("SyntecRemoteServer");
            foreach (Process p in process)
            {
                p.Kill();
            }

        }


        public static bool isConnected()
        {
            refreshState();
            return connected;
        }
        public static bool isBusy()
        {
            refreshState();
            return busy;
        }

        public static bool isPaused()
        {
            refreshState();
            return paused;
        }

        public static short uploadNCFile(string ncSrc)
        {
            short result = cnc.UPLOAD_nc_mem(ncSrc);
            if (result != SUCCESSFUL)
                return result;

            //WaitForUploadFile
            while (!cnc.isFileUploadDone)
                Thread.Sleep(500);

            result = cnc.FileUploadErrorCode;
            if (result != SUCCESSFUL)
                return result;

            cnc.WRITE_nc_main(ncSrc);
            return result;

        }

        public static void cycleStart()
        {
            setControlMode(ControlMode.AUTO);

            writeReg(19, 1);
            Thread.Sleep(100);
            writeReg(19, 0);
        }

        public static void cycleStop()
        {
            writeReg(15206, 1);
            Thread.Sleep(100);
            writeReg(15206, 0);
        }

        public static void cycleReset()
        {
            writeReg(15207, 1);
            Thread.Sleep(100);
            writeReg(15207, 0);
        }


        public static void motorServoSwitch()
        {
            bool sw = readCBit(36) ? false : true;
            writeCBit(36, sw);

        }
        public static bool readCBit(int addr)
        {
            byte[] val;
            cnc.READ_plc_cbit(addr, addr, out val);

            if (val != null)
                return val[0] == 0 ? false : true;

            return false;
        }


        public static double readSingleVar(int no)
        {
            double var;
            cnc.READ_macro_single(no, out var);
            if (var != null) { 
                return var;
            }
            else
            {
                return 0;
            }
        }

        public static void writeGVar(int no,double val)
        {
            cnc.WRITE_macro_single(no, val);
            Program.form.mesPrintln(string.Format("SyntectClient: Write global var @{0:d} = {1:f3}", no, val));

        }


        public static void writeReg(int addr, int val)
        {
            cnc.WRITE_plc_register(addr, addr, new int[] { val });
        }

        public static void writeReg(int addrStart, int addrEnd, int[] vals)
        {
            cnc.WRITE_plc_register(addrStart, addrEnd, vals);
        }

        public static void writeCBit(int addr, bool val)
        {
            cnc.WRITE_plc_cbit(addr, addr, val ? new byte[] { 1 } : new byte[] { 0 });
        }

        public static int readReg(int addr)
        {
            int[] var;
            cnc.READ_plc_register(addr, addr, out var);
            if(var!=null)
            return var[0];

            return 0;

        }

        public static void writeOBit(int addr,bool val)
        {
            Program.form.mesPrintln("SyntecClient: Setting Obit... Please wait.");
            NCGen.genObitSetterNC(addr, val);
            uploadNCFile(NCFileName.OBIT_SETTER);
            cycleStart();

        }

        public static class NCFileName
        {
            public const string INITIALIZER = "initializer.txt";
            public const string MAIN_JOB = "mainjob.txt";
            public const string OBIT_SETTER = "obitsetter.txt";
        }

        public static void setControlMode(int mode)
        {
            writeReg(13, mode);
        }

        public static void setOrigin()
        {
            setControlMode(ControlMode.SET_ORIGIN);
            writeReg(15208, 7);

        }

        public static void JOG(int axis, int axisMode)
        {
            setControlMode(ControlMode.JOG);
            writeReg(20 + axis, axisMode);
        }

        public static void JOG(int axis1Mode, int axis2Mode, int axis3Mode, int axis4Mode)
        {
            setControlMode(ControlMode.JOG);
            writeReg(21, 24, new int[] { axis1Mode, axis2Mode, axis3Mode, axis4Mode });

        }

        public static void setPos()
        {
            if (cnc == null)
            {
                connected = false;
                return;
            }

            double[] pos = { 0, 0, 0, 0 };


            if (AxisName.Length > 0)
            {
                cnc.WRITE_relpos(AxisName[0], pos[0]);
                cnc.WRITE_relpos(AxisName[1], pos[1]);
                cnc.WRITE_relpos(AxisName[2], pos[2]);
                cnc.WRITE_relpos(AxisName[3], pos[3]);
            }
        }

            
        /*private*/
        //
        private static string remoteIP;
        private static SyntecRemoteCNC cnc = null;
        private static bool connected = false;
        private static bool busy = false;
        private static bool paused = false;


        private static void refreshState()
        {
            if (cnc == null)
            {
                connected = false;
                return;
            }

            if (!cnc.isConnected())
            {
                connected = false;
                return;
            }


            short DecPoint = 0;
            short result = cnc.READ_position(out AxisName, out DecPoint, out Unit, out Mach, out Abs, out Rel, out Dist);

            if (result == 0)
            {
                connected = true;

                if (AxisName.Length > 0)
                {
                    //label1.Text = AxisName[0] + " : " + Mach[0].ToString();
                }
                if (AxisName.Length > 1)
                {
                    //label2.Text = AxisName[1] + " : " + Mach[1].ToString();
                }
                if (AxisName.Length > 2)
                {
                    //label3.Text = AxisName[2] + " : " + Mach[2].ToString();
                }
                if (AxisName.Length > 3)
                {
                    //label4.Text = AxisName[3] + " : " + Mach[3].ToString();
                }
            }
            else
            {
                connected = false;
                //label1.Text = "Err : " + result.ToString();
                //label2.Text = "Err : " + result.ToString();
                //label3.Text = "Err : " + result.ToString();
                //label4.Text = "Err : " + result.ToString();
            }

            byte[] sbits = null;
            cnc.READ_plc_sbit(0, 1, out sbits);

            if (sbits != null)
            {
                busy = sbits[0] != 0 ? true : false;
                paused = sbits[1] != 0 ? true : false;
            }
        }


    }
}
