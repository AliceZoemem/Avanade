using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Plugin1
{
    public class CreaAttivita : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext executionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            IOrganizationService myService = serviceFactory.CreateOrganizationService(executionContext.UserId);

            if(executionContext.InputParameters.Contains("Target") && executionContext.InputParameters["Target"] is Entity)
            {
                Entity contactEntity = (Entity)executionContext.InputParameters["Target"];

                try
                {
                    Entity taskRecord = new Entity("task");
                    tracingService.Trace("Entita creata");
                    taskRecord.Attributes.Add("description", "Per favore richiamare questo nuovo contatto");
                    taskRecord.Attributes.Add("subject", "Creato da Plugin");
                    taskRecord.Attributes.Add("scheduledend", DateTime.Now.AddDays(2));

                    tracingService.Trace("Attributi collegati");
                    //new EntityReference("contact", contactEntity.Attributes(""));
                    taskRecord.Attributes.Add("regardingobjectid", contactEntity.ToEntityReference());
                    tracingService.Trace("Attivita creata");
                    Guid task = myService.Create(taskRecord);

                    Entity coda = new Entity("queueitem"); //Elementi cosa
                    coda.Attributes.Add("title", "Da richiamare");
                    coda.Attributes.Add("queueid", new EntityReference("queue", new Guid("f300d240-0c07-ec11-94ef-002248817c98")));
                    coda.Attributes.Add("objectid", new EntityReference("task", task));

                    tracingService.Trace("Coda creata");
                    myService.Create(coda);
                    tracingService.Trace("Fine");
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
