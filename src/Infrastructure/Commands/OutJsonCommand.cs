using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using PowerShell.Infrastructure.Utilities;
using Formatting = Newtonsoft.Json.Formatting;

namespace PowerShell.Infrastructure.Commands
{
    [Cmdlet(VerbsData.Out, "Json")]
    public class OutJsonCommand : AbstractPSCmdlet
    {
        [Parameter(ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ValueFromRemainingArguments = true)]
        public PSObject InputObject { get; set; }

        [Parameter]
        public SwitchParameter Schema { get; set; }

        /// <summary>
        /// When overridden in the derived class, performs execution
        /// of the command.
        /// </summary>
        /// <exception cref="T:System.Exception">
        /// This method is overridden in the implementation of
        /// individual Cmdlets, and can throw literally any exception.
        /// </exception>
        protected override void ProcessRecord()
        {
            if (InputObject?.BaseObject != null)
            {

                if (Schema)
                {
                    JSchemaGenerator generator = new JSchemaGenerator
                    {
                        SchemaIdGenerationHandling = SchemaIdGenerationHandling.FullTypeName,
                        SchemaPropertyOrderHandling = SchemaPropertyOrderHandling.Default,
                        SchemaLocationHandling = SchemaLocationHandling.Inline,
                        SchemaReferenceHandling = SchemaReferenceHandling.All,
                        GenerationProviders = { new SchemaGenerationProvider() }

                    };
                    JSchema schema = generator.Generate(InputObject.BaseObject.GetType());
                    WriteObject(schema.ToJSchemaString());

                }
                else
                {
                    string json = JsonConvert.SerializeObject(InputObject.BaseObject, Formatting.Indented);
                    WriteObject(json);
                }

            }
        }

        private class SchemaGenerationProvider : JSchemaGenerationProvider
        {
            /// <summary>
            /// Gets a <see cref="T:Newtonsoft.Json.Schema.JSchema" /> for a <see cref="T:System.Type" />.
            /// </summary>
            /// <param name="context">The <see cref="T:System.Type" /> and associated information used to generate a <see cref="T:Newtonsoft.Json.Schema.JSchema" />.</param>
            /// <returns>The generated <see cref="T:Newtonsoft.Json.Schema.JSchema" /> or <c>null</c> if type should not have a customized schema.</returns>
            public override JSchema GetSchema(JSchemaTypeGenerationContext context)
            {
                string typeName = context.ObjectType.Name;
                JSchema generatedSchema = context.Generator.Generate(context.ObjectType);
                generatedSchema.AllowAdditionalProperties = true;
                generatedSchema.AllowAdditionalItems = true;
                generatedSchema.Required.Clear();
                foreach (KeyValuePair<string, JSchema> kvp in generatedSchema.Properties)
                {
                    kvp.Value.Description = $"Represents the {kvp.Key} property.";
                    if (kvp.Value.Type == JSchemaType.Boolean)
                    {
                        kvp.Value.Default = false;
                    }
                }
                //JSchema schema = new JSchema
                //{
                //    Type = JSchemaType.Object,
                //    SchemaVersion = new Uri("http://json-schema.org/draft-06/schema#", UriKind.Absolute),
                //    Description = $"Json Schema for the {typeName} CLR object.",
                //    Properties =
                //    {
                //        { "$ref", new JSchema { Type = JSchemaType.String , Reference = new Uri($"/definitions/{typeName}", UriKind.Relative)} }
                //    }
                //};
                //schema.ExtensionData["definitions"] = new JSchema
                //{
                //    Type = JSchemaType.Object,
                //    Properties =
                //    {
                //        { typeName, generatedSchema }
                //    }
                //};
                return generatedSchema;
            }


        }
    }
}
