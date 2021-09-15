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
    public class RecipeIngredient : IPlugin
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
                   
                    if (contactEntity.Attributes.Contains("cr23c_ricettapadre"))
                    {
                        QueryExpression query = new QueryExpression("cr23c_ingrediente_ricetta");
                        query.Criteria.AddCondition("cr23c_ricettaid", ConditionOperator.Equal, contactEntity.Attributes["cr23c_ricettapadre"]);
                        EntityCollection collection = myService.RetrieveMultiple(query);
                        //myService.Update(contactEntity);
                        foreach (Entity e in collection.Entities)
                        {
                            Entity pivotRecord = new Entity("cr23c_ingrediente_ricetta");
                            pivotRecord.Attributes.Add("cr23c_ricettaid", contactEntity.ToEntityReference());
                            pivotRecord.Attributes.Add("cr23c_ingredienteid", e.Attributes["cr23c_ingredienteid"]);
                            tracingService.Trace("Ingrediente " + e.Attributes["cr23c_ingredienteid"]);
                            Guid ing_rec = myService.Create(pivotRecord);
                            tracingService.Trace("Creato ingrediente da padre " + ing_rec.ToString());
                        }

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
