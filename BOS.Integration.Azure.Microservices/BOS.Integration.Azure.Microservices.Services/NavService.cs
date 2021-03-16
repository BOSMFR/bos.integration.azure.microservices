﻿using BOS.Integration.Azure.Microservices.Domain;
using BOS.Integration.Azure.Microservices.Infrastructure.Configuration;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using System;
using System.IO;
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

                var getSkuResult = await this.httpService.PostSoapAsync(configuration.NavSettings.Url, readSkuBody, configuration.NavSettings.SoapAction, 
                                                                            configuration.NavSettings.UserName, configuration.NavSettings.Password);

                if (!getSkuResult.Succeeded)
                {
                    actionResult.Error = getSkuResult.Error;
                    return actionResult;
                }

                string key = this.GetKeyFromXml(getSkuResult.Content);

                if (string.IsNullOrEmpty(key))
                {
                    actionResult.Error = "Could not read a key property from the XML document";
                    return actionResult;
                }

                // Update the sku into Nav
                string updateSkuBody = GetXmlBody(key, eanNo, productId);

                if (string.IsNullOrEmpty(updateSkuBody))
                {
                    actionResult.Error = "Could not read xml template";
                    return actionResult;
                }

                var updateSkuResult = await this.httpService.PostSoapAsync(configuration.NavSettings.Url, updateSkuBody, configuration.NavSettings.SoapAction,
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

        private string GetXmlBody(string key, string eanNo, string productId)
        {
            try
            {
                string fileText = File.ReadAllText("NavXmlTemplates\\UpdateSkuBody.xml");

                return string.Format(fileText, key, eanNo, productId);
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
                string fileText = File.ReadAllText("NavXmlTemplates\\ReadSkuBody.xml");

                return string.Format(fileText, eanNo);
            }
            catch
            {
                return null;
            }
        }
    }
}
