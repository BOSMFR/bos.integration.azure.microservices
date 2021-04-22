namespace BOS.Integration.Azure.Microservices.Domain.Constants
{
    public class XmlTemplate
    {
        public const string ReadSkuBody = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ean=""urn:microsoft-dynamics-schemas/page/eannoupdate"">
                                               <soapenv:Header/>
                                               <soapenv:Body>
                                                  <ean:Read>
                                                     <ean:EANNo>{0}</ean:EANNo>
                                                  </ean:Read>
                                               </soapenv:Body>
                                            </soapenv:Envelope>";

        public const string UpdateSkuBody = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ean=""urn:microsoft-dynamics-schemas/page/eannoupdate"">
                                                  <soapenv:Header/>
                                                  <soapenv:Body>
                                                      <ean:Update>
                                                      <ean:EanNoUpdate>
                                                          <ean:Key>{0}</ean:Key>
                                                          <ean:EANNo>{1}</ean:EANNo>
                                                          <ean:PrimeCargoProductId>{2}</ean:PrimeCargoProductId>
                                                          <ean:WMSStatus>{3}</ean:WMSStatus>
                                                      </ean:EanNoUpdate>
                                                      </ean:Update>
                                                  </soapenv:Body>
                                              </soapenv:Envelope>";
    }
}
