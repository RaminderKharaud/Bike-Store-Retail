
$(document).ready(function () {

    let divider = $('.divider');
    let currTab;
    if ($("#pageName").val() == "Chart") {

        currTab = $("#charts");
        $('body').css('background', '#343338');
        $('.BKtitle').css('color', '#f9690e');
        $('.tabs li a').css('color', 'white');
        $('#divCopy').css('color', 'wheat');

    } else if ($("#pageName").val() == "SalesData") {
        currTab = $("#sales");
        $('body').css('background', 'white');
        $('.BKtitle').css('color', '#0588FC');
        $('.tabs li a').css('color', '#0588FC');
        $('#divCopy').css('color', 'black');
    }
    changeDivider(currTab);

    function changeDivider(element) {

        divider.css({
            left: $(element).offset().left,
            width: $(element).outerWidth()
        });
    }
    $(window).on("resize", function () {
        changeDivider(currTab);
    });

    /* auto complete feature of product name search. It uses ajax calls to get suggestions from database */
    $("#Products").autocomplete({
        clearButton: true,
        selectFirst: true,
        minLength: 2,
        source: function (request, response) {

            $.ajax({
                type: 'GET',
                url: '/Home/GetProductList',
                data: { productName: request.term },

                success: function (data) {
                    response(JSON.parse(data));
                }
            });
        },
        focus: function (event, ui) {

            $("#Products").val(ui.item.label);
            $("#ProductId").val(ui.item.value);
            return false;
        },
        select: function (event, ui) {

            $("#Products").val(ui.item.label);
            $("#ProductId").val(ui.item.value);

            return false;
        }
    });


});

var filterFormData;

//filter data with ajax and update component view
function submitFilterForm(filterData = false, pageNumber = 1) {


    if (filterData) {

        filterFormData = $("#filterForm").serialize();
    }

    data = 'PageNumber=' + pageNumber + '&' + filterFormData;

    $.ajax({
        type: 'POST',
        url: '/Home/SearchStockData',
        contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
        data: data,
        success: function (result) {
            $("#stock-grid-container").html(result);
        },
        error: function () {
            $("#stock-grid-container").html("<p>Error Loading Data</p>");
        }
    })
}
//implementation of quantity update functionality inside grid
function update(storeId, productId, quantity) {
    let quantityColumnId = storeId + '-' + productId + '-' + 'quantity';
    let updateColumnId = storeId + '-' + productId + '-' + 'update';
    let updateButtonId = storeId + '-' + productId + '-' + 'updateButton';
    let cancelButtonId = storeId + '-' + productId + '-' + 'cancelButton';
    let saveButtonId = storeId + '-' + productId + '-' + 'saveButton';
    let inputId = storeId + '-' + productId + '-' + 'input';

    let CurrVal = $('#' + quantityColumnId).html();
    
    $('#' + updateButtonId).hide();
    $('#' + quantityColumnId).html("<input id='" + inputId + "' value='" + CurrVal.trim() + "' />");
    $('#' + inputId).select();

    $('#' + cancelButtonId).show();
    $('#' + cancelButtonId).off('click');
    $('#' + cancelButtonId).on('click',function () {
        $('#' + updateButtonId).html('Update Quantity');
        $('#' + quantityColumnId).html(CurrVal.trim());
        $(this).hide();
        $('#' + saveButtonId).hide();
        $('#' + updateButtonId).show();

    });

    $('#' + inputId).off('keyup');
    $('#' + inputId).on('keyup',function () {
        var $input = $(this);
        $input.val($input.val().replace(/[^\d]+/g, ''));
    })
    $('#' + saveButtonId).show();

    $('#' + saveButtonId).off('click');
    $('#' + saveButtonId).on('click',function () {
        $('#' + cancelButtonId).hide();
        $(this).hide();
        $('#' + updateButtonId).show();
        let newQuantity = document.getElementById(inputId).value;
        if (newQuantity == parseInt(newQuantity, 10)) {

            $.ajax({
                type: 'GET',
                url: '/Home/UpdateQuantity',
                data: { "storeId": storeId, "productId": productId, "quantity": newQuantity },

                success: function () {
                    $('#' + quantityColumnId).html(newQuantity);
                    alert("Quantity Updated Successfully!");
                 //   $('#' + updateButtonId).unbind('click');
                 //  $('#' + updateButtonId).bind('click',update(storeId, productId, newQuantity));
                },
                error: function (ex) {
                    $('#' + quantityColumnId).html(quantity);
                    alert("error updating data. Server Response: " + ex.responseText);
                }

            })
        }

        else {
            alert("Quantity must be an integer value");
            $('#' + quantityColumnId).html(quantity);
        }
    });

}
//update chart based on radio button selected on chart page
function updateChart(dictionary, title) {

    let data = JSON.parse(dictionary);
    let chart = $('#barchart').data('kendoChart');
    let i = 0;
    chart.options.title.text = title;

    for (let key in data) {
        chart.options.series[i].data = data[key];
        i++;
    }
    chart.refresh();
}
