using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PCController
{
    class Initializer
    {
        public static volatile bool moverRunning = false;
        public static volatile bool isMovStageAllowed = false;

        public static int initStage;

        public static void setup()
        {
            int pointCount = 20;

            double[] coor = new double[] { ArmData.coordinate[0, 0], ArmData.coordinate[0, 1], ArmData.coordinate[0, 2], ArmData.coordinate[0, 3] };

            AngleList linearAngleList = RoutPlanning.routplanning(coor[0], coor[1], coor[2], coor[3], ArmData.distance + 20, pointCount);

            Angle currNode = linearAngleList.headAngle;

            if (currNode != null)
                currNode = currNode.nextangle;
            else
            {
                Program.form.showWarnning("motivation error!");
                return;
            }

            Program.form.mesPrintln("Initializer: Initializing...");

            SyntecClient.writeGVar(699, pointCount);

            int i = 0;
            while (currNode != null)
            {
                SyntecClient.writeGVar(700 + i, (double)currNode.one - coor[0]);
                SyntecClient.writeGVar(800 + i, (double)currNode.three - coor[2]);
                SyntecClient.writeGVar(900 + i++, (double)currNode.four - coor[3]);


                coor[0] = (double)currNode.one;
                coor[2] = (double)currNode.three;
                coor[3] = (double)currNode.four;

                currNode = currNode.nextangle;
            }

            setupZ();

            ArmData.measureangle = new double[10, 4];
            for (i = 0; i < 10; i++)
            {
                ArmData.measureangle[i, 0] = 0;
                ArmData.measureangle[i, 1] = 0;
                ArmData.measureangle[i, 2] = 0;
                ArmData.measureangle[i, 3] = 0;
            }

            Program.form.mesPrintln("Initializer: Initialized successfully!");
            isMovStageAllowed = true;

        }

        public static void setupZ()
        {
            setupZ(120, 5);
        }

        public static void setupZ(double offset, int pointCount)
        {
            double[] coor = new double[] { ArmData.coordinate[8, 0], ArmData.coordinate[8, 1], ArmData.coordinate[8, 2], ArmData.coordinate[8, 3] };

            AngleList linearAngleList = RoutPlanning.routplanning(coor[0], coor[1], coor[2], coor[3], ArmData.distance - offset, pointCount);

            Angle currNode = linearAngleList.headAngle;

            if (currNode != null)
                currNode = currNode.nextangle;
            else
            {
                Program.form.showWarnning("motivation error!");
                return;
            }

            Program.form.mesPrintln("Initializer: Initializing...");

            SyntecClient.writeGVar(1500, pointCount);

            int i = 0;
            while (currNode != null)
            {
                SyntecClient.writeGVar(1600 + i, (double)currNode.one - coor[0]);
                SyntecClient.writeGVar(1700 + i, (double)currNode.three - coor[2]);
                SyntecClient.writeGVar(1800 + i++, (double)currNode.four - coor[3]);


                coor[0] = (double)currNode.one;
                coor[2] = (double)currNode.three;
                coor[3] = (double)currNode.four;

                currNode = currNode.nextangle;
            }

        }

        public static void originSetter()
        {
            /*
SyntecClient.setJOGSpeed(70);


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
*/

            Program.form.mesPrintln("Initializer: Starting initializer...");



            //origin finder

            //                double[] org = new double[4];
            double[] tmpPos = new double[2];

            double[] pos;


            //init C3
            Program.form.mesPrintln("Initializer: Init C3...");
            JOGUntilSensor(3, SyntecClient.JOGMode.POSITIVE, 10, false);
            JOGUntilSensor(3, SyntecClient.JOGMode.NEGATIVE, 30, true);

            Program.form.mesPrintln("Initializer: Setting origin C3...");
            SyntecClient.setOrigin(3);
            Thread.Sleep(300);


            //init C2
            Program.form.mesPrintln("Initializer: Init C2...");

            //JOG until sensor on and then off
            JOGUntilSensor(2, SyntecClient.JOGMode.NEGATIVE, 30, true);
            Program.form.mesPrintln("Initializer: JOG Until C2 sensor off...");
            JOGUntilSensor(2, SyntecClient.JOGMode.NEGATIVE, 30, false);

            //JOG back until sensor on
            JOGUntilSensor(2, SyntecClient.JOGMode.POSITIVE, 10, true);

            //save cur pos
            SyntecClient.getPos(out pos);
            tmpPos[0] = pos[1];

            //keep JOGing until sensor off
            JOGUntilSensor(2, SyntecClient.JOGMode.POSITIVE, 30, false);

            //JOG back until sensor on
            JOGUntilSensor(2, SyntecClient.JOGMode.NEGATIVE, 10, true);

            //save cur pos
            SyntecClient.getPos(out pos);
            tmpPos[1] = pos[1];

            SyntecClient.writeGVar(1002, (tmpPos[0] + tmpPos[1]) / 2 - tmpPos[1]);
            runInitAndWait(1);
            Program.form.mesPrintln("Initializer: Setting origin C2...");
            SyntecClient.setOrigin(2);
            Thread.Sleep(300);



            //init C1
            Program.form.mesPrintln("Initializer: Init C1...");

            JOGUntilSensor(1, SyntecClient.JOGMode.POSITIVE, 30, false);
            JOGUntilSensor(1, SyntecClient.JOGMode.NEGATIVE, 30, true);

            Program.form.mesPrintln("Initializer: Setting origin C1...");
            SyntecClient.setOrigin(1);
            Thread.Sleep(300);

            runInitAndWait(2);

            //init C4
            Program.form.mesPrintln("Initializer: Init C4...");

            //JOG until sensor on and then off
            JOGUntilSensor(4, SyntecClient.JOGMode.NEGATIVE, 30, true);
            JOGUntilSensor(4, SyntecClient.JOGMode.NEGATIVE, 30, false);

            //JOG back until sensor on
            JOGUntilSensor(4, SyntecClient.JOGMode.POSITIVE, 10, true);

            //save cur pos
            SyntecClient.getPos(out pos);
            tmpPos[0] = pos[3];

            //keep JOGing until sensor off
            JOGUntilSensor(4, SyntecClient.JOGMode.POSITIVE, 30, false);

            //JOG back until sensor on
            JOGUntilSensor(4, SyntecClient.JOGMode.NEGATIVE, 10, true);

            //save cur pos
            SyntecClient.getPos(out pos);
            tmpPos[1] = pos[3];

            SyntecClient.writeGVar(1004, (tmpPos[0] + tmpPos[1]) / 2 - tmpPos[1]);
            runInitAndWait(3);
            Program.form.mesPrintln("Initializer: Setting origin C4...");
            SyntecClient.setOrigin(4);
            Thread.Sleep(300);

            runInitAndWait(4);

            home(false);

            Program.form.mesPrintln("Initializer: Done!");

        }
        public static void home(bool isC1LeftSide)
        {
            SyntecClient.cycleReset();

            writeG91AngleByAbs(2, 150);
            writeG91AngleByAbs(3, 60);
            writeG91AngleByAbs(4, -120);

            if (isC1LeftSide)
                runInitAndWait(5);
            else
                runInitAndWait(6);

        }

        public static void linearMOV(double distance, int pointCount)
        {
            while (SyntecClient.readSingleVar(11) != 0)
                Thread.Sleep(20);

            double[] Pos;
            SyntecClient.getPos(out Pos);

            AngleList linearAngleList = RoutPlanning.routplanning(Pos[0], Pos[2], Pos[1], Pos[3], distance, pointCount);

            SyntecClient.writeGVar(12, pointCount);

            Angle currNode = linearAngleList.headAngle;

            if (currNode != null)
                currNode = currNode.nextangle;
            else
            {
                Program.form.showWarnning("motivation error!");
                return;
            }

            int i = 0;
            while (currNode != null)
            {
                SyntecClient.writeGVar(1011 + 10 * i, (double)currNode.one - Pos[0]);
                SyntecClient.writeGVar(1012 + 10 * i, (double)currNode.three - Pos[1]);
                SyntecClient.writeGVar(1014 + 10 * i++, (double)currNode.four - Pos[3]);

                Pos[0] = (double)currNode.one;
                Pos[1] = (double)currNode.three;
                Pos[3] = (double)currNode.four;

                currNode = currNode.nextangle;
            }

            SyntecClient.writeGVar(11, 1);
        }

        public static void thetaP(bool precise)
        {
            while (SyntecClient.readSingleVar(11) != 0)
                Thread.Sleep(10);


            if (precise)
                SyntecClient.writeGVar(11, 5);
            else
                SyntecClient.writeGVar(11, 2);


        }
        public static void thetaN(bool precise)
        {

            while (SyntecClient.readSingleVar(11) != 0)
                Thread.Sleep(10);


            if (precise)
                SyntecClient.writeGVar(11, 6);
            else
                SyntecClient.writeGVar(11, 3);

        }

        private static void writeG91AngleByAbs(int axis, double absAngle)
        {
            double[] pos;
            SyntecClient.getPos(out pos);
            Program.form.mesPrintln(string.Format("Move to C{0:d} = {1:f3}", axis, absAngle));
            SyntecClient.writeGVar(1000 + axis, absAngle - pos[axis - 1]);
            Program.form.mesPrintln(string.Format("G91 : {0:f3}", absAngle - pos[axis - 1]));
        }

        private static void writeG91AngleByOffset(int axis, double angleOffset)
        {
            SyntecClient.writeGVar(1000 + axis, angleOffset);

        }

        private static void JOGUntilSensor(int axis, int direction, int speed, bool sensorState)
        {
            SyntecClient.cycleReset();
            SyntecClient.setJOGSpeed(speed);

            SyntecClient.writeReg(25, sensorState ? 1 : 0);

            SyntecClient.JOG(axis, direction);

            double pos = SyntecClient.Pos[axis - 1];
            int count = 0;

            while (SyntecClient.readIBit(330 + axis) != sensorState)
            {
                Thread.Sleep(20);

                if (count++ % 5 == 1)
                    if (Math.Abs(SyntecClient.Pos[axis - 1] - pos) < 0.5)
                    {
                        Program.form.mesPrintln("Initializer: JOG is not responsing, trying reJOG ...");
                        SyntecClient.JOG(axis, SyntecClient.JOGMode.STOP);
                        Thread.Sleep(300);
                        pos = SyntecClient.Pos[axis - 1];
                        SyntecClient.JOG(axis, direction);
                    }
            }

            SyntecClient.JOG(axis, SyntecClient.JOGMode.STOP);

            SyntecClient.writeReg(25, 0);

            SyntecClient.cycleReset();



        }

        private static void JOGZUntilSensorChange(int direction, int speed)
        {
            SyntecClient.cycleReset();
            SyntecClient.setJOGSpeed(speed);

            SyntecClient.writeReg(25, 0);
            SyntecClient.writeReg(57, 0);
            SyntecClient.writeReg(57, 1);

            SyntecClient.JOG(3, direction);

            double pos = SyntecClient.Pos[2];
            int count = 0;

            while (SyntecClient.readReg(56) != 1 || !SyntecClient.readIBit(335))
            {
                Thread.Sleep(5);

                if (count++ % 20 == 1)
                    if (Math.Abs(SyntecClient.Pos[2] - pos) < 0.3)
                    {
                        Program.form.mesPrintln("Initializer: JOG is not responsing, trying reJOG ...");
                        SyntecClient.JOG(3, SyntecClient.JOGMode.STOP);
                        Thread.Sleep(300);
                        pos = SyntecClient.Pos[2];
                        SyntecClient.JOG(3, direction);
                    }
            }

            SyntecClient.JOG(3, SyntecClient.JOGMode.STOP);

            SyntecClient.cycleReset();

        }

        private static void JOGZUntilSensor_InitOn(int direction, int speed)
        {
            SyntecClient.cycleReset();
            SyntecClient.setJOGSpeed(speed);

            SyntecClient.writeReg(25, 1);
            SyntecClient.writeReg(55, 0);
            SyntecClient.writeReg(57, 0);
            SyntecClient.writeReg(57, 1);

            SyntecClient.JOG(3, direction);

            double pos = SyntecClient.Pos[2];
            int count = 0;

            while (SyntecClient.readIBit(335) && SyntecClient.readReg(55) != 1)
            {
                if (count++ % 20 == 1)
                    if (Math.Abs(SyntecClient.Pos[2] - pos) < 0.3)
                    {
                        Program.form.mesPrintln("Initializer: JOG is not responsing, trying reJOG ...");
                        SyntecClient.JOG(3, SyntecClient.JOGMode.STOP);
                        Thread.Sleep(300);
                        pos = SyntecClient.Pos[2];
                        SyntecClient.JOG(3, direction);
                    }
                Thread.Sleep(5);
            }

            SyntecClient.JOG(3, SyntecClient.JOGMode.STOP);

            SyntecClient.writeReg(25, 0);

            SyntecClient.cycleReset();


        }


        private static void runInitAndWait(int step)
        {
            SyntecClient.cycleReset();

            NCGen.genInitNC(step);

            SyntecClient.uploadNCFile(SyntecClient.NCFileName.INITIALIZER);

            SyntecClient.cycleStart();

            while (!SyntecClient.isBusy())
                Thread.Sleep(50);

            while (!SyntecClient.isIdle())
                Thread.Sleep(50);

            Thread.Sleep(300);
            SyntecClient.cycleReset();
        }

        public static void movToStage(int stageIndex)
        {
            Program.form.mesPrintln(string.Format("Move to stage {0:d}", stageIndex));

            if (stageIndex == 6)
            {
                initStage = 6;
                writeG91AngleByAbs(1, ArmData.coordinate[6, 0]);
                writeG91AngleByAbs(2, ArmData.coordinate[6, 2]);
                writeG91AngleByAbs(3, 25);
                writeG91AngleByAbs(4, ArmData.coordinate[6, 3]);

            }
            else if (stageIndex == 7)
            {
                initStage = 7;
                writeG91AngleByAbs(1, ArmData.coordinate[7, 0]);
                writeG91AngleByAbs(2, ArmData.coordinate[7, 2]);
                writeG91AngleByAbs(3, 25);
                writeG91AngleByAbs(4, ArmData.coordinate[7, 3]);

            }
            else if (stageIndex == 8)
            {
                initStage = 8;
                writeG91AngleByAbs(1, ArmData.coordinate[8, 0] + 2 * ArmData.coordinate[8, 2] + 2 * ArmData.coordinate[8, 3]);
                writeG91AngleByAbs(2, -ArmData.coordinate[8, 2]);
                writeG91AngleByAbs(3, 25);
                writeG91AngleByAbs(4, -ArmData.coordinate[8, 3]);

            }
            else
            {
                Program.form.mesPrintln("stage denied.");
                return;
            }

            runInitAndWait(7);

            Program.form.mesPrintln("Move to stage successful!");

            startMover();

        }

        private static void startMover()
        {
            ThreadsController.addThreadAndStartByFunc(() =>
            {
                moverRunning = true;
                runInitAndWait(9);
                moverRunning = false;
                Program.form.mesPrintln("Initializer: Move Control Loop has been left successfully!");

            });

        }
        public static void readInitPoint()
        {
            StreamReader rd = new StreamReader("initCoo.txt");
            int stageId;

            for (int i = 5; i < 10; i++)
            {
                stageId = Convert.ToInt32(rd.ReadLine());

                ArmData.measureangle[stageId, 0] = Convert.ToDouble(rd.ReadLine());
                ArmData.measureangle[stageId, 2] = Convert.ToDouble(rd.ReadLine());
                ArmData.measureangle[stageId, 3] = Convert.ToDouble(rd.ReadLine());

            }
            rd.Close();
        }


        public static void getPoint()
        {
            double[] pos;
            SyntecClient.getPos(out pos);

            ArmData.measureangle[initStage, 0] = pos[0];
            ArmData.measureangle[initStage, 2] = pos[1];
            ArmData.measureangle[initStage, 3] = pos[3];

            StreamWriter initCooWriter = new StreamWriter("initCoo.txt");
            for (int i = 5; i < 10; i++)
            {
                initCooWriter.WriteLine(i);
                initCooWriter.WriteLine(ArmData.measureangle[i, 0]);
                initCooWriter.WriteLine(ArmData.measureangle[i, 2]);
                initCooWriter.WriteLine(ArmData.measureangle[i, 3]);
            }
            initCooWriter.Close();

            Program.form.mesPrintln(
                string.Format("Stage {0:d} has been initialized by: ", initStage) +
                string.Format("C1 = {0:f3}", pos[0]) +
                string.Format("C2 = {0:f3}", pos[1]) +
                string.Format("C4 = {0:f3}", pos[3])
                );

            Program.form.mesPrintln("Waiting...");

            while (!SyntecClient.isBusy())
                Thread.Sleep(50);


            while (SyntecClient.readSingleVar(11) != 0)
                Thread.Sleep(50);


            Program.form.mesPrintln("Leaving...");

            SyntecClient.writeGVar(11, 4);

        }
        public static void catchZInit()
        {
            if (SyntecClient.Pos[1] > 0)//C1 at Right side
                home(true);

            setupZ();

            writeG91AngleByAbs(1, ArmData.coordinate[3, 0]);
            writeG91AngleByAbs(2, ArmData.coordinate[3, 2]);
            writeG91AngleByAbs(3, 110);
            writeG91AngleByAbs(4, ArmData.coordinate[3, 3]);

            while (!SyntecClient.isIdle())
                Thread.Sleep(500);

            runInitAndWait(8);

            Program.form.mesPrintln("OK!");
            /*
            SyntecClient.writeCBit(36, true);
            Program.form.showWarnning("move arm to chamber and then press OK");
            SyntecClient.writeCBit(36, false);
            */
        }
        public static void manualCatch()
        {
            setupZ(-20, 30);

            writeG91AngleByAbs(1, ArmData.coordinate[8, 0]);
            writeG91AngleByAbs(2, ArmData.coordinate[8, 2]);
            writeG91AngleByOffset(3, 0);
            writeG91AngleByAbs(4, ArmData.coordinate[8, 3]);

            runInitAndWait(8);

            Program.form.showWarnning("press OK to move back");

            runInitAndWait(11);

        }

        public static void getWaferTester(int id)
        {
            setupZ(-20,35);

            writeG91AngleByAbs(1, ArmData.coordinate[8, 0]);
            writeG91AngleByAbs(2, ArmData.coordinate[8, 2]);
            writeG91AngleByAbs(3, ArmData.WaferZs_IO[id, 0]);
            writeG91AngleByAbs(4, ArmData.coordinate[8, 3]);

            runInitAndWait(8);

            Program.form.showWarnning("press OK to get wafer");

            SyntecClient.writeOBit(320, true);
            while (SyntecClient.isBusy())
            {
                Thread.Sleep(50);
            }

            writeG91AngleByAbs(3, ArmData.WaferZs_IO[id, 1]);

            runInitAndWait(10);
            runInitAndWait(11);

            SyntecClient.writeOBit(320, false);
            while (SyntecClient.isBusy())
            {
                Thread.Sleep(50);
            }

        }

        public static void catchZ(int id)
        {

            //catch Z
            if (!SyntecClient.readIBit(335))
            {
                Program.form.mesPrintln("z sensor is not connected");
                return;
            }

            /*
            //move Z
            writeG91AngleByAbs(3, 105 + id * 4 - 4);
            runInitAndWait(10);
            */
            Program.form.mesPrintln("Catch Z...");

            double[] pos;
            double z1, z2;

            JOGZUntilSensor_InitOn(SyntecClient.JOGMode.POSITIVE, 10);
            SyntecClient.getPos(out pos);
            z1 = pos[2];
            Program.form.mesPrintln("Z1 got.");

            writeG91AngleByOffset(3, 2);
            runInitAndWait(10);

            JOGZUntilSensor_InitOn(SyntecClient.JOGMode.NEGATIVE, 10);
            SyntecClient.getPos(out pos);
            z2 = pos[2];

            writeG91AngleByOffset(3, 6);
            runInitAndWait(10);

            //ArmData.WaferZs[id] = (z1 + z2) / 2;

            //Program.form.mesPrintln(string.Format("Z = {0:f3}", ArmData.WaferZs[id]));

            Program.form.mesPrintln("Done");

        }

    }
}
