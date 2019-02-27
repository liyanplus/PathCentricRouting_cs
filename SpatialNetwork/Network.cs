using System;
using System.IO;
using System.Collections.Generic;
using CsvHelper;

namespace SpatialNetwork
{
    public class Network
    {
        #region property
        public Dictionary<uint, Node> Nodes { get; set; }
        public Dictionary<uint, Edge> Edges { get; set; }
        #endregion
        public Network()
        {
        }

        public Network(string pedgeFilepath,
            string pedgeIdFieldName, string pfromNodeIdFieldName, string ptoNodeIdFieldName, string pbiDirectionalFieldName,
            string pnodeFilepath = null, 
            string pnodeIdFieldName = null, string pnodeLatFieldName = null, string pnodeLonFieldName = null)
        {
            if (pnodeFilepath != null && pnodeIdFieldName != null &&
                pnodeLatFieldName != null && pnodeLonFieldName != null)
            {
                // if the filepath to the csv file containing info of nodes
                // then read the file and import the nodes
                using (var fileReader = new StreamReader(pnodeFilepath))
                using (var csvReader = new CsvReader(fileReader))
                {
                    csvReader.Configuration.HasHeaderRecord = true;
                    csvReader.Read();
                    csvReader.ReadHeader();
                    while (csvReader.Read())
                    {
                        try
                        {
                            var n = new Node()
                            {
                                Id = Convert.ToUInt32(csvReader[pnodeIdFieldName]),
                                Latitude = Convert.ToSingle(csvReader[pnodeLatFieldName]),
                                Longitude = Convert.ToSingle(csvReader[pnodeLonFieldName])
                            };

                            foreach (var fieldName in csvReader.Context.HeaderRecord)
                            {
                                if (fieldName != pnodeIdFieldName &&
                                    fieldName != pnodeLatFieldName &&
                                    fieldName != pnodeLonFieldName)
                                {
                                    n.Attrs.Add(fieldName, csvReader[fieldName]);
                                }
                            }

                            Nodes.Add(n.Id, n);
                        }
                        catch (FormatException ex)
                        {
                            Console.Write(ex);
                        }
                    }
                }
            }

            // read the csv file containing info of edges
            using (var fileReader = new StreamReader(pedgeFilepath))
            using (var csvReader = new CsvReader(fileReader))
            {
                csvReader.Configuration.HasHeaderRecord = true;
                csvReader.Read();
                csvReader.ReadHeader();
                while (csvReader.Read())
                {
                    try
                    {
                        var e = new Edge()
                        {
                            Id = Convert.ToUInt32(csvReader[pedgeIdFieldName]),
                            FromNodeId = Convert.ToUInt32(csvReader[pfromNodeIdFieldName]),
                            ToNodeId = Convert.ToUInt32(csvReader[ptoNodeIdFieldName]),
                            BiDirectional = (csvReader[pbiDirectionalFieldName] == "0")
                        };

                        foreach (var fieldName in csvReader.Context.HeaderRecord)
                        {
                            if (fieldName != pedgeIdFieldName &&
                                fieldName != pfromNodeIdFieldName &&
                                fieldName != ptoNodeIdFieldName)
                            {
                                e.Attrs.Add(fieldName, csvReader[fieldName]);
                            }
                        }

                        Edges.Add(e.Id, e);

                        // add the endpoints of the new edge into the network
                        if (!Nodes.ContainsKey(e.FromNodeId))
                        {
                            Nodes.Add(e.FromNodeId, new Node() { Id = e.FromNodeId });
                        }
                        if (!Nodes.ContainsKey(e.ToNodeId))
                        {
                            Nodes.Add(e.ToNodeId, new Node() { Id = e.ToNodeId });
                        }
                        Nodes[e.FromNodeId].ConnectedEdgesId.Add(e.Id);
                        Nodes[e.ToNodeId].ConnectedEdgesId.Add(e.Id);
                    }
                    catch (FormatException ex)
                    {
                        Console.Write(ex);
                    }
                }
            }
        }

    }
}
