using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Threading;
using PSELookup;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client.Services;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Microsoft.SharePoint.Administration;
using RedSys.RFC.Core.Helper;

namespace PSELookup.Services
{
    [ServiceContract]
    public interface IDataService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", 
            BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        FindResult[] Find(string siteID, string webID, string list, string valueField, string titleField, string fields, string maxRows, string filter, string dynamicFilter, string dynamicFilterSourceField, string OrderByASC, string orderBy, string query);
    }

    [DataContract]
    public class FindResult
    {
        [DataMember(Name = "id")]
        public string ID { get; set; }

        [DataMember(Name="value")]
        public string Value { get; set; }

        [DataMember(Name="title")]
        public string Title { get; set; }

        [DataMember(Name="description")]
        public string Description { get; set; }

        public static FindResult Create(SPListItem item, string valueField, string titleField, string[] fields)
        {
            return new FindResult
                             {
                                 ID = Convert.ToString(item.ID),
                                 Value = GetItemFieldValue(item, valueField),
                                 Title = GetItemFieldValue(item, titleField),
                                 Description = string.Join("&nbsp;&nbsp;&nbsp;", fields.Select(f => GetItemFieldValue(item, f)).Where(f => !string.IsNullOrEmpty(f)).ToArray())
                             };
        }

        protected static string GetItemFieldValue(SPListItem item, string fieldName)
        {
            fieldName = fieldName.Trim();

            if (item[fieldName] == null) return string.Empty;

            var field = item.Fields.GetField(fieldName);

            switch(field.Type)
            {
                case SPFieldType.User:
                case SPFieldType.Lookup:
                case SPFieldType.Calculated:
                    return item[fieldName].ToString().Split(new[] { ";#" }, StringSplitOptions.None)[1];
                case SPFieldType.Boolean:
                    var val = Convert.ToBoolean(item[fieldName]);
                    return !val ? string.Empty : string.Format("{0}:{1}", field.Title, SPFieldBoolean.GetFieldValueAsText(val));
                case SPFieldType.URL:
                    SPFieldUrl urlImage = (SPFieldUrl)field;
                    if (urlImage.DisplayFormat == SPUrlFieldFormatType.Image)
                    {
                        SPFieldUrlValue fieldValue = new SPFieldUrlValue(item[fieldName].ToString());
                        return "<img style='max-width:150px;' src='" + fieldValue.Url + "'>";
                    }
                    var urlValue = item[fieldName] as SPFieldLookupValue;
                    return urlValue != null ? urlValue.LookupValue : Convert.ToString(item[fieldName]);


                default:
                    var lookupValue = item[fieldName] as SPFieldLookupValue;
                    return lookupValue != null ? lookupValue.LookupValue : Convert.ToString(item[fieldName]);
            }
        }
    }

    [BasicHttpBindingServiceMetadataExchangeEndpointAttribute]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class DataService : IDataService
    {
        public FindResult[] Find(string siteID, string webID, string list, string valueField, string titleField, string fields, string maxRows, string filter, string dynamicFilter, string dynamicFilterSourceField, string OrderByASC, string orderBy, string query)
        {
            try
            {
                using (var spSite = new SPSite(new Guid(siteID)))
                {
                    using (var spWeb = spSite.OpenWeb(new Guid(webID)))
                    {
                        if (string.IsNullOrEmpty(list)) return new FindResult[0];

                        var spList = spWeb.Lists[new Guid(list)];

                        var flds = new List<string>();

                        if (!string.IsNullOrEmpty(fields))
                            flds.AddRange(fields.Split(';'));

                        var spQuery = new SPQuery { RowLimit = Convert.ToUInt32(maxRows) };

                        object filterQuery = null;
                        object dynamicFilterQuery = null;
                        object termQuery = null;
                        if (!string.IsNullOrEmpty(filter))
                        {
                            filterQuery = XElement.Parse(parseFilterQuery(filter, spList));
                        }
                        if (!string.IsNullOrEmpty(dynamicFilter))
                        {
                            string[] dfValues = dynamicFilter.Split(':');
                            if (dfValues.Length > 0)
                            {
                                // if dynamicfilter was specified, but value was not seleced in associated rich lookup control, then do not return anything
                                if (dfValues.Length == 1)
                                {
                                    return new[] {
                                        new FindResult {
                                       ID = 0.ToString(),
                                       Title = string.Format("Сначала выберите значение в поле '{0}'", dfValues[0]),
                                       Description = string.Empty,
                                       Value = string.Format("Сначала выберите значение в поле '{0}'", dfValues[0]) }
                                    };
                                }

                                string dynamicKey = "";

                                if (spList.Fields.ContainsField(dfValues[0]))
                                {
                                    dynamicKey = spList.Fields.GetField(dfValues[0]).InternalName;
                                }

                                if (!string.IsNullOrEmpty(dynamicFilterSourceField))
                                {
                                    if (spList.Fields.ContainsField(dynamicFilterSourceField))
                                    {
                                        dynamicKey = spList.Fields.GetField(dynamicFilterSourceField).InternalName;
                                    }
                                }

                                if (dynamicKey == "")
                                {
                                    return new[] {
                                        new FindResult {
                                       ID = 0.ToString(),
                                       Title = string.Format("Cascade field doesn't exists in source list!"),
                                       Description = string.Empty,
                                       Value = string.Format("Cascade field doesn't exists in source list!") }
                                    };
                                }

                                // instead of <Eq> for exact match use <Contains> for finding, if the ID is between all associated lookups, so if multiple is allowed on that field, it will find correct result, if somebody looks forit
                                dynamicFilter = "<Eq><FieldRef Name='" + dynamicKey + "' LookupId=\"TRUE\" /><Value Type='Lookup'>" + dfValues[1] + "</Value></Eq>";
                                dynamicFilterQuery = XElement.Parse(dynamicFilter);
                                
                            }
                        }

                        if (!string.IsNullOrEmpty(query))
                        {
                            var rootOr = new XElement("Or");
                            var or = rootOr;

                            var internalFields =
                                new[] {titleField}.Union(flds).Distinct().Select(f => spList.Fields.GetField(f.Trim()).InternalName).
                                    ToList();

                            for (int i = 0; i < internalFields.Count; i++)
                            {
                                or.Add(new XElement("Contains",
                                                    new XElement("FieldRef", new XAttribute("Name", internalFields[i])),
                                                    new XElement("Value", new XAttribute("Type", "Text"), query)));
                                if (i < internalFields.Count - 2)
                                {
                                    var old = or;
                                    or = new XElement("Or");
                                    old.Add(or);
                                }
                            }

                            if (internalFields.Count > 1) termQuery = rootOr;
                            else termQuery = rootOr.Elements();
                        }

                        var where = new XElement("Where");
                        var container = where;

                        // add two <and> to the caml, because 3 condition will be <and><cond1><and><cond2><cond3></and></and>
                        if (filterQuery != null && termQuery != null && dynamicFilterQuery != null)
                        {
                            var and = new XElement("And");  // first and, if all three will be present
                            container.Add(and);
                            container = and;

                            container.Add(filterQuery); // first condition

                            var and2 = new XElement("And"); // second and, to group cond2 and cond3
                            container.Add(and2);
                            container = and2;

                            container.Add(termQuery);   // condition 1 & 2
                            container.Add(dynamicFilterQuery);
                        }
                        else
                        {
                            int notnulls = 0;
                            if (filterQuery != null)
                                notnulls++;
                            if (termQuery != null)
                                notnulls++;
                            if (dynamicFilterQuery != null)
                                notnulls++; // this should be allways < 3, because otherwise first if condition with all 3 != null would be fullfiled

                            if (notnulls == 2)  // if we have 2 conditions, then we need another <and>, otherwise not
                            {
                                var and = new XElement("And");
                                container.Add(and);
                                container = and;
                            }

                            if (filterQuery != null)
                                container.Add(filterQuery);
                            if (termQuery != null)
                                container.Add(termQuery);
                            if (dynamicFilterQuery != null)
                                container.Add(dynamicFilterQuery);
                        }

                        if (!where.IsEmpty)
                            spQuery.Query = where.ToString();

                        if (!string.IsNullOrEmpty(orderBy))
                            spQuery.Query += parseOrderByQuery(orderBy, OrderByASC, spList);

                        spQuery.ViewAttributes = "Scope=\"RecursiveAll\"";

                        var items = spList.GetItems(spQuery);

                        return (from SPListItem item in items select FindResult.Create(item, valueField, titleField, flds.ToArray())).ToArray();
                    }
                }
            }
            catch (Exception exc)
            {
				ExceptionHelper.DUmpException(exc);
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("PSELookup", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, 
                    exc.Message, exc.StackTrace, list, valueField, titleField, fields, maxRows, filter, query);

                var message = string.Format("{0}:{1},{2},{3},{4},{5},{6},{7}",
                                            exc.Message, exc.StackTrace, list, valueField, titleField, fields, filter,
                                            query);
                return new[]
                           {
                               new FindResult
                                   {
                                       ID = 0.ToString(),
                                       Title = message,
                                       Description = string.Empty,
                                       Value = message
                                   }
                           };
            }
        
        }

        public string parseFilterQuery(string Filter, SPList list)
        {
            string newFilter = "";
            foreach (string filterPart in Filter.Split(';'))
            {
                string[] eq = filterPart.Split('=');
                string Fname = eq[0];
                string Fval = eq[1];
                if (list.Fields.ContainsField(Fname))
                {
                    string Fstring = "<Eq><FieldRef Name='" + list.Fields[Fname].InternalName + "' /><Value Type='Text'>" + Fval + "</Value></Eq>";
                    if (newFilter == "")
                        newFilter = Fstring;
                    else
                        newFilter = "<And>" + newFilter + Fstring + "</And>";
                }
            }
            return newFilter;
        }

        public string parseOrderByQuery(string OrderBy,string OrderByASC, SPList list)
        {
            string newOrderBy = "";
            foreach (string OrderByPart in OrderBy.Split(';'))
            {
                if (list.Fields.ContainsField(OrderByPart))
                {
                    newOrderBy = "<FieldRef Name='" + list.Fields[OrderByPart].InternalName + "' Ascending='" + OrderByASC + "'/>";
                }
            }
            newOrderBy = "<OrderBy>" + newOrderBy + "</OrderBy>";
            return newOrderBy;
        }
    }
}
