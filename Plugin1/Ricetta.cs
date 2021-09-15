using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Plugin1
{
    public class Ricetta : IPlugin
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
                try
                {
                    if (contactEntity.Attributes.Contains("cr23c_codicericetta"))
                    {
                        contactEntity["cr23c_codicericetta"] = contactEntity["cr23c_codicericetta"].ToString().ToUpper();
                        //myService.Update(contactEntity);
                        tracingService.Trace("Finish ");
                    }
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("ERRORE PLUGIN " + ex);
                }
                catch (Exception e)
                {
                    tracingService.Trace("CUSTOM PLUGIN ERROR: {0}", e.ToString());
                    throw;
                }
            }
        }
    }
}
