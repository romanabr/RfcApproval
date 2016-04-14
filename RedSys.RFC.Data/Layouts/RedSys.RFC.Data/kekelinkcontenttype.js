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
		var keType = urlParams['KeType'];
		SPUtility.GetSPFieldByInternalName('RFCKeLink').SetValue(urlValue).MakeReadOnly();
		SPUtility.GetSPFieldByInternalName('Title').SetValue('КЕ проведения по запросу на изменение' + urlValue).MakeReadOnly();
		SPUtility.GetSPFieldByInternalName('KeType').SetValue(keType).MakeReadOnly();
	} catch (ex) {
		alert(ex.toString());
	}
});