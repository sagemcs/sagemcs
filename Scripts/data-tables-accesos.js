//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
var table;
$(document).ready(function () {

    var url_list = AccesosWebService.get_path() + "/listar";
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
                data.UserID = $('#MainContent_inputUserID').val();
                data.UserName = $('#MainContent_inputUsername').val();
                data.IP = $('#MainContent_inputIP').val();
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
           // { "data": "UUID" , 'className': "centrar-data", "orderable": false},
            { "data": "Usuario", 'className': "centrar-data" },
            { "data": "Nombre", 'className': "centrar-data text_align_left" },        
            { "data": "Fecha", 'className': "centrar-data" },
            { "data": "IP", 'className': "centrar-data" }
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

        $('#MainContent_inputUserID').val('');
        $('#MainContent_inputUsername').val('');
        $('#MainContent_inputIP').val('');
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
        var userId = $('#MainContent_inputUserID').val();
        var username = $('#MainContent_inputUsername').val();
        var ip = $('#MainContent_inputIP').val();

        var url_report = "/Logged/Reports/Accesos";     
        url_report += "?userId=" + userId;
        url_report += "&username=" + username;
        url_report += "&ip=" + ip;   
        window.open(url_report, '_blank');
    }
} );

