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
    public class UseActionsFromPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext executionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService myService = serviceFactory.CreateOrganizationService(executionContext.UserId);
            try
            {
                string padre = "";
                if (executionContext.InputParameters.Contains("cr23c_ricettapadre"))
                {
                    Entity padreEnt = (Entity)executionContext.InputParameters["cr23c_ricettapadre"];
                    tracingService.Trace("Padre ", padreEnt);
                    padre = padreEnt.Id.ToString();
                }
                else {
                    padre = "8725c256-54dc-46cb-beeb-a0067555a1ce";
                }

                QueryExpression recIng = new QueryExpression("cr23c_ingrediente_ricetta");
                recIng.ColumnSet.AddColumn("cr23c_ingredienteid");
                tracingService.Trace("Padre ", padre);
                recIng.Criteria.AddCondition("cr23c_ricettaid", ConditionOperator.Equal,
                    padre);
                EntityCollection ingredienti = myService.RetrieveMultiple(recIng);

                OrganizationRequest customAction = new OrganizationRequest("new_azionedaplugin1");
                customAction["numing"] = ingredienti.Entities.Count();

                myService.Execute(customAction);
                
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
