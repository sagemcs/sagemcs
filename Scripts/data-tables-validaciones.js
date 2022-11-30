//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA

var table;
$(document).ready(function () {
    var url_list = ValidacionesWebService.get_path() + "/listar";
    
    table = $('#list').DataTable({
        language: window.datatableLang,
        "ajax": {
            "url": url_list,
            "type": "POST",
            "beforeSend": function (xhr) {   //Include the bearer token in header
                xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
            },
            "draw": 1,
            "data": function (data) {
                data.order_col = data.order[0]['column'];
                data.order_dir = data.order[0]['dir'];
                data.ItemID = $('#MainContent_inputItemID').val();
                data.fechaerror = $('#MainContent_inputFecha').val();
               
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
            { "data": "Articulo" , 'className': "centrar-data"},
            { "data": "Fecha" , 'className': "centrar-data text_align_left"},
            { "data": "Descripcion", 'className': "centrar-data" },
            { "data": "Etapa" , 'className': "centrar-data"}
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
        $('#MainContent_inputItemID').val('');
        $('#MainContent_inputFecha').val('');
        
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
        var ItemID = $('#MainContent_inputItemID').val();
        var fechaerror = $('#MainContent_inputFecha').val();

        var url_report = "/Logged/Reports/Validaciones";
        
        url_report += "?ItemID=" + ItemID;
        url_report += "&fechaerror=" + fechaerror;
        window.open(url_report, '_blank');
    }
} );

