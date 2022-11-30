<%@ Page Title="Administrar" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Administracion_Doc.aspx.cs" Inherits="Logged_Administrar" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <!--Version 08-Abril-2019 By Luis Angel Garcia P-->
    <META HTTP-EQUIV="Cache-Control" CONTENT ="no-cache">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Pragma" content="no-cache" />
    <script src ="../../Css/sweetalert.min.js" type="text/javascript"></script>
    <link href ="../../Css/HojaProv.css" rel="stylesheet" type ="text/css" />
    <script src ="../../Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script src ="../../Scripts/custom.js" type="text/javascript"></script>  


    <script>
        $(document).ready(function () {

            var ctrlKeyDown = false;            
            $(document).on("keydown", keydown);
            $(document).on("keyup", keyup);

            function keydown(e) {

                if ((e.which || e.keyCode) == 116 || ((e.which || e.keyCode) == 82 && ctrlKeyDown)) {
                    // Pressing F5 or Ctrl+R
                    VariableG();
                    e.preventDefault();                    
                    setTimeout("location.href='Administracion_Doc'", 10);
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

          $('#btn_send_comments').click(function (e) {
              $('#myModal').modal('hide');
              $('#Update').modal('show');
              Proceso(e, "/Rechazar_Doc")
              return false;
          });

          $('#btn_No_Send').click(function (e) {
              Proceso(e, "/Rechazar_NoDoc");
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


                          swal({
                              title: "Notificaciones T|SYS|",
                              text: respuesta.msg,
                              icon: "success",
                              buttons: ["OK"],
                              confirmButtonColor: "#FFFFFF",
                              closeModal: true,
                              closeOnConfirm: false,
                              dangerMode: true,
                             })
                            .then((willDelete) => {
                                if (willDelete) {
                                    VariableG();
                                    setTimeout("location.href='Administracion_Docs'", 1);
                                }
                                else {
                                    VariableG();
                                    setTimeout("location.href='Administracion_Doc'", 1);
                                }
                            });
                      }
                      else
                      {
                          $('#myModal').modal('hide');
                          $('#Update').modal('hide');
                          swal('Notificaciones T|SYS|', respuesta.msg, 'error');
                      }
   
                  },
                  error: function (respuesta) {
                      $('#Update').modal('close');
                      $('#myModal').modal('close');
                      swal("La notificación de rechazo de factura no ha podido ser enviada");
                      VariableG();
                  }
              });

              return false;
          }
     });

    </script>

    <script>
            function Pregunta(folio, llave, Fila) {

                swal({
                    title: "¿Desea cancelar este documento?",
                    text: "Una vez cancelado, no podrá volver actualizar el estatus del documento",
                    icon: "warning",
                    buttons: ["No", "Si"],
                    confirmButtonColor: "#FF6600",
                    closeModal: true,
                    closeOnConfirm: false,
                    dangerMode: true,
                })
                    .then((willDelete) => {
                        if (willDelete) {
                            $('#myModal').modal('show');
                            $('#inputFolio').attr('value', folio);
                            $('#inputllave').attr('value', llave);
                            $('#inputFila').attr('value', Fila);
                        }
                        else {
                            folio = "";
                            llave = "";
                            Fila = "";
                            VariableG();                           
                        }
                    });

                $('#Update').modal('close');
                $('#myModal').modal('close');
                return true;
            }
    </script>

    <script>
        function alertme(titulo,mesaje,Tipo) {
            swal(titulo, mesaje, Tipo)
            unblockUI();
            return true;
        }
    </script>

    <SCRIPT language="JavaScript">
  function numbersonly()
  {
      alert("Entra")
  }  
