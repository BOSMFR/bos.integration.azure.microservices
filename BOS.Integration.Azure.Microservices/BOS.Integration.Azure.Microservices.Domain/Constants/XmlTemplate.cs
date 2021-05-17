namespace BOS.Integration.Azure.Microservices.Domain.Constants
{
    public class XmlTemplate
    {
        // SKU
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

        // GoodsReceival
        public const string UpdateGoodsReceivalBody = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:apig=""urn:microsoft-dynamics-schemas/codeunit/APIGoodsReceival"" xmlns:x50=""urn:microsoft-dynamics-nav/xmlports/x50006"">
                                                           <soapenv:Header/>
                                                           <soapenv:Body>
                                                              <apig:ImportGoodReceiveXMLFile>
                                                                 <apig:xMLFile>
                                                        			{0}
                                                        		</apig:xMLFile>
                                                              </apig:ImportGoodReceiveXMLFile>
                                                           </soapenv:Body>
                                                        </soapenv:Envelope>";
    }
}
