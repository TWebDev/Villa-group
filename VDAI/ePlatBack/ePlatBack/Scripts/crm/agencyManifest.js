

var AMANIFEST = function () {

    var oManifestTable;

    var init = function () {
        COMMON.getServerDateTime();

        $('#ManifestSearch_Date').on('keydown', function (e) {
            e.preventDefault();
        });

        $('#ManifestSearch_Date').datepicker({
            dateFormat: 'yy-mm-dd',
            minDate: -22,
            maxDate: 0,
            onClose: function (dateText, inst) {
                if (dateText != '') {
                    localStorage.Eplat_AgencyManifest_Request = new Date($('#ManifestSearch_Date').datepicker('getDate'));
                    AMANIFEST.getAgencyManifest(dateText);
                }
            }
        });

        $('#btnShowManifest').on('click', function () {
            $('.btn-show-manifest').toggle();
            $('.manifest-related').toggle();
            $('#ManifestSearch_Date').datepicker('setDate', COMMON.getDate());
            $('#ManifestSearch_Date').datepicker('refresh');
            $('#ManifestSearch_Date').datepicker('show');
            $('#ManifestSearch_Date').datepicker('hide');
            $('#ManifestSearch_Date').trigger('blur');
            //localStorage.Eplat_AgencyManifest_Request contiene la última fecha usada como parámetro
            //comparar fecha usada en parámetro con fecha del campo del formulario.
            if (localStorage.Eplat_AgencyManifest_Request != undefined && localStorage.Eplat_AgencyManifest_Request != null && localStorage.Eplat_AgencyManifest_Request != '') {
                $('#manifestRequestDate').text(new Date(localStorage.Eplat_AgencyManifest_Request).toDateString());
                if (new Date(localStorage.Eplat_AgencyManifest_Request).toDateString() != new Date().toDateString()) {
                    $('#ManifestSearch_Date').addClass('mb-warning');
                }
                else {
                    $('#ManifestSearch_Date').removeClass('mb-warning');
                }
            }
            else {
                if (new Date(localStorage.Eplat_AgencyManifest_Update).toDateString() != new Date().toDateString()) {
                    $('#ManifestSearch_Date').addClass('mb-warning');
                }
                else {
                    $('#ManifestSearch_Date').removeClass('mb-warning');
                }
                //$('#ManifestSearch_Date').removeClass('mb-warning');
            }
            if (localStorage.Eplat_Agency_Manifest == undefined || localStorage.Eplat_Agency_Manifest == null || localStorage.Eplat_Agency_Manifest == '') {
                localStorage.Eplat_AgencyManifest_Request = new Date($('#ManifestSearch_Date').datepicker('getDate'));
                AMANIFEST.getAgencyManifest($('#ManifestSearch_Date').val());
            }
            else {
                AMANIFEST.renderAgencyManifest();
                $('#displayLastUpdate').text(localStorage.Eplat_AgencyManifest_Update);
            }
            $('.manifest-related').show();
        });

        $('#btnGetManifest').on('click', function () {
            $('#btnResetQuickSale').trigger('click');
            localStorage.Eplat_AgencyManifest_Request = new Date($('#ManifestSearch_Date').datepicker('getDate'));
            AMANIFEST.getAgencyManifest($('#ManifestSearch_Date').val());
        });

        $('#btnHideManifest').on('click', function () {
            $('.btn-show-manifest').toggle();
            $('.manifest-related').toggle();
            if ($('#frmEgressInfo').length > 0) {
                $('#frmEgressInfo').clearForm();
            }
            if ($('#fastSaleFieldset').length > 0) {
                $('#fastSaleFieldset').clearForm();
            }
        });
    }

    var getAgencyManifest = function (date) {
        $.ajax({
            url: '/SPI/GetManifestForAgency',
            cache: false,
            type: 'POST',
            data: { date: date },
            success: function (data) {
                //localStorage.Eplat_AgencyManifest_Request = new Date(date);
                localStorage.Eplat_Agency_Manifest = data;
                AMANIFEST.renderAgencyManifest();
                var _date = new Date();
                localStorage.Eplat_AgencyManifest_Update = _date.toDateString() + ' ' + _date.toLocaleTimeString();
                $('#displayLastUpdate').text(localStorage.Eplat_AgencyManifest_Update);
                $('#manifestRequestDate').text(new Date(localStorage.Eplat_AgencyManifest_Request).toDateString());
                if (new Date(localStorage.Eplat_AgencyManifest_Request).toDateString() != _date.toDateString()) {
                    $('#ManifestSearch_Date').addClass('mb-warning');
                }
                else {
                    $('#ManifestSearch_Date').removeClass('mb-warning');
                }
            }
        });
    }

    var renderAgencyManifest = function () {
        var data = new Array();
        var json = $.parseJSON(localStorage.Eplat_Agency_Manifest);
        $.each(json, function (index, item) {
            data = data.concat(item);
        });

        AMANIFEST.oManifestTable = $('#tblManifest').dataTable({
            "bDestroy": true,
            "bFilter": true,
            "bProcessing": true,
            "bAutoWidth": false,
            "oLanguage": {
                "oPaginate": {
                    "sPrevious": "",
                    "sNext": ""
                }
            },
            "aaData": eval(data),//this line will change in order of the new object's model
            "aoColumnDefs": [
                {
                    "aTargets": [0],
                    "bVisible": false
                },
                {
                    "aTargets": [5],
                    "bVisible": false
                },
                {
                    "aTargets": [6],
                    "bVisible": false
                },
                {
                    "aTargets": [7],
                    "bVisible": false
                },
                {
                    "aTargets": [9],
                    "bVisible": false
                },
                {
                    "aTargets": [10],
                    "bVisible": false
                },
                {
                    "aTargets": [11],
                    "bVisible": false
                },
                {
                    "aTargets": [12],
                    "bVisible": false
                }
            ],
            "aoColumns": [
                { "mData": "CustomerID" },
                { "mData": "FirstName" },
                { "mData": "LastName" },
                { "mData": "Country" },
                { "mData": "MarketingProgram" },
                { "mData": "Subdivision" },
                { "mData": "Source" },
                { "mData": "OPCID" },
                { "mData": "OPC" },
                { "mData": "FrontOfficeGuestID" },
        { "mData": "FrontOfficeResortID" },
            { "mData": "TourID" },
            { "mData": "TourDate" }],
            "aaSorting": [[0, 'desc']]
            //"aoRowCallback": [UI.applyFormat('currency', 'tblSearchCouponsResults')]
        });
        //UI.applyFormat('currency', 'tblSearchCouponsResults');
        UI.setTableRowsClickable({
            tblID: "tblManifest",
            onClickCallbackFunction: AMANIFEST.loadManifestRow
        });
    }

    var loadManifestRow = function (params) {
        $('#btnNewEgressInfo').trigger('click');
        var array = AMANIFEST.oManifestTable.fnGetNodes();
        var nTr = AMANIFEST.oManifestTable.$('tr.selected-row');
        var position = AMANIFEST.oManifestTable.fnGetPosition(nTr[0]);
        var row = AMANIFEST.oManifestTable.fnGetData(array[position]);
        
        $('#CustomerID').val(row.CustomerID);
        $('#Country').val(row.Country);
        $('#FrontOfficeGuestID').val(row.FrontOfficeGuestID);
        $('#FrontOfficeResortID').val(row.FrontOfficeResortID);
        $('#MarketingProgram').val(row.MarketingProgram);
        $('#OPC').val(row.OPC);
        $('#OPCID').val(row.OPCID);
        $('#Source').val(row.Source);
        $('#TourID').val(row.TourID);
        $('#TourDate').val(row.TourDate);
        $('#Subdivision').val(row.Subdivision);
        $('#FastSaleInfo_FirstName').val(row.FirstName);
        $('#FastSaleInfo_LastName').val(row.LastName);
        var source = $('#FastSaleInfo_LeadSource option').filter(function () { var text = $(this).html().toLowerCase().trim().split('-').join('').split(' ').join(''); return row.MarketingProgram.toLowerCase().trim().split('-').join().split(' ').join('').indexOf(text) != -1; }).val();
        $('#FastSaleInfo_LeadSource option[value="' + source + '"]').attr('selected', true);
        if (row.Country.toLowerCase().indexOf('usa') != -1 || row.Country.toLowerCase().indexOf('canada') != -1) {
            $('#FastSaleInfo_Language option[value="en-US"]').attr('selected', true).trigger('change');
        }
        else {
            $('#FastSaleInfo_Language option[value="es-MX"]').attr('selected', true).trigger('change');
        }

        $('#OutcomeInfo_Customer').val(row.FirstName + ' ' + row.LastName);
        if (row.OPC != null && row.OPC != '') {
            var _opc = $('#OutcomeInfo_Opc option').filter(function () { var text = $(this).html().split('  ').join(' ').toLowerCase().trim(); return text == row.OPC.toLowerCase().trim(); }).val();
            if (_opc != undefined) {
                $('#OutcomeInfo_Opc option[value="' + _opc + '"]').attr('selected', true);
                $('#OutcomeInfo_Opc').multiselect('refresh');
                $('#OutcomeInfo_Opc').trigger('change');
            }
            else {
                $('#OutcomeInfo_Opc option[value=""]').attr('selected', true);
                $('#OutcomeInfo_Opc').multiselect('refresh');
                $('#OutcomeInfo_Opc').trigger('change');
            }
        }
        //if ($('#fdsEgressInfo').length > 0) {
        //    UI.expandFieldset('fdsEgressInfo');
        //    UI.scrollTo('fdsEgressInfo', null);
        //}
    }

    return {
        init: init,
        getAgencyManifest: getAgencyManifest,
        renderAgencyManifest: renderAgencyManifest,
        loadManifestRow: loadManifestRow
    }
}();

$(function () {
    AMANIFEST.init();
});