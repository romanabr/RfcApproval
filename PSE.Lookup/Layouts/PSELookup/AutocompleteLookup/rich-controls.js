(function ($) {
    $.fn.richlookup = function (options) {
        var tb = this;

        $.each(options, function (key, value) {
            tb.data(key, value);
        });


        tb.autocomplete({
            source: function (request, response) {
                var dynamicFilterVal = "";
                //  var dynamicValueEmpty = false;

                if (tb.data('dynamicFilter').length > 0 && $('input[type="text"][id*="' + tb.data('dynamicFilterClientID') + '"]').length > 0) {
                    // dynamicValueEmpty = true;
                    dynamicFilterVal = $('input[type="hidden"][name*="' + tb.data('dynamicFilterClientID') + '"]').val();
                    if (dynamicFilterVal != null && dynamicFilterVal != "undefined") {
                        var regres = dynamicFilterVal.match(/\d+/);
                        if (regres.length > 0)
                            dynamicFilterVal = tb.data('dynamicFilter') + ":" + regres[0];  // the : separator is used to split this value in DataService.cs, do not change !!!
                        else
                            dynamicFilterVal = tb.data('dynamicFilter');
                    } else
                        dynamicFilterVal = tb.data('dynamicFilter');
                }
                var aj = $.ajax({
                    url: '/_vti_bin/PSELookup/DataService.svc/Find',
                    dataType: 'json',
                    type: 'post',
                    contentType: 'application/json; charset=utf-8',
                    success: function (data) {
                        response($.map(data.FindResult, function (item) { return item }));
                    },
                    data: '{"siteID":"' + tb.data('siteID') + '","webID":"' + tb.data('webID')
                    + '","list":"' + tb.data('list') + '","valueField":"' + tb.data('valueField')
                    + '","titleField":"' + tb.data('titleField') + '","fields":"' + tb.data('descFields')
                    + '","maxRows":"' + tb.data('maxRows') + '","filter":"' + tb.data('filter') + '","dynamicFilter":"' + dynamicFilterVal + '","dynamicFilterSourceField":"' + tb.data('dynamicFilterSourceField')
                    + '","OrderByASC":"' + tb.data('OrderByASC') + '","orderBy":"' + tb.data('orderBy') + '","query":"' + request.term + '"}'
                });
            },
            minLength: tb.data('minLength'),
            select: function (event, ui) {
                var onSelect = tb.data('onSelect');
                ClearDepLookups(tb.data('thisName'));

                tb.addValueToRichlookup(ui.item.id, ui.item.title, true);

                if (onSelect) {
                    onSelect(ui.item.id, ui.item.title);
                }
                if (tb.data('isMultiple')) return false;
            }
        }).data("ui-autocomplete")._renderItem =
            function (ul, item) {
                if (tb.richlookupValues().length > 0 && tb.data("firstrun") == null)
                {
                    tb.data("firstrun", "1");
                    if (item.id == tb.richlookupValues()[0])
                        return $("<li></li>").data("ui-autocomplete-item", item);
                }
                else
                {
                    tb.data("firstrun", "1");
                }
                var title = item.title;
                var desc = item.description;
                if (this.term.length > 0) {
                    var highlightRegExp = new RegExp(this.term, "gi");
                    title = title.replace(highlightRegExp, "<span class='lookupHighlighting'>" + this.term + "</span>");
                    if (desc)
                        desc = desc.replace(highlightRegExp, "<span class='lookupHighlighting'>" + this.term + "</span>");
                }

                var template = "<a><span class='lookupItemTitle'>" + title + "</span>";
                if (tb.data('descFields').length > 0)
                    template += "<br /><span class='lookupItemDescription'>" + desc + "</span>";
                template += "</a>";

                return $("<li></li>").data("ui-autocomplete-item", item).append(template).appendTo(ul);
            };

        var searchHandler = function () {
            var input = tb;
            // close if already visible
            if (input.autocomplete("widget").is(":visible")) {
                input.autocomplete("close");
                return;
            }

            var minLength = input.autocomplete("option", "minLength");
            input.autocomplete("option", "minLength", 0);
            // pass empty string as value to search for, displaying all results
            input.autocomplete("search", "");
            input.autocomplete("option", "minLength", minLength);
            input.focus();
        };

        tb.keydown(function (event) {
            if (event.keyCode == 40 && !tb.data("autocomplete").menu.element.is(":visible")) {
                searchHandler();
            }
        });

        $(tb.data('lookupButton')).click(searchHandler);

        tb.parent().closest('div').mouseover(function () {
            $(tb.data('clearButton')).css("display", "block");
        });

        
        tb.parent().closest('div').mouseout(function () {
            $(tb.data('clearButton')).css("display", "none");
        });


        $(tb.data('clearButton')).click(
            function () {
                tb.clearRichlookup();
        });

        var viewButton = $(tb.data('viewButton'));
        viewButton.addClass('ui-state-disabled');
        viewButton.on('click', function (e) {
            if ($(this).is('.ui-state-disabled'))
                e.preventDefault();
        });

        $('.lookupButton').mouseenter(function () {
            $(this).addClass("ui-state-hover");
        });

        $('.lookupButton').mouseleave(function () {
            $(this).removeClass("ui-state-hover");
        });

        //http://stackoverflow.com/questions/14632046/jquery-liveclick-function-doesnt-work-for-body
        $(document).on('click', '.lookupResultRemove', function () {
            $(this).parent().remove();
        });

        //$('.lookupResultRemove').on('click', function () {
        //    $(this).parent().remove();
        //});
    };
})(jQuery);

function ClearDepLookups(Name)
{
    if (DynamicFilterList[Name] != null) {
        DynamicFilterList[Name].every(function (x) { $('#' + x[0]).val(''); ClearDepLookups(x[1]); return true; });
    }
    return true;
}

(function( $ ){
    $.fn.addValueToRichlookup = function(id, value, check) {
        var isMultiple = this.data('isMultiple');
        var selection = $(this.data('selectionID'));

        if (!isMultiple)
            selection.empty();
        else if (check && selection.children('div[item-id=' + id + ']').length > 0)
            return;
      
        var viewButton = $(this.data('viewButton'));
        viewButton.removeClass('ui-state-disabled');
        if (typeof viewButton.attr('href') !== "undefined") {
            viewButton.attr('href', viewButton.attr('href').replace( /ID=.*/ , 'ID=' + id));
        }

        var itemDiv = $('<div item-id="' + id + '"/>').appendTo(selection)
                    .append('<input type="hidden" name="' + this.data('selectionID') + '" value="[' + id + ';' + encodeURIComponent(value) + ']" />');

        if (isMultiple) {
            itemDiv.append('<span class="lookupResult">' + value + '</span>')
                    .append('<a class="lookupResultRemove ui-icon ui-icon-trash" href="javascript:;"></a>');
            selection.nextAll('.ms-formvalidation').hide();
        }
    if(check && this.data("autoPostBack")=="true")
        __doPostBack();
  };
})( jQuery );

(function( $ ){
  $.fn.clearRichlookup = function() {
      var selection = $(this.data('selectionID'));
        selection.empty();
        this.val(null);
        ClearDepLookups(this.data('thisName'));
  };
})( jQuery );

(function( $ ){
  $.fn.richlookupValues = function() {
        var selection = $(this.data('selectionID'));
        return jQuery.map(selection.children('div'), function(div){
            return $(div).attr('item-id');
        });
  };
})( jQuery );