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
            scheduleing[0, 0] = 1;
            scheduleing[0, 1] = 5;
            /*
                scheduleing[1][0]=2;
                scheduleing[1][1]=6;
            */

        }

    }
}
