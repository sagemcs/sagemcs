//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
var table;
$(document).ready(function () {
    var url_list = CargaArticulosWebService.get_path() + "/listar";
    
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
                data.ItemID = $('#MainContent_inputArticulo').val();
                data.Qty = $('#MainContent_inputCantidad').val();
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
            { "data": "Articulo" , 'className': "centrar-data", "orderable": false},
            { "data": "Articulo" , 'className': "centrar-data text_align_left"},
            { "data": "Cantidad" , 'className': "centrar-data text_align_left"},
            { "data": "Costo", 'className': "centrar-data text_align_right" },
            { "data": "Monto", 'className': "centrar-data text_align_right" },
            { "data": "Comentario", 'className': "centrar-data" },
            { "data": "Estado" , 'className': "centrar-data"}
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

    $('#MainContent_inputCantidad').mouseleave(function (e) {
        if (IsNumber('MainContent_inputCantidad')) {
            $('#error_inputCantidad').css('display', 'none');
        }
        else
            $('#error_inputCantidad').css('display', 'block');
    });

    $('.buscar').click(function (e) {
        if (IsNumber('MainContent_inputCantidad')) {
            e.preventDefault();
            table.ajax.reload();
        }
        else
            $('#error_inputCantidad').css('display', 'block');
        
    });
    $('.limpiar').click(function (e) {
        e.preventDefault();

        $("#MainContent_comboEstado").val($("#MainContent_comboEstado option:first").val()).change();
        $('#MainContent_inputArticulo').val('');
        $('#MainContent_inputCantidad').val('');
        $('#MainContent_inputImpuesto').val('');
       
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
        var ItemID = $('#MainContent_inputArticulo').val();
        var Qty = $('#MainContent_inputCantidad').val();
        var TaxID = $('#MainContent_inputImpuesto').val();
        var Status = $('#MainContent_comboEstado').val();

        var url_report = "/Logged/Reports/CargaArticulos";
       
        url_report += "?ItemID=" + ItemID;
        url_report += "&Qty=" + Qty;
        url_report += "&TaxID=" + TaxID;
        url_report += "&Status=" + Status;
        window.open(url_report, '_blank');
    }

} );

