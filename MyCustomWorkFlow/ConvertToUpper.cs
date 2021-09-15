using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCustomWorkFlow
{
    public class ConvertToUpper : CodeActivity
    {
        /*
         * Dobbiamo settare input e output
         *  [Input("Nome")]
         */
        [RequiredArgument]
        [Input("Testo")]
        public InArgument<String> text { get; set; }

        [Output("TestoUpper")]
        public OutArgument<String> textUpper { get; set; }
        protected override void Execute(CodeActivityContext executionContext)
        {
            string toConvert = this.text.Get<String>(executionContext);
            this.textUpper.Set(executionContext, toConvert.ToUpper());
        
        }
    }
}
