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
        public static int[] record_dst = new int[6];
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
            string[] para, para2, para3;


            // first handShake
            command = getCmd();
            mesPrintln(command);
            para = command.Split(',');
            para2 = para[3].Split(';');
            para3 = para[3].Split(':');

            // initial src, dst, cass number, & steps
            src = (para2[0].ElementAt(6) - '0') * 10
                    + (para2[0].ElementAt(7) - '0');
            record_wafer[now] = src;
            dst = (para2[4].ElementAt(6) - '0') * 10
                    + (para2[4].ElementAt(7) - '0');
            record_dst[now] = dst;
            cassNo += (src / 10);
            cassNo += (src % 10);

            for (int i = 0; i < 3; i++)
            {
                record_stage[now, i] = para2[i + 1].ElementAt(0) - 'A' + 1;
                if (para2[i + 1].ElementAt(2) - 'A' + 1 > 0)
                {
                    record_stage[now, i] = record_stage[now, i] * 10;
                    record_stage[now, i] = record_stage[now, i] + para2[i + 1].ElementAt(2) - 'A' + 1;
                    if (para2[i + 1].ElementAt(4) - 'A' + 1 > 0)
                    {
                        record_stage[now, i] = record_stage[now, i] * 10;
                        record_stage[now, i] = record_stage[now, i] + para2[i + 1].ElementAt(4) - 'A' + 1;
                    }
                }
            }


            for (int i = 0; i < 3; i++)
            {
                record_time[now, i] = para3[i + 1].ElementAt(0) - '0';
                if (para3[i + 1].ElementAt(1) <= '9')
                {
                    record_time[now, i] *= 10;
                    record_time[now, i] += para3[i + 1].ElementAt(1) - '0';
                }
            }
            //Program.form.mesPrintln(string.Format(" stage{0:d} = {1:d} {2:d} {3:d} time:{4:d} {5:d} {6:d}", now, record_stage[now, 0], record_stage[now, 1], record_stage[now, 2], record_time[now, 0], record_time[now, 1], record_time[now, 2]));
            now++;

            for (int i = 1; i < para.Length; i++)
                answer += "," + para[i];

            mesPrintln(answer);
            Thread.Sleep(500);
            sentCmd(answer);

            // second handShake

        }

        public static void handShake1()
        {
            string command, answer = "~Ack";
            string[] para, para2, para3;


            // first handShake
            command = getCmd();
            mesPrintln(command);
            para = command.Split(',');
            para2 = para[3].Split(';');
            para3 = para[3].Split(':');

            // initial src, dst, cass number, & steps
            src = (para2[0].ElementAt(6) - '0') * 10
                    + (para2[0].ElementAt(7) - '0');
            record_wafer[0] = src;

            for (int i = 0; i < 3; i++)
            {
                record_time[0, i] = para3[i + 1].ElementAt(0) - '0';
                if (para3[i + 1].ElementAt(1) <= '9')
                {
                    record_time[0, i] *= 10;
                    record_time[0, i] += para3[i + 1].ElementAt(1) - '0';
                }
            }


            for (int i = 1; i < para.Length; i++)
                answer += "," + para[i];

            mesPrintln(answer);
            Thread.Sleep(500);
            sentCmd(answer);

            // second handShake
        }
        public static void prostart()
        {
            string command, answer = "~Ack";
            string[] para, para2;
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
        public static void end()
        {
            sentCmd("~Evt,ProcessCompleted@");

        }

        public static int sentEvent(int opCode, int location, int waferNum, int cassNum)
        {
            int accept = 0;
            string[] op = { "GetWaferStart", "GetWaferCompleted",
                            "PutWaferStart", "PutWaferCompleted" };
            string evt = "~Evt,";
            string wafer = "W";
            string[] loc = { "CassA", "A", "B", "C", "D", "E", "F", "CassB" };
            string target;
            wafer += waferNum / 10;
            wafer += waferNum % 10;

            target = loc[location];
            if (location == 0) target += "-" + cassNum / 10 + cassNum % 10;
            else if (location == 7) target += "-" + cassNum / 10 + cassNum % 10;
            evt = evt + op[opCode] + "," + wafer + "," + target + "@";
            sentCmd(evt);
            mesPrintln(evt);
            accept = getResponse();
            return accept;
        }

        public static int getResponse()
        {
            string command;
            string[] para;

            command = getCmd();
            mesPrintln(command);
            para = command.Split(',');
            string reject = para.ElementAt(para.Length - 1);
            if (para.Length == 4) mesPrintln("Completed");
            else if (reject.Equals("0@"))
            {
                mesPrintln("Accept");
                return 1;
            }
            else mesPrintln("Reject");
            return 0;
            // Thread.Sleep(1000);
        }

        public static void init()
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
            prostart();

            /* 0 : GET_START
             * 1 : GET_COMP
             * 2 : PUT_START
             * 3 : PUT_COMP */

            // sentEvent(OPCODE, STAGE);   
            // getStageTime(STAGE,WAFER_NUM);  
        }

        public static void init1()
        {
            if (!isConnected())
            {
                connectionErrorHandler();
                return;
            }

            handShake1();
            prostart();

            /* 0 : GET_START
             * 1 : GET_COMP
             * 2 : PUT_START
             * 3 : PUT_COMP */

            // sentEvent(OPCODE, STAGE);   
            // getStageTime(STAGE,WAFER_NUM);  
        }


        public int getStageTime(int stage, int wafer)
        {
            int i;
            for (i = 0; i < 6; i++)
                if (record_wafer[i] == wafer) break;
            return record_time[i, stage];
        }

        public void finish()
        {
            tcpWriter.Close();
            tcpReader.Close();
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