//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.

//const { swal } = require("../Css");

//ACTUALIZADO POR : LUIS ANGEL GARCIA
var table;
$(document).ready(function () {
    hide_combo_proveedores(ContrarecibosWebService.get_path());
    var url_list = ContrarecibosWebService.get_path() + "/listar2";
   
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
            { "data": "Total", 'className': "cetrar-data text_align_right" },
            { "data": "Aprobado", 'className': "cetrar-data text_align_right" }
        ],
        "columnDefs": [ {
            'targets': 0,
            'checkboxes': {
                'selectRow': true
            }
        }],
        "createdRow": function (row, data, dataIndex) {
            if (data.Bloqueo == "SI") {
                $(row).find("td:nth-child(1) input[type=checkbox]").attr("disabled", true);
                $(row).prop('disabled', row.length === 0);
            }
        },
        'select': {
            selector:'td:nth-child(1) input[type=checkbox]',
            'style': 'multi'
        },
        'order': [[1, 'asc']]
    });

    $('.buscar').click(function (e) {
        e.preventDefault();
        table.ajax.reload();
    });

    $('.limpiar').click(function (e) {
        e.preventDefault();
        $("#MainContent_comboProveedores").val($("#MainContent_comboProveedores option:first").val()).change();
        $('#MainContent_inputRFC').val('');
        $('#MainContent_inputTotal').val('');
        $('#MainContent_inputFolio').val('');
        $('#MainContent_inputFechaRecepcion').val('');
        $('#MainContent_inputFechaPago').val('');
        table.ajax.reload();
    });

    function limpiar()
    {
        $("#MainContent_comboProveedores").val($("#MainContent_comboProveedores option:first").val()).change();
        $('#MainContent_inputRFC').val('');
        $('#MainContent_inputTotal').val('');
        $('#MainContent_inputFolio').val('');
        $('#MainContent_inputFechaRecepcion').val('');
        $('#MainContent_inputFechaPago').val('');
        table.ajax.reload();
    }

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

        swal.fire({
            title: "",
            text: "Desea crear la solicitud de cheque?",
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
            if (result.isConfirmed)
            {
                crearSolicitudCheque(ids);
                var json_ids = JSON.stringify(ids);
                $('#MainContent_contrarecibos_seleccionados').val(json_ids);
                return false;
            }
        })
        
        //showToastConfirmation("Desea crear la solicitud de cheque?", "", function () {
        //    crearSolicitudCheque(ids); return false;
        //}, function () {

        //}
        //);
        //return false;
        //var json_ids = JSON.stringify(ids);
        //$('#MainContent_contrarecibos_seleccionados').val(json_ids);
        return true;
    });

    $('#btn-generar').click(function (e)
    {
        try
        {
        var ids = getIds();
        if (ids.length == 0) {
            //showToastError("No existen registros para generar el reporte.");
            Swal.fire({ icon: 'error', title: 'Error', customClass: 'swal-wide', text: 'No existen registros para generar el reporte.' })
            return false;
        }
        else
        {
            var ids = getSelected();
            if (ids.length == 0) {
                //showToastError("Seleccione los contrarecibos para generar la solicitud de cheque.");
                Swal.fire({ icon: 'error', title: 'Error', customClass: 'swal-wide', text: 'Seleccione los contrarecibos para generar la solicitud de cheque.'})
                return false;
            }

            Generar_Token();
            //$('#myModal').modal('show');
        }   

        }
            catch (err) {
            var err = err.message;
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
                    Swal.fire({icon: 'error', title: 'Error', customClass: 'swal-wide', text: respuesta.msg ? respuesta.msg : "No se pudo crear la solicitud de cheque."})
                    //showToastError(respuesta.msg ? respuesta.msg : "No se pudo crear la solicitud de cheque.");
                }
            },
            error: function (respuesta) {
                unblockUI();
                //showToastError("No se pudo crear la solicitud de cheque.");
                Swal.fire({ icon: 'error', title: 'Error', customClass: 'swal-wide', text: 'No se pudo crear la solicitud de cheque' })

            }
        });
    }

    function redirectToReport(id) {
        var url_report = "/Logged/Reports/SolicitudCheque?id=" + id;
        window.open(url_report, '_blank');
    }

    async function obtenervariables() {
        var variablas = [];
        let variables;
        try {

            var url_generar_solicitud = ContrarecibosWebService.get_path() + "/getrol";
            variables = await $.ajax({
                type: "POST",
                beforeSend: function (xhr) {   //Include the bearer token in header
                    xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
                },
                url: url_generar_solicitud,
                dataType: "json",
                data: {
                    ids: JSON.stringify(1),
                    comentario: ""
                },
                success: function (respuesta) {
                    if (respuesta.success == true) {
                        variablas.push({ respuesta: respuesta.success, rol: respuesta.rol, userkey: respuesta.user, token: respuesta.token });
                        return variablas;
                    } else {
                        //showToastError("No se pudo crear la solicitud de cheque., se generaron errores al intentar obtener las variables de session");
                        Swal.fire({ icon: 'error', title: 'Error', customClass: 'swal-wide', text: 'No se pudo crear la solicitud de cheque., se generaron errores al intentar obtener las variables de session' })
                    }
                },
                error: function (respuesta) {
                    //showToastError("No se pudo crear la solicitud de cheque.");
                    Swal.fire({ icon: 'error', title: 'Error', customClass: 'swal-wide', text: 'No se pudo crear la solicitud de cheque' })
                }
            });

        }
        catch (err) {
            return err.message;
        }
        return variables;
    }

    async function nuevasvariables() {
        var variablas = [];
        let variables;
        try {

            var url_generar_solicitud = ContrarecibosWebService.get_path() + "/newtoken";
            variables = await $.ajax({
                type: "POST",
                beforeSend: function (xhr) {   //Include the bearer token in header
                    xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
                },
                url: url_generar_solicitud,
                dataType: "json",
                data: {
                    ids: JSON.stringify(1),
                    comentario: ""
                },
                success: function (respuesta) {
                    if (respuesta.success == true) {
                        variablas.push({ respuesta: respuesta.success, rol: respuesta.rol, userkey: respuesta.user, token: respuesta.token });
                        return variablas;
                    } else {
                        //showToastError("No se pudo crear la solicitud de cheque., se generaron errores al intentar obtener las variables de session");
                        Swal.fire({ icon: 'error', title: 'Error', customClass: 'swal-wide', text: 'No se pudo crear la solicitud de cheque., se generaron errores al intentar obtener las variables de session' })
                    }
                },
                error: function (respuesta) {
                    //showToastError("No se pudo crear la solicitud de cheque.");
                    Swal.fire({ icon: 'error', title: 'Error', customClass: 'swal-wide', text: 'No se pudo crear la solicitud de cheque' })
                }
            });

        }
        catch (err) {
            return err.message;
        }
        return variables;
    }

    async function Generar_Token() {
        try {

            swal.fire({
                title: "Confirmacion de Autorizacion",
                text: "Tendras que confirmar la Solicitud de Cheque a traves de un Token que sera enviado a tu correo electronico ,Deseas continuar?",
                icon: "warning",
                buttons: ["No", "Si"],
                confirmButtonText: 'Si, Enviar',
                cancelButtonText: 'No, Cancelar',
                confirmButtonColor: "#FF6600",
                customClass: 'swal-wide',
                closeModal: true,
                allowOutsideClick: false,
                closeOnConfirm: false,
                showCancelButton: true,
                dangerMode: true,
            }).then((result) => {
                if (result.isConfirmed)
                {   
                    NuevoTok();
                }
            })
        }
        catch (err) {
            return err.message;
        }

        return true;
    }

    async function NuevoTok()
    {
        try {

        let variables;
        variables = await obtenervariables(); 
            
        if (variables.token == "enviado")
        {
            Preguntas();
        }
        else
        {
            swal.fire({
                title: "Confirmacion de Autorizacion",
                text: "Se ha generado un error al intentar obtener el token",
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
        catch (err)
        {
            return err.message;
        }
        return true;
    }

    async function Preguntas() {

        const { value: password } = await swal.fire({
            title: "Confirmacion de Autorizacion",
            input: 'password',
            inputLabel: 'Se ha enviado un Token a tu correo ,Ingresa el Token recibido',
            inputPlaceholder: 'Ingresa Token',
            customClass: 'swal-wide',
            inputAttributes: {
                maxlength: 20,
                autocapitalize: 'off',
                autocorrect: 'off'
            },
            icon: "success",
            buttons: ["Cancelar", "Enviar"],
            confirmButtonText: 'Enviar',
            cancelButtonText: 'Cancelar',
            confirmButtonColor: "#FF6600",
            customClass: 'swal-wide',
            closeModal: true,
            allowOutsideClick: false,
            closeOnConfirm: false,
            showCancelButton: true,
            dangerMode: true,
        })
        if (password) {
            validatoken(password);
        }
        else {
            swal.fire({
                title: "Confirmacion de Autorizacion",
                text: "Ingresa el token enviado a tu correo",
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

        return true;
    }

    async function validatoken(password)
    {
        try
        {   
            let variables;
            variables = await revisaSolicitud(password);  
            if (variables.token == "correcto")
            {   
                update();
            }
            else
            {
                swal.fire({
                    title: "Confirmacion de Autorizacion",
                    text: "El token ingresado es incorrecto o ya expiro,intenta de nuevo o genera uno nuevo.",
                    icon: "error",
                    confirmButtonText: 'Generar Nuevo',
                    cancelButtonText: 'Cancelar',
                    confirmButtonColor: "#FF6600",
                    customClass: 'swal-wide',
                    closeModal: true,
                    closeOnConfirm: false,
                    showCancelButton: true,
                    dangerMode: true,
                }).then((result) => {
                    if (result.isConfirmed) {
                        swal.close();
                        nuevotoken();
                    }
                })
            }
        }
        catch (err)
        {
            return err.message;
        }

        return true;
    }

    async function nuevotoken()
    {   
        let variables
        variables = await nuevasvariables();
        if (variables.token == "enviado") {
            Preguntas();
        }
        else {
            swal.fire({
                title: "Confirmacion de Autorizacion",
                text: "Se ha generado un error al intentar obtener el token",
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

    async function update()
    {   
        try
        {   
            let variables;
            var ids = getSelected();
            variables = await actualizarSolicitud(ids);
            if (variables.token == "actualizado")
            {
                swal.fire({
                    title: "Confirmacion de Autorizacion",
                    text: "El token ingresado es correcto, haz autorizado la solicitud.",
                    icon: "success",
                    confirmButtonText: 'Ok',
                    cancelButtonText: 'Cancelar',
                    customClass: 'swal-wide',
                    closeModal: true,
                    closeOnConfirm: false,
                    showCancelButton: false,
                    allowOutsideClick: false,
                    dangerMode: true,
                }).then((result) => {
                    if (result.isConfirmed) {
                        if (variables.Genera == "SI")
                        {
                            $('#myModal').modal('show');
                            swal.close();
                        }
                        else
                        {
                          limpiar();
                        }
                        
                    }
                })
            }
            else
            {
                swal.fire({
                    title: "Confirmacion de Autorizacion",
                    text: "Se ha generado un error al intentar actualizar el estado a la solicitud de cheque, intentalo de nuevo",
                    icon: "error",
                    confirmButtonText: 'Ok',
                    cancelButtonText: 'Ok',
                    confirmButtonColor: "#FF6600",
                    customClass: 'swal-wide',
                    closeModal: true,
                    closeOnConfirm: true,
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
    }

    async function actualizarSolicitud(ids) {
        var variablas = [];
        let variables;
        try {

            var url_generar_solicitud = ContrarecibosWebService.get_path() + "/updateSOL";
            variables = await $.ajax({
                type: "POST",
                beforeSend: function (xhr) {   //Include the bearer token in header
                    xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
                },
                url: url_generar_solicitud,
                dataType: "json",
                data: {
                    ids: JSON.stringify(ids),
                    comentario: ""
                },
                success: function (respuesta) {
                    if (respuesta.success == true) {
                        variablas.push({ respuesta: respuesta.success, rol: respuesta.rol, userkey: respuesta.user, token: respuesta.token });
                        return variablas;
                    } else {
                        //showToastError("No se pudo crear la solicitud de cheque., se generaron errores al intentar obtener las variables de session");
                        Swal.fire({ icon: 'error', title: 'Error', customClass: 'swal-wide', text: 'No se pudo crear la solicitud de cheque., se generaron errores al intentar obtener las variables de session' })
                    }
                },
                error: function (respuesta) {
                    //showToastError("No se pudo crear la solicitud de cheque.");
                    Swal.fire({ icon: 'error', title: 'Error', customClass: 'swal-wide', text: 'No se pudo crear la solicitud de cheque' })
                }
            });

        }
        catch (err) {
            return err.message;
        }
        return variables;
    }

    async function revisaSolicitud(ids) {
        var variablas = [];
        let variables;
        try {

            var url_generar_solicitud = ContrarecibosWebService.get_path() + "/RevToken";
            variables = await $.ajax({
                type: "POST",
                beforeSend: function (xhr) {   //Include the bearer token in header
                    xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
                },
                url: url_generar_solicitud,
                dataType: "json",
                data: {
                    ids: ids,
                    comentario: ""
                },
                success: function (respuesta) {
                    if (respuesta.success == true) {
                        variablas.push({ respuesta: respuesta.success, rol: respuesta.rol, userkey: respuesta.user, token: respuesta.token });
                        return variablas;
                    } else {
                        //showToastError("No se pudo crear la solicitud de cheque., se generaron errores al intentar obtener las variables de session");
                        Swal.fire({ icon: 'error', title: 'Error', customClass: 'swal-wide', text: 'No se pudo crear la solicitud de cheque., se generaron errores al intentar obtener las variables de session' })
                    }
                },
                error: function (respuesta) {
                    //showToastError("No se pudo crear la solicitud de cheque.");
                    Swal.fire({ icon: 'error', title: 'Error', customClass: 'swal-wide', text: 'No se pudo crear la solicitud de cheque' })
                }
            });

        }
        catch (err) {
            return err.message;
        }
        return variables;
    }
    
} );

