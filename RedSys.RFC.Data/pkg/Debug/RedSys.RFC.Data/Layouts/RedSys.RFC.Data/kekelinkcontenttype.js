(window.onpopstate = function () {
	var match,
		pl = /\+/g,  // Regex for replacing addition symbol with a space
		search = /([^&=]+)=?([^&]*)/g,
		decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
		query = window.location.search;
	// fix for sharepoint 2013 URLs which use the hash
	if (query === '') {
		query = window.location.hash.substring(window.location.hash.indexOf('?'));
	}
	query = query.substring(1);
	urlParams = {};
	while (match = search.exec(query))
		urlParams[decode(match[1])] = decode(match[2]);
})();
// wait for the window to load
$(window).load(function () {
	try {
		var urlValue = urlParams['ItemID'];
		var typeValue = urlParams["KeType"];
		var rfcLink = SPUtility.GetSPFieldByInternalName('RFCKeLink').SetValue(parseInt(urlValue)).Hide();
		var title = SPUtility.GetSPFieldByInternalName('Title').SetValue(rfcLink.GetValue()).MakeReadOnly().Hide();
		var keLink = SPUtility.GetSPFieldByInternalName('KeKeLink');
		title.SetValue(rfcLink.GetValue() + "-" + keLink.GetValue());
		$(keLink.Dropdown).change(function () { title.SetValue(rfcLink.GetValue() + "-" + keLink.GetValue()); });
		var keType = SPUtility.GetSPFieldByInternalName('RFCKeType').SetValue(typeValue).Hide();
	} catch (ex) {
		alert(ex.toString());
	}
});