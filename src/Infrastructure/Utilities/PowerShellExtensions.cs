// -----------------------------------------------------------------------
// <copyright file="PowerShellExtensions.cs" company="Ollon, LLC">
//     Copyright (c) 2018 Ollon, LLC. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Formatting = Newtonsoft.Json.Formatting;

namespace PowerShell.Infrastructure.Utilities
{
    public static class PowerShellExtensions
    {
        public static T Cast<T>(this object o) => (T) o;

        public static TSchemaObject First<TSchemaObject>(this XmlSchemaObjectTable table) where TSchemaObject: XmlSchemaObject
        {
            foreach (var entry in table.Values)
            {
                if (entry is TSchemaObject schemaObject)
                {
                    return schemaObject;
                }
            }

            return null;
        }

        public static void AddDocumentation(this XmlSchemaAnnotated annotated, string text)
        {
            XmlSchemaAnnotation annotation = new XmlSchemaAnnotation();
            XmlDocument document = new XmlDocument();

            annotation.Items.Add(new XmlSchemaDocumentation()
            {
                Markup = new XmlNode[]
                {
                    document.CreateTextNode(text)
                }
            });
            annotated.Annotation = annotation;
        }

        public static IEnumerable<XmlSchemaElement> GetElementComplexTypeItems(this XmlSchemaElement element)
        {
            if (element.ElementSchemaType is XmlSchemaComplexType complexType)
            {
                if (complexType.Particle is XmlSchemaGroupBase groupBase)
                {
                    foreach (XmlSchemaObject obj in groupBase.Items)
                    {
                        if (obj is XmlSchemaElement schemaElement)
                        {
                            yield return schemaElement;

                            foreach (XmlSchemaElement schemaElement2 in GetElementComplexTypeItems(schemaElement))
                            {
                                yield return schemaElement2;
                            }
                        }
                    }
                }
                ;
            }
        }
        public static string ToJSchemaString(this JSchema schema)
        {
            using (StringWriter sw = new StringWriter())
            using (JsonTextWriter writer = new JsonTextWriter(sw))
            {
                writer.IndentChar = ' ';
                writer.Indentation = 4;
                writer.Formatting = Formatting.Indented;
                schema.WriteTo(writer);
                return sw.ToString();
            }
        }
        public static string ToXmlString(this XmlSchema schema, XmlWriterSettings settings = null)
        {
            if (schema is null) throw new ArgumentNullException(nameof(schema));

            StringWriter writer = new StringWriter();
            using (XmlWriter xmlWriter = XmlWriter.Create(writer,
                settings ??
                new XmlWriterSettings()
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\r\n"
                }))
            {
                schema.Write(xmlWriter);

                return writer.ToString();
            }
        }
    }
}
