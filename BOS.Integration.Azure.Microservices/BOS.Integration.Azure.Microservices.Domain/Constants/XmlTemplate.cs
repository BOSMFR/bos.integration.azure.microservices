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
        public const string UpdateGoodsReceivalBody = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:apig=""urn:microsoft-dynamics-schemas/codeunit/ApiGoodsReceival"">
                                                            <soapenv:Header/>
                                                            <soapenv:Body>
                                                              <apig:GoodsReceivalCreated>
                                                                 <apig:wMSDocumentNo>{0}</apig:wMSDocumentNo>
                                                                 <apig:wMSDocumentLineNo>{1}</apig:wMSDocumentLineNo>
                                                                 <apig:wMSGoodsReceivalID>{2}</apig:wMSGoodsReceivalID>
                                                                 <apig:wMSGoodsReceiva_LineID>{3}</apig:wMSGoodsReceiva_LineID>
                                                              </apig:GoodsReceivalCreated>
                                                           </soapenv:Body>
                                                        </soapenv:Envelope>";
    }
}
