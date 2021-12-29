using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BMC.StreamProcessor
{
    public class SchemaConverter
    {
        public static DataTable ExpandoToDataTable(List<dynamic> data)
        {
            int counter = 0;
            DataTable dt = new DataTable("data");
            //generate columns
            foreach (dynamic item in data)
            {
                if (item is ExpandoObject)
                {
                    if (counter <= 0)
                    {
                        foreach (var dataitem in (IDictionary<string, object>)item)
                        {
                            dt.Columns.Add(dataitem.Key);
                        }
                    }

                    {
                        var NewItem = dt.NewRow();
                        foreach (var dataitem in (IDictionary<string, object>)item)
                        {
                            NewItem[dataitem.Key] = dataitem.Value;
                        }
                        dt.Rows.Add(NewItem);
                    }
                    counter++;
                }

            }
            dt.AcceptChanges();
            return dt;
        }
        public static dynamic JsonToExpando(string JsonStr)
        {
            return JsonConvert.DeserializeObject<ExpandoObject>(JsonStr, new ExpandoObjectConverter());
        }
      

        public static object GetSample(Type sample)
        {
            var dict = new Dictionary<Type, object> {
            { typeof(String), "sample" },
            { typeof(int), 123 },
            { typeof(double), 12.3 },
            { typeof(DateTime), DateTime.MinValue },
            { typeof(float), 12.3f },
            { typeof(decimal), 12.3 },
            { typeof(Boolean), true },
            { typeof(char), 'a' },
            { typeof(Int64), 0 },
        };

            return dict[sample];
        }
       
        public static string ExpandoToCsv(dynamic node)
        {
            bool isColumnGenerated = false;
            StringBuilder sb = new StringBuilder();
            string ColNames = string.Empty;
            if (node is List<dynamic>)
            {
                foreach (var element in (List<dynamic>)node)
                {
                    if (element is ExpandoObject)
                    {
                        var counter = 0;
                        foreach (var property in (IDictionary<String, Object>)element)
                        {
                            if (!isColumnGenerated)
                            {
                                if (counter > 0) ColNames += ";";
                                ColNames += property.Key.Replace(" ", "_");
                            }
                            if (counter > 0)
                                sb.Append(";");
                            sb.Append(property.Value);
                            counter++;
                        }

                        sb.Append(Environment.NewLine);
                        isColumnGenerated = true;
                    }
                }
                if (!string.IsNullOrEmpty(ColNames))
                {
                    sb.Insert(0, ColNames + Environment.NewLine);
                }
            }
            else
            {
                if (node is ExpandoObject)
                {
                    var counter = 0;
                    foreach (var property in (IDictionary<String, Object>)node)
                    {
                        if (!isColumnGenerated)
                        {
                            if (counter > 0) ColNames += ";";
                            ColNames += property.Key.Replace(" ", "_");
                        }
                        if (counter > 0)
                            sb.Append(";");
                        sb.Append(property.Value);
                        counter++;
                    }

                    sb.Append(Environment.NewLine);
                    isColumnGenerated = true;
                    if (!string.IsNullOrEmpty(ColNames))
                    {
                        sb.Insert(0, ColNames + Environment.NewLine);
                    }
                }
            }

            return sb.ToString();
        }
        public static XElement ExpandoToXML(dynamic node, String nodeName)
        {
            XElement xmlNode = new XElement(nodeName);
            if (node is List<dynamic> || node is List<Dictionary<string, object>>)
            {
                if (node is List<dynamic>)
                {
                    foreach (var element in (List<dynamic>)node)
                        xmlNode.Add(ExpandoToXML(element as ExpandoObject, "row"));
                }
                else
                {
                    foreach (var element in (List<Dictionary<string, object>>)node)
                        xmlNode.Add(ExpandoToXML(element, "row"));
                }
            }
            else
            {
                foreach (var property in (IDictionary<String, Object>)node)
                {

                    if (property.Value.GetType() == typeof(ExpandoObject))
                        xmlNode.Add(ExpandoToXML(property.Value, property.Key));

                    else
                        if (property.Value.GetType() == typeof(List<dynamic>))
                        foreach (var element in (List<dynamic>)property.Value)
                            xmlNode.Add(ExpandoToXML(element, property.Key));
                    else
                        xmlNode.Add(new XElement(property.Key, property.Value));
                }
            }
            return xmlNode;
        }
       
        public static bool AreExpandoStructureEquals(ExpandoObject obj1, ExpandoObject obj2)
        {
            var obj1AsColl = (ICollection<KeyValuePair<string, object>>)obj1;
            var obj2AsDict = (IDictionary<string, object>)obj2;

            // Make sure they have the same number of properties
            if (obj1AsColl.Count != obj2AsDict.Count)
                return false;

            foreach (var pair in obj1AsColl)
            {
                if (!obj2AsDict.ContainsKey(pair.Key))
                {
                    return false;
                }
                else
                {
                    //if (obj2AsDict[pair.Key].GetType() != pair.Value.GetType())
                    //    return false;
                }

            }

            // Everything matches
            return true;
        }
        public static dynamic ConvertToExpandoObjects(IEnumerable<KeyValuePair<string, object>> pairs)
        {
            IDictionary<string, object> result = new ExpandoObject();
            foreach (var pair in pairs)
                result.Add(pair.Key, pair.Value);
            return result;
        }
    }
}
