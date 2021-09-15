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
    public class DuplicateCheck : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext executionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            IOrganizationService myService = serviceFactory.CreateOrganizationService(executionContext.UserId);

            if (executionContext.InputParameters.Contains("Target") && executionContext.InputParameters["Target"] is Entity)
            {
                Entity ricetta = (Entity)executionContext.InputParameters["Target"];

                try
                {
                    if (ricetta.Attributes.Contains("cr23c_codicericetta"))
                    {
                        QueryExpression query = new QueryExpression("cr23c_ricetta");
                        query.Criteria.AddCondition("cr23c_codicericetta", ConditionOperator.Equal, ricetta.Attributes["cr23c_codicericetta"].ToString());
                        EntityCollection collection = myService.RetrieveMultiple(query);

                        if(collection.Entities.Count > 0)
                        {
                            tracingService.Trace("Entita duplicata");
                            throw new InvalidPluginExecutionException("Codice ricetta non disponibile. Entità duplicata");
                        }
                    }
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("ERRORE PLUGIN");
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
