//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
var table;
$(document).ready(function () {
    hide_combo_proveedores(UsuariosWebService.get_path());
    var url_list = UsuariosWebService.get_path() + "/listar";
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
                data.username = $('#MainContent_inputCorreo').val();
                data.VendName = $('#MainContent_inputNombre').val();
                data.Interno = $('#MainContent_comboInterno').val();
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
            { "data": "Correo", 'className': "centrar-data", "orderable": false },
            { "data": "Correo", 'className': "centrar-data" },
            { "data": "Nombre", 'className': "centrar-data text_align_left" },
            //{ "data": "Proveedor" , 'className': "centrar-data"},
            { "data": "Interno", 'className': "centrar-data" },
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


    $('.buscar').click(function (e) {
        e.preventDefault();
        table.ajax.reload(  );
    });
    $('.limpiar').click(function (e) {
        e.preventDefault();

        $("#MainContent_comboProveedores").val($("#MainContent_comboProveedores option:first").val()).change();
        $("#MainContent_comboInterno").val($("#MainContent_comboInterno option:first").val()).change();
        $("#MainContent_comboEstado").val($("#MainContent_comboEstado option:first").val()).change();
        $('#MainContent_inputNombre').val('');
        $('#MainContent_inputCorreo').val('');
     
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
        var username = $('#MainContent_inputCorreo').val();
        var VendName = $('#MainContent_inputNombre').val();
        var provId = $('#MainContent_comboProveedores').val();
        var interno = $('#MainContent_comboInterno').val();
        var estado = $('#MainContent_comboEstado').val();

        var url_report = "/Logged/Reports/Usuarios";
        
        url_report += "?VendName=" + VendName;
        url_report += "&username=" + username;
        url_report += "&interno=" + interno;
        url_report += "&estado=" + estado;
        window.open(url_report, '_blank');
    }
} );

