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


        public static int state;
        public static string[] AxisName = null, Unit = null;
        public static float[] Mach = null, Abs = null, Rel = null, Dist = null;

        public static double[] Pos = null;


        public static class NCFileName
        {
            public const string INITIALIZER = "initializer.txt";
            public const string MAIN_JOB = "mainjob.txt";
            public const string OBIT_SETTER = "obitsetter.txt";
        }


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
        public static class States
        {
            public const int ERROR = -1;
            public const int IDLE = 0;
            public const int BUSY = 1;
            public const int PAUSED = 2;

            public static string getStateStr(int state)
            {
                string[] stateStr = { "ERROR", "Idle", "Busy", "Paused" };
                return stateStr[state + 1];
            }
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
            getSate();
            return connected;
        }

        public static bool isBusy()
        {
            getSate();
            return state == States.BUSY;
        }

        public static bool isIdle()
        {
            getSate();
            return state == States.IDLE;
        }


        public static short uploadNCFile(string ncSrc)
        {
            short result = cnc.UPLOAD_nc_mem(ncSrc);
            if (result != SUCCESSFUL)
                return result;

            int timeOut = 25;
            //WaitForUploadFile
            while (!cnc.isFileUploadDone && timeOut-- > 0)
                Thread.Sleep(200);

            result = cnc.FileUploadErrorCode;
            if (result != SUCCESSFUL|| timeOut < 0)
                return result;

            cnc.WRITE_nc_main(ncSrc);
            return result;

        }

        public static void cycleStart()
        {
            setJOGSpeed(150);

            setControlMode(ControlMode.AUTO);

            writeReg(19, 1);
            Thread.Sleep(50);
            writeReg(19, 0);
        }

        public static void cycleStop()
        {
            writeReg(15206, 1);
            Thread.Sleep(50);
            writeReg(15206, 0);
        }

        public static void cycleReset()
        {
            writeReg(15207, 1);
            Thread.Sleep(50);
            writeReg(15207, 0);
        }


        public static void motorServoSwitch()
        {
            bool sw = readCBit(36) ? false : true;
            writeCBit(36, sw);

        }

        public static bool readIBit(int addr)
        {
            byte[] val;
            cnc.READ_plc_ibit(addr, addr, out val);

            if (val != null)
                return val[0] == 0 ? false : true;

            return false;
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

            try
            {
                cnc.READ_macro_single(no, out var);
            }
            catch
            {
                var = 1;
            }

            return var;
        }

        public static void writeGVar(int no,double val)
        {
            cnc.WRITE_macro_single(no, val);
            //Program.form.mesPrintln(string.Format("SyntectClient: Write global var @{0:d} = {1:f3}", no, val));
            Thread.Sleep(20);

        }


        public static void writeReg(int addr, int val)
        {
            cnc.WRITE_plc_register(addr, addr, new int[] { val });
            Thread.Sleep(20);
        }

        public static void writeReg(int addrStart, int addrEnd, int[] vals)
        {
            cnc.WRITE_plc_register(addrStart, addrEnd, vals);
            Thread.Sleep(20);
        }

        public static void writeCBit(int addr, bool val)
        {
            cnc.WRITE_plc_cbit(addr, addr, val ? new byte[] { 1 } : new byte[] { 0 });
        }


        public static void writeIBit(int addr, bool val)
        {
            cnc.WRITE_plc_ibit(addr, addr, val ? new byte[] { 1 } : new byte[] { 0 });
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


        public static void setControlMode(int mode)
        {
            writeReg(13, mode);
            Thread.Sleep(50);
        }

        public static void setOrigin()
        {
            setOrigin(7);
        }

        public static void setOrigin(int axis)
        {
            setControlMode(ControlMode.SET_ORIGIN);
            writeReg(15208, axis);
            Thread.Sleep(50);
            writeReg(15208, 0);
        }

        public static void setJOGSpeed(int percent)
        {
            writeReg(17, percent);

        }

        public static void JOG(int axis, int mode)
        {
            setControlMode(ControlMode.JOG);
            writeReg(20 + axis, mode);
        }

        public static void JOG(int axis1Mode, int axis2Mode, int axis3Mode, int axis4Mode)
        {
            setControlMode(ControlMode.JOG);
            writeReg(21, 24, new int[] { axis1Mode, axis2Mode, axis3Mode, axis4Mode });

        }

        /*
        public static void JOGOver(int axis, double angle)
        {
            ThreadsController.addThreadAndStartByFunc(() =>
            {
                cycleReset();

                double[] curPos;
                getPos(out curPos);

                if (angle > curPos[axis - 1])
                    while (curPos[axis - 1] < angle)
                    {
                        JOG(axis, JOGMode.POSITIVE);
                        Thread.Sleep(300);
                        getPos(out curPos);
                    }
                else
                    while (curPos[axis - 1] > angle)
                    {
                        JOG(axis, JOGMode.NEGATIVE);
                        Thread.Sleep(300);
                        getPos(out curPos);
                    }

                JOG(axis, JOGMode.STOP);


            });
        }
        */


        public static void getPos(out double[] Pos_in)
        {
            Pos_in = Pos;

            float[] tmpM;

            getMach(out tmpM);

            if (tmpM == null)
                return;

            //translate to + - form
            Pos_in = new double[tmpM.Length];

            for (int i = 0; i < tmpM.Length; i++)
            {
                if (tmpM[i] > 180)
                    Pos_in[i] = tmpM[i] - 360;
                else
                    Pos_in[i] = tmpM[i];
            }

            return;
        }


        public static string stateStr()
        {
            return States.getStateStr(state);
        }


        public static void refresh()
        {
            getSate();
            getPos(out Pos);
        }


        /*private*/
        //

        private static string remoteIP;
        private static SyntecRemoteCNC cnc = null;
        private static bool connected = false;

        private static void getSate()
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

            connected = true;


            byte[] sbits = null;
            cnc.READ_plc_sbit(0, 1, out sbits);

            if (sbits != null)
            {
                state = sbits[0] == 0 ? States.IDLE : sbits[1] == 0 ? States.BUSY : States.PAUSED;
            }
            else
                state = States.ERROR;
        }


        private static void getMach(out float[] Mach_in)
        {
            Mach_in = Mach;

            if (!connected)
                return;

            float[] tmpM;
            short DecPoint = 0;
            short result = -1;
            int retryCounter = 5;

            result = cnc.READ_position(out AxisName, out DecPoint, out Unit, out tmpM, out Abs, out Rel, out Dist);
            while (retryCounter-- > 0 && result != SUCCESSFUL)
            {
                Thread.Sleep(5);
                result = cnc.READ_position(out AxisName, out DecPoint, out Unit, out tmpM, out Abs, out Rel, out Dist);
            }

            if (result == SUCCESSFUL)
            {
                Mach = new float[tmpM.Length];
                Mach_in = new float[tmpM.Length];

                tmpM.CopyTo(Mach, 0);
                tmpM.CopyTo(Mach_in, 0);

                return;
            }

            state = States.ERROR;

        }

    }
}
