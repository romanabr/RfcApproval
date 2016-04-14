using System.Xml.Serialization;
using PSELookup.CONTROLTEMPLATES;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using System.Web.UI;

namespace PSELookup.Field
{
    [ToolboxData("<{0}:PSELookupFieldControl runat=server></{0}:PSELookupFieldControl>"),
    XmlRoot(Namespace = "PSELookup")]
    public class PSELookupFieldControl : BaseFieldControl
    {
        protected PSELookupAutocompleteField _PSELookupAutocompleteField;

        public override object Value
        {
            get
            {
                EnsureChildControls();
                return _PSELookupAutocompleteField == null ? null : _PSELookupAutocompleteField.Value;
            }
            set
            {
                EnsureChildControls();
                if(_PSELookupAutocompleteField != null)
                    _PSELookupAutocompleteField.Value = value;
            }
        }

        public string ValueField { get; set; }
        public string TitleField { get; set; }
        public string DescriptionFields { get; set; }
        public string MinLength { get; set; }
        public string MaxRows { get; set; }
        public string MaxHeight { get; set; }
        public string Filter { get; set; }
        public string AutoPostBack { get; set; }
        public string DynamicFilter { get; set; }
        public string DynamicFilterSourceField { get; set; }
        public string OrderBy { get; set; }
        public string OrderByASC { get; set; }
        public string ReadOnlyLookUp { get; set; }
        public string MultipleValues { get; set; }
        public string ListOfLists { get; set; }
        public string ListOfSites { get; set; }
        public string ListOfFields { get; set; }
        public string OtherLookUp { get; set; }
        public string OldLookUp { get; set; }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            _PSELookupAutocompleteField = Page.LoadControl("~/_controltemplates/15/PSELookupAutocompleteField.ascx") as PSELookupAutocompleteField;
            if (_PSELookupAutocompleteField != null)
            {
                var field = Field as SPFieldLookup;
                if (field != null)
                {
                    MaxHeight = "200";
                    _PSELookupAutocompleteField.Field = Field as SPFieldLookup;
                    _PSELookupAutocompleteField.TitleField = TitleField ?? field.GetFieldAttribute("TitleField");
                    _PSELookupAutocompleteField.DescriptionFields = DescriptionFields ?? field.GetFieldAttribute("DescriptionFields");
                    _PSELookupAutocompleteField.MaxHeight = MaxHeight ?? field.GetFieldAttribute("MaxHeight");
                    _PSELookupAutocompleteField.MaxRows = MaxRows ?? field.GetFieldAttribute("MaxRows");
                    _PSELookupAutocompleteField.MinLength = MinLength ?? field.GetFieldAttribute("MinLength");
                    _PSELookupAutocompleteField.ValueField = ValueField ?? field.GetFieldAttribute("ValueField");
                    _PSELookupAutocompleteField.Filter = Filter ?? field.GetFieldAttribute("Filter");
                    _PSELookupAutocompleteField.AutoPostBack = AutoPostBack ?? field.GetFieldAttribute("AutoPostBack");
                    _PSELookupAutocompleteField.ReadOnlyLookUp = ReadOnlyLookUp ?? field.GetFieldAttribute("ReadOnlyLookUp");
                    _PSELookupAutocompleteField.DynamicFilter = DynamicFilter ?? field.GetFieldAttribute("DynamicFilter");
                    _PSELookupAutocompleteField.DynamicFilterSourceField = DynamicFilter ?? field.GetFieldAttribute("DynamicFilterSourceField");
                    _PSELookupAutocompleteField.OrderBy = OrderBy ?? field.GetFieldAttribute("OrderBy");
                    _PSELookupAutocompleteField.OrderByASC = OrderByASC ?? field.GetFieldAttribute("OrderByASC");
                    _PSELookupAutocompleteField.MultipleValues = MultipleValues ?? field.GetFieldAttribute("MultipleValues");
                    _PSELookupAutocompleteField.ListOfList = ListOfLists ?? field.GetFieldAttribute("ListOfLists");
                    _PSELookupAutocompleteField.ListOfSites = ListOfSites ?? field.GetFieldAttribute("ListOfSites");
                    _PSELookupAutocompleteField.ListOfFields = ListOfFields ?? field.GetFieldAttribute("ListOfFields");
                    _PSELookupAutocompleteField.ListOfFields = ListOfFields ?? field.GetFieldAttribute("OtherLookUp");
                    _PSELookupAutocompleteField.ListOfFields = ListOfFields ?? field.GetFieldAttribute("OldLookUp");
                }

                Control dependentControl = FindRecursiveControl(this.Page.Form, _PSELookupAutocompleteField.DynamicFilter);
                if (dependentControl != null)
                {
                    _PSELookupAutocompleteField.DynamicFilterClientID = dependentControl.ClientID;
                }

                this.Controls.Add(_PSELookupAutocompleteField);
            }
        }

        public override void Validate()
        {
            if (ControlMode == SPControlMode.Display) return;

            base.Validate();
            if (IsValid && Value == null)
            {
                if (Field.Required)
                {
                    IsValid = false;
                    ErrorMessage = SPResource.GetString("MissingRequiredField");
                }
            }
            else if (!IsValid)
            {
                ErrorMessage = "Неверно указаны данные";
            }
        }

        public override void UpdateFieldValueInItem()
        {
            base.UpdateFieldValueInItem();
        }
        public static Control FindRecursiveControl(Control container, string controlID)
        {
            if (string.IsNullOrEmpty(controlID))
                return null;

            Control foundControl = null;

            if (container.HasControls())
            {   // maybe the find control is not proper method, because the control is never going to have the same ID as a field has name and even if yes, it would not be the correct one
                foundControl = container.FindControl(controlID);
                if(foundControl != null)
                    return foundControl;

                foreach (Control c in container.Controls)
                {
                    //if (!string.IsNullOrEmpty(c.ID) && c.ID.Contains(controlID))
                    //    return c;   // same here, this is probably not correct, because there can be other control, which has the controlID as a part of the ID, it is not correct

                    if (c.GetType() == typeof(FormField))
                    {
                        FormField ff = (FormField)c;
                        if (ff.Field.Title == controlID)
                            return c;   // this is the only correct way, because we are looking for SharePoint:FormField type control wich is bound to the FormField with static name same as a text entered in dynamic filter
                    }

                    if (c.HasControls())
                        foundControl = FindRecursiveControl(c, controlID);

                    if (foundControl != null)
                        return foundControl;
                }
            }

            return null;
        }
    }
}
