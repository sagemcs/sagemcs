//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
var table;
$(document).ready(function () {
    hide_combo_proveedores(ChequeSolicitudesWebService.get_path());
    var url_list = ChequeSolicitudesWebService.get_path() + "/listar";
    //var Pago = document.getElementById("inputaPago").value;
    //var Fecha = document.getElementById("inputFecha").value;
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
                data.Serie = $('#MainContent_inputSerie').val();
                data.VendID = $('#MainContent_comboProveedores').val();
                data.UserID = $('#MainContent_comboSolicitantes').val();
                data.Total = $('#MainContent_inputTotal').val();
                data.ChkReqDate = $('#MainContent_inputFecha').val();
                data.PagoDate = $('#MainContent_inputPago').val();
            
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
            { "data": "Id", 'className': "centrar-data" },
            { "data": "Serie", 'className': "centrar-data text_align_left" },
            { "data": "Proveedor", 'className': "centrar-data text_align_left" },
            { "data": "NombreSolicitante", 'className': "centrar-data text_align_left" },
            { "data": "Total", 'className': "cetrar-data text_align_right" },
            { "data": "Fecha", 'className': "centrar-data" },
            { "data": "Fecha_Programada_Pago", 'className': "centrar-data" },
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
                render += '<a class="btn btn-xs btn-success" title="Exportar reporte" data-tootle="tooltip" data-id="' + row.Id + '" href="/Logged/Reports/SolicitudCheque?id=' + row.Id + '" target="_blank"><span class="glyphicon glyphicon-download" aria-hidden="true"></span></a>';

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
        $("#MainContent_comboSolicitantes").val($("#MainContent_comboSolicitantes option:first").val()).change();
        $('#MainContent_inputSerie').val('');
        $('#MainContent_inputTotal').val('');
        $('#MainContent_inputFecha').val('');
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
        var serie = $('#MainContent_inputSerie').val();
        var provId = $('#MainContent_comboProveedores').val();
        var userId = $('#MainContent_comboSolicitantes').val();
        var total = $('#MainContent_inputTotal').val();
        var fecha = $('#MainContent_inputFecha').val();
        var fechaP = $('#MainContent_inputPago').val();

        var url_report = "/Logged/Reports/SolicitudesCheque";

        url_report += "?serie=" + serie;
        url_report += "&fecha=" + fecha;
        url_report += "&fechaP=" + fechaP;
        url_report += "&provId=" + provId;
        url_report += "&userId=" + userId;
        url_report += "&total=" + total;
        window.open(url_report, '_blank');
    }
} );

