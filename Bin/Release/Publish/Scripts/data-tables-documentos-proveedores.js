//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
var table;
$(document).ready(function () {
    var url_list = DocumentosProveedoresWebService.get_path() + "/listar";
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
                data.DocID = $('#MainContent_inputID').val();
                data.VendID = $('#MainContent_inputNombre').val();  
                data.VendName = $('#MainContent_inputSocial').val();
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
            { "data": "ID_Documento" , 'className': "centrar-data", "orderable": false},
            { "data": "Social" , 'className': "centrar-data"},
            { "data": "Actualizacion", 'className': "centrar-data" },
            { "data": "Descripcion", 'className': "centrar-data" },
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


    $('.buscar').click(function (e) {
        e.preventDefault();
        table.ajax.reload(  );
    });
    $('.limpiar').click(function (e) {
        e.preventDefault();

        $("#MainContent_comboEstado").val($("#MainContent_comboEstado option:first").val()).change();
        $('#MainContent_inputID').val('');
        $('#MainContent_inputNombre').val('');
        $('#MainContent_inputSocial').val('');

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
        var id = $('#MainContent_inputID').val();
        var nombre = $('#MainContent_inputNombre').val();
        var social = $('#MainContent_inputSocial').val();
        var estado = $('#MainContent_comboEstado').val();

        var url_report = "/Logged/Reports/ProveedoresDocumentos";
        
        url_report += "?id=" + id;
        url_report += "&nombre=" + nombre;
        url_report += "&social=" + social;
        url_report += "&estado=" + estado;
        window.open(url_report, '_blank');
    }
} );

