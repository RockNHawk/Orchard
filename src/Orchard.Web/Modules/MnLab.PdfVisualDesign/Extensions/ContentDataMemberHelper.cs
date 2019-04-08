using AutoMapper;
using MnLab.PdfVisualDesign.Binding.Elements;
using MnLab.PdfVisualDesign.ViewModels;
using Orchard.Environment.Extensions;
using Orchard.Layouts.Framework.Display;
using Orchard.Layouts.Framework.Drivers;
using Orchard.Layouts.Helpers;
using Orchard.Layouts.Services;
using Orchard.Layouts.ViewModels;
using System.Linq;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.FileSystems.VirtualPath;
using Orchard;
using Orchard.ContentManagement.Handlers;
using Orchard.UI.Zones;
using System.Web;
using System.Reflection;
using System.Web.Mvc;
using Orchard.Core.Title.Models;

namespace MnLab.PdfVisualDesign.Binding.Drivers
{
[System.Diagnostics.DebuggerDisplay("Name={Part.Name},BindingDef={BindingDef.Key},Field={Field},Property={PropertyInfo}")]
    public class ContentPartDataMemberHelper {

        public ContentField Field;

        public PropertyInfo PropertyInfo;

        public ContentPart Part;

        public string Expression;

        public IValueBindingDef BindingDef;

        public bool hasDataMember { get => Field != null || PropertyInfo != null; }

        public ContentPartDataMemberHelper(ContentPart contentItem, IValueBindingDef bindingDef, ContentField field) : this(contentItem, bindingDef) {
            this.Field = field;
        }
        public ContentPartDataMemberHelper(ContentPart contentItem, IValueBindingDef bindingDef, PropertyInfo field) : this(contentItem, bindingDef) {
            this.PropertyInfo = field;
        }

        public ContentPartDataMemberHelper(ContentPart contentItem, IValueBindingDef bindingDef) {
            this.Part = contentItem;
            this.BindingDef = bindingDef;
            this.Expression = bindingDef.MemberExpression;
        }


        public IContentPartMemberAccessor GetAccessor() {
            if (Field != null) {
                return new ContentPartFieldMemberAccessor(Part, Field, Expression);
            }
            else if (PropertyInfo != null) {
                return new ContenttPartPropertyMemberAccessor(Part, PropertyInfo, Expression);
            }
            else {
                throw new InvalidOperationException(string.Format("Data member '{0}' not setted", BindingDef.Key));
            }
        }

        public static ContentPartDataMemberHelper FindFromContentItem(ContentItem contentItem, IValueBindingDef bindingDef) {

            var partName = bindingDef.ContentPartName;
            string MemberExpression = bindingDef.MemberExpression;
            if (partName == null) throw new ArgumentNullException(nameof(partName));
            if (MemberExpression == null) throw new ArgumentNullException(nameof(MemberExpression));
            var part = contentItem.Parts.FirstOrDefault(x => x.PartDefinition.Name == partName);
            if (part == null) return null;
            return FindFromContentPart(part, bindingDef);
        }

        public static ContentPartDataMemberHelper FindFromContentPart(ContentPart part, IValueBindingDef bindingDef) {
            var me = bindingDef.MemberExpression;
            /*
               https://docs.orchardproject.net/en/latest/Documentation/Creating-a-custom-field-type/

            field in Orchard is dynamic added to a ContentPart by user
            the internal ContentPart own it's "hard code" Property like TitlePart.Title is a hard coded property
            it also storage the data, but it only can edit by hole part no single Property editor support (see TitlePartDriver), or I can add this feature by self.
            */
            var field = part?.Fields.FirstOrDefault(x => x.Name == me);

            var helper = new ContentPartDataMemberHelper(part, bindingDef);
            if (field != null) {
                helper.Field = field;
            }
            else {
                var property = part.GetType().GetProperty(me);
                helper.PropertyInfo = property;
            }
            return helper;
        }

    }
}