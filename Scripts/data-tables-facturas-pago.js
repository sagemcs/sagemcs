//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
var table;
var cli;
$(document).ready(function () {

    hide_combo_proveedores(FacturasWebService.get_path());
    var url_list = FacturasWebService.get_path() + "/listar22";
    cli = '1';
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
                data.contrarecibo = $('#MainContent_inputContrarecibo').val();
                data.solicitud  = $('#MainContent_inputSolicitud').val();
                data.Fecha = $('#MainContent_inputFecha').val();
                data.FechaR = $('#MainContent_inputFechaR').val();
                data.FechaPP = $('#MainContent_inputFechaPP').val();
                data.VendorId = $('#MainContent_comboProveedores').val();
                data.Cont = cli;
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
            { "data": "Invckey", 'className': "centrar-data", "orderable": false },
            { "data": "Proveedor", 'className': "centrar-data" },
            { "data": "Folio" , 'className': "centrar-data text_align_left"},
            { "data": "Fecha_Recepcion", 'className': "centrar-data" },
            { "data": "Fecha", 'className': "centrar-data" },
            { "data": "Contrarecibo_Folio", 'className': "centrar-data" },
            { "data": "Solicitud_Folio", 'className': "centrar-data" },
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


    $('#MainContent_inputTotal').mouseleave(function (e) {
        if (IsNumber('MainContent_inputTotal')) {
            $('#error_inputTotal').css('display', 'none');
        }
        else
            $('#error_inputTotal').css('display', 'block');
    });

    $('.limpiar').click(function (e) {
        e.preventDefault();

        $("#MainContent_comboProveedores").val($("#MainContent_comboProveedores option:first").val()).change();
        $('#MainContent_inputFolio').val('');
        $('#MainContent_inputContrarecibo').val('');
        $('#MainContent_inputSolicitud').val('');
        $('#MainContent_inputFecha').val('');
        $('#MainContent_inputFechaR').val('');
        $('#MainContent_inputFechaPP').val('');
        $('#MainContent_comboProveedores').val('');
        cli = '1';
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

    function getSelected() {
        var rows_selected = table.column(0).checkboxes.selected();
        var ids = [];
        $.each(rows_selected, function (index, id) {
            ids.push(id);
        });
        return ids;
    }

    function getSelectedProv() {

        var rows_selected = table.column(0).checkboxes.selected();
        var ids = [];
        var ant = "";
        var conteo = 0;
        $.each(rows_selected, function (index, id)
        {
            row = table.rows[index];
            var test = rows_selected;
            var prov = table.rows[index];
            //var prov = table.rows[index].cells[2];
            if (ant = "")
            {
                ant = prov;
            }
            else
            {
                if (ant = prov)
                {
                    conteo = conteo + 1;
                }
            }
            //ids.push(id);
        });
        return conteo;
    }

    $('form').submit(function () {
        
        var ids = getIds();    
        if (ids.length == 0) {
            Swal.fire({ icon: 'error', title: 'Error', customClass: 'swal-wide', text: 'No existen registros.' })
            return false;
        }
        else
        {
            var ids = getSelected();
            if (ids.length == 0) {
                //showToastError("Seleccione los contrarecibos para generar la solicitud de cheque.");
                Swal.fire({ icon: 'error', title: 'Error', customClass: 'swal-wide', text: 'Seleccione las facturas para adjuntar el comprobante de pago.' })
                return false;
            }

            //var provs = getSelectedProv();
            //if (provs > 0) {
            //    //showToastError("Seleccione los contrarecibos para generar la solicitud de cheque.");
            //    Swal.fire({ icon: 'error', title: 'Error', customClass: 'swal-wide', text: 'Ha seleccionado facturas que son de diferentes proveedores.' })
            //    return false;
            //}

            swal.fire({
                title: "",
                text: "Confirma que desea agregar comprobante de pago a los elementos seleccionados?",
                icon: "warning",
                confirmButtonText: 'Si',
                cancelButtonText: 'Cancelar',
                confirmButtonColor: "#FF6600",
                customClass: 'swal-wide',
                closeModal: true,
                closeOnConfirm: false,
                showCancelButton: true,
                dangerMode: true,
            }).then((result) => {
                if (result.isConfirmed) {
                    cargararchivo();
                }
            })

        }

        //redirectToReport(); return false;
        return false;
    });

    async function cargararchivo()
    {
        const { value: file } = await Swal.fire({
            title: 'Seleccione Comprobante de Pago',
            icon: "success",
            customClass: 'swal-wide',
            input: 'file',
            inputAttributes: {
                'accept': 'application/pdf',
                'aria-label': 'Seleccione un documento'
            }
        })

        if (file) {
            const reader = new FileReader();
            reader.onload = (e) => {
                filecontent = e.target.result;
                enviarchivo(filecontent);
            }
            reader.readAsDataURL(file)
        }

        return true;
    }

    async function enviarchivo(binary)
    {
        try {

            let variables;
            var ids = getSelected();
            variables = await actualizarFacturas(binary, ids);

            if (variables.token == "enviado") {
                swal.fire({
                    title: "Confirmacion de Actualizacion",
                    text: "Se ha actualizado correctamente las facturas ",
                    icon: "success",
                    confirmButtonText: 'Ok',
                    cancelButtonText: 'Cancelar',
                    confirmButtonColor: "#FF6600",
                    customClass: 'swal-wide',
                    closeModal: true,
                    closeOnConfirm: false,
                    showCancelButton: false,
                    dangerMode: true,
                }).then((result) => {
                    if (result.isConfirmed) {
                    }
                })
            }
            else {
                swal.fire({
                    title: "Confirmacion de Autorizacion",
                    text: variables.error,
                    //text: "Se ha generado un error al intentar subir el archivo ",
                    icon: "error",
                    confirmButtonText: 'Ok',
                    cancelButtonText: 'Cancelar',
                    confirmButtonColor: "#FF6600",
                    customClass: 'swal-wide',
                    closeModal: true,
                    closeOnConfirm: false,
                    showCancelButton: false,
                    dangerMode: true,
                }).then((result) => {
                    if (result.isConfirmed) {
                    }
                })
            }

        }
        catch (err) {
            return err.message;
        }
        return true;
    }

    async function actualizarFacturas(file, ids) {
        var variablas = [];
        let variables;
        try {

            var url_generar_solicitud = FacturasWebService.get_path() + "/Actulizar22";
            variables = await $.ajax({
                type: "POST",
                beforeSend: function (xhr) {   //Include the bearer token in header
                    xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
                },
                url: url_generar_solicitud,
                dataType: "json",
                data: {
                    bytes1: file,
                    Cont: JSON.stringify(ids),
                },
                success: function (respuesta) {
                    if (respuesta.success == true) {
                        variablas.push({ respuesta: respuesta.success, userkey: respuesta.user });
                        return variablas;
                    } else {
                        //showToastError("No se pudo crear la solicitud de cheque., se generaron errores al intentar obtener las variables de session");
                        Swal.fire({ icon: 'error', title: 'Error', customClass: 'swal-wide', text: 'No se pudo actualziar la factura, ocurrieron errores durante el procesamiento el archivo' })
                    }
                },
                error: function (respuesta) {
                    //showToastError("No se pudo crear la solicitud de cheque.");
                    Swal.fire({ icon: 'error', title: 'Error', customClass: 'swal-wide', text: 'No se pudo procesar la solicitud' })
                }
            });

        }
        catch (err) {
            return err.message;
        }
        return variables;
    }

    function redirectToReport() {
        var folio = $('#MainContent_inputFolio').val();
        var serie = $('#MainContent_inputSerie').val();
        var fecha = $('#MainContent_inputFecha').val();
        var fechaR = $('#MainContent_inputFechaR').val();
        var provId = $('#MainContent_comboProveedores').val();
        var total = $('#MainContent_inputTotal').val();
        var uuid = $('#MainContent_inputUUID').val();
        var estado = $('#MainContent_comboEstado').val();

        var url_report = "/Logged/Reports/Facturas";
        
        url_report += "?folio=" + folio;
        url_report += "&serie=" + serie;
        url_report += "&fecha=" + fecha;
        url_report += "&fechaR=" + fechaR;
        url_report += "&provId=" + provId;
        url_report += "&total=" + total;
        url_report += "&uuid=" + uuid;
        url_report += "&estado=" + estado;
        window.open(url_report, '_blank');
    }  
});

function Test(e, v) {
    cli = '0';
    table.ajax.reload();
}

