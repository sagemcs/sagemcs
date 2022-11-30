//PORTAL DE PROVEDORES T|SYS|
//12 MARZO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
var table;
$(document).ready(function () {
    hide_combo_proveedores(ContrarecibosWebService.get_path());
    var url_list = ContrarecibosWebService.get_path() + "/listarC";
    
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
                data.Folio = $('#MainContent_inputFolio').val();
                data.VendID = $('#MainContent_comboProveedores').val();
                data.AliasDBA = $('#MainContent_inputRFC').val();
                data.Total = $('#MainContent_inputTotal').val();
                data.RcptDate = $('#MainContent_inputFechaRecepcion').val();
                data.RcptPago = $('#MainContent_inputPago').val();
                data.sin_solicitud = false;
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
            { "data": "Id", 'className': "centrar-data " },
            { "data": "Folio", 'className': "centrar-data " },
            { "data": "Proveedor", 'className': "centrar-data text_align_left" },
            { "data": "RFC", 'className': "centrar-data text_align_left" },
            { "data": "Condiciones" , 'className': "centrar-data text_align_left"},
            { "data": "Fecha_Recepcion", 'className': "centrar-data"},
            { "data": "Fecha_Programada_Pago", 'className': "centrar-data" },
            { "data": "Total", 'className': "centrar-data text_align_right" },
            { "data": "actions", 'className': "centrar-data", "orderable": false }
        ],

        "columnDefs": [ {
            'targets': 0,
            'checkboxes': {
                'selectRow': true
            }
        }, {
            "targets": -1,
            "data": null,
            "render": function (data, type, row) {
                var render = "";
                render += '<a class="btn btn-xs btn-success" title="Exportar reporte" data-tootle="tooltip" data-id="' + row.Id + '" href="/Logged/Reports/Contrarecibo?id=' + row.Id + '" target="_blank"><span class="glyphicon glyphicon-download" aria-hidden="true"></span></a>';
                //render += '<a class="btn btn-xs btn-danger" title="Imprimir reporte" data-tootle="tooltip" data-id="' + row.Id + '" href="/Logged/Reports/Contrarecibo?id=' + row.Id + '&print=true" target="_blank"><span class="glyphicon glyphicon-print" aria-hidden="true"></span></a>';
                return render;
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
        $('#MainContent_inputFolio').val('');
        $('#MainContent_inputRFC').val('');
        $('#MainContent_inputTotal').val('');
        $('#MainContent_inputFechaRecepcion').val('');
        $('#MainContent_inputPago').val('');
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
        var folio = $('#MainContent_inputFolio').val();
        var provId = $('#MainContent_comboProveedores').val();
        var rfc = $('#MainContent_inputRFC').val();
        var total = $('#MainContent_inputTotal').val();
        var fecha = $('#MainContent_inputFechaRecepcion').val();
        var fechaPago = $('#MainContent_inputPago').val();

        var url_report = "/Logged/Reports/Contrarecibos";
       
        url_report += "?folio=" + folio;
        url_report += "&provId=" + provId;
        url_report += "&total=" + total;
        url_report += "&rfc=" + rfc;
        url_report += "&fechaPago=" + fechaPago;
        url_report += "&fecha=" + fecha;
        window.open(url_report, '_blank');
    }
} );

