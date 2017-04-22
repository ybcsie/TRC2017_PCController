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

        public static bool isConnected()
        {
            if (tcpClient == null)
                return false;

            return tcpClient.Connected;
        }

        public static void connect(string ip_in,int port_in)
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
                tcpReader= null;
            }

            if (tcpWriter != null)
            {
                tcpWriter.Close();
                tcpWriter= null;
            }

            if (tcpClient != null)
            {
                tcpClient.Close();
                tcpClient = null;
            }

        }


        private static string getCmd()
        {
            if(!isConnected())
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
            if(!isConnected())
            {
                connectionErrorHandler();
                return -1;
            }

        }


        public static void communicate()
        {
            if (!isConnected())
            {
                connectionErrorHandler();
                return;
            }

            string command, answer = "~Ack,";
            string[] para, para2, para3, para4, para5, para7, para9, para11, para13, para15;

            //---1---
            command = getCmd();

            para = command.Split(',');

            mesPrintln(command);


            for (int i = 1; i < para.Length; i++)
            {
                answer += para[i];
                if (i < para.Length - 1) answer += ",";
            }

            mesPrintln(answer);

            Thread.Sleep(1000);
            tcpWriter.WriteLine(answer);
            tcpWriter.Flush();



            //---2---
            command = getCmd();

            para2 = command.Split(',');

            mesPrintln(command);

            answer = "~Ack,";
            for (int i = 1; i < para2.Length; i++)
            {
                answer += para2[i];
                if (i < para2.Length - 1) answer += ",";
            }

            mesPrintln(answer);

            Thread.Sleep(1000);
            tcpWriter.WriteLine(answer);
            tcpWriter.Flush();




            //---event1---
            string event1 = "~Evt,GetWaferStart,W02,CassA-02@";
            tcpWriter.WriteLine(event1);
            tcpWriter.Flush();

            command=getCmd();
            para3 = command.Split(',');

            mesPrintln(command);

            for (int i = 1; i < para3.Length; i++)
            {
                if (i == para3.Length - 1)
                {
                    if (para3[i].Equals('0')) break;
                    else
                    {
                        mesPrintln("Rejected event1!");
                        //System.out.println("Rejected event1!");  //simulator must stop
                    }
                }
            }

            Thread.Sleep(1000);

            //---event2---
            string event2 = "~Evt,GetWaferCompleted,W02,CassA-02@";
            tcpWriter.WriteLine(event2);
            tcpWriter.Flush();

            command=getCmd();

            mesPrintln(command);

            //---event3--- 
            string event3 = "~Evt,PutWaferStart,W02,A@";
            tcpWriter.WriteLine(event3);
            tcpWriter.Flush();

            command=getCmd();
            para4 = command.Split(',');

            mesPrintln(command);

            for (int i = 1; i < para4.Length; i++)
            {
                if (i == para4.Length - 1)
                {
                    if (para4[i].Equals('0')) break;
                    else
                    {
                        mesPrintln("Rejected event3!");
                        //System.out.println("Rejected event3!");  //simulator must stop
                    }
                }
            }
            Thread.Sleep(1000);

            //---event4---
            string event4 = "~Evt,PutWaferCompleted,W02,A@";
            tcpWriter.WriteLine(event4);
            tcpWriter.Flush();

            command=getCmd();
            mesPrintln(command);

            Thread.Sleep(50000);


            //---event5--- 
            string event5 = "~Evt,GetWaferStart,W02,A@";
            tcpWriter.WriteLine(event5);
            tcpWriter.Flush();

            command=getCmd();
            para5 = command.Split(',');

            mesPrintln(command);

            for (int i = 1; i < para5.Length; i++)
            {
                if (i == para5.Length - 1)
                {
                    if (para5[i].Equals('0')) break;
                    else
                    {
                        mesPrintln("Rejected event5!");
                        //System.out.println("Rejected event5!");  //simulator must stop
                    }
                }
            }

            Thread.Sleep(1000);



            //---event6
            string event6 = "~Evt,GetWaferCompleted,W02,A@";

            tcpWriter.WriteLine(event6);
            tcpWriter.Flush();

            command=getCmd();

            mesPrintln(command);

            //---event7
            string event7 = "~Evt,PutWaferStart,W02,B@";
            tcpWriter.WriteLine(event7);
            tcpWriter.Flush();

            command=getCmd();
            para7 = command.Split(',');

            mesPrintln(command);

            for (int i = 1; i < para7.Length; i++)
            {
                if (i == para7.Length - 1)
                {
                    if (para7[i].Equals('0')) break;
                    else
                    {
                        mesPrintln("Rejected event7!");
                        //System.out.println("Rejected event7!");  //simulator must stop
                    }
                }
            }

            Thread.Sleep(1000);




            //---event8
            string event8 = "~Evt,PutWaferCompleted,W02,B@";

            tcpWriter.WriteLine(event8);
            tcpWriter.Flush();

            command=getCmd();

            mesPrintln(command);

            Thread.Sleep(50000);




            //---event9
            string event9 = "~Evt,GetWaferStart,W02,B@";
            tcpWriter.WriteLine(event9);
            tcpWriter.Flush();

            command=getCmd();
            para9 = command.Split(',');

            mesPrintln(command);

            for (int i = 1; i < para9.Length; i++)
            {
                if (i == para9.Length - 1)
                {
                    if (para9[i].Equals('0')) break;
                    else
                    {
                        mesPrintln("Rejected event9!");
                        //System.out.println("Rejected event9!");  //simulator must stop
                    }
                }
            }

            Thread.Sleep(1000);


            //---event10
            string event10 = "~Evt,GetWaferCompleted,W02,B@";

            tcpWriter.WriteLine(event10);
            tcpWriter.Flush();

            command=getCmd();
            mesPrintln(command);


            //---event11
            string event11 = "~Evt,PutWaferStart,W02,E@";
            tcpWriter.WriteLine(event11);
            tcpWriter.Flush();

            command=getCmd();
            para11 = command.Split(',');
            mesPrintln(command);

            for (int i = 1; i < para11.Length; i++)
            {
                if (i == para11.Length - 1)
                {
                    if (para11[i].Equals('0')) break;
                    else
                    {
                        mesPrintln("Rejected event11!");
                        //System.out.println("Rejected event11!");  //simulator must stop
                    }
                }
            }

            Thread.Sleep(1000);




            //---event12
            string event12 = "~Evt,PutWaferCompleted,W02,E@";
            tcpWriter.WriteLine(event12);
            tcpWriter.Flush();

            command=getCmd();
            mesPrintln(command);

            Thread.Sleep(50000);



            //---event13
            string event13 = "~Evt,GetWaferStart,W02,E@";
            tcpWriter.WriteLine(event13);
            tcpWriter.Flush();

            command=getCmd();
            para13 = command.Split(',');
            mesPrintln(command);

            for (int i = 1; i < para13.Length; i++)
            {
                if (i == para13.Length - 1)
                {
                    if (para13[i].Equals('0')) break;
                    else
                    {
                        mesPrintln("Rejected event13!");
                        //System.out.println("Rejected event13!");  //simulator must stop
                    }
                }
            }
            Thread.Sleep(1000);

            //---event14
            string event14 = "~Evt,GetWaferCompleted,W02,E@";
            tcpWriter.WriteLine(event14);
            tcpWriter.Flush();

            command=getCmd();
            mesPrintln(command);

            Thread.Sleep(15000);


            //---event15
            string event15 = "~Evt,PutWaferStart,W02,CassB-24@";

            tcpWriter.WriteLine(event15);
            tcpWriter.Flush();

            command=getCmd();
            para15 = command.Split(',');
            mesPrintln(command);

            for (int i = 1; i < para15.Length; i++)
            {
                if (i == para15.Length - 1)
                {
                    if (para15[i].Equals('0')) break;
                    else
                    {
                        mesPrintln("Rejected event15!");
                        //System.out.println("Rejected event15!");  //simulator must stop
                    }
                }
            }
            Thread.Sleep(1000);

            //---event16
            string event16 = "~Evt,PutWaferCompleted,W02,CassB-24@";
            tcpWriter.WriteLine(event16);
            tcpWriter.Flush();

            command=getCmd();
            mesPrintln(command);

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
