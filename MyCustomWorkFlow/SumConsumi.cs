using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCustomWorkFlow
{
    public class SumConsumi : CodeActivity
    {

        [Input("KgLuppolo")]
        public InArgument<decimal> kgLuppolo { get; set; }
        
        [Input("KgMalto")]
        public InArgument<decimal> kgMalto { get; set; } 

        [Input("RicettaIn")]
        [ReferenceTarget(cr23c_Ricetta.EntityLogicalName)]
        public InArgument<EntityReference> ricettaIn { get; set; }
        
        [Output("Somma")]
        public OutArgument<decimal> somma { get; set; } 
        
        [Output("RicettaOut")]
        [ReferenceTarget(cr23c_Ricetta.EntityLogicalName)]
        public OutArgument<EntityReference> ricettaOut { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();
            IWorkflowContext workflowContext = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);

            this.somma.Set(executionContext, this.kgLuppolo.Get<decimal>(executionContext) + this.kgMalto.Get<decimal>(executionContext));
            //Early Bound
            cr23c_Ricetta ricettaEntity = new cr23c_Ricetta();

            this.ricettaOut.Set(executionContext, this.ricettaIn.Get<EntityReference>(executionContext));

        }
    }
}
