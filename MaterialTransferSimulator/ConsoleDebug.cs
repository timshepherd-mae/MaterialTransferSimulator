using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialTransferSimulator
{
    internal class ConsoleDebug
    {
        public static void Main()
        {
            Console.WriteLine("ok");

            // create a simulation instance
            Simulation sim = new Simulation()
            {
                dateStart = new DateTime(2024, 10, 1),
                dateEnd = new DateTime(2024,10,11)
            };

            Container c = new Container()
            {
                name = "spoil",
                currentVolume = 0,
                capacity = 3000,
                bulkOnImport = 0.8
            };
            sim.Add(c);

            Console.WriteLine(c.ToString());

            Container d = new Container()
            {
                name = "excavA",
                currentVolume = 2000,
                capacity = 2000,
                bulkOnExport = 1.50
            };
            sim.Add(d);

            Console.WriteLine(d.ToString());

            Container e = new Container()
            {
                name = "excavB",
                currentVolume = 1000,
                capacity = 1000,
                bulkOnExport = 1.50
            };
            sim.Add(e);

            Console.WriteLine(e.ToString());

            Transfer t0 = new Transfer()
            {
                linkFrom = d,
                linkTo = c,
                loadPlanned = 50,
                loadBias = LoadBias.Target
            };
            t0.SetLoadActuals();
            t0.SetName();
            sim.Add(t0);

            Console.WriteLine(t0.ToString());

            Transfer t1 = new Transfer()
            {
                linkFrom = e,
                linkTo = c,
                loadPlanned = 75,
                loadBias = LoadBias.Target
                
            };
            t1.SetLoadActuals();
            t1.SetName();
            sim.Add(t1);

            Console.WriteLine(t1.ToString());

            Result res = sim.Run();

            // output the result to console
            foreach (LogEntry log in res.entries)
            {
                Console.WriteLine(log.ToString());
            }

        }
    }
}
