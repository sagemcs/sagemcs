//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
        $(document).ready(function () {

            var ctrlKeyDown = false;            
            $(document).on("keydown", keydown);
            $(document).on("keyup", keyup);

            function keydown(e) {

                if ((e.which || e.keyCode) == 116 || ((e.which || e.keyCode) == 82 && ctrlKeyDown)) {
                    // Pressing F5 or Ctrl+R
                    VariableG();
                    e.preventDefault();                    
                    setTimeout("location.href='admFacturas'", 10);
                } else if ((e.which || e.keyCode) == 17) {
                    // Pressing  only Ctrl
                    ctrlKeyDown = true;
                }
            };

            function keyup(e) {
                // Key up Ctrl
                if ((e.which || e.keyCode) == 17)
                    ctrlKeyDown = false;
            };

          $('#MainContent_btn_send_comments').click(function (e) {
              $('#MainContent_myModal').modal('hide');
              $('#MainContent_Update').modal('show');
              Proceso(e, "/enviar_notificacion_de_rechazo_de_factura")
              return false;
          });

          $('#MainContent_myModal.btn_No_Send').click(function (e) {
              Proceso(e, "/NOenviar_notificacion_de_rechazo_de_factura");
              //VariableG();
              return false;
          });
          
          function VariableG()
          {
              var Proce = "/Actualizar_Variable";
              var Act = 'Java';
              var Prv_Adm = $('#MainContent_IdProveedor').val();
              var Fol_Adm = $('#MainContent_Folio').val();
              var NOC_Adm = $('#MainContent_NoOC').val();
              var Sts_Adm = $('#MainContent_dpEstatus').val();
              var Chk_Adm = $('#MainContent_chkFechas').prop("checked");
              var Fe1_Adm = $('#MainContent_txtdtp').val();
              var Fe2_Adm = $('#MainContent_txtdtp2').val();
              var url_list = NotificacionesWebService.get_path() + Proce;
              $.ajax({
                  type: "POST",
                  "beforeSend": function (xhr) {
                      xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
                  },
                  url: url_list,
                  dataType: "json",
                  data: {
                      bts: Act,
                      Prv: Prv_Adm,
                      Fol: Fol_Adm,
                      NOC: NOC_Adm,
                      Sts: Sts_Adm,
                      Chk: Chk_Adm,
                      Fe1: Fe1_Adm,
                      Fe2: Fe2_Adm
                  }
              });
          }

          function Proceso(e,Proce)
          {
              e.preventDefault();
              var comentario = $('#MainContent_inputComentario').val();
              var folio_factura = $('#inputFolio').val();
              var folio_llave = $('#inputllave').val();
              var Fila = $('#inputFila').val();
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
                      llave: folio_llave
                  },
                  success: function (respuesta) {
                      $('#myModal').modal('hide');
                      $('#Update').modal('hide');
                      if (respuesta.success) {
                          $('#MainContent_inputComentario').val('');
                          $('#inputFolio').attr('value', '');
                          $('#inputllave').attr('value', '');
                          var table = document.getElementById("<%= gvFacturas.ClientID %>");
                          table.deleteRow(Fila);
                          swal({title:'Notificaciones T|SYS|',text: respuesta.msg,type: 'success'}
                              .then(function () {
                                  VariableG();
                                  setTimeout("location.href='admFacturas'", 10);
                              //window.location = "redirectURL";
                              }));
                          //VariableG();
                          //setTimeout("location.href='admFacturas'", 3000);
                      }
                      else
                      {
                          $('#myModal').modal('hide');
                          $('#Update').modal('hide');
                          swal('Notificaciones T|SYS|', respuesta.msg, 'error');
                          //VariableG();
                          //setTimeout("location.href='admFacturas'", 1500);
                      }
   
                  },
                  error: function (respuesta) {
                      $('#Update').modal('close');
                      $('#myModal').modal('close');
                      swal("La notificación de rechazo de factura no ha podido ser enviada");
                      VariableG();
                      //setTimeout("location.href='admFacturas'", 2000);
                  }
              });

              return false;
          }
     });

      function alert(campo) {
          $(campo).show("slow").delay(2000).hide("slow");
          return true;
      }

      function Pregunta(folio, llave, Fila) {

                swal({
                    title: "\u00BFCancelar esta Factura?",
                    text: "Una vez cancelada, no podr\u00E1 volver actualizar el status de la Factura",
                    icon: "warning",
                    buttons: ["No", "Si"],
                    confirmButtonColor: "#FF6600",
                    closeModal: true,
                    closeOnConfirm: false,
                    dangerMode: true,
                })
                    .then((willDelete) => {
                        if (willDelete) {
                            $('#MainContent_myModal').modal('show');
                            $('#inputFolio').attr('value', folio);
                            $('#inputllave').attr('value', llave);
                            $('#inputFila').attr('value', Fila);
                        }
                        else {
                            folio = "";
                            llave = "";
                            Fila = "";
                            function a()
                            {
                                VariableG();
                            }
                            
                            //setTimeout("location.href='admFacturas'", 300);
                            
                        }
                    });

                $('#MainContent_Update').modal('close');
                $('#MainContent_myModal').modal('close');
                return true;
            }

        function alertme(titulo,mesaje,Tipo) {
            swal(titulo, mesaje, Tipo);
        }