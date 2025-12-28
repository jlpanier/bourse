using System.Net.Security;
using System.Xml.Serialization;

namespace Business
{
    [Serializable()]
    [XmlRoot("data")]
    public class XmlData
    {
        [XmlElement("shares")]
        public List<XmlShare>? Shares { get; set; }
    }

    public class XmlShare
    {
        [XmlElement("code")]
        public string Code { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("url")]
        public string Url { get; set; }

        [XmlElement("cac40")]
        public bool CAC40 { get; set; }

        [XmlElement("datemaj")]
        public DateTime DateMaj{ get; set; }

        [XmlElement("prices")]
        public List<XmlPrice> Prices { get; set; }
    }

    public class XmlPrice
    {
        [XmlElement("amount")]
        public double Amount { get; set; }

        [XmlElement("dateon")]
        public DateTime DateOn { get; set; }

        [XmlElement("risk")]
        public string Risk { get; set; }

        [XmlElement("consensus")]
        public double Concensus { get; set; }

        [XmlElement("rendement")]
        public double Rendement { get; set; }

        [XmlElement("datemaj")]
        public DateTime DateMaj { get; set; }

    }
}
