//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
$(document).ready(function () {

          $('#btn_send_comments').click(function (e) {
              Proceso(e, "/enviar_notificacion_de_rechazo_de_factura")
              return false;
          });

          $('#btn_No_Send').click(function (e) {
              //Proceso(e, "/NOenviar_notificacion_de_rechazo_de_factura")
              Llenado(e, "/Llenado")
              return false;
          });

          function Proceso(e,Proce)
          {
              e.preventDefault();
              var comentario = $('#MainContent_inputComentario').val();
              var folio_factura = $('#inputFolio').val();
              var folio_llave = $('#inputllave').val();
              var VenId = $('#VendId').val();
              var CFolio = $('#CFolio').val();
              var NoOdc = $('#NoOdc').val();
              var Estatus = $('#Estatus').val();
              var Opcion = $('#Opcion').val();
              var Fecha1 = $('#Fecha1').val();
              var Fecha2 = $('#Fecha2').val();

              var url_list = NotificacionesWebService.get_path() + Proce;
              $.ajax({
                  type: "POST",
                  "beforeSend": function (xhr) {
                      xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
                  },
                  url: url_list,
                  dataType: "json",
                  data: {
                      folio: folio_factura,
                      texto: comentario,
                      llave: folio_llave,
                      VendId: VenId,
                      CFolio: CFolio,
                      NoOdc: NoOdc,
                      Estatus: Estatus,
                      Opcion: Opcion,
                      Fecha1: Fecha1,
                      Fecha2: Fecha2
                  },
                  success: function (respuesta) {
                      $('#myModal').modal('hide');
                      if (respuesta.success) {
                          $('#MainContent_inputComentario').val('');
                          $('#inputFolio').attr('value', '');
                          $('#inputllave').attr('value', '');
                          $('#VendId').attr('value', '');
                          $('#CFolio').attr('value', '');
                          $('#NoOdc').attr('value', '');
                          $('#Estatus').attr('value', '');
                          $('#Opcion').attr('value', '');
                          $('#Fecha1').attr('value', '');
                          $('#Fecha2').attr('value', '');
                          swal('Notificaciones T|SYS|', respuesta.msg, 'success');
                      }
                      else
                          swal('Notificaciones T|SYS|', respuesta.msg, 'error');
                  },
                  error: function (respuesta) {
                      $('#myModal').modal('hide');
                      swal("La notificación de rechazo de factura no ha podido ser enviada");
                  }
              });

              return false;
          }

          function Llenado(e, Proce) {
              e.preventDefault();
              var comentario = $('#MainContent_inputComentario').val();
              var folio_factura = $('#inputFolio').val();
              var folio_llave = $('#inputllave').val();
              var VenId = $('#VendId').val();
              var CFolio = $('#CFolio').val();
              var NoOdc = $('#NoOdc').val();
              var Estatus = $('#Estatus').val();
              var Opcion = $('#Opcion').val();
              var Fecha1 = $('#Fecha1').val();
              var Fecha2 = $('#Fecha2').val();

              var url_list = NotificacionesWebService.get_path() + Proce;
              $.ajax({
                  type: "POST",
                  "beforeSend": function (xhr) {
                      xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
                  },
                  url: url_list,
                  dataType: "json",
                  data: {
                      folio: folio_factura,
                      texto: comentario,
                      llave: folio_llave,
                      VendId: VenId,
                      CFolio: CFolio,
                      NoOdc: NoOdc,
                      Estatus: Estatus,
                      Opcion: Opcion,
                      Fecha1: Fecha1,
                      Fecha2: Fecha2
                  },
                  success: function (respuesta) {
                      $('#myModal').modal('hide');
                      if (respuesta.success) {
                          $('#MainContent_inputComentario').val('');
                          $('#inputFolio').attr('value', '');
                          $('#inputllave').attr('value', '');
                          $('#VendId').attr('value', '');
                          $('#CFolio').attr('value', '');
                          $('#NoOdc').attr('value', '');
                          $('#Estatus').attr('value', '');
                          $('#Opcion').attr('value', '');
                          $('#Fecha1').attr('value', '');
                          $('#Fecha2').attr('value', '');
                          swal('Notificaciones T|SYS|', respuesta.msg, 'success');
                      }
                      else
                          swal('Notificaciones T|SYS|', respuesta.msg, 'error');
                  },
                  error: function (respuesta) {
                      $('#myModal').modal('hide');
                      swal("La notificación de rechazo de factura no ha podido ser enviada");
                  }
              });

              return false;
          }

          function OnSuccess(response) {
              var xmlDoc = $.parseXML(response.d);
              var xml = $(xmlDoc);
              var customers = xml.find("Table");
              var row = $("[id*=gvFacturas] tr:last-child").clone(true);
              $("[id*=gvFacturas] tr").not($("[id*=gvFacturas] tr:first-child")).remove();
              $.each(customers, function () {
                  var customer = $(this);
                  $("td", row).eq(0).html($(this).find("InvoiceKey").text());
                  $("td", row).eq(1).html($(this).find("VendName").text());
                  $("td", row).eq(2).html($(this).find("NodeOc").text());
                  $("td", row).eq(3).html($(this).find("UUID").text());
                  $("td", row).eq(4).html($(this).find("FechaTransaccion").text());
                  $("td", row).eq(5).html($(this).find("Status").text());
                  $("[id*=gvFacturas]").append(row);
                  row = $("[id*=gvFacturas] tr:last-child").clone(true);
              });
              return false;
          }

      });

function alert(campo) {
    $(campo).show("slow").delay(2000).hide("slow")
    return true;
}


