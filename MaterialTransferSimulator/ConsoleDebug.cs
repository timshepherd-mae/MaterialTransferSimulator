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
            Container c = new Container()
            {
                name = "spoil",
                currentVolume = 0,
                capacity = 2500
            };

            Console.WriteLine(c.ToString());

            Container d = new Container()
            {
                name = "excav",
                currentVolume = 1000,
                capacity = 1000
            };

            Console.WriteLine(d.ToString());

            Transfer t = new Transfer()
            {
                linkFrom = d,
                linkTo = c,
                load = 50
            };

            t.SetName();
            Console.WriteLine(t.ToString());

            Simulation sim = new Simulation()
            {
                dateStart = new DateTime(2024, 10, 1),
                dateEnd = new DateTime(2024,10,10)
            };

            Result res = sim.Run();


        }
    }
}
