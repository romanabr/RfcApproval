using PSELookup;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using System.Linq;
using RedSys.RFC.Core.Helper;

namespace PSELookup.Field
{
    class PSELookupField : SPFieldLookup
    {
        protected const string JSLinkUrl = "~site/_layouts/15/PSELookup/AutocompleteLookup/richcontrols.js";

        List<SPWeb> allwebs = new List<SPWeb>();

        public override string JSLink
        {
            get
            {
                if (SPContext.Current != null && (SPContext.Current.FormContext.FormMode == SPControlMode.Invalid || SPContext.Current.FormContext.FormMode == SPControlMode.Display))
                    return JSLinkUrl;
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                base.JSLink = value;
            }
        }

        public override void OnAdded(SPAddFieldOptions op)
        {
            base.OnAdded(op);
            DependedLookUp();
        }

        public override void Update()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(base.SchemaXml);

            string multipleValue = GetAttributeFromXML(doc, "MultipleValues");
            string showField = GetAttributeFromXML(doc, "ValueField");

            if(showField != "")
                EnsureAttribute(doc, "ShowField", showField);

            if (multipleValue != null)
            {
                if (multipleValue.ToLower() == "true")
                    EnsureAttribute(doc, "Mult", "TRUE");
                else
                    EnsureAttribute(doc, "Mult", "FALSE");
            }

            base.SchemaXml = doc.OuterXml;
            base.Update();
            DependedLookUp();          
        }

        protected void GetAllWebsSafely(SPWeb subweb, List<SPWeb> allwebs)
        {
            allwebs = subweb.GetSubwebsForCurrentUser().ToList();
            foreach (SPWeb sweb in subweb.GetSubwebsForCurrentUser())
                GetAllWebsSafely(sweb, allwebs);
        }
        
        public void DependedLookUp()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(base.SchemaXml);
            #region - Deppendend Lists -
            string newFields = "";
            string DependentLookUp = GetAttributeFromXML(doc, "DependentLookUp");
            string oldFields = GetAttributeFromXML(doc, "OldDependentLookUp");

            if (!String.IsNullOrEmpty(DependentLookUp))
            {
                string[] arrayDependentLookUp = DependentLookUp.Split(';');
                Guid LookupWebId = base.LookupWebId;
                string LookupList = base.LookupList;
                Guid LookupField = base.Id;

                SPWeb selectedWeb = SPContext.Current.Web;
                SPList selectedList = this.ParentList;
                SPField selectedField = selectedList.Fields[LookupField];
                SPList lookUpList = selectedWeb.Lists[new Guid(LookupList)];
                List<SPFieldLookup> arrayFieldExists = new List<SPFieldLookup>();

                #region Check for existing fields
                if (!String.IsNullOrEmpty(oldFields))
                {
                    List<string> arrayOldLookUp = new List<string>(oldFields.Split(';'));
                    foreach (string oldFieldName in arrayOldLookUp)
                    {
                        try
                        {
                            SPField testField = this.ParentList.Fields.GetFieldByInternalName(oldFieldName);
                            if (testField != null)
                            {
                                SPFieldLookup fieldDepLookup = (SPFieldLookup)testField;
                                arrayFieldExists.Add(fieldDepLookup);
                            }
                        }
                        catch (Exception exc)
                        {
							ExceptionHelper.DUmpException(exc);
                            List<string> arrayOldLookUpCopy = arrayOldLookUp.ToList();
                            arrayOldLookUpCopy.Remove(oldFieldName);
                            if (arrayOldLookUpCopy.Count() != 0)
                                oldFields = arrayOldLookUpCopy.Aggregate((a, x) => a + ";" + x);
                            else
                                oldFields = "";
                        }
                    }
                }
                #endregion

                foreach (string lookUpFieldName in arrayDependentLookUp)
                {
                    string fieldName = lookUpFieldName.Trim();

                    if (lookUpList.Fields.ContainsField(fieldName))
                    {
                        SPField currentfield = lookUpList.Fields[fieldName];
                        try
                        {
                            if (currentfield != null)
                            {
                                string fieldInternalName = currentfield.InternalName;

                                bool notExists = true;
                                foreach (SPFieldLookup spfield in arrayFieldExists)
                                {
                                    if (spfield.LookupField == fieldInternalName)
                                    {
                                        newFields += spfield.InternalName + ";";
                                        notExists = false;
                                        break;
                                    }
                                }

                                if (notExists)
                                {
                                    string depLookUp = this.ParentList.Fields.AddDependentLookup(selectedField.Title + ":" + currentfield.Title, base.Id);
                                    SPFieldLookup fieldDepLookup = (SPFieldLookup)this.ParentList.Fields.GetFieldByInternalName(depLookUp);

                                    fieldDepLookup.LookupWebId = LookupWebId;
                                    fieldDepLookup.LookupField = lookUpList.Fields[currentfield.Title].InternalName;
                                    fieldDepLookup.Update();

                                    newFields += fieldDepLookup.InternalName + ";";
                                }
                            }
                        }
                        catch (Exception exc)
                        {
							ExceptionHelper.DUmpException(exc);
                        }
                    }
                }
            }

