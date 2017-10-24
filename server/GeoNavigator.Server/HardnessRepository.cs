using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace GeoNavigator.Hardness
{
    public class HardnessRepository
    {
        public const string FileName = @"E:\minecraft\OpenComputers-GeoNavigator\server\GeoNavigator.MapViewer\bin\Debug\hardness.json";
        private const string BackupFileName = @"E:\minecraft\OpenComputers-GeoNavigator\server\GeoNavigator.MapViewer\bin\Debug\hardness-backup.json";

        private static readonly object _lock = new object();

        private readonly ConcurrentDictionary<Tuple<long, long, long>, double> _hardness
            = new ConcurrentDictionary<Tuple<long, long, long>, double>();

        public HardnessRepository()
        {
            if (File.Exists(FileName))
            {
                var no = 1;
                string content;
                doTry:
                try
                {
                    content = File.ReadAllText(FileName);
                }
                catch (IOException e)
                {
                    if (no < 3) no++;
                    else
                    {
                        throw;
                    }
                    Thread.Sleep(500);
                    goto doTry;
                }

                var array = JsonConvert.DeserializeObject<KeyValuePair<Tuple<long, long, long>, double>[]>(content);
                foreach (var kv in array)
                {
                    if (!_hardness.TryAdd(kv.Key, kv.Value))
                        throw new InvalidOperationException();
                }

                Console.WriteLine("Hardness values loaded.");
            }
            else
            {
                Console.WriteLine("Hardness values not found.");
            }
        }

        public double? Get(long x, long y, long z)
        {
            if (_hardness.TryGetValue(Tuple.Create(x, y, z), out double value))
                return Math.Max(value, 0);

            return null;
        }

        public bool Any() => _hardness.Any();


        public long MaxX()
        {
            return _hardness.Max(kv => kv.Key.Item1);
        }

        public long MinX()
        {
            return _hardness.Min(kv => kv.Key.Item1);
        }

        public long MaxY()
        {
            return _hardness.Max(kv => kv.Key.Item2);
        }

        public long MinY()
        {
            return _hardness.Min(kv => kv.Key.Item2);
        }

        public long MaxZ()
        {
            return _hardness.Max(kv => kv.Key.Item3);
        }

        public long MinZ()
        {
            return _hardness.Min(kv => kv.Key.Item3);
        }

        public double HardnessMin()
        {
            return _hardness.Min(kv => kv.Value);
        }

        public double HardnessMax()
        {
            return _hardness.Max(kv => kv.Value);
        }

        public double HardnessAverage()
        {
            return _hardness.Average(kv => kv.Value);
        }

        public void StoreHardness(IEnumerable<PointValue> values)
        {
            lock (_lock)
            {
                foreach (var pointValue in values)
                {
                    var key = Tuple.Create(pointValue.PosX, pointValue.PosY, pointValue.PosZ);
                    _hardness.AddOrUpdate(key, pointValue.Hardness, (k, v) => pointValue.Hardness);
                }

                Save();
            }
        }

        private void Save()
        {
            if (File.Exists(FileName))
            {
                if (File.Exists(BackupFileName))
                    File.Delete(BackupFileName);

                File.Move(FileName, BackupFileName);
            }

            var serialized = JsonConvert.SerializeObject(_hardness.ToArray());
            File.WriteAllText(FileName, serialized);
        }
    }
}
