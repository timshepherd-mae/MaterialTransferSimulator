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
                dateEnd = new DateTime(2024, 11, 23)
            };

            Console.WriteLine("\nContainer Definitions\n---------------------");

            // define containers
            Container c0 = new Container()
            {
                name = "Excav-A",
                currentVolume = 3000,
                capacity = 3000,
                bulkOnExport = 1.5
            };
            sim.Add(c0);

            Console.WriteLine(c0.ToString());

            Container c1 = new Container()
            {
                name = "Excav-B",
                currentVolume = 4000,
                capacity = 4000,
                bulkOnExport = 1.50
            };
            sim.Add(c1);

            Console.WriteLine(c1.ToString());

            Container c2 = new Container()
            {
                name = "Spoil-1",
                currentVolume = 0,
                capacity = 10000,
                bulkOnImport = 0.8,
                bulkOnExport = 1.2
            };
            sim.Add(c2);

            Console.WriteLine(c2.ToString());

            Container c3 = new Container()
            {
                name = "Off-Site",
                currentVolume = 0,
                capacity = 100000,
                bulkOnImport = 1,
                bulkOnExport = 1
            };
            sim.Add(c3);

            Console.WriteLine(c3.ToString());

            Console.WriteLine("\nTransfer Definitions\n--------------------");

            // define transfers
            Transfer t0 = new Transfer()
            {
                linkFrom = c0,
                linkTo = c2,
                loadPlanned = 50,
                loadBias = LoadBias.Source
            };
            t0.SetLoadActuals();
            t0.SetName();
            t0.activeEvents.Add(sim.SimulationExtents());
            t0.suspendEvents.Add(
                new DateRange()
                {
                    Start = new DateTime(2024, 10, 12),
                    End = new DateTime(2024, 10, 15),
                    Description = "T0 Suspend 1"
                }

            );
            sim.Add(t0);

            Console.WriteLine(t0.ToString());

            Transfer t1 = new Transfer()
            {
                linkFrom = c1,
                linkTo = c2,
                loadPlanned = 75,
                loadBias = LoadBias.Source
            };
            t1.SetLoadActuals();
            t1.SetName();
            t1.activeEvents.Add(
                new DateRange()
                {
                    Start = new DateTime(2024, 10, 6),
                    End = new DateTime(2024, 10, 26),
                    Description = "T1 Phase 1"
                }
            );
            t1.activeEvents.Add(
                new DateRange()
                {
                    Start = new DateTime(2024, 10, 30),
                    End = sim.dateEnd,
                    Description = "T1 Phase 2"
                }
            );
            sim.Add(t1);

            Console.WriteLine(t1.ToString());

            Transfer t2 = new Transfer()
            {
                linkFrom = c2,
                linkTo = c3,
                loadPlanned = 200,
                loadBias = LoadBias.Transfer
            };
            t2.SetLoadActuals();
            t2.SetName();
            t2.activeEvents.Add(
                new DateRange()
                {
                    Start = new DateTime(2024, 10, 10),
                    End = new DateTime(2024, 10, 20),
                    Description = "T2 Phase 1"
                }
            );
            t2.activeEvents.Add(
                new DateRange()
                {
                    Start = new DateTime(2024, 10, 25),
                    End = new DateTime(2024, 11, 5),
                    Description = "T2 Phase 2"
                }
            );
            t2.activeEvents.Add(
                new DateRange()
                {
                    Start = new DateTime(2024, 11, 10),
                    End = new DateTime(2024, 11, 20),
                    Description = "T2 Phase 3"
                }
            );
            sim.Add(t2);

            Console.WriteLine(t2.ToString());


            Console.WriteLine("\nResults\n-------");

            Result res = sim.Run();

            string fpath = "C:\\Users\\times\\Documents\\MTS_tests\\out01.csv";
            res.LogRecordsToFile(fpath);


            // output the result to console
            foreach (LogRecord log in res.LogEntries)
            {
                Console.WriteLine(log.ToString());
            }


        }
    }
}
