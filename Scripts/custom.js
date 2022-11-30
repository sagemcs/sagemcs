$(document).ready(function () {
    if ($('.datepicker').length) {
        $('.datepicker').datepicker({
            format: 'dd/mm/yyyy'
        });
    }
    $('#error_inputTotal').css('display', 'none');
    $('#Text_Sec').css('display', 'none');
    $('.cargar').click(function () {
        blockUI({
            boxed: true,
            message: 'Espere un momento'
        });
    });
});

window.datatableLang = {
    "sProcessing": "Procesando...",
    "sLengthMenu": "Mostrar _MENU_ registros",
    "sZeroRecords": "No se encontraron resultados",
    "sEmptyTable": "Ningún dato disponible en esta tabla",
    "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
    "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
    "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
    "sInfoPostFix": "",
    "sSearch": "Buscar:",
    "sUrl": "",
    "sInfoThousands": ",",
    "sLoadingRecords": "Cargando...",
    "oPaginate": {
        "sFirst": "Primero",
        "sLast": "Último",
        "sNext": "Siguiente",
        "sPrevious": "Anterior"
    },
    "oAria": {
        "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
        "sSortDescending": ": Activar para ordenar la columna de manera descendente"
    }
};

//toastr.options = { closeButton: true, debug: false, positionClass: "toast-top-right", onclick: null, showDuration: "5000", hideDuration: "5000", timeOut: "5000", extendedTimeOut: "5000", showEasing: "swing", hideEasing: "linear", showMethod: "fadeIn", hideMethod: "fadeOut" };
function showToastError(message) {
    //toastr.error(message, "Error");
    swal("Error", message, "error");
}
function showToastSuccess(message) {
    //toastr.success(message, "");
    swal("", message, "success");
}
function showToastWarning(message) {
    swal("", message, "warning");
}
function showToastInfo(message) {
    swal("", message, "info");
}

function showToastConfirmation(title, message, cb, cbNegative) {
    swal({
        title: title,
        text: message,
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then(function(willDelete) {
            if (willDelete) {
                cb();
            } else {
                cbNegative();
            }
        });
}

function alertme(titulo,mesaje,Tipo) {
    swal(titulo, mesaje, Tipo)
}

function alert(campo) {
    $(campo).show("slow").delay(2000).hide("slow")
}

function Reset() {
    document.getElementById("tab-2").checked = true;
}

function block_screen() {
    var height = $('body').height() + "px";
    var height = '100%';
    $('div.bloqueo').css({ 'height': height });
    $('div.bloqueo').show();
}


function unblock_screen() {
    $('.modal-backdrop').remove();
    $('div.bloqueo').hide();
}

// wrapper function to  block element(indicate loading)
function blockUI(options) {
    var options = $.extend(true, {}, options);
    var html = '';
    if (options.iconOnly) {
        html = '<div class="loading-message ' + (options.boxed ? 'loading-message-boxed' : '')+'"><img style="" src="/Img/loading-spinner-grey.gif" align=""></div>';
    } else if (options.textOnly) {
        html = '<div class="loading-message ' + (options.boxed ? 'loading-message-boxed' : '')+'"><span>&nbsp;&nbsp;' + (options.message ? options.message : 'CARGANDO...') + '</span></div>';
    } else {    
        html = '<div class="loading-message ' + (options.boxed ? 'loading-message-boxed' : '') + '"><img style="" src="/Img/loading-spinner-grey.gif" align=""><span>&nbsp;&nbsp;' + (options.message ? options.message : 'CARGANDO...') + '</span></div>';
    }

    if (options.target) { // element blocking
        var el = jQuery(options.target);
        if (el.height() <= ($(window).height())) {
            options.cenrerY = true;
        }            
        el.block({
            message: html,
            baseZ: options.zIndex ? options.zIndex : 1000,
            centerY: options.cenrerY != undefined ? options.cenrerY : false,
            css: {
                top: '10%',
                border: '0',
                padding: '0',
                backgroundColor: 'none'
            },
            overlayCSS: {
                backgroundColor: options.overlayColor ? options.overlayColor : '#000',
                opacity: options.boxed ? 0.05 : 0.1, 
                cursor: 'wait'
            }
        });
    } else { // page blocking
        $.blockUI({
            message: html,
            baseZ: options.zIndex ? options.zIndex : 1000,
            css: {
                border: '0',
                padding: '0',
                backgroundColor: 'none'
            },
            overlayCSS: {
                backgroundColor: options.overlayColor ? options.overlayColor : '#000',
                opacity: options.boxed ? 0.05 : 0.1,
                cursor: 'wait'
            }
        });
    }            
}

        // wrapper function to  un-block element(finish loading)
function unblockUI(target) {
    if (target) {
        jQuery(target).unblock({
            onUnblock: function () {
                jQuery(target).css('position', '');
                jQuery(target).css('zoom', '');
            }
        });
    } else {
        $.unblockUI();
    }
}

function hide_combo_proveedores(get_path) {
    $.ajax({
        url: get_path + "/es_proveedor",
        type: "GET",
        processData: true,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (results) {
            if (results.d == false) {
                $("#MainContent_comboProveedores").attr('disabled','disabled');
            }
        },
        error: function (results) {
            s = results;
        }
    });
}

function IsNumber(id) {
    return !isNaN(document.getElementById(id).value);    
}

function base64url(source) {
    // Encode in classical base64
    encodedSource = CryptoJS.enc.Base64.stringify(source);
    // Remove padding equal characters
    encodedSource = encodedSource.replace(/=+$/, '');
    // Replace characters according to base64url specifications
    encodedSource = encodedSource.replace(/\+/g, '-');
    encodedSource = encodedSource.replace(/\//g, '_');

    return encodedSource;
}

function GetJWT() {
    
    try
    {        
        var header = {
            "typ": "JWT",
            "alg": "HS256"            
        };

        var stringifiedHeader = CryptoJS.enc.Utf8.parse(JSON.stringify(header));
        var encodedHeader = base64url(stringifiedHeader);

        var payload = {
            "iss": "http://*.tsys.com",
            "aud": "1234567890",
        };

        var stringifiedData = CryptoJS.enc.Utf8.parse(JSON.stringify(payload));
        var encodedData = base64url(stringifiedData);

        var token = encodedHeader + "." + encodedData;

        //var secret = "SXhyQWpranNkYWhfWGF3PGxreDxjw7FsenhqZXRsa3M=";

        var secret = $('#Text_Sec').val();

        var signature = CryptoJS.HmacSHA256(token, secret);
        signature = base64url(signature);

        var signedToken = token + "." + signature;
        
        return signedToken;
    }
    catch (err) {
        return err.message;
    }
}