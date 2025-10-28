#region

using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

#endregion

namespace Common.FeatureData
{
    public class ConstellationsData
    {
        private static ConstellationsData _instance;
        public ConstellationNodes Data { get; private set; }

        private ConstellationsData()
        {
            // Load the XML document and deserialize
            var serializer = new XmlSerializer(typeof(ConstellationNodes));
            using (var reader = new StreamReader("FeatureData/constellationsData.xml"))
            {
                Data = (ConstellationNodes)serializer.Deserialize(reader);
            }
        }

        public static ConstellationsData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ConstellationsData();
                }

                return _instance;
            }
        }
    }

    [XmlRoot("Nodes")]
    public class ConstellationNodes
    {
        [XmlElement("Node")] public List<ConstellationNode> NodeList { get; set; }
    }

    public class ConstellationNode
    {
        [XmlAttribute("constellation")] public int Constellation { get; set; }

        [XmlAttribute("large")] public bool Large { get; set; }

        [XmlAttribute("id")] public int Id { get; set; }

        [XmlAttribute("name")] public string Name { get; set; }

        [XmlAttribute("row")] public int Row { get; set; }

        public string Description { get; set; }
    }
}