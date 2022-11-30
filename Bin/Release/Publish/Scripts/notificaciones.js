$(document).ready(function () {
    var url_list = NotificacionesWebService.get_path() + "/listar";
    $.ajax({
        type: "POST",
        "beforeSend": function (xhr) {
            xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
        },
        url: url_list,
        dataType: "json",
        success: function (respuesta) {
            var facturas = respuesta.facturas;
            var pagos = respuesta.pagos;
            var documentos = respuesta.documentos;
            var html = [];
            facturas.forEach(function (message) {
                var tr = "<tr><td>" + message + "</td></tr>";
                html.push(tr);
            });
            pagos.forEach(function (message) {
                var tr = "<tr><td>" + message + "</td></tr>";

                html.push(tr);
            });
            documentos.forEach(function (message) {
                var tr = "<tr><td>" + message + "</td></tr>";

                html.push(tr);
            });
            var table = '<tr><td style="text-align:center">No tiene nuevos mensajes</td></tr>';
            if (html.length > 0) {
                table = html.join('');
            }
            $('#noticicaciones-container').html(table);
        },
        error: function (respuesta) {
            var table = '<tr><td style="text-align:center">Error obteniendo los mensajes</td></tr>';
          
            $('#noticicaciones-container').html(table);
        }
    });
});