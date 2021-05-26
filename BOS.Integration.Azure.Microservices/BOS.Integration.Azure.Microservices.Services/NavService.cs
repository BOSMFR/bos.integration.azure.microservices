using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Domain.Constants;
using BOS.Integration.Azure.Microservices.Domain.DTOs.GoodsReceival;
using BOS.Integration.Azure.Microservices.Domain.DTOs.PickOrder;
using BOS.Integration.Azure.Microservices.Infrastructure.Configuration;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using BOS.Integration.Azure.Microservices.Services.Helpers;
using System;
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

        public async Task<ActionExecutionResult> UpdateGoodsReceivalIntoNavAsync(PrimeCargoGoodsReceivalResponseDTO primeCargoResponse)
        {
            var actionResult = new ActionExecutionResult();

            try
            { 
                string updateGoodsReceivalBody = GetXmlBody(primeCargoResponse, XmlTemplate.UpdateGoodsReceivalBody);

                if (string.IsNullOrEmpty(updateGoodsReceivalBody))
                {
                    actionResult.Error = "Could not generate an xml template";
                    return actionResult;
                }

                var updateGoodsReceivalResult = await this.httpService.PostSoapAsync(configuration.NavSettings.GoodsReceival.Url, updateGoodsReceivalBody, configuration.NavSettings.GoodsReceival.SoapAction,
                                                                            configuration.NavSettings.UserName, configuration.NavSettings.Password);

                if (!updateGoodsReceivalResult.Succeeded)
                {
                    actionResult.Error = $"Could not update a GoodsReceival with id {primeCargoResponse.GoodsReceivalId}";
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

        public async Task<ActionExecutionResult> UpdatePickOrderIntoNavAsync(PrimeCargoPickOrderResponseDTO primeCargoResponse)
        {
            var actionResult = new ActionExecutionResult();

            try
            {
                string updatePickOrderBody = GetXmlBody(primeCargoResponse, XmlTemplate.UpdatePickOrderBody);

                if (string.IsNullOrEmpty(updatePickOrderBody))
                {
                    actionResult.Error = "Could not generate an xml template";
                    return actionResult;
                }

                var updatePickOrderResult = await this.httpService.PostSoapAsync(configuration.NavSettings.PickOrder.Url, updatePickOrderBody, configuration.NavSettings.PickOrder.SoapAction,
                                                                            configuration.NavSettings.UserName, configuration.NavSettings.Password);

                if (!updatePickOrderResult.Succeeded)
                {
                    actionResult.Error = $"Could not update a PickOrder with id {primeCargoResponse.OrderNumber}";
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

        private string GetXmlBody<T>(T primeCargoResponse, string xmlBody)
        {
            try
            {
                string xmlContent = XmlHelper.ConvertItemToXmlString(primeCargoResponse);

                return string.IsNullOrEmpty(xmlContent) ? null : string.Format(xmlBody, xmlContent);
            }
            catch
            {
                return null;
            }
        }
    }
}