            #region Delete Unneded
            if (!String.IsNullOrEmpty(newFields))
            {
                string[] arrayOldFields = null;
                string[] arrayNewFields = newFields.TrimEnd(new char[] { ';' }).Split(';');

                if (!String.IsNullOrEmpty(oldFields))
                    arrayOldFields = oldFields.Split(';');

                if (arrayOldFields != null)
                {
                    foreach (string oldField in arrayOldFields)
                    {
                        try
                        {
                            int pos = Array.IndexOf(arrayNewFields, oldField);
                            if (pos > -1) { }
                            else
                            {
                                SPField removeField = this.ParentList.Fields.GetFieldByInternalName(oldField);
                                removeField.Delete();
                            }
                        }
                        catch (Exception exc)
                        {
							ExceptionHelper.DUmpException(exc);
                        }
                    }
                }
            }
            else
            {
                string[] arrayOldFields = null;
                if (!String.IsNullOrEmpty(oldFields))
                    arrayOldFields = oldFields.Split(';');
                if (arrayOldFields != null)
                {
                    foreach (string oldField in arrayOldFields)
                    {
                        SPField removeField = this.ParentList.Fields.GetFieldByInternalName(oldField);
                        removeField.Delete();
                    }
                }
            }
            #endregion

            EnsureAttribute(doc, "OldDependentLookUp", newFields.TrimEnd(new char[] { ';' }));
            base.SchemaXml = doc.OuterXml;
            base.Update();
            #endregion
        }

        protected void EnsureAttribute(XmlDocument doc, string name, string value)
        {
            XmlAttribute attribute = doc.DocumentElement.Attributes[name];
            if (attribute == null)
            {
                attribute = doc.CreateAttribute(name);
                doc.DocumentElement.Attributes.Append(attribute);
            }
            doc.DocumentElement.Attributes[name].Value = value;
        }

        protected string GetAttributeFromXML(XmlDocument doc, string propertyName)
        {
            XmlAttribute attribute = doc.DocumentElement.Attributes[propertyName];
            if (attribute != null)
                return doc.DocumentElement.Attributes[propertyName].Value;
            return null;
        }

        public PSELookupField(SPFieldCollection fields, string fieldName) : base(fields, fieldName)
        {
        }

        public PSELookupField(SPFieldCollection fields, string typeName, string displayName)
            : base(fields, typeName, displayName)
        {
        }

        public override Microsoft.SharePoint.WebControls.BaseFieldControl FieldRenderingControl
        {
            get { return new PSELookupFieldControl() { FieldName = InternalName }; }
        }

        public string Filter
        {
            get { return this.GetFieldAttribute("Filter");}
            set { this.SetFieldAttribute("Filter", value);}
        }
    }
}
