using Orchard.ContentManagement;
using Orchard.DisplayManagement.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchard.ContentExtensions.Services
{
    /// <summary>
    /// This class contains a dictionary of the Part converter. Each converter convert the part to an object
    /// that can be serialized to JSON
    /// </summary>
    public class PartSerializationManager : IPartSerializationManager
    {
        private Dictionary<string, IContetnPartSerializer> dictionary = new Dictionary<string, IContetnPartSerializer>();

        public PartSerializationManager(IEnumerable<IContetnPartSerializer> contetnPartSerializers)
        {
            foreach (var item in contetnPartSerializers)
            {
                dictionary.Add(item.ForContentPart, item);
            }
        }

        public object Convert(ContentPart part)
        {
            if (dictionary.ContainsKey(part.PartDefinition.Name))
            {
                var converter = dictionary[part.PartDefinition.Name];
                return converter.GetSerializableObject(part);
            }
            else
            {
                return null;
            }
        }

        public object Convert(ContentItem contentItem)
        {
            dynamic returnValue = new Composite();

            foreach (var part in contentItem.Parts)
            {
                if (dictionary.ContainsKey(part.PartDefinition.Name))
                {
                    var converter = dictionary[part.PartDefinition.Name];
                    returnValue[part.PartDefinition.Name] = converter.GetSerializableObject(part);
                }
            }

            return returnValue;
        }
    }
}