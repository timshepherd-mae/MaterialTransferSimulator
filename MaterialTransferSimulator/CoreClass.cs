using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialTransferSimulator
{
    public class Simulation
    {
        public string name = string.Empty;

        public DateTime dateStart = DateTime.MinValue;
        public DateTime dateEnd = DateTime.MinValue;  
        public TimeSpan stepTime = TimeSpan.FromDays(1);

        public List<Container> containers = new List<Container>();
        public List<Transfer> transfers = new List<Transfer>();

        public void Add(Container c) // overloaded add container
        {
            containers.Add(c);
        }
        public void Add(Transfer t) // overloaded add transfer
        {
            transfers.Add(t);
        }

        public Result Run()
        {
            // initialise result and log
            Result result = new Result();
            result.entries = new List<LogEntry>();

            LogEntry logEntry = new LogEntry();
            logEntry.volumes = new List<double>();

            DateTime currentDate = dateStart;

            // cycle through simulation period
            while (currentDate <= dateEnd)
            {
                // clear the log and set date
                logEntry.Clear();
                logEntry.SetDate(currentDate);

                // collect current state of containers
                foreach (Container c in containers)
                {
                    logEntry.Add(c.currentVolume);
                }

                // Console.WriteLine(logEntry.ToString());

                // update the result
                result.Add(logEntry);

                // transfer materials
                foreach (Transfer t in transfers)
                {
                    // check transfer is valid
                    // (is active at current date and has links From/To)
                    if (t.dateStart <= currentDate & t.linkFrom != null & t.linkTo != null)
                    {
                        t.linkFrom.currentVolume -= t.load;
                        t.linkTo.currentVolume -= t.load;
                    }
                }

                currentDate += stepTime;

            }

            return result;
        }
    }

    public class Container
    {
        public static int nextContainerId = 0;

        public int id;
        public string name = string.Empty;
        public double currentVolume = 0;
        public double capacity = 1000;
        public double bulkOnImport = 1;
        public double bulkOnExport = 1;

        public Container() // default constructor
        {
            id = nextContainerId++;
        }

        public override string ToString()
        {
            return $"{id}:{name} - {currentVolume}";
        }
    }

    public class Transfer
    {
        public static int nextTransferId = 0;

        public int id;
        public string name = string.Empty;
        public double load = 0;
        public Container linkFrom = null;
        public Container linkTo = null;
        public LoadBias loadBias = LoadBias.Transfer;
        public DateTime dateStart = DateTime.MinValue;

        public Transfer() // default constructor
        {
            id = nextTransferId++;
        }

        public void SetName()
        {
            string nameFrom = (linkFrom == null) ? "UnDef" : linkFrom.name;
            string nameTo = (linkTo == null) ? "UnDef" : linkTo.name;
            name = $"{nameFrom}-->{nameTo}";
        }

        public override string ToString()
        {
            return $"{id}:{name} - {load}";
        }
    }

    public struct LogEntry
    {
        public DateTime date;
        public List<double> volumes;

        public void Clear()
        {
            date = DateTime.MinValue;
            volumes.Clear();
        }

        public void SetDate(DateTime d)
        {
            date = d;
        }

        public void Add(double v)
        {
            volumes.Add(v);
        }

        public override string ToString()
        {
            string output = date.ToString("yyyy-MM-dd");
            foreach (double v in volumes)
            {
                output += "\t0" + v.ToString();
            }

            return output;
        }
    }

    public struct Result
    {
        public List<LogEntry> entries;

        public void Add(LogEntry l)
        {
            entries.Add(l);
        }
    }

    public enum LoadBias
    {
        Source = 0,
        Transfer = 1,
        Target = 2
    }
}
