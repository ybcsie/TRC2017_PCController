using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PCController
{
    class TRCClient
    {
        public const string DEFAULT_IP = "127.0.0.1";
        public const int DEFAULT_PORT = 5001;

        private static string address;
        private static int port;

        private static TcpClient tcpClient = null;
        private static StreamReader tcpReader = null;
        private static StreamWriter tcpWriter = null;

        public static int src, dst;
        //  public static int[] step = new int[3];
        //  public static int[] time = new int[3];
        public static string cassNo = "";
        public static int[,] record_stage = new int[6, 3];
        public static int[,] record_time = new int[6, 3];
        public static int[] record_wafer = new int[6];
        public static int now = 0;

        public static bool isConnected()
        {
            if (tcpClient == null)
                return false;

            return tcpClient.Connected;
        }

        public static void connect(string ip_in, int port_in)
        {
            address = ip_in;
            port = port_in;

            disconnect();

            try
            {
                tcpClient = new TcpClient(address, port);
                NetworkStream stream = tcpClient.GetStream();
                tcpReader = new StreamReader(stream);
                tcpWriter = new StreamWriter(stream);

            }
            catch
            {
                mesPrintln("TRCClient: Cannnot connect to TRC server.");
            }


        }

        public static void disconnect()
        {

            if (tcpReader != null)
            {
                tcpReader.Close();
                tcpReader = null;
            }

            if (tcpWriter != null)
            {
                tcpWriter.Close();
                tcpWriter = null;
            }

            if (tcpClient != null)
            {
                tcpClient.Close();
                tcpClient = null;
            }

        }


        private static string getCmd()
        {
            if (!isConnected())
            {
                connectionErrorHandler();
                return null;
            }

            char[] buf = new char[100];
            int count = 0;

            try
            {
                count = tcpReader.Read(buf, 0, 100);
            }
            catch
            {
            }

            return new string(buf, 0, count);

        }

        private static int sentCmd(string str)
        {
            if (!isConnected())
            {
                connectionErrorHandler();
                return -1;
            }
            tcpWriter.WriteLine(str);
            tcpWriter.Flush();
            return 0;
        }

        public static void handShake()
        {
            string command, answer = "~Ack";
            string[] para, para2;


            // first handShake
            command = getCmd();
            mesPrintln(command);
            para = command.Split(',');
            para2 = para[3].Split(';');

            // initial src, dst, cass number, & steps
            src = (para2[0].ElementAt(6) - '0') * 10
                    + (para2[0].ElementAt(7) - '0');
            record_wafer[now] = src;
            dst = (para2[4].ElementAt(6) - '0') * 10
                    + (para2[4].ElementAt(7) - '0');
            cassNo += (src / 10);
            cassNo += (src % 10);

            for (int i = 0; i < 3; i++)
            {
                record_stage[now, i] = para2[i + 1].ElementAt(0) - 'A' + 1;
            }
            for (int i = 0; i < 3; i++)
            {
                record_time[now, i] = para2[i + 1].ElementAt(2) - '0';
                if (para2[i + 1].Length == 4)
                {
                    record_time[now, i] *= 10;
                    record_time[now, i] += para2[i + 1].ElementAt(3) - '0';
                }

            }

            for (int i = 1; i < para.Length; i++)
                answer += "," + para[i];

            mesPrintln(answer);
            Thread.Sleep(1000);
            sentCmd(answer);

            // second handShake
            command = getCmd();
            mesPrintln(command);
            para = command.Split(',');
            answer = "~Ack";
            for (int i = 1; i < para.Length; i++)
                answer += "," + para[i];
            mesPrintln(answer);
            Thread.Sleep(1000);
            sentCmd(answer);
        }

        public static void sentEvent(int opCode, int location)
        {
            string[] op = { "GetWaferStart", "GetWaferCompleted",
                            "PutWaferStart", "PutWaferCompleted" };
            string evt = "~Evt,";
            string cass = "W" + cassNo;
            string[] loc = { "CassA", "A", "B", "C", "D", "E", "F", "CassB" };
            string target;

            target = loc[location];
            if (location == 0) target += "-" + src / 10 + src % 10;
            else if (location == 7) target += "-" + dst / 10 + dst % 10;
            evt = evt + op[opCode] + "," + cass + "," + target + "@";
            sentCmd(evt);
            getResponse();
        }

        public static void getResponse()
        {
            string command;
            string[] para;

            command = getCmd();
            mesPrintln(command);
            para = command.Split(',');
            string reject = para.ElementAt(para.Length - 1);
            if (para.Length == 4) mesPrintln("Completed");
            else if (reject.Equals("0@")) mesPrintln("Accept");
            else mesPrintln("Reject");
            Thread.Sleep(1000);
        }

        public static void communicate()
        {
            if (!isConnected())
            {
                connectionErrorHandler();
                return;
            }

            handShake();
            handShake();
            handShake();
            handShake();
            handShake();
            handShake();

            /* 0 : GET_START
             * 1 : GET_COMP
             * 2 : PUT_START
             * 3 : PUT_COMP */

            // sentEvent(OPCODE, STAGE);   
            // getStageTime(STAGE,WAFER_NUM);  

            tcpWriter.Close();
            tcpReader.Close();
        }


        public int getStageTime(int stage, int wafer)
        {
            int i;
            for (i = 0; i < 6; i++)
                if (record_wafer[i] == wafer) break;
            return record_time[i, stage];
        }


        private static void connectionErrorHandler()
        {
            Program.form.showWarnning("TRCClient: TRC server is not connected");
        }

        private static void mesPrint(string str)
        {
            if (Program.form != null)
                Program.form.mesPrint(str);
        }
        private static void mesPrintln(string str)
        {
            if (Program.form != null)
                Program.form.mesPrintln(str);
        }

    }
}