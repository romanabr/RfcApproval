(function () {

	var filteredLookupContext = {};

	// Init Templates object for View mode
	filteredLookupContext.Templates = {};
	filteredLookupContext.Templates.Fields = {
	    "PSELookupField": { "View": filteredLookupViewTemplate } // pass rendering control to delegate function filteredLookupViewTemplate
	};
	// register rendering overriding
	SPClientTemplates.TemplateManager.RegisterTemplateOverrides(filteredLookupContext);
})();

function filteredLookupViewTemplate(ctx) {

    var arr = [];
    var fld = ctx.CurrentItem[ctx.CurrentFieldSchema.Name];
    for (i = 0; i < fld.length; i++) {

        var link = ctx.CurrentFieldSchema.DispFormUrl + '&ID=' + fld[i].lookupId;

        arr.push('<a href=\'' + link + '\' target=\'_blank\'>' + fld[i].lookupValue + '</a>');
    }
    return arr.join("; ");

	//var lookup = ctx.CurrentItem[ctx.CurrentFieldSchema.Name];
	//// read lookup value and returns as string
	//if (lookup)
	//	return lookup[0].lookupValue;
	//return "";
}