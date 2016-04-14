using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint;
using System.Text.RegularExpressions;
using System.Web;
using System.Text;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Administration;
using System.Reflection;
using RedSys.RFC.Core.Helper;

namespace PSELookup.CONTROLTEMPLATES
{
    public partial class PSELookupAutocompleteField : UserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if(string.IsNullOrEmpty(MinLength))
                MinLength = 2.ToString();

            if(string.IsNullOrEmpty(MaxRows))
                MaxRows = 10.ToString();

            if (string.IsNullOrEmpty(ValueField))
                ValueField = TitleField;

            if (string.IsNullOrEmpty(AutoPostBack))
                AutoPostBack = "false";

            if (string.IsNullOrEmpty(DynamicFilter))
                DynamicFilter = "";

            if (string.IsNullOrEmpty(DynamicFilterSourceField))
                DynamicFilterSourceField = "";

            if (string.IsNullOrEmpty(ReadOnlyLookUp))
                ReadOnlyLookUp = "false";

            if (string.IsNullOrEmpty(MultipleValues))
                MultipleValues = "false";

            if (string.IsNullOrEmpty(DynamicFilterClientID))
                DynamicFilterClientID = "";

            if (string.IsNullOrEmpty(OrderBy))
                OrderBy = "";
            
            if (string.IsNullOrEmpty(OrderByASC))
                OrderByASC = "false";

            if (string.IsNullOrEmpty(ListOfList))
                ListOfList = "";

            if (string.IsNullOrEmpty(ListOfSites))
                ListOfSites = "";

            if (string.IsNullOrEmpty(ListOfFields))
                ListOfFields = "";

            if (string.IsNullOrEmpty(OtherLookUp))
                OtherLookUp = "";

            if (Filter == null) Filter = string.Empty;

