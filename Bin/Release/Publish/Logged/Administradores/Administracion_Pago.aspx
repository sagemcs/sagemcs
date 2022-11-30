<%@ Page Title="Administrar" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Administracion_Pago.aspx.cs" Inherits="Logged_Administrar" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    Version 13-ENERO-2020 By Luis Angel Garcia P
    <META HTTP-EQUIV="Cache-Control" CONTENT ="no-cache">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Pragma" content="no-cache" />
        <script src ="../../Css/sweetalert.min.js" type="text/javascript"></script>
    <script src ="../../Scripts/jquery.blockui.min.js" type="text/javascript"></script>

        <script>
      $(document).ready(function () {

          $('#btn_send_comments').click(function (e) {
              $('#myModal').modal('hide');
              $('#Update').modal('show');
              Proceso(e, "/enviar_notificacion_de_rechazo_de_pago");
              return false;
          });

          $('#btn_No_Send').click(function (e) {
              Proceso(e, "/NOenviar_notificacion_de_rechazo_de_pago")
              return false;
          });

          llave = "";
          Fila  = "";
          folio = "";

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
                          swal('Notificaciones T|SYS|', respuesta.msg, 'success');
                          setTimeout("location.href='admFacturas'", 3000);
                      }
                      else
                      {
                          $('#myModal').modal('hide');
                          $('#Update').modal('hide');
                          swal('Notificaciones T|SYS|', respuesta.msg, 'error');
                          setTimeout("location.href='admFacturas'", 1500);
                      }
   
                  },
                  error: function (respuesta) {
                      $('#Update').modal('hide');
                      $('#myModal').modal('hide');
                      swal("La notificación de rechazo de pago no ha podido ser enviada");
                      setTimeout("location.href='admFacturas'", 2000);
                  }
              });

              return false;
          }
     });

      function alert(campo) {
          $(campo).show("slow").delay(2000).hide("slow")
          return false;
      }
    </script>

    <script>        
            function Pregunta(folio, llave, Fila) {
            swal({
                title: "¿Rechazar este Complemento de Pago?",
                text: "Una vez cancelado, no podrá volver actualizar el status del Complemento de Pago",
                icon: "warning",
                buttons: ["No", "Si"],
                confirmButtonClass: "btn-success",
                dangerMode: true,
            })
                .then((willDelete) => {
                    if (willDelete)
                    {
                      $('#myModal').modal('show');
                      $('#inputFolio').attr('value', folio);
                      $('#inputllave').attr('value', llave);
                      $('#inputFila').attr('value', Fila);
                    }
                    else
                    {   
                        folio = "";
                        llave = "";
                        Fila = "";
                        //swal("No se realizó ningun cambio!");
                        setTimeout("location.href='admFacturas'", 300);
                    }
                });
                
            return false;
        }
    </script>

    <script>
        function alertme(titulo,mesaje,Tipo) {
            swal(titulo, mesaje, Tipo)
            unblockUI();
        }
    </script>




  <SCRIPT language="JavaScript">
  function numbersonly(e)
  {
    var unicode=e.charCode? e.charCode : e.keyCode
    if (unicode!=8 && unicode!=44)
    {
        if (unicode < 46 || unicode > 57) //if not a number
      { return false} //disable key press    
    }  
  }  
</SCRIPT>

