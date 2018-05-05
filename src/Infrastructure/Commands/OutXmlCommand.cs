using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using PowerShell.Infrastructure.Utilities;

namespace PowerShell.Infrastructure.Commands
{
    [Cmdlet(VerbsData.Out, "Xml")]
    public class OutXmlCommand : AbstractPSCmdlet
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
                XmlSerializer xsSubmit = new XmlSerializer(InputObject.BaseObject.GetType());


                using (var sw = new StringWriter())
                {
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true
                    };

                    using (XmlWriter writer = XmlWriter.Create(sw, settings))
                    {

                        xsSubmit.Serialize(writer, InputObject.BaseObject);
                        string xml = Cleanse(sw.ToString()).ToString();

                        if (Schema)
                        {
                            WriteXmlSchemaObject(xml);
                        }
                        else
                        {
                            WriteObject(xml);
                        }
                    }
                }
            }
        }

        private XmlSchema _schema;

        private void WriteXmlSchemaObject(string xml)
        {
            using (XmlTextReader reader = new XmlTextReader(new StringReader(xml)))
            {
                XmlSchemaInference inference = new XmlSchemaInference();
                XmlSchemaSet set = inference.InferSchema(reader);
                set.Compile();
                _schema = new XmlSchema();
                foreach (XmlSchema schema in set.Schemas())
                {
                    XmlSchemaOrganizer organizer = XmlSchemaOrganizer.From(schema);
                    XmlSchemaBuilder builder = new XmlSchemaBuilder();

                    builder.AddGardenOfEdenItem(schema.Elements.First<XmlSchemaElement>());


                    WriteObject(builder.ToSchema().ToXmlString());
                }
            }
        }

        private static XElement Cleanse(string xml)
        {
            XElement element = XElement.Parse(xml);

            RemoveAttribute(element, "xsd");
            RemoveAttribute(element, "xsi");

            return element;
        }

        public enum XmlSchemaGroupType
        {
            All,
            Sequence,
            Choice
        }

        public class XmlSchemaBuilder
        {
            private readonly XmlSchema _schema;

            private List<XmlSchemaGroup> _groups;
            private List<XmlSchemaAttributeGroup> _attributeGroups;
            private List<XmlSchemaSimpleType> _simpleTypes;
            private List<XmlSchemaElement> _elements;

            private Dictionary<XmlSchemaElement, XmlSchemaComplexType> _garden;

            public XmlSchemaBuilder()
            {
                _schema = new XmlSchema
                {
                    ElementFormDefault = XmlSchemaForm.Qualified,
                    AttributeFormDefault = XmlSchemaForm.Qualified
                };
                _groups = new List<XmlSchemaGroup>();
                _attributeGroups = new List<XmlSchemaAttributeGroup>();
                _elements = new List<XmlSchemaElement>();
                _simpleTypes = new List<XmlSchemaSimpleType>();
                _garden = new Dictionary<XmlSchemaElement, XmlSchemaComplexType>();
            }

            public XmlSchema ToSchema()
            {
                foreach (XmlSchemaGroup group in _groups)
                {
                    _schema.Items.Add(group);
                }

                foreach (XmlSchemaAttributeGroup attributeGroup in _attributeGroups)
                {
                    _schema.Items.Add(attributeGroup);
                }

                foreach (XmlSchemaSimpleType simpleType in _simpleTypes)
                {
                    _schema.Items.Add(simpleType);
                }

                foreach (XmlSchemaElement element in _elements)
                {
                    _schema.Items.Add(element);
                }
                foreach (KeyValuePair<XmlSchemaElement, XmlSchemaComplexType> kvp in _garden)
                {
                    _schema.Items.Add(kvp.Key);
                    _schema.Items.Add(kvp.Value);
                }

                return _schema;
            }


            public void AddGardenOfEdenItem(XmlSchemaElement element)
            {
                string baseName = element.Name;
                string complexName = baseName + "Type";
                string simpleName = baseName + "Kind";
                string groupName = baseName + "Group";

                XmlSchemaComplexType complexType = new XmlSchemaComplexType {Name = complexName};
                XmlSchemaSequence sequence = new XmlSchemaSequence();

                

                XmlSchemaGroup group = new XmlSchemaGroup();
                group.Name = groupName;
                group.Particle = sequence;

                foreach (var element1 in element.GetElementComplexTypeItems())
                {
                    element1.AddDocumentation($"Represents the {element1.Name} property.");
                    _elements.Add(element1);
                    XmlSchemaElement refElement = new XmlSchemaElement();
                    refElement.RefName = new XmlQualifiedName(element1.Name);
                    sequence.Items.Add(refElement);
                }

                _groups.Add(group);

                complexType.Particle = new XmlSchemaGroupRef()
                {
                    RefName = new XmlQualifiedName(groupName)
                };

                XmlSchemaElement newElement = new XmlSchemaElement
                {
                    Name = baseName,
                    SchemaTypeName = new XmlQualifiedName(complexName)
                };

                newElement.AddDocumentation($"Represents the {baseName} object.");

                if (!_garden.ContainsKey(newElement))
                {
                    _garden[newElement] = complexType;
                }
            }

        }


        public XmlSchemaComplexType CreateComplexType(string name, XmlSchemaGroupType type, decimal minOccurs = 1, decimal maxOccurs = 1, params string[] elementRefs)
        {
            XmlSchemaComplexType complexType = new XmlSchemaComplexType {Name = name};
            complexType.BaseXmlSchemaType.Name = name.Replace("Type", string.Empty);

            XmlSchemaGroupBase groupBase = null;

            XmlSchemaGroup group = new XmlSchemaGroup();
            group.Name = name + "Group";

            switch (type)
            {
                case XmlSchemaGroupType.All:
                    XmlSchemaAll all = new XmlSchemaAll();
                    all.MinOccurs = minOccurs;
                    all.MaxOccurs = maxOccurs;
                    groupBase = all;
                    break;
                case XmlSchemaGroupType.Sequence:
                    XmlSchemaSequence sequence = new XmlSchemaSequence();
                    sequence.MinOccurs = minOccurs;
                    sequence.MaxOccurs = maxOccurs;
                    groupBase = sequence;
                    break;
                case XmlSchemaGroupType.Choice:
                    XmlSchemaChoice choice = new XmlSchemaChoice();
                    choice.MinOccurs = minOccurs;
                    choice.MaxOccurs = maxOccurs;
                    groupBase = choice;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            foreach (var elementName in elementRefs)
            {
                XmlSchemaElement refElement = new XmlSchemaElement();
                refElement.RefName = new XmlQualifiedName(elementName);
                groupBase.Items.Add(refElement);
            }

            complexType.Particle = groupBase;

            return complexType;

        }



        public class XmlSchemaOrganizer
        {
            private XmlSchema _schema;

            public XmlSchemaOrganizer(XmlSchema schema)
            {
                _schema = schema;
            }

            public static XmlSchemaOrganizer From(XmlSchema schema)
            {
                return new XmlSchemaOrganizer(schema);
            }


            public IList<XmlSchemaElement> Elements
            {
                get
                {
                    return GetElements(_schema);
                }

            }

            public IList<XmlSchemaAttribute> Attributes
            {
                get
                {

                    return CreateList<XmlSchemaAttribute>(_schema.Attributes);
                }

            }

            public IList<XmlSchemaAttributeGroup> AttributeGroups
            {
                get
                {

                    return CreateList<XmlSchemaAttributeGroup>(_schema.AttributeGroups);
                }

            }

            public IList<XmlSchemaGroup> Groups
            {
                get
                {
                    return CreateList<XmlSchemaGroup>(_schema.Groups);
                }

            }

            public IList<XmlSchemaType> SchemaTypes
            {
                get
                {
                    return CreateList<XmlSchemaType>(_schema.SchemaTypes);
                }

            }

            public IList<XmlSchemaObject> Items
            {
                get
                {
                    return CreateList<XmlSchemaObject>(_schema.Items);
                }

            }

            private static List<XmlSchemaElement> GetElements(XmlSchema schema)
            {
                List<XmlSchemaElement> list = new List<XmlSchemaElement>();
                Stack<XmlSchemaElement> stack = new Stack<XmlSchemaElement>();
                foreach (XmlSchemaElement element in CreateList<XmlSchemaElement>(schema.Elements))
                {
                    stack.Push(element);
                }
                ProcessStack(stack, list);
                return list;
            }

            private static void ProcessStack(Stack<XmlSchemaElement> stack, List<XmlSchemaElement> list)
            {
                while (stack.Count != 0)
                {
                    XmlSchemaElement element = stack.Pop();

                    if (element.ElementSchemaType is XmlSchemaComplexType complexType)
                    {
                        if (complexType.Particle != null && complexType.Particle is XmlSchemaGroupBase groupBase)
                        {
                            ProcessStack(stack, groupBase);
                        }
                    }

                    list.Add(element);
                }
            }

            private static void ProcessStack(Stack<XmlSchemaElement> stack, XmlSchemaGroupBase group)
            {
                foreach (XmlSchemaObject obj in CreateList<XmlSchemaObject>(group.Items))
                {
                    if (obj is XmlSchemaElement element2)
                    {
                        stack.Push(element2);
                    }
                }
            }

            private static List<TSchemaObject> CreateList<TSchemaObject>(XmlSchemaObjectCollection collection)
                where TSchemaObject : XmlSchemaObject
            {
                return new List<TSchemaObject>(Cast<TSchemaObject>(CreateListCore(collection)));
            }
            private static List<TSchemaObject> CreateList<TSchemaObject>(XmlSchemaObjectTable collection)
                where TSchemaObject : XmlSchemaObject
            {
                return new List<TSchemaObject>(Cast<TSchemaObject>(CreateListCore(collection.Values)));
            }


            private static IEnumerable<XmlSchemaObject> CreateListCore(IEnumerable collection)
            {
                List<XmlSchemaObject> list = new List<XmlSchemaObject>();
                XmlSchemaObjectEnumerator enumerator = new XmlSchemaObjectEnumerator(collection);
                while (enumerator.MoveNext())
                {
                    list.Add(enumerator.Current);
                }

                return list;
            }

            private static IEnumerable<T> Cast<T>(IEnumerable items)
            {
                IEnumerator enumerator = items.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    yield return (T)enumerator.Current;
                }
            }


            private class XmlSchemaObjectEnumerator : IEnumerator<XmlSchemaObject>
            {
                private XmlSchemaObjectCollection _collection;
                private int _index;
                private int _count;

                public XmlSchemaObjectEnumerator(IEnumerable collection)
                {
                    _collection = new XmlSchemaObjectCollection();
                    foreach (XmlSchemaObject o in collection)
                    {
                        _collection.Add(o);
                    }
                    _index = -1;
                    _count = _collection.Count;
                }


                /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
                public void Dispose()
                {
                }

                /// <summary>Advances the enumerator to the next element of the collection.</summary>
                /// <returns>
                /// <see langword="true" /> if the enumerator was successfully advanced to the next element; <see langword="false" /> if the enumerator has passed the end of the collection.</returns>
                /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
                public bool MoveNext()
                {
                    _index++;
                    return _index < _count;
                }

                /// <summary>Sets the enumerator to its initial position, which is before the first element in the collection.</summary>
                /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
                public void Reset()
                {
                    _index = -1;
                }

                /// <summary>Gets the element in the collection at the current position of the enumerator.</summary>
                /// <returns>The element in the collection at the current position of the enumerator.</returns>
                public XmlSchemaObject Current => _collection[_index];

                /// <summary>Gets the element in the collection at the current position of the enumerator.</summary>
                /// <returns>The element in the collection at the current position of the enumerator.</returns>
                object IEnumerator.Current
                {
                    get
                    {
                        return Current;
                    }
                }
            }
        }

        private static void RemoveAttribute(XElement element, string localName)
        {
            if (element.Attributes().Select(a => a.Name.LocalName).Contains(localName))
            {
                foreach (XAttribute attribute in element.Attributes())
                {
                    if (attribute.Name.LocalName == localName)
                    {
                        attribute.Remove();
                    }
                }
            }
        }


        private void ProcessSchema(XmlSchema node)
        {
            foreach (XmlSchemaObject obj in node.Items)
            {
                ProcessSchemaObject(obj);
            }
        }


        private void ProcessSchemaObject(XmlSchemaObject node)
        {

            switch (node)
            {
                case XmlSchema xmlSchema: ProcessSchema(xmlSchema); break;
                case XmlSchemaAll xmlSchemaAll: ProcessSchemaAll(xmlSchemaAll); break;
                case XmlSchemaAnnotated xmlSchemaAnnotated: ProcessSchemaAnnotated(xmlSchemaAnnotated); break;
                case XmlSchemaAnnotation xmlSchemaAnnotation: ProcessSchemaAnnotation(xmlSchemaAnnotation); break;
                case XmlSchemaAppInfo xmlSchemaAppInfo: ProcessSchemaAppInfo(xmlSchemaAppInfo); break;
                case XmlSchemaDocumentation xmlSchemaDocumentation: ProcessSchemaDocumentation(xmlSchemaDocumentation); break;
                case XmlSchemaExternal xmlSchemaExternal: ProcessSchemaExternal(xmlSchemaExternal); break;
            }
        }


        private void ProcessSchemaAnnotated(XmlSchemaAnnotated node)
        {

            switch (node)
            {
                case XmlSchemaAll xmlSchemaAll: ProcessSchemaAll(xmlSchemaAll); break;
                case XmlSchemaAny xmlSchemaAny: ProcessSchemaAny(xmlSchemaAny); break;
                case XmlSchemaAnyAttribute xmlSchemaAnyAttribute: ProcessSchemaAnyAttribute(xmlSchemaAnyAttribute); break;
                case XmlSchemaAttribute xmlSchemaAttribute: ProcessSchemaAttribute(xmlSchemaAttribute); break;
                case XmlSchemaAttributeGroup xmlSchemaAttributeGroup: ProcessSchemaAttributeGroup(xmlSchemaAttributeGroup); break;
                case XmlSchemaAttributeGroupRef xmlSchemaAttributeGroupRef: ProcessSchemaAttributeGroupRef(xmlSchemaAttributeGroupRef); break;
                case XmlSchemaChoice xmlSchemaChoice: ProcessSchemaChoice(xmlSchemaChoice); break;
                case XmlSchemaComplexContent xmlSchemaComplexContent: ProcessSchemaComplexContent(xmlSchemaComplexContent); break;
                case XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension: ProcessSchemaComplexContentExtension(xmlSchemaComplexContentExtension); break;
                case XmlSchemaComplexContentRestriction xmlSchemaComplexContentRestriction: ProcessSchemaComplexContentRestriction(xmlSchemaComplexContentRestriction); break;
                case XmlSchemaComplexType xmlSchemaComplexType: ProcessSchemaComplexType(xmlSchemaComplexType); break;
                case XmlSchemaContent xmlSchemaContent: ProcessSchemaContent(xmlSchemaContent); break;
                case XmlSchemaContentModel xmlSchemaContentModel: ProcessSchemaContentModel(xmlSchemaContentModel); break;
                case XmlSchemaElement xmlSchemaElement: ProcessSchemaElement(xmlSchemaElement); break;
                case XmlSchemaFacet xmlSchemaFacet: ProcessSchemaFacet(xmlSchemaFacet); break;
                case XmlSchemaGroup xmlSchemaGroup: ProcessSchemaGroup(xmlSchemaGroup); break;
                case XmlSchemaGroupBase xmlSchemaGroupBase: ProcessSchemaGroupBase(xmlSchemaGroupBase); break;
                case XmlSchemaGroupRef xmlSchemaGroupRef: ProcessSchemaGroupRef(xmlSchemaGroupRef); break;
                case XmlSchemaIdentityConstraint xmlSchemaIdentityConstraint: ProcessSchemaIdentityConstraint(xmlSchemaIdentityConstraint); break;
                case XmlSchemaXPath xmlSchemaXPath: ProcessSchemaXPath(xmlSchemaXPath); break;
                case XmlSchemaNotation xmlSchemaNotation: ProcessSchemaNotation(xmlSchemaNotation); break;
                case XmlSchemaParticle xmlSchemaParticle: ProcessSchemaParticle(xmlSchemaParticle); break;
                case XmlSchemaSimpleType xmlSchemaSimpleType: ProcessSchemaSimpleType(xmlSchemaSimpleType); break;
                case XmlSchemaSimpleTypeContent xmlSchemaSimpleTypeContent: ProcessSchemaSimpleTypeContent(xmlSchemaSimpleTypeContent); break;
                case XmlSchemaType xmlSchemaType: ProcessSchemaType(xmlSchemaType); break;
            }
        }

        private void ProcessSchemaAll(XmlSchemaAll node)
        {

        }

        private void ProcessSchemaAny(XmlSchemaAny node)
        {

        }

        private void ProcessSchemaAnyAttribute(XmlSchemaAnyAttribute node)
        {

        }

        private void ProcessSchemaAttribute(XmlSchemaAttribute node)
        {

        }

        private void ProcessSchemaAttributeGroup(XmlSchemaAttributeGroup node)
        {

        }

        private void ProcessSchemaAttributeGroupRef(XmlSchemaAttributeGroupRef node)
        {

        }

        private void ProcessSchemaChoice(XmlSchemaChoice node)
        {

        }

        private void ProcessSchemaComplexContent(XmlSchemaComplexContent node)
        {

        }

        private void ProcessSchemaComplexContentExtension(XmlSchemaComplexContentExtension node)
        {

        }

        private void ProcessSchemaComplexContentRestriction(XmlSchemaComplexContentRestriction node)
        {

        }

        private void ProcessSchemaComplexType(XmlSchemaComplexType node)
        {

        }

        private void ProcessSchemaContent(XmlSchemaContent node)
        {

            switch (node)
            {
                case XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension: ProcessSchemaComplexContentExtension(xmlSchemaComplexContentExtension); break;
                case XmlSchemaComplexContentRestriction xmlSchemaComplexContentRestriction: ProcessSchemaComplexContentRestriction(xmlSchemaComplexContentRestriction); break;
                case XmlSchemaSimpleContentExtension xmlSchemaSimpleContentExtension: ProcessSchemaSimpleContentExtension(xmlSchemaSimpleContentExtension); break;
                case XmlSchemaSimpleContentRestriction xmlSchemaSimpleContentRestriction: ProcessSchemaSimpleContentRestriction(xmlSchemaSimpleContentRestriction); break;
            }
        }

        private void ProcessSchemaSimpleContentExtension(XmlSchemaSimpleContentExtension node)
        {

        }

        private void ProcessSchemaSimpleContentRestriction(XmlSchemaSimpleContentRestriction node)
        {

        }

        private void ProcessSchemaContentModel(XmlSchemaContentModel node)
        {

            switch (node)
            {
                case XmlSchemaComplexContent xmlSchemaComplexContent: ProcessSchemaComplexContent(xmlSchemaComplexContent); break;
                case XmlSchemaSimpleContent xmlSchemaSimpleContent: ProcessSchemaSimpleContent(xmlSchemaSimpleContent); break;
            }
        }

        private void ProcessSchemaSimpleContent(XmlSchemaSimpleContent node)
        {

        }

        private void ProcessSchemaElement(XmlSchemaElement node)
        {

        }

        private void ProcessSchemaFacet(XmlSchemaFacet node)
        {

            switch (node)
            {
                case XmlSchemaNumericFacet xmlSchemaNumericFacet: ProcessSchemaNumericFacet(xmlSchemaNumericFacet); break;
                case XmlSchemaPatternFacet xmlSchemaPatternFacet: ProcessSchemaPatternFacet(xmlSchemaPatternFacet); break;
                case XmlSchemaEnumerationFacet xmlSchemaEnumerationFacet: ProcessSchemaEnumerationFacet(xmlSchemaEnumerationFacet); break;
                case XmlSchemaMinExclusiveFacet xmlSchemaMinExclusiveFacet: ProcessSchemaMinExclusiveFacet(xmlSchemaMinExclusiveFacet); break;
                case XmlSchemaMinInclusiveFacet xmlSchemaMinInclusiveFacet: ProcessSchemaMinInclusiveFacet(xmlSchemaMinInclusiveFacet); break;
                case XmlSchemaMaxExclusiveFacet xmlSchemaMaxExclusiveFacet: ProcessSchemaMaxExclusiveFacet(xmlSchemaMaxExclusiveFacet); break;
                case XmlSchemaMaxInclusiveFacet xmlSchemaMaxInclusiveFacet: ProcessSchemaMaxInclusiveFacet(xmlSchemaMaxInclusiveFacet); break;
                case XmlSchemaWhiteSpaceFacet xmlSchemaWhiteSpaceFacet: ProcessSchemaWhiteSpaceFacet(xmlSchemaWhiteSpaceFacet); break;
            }
        }

        private void ProcessSchemaNumericFacet(XmlSchemaNumericFacet node)
        {

            switch (node)
            {
                case XmlSchemaLengthFacet xmlSchemaLengthFacet: ProcessSchemaLengthFacet(xmlSchemaLengthFacet); break;
                case XmlSchemaMinLengthFacet xmlSchemaMinLengthFacet: ProcessSchemaMinLengthFacet(xmlSchemaMinLengthFacet); break;
                case XmlSchemaMaxLengthFacet xmlSchemaMaxLengthFacet: ProcessSchemaMaxLengthFacet(xmlSchemaMaxLengthFacet); break;
                case XmlSchemaTotalDigitsFacet xmlSchemaTotalDigitsFacet: ProcessSchemaTotalDigitsFacet(xmlSchemaTotalDigitsFacet); break;
                case XmlSchemaFractionDigitsFacet xmlSchemaFractionDigitsFacet: ProcessSchemaFractionDigitsFacet(xmlSchemaFractionDigitsFacet); break;
            }
        }

        private void ProcessSchemaLengthFacet(XmlSchemaLengthFacet node)
        {

        }

        private void ProcessSchemaMinLengthFacet(XmlSchemaMinLengthFacet node)
        {

        }

        private void ProcessSchemaMaxLengthFacet(XmlSchemaMaxLengthFacet node)
        {

        }

        private void ProcessSchemaTotalDigitsFacet(XmlSchemaTotalDigitsFacet node)
        {

        }

        private void ProcessSchemaFractionDigitsFacet(XmlSchemaFractionDigitsFacet node)
        {

        }

        private void ProcessSchemaPatternFacet(XmlSchemaPatternFacet node)
        {

        }

        private void ProcessSchemaEnumerationFacet(XmlSchemaEnumerationFacet node)
        {

        }

        private void ProcessSchemaMinExclusiveFacet(XmlSchemaMinExclusiveFacet node)
        {

        }

        private void ProcessSchemaMinInclusiveFacet(XmlSchemaMinInclusiveFacet node)
        {

        }

        private void ProcessSchemaMaxExclusiveFacet(XmlSchemaMaxExclusiveFacet node)
        {

        }

        private void ProcessSchemaMaxInclusiveFacet(XmlSchemaMaxInclusiveFacet node)
        {

        }

        private void ProcessSchemaWhiteSpaceFacet(XmlSchemaWhiteSpaceFacet node)
        {

        }

        private void ProcessSchemaGroup(XmlSchemaGroup node)
        {

        }

        private void ProcessSchemaGroupBase(XmlSchemaGroupBase node)
        {

            switch (node)
            {
                case XmlSchemaAll xmlSchemaAll: ProcessSchemaAll(xmlSchemaAll); break;
                case XmlSchemaChoice xmlSchemaChoice: ProcessSchemaChoice(xmlSchemaChoice); break;
                case XmlSchemaSequence xmlSchemaSequence: ProcessSchemaSequence(xmlSchemaSequence); break;
            }
        }

        private void ProcessSchemaSequence(XmlSchemaSequence node)
        {

        }

        private void ProcessSchemaGroupRef(XmlSchemaGroupRef node)
        {

        }

        private void ProcessSchemaIdentityConstraint(XmlSchemaIdentityConstraint node)
        {

            switch (node)
            {
                case XmlSchemaUnique xmlSchemaUnique: ProcessSchemaUnique(xmlSchemaUnique); break;
                case XmlSchemaKey xmlSchemaKey: ProcessSchemaKey(xmlSchemaKey); break;
                case XmlSchemaKeyref xmlSchemaKeyref: ProcessSchemaKeyref(xmlSchemaKeyref); break;
            }
        }

        private void ProcessSchemaUnique(XmlSchemaUnique node)
        {

        }

        private void ProcessSchemaKey(XmlSchemaKey node)
        {

        }

        private void ProcessSchemaKeyref(XmlSchemaKeyref node)
        {

        }

        private void ProcessSchemaXPath(XmlSchemaXPath node)
        {

        }

        private void ProcessSchemaNotation(XmlSchemaNotation node)
        {

        }

        private void ProcessSchemaParticle(XmlSchemaParticle node)
        {

            switch (node)
            {
                case XmlSchemaAll xmlSchemaAll: ProcessSchemaAll(xmlSchemaAll); break;
                case XmlSchemaAny xmlSchemaAny: ProcessSchemaAny(xmlSchemaAny); break;
                case XmlSchemaChoice xmlSchemaChoice: ProcessSchemaChoice(xmlSchemaChoice); break;
                case XmlSchemaElement xmlSchemaElement: ProcessSchemaElement(xmlSchemaElement); break;
                case XmlSchemaGroupBase xmlSchemaGroupBase: ProcessSchemaGroupBase(xmlSchemaGroupBase); break;
                case XmlSchemaGroupRef xmlSchemaGroupRef: ProcessSchemaGroupRef(xmlSchemaGroupRef); break;
            }
        }

        private void ProcessSchemaSimpleType(XmlSchemaSimpleType node)
        {

        }

        private void ProcessSchemaSimpleTypeContent(XmlSchemaSimpleTypeContent node)
        {

            switch (node)
            {
                case XmlSchemaSimpleTypeList xmlSchemaSimpleTypeList: ProcessSchemaSimpleTypeList(xmlSchemaSimpleTypeList); break;
                case XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction: ProcessSchemaSimpleTypeRestriction(xmlSchemaSimpleTypeRestriction); break;
                case XmlSchemaSimpleTypeUnion xmlSchemaSimpleTypeUnion: ProcessSchemaSimpleTypeUnion(xmlSchemaSimpleTypeUnion); break;
            }
        }

        private void ProcessSchemaSimpleTypeList(XmlSchemaSimpleTypeList node)
        {

        }

        private void ProcessSchemaSimpleTypeRestriction(XmlSchemaSimpleTypeRestriction node)
        {

        }

        private void ProcessSchemaSimpleTypeUnion(XmlSchemaSimpleTypeUnion node)
        {

        }

        private void ProcessSchemaType(XmlSchemaType node)
        {

            switch (node)
            {
                case XmlSchemaComplexType xmlSchemaComplexType: ProcessSchemaComplexType(xmlSchemaComplexType); break;
                case XmlSchemaSimpleType xmlSchemaSimpleType: ProcessSchemaSimpleType(xmlSchemaSimpleType); break;
            }
        }

        private void ProcessSchemaAnnotation(XmlSchemaAnnotation node)
        {

        }

        private void ProcessSchemaAppInfo(XmlSchemaAppInfo node)
        {

        }

        private void ProcessSchemaDocumentation(XmlSchemaDocumentation node)
        {

        }

        private void ProcessSchemaExternal(XmlSchemaExternal node)
        {

            switch (node)
            {
                case XmlSchemaImport xmlSchemaImport: ProcessSchemaImport(xmlSchemaImport); break;
                case XmlSchemaInclude xmlSchemaInclude: ProcessSchemaInclude(xmlSchemaInclude); break;
                case XmlSchemaRedefine xmlSchemaRedefine: ProcessSchemaRedefine(xmlSchemaRedefine); break;
            }
        }

        private void ProcessSchemaImport(XmlSchemaImport node)
        {

        }

        private void ProcessSchemaInclude(XmlSchemaInclude node)
        {

        }

        private void ProcessSchemaRedefine(XmlSchemaRedefine node)
        {

        }

    }
}
