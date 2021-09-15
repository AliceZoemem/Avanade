using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Plugin1
{
    public class HelloWorld : IPlugin
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

            /*
             * account.fullname -> Early Bound
             * account["fullname"] -> Late Bound 
             */
            /*
             * Target è l' entità entità su cui sto registrando il mio plugin
             * in questo caso sarà comunque 'Contatto'
             * 
             * Il plugin non è direttamente collegato ad una sola entità
             * lo posso registrare per diverse entità
             */
            if (executionContext.InputParameters.Contains("Target") && executionContext.InputParameters["Target"] is Entity)
            {
                Entity contactEntity = (Entity)executionContext.InputParameters["Target"];
                try
                {
                    string name = string.Empty;
                    string telefono = string.Empty;
                    if (contactEntity.Attributes.Contains("name") && !contactEntity.Attributes.Contains("telephone1"))
                    {
                        name = contactEntity.Attributes["name"].ToString();
                        contactEntity.Attributes.Add("description", "Hello World Mr./Mss " + name);
                    }   
                    if (contactEntity.Attributes.Contains("telephone1") && !contactEntity.Attributes.Contains("name"))
                    {
                        telefono = contactEntity.Attributes["telephone1"].ToString();
                        contactEntity.Attributes.Add("description", "Hello World Mr./Mss " + telefono);
                    }
                    if(contactEntity.Attributes.Contains("telephone1") && contactEntity.Attributes.Contains("name"))
                    {
                        name = contactEntity.Attributes["name"].ToString();
                        telefono = contactEntity.Attributes["telephone1"].ToString();
                        contactEntity.Attributes.Add("description", "Hello World Mr./Mss " + telefono + " " + name);
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
