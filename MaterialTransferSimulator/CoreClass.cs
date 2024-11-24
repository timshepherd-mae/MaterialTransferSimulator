using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.ComponentModel.Design;

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
            Result result = new Result
            {
                LogEntries = new List<LogRecord>()
            };

            //LogEntry logEntry = new LogEntry();
            //logEntry.volumes = new List<double>();

            DateTime currentDate = dateStart;
            double tempVol = 0;

            // assign 


            // cycle through simulation period
            while (currentDate <= dateEnd)
            {
                // initialise new log entry
                LogEntry logEntry = new LogEntry();
                logEntry.SetDate(currentDate);
                logEntry.volumes = new List<double>();

                // collect current state of containers
                foreach (Container c in containers)
                {
                    tempVol = c.currentVolume;
                    logEntry.Add(tempVol);
                    c.WriteLogRecord(currentDate);
                }

                // write daily log to console
                Console.WriteLine(logEntry.ToString());

                // transfer materials
                foreach (Transfer t in transfers)
                {
                    // check transfer has To/From links
                    if (t.linkFrom != null && t.linkTo != null)
                    {
                        // check transfer is active
                        if (t.IsActiveOn(currentDate) && !t.IsSuspendedOn(currentDate))
                        {
                            t.linkFrom.currentVolume -= t.loadActuals[0];
                            t.linkTo.currentVolume += t.loadActuals[2];
                            t.WriteLogRecord(currentDate, t.loadActuals[1]);
                        }
                        else
                        {
                            t.WriteLogRecord(currentDate, 0);
                        }
                    }

                }

                currentDate += stepTime;

            }

            // write the container log records to result
            foreach (Container c in containers)
            {
                foreach (LogRecord r in c.ContainerLog)
                {
                    result.LogEntries.Add(r);
                }
            }

            // write the transfer log records to result
            foreach (Transfer t in transfers)
            {
                foreach (LogRecord r in t.TransferLog)
                {
                    result.LogEntries.Add(r);
                }
            }

            return result;
        }

        public DateRange SimulationExtents()
        {
            return new DateRange()
            {
                Start = dateStart,
                End = dateEnd,
                Description = "Simulation Extents"
            };
        }

    }

    public class Container
    {
        public static int nextContainerId = 0;

        public int id;
        public string name = string.Empty;
        public double currentVolume = 0;
        public double capacity = 0;
        public double bulkOnImport = 1;
        public double bulkOnExport = 1;
        public List<LogRecord> ContainerLog = new List<LogRecord>();

        public int group = 0;
        // public List<int> group = new List<int>();

        public Container() // default constructor
        {
            id = nextContainerId++;
        }

        public void WriteLogRecord(DateTime entrydate)
        {
            LogRecord entry = new LogRecord
            {
                LogId = id,
                LogName = name,
                LogType = "Container",
                LogGroup = group,
                LogDate = entrydate,
                LogValue = currentVolume
            };

            this.ContainerLog.Add(entry);
        }

        public override string ToString()
        {
            return $"{id}: {name}\t{currentVolume}/{capacity}";
        }

    }

    public class Transfer
    {
        public static int nextTransferId = 0;

        public int id;
        public string name = string.Empty;
        public string description = string.Empty;
        public double loadPlanned = 0;
        public double[] loadActuals = new double[3];
        public Container linkFrom = null;
        public Container linkTo = null;
        public double routeLength = 0;  // pending route alignment and container growth modelling
        public LoadBias loadBias = LoadBias.Transfer;
        public List<LogRecord> TransferLog = new List<LogRecord>();

        public int group = 0;
        // public List<int> group = new List<int>();

        public List<DateRange> activeEvents = new List<DateRange>();
        public List<DateRange> suspendEvents = new List<DateRange>();
        public List<DateRange> modifyEvents = new List<DateRange>();

        public Transfer() // default constructor
        {
            id = nextTransferId++;
        }

        public void SetName()
        {
            string nameFrom = (linkFrom == null) ? "UnDef" : linkFrom.name;
            string nameTo = (linkTo == null) ? "UnDef" : linkTo.name;
            string loads = "(";
            loads += $"{loadActuals[0]:F0}";
            loads += (loadBias == LoadBias.Source) ? "*/" : "/";
            loads += $"{loadActuals[1]:F0}";
            loads += (loadBias == LoadBias.Transfer) ? "*/" : "/";
            loads += $"{loadActuals[2]:F0}";
            loads += (loadBias == LoadBias.Target) ? "*)" : ")";

            name = $"{nameFrom} --> {loads} --> {nameTo}";
        }

        public void SetLoadActuals()
        {
            // check for both container connections
            if (linkFrom != null & linkTo != null)
            {
                switch (loadBias)
                {
                    case (LoadBias.Source):

                        loadActuals[0] = loadPlanned;
                        loadActuals[1] = loadPlanned * linkFrom.bulkOnExport;
                        loadActuals[2] = loadPlanned * linkFrom.bulkOnExport * linkTo.bulkOnImport;
                        break;

                    case (LoadBias.Transfer):

                        loadActuals[0] = loadPlanned / linkFrom.bulkOnExport;
                        loadActuals[1] = loadPlanned;
                        loadActuals[2] = loadPlanned * linkTo.bulkOnImport;
                        break;

                    case (LoadBias.Target):

                        loadActuals[0] = loadPlanned / (linkFrom.bulkOnExport * linkTo.bulkOnImport);
                        loadActuals[1] = loadPlanned / linkTo.bulkOnImport;
                        loadActuals[2] = loadPlanned;
                        break;
                }
            }
        }

        public bool IsActiveOn(DateTime date)
        {
            bool active = false;
            foreach (DateRange dr in activeEvents)
            {
                if (date >= dr.Start && date <= dr.End) active = true;
            }
            return active;
        }
        public bool IsSuspendedOn(DateTime date)
        {
            bool suspend = false;
            foreach (DateRange dr in suspendEvents)
            {
                if (date >= dr.Start && date <= dr.End) suspend = true;
            }
            return suspend;
        }

        public void WriteLogRecord(DateTime entrydate, double loadValue)
        {
            LogRecord entry = new LogRecord
            {
                LogId = id,
                LogName = name,
                LogType = "Transfer",
                LogGroup = group,
                LogDate = entrydate,
                LogValue = loadValue
            };

            this.TransferLog.Add(entry);
        }

        public override string ToString()
        {
            return $"{id}: {name}";
        }
    }

    public class LogRecord
    {
        public int LogId;
        public string LogName;
        public string LogType;
        public int LogGroup;
        public DateTime LogDate;
        public Double LogValue;

        public override string ToString()
        {
            string output = LogId.ToString();
            output += "\t" + LogName;
            output += "\t" + LogType;
            output += "\t" + LogGroup.ToString();
            output += "\t" + LogDate.ToString("yyyy-MM-dd");
            output += "\t" + LogValue.ToString();

            return output;
        }

        public string ToSeparatedString(string sep)
        {
            string output = LogId.ToString();
            output += sep + LogName;
            output += sep + LogType;
            output += sep + LogGroup.ToString();
            output += sep + LogDate.ToString("yyyy-MM-dd");
            output += sep + LogValue.ToString();

            return output;
        }

    }

    public struct DateRange
    {
        public DateTime Start;
        public DateTime End;
        public string Description;
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
                output += "\t" + v.ToString("0");
            }

            return output;
        }
    }

    public struct Result
    {
        public List<LogRecord> LogEntries;

        public void Add(LogRecord l)
        {
            LogEntries.Add(l);
        }

        public void LogRecordsToFile(string fpath)
        {
            using (StreamWriter writer = new StreamWriter(fpath))
            {
                string header = "Id,Name,Type,Group,ObservationDate,CurrentValue";
                writer.WriteLine(header);

                foreach (LogRecord l in LogEntries)
                {
                    writer.WriteLine(l.ToSeparatedString(","));
                }
            }
        }

        public void SimulationInfoToFile(string fpath)
        {
            using (StreamWriter writer = new StreamWriter(fpath))
            {
                int x = 0;
                x++;
            }
        }
    }

    public enum LoadBias
    {
        Source = 0,
        Transfer = 1,
        Target = 2
    }

}
