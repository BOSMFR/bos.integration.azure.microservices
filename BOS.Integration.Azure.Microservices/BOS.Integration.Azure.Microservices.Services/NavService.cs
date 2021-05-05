using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival;
using BOS.Integration.Azure.Microservices.Infrastructure.Configuration;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace BOS.Integration.Azure.Microservices.Services
{
    public class NavService : INavService
    {
        private readonly IConfigurationManager configuration;
        private readonly IHttpService httpService;

        public NavService(
            IConfigurationManager configuration, 
            IHttpService httpService)
        {
            this.configuration = configuration;
            this.httpService = httpService;
        }

        public async Task<ActionExecutionResult> UpdateSkuIntoNavAsync(string eanNo, string productId)
        {
            var actionResult = new ActionExecutionResult();

            try
            {
                // Get sku key by eanNo
                string readSkuBody = GetXmlBody(eanNo);

                if (string.IsNullOrEmpty(readSkuBody))
                {
                    actionResult.Error = "Could not read xml template";
                    return actionResult;
                }

                var getSkuResult = await this.httpService.PostSoapAsync(configuration.NavSettings.Sku.Url, readSkuBody, configuration.NavSettings.Sku.SoapAction, 
                                                                            configuration.NavSettings.UserName, configuration.NavSettings.Password);

                if (!getSkuResult.Succeeded)
                {
                    actionResult.Error = getSkuResult.Error;
                    return actionResult;
                }

                string key = GetKeyFromXml(getSkuResult.Content);

                if (string.IsNullOrEmpty(key))
                {
                    actionResult.Error = "Could not read a key property from the XML document";
                    return actionResult;
                }

                // Update the sku into Nav
                string wmsStatus = productId == "0" ? NavWmsStatus.Failed : NavWmsStatus.Successfully;

                string updateSkuBody = GetXmlBody(key, eanNo, productId, wmsStatus);

                if (string.IsNullOrEmpty(updateSkuBody))
                {
                    actionResult.Error = "Could not read xml template";
                    return actionResult;
                }

                var updateSkuResult = await this.httpService.PostSoapAsync(configuration.NavSettings.Sku.Url, updateSkuBody, configuration.NavSettings.Sku.SoapAction,
                                                                            configuration.NavSettings.UserName, configuration.NavSettings.Password);

                if (!updateSkuResult.Succeeded)
                {
                    actionResult.Error = updateSkuResult.Error;
                    return actionResult;
                }

                actionResult.Succeeded = true;

                return actionResult;
            }
            catch (Exception ex)
            {
                actionResult.Error = ex.Message;
                return actionResult;
            }
        }

        public async Task<ActionExecutionResult> UpdateGoodsReceivalIntoNavAsync(ResponseMessage<PrimeCargoGoodsReceivalResponseDTO> primeCargoResponse)
        {
            var actionResult = new ActionExecutionResult();

            try
            {
                // Select request data from PrimeCargoGoodsReceivalResponse object
                var requestLines = primeCargoResponse.ResponseObject.Lines.Select(x => new GoodsReceivalNavRequestDTO
                {
                    WMSGoodsReceivalId = primeCargoResponse.ResponseObject.GoodsReceivalId ?? default,
                    WMSDocumentNo = primeCargoResponse.ResponseObject.ReceivalNumber,
                    WMSGoodsReceivalLineId = x.GoodsReceivalLineId ?? default,
                    VMSDocumentLineNo = x.ExtReference
                }).ToList();

                // Update GoodsReceival purchase lines
                foreach (var requestLine in requestLines)
                {
                    string updateGoodsReceivalBody = GetXmlBody(requestLine);

                    if (string.IsNullOrEmpty(updateGoodsReceivalBody))
                    {
                        actionResult.Error = "Could not read xml template";
                        return actionResult;
                    }

                    var updateSkuResult = await this.httpService.PostSoapAsync(configuration.NavSettings.GoodsReceival.Url, updateGoodsReceivalBody, configuration.NavSettings.GoodsReceival.SoapAction,
                                                                                configuration.NavSettings.UserName, configuration.NavSettings.Password);

                    if (!updateSkuResult.Succeeded)
                    {
                        actionResult.Error = $"Could not update a GoodsReceival line with id {requestLine.WMSGoodsReceivalLineId}";
                        return actionResult;
                    }
                }

                actionResult.Succeeded = true;

                return actionResult;
            }
            catch (Exception ex)
            {
                actionResult.Error = ex.Message;
                return actionResult;
            }
        }
        

        private string GetKeyFromXml(string xmlResponse)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlResponse);

                return ProcessNodes(doc.ChildNodes, "Key");
            }
            catch
            {
                return null;
            }
        }

        private string ProcessNodes(XmlNodeList nodes, string searProperty)
        {
            string key = null;

            foreach (XmlNode childNode in nodes)
            {
                if (childNode.Name == searProperty)
                {
                    return childNode.InnerText;
                }

                if (childNode.ChildNodes.Count > 0)
                {
                    key = ProcessNodes(childNode.ChildNodes, searProperty);
                }
            }

            return key;
        }

        private string GetXmlBody(string key, string eanNo, string productId, string wmsStatus)
        {
            try
            {
                return string.Format(XmlTemplate.UpdateSkuBody, key, eanNo, productId, wmsStatus);
            }
            catch
            {
                return null;
            }
        }

        private string GetXmlBody(string eanNo)
        {
            try
            {
                return string.Format(XmlTemplate.ReadSkuBody, eanNo);
            }
            catch
            {
                return null;
            }
        }

        private string GetXmlBody(GoodsReceivalNavRequestDTO requestLine)
        {
            try
            {
                return string.Format(XmlTemplate.UpdateGoodsReceivalBody, requestLine.WMSDocumentNo, requestLine.VMSDocumentLineNo, requestLine.WMSGoodsReceivalId, requestLine.WMSGoodsReceivalLineId);
            }
            catch
            {
                return null;
            }
        }
    }
}