<%--  <asp:UpdatePanel runat="server" id="UpdatePanel" updatemode="Conditional">
  <ContentTemplate>--%>

    <div class="col-lg-12 col-sm-12 col-12" id="M1">
        <h3>Administracion de Pagos</h3>
    </div>

        <asp:UpdatePanel runat="server" id="UpdatePanel" updatemode="Conditional">
        <ContentTemplate>  


      <div class="row">

      <div class="col-xs-12 md-3 col-lg-3">
         <label>Folio de Pago</label>
        <asp:TextBox runat="server" AutoComplete = "off" AutoCompleteType="Disabled" type="text" MaxLength="20" ID="Folio" class="form-control" TabIndex="3" />
      </div>

      <div class="col-xs-12 md-3 col-lg-3">
          <label>Folio de Factura</label>
        <asp:TextBox runat="server" AutoComplete = "off" AutoCompleteType="Disabled" type="text" MaxLength="20" ID="Factura" class="form-control" TabIndex="4" />
      </div>

     <div class="col-xs-12 md-3 col-lg-3">
          <label>Monto</label>
        <asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" MaxLength="200" ID="Monto" class="form-control" TabIndex="2" onkeypress="return numbersonly(event);" />
      </div>

      <div class="col-xs-12 col-md-3 col-lg-3">
          <label>Estatus</label>
        <asp:DropDownList ID="List" runat="server" class="selectpicker show-tick form-control" AutoPostBack="True" OnSelectedIndexChanged="List_SelectedIndexChanged">
           <asp:ListItem Value="2">Aprobado</asp:ListItem>
           <asp:ListItem Value="1">Pendiente</asp:ListItem>
           <asp:ListItem Value="3">Rechazado</asp:ListItem>
           <asp:ListItem Value="4">Eliminado</asp:ListItem>
        </asp:DropDownList>
      </div>

      <div class="col-xs-12 col-md-3 col-lg-3">
          <label>Proveedor</label>
          <asp:DropDownList ID="Proveedores" runat="server" class="selectpicker show-tick form-control"></asp:DropDownList>
      </div>




      <div class="col-xs-12 md-3 col-lg-3">
        <asp:CheckBox runat="server" id="ChkFechas" Text="Filtrar Por Fechas" AutoPostBack="true" OnCheckedChanged="ChkFechas_CheckedChanged"/>
        <asp:DropDownList ID="LFechas" runat="server" class="selectpicker show-tick form-control">
           <asp:ListItem Value="Factura">Fecha de Recepcion de Pago</asp:ListItem>
           <asp:ListItem Value="Pago">Fecha de Pago</asp:ListItem>
        </asp:DropDownList>
       </div>

      <div class="col-xs-12 md-3 col-lg-3">
          <label>Desde</label>
        <asp:TextBox type="date" name="fecha" ID="F1" min="1980-01-01" max="2050-12-31" AutoComplete = "off" AutoCompleteType="Disabled" step="1"  class="form-control"  runat="server"/>
      </div>

      <div class="col-xs-12 md-3 col-lg-3">
          <label>Hasta</label>
        <asp:TextBox type="date" name="fecha" ID="F2" min="1980-01-01" max="2050-12-31" AutoComplete = "off" AutoCompleteType="Disabled" step="1"  class="form-control"  runat="server"/>
      </div>
        <br />
        <br />
      </div> 

      <div class="row">
              <div class="col-xs-3 col-sm-2 col-md-1">
                  <div class="form-group row">
                      <label class="col-form-label col-xs-11 col-sm-11 col-md-11"></label>
                      <div class="col-xs-4 col-sm-2 col-md-2">
                          <asp:Button Text="Buscar" ID="Button1" runat="server" CssClass="btn btn-primary" OnClick="Buscar" title="Generar Busqueda" />
                      </div>
                  </div>

              </div>

              <div class="col-xs-3 col-sm-2 col-md-1">
                  <div class="form-group row">
                      <label class="col-form-label col-xs-11 col-sm-11 col-md-11"></label>
                      <div class="col-xs-4 col-sm-2 col-md-2">
                          <asp:Button Text="Limpiar" ID="Button2" runat="server" CssClass="btn btn-tsys" OnClick="Limpia" title="Generar Busqueda" />
                      </div>
                  </div>

              </div>
          </div>

            <div class="col-sm-9">
       <div class="alert alert-block alert-success" id="AL2" style="display: none">
         <h5>¡Exito!</h5>
         <asp:Label ID="lblMsj1" runat="server" Text=""></asp:Label>
       </div>
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


      <div class="row">
            <asp:GridView ID="gvFacturas" runat="server" CssClass="table table-bordered bs-table" margin-left="auto" margin-right="auto"  AutoGenerateColumns="False" CellPadding="4" OnRowCommand="GridView2_RowCommand" >
            <AlternatingRowStyle BackColor="White" />
            <HeaderStyle BackColor="#337ab7" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#ffffcc" />
            <EmptyDataRowStyle forecolor="Red" CssClass="table table-bordered" /> 
        <Columns>        
         <asp:BoundField DataField="PKey" HeaderText="ID" ReadOnly="True" SortExpression="Key" />
         <asp:BoundField DataField="Factura" HeaderText="Folio de Factura" HeaderStyle-Width="60px" ReadOnly="True" SortExpression="InvoiceKey" />
         <asp:BoundField DataField="Serie" HeaderText="Serie de Pago" HeaderStyle-Width="65px" ReadOnly="True" SortExpression="VendId" />
         <asp:BoundField DataField="Folio" HeaderText="Folio de Pago" HeaderStyle-Width="65px" ReadOnly="True" SortExpression="OC" />
         <asp:BoundField DataField="Total" HeaderText="Total" HeaderStyle-Width="95px" ReadOnly="True" SortExpression="UUID" />
         <asp:BoundField DataField="Proveedor" HeaderText="Proveedor" HeaderStyle-Width="150px" ReadOnly="True" SortExpression="FechaTransaccion" />
         <asp:BoundField DataField="FechaR" HeaderText="Fecha Recepción de Pago"  HeaderStyle-Width="70px" ReadOnly="True" SortExpression="UUID" />
         <asp:BoundField DataField="FechaP" HeaderText="Fecha Pago" HeaderStyle-Width="70px" ReadOnly="True" SortExpression="UUID" />
         <asp:BoundField DataField="Status" HeaderText="Estatus" ReadOnly="True" SortExpression="Status" />
         <asp:TemplateField>
          <ItemTemplate>
            <asp:Button ID="Documento_1" CssClass="btn btn-default" runat="server" CommandName="Documento_1" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="XML" />
          </ItemTemplate> 
           </asp:TemplateField>
             <asp:TemplateField>
              <ItemTemplate>
            <asp:Button ID="Documento_2" CssClass="btn btn-default" runat="server" CommandName="Documento_2" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="PDF" />
          </ItemTemplate>
         </asp:TemplateField>
         <asp:TemplateField>
          <ItemTemplate>
            <asp:Button ID="Aprobar" CssClass="btn btn-default" runat="server" CommandName="Aprobar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="Aprobar" />
          </ItemTemplate>
         </asp:TemplateField>
             <asp:TemplateField>
          <ItemTemplate>
            <asp:Button ID="Cancelar" CssClass="btn btn-default" runat="server" CommandName="Cancelar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="Rechazar" />
          </ItemTemplate>
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

      </div>

   </ContentTemplate>
   <Triggers>
    <asp:AsyncPostBackTrigger controlid="Button1" eventname="Click" />
    <asp:AsyncPostBackTrigger ControlID="List" EventName="selectedindexchanged" />
<%--    <asp:AsyncPostBackTrigger controlid="gvFacturas" eventname="RowCommand" />--%>
    <asp:PostBackTrigger ControlID="gvFacturas" />
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





</asp:Content>