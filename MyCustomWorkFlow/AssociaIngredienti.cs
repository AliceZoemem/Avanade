using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCustomWorkFlow
{
    public class AssociaIngredienti : CodeActivity
    {
        protected override void Execute(CodeActivityContext executionContext)
        {
            ITracingService tracingService = executionContext.GetExtension<ITracingService>();
            IWorkflowContext workflowContext = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);

            /* 
             * 1) Recupero tutti gli ingredienti
             * 2) Associo alla nuova ricetta
                CODICE ALICE
                tracingService.Trace("CWA", "Inizio recupero ingredienti");
            
                QueryExpression query = new QueryExpression("cr23c_ingrediente_ricetta");
                query.ColumnSet.AddColumn("cr23c_ingredienteid");
                query.Criteria.AddCondition(new ConditionExpression(cr23c_Ricetta.PrimaryIdAttribute, ConditionOperator.Equal, this.ricettaPadre.Get(executionContext).Id));
                tracingService.Trace("CWA", "Lancio query");
                EntityCollection ingredienti = service.RetrieveMultiple(query);
                this.numero.Set(executionContext, ingredienti.Entities.Count);
                string rr = this.ricetta.Get<EntityReference>(executionContext).Id.ToString();
                EntityReferenceCollection er = new EntityReferenceCollection(ingredienti.Entities.Select(elem => new EntityReference("cr23c_ingrediente", new Guid(elem.Attributes["cr23c_ingredienteid"].ToString()))).ToList());
                service.Associate(cr23c_Ricetta.EntityLogicalName, 
                this.ricetta.Get<EntityReference>(executionContext).Id,
                new Relationship("cr23c_ingrediente_ricetta"),
                er);
                FINE CODICE ALICE
             */


            //OLD CODICE
            //Ricetta -> Tabella di transizione
            /*LinkEntity link1 =
                new LinkEntity(cr23c_Ricetta.EntityLogicalName, "cr23c_ingrediente_ricetta", cr23c_Ricetta.PrimaryIdAttribute, "cr23c_ricettaid", JoinOperator.Inner);
            //tabella di transizione-> Ingredienti
            LinkEntity link2= new LinkEntity("cr23c_ingrediente", "cr23c_ingrediente_ricetta", "crc_ingredienteid", "cr23c_ingredienteid", JoinOperator.Inner);

            link1.LinkCriteria = new FilterExpression();
            link1.LinkCriteria.AddCondition(new ConditionExpression(cr23c_Ricetta.PrimaryIdAttribute, ConditionOperator.Equal, this.ricettaPadre.Get(executionContext).Id));

            query.LinkEntities.Add(link1);
            query.LinkEntities.Add(link2);*/

            /*EntityReferenceCollection e = new EntityReferenceCollection();
            foreach(Entity ing in ingredienti.Entities)
            {
                
                e.Add(ing.ToEntityReference());
            }
            END OLD CODICE
            */

            tracingService.Trace("CWA: ", "Inizio recupero ingredienti");

            QueryExpression recIng = new QueryExpression("cr23c_ingrediente_ricetta");
            recIng.ColumnSet.AddColumn("cr23c_ingredienteid");
            recIng.Criteria.AddCondition("cr23c_ricettaid", ConditionOperator.Equal,
                ricettaPadre.Get<EntityReference>(executionContext).Id);
            EntityCollection ingredienti = service.RetrieveMultiple(recIng);

            this.numero.Set(executionContext, ingredienti.Entities.Count);

            EntityReferenceCollection erc = new EntityReferenceCollection();

            foreach (Entity ing in ingredienti.Entities)
            {
                erc.Add(new EntityReference("cr23c_ingrediente",
                    new Guid(ing.Attributes["cr23c_ingredienteid"].ToString())));
            }

            service.Associate(
                cr23c_Ricetta.EntityLogicalName,
                this.ricetta.Get<EntityReference>(executionContext).Id,
                new Relationship("cr23c_ingrediente_ricetta"),
                erc
             );

        }

        [Input("Ricetta")]
        [ReferenceTarget(cr23c_Ricetta.EntityLogicalName)]
        public InArgument<EntityReference> ricetta { get; set; }
        
        [Input("RicettaPadre")]
        [ReferenceTarget(cr23c_Ricetta.EntityLogicalName)]
        public InArgument<EntityReference> ricettaPadre { get; set; }
        
        [Output("numeroIngredienti")]
        public OutArgument<int> numero{ get; set; }



    }
}