            thisStaticName = Field.StaticName;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            InitializeValues();
            if (!this.Page.ClientScript.IsClientScriptBlockRegistered("DynamicFilterListDef"))
                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "DynamicFilterListDef", "var DynamicFilterList = new Array();", true);

            if (ReadOnlyLookUp == "true")
                lookupTextBox.Enabled = false;
            
            if(!IsPostBack)
                DataBind();
        }

        public SPFieldLookup Field { get; set; }

        public string ListUrl
        {
            get { return (string) ViewState["ListUrl"]; } 
            set { ViewState["ListUrl"] = value; }
        }

        protected bool allowMultipleValues;
        public bool AllowMultipleValues
        {
            get
            {
                if (Field != null) return Field.AllowMultipleValues;
                return allowMultipleValues;
            }
            set { allowMultipleValues = value; }
            
        }

        public string SiteID
        {
            get
            {
                var list = GetList();

                return list != null ? list.ParentWeb.Site.ID.ToString() : SPContext.Current.Web.Site.ID.ToString();
            }
        }

        public string WebID
        {
            get
            {
                var list = GetList();

                return list != null ? list.ParentWeb.ID.ToString() : DefaultWebId.ToString();
            }
        }

        public bool IsAddHidden { get; set; }

        public bool IsViewHidden { get; set; }

        public string ListGuid
        {
            get
            {
                if (Field != null) return Field.LookupList;

                var list = GetList();

                return list != null ? list.ID.ToString() : string.Empty;
            }
        }

        protected bool _valuesInitialized;
        public readonly SPFieldLookupValueCollection Values = new SPFieldLookupValueCollection();

        protected void InitializeValues()
        {
            if (!_valuesInitialized)
            {
                _valuesInitialized = true;

                if (selection != null)
                {
                    var vals = Request.Params["#" + selection.ClientID];
                    if (string.IsNullOrEmpty(vals)) return;

                    var regex = new Regex(@"\[([^;]+);([^]]+)\]", RegexOptions.Compiled);
                    foreach (Match match in regex.Matches(vals))
                    {
                        Values.Add(new SPFieldLookupValue(Convert.ToInt32(match.Groups[1].Value),
                            HttpUtility.UrlDecode(match.Groups[2].Value)));
                    }
                }
                else return;
            }
        }

        public object Value
        {
            get
            {
                InitializeValues();

                if (AllowMultipleValues)
                    return !Values.Any() ? null : Values;

                return Values.FirstOrDefault();
            }
            set
            {
                _valuesInitialized = true;

                if (value == null)
                {
                    Values.Clear();
                    lookupTextBox.Text = null;
                    return;
                }

                var collection = value as SPFieldLookupValueCollection;
                if(collection != null)
                {
                    Values.AddRange(collection);
                } 
                else if(value is SPFieldLookupValue)
                {
                    Values.Add((SPFieldLookupValue)value);
                }

                if (Values.Count > 0)
                    lookupTextBox.Text = Values.First().LookupValue;
            }
        }

        public string MinLength { get; set; }

        public string MaxRows { get; set; }

        public string MaxHeight { get; set; }

        public string ValueField { get; set; }

        public string TitleField { get; set; }

        public string DescriptionFields { get; set; }

        public string Filter { get; set; }

        public string AutoPostBack { get; set; }

        public string DynamicFilter { get; set; }

        public string DynamicFilterSourceField { get; set; }

        public string ReadOnlyLookUp { get; set; }

        public string MultipleValues { get; set; }

        public string DynamicFilterClientID { get; set; }

        public string OrderBy { get; set; }

        public string OrderByASC { get; set; }

        public string thisStaticName { get; set; }

        public string ListOfList { get; set; }

        public string ListOfSites { get; set; }

        public string ListOfFields { get; set; }

        public string OtherLookUp { get; set; }

        public string GetLocalizedString(string key)
        {
            return SPUtility.GetLocalizedString("$Resources:PSELookup," + key, "PSELookup", SPContext.Current.Web.Language);
        }

        public string GetNewFormUrl()
        {
            try
            {
                var list = GetList();
                return list == null ? string.Empty : list.DefaultNewFormUrl;
            }
            catch (Exception exc)
            {
				ExceptionHelper.DUmpException(exc);
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("PSELookup", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                    exc.Message, exc.StackTrace);
                return string.Empty;
            }
        }

        public string GetDisplayFormUrl()
        {
            try
            {
                var list = GetList();
                if (list == null) return string.Empty;

                var url = list.DefaultDisplayFormUrl + "?ID=";
                if(Values.Count > 0)
                {
                    url += Values.First().LookupId;
                }
                return url;
            }
            catch (Exception exc)
            {
				ExceptionHelper.DUmpException(exc);
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("PSELookup", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected,
                    exc.Message, exc.StackTrace);
                return string.Empty;
            }
        }

        protected SPWeb theWeb;
        protected SPList theList;
        protected bool listAccessDenied;

        public Guid DefaultWebId
        {
            get
            {
                var defaultWebId = ViewState["DefaultWebId"];
                return defaultWebId == null || ((Guid) defaultWebId).Equals(Guid.Empty) 
                    ? SPContext.Current.Web.ID : (Guid) defaultWebId;
            }
            set
            {
                ViewState["DefaultWebId"] = value;
            }
        }

        protected SPWeb GetWeb()
        {
            if(theWeb == null)
                return theWeb = SPContext.Current.Site.OpenWeb(Field == null ? DefaultWebId : Field.LookupWebId);

            return theWeb;
        }

        protected SPList GetList()
        {
            if (theList != null || listAccessDenied) return theList;

            if (Field == null && string.IsNullOrEmpty(ListUrl))
                throw new ArgumentException("Field or ListUrl is not specified.");

            var web = GetWeb();
            web.Lists.ListsForCurrentUser = true;

            foreach (var list in web.Lists.Cast<SPList>())
            {
                if (Field != null)
                {
                    if (list.ID == new Guid(Field.LookupList))
                        return theList = list;
                }

                if (!string.IsNullOrEmpty(ListUrl))
                {
                    if (list.DefaultViewUrl.StartsWith(GetListUrl(web, ListUrl)))
                        return theList = list;
                }
            }


            listAccessDenied = true;
            return null;
        }

        public static string GetListUrl(SPWeb web, string listUrl)
        {
            return GetListUrl(web.ServerRelativeUrl, listUrl);
        }

        public static string GetListUrl(string webRelativeUrl, string listUrl)
        {
            if (webRelativeUrl[webRelativeUrl.Length - 1] != '/') return (webRelativeUrl + '/' + listUrl);
            return (webRelativeUrl + listUrl); // Root web case
        }
    }
}
