//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
var table;
$(document).ready(function () {
    var url_list = FacturasWebService.get_path() + "/listar";
   
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
                data.VendID = $('#MainContent_comboProveedores').val();
                data.Folio = '';
                data.Serie = '';
                data.Fecha = $('#MainContent_inputFecha').val();
                data.Total = '';
                data.UUID = '';
                data.Status = '';
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
            { "data": "UUID" , 'className': "centrar-data", "orderable": false},
            { "data": "Compania" , 'className': "centrar-data text_align_left"},
            { "data": "Folio" , 'className': "centrar-data text_align_left"},
            { "data": "Serie", 'className': "centrar-data" },
            { "data": "Fecha" , 'className': "centrar-data"},
            { "data": "Proveedor" , 'className': "centrar-data"},
            { "data": "Subtotal", 'className': "centrar-data" },
            { "data": "Impuestos" , 'className': "centrar-data"}
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
        $('#MainContent_inputFecha').val('');
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

    $('form').submit(function () {
        var ids = getSelected();
        var json_ids = JSON.stringify(ids);
        $('#MainContent_facturas_seleccionadas').val(json_ids);
        console.log($('#MainContent_facturas_seleccionadas').val());
        return true;
    });
    
} );

