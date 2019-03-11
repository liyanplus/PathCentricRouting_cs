using System;
using System.IO;
using System.Collections.Generic;
using CsvHelper;
using MathNet.Numerics.LinearAlgebra;

namespace EnergyCostEstimator
{
    public class TraceDB : Dictionary<uint, Trace>
    {
        #region property
        public Dictionary<uint, HashSet<uint>> EdgeTraceMapping { get; set; }
        #endregion

        public TraceDB()
        {
        }

        public HashSet<uint> GetTracesAlongPath(List<uint> path)
        {

            // If no paths given, return empty set (No traces)
            if (path.Count == 0)
                return new HashSet<uint>();

            HashSet<uint> result = new HashSet<uint>();

            // Find traces on first path
            EdgeTraceMapping.TryGetValue(path[0], out result);

            HashSet<uint> newValues = new HashSet<uint>();

            // Check for traces on remaining paths
            for (int i=1; i<path.Count; i++)
            {
                // Traces found on path stored in newValues
                bool foundValues = EdgeTraceMapping.TryGetValue(path[i], out newValues);

                // If no traces found on path, then no traces returned
                if (!foundValues)
                {
                    return new HashSet<uint>();
                }

                // Otherwise update result to store intersection of these traces
                result.IntersectWith(newValues);
            }

            // return result
            return result;
       
        }

        public TraceDB(string pparaFilepath, string ptripIdFieldName, string pmassFieldName, string pareaFieldName,
            string ptraceFilepath, string pedgeIdFieldName, string pcostFieldName)
        {
            var traceParas = new Dictionary<string, Dictionary<string, PhysicalParas>>();
            if (Directory.Exists(pparaFilepath))
            {
                foreach (var paraFilename in Directory.GetFiles(pparaFilepath, "*.csv"))
                {
                    // paraFilename is "$vehicle#.csv"
                    string vehicleNo = paraFilename.Split('.')[0];
                    traceParas.Add(vehicleNo, new Dictionary<string, PhysicalParas>());

                    using (var fileReader = new StreamReader(Path.Combine(pparaFilepath, paraFilename)))
                    using (var csvReader = new CsvReader(fileReader))
                    {
                        csvReader.Configuration.HasHeaderRecord = true;
                        csvReader.Read();
                        csvReader.ReadHeader();
                        while (csvReader.Read())
                        {
                            traceParas[vehicleNo].Add(csvReader[ptripIdFieldName], new PhysicalParas()
                            {
                                MassFactor = Convert.ToDouble(csvReader[pmassFieldName]),
                                AreaFactor = Convert.ToDouble(csvReader[pareaFieldName])
                            });
                        }
                    }
                }

                foreach (var traceDirname in Directory.GetDirectories(ptraceFilepath, "*_edge_data"))
                {
                    // traceDirname is "$vehicle#_edge_data"
                    string vehicleNo = traceDirname.Split('_')[0];

                    foreach (var traceFilename in
                        Directory.GetFiles(Path.Combine(ptraceFilepath, traceDirname), "*.csv"))
                    {
                        // traceFilename is "$trace#.csv"
                        string traceName = traceFilename.Split('.')[0];

                        if (traceParas.ContainsKey(vehicleNo) && traceParas[vehicleNo].ContainsKey(traceName))
                        {
                            var t = new Trace()
                            {
                                Id = (uint)this.Count,
                                Name = traceName,
                                Paras = traceParas[vehicleNo][traceName]
                            };

                            var traceCost = new List<double>();

                            using (var fileReader = new StreamReader(Path.Combine(ptraceFilepath, traceDirname, traceFilename)))
                            using (var csvReader = new CsvReader(fileReader))
                            {
                                csvReader.Configuration.HasHeaderRecord = true;
                                csvReader.Read();
                                csvReader.ReadHeader();
                                while (csvReader.Read())
                                {
                                    try
                                    {
                                        t.Add(Convert.ToUInt32(csvReader[pedgeIdFieldName]));
                                        traceCost.Add(Convert.ToDouble(csvReader[pcostFieldName]));
                                    }
                                    catch (FormatException ex)
                                    {
                                        Console.WriteLine(ex);
                                        break;
                                    }
                                }
                            }

                            if (t.Count > 4)
                            {
                                t.Costs = Vector<double>.Build.Dense(traceCost.ToArray());
                                foreach (var partEdgeId in t)
                                {
                                    if (!EdgeTraceMapping.ContainsKey(partEdgeId))
                                    {
                                        EdgeTraceMapping.Add(partEdgeId, new HashSet<uint>());
                                    }
                                    EdgeTraceMapping[partEdgeId].Add(t.Id);
                                }
                                Add(t.Id, t);
                            }
                        }
                    }
                }
            }
            Console.WriteLine(string.Format("Successfully import {0} trajectories.", this.Count));
        }
    }
}
