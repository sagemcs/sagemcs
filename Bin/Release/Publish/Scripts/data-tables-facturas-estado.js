//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
var table;
$(document).ready(function () {
    hide_combo_proveedores(FacturasWebService.get_path());
    var url_list = FacturasWebService.get_path() + "/listar_estado_factura";
    
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
                data.VendorId= $('#MainContent_comboProveedores').val();
                data.Folio = $('#MainContent_inputFolio').val();
                data.Fecha = $('#MainContent_inputFecha').val();
                data.FechaR = $('#MainContent_inputFechaR').val();
                data.contrarecibo = $('#MainContent_inputContrarecibo').val();
                data.solicitud = $('#MainContent_inputSolicitud').val();               
                data.Estado = $('#MainContent_comboEstado').val();
            }
        },
        searching: false,
        'aLengthMenu':[[10, 25, 50, 100, -1],[10, 25, 50, 100, "All"]],
        "processing": true,
        "serverSide": true,
        "stateSave": true,
        "columns": [
            { "data": "UUID", 'className': "centrar-data", "orderable": false },
            { "data": "Folio" , 'className': "centrar-data text_align_left"},
            { "data": "Serie", 'className': "centrar-data" },
            { "data": "Proveedor_Nombre", 'className': "centrar-data" },
            { "data": "Fecha" , 'className': "centrar-data"},
            { "data": "Fecha_Recepcion", 'className': "centrar-data" },
            { "data": "Total", 'className': "cetrar-data text_align_right" },
            { "data": "Contrarecibo_Folio", 'className': "centrar-data" },
            { "data": "Solicitud_Folio", 'className': "centrar-data" },
            { "data": "Estado", 'className': "centrar-data" },
            { "data": "Estado_Img", 'className': "centrar-data" },
        ],
        //"fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
        //    if (aData["Estado"] == "Pendiente") {
        //        $('td', nRow).css('background-color', '#f2dede');
        //    }
        //    else if (aData["Estado"] == "Aprobado") {
        //        $('td', nRow).css('background-color', '#dff0d8');
        //    }
        //    else if (aData["Estado"] == "Cancelado") {
        //        $('td', nRow).css('background-color', '#fcf8e3');
        //    }
        //    else {
        //        $('td', nRow).css('background-color', '#d9edf7');
        //    }
        //},
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
        $("#MainContent_comboEstado").val($("#MainContent_comboEstado option:first").val()).change();
        $('#MainContent_inputFolio').val('');
        $('#MainContent_inputFecha').val('');
        $('#MainContent_inputFechaR').val('');
        $('#MainContent_inputContrarecibo').val('');
        $('#MainContent_inputSolicitud').val('');

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
        console.log(ids);
    
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
        var fecha = $('#MainContent_inputFecha').val();
        var fechaR = $('#MainContent_inputFechaR').val();
        var contrarecibo = $('#MainContent_inputContrarecibo').val();
        var solicitud = $('#MainContent_inputSolicitud').val();
        var estado = $('#MainContent_comboEstado').val();

        var url_report = "/Logged/Reports/FacturasEstado";
        
        url_report += "?folio=" + folio;
        url_report += "&fecha=" + fecha;
        url_report += "&fechaR=" + fechaR;
        url_report += "&provId=" + provId;
        url_report += "&contrarecibo=" + contrarecibo;
        url_report += "&solicitud=" + solicitud;
        url_report += "&estado=" + estado;
        window.open(url_report, '_blank');
    }    
} );