</SCRIPT>


    <div class="col-lg-12 col-sm-12 col-12" id="M1">
        <h3>Administracion de Documentos</h3>
    </div>

  <asp:UpdatePanel runat="server" id="UpdatePanel1">
   <ContentTemplate>

        <div class="col-xs-12 md-12 col-lg-12">
            <div class="col-xs-12 md-8 col-lg-4">
            <label>Compañia:</label>
            <asp:DropDownList ID="SelComp" AutoPostBack="true" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server" OnSelectedIndexChanged="SelComp_SelectedIndexChanged" ></asp:DropDownList>
            </div>
        </div>

        <div class="col-xs-12 md-4 col-lg-4">
        <br />
        <label>Seleccione Proveedor</label>
        <asp:DropDownList ID="SelProv" AutoPostBack="true" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server" OnSelectedIndexChanged="SelProv_SelectedIndexChanged1"></asp:DropDownList>
    </div>   


        <div class="col-xs-12 md-4 col-lg-4">
         <br />
         <label>ID SAGE:</label>
         <asp:TextBox runat="server" type="text" MaxLength="200" ID="IDSAGE" class="form-control" TabIndex="3" readonly="true" />
        </div>

        <div class="col-xs-12 md-4 col-lg-4">
        <br />
        <label>Email:</label>
         <asp:TextBox runat="server" type="text" MaxLength="200" ID="Mail" class="form-control" TabIndex="2" ReadOnly="true" />
        </div>

        <div class="col-xs-12 md-4 col-lg-4">
        <label>Status:</label>
        <asp:DropDownList ID="DropDownList1" AutoPostBack="true" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged1" >
            <asp:ListItem Value="1">Pendiente</asp:ListItem>
            <asp:ListItem Value="2">Aprobado</asp:ListItem>
        </asp:DropDownList>
        </div>
        

        <div class="col-xs-12 md-12 col-lg-12">
            <br />
        </div>
        <div class="col-xs-12 md-12 col-lg-12">
            <p>Selecciona unicamente los documentos que sean aprobados</p>
        </div>
     
     <asp:GridView ID="gvFacturas" runat="server" CssClass="table table-bordered bs-table" margin-left="auto" margin-right="auto"  OnRowDataBound="GridView2_RowDataBound" AutoGenerateColumns="False" CellPadding="1" OnRowCommand="GridView2_RowCommand" GridLines="Horizontal" HorizontalAlign="Center" >
            <AlternatingRowStyle BackColor="White" />
            <HeaderStyle BackColor="#337ab7" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#ffffcc" />
            <EmptyDataRowStyle forecolor="Red" CssClass="table table-bordered" /> 
        <Columns>        

         <asp:TemplateField  ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="5px">
          <ItemTemplate>
             <asp:LinkButton ID="Documento_1" Widht="5px" CssClass="btn btn-info" title="Descargar Documento" runat="server" CommandName="Documento_1" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"><span aria-hidden="true" class="glyphicon glyphicon-download-alt"></span></asp:LinkButton>
          </ItemTemplate> 
             <HeaderStyle Width="5px" />
             <ItemStyle HorizontalAlign="Center" />
         </asp:TemplateField>

         <asp:TemplateField  ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="15px"  HeaderText="Aprobado" ValidateRequestMode="Enabled">
          <ItemTemplate>
                <asp:CheckBox ID="Check" CssClass="ChkBoxClass" style="width: 20px; height: 20px;" runat ="server" CommandName="Check" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
          </ItemTemplate> 
             <HeaderStyle Width="15px" />
             <ItemStyle HorizontalAlign="Center" />
         </asp:TemplateField>

         <asp:BoundField DataField="status" HeaderText="Status" ReadOnly="True" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="100px">
                <HeaderStyle Width="100px" HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>

                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="100px"></ItemStyle>
            </asp:BoundField>

         <asp:BoundField DataField="Nombre" HeaderText="Documento" ReadOnly="True" ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="300px">
                <HeaderStyle Width="300px" HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>

                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="300px"></ItemStyle>
            </asp:BoundField>

         <asp:BoundField DataField="Archivo" HeaderText="Archivo Recibido" ReadOnly="False" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="60px" Visible ="false">
                <HeaderStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>

                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="60px"></ItemStyle>
            </asp:BoundField>

         <asp:BoundField DataField="Fecha" HeaderText="Fecha Aceptación" ReadOnly="True" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="180px">
                <HeaderStyle Width="180px" HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>

                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="180px"></ItemStyle>
         </asp:BoundField>

         <asp:BoundField DataField="Fecha1" HeaderText="Fecha Recepción" ReadOnly="True" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="180px">
                <HeaderStyle Width="180px" HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>

                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="180px"></ItemStyle>
         </asp:BoundField>

         <asp:TemplateField  ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="5px">
          <ItemTemplate>
             <asp:LinkButton ID="Documento_D" Widht="5px" CssClass="btn btn-danger" title="Cancelar Documento" runat="server" CommandName="Documento_D" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"><span aria-hidden="true" class="glyphicon glyphicon-remove"></asp:LinkButton>
          </ItemTemplate> 
             <HeaderStyle Width="5px" />
             <ItemStyle HorizontalAlign="Center" />
         </asp:TemplateField>


         </Columns>
        <FooterStyle BackColor="#507CD1" ForeColor="White" Font-Bold="True" /> 
        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" /> 
        <RowStyle CssClass="table table-bordered" BackColor="#EFF3FB" /> 
        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" /> 

        <SortedAscendingCellStyle BackColor="#F5F7FB" />
        <SortedAscendingHeaderStyle BackColor="#6D95E1" />
        <SortedDescendingCellStyle BackColor="#E9EBEF" />
        <SortedDescendingHeaderStyle BackColor="#4870BE" />
    </asp:GridView>

     <div class="row">
          <div class="col-xs-11 col-sm-3 col-md-3">
              <div class="form-group row">
                  <div class="col-xs-11 col-sm-6 col-md-6">
                      <asp:Button Text="Seleccionar Todos" ID="Button2" runat="server" CssClass="btn btn-tsys" title="Seleccionar Todo"  OnClick="OkAll" />
                  </div>

                   <div class="col-xs-11 col-sm-6 col-md-6">
                      <asp:Button Text="Quitar Seleccion" ID="Button3" runat="server" CssClass="btn btn-success" title="Deseleccionar Todo" OnClick="NoAll" />
                  </div>
              </div>

          </div>

          <label class="col-form-label col-xs-11 col-sm-6 col-md-6"></label>

          <div class="col-xs-11 col-sm-3 col-md-3">
              <div class="form-group row">
                  <label class="col-form-label col-sm-11"></label>
                  <div class="col-sm-1">
                      <asp:Button Text="Guardar" ID="Button1" runat="server" CssClass="btn btn-primary cargar" title="Guardar Datos" OnClick="Aceptar" />
                  </div>
              </div>
          </div>
   </div>
   
