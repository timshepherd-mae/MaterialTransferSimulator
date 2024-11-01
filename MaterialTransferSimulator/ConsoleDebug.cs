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
                dateEnd = new DateTime(2024,10,10)
            };

            Container c = new Container()
            {
                name = "spoil",
                currentVolume = 0,
                capacity = 2500
            };
            sim.Add(c);

            Console.WriteLine(c.ToString());

            Container d = new Container()
            {
                name = "excav",
                currentVolume = 1000,
                capacity = 1000
            };
            sim.Add(d);

            Console.WriteLine(d.ToString());

            Transfer t = new Transfer()
            {
                linkFrom = d,
                linkTo = c,
                load = 50
            };
            sim.Add(t);

            t.SetName();
            Console.WriteLine(t.ToString());

            Result res = sim.Run();


        }
    }
}
