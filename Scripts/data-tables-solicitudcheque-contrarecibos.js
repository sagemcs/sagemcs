//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
var table;
$(document).ready(function () {
    hide_combo_proveedores(ContrarecibosWebService.get_path());
    var url_list = ContrarecibosWebService.get_path() + "/listar";
   
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
                data.RcptPago = $('#MainContent_inputFechaPago').val();
                data.sin_solicitud = true;
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
            { "data": "Id", 'className': "centrar-data text_align_left" },
            { "data": "Folio", 'className': "centrar-data text_align_left" },
            { "data": "Proveedor", 'className': "centrar-data text_align_left" },
            { "data": "RFC", 'className': "centrar-data text_align_left" },
            { "data": "Condiciones", 'className': "centrar-data text_align_left" },
            { "data": "Fecha_AprobacionF", 'className': "centrar-data" },
            { "data": "Fecha_Programada_Pago", 'className': "centrar-data" },
            { "data": "Total", 'className': "cetrar-data text_align_right" }
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


    $('.buscar').click(function (e) {
        e.preventDefault();
        table.ajax.reload(  );
    });

    $('.limpiar').click(function (e) {
        e.preventDefault();

        $("#MainContent_comboProveedores").val($("#MainContent_comboProveedores option:first").val()).change();
        $('#MainContent_inputRFC').val('');
        $('#MainContent_inputTotal').val('');
        $('#MainContent_inputFolio').val('');
        $('#MainContent_inputFechaRecepcion').val('');
        $('#MainContent_inputFechaPago').val('');
        table.ajax.reload(  );
    
    });

    function getSelected() {
        var rows_selected = table.column(0).checkboxes.selected();
        var ids = [];
        $.each(rows_selected, function (index, id) {
            ids.push(id);
        });
        return ids;
    }

    function getIds() {
        var rows_selected = table.column(0).data();
        var fechas = [];
        $.each(rows_selected, function (index, id) {

            fechas.push(id);
        });
        return fechas;
    }

    $('form').submit(function () {
        
        var ids = getSelected();
        
        showToastConfirmation("Desea crear la solicitud de cheque?", "", function () {
            crearSolicitudCheque(ids); return false;
        }, function () {

        });
        return false;
        var json_ids = JSON.stringify(ids);
        $('#MainContent_contrarecibos_seleccionados').val(json_ids);
        return true;
    });

    $('#btn-generar').click(function (e) {
        var ids = getIds();
        if (ids.length == 0) {
            showToastError("No existen registros para generar el reporte.");
            return false;
        } else {
            var ids = getSelected();
            if (ids.length == 0) {
                showToastError("Seleccione los contrarecibos para generar la solicitud de cheque.");
                return false;
            }
            $('#myModal').modal('show');
        }
        
    });

    function crearSolicitudCheque(ids) {
        var url_generar_solicitud = ChequeSolicitudesWebService.get_path() + "/generar";
        var comentario = $('#MainContent_inputComentario').val();
        blockUI({
            boxed: true,
            message: 'Generando solicitud de cheque...'
        });
        $.ajax({
            type: "POST",
            beforeSend: function (xhr) {   //Include the bearer token in header
                xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
            },
            url: url_generar_solicitud,
            dataType: "json",
            data: {
                ids: JSON.stringify(ids),
                comentario: comentario
            },
            success: function (respuesta) {
                $('#MainContent_inputComentario').val('');
                $('#myModal').modal('hide');
                unblockUI();
                if (respuesta.success == true) {
                    table.ajax.reload();
                    var solicitud_id = respuesta.id;
                    redirectToReport(solicitud_id);
                } else {
                    showToastError(respuesta.msg ? respuesta.msg : "No se pudo crear la solicitud de cheque.");
                }
            },
            error: function (respuesta) {
                unblockUI();
                showToastError("No se pudo crear la solicitud de cheque.");
            }
        });
    }

    function redirectToReport(id) {
        var url_report = "/Logged/Reports/SolicitudCheque?id=" + id;
        window.open(url_report, '_blank');
    }
    
} );

