//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
var table;
$(document).ready(function () {
    hide_combo_proveedores(CargaFacturasWebService.get_path());
    var url_list = CargaFacturasWebService.get_path() + "/listar";
    
    table = $('#list').DataTable({
        language: window.datatableLang,
        "ajax": {
            "url": url_list,
            "type": "POST",
            "beforeSend": function (xhr) {
                xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
            },
            "draw": 1,
            "data": function (data) {
                data.order_col = data.order[0]['column'];
                data.order_dir = data.order[0]['dir'];
                data.NumVoucher = $('#MainContent_inputFactura').val();
                data.Vendkey = $('#MainContent_comboProveedores').val();
                data.RFC = $('#MainContent_inputRFC').val();
                data.POTranID = $('#MainContent_inputOrden').val();
                data.Total = $('#MainContent_inputTotal').val();
                data.Status = $('#MainContent_comboEstado').val();
            }
        },
        searching: false,
        'aLengthMenu':[[10, 25, 50, 100, -1],[10, 25, 50, 100, "All"]],
        // 'aLengthMenu':[[2, 5, 10, 100, -1],[2, 5, 10, 100, "All"]],
        // "bStateSave": true,
        "processing": true,
        "serverSide": true,
        "stateSave": true,
        "columns": [
            { "data": "Factura" , 'className': "centrar-data", "orderable": false},
            { "data": "Factura" , 'className': "centrar-data text_align_left"},
            { "data": "Proveedor" , 'className': "centrar-data text_align_left"},
            { "data": "RFC", 'className': "centrar-data" },
            { "data": "Orden" , 'className': "centrar-data"},
            { "data": "Subtotal", 'className': "centrar-data text_align_right" },
            { "data": "Impuestos", 'className': "centrar-data text_align_right" },
            { "data": "Total", 'className': "centrar-data text_align_right" },
            { "data": "Estado", 'className': "centrar-data" }
        ],

        "columnDefs": [ {
            'targets': 0,
            'checkboxes': {
                'selectRow': true
            }
        } ],
        'select': {
            selector:'td:not(:last-child)',
            'style': 'multi'
        },
        'order': [[1, 'asc']]
    });

    $('#MainContent_inputTotal').mouseleave(function (e) {
        if (IsNumber('MainContent_inputTotal')) {
            $('#error_inputTotal').css('display', 'none');
        }
        else
            $('#error_inputTotal').css('display', 'block');
    });

    $('.buscar').click(function (e) {
        if (IsNumber('MainContent_inputTotal')) {
            e.preventDefault();
            table.ajax.reload();
        }
        else
            $('#error_inputTotal').css('display', 'block');
    });
    $('.limpiar').click(function (e) {
        e.preventDefault();

        $("#MainContent_comboProveedores").val($("#MainContent_comboProveedores option:first").val()).change();
        $("#MainContent_comboEstado").val($("#MainContent_comboEstado option:first").val()).change();
        $('#MainContent_inputFactura').val('');
        $('#MainContent_inputRFC').val('');
        $('#MainContent_inputOrden').val('');
        $('#MainContent_inputTotal').val('');

        table.ajax.reload(  );
    });

    function getIds() {
        var rows_selected = table.column(0).data();
        var fechas = [];
        $.each(rows_selected, function (index, id) {

            fechas.push(id);
        });
        return fechas;
    }

    $('form').submit(function () {
        
        var ids = getIds();
        if (ids.length == 0) {
            showToastError("No existen registros para generar el reporte.");
            return false;
        }
redirectToReport(); return false;
        return true;
    });

    function redirectToReport() {
        var NumVoucher = $('#MainContent_inputFactura').val();
        var Vendkey = $('#MainContent_comboProveedores').val();
        var RFC = $('#MainContent_inputRFC').val();
        var POTranID = $('#MainContent_inputOrden').val();
        var Total = $('#MainContent_inputTotal').val();
        var Status = $('#MainContent_comboEstado').val();

        var url_report = "/Logged/Reports/CargaFacturas";
       
        url_report += "?NumVoucher=" + NumVoucher;
        url_report += "&Vendkey=" + Vendkey;
        url_report += "&RFC=" + RFC;
        url_report += "&POTranID=" + POTranID;
        url_report += "&Total=" + Total;
        url_report += "&Status=" + Status;
        window.open(url_report, '_blank');
    }
} );

