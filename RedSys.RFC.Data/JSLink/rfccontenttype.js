$(document).ready(function () {

	SP.SOD.executeFunc("clientpeoplepicker.js", "SPClientPeoplePicker", function () {
		SP.SOD.executeFunc("clienttemplates.js", "SPClientTemplates", function () {
			SP.SOD.executeFunc("sp.js", "SP.ClientContext", LoadCurrentUser);
		});
	});
});

function LoadCurrentUser() {
	var Name = SPUtility.GetSPField('Name');
	Name.LabelRow.innerText = 'Номер изменения';
	Name.SetValue('Будет заполнен автоматически').MakeReadOnly();
	var Type = SPUtility.GetSPFieldByInternalName('RFCType');
	$(Type.LookupSelection).bind('DOMNodeInserted', SetDependentField);
	$(Type.LookupSelection).bind('DOMNodeRemoved', ClearField);
}

function SetDependentField() {
	var Type = SPUtility.GetSPFieldByInternalName('RFCType');
	var Manager = SPUtility.GetSPFieldByInternalName("RFCManager");
	var fieldValue = Type.GetValue();
	if (fieldValue == "undefined") {
		Manager.SetValue().MakeEditable();
		return;
	};
	var query = new CamlBuilder().Where().LookupField("RFCType").Id().EqualTo(Type.GetValueID()).ToString().replace('<Where>', '').replace('</Where>', '')
	$SP().list("Менеджеры RFC").get({
		fields: Manager.InternalName,
		where: query,
		whereCAML: true
	}, function getData(data) {
		for (var i = 0; i < data.length; i++) { Manager.SetValue(data[i].getAttribute(Manager.InternalName).split('#')[1]).MakeReadOnly() }
	});
}

function ClearField() {
	var Manager = SPUtility.GetSPFieldByInternalName("RFCManager");
	Manager.SetValue('').MakeEditable();
}