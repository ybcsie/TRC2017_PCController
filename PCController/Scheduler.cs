using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCController
{
    class Scheduler
    {
        public static void ScheduleFunction(int[,] scheduleing)
        {

            int x, y;
            for (x = 0; x < 100; x++)
            {
                for (y = 0; y < 2; y++)
                {
                    scheduleing[x, y] = 0;
                }
            }
            scheduleing[0, 0] = 4;
            scheduleing[0, 1] = 1;
            scheduleing[1, 0] = 1;
            scheduleing[1, 1] = 2;
            scheduleing[2, 0] = 2;
            scheduleing[2, 1] = 7;
            scheduleing[3, 0] = 7;
            scheduleing[3, 1] = 3;
            scheduleing[4, 0] = 3;
            scheduleing[4, 1] = 8;
            scheduleing[5, 0] = 8;
            scheduleing[5, 1] = 9;
            scheduleing[6, 0] = 9;
            scheduleing[6, 1] = 5;
            /*
                scheduleing[1][0]=2;
                scheduleing[1][1]=6;
            */

        }

    }
}
