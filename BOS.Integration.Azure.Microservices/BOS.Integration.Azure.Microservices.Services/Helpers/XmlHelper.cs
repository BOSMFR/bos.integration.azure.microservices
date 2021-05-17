using System;
using System.IO;
using System.Xml.Serialization;

namespace BOS.Integration.Azure.Microservices.Services.Helpers
{
    public static class XmlHelper
    {
        private const string RootAttribute = " xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"";
        private const string ElementAttribute = " xsi:nil=\"true\"";

        public static string ConvertItemToXmlString<T>(T item)
        {
            try
            {
                using (var stringwriter = new StringWriter())
                {
                    var serializer = new XmlSerializer(item.GetType());

                    serializer.Serialize(stringwriter, item);

                    string xmlStr = stringwriter.ToString().Replace(RootAttribute, string.Empty).Replace(ElementAttribute, string.Empty);

                    return xmlStr.Substring(xmlStr.IndexOf(Environment.NewLine) + 2);
                }
            }
            catch (Exception)
            {
                return null;
            }            
        }
    }
}
