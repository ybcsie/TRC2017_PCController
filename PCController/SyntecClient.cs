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
        public const string DEFAULT_NCFILENAME = "mainjob.txt";

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
            cnc.WRITE_plc_register(19, 19, new int[1] { 1 });
            Thread.Sleep(300);
            cnc.WRITE_plc_register(19, 19, new int[1] { 0 });
        }

        public static double readSingleVar(int no)
        {
            double var;
            cnc.READ_macro_single(no, out var);

            return var;
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

        public static void writeCBit(int addr,bool val)
        {
            cnc.WRITE_plc_cbit(addr, addr, val ? new byte[] { 1 } : new byte[] { 0 });
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
            cnc.READ_plc_sbit(0, 0, out sbits);

            if (sbits != null)
                busy = sbits[0] != 0 ? true : false;
        }


    }
}
