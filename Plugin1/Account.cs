using System;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Plugin1
{
    public class Account : IPlugin
    {

        public void Execute(IServiceProvider serviceProvider)
        {
            /* IServiceProvider ci permette di accedere
             * all' executionContext
             * tracingService
             * ServiceFactory
             */
            IPluginExecutionContext executionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            IOrganizationService myService = serviceFactory.CreateOrganizationService(executionContext.UserId);

            if (executionContext.InputParameters.Contains("Target") && executionContext.InputParameters["Target"] is Entity)
            {
                Entity contactEntity = (Entity)executionContext.InputParameters["Target"];

                if (contactEntity.Attributes.Contains("telephone1"))
                {
                    contactEntity["telephone1"] = contactEntity["telephone1"].ToString().Replace(" ", "");
                    string pattern = @"^\s*(?:\+?(\d{1,3}))?[-. (]*(\d{3})[-. )]*(\d{3})[-. ]*(\d{4})(?: *x(\d+))?\s*$";
                    Regex rgx = new Regex(pattern);
                    Match match = rgx.Match(contactEntity["telephone1"].ToString());
                    tracingService.Trace("tel " + contactEntity["telephone1"]);
                    if (match.Success)
                    {
                        tracingService.Trace("Telephone correct ");
                        contactEntity["telephone1"] = "(" + contactEntity["telephone1"].ToString().Substring(0, 5) + ") " + contactEntity["telephone1"].ToString().Substring(5, contactEntity["telephone1"].ToString().Length - 5);
                    }
                    else
                    {
                        throw new InvalidPluginExecutionException("Formato telefono invalido");
                    }
                }
            }
        }
    }
}