<%--   </ContentTemplate>
  </asp:UpdatePanel>--%>


      <div class="col-xs-12 md-11 col-lg-11">
      <label id="lc" runat ="server">Comentarios</label>
      <asp:TextBox runat="server" type="text" MaxLength="120" onKeyPress="return numbersonly();" Maxheight="300px" MaxWidht="700px" class="form-control" ID="Comentarios"></asp:TextBox>
      </div>
      <asp:Panel runat="server" ID="DatosV" Width="90%" Height=" 300px">
        <div class="col-xs-12">
            <font size="5" color="#A4A4A4">No Se Encontraron Resultados</font>
        </div>
        <div class="col-xs-12" id="Timg">
            <center>     
               <asp:Image runat="server" ID="PckT" Width="50%" Height="50%" ImageUrl="../../Img/TGris.png" />      
            </center>
        </div>
    </asp:Panel>

     </ContentTemplate>
   <Triggers>
        <asp:AsyncPostBackTrigger ControlID="SelProv" EventName="selectedindexchanged" />
        <asp:AsyncPostBackTrigger controlid="Button2" eventname="Click" />
        <asp:AsyncPostBackTrigger ControlID="Button3" EventName="Click" />
        <asp:PostBackTrigger ControlID="gvFacturas"/>
    </Triggers>
  </asp:UpdatePanel>



            <div id="Update" style="display:none">
            </div>

            <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span
                                    aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title"
                            id="myModalLabel">¿Desea notificar al Proveedor?</h4>
                    </div>
                    <div class="modal-body">

                            <div class="container-fluid">
                                <div class="row">
                                    Agregar Comentario:
                                    <div class="col-md-12">
                                           <div class="form-group">
                                               <asp:TextBox ID="inputComentario" MaxLength="200" runat="server" CssClass="form-control" AutoComplete = "off" AutoCompleteType="Disabled"  ></asp:TextBox>
                                               <input type="hidden" id="inputFolio" /> 
                                               <input type="hidden" id="inputllave" /> 
                                               <input type="hidden" id="inputFila" /> 
                                            </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="modal-footer">
                            <button id="btn_No_Send" type="button" class="btn btn-tsys pull-left" data-dismiss="modal">No Enviar
                            </button>
                           

                            <button id="btn_send_comments" class="btn btn-primary" title="Rechazar Factura" data-toggle="tooltip">
                                Si Enviar
                            </button>
                        </div>
                </div>
            </div>
        </div>


    <br />
    <br />
    <br />
    <br />


</asp:Content>