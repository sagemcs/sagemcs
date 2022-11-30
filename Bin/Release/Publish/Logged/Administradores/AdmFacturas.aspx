<%@ Page Title="Administracion de Facturas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="AdmFacturas.aspx.cs" Inherits="Logged_AdmFacturas" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!--Version 10-JUNIO-2019 By Luis Angel-->

    <META HTTP-EQUIV="Cache-Control" CONTENT ="no-cache">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="Last-Modified" content="0">
    <meta http-equiv="Content-Type" content="text/html; charset=ISO-8859-1">
    <meta http-equiv="Cache-Control" content="no-cache, mustrevalidate">
    <script src ="../../Css/sweetalert.min.js" type="text/javascript"></script>
    <script src ="../../Scripts/jquery.blockui.min.js" type ="text/javascript"></script>
    <script src ="../../Scripts/custom.js" type ="text/javascript"></script>
<%--    <script src ="../../Scripts/AdmFac.js" type ="text/javascript"></script>--%>

    <style>
        #Update{
        position :fixed;
        top:40%;
        bottom :40%;
        left : 40%;
        right: 40%;
        position: absolute;
        z-index : 1001;
        overflow : hidden;
        margin :0;
        padding :0;
        background-color : #fcfcfc;
        filter :alpha(opacity =99);
        opacity : 2.50;
        border: 1px solid gray;
        border:none;
        background-image :url("../../Img/mail_success.gif");
        background-repeat :no-repeat;
        background-size:cover;
        background-position:center;
        border-radius: 20px 20px 20px 20px;
        -moz-border-radius: 20px 20px 20px 20px;
        -webkit-border-radius: 20px 20px 20px 20px;
        /*border: 0px none #000000;*/
}
    </style>

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

          $('#btn_send_comments').click(function (e) {
              $('#myModal').modal('hide');
              $('#Update').modal('show');
              Proceso(e, "/enviar_notificacion_de_rechazo_de_factura")
              return false;
          });

          $('#btn_No_Send').click(function (e) {
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
                                    setTimeout("location.href='admFacturas'", 1);
                                }
                                else {
                                    VariableG();
                                    setTimeout("location.href='admFacturas'", 1);
                                }
                            });

                          //swal({
                          //    title: "Notificaciones T|SYS|",
                          //    text: respuesta.msg,
                          //    icon: "success",
                          //    buttons: ["OK"],
                          //}, function () {
                          //    location.href = "admFacturas";
                          //});

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
    </script>

    <script>
            function Pregunta(folio, llave, Fila) {

                swal({
                    title: "¿Cancelar esta Factura?",
                    text: "Una vez cancelada, no podrá volver actualizar el status de la Factura",
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
                            //setTimeout("location.href='admFacturas'", 300);
                            
                        }
                    });

                $('#Update').modal('close');
                $('#myModal').modal('close');
                return true;
            }
    </script>

    <script>
        function alertme(titulo,mesaje,Tipo) {
            swal(titulo, mesaje, Tipo);
        }
    </script>
        <br />
        <br />
        <br />
        <br />
        <div class="col-lg-11 col-sm-11 col-xs-11">
        <label class="col-form-label col-lg-5 col-sm-3 col-xs-1"></label>
        <div class="form-group col-lg-6 col-sm-8 col-xs-10">
            <h3>Administración de Facturas</h3>
        </div>
    </div>

        <asp:UpdatePanel runat="server" id="UpdatePanel" updatemode="Conditional">
        <ContentTemplate>   

       <div class="col-xs-12 md-3 col-lg-3">
        <label>Proveedor</label>
        <asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" ID="IdProveedor" class="form-control" TabIndex="3" />
      </div>

      <div class="col-xs-12 md-3 col-lg-3">
        <label>Folio</label>
        <asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" ID="Folio" class="form-control" TabIndex="3" />
      </div>

     <div class="col-xs-12 md-3 col-lg-3">
        <label>No.O.C.</label>
        <asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" ID="NoOC" class="form-control" TabIndex="3" />
      </div>

<%--      <div class="col-xs-12 md-3 col-lg-3">
        <label>R.F.C</label>
        <asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" ID="RFC" class="form-control" TabIndex="3" />
      </div>--%>

<%--      <div class="col-xs-12 md-3 col-lg-3" >
        <label >UUID</label>
        <asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" ID="UUID" class="form-control" TabIndex="3" />
      </div>--%>

      <div class="col-xs-12 col-md-3 col-lg-3">
      <label>Estatus</label>
        <asp:DropDownList ID="dpEstatus" runat="server" class="selectpicker show-tick form-control" AutoPostBack="true" OnSelectedIndexChanged="dpEstatus_SelectedIndexChanged" >
            <asp:ListItem Value="1">Pendiente</asp:ListItem>
            <asp:ListItem Value="2">Aprobado</asp:ListItem>
            <asp:ListItem Value="3">Rechazado</asp:ListItem>
            <asp:ListItem Value="4">Aplicado</asp:ListItem>
            <asp:ListItem Value="5">En Contrarecibo</asp:ListItem>
            <asp:ListItem Value="6">En Solicitud</asp:ListItem>
            <asp:ListItem Value="7">Pagado</asp:ListItem>
            <asp:ListItem Value="8">Aprobacion Complemento de Pago</asp:ListItem>
            <asp:ListItem Value="9">Proceso Finalizado</asp:ListItem>
<%--            <asp:ListItem Value="10">Eliminado</asp:ListItem>--%>
        </asp:DropDownList>
      </div>
    
      <div class="col-xs-6 md-4 col-lg-3">
          <asp:CheckBox ID="chkFechas" runat="server" Text="Incluir Fechas" />
        <asp:TextBox type="date" name="fecha" ID="txtdtp" AutoComplete = "off" AutoCompleteType="Disabled" min="1980-01-01" max="2050-12-31" step="1"  class="form-control"  runat="server"/>
      </div>

      <div class="col-xs-6 md-4 col-lg-3">
          <label>Fecha Final</label>
        <asp:TextBox type="date" name="fecha" ID="txtdtp2" AutoComplete = "off" AutoCompleteType="Disabled" min="1980-01-01" max="2050-12-31" step="1"  class="form-control"  runat="server"/>
      </div>

    <div class="col-xs-11 col-sm-11 col-md-11">
        <div class="form-group row">
            <label class="col-form-label col-xs-12 col-sm-12 col-md-12"></label>
            <div class="col-xs-4 col-sm-2 col-md-1">
                <asp:Button ID="Buscar" runat="server" Text="Buscar" OnClick="Buscar_Click1" CssClass="btn btn-primary" title="Generar Busqueda" />
            </div>

            <div class="col-xs-4 col-sm-2 col-md-1">
                <asp:Button ID="Button2" runat="server" Text="Limpiar" OnClick="Limpiar_Click1" CssClass="btn btn-tsys" title="Limpiar Busqueda" />
            </div>
        </div>


    </div>

    <div class="col-sm-9">
        <div class="alert alert-block alert-danger" id="AL" style="display: none">
          <h5>¡Error!</h5>
          <asp:Label ID="Label4" runat="server" Text=""></asp:Label>
        </div>
    </div>
 
    <div class="col-sm-9">
       <div class="alert alert-block alert-danger" id="AL1" style="display: none">
        <h5>¡Error!</h5>
        <asp:Label ID="lblMsj" runat="server" Text=""></asp:Label>
       </div>
    </div>

    <div class="col-sm-9">
       <div class="alert alert-block alert-danger" id="AL3" style="display: none">
        <h5>¡Se ha Rechazado la Factura!</h5>
<%--        <asp:Label ID="lblMsj2" runat="server" Text=""></asp:Label>--%>
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



<%--    <asp:Panel ID="Panel1" runat="server" BorderWidth="0">--%>

    <asp:GridView ID="gvFacturas" runat="server" CssClass="table table-bordered bs-table" margin-left="auto" margin-right="auto"  AutoGenerateColumns="False" CellPadding="4" OnRowCommand="GridView1_RowCommand"  >
            <AlternatingRowStyle BackColor="White" />
            <HeaderStyle BackColor="#337ab7" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#ffffcc" />
            <EmptyDataRowStyle forecolor="Red" CssClass="table table-bordered" /> 
        <Columns>          
         <asp:BoundField DataField="InvoiceKey" HeaderText="ID.Factura" ReadOnly="True" SortExpression="InvoiceKey" />
         <asp:BoundField DataField="VendName" HeaderText="Proveedor" ReadOnly="True" SortExpression="VendId" />
         <asp:BoundField DataField="NodeOc" HeaderText="No. O.C." ReadOnly="True" SortExpression="OC" />
         <asp:BoundField DataField="UUID" HeaderText="Folio Factura" ReadOnly="True" SortExpression="UUID" />
         <asp:BoundField DataField="FechaTransaccion" HeaderText="Fecha de Factura" ReadOnly="True" SortExpression="FechaTransaccion" />
         <asp:BoundField DataField="FechaCarga" HeaderText="Fecha de Recepción" ReadOnly="True" SortExpression="FechaCarga" />
         <asp:BoundField DataField="Status" HeaderText="Estatus" ReadOnly="True" SortExpression="Status" />
         
         <asp:TemplateField>
          <ItemTemplate>
           <asp:Button ID="Documento_1" CssClass="btn btn-default" runat="server" CommandName="Documento_1" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="XML" Visible='<%# Eval("Tipo").ToString() == "Extranjero" ? false : true %>' />
              <%--<asp:Button ID="Documento_1" CssClass="btn btn-default" runat="server" CommandName="Documento_1" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="XML" />--%>
          </ItemTemplate> 
           </asp:TemplateField>
             <asp:TemplateField>
              <ItemTemplate>
            <asp:Button ID="Documento_2" CssClass="btn btn-default" runat="server" CommandName="Documento_2" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="Comprobante PDF" />
          </ItemTemplate>
         </asp:TemplateField>
         <asp:TemplateField>
          <ItemTemplate>
            <asp:Button ID="Documento_3" CssClass="btn btn-default" runat="server" CommandName="Documento_3" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="Anexo PDF" />
          </ItemTemplate>
         </asp:TemplateField>
                 <asp:TemplateField>
          <ItemTemplate>
            <asp:Button ID="Aprobar" CssClass="btn btn-default cargar" runat="server" CommandName="Aprobar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="Aprobar" />
          </ItemTemplate>
         </asp:TemplateField>
             <asp:TemplateField>
          <ItemTemplate>
            <asp:Button ID="Cancelar" CssClass="btn btn-default" runat="server" CommandName="Cancelar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="Rechazar" />
          </ItemTemplate>
         </asp:TemplateField> 

        <asp:BoundField DataField="Tipo" HeaderText="Tipo" ReadOnly="True" SortExpression="Tipo" Visible="false" />
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
    <br />
    <asp:GridView ID="gvValidacion" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered bs-table" margin-left="auto" margin-right="auto" CellPadding="4" >
            <AlternatingRowStyle BackColor="White" />
            <HeaderStyle BackColor="Tomato" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#ffffcc" />
            <EmptyDataRowStyle forecolor="Red" CssClass="table table-bordered" /> 
        <RowStyle BackColor="White" ForeColor="DarkRed" /> 
        <Columns>
         <asp:BoundField DataField="InvoiceKey" HeaderText="ID" ReadOnly="True" SortExpression="InvoiceKey" />
         <asp:BoundField DataField="folio" HeaderText="folio" ReadOnly="True" SortExpression="folio" />
         <asp:BoundField DataField="UUID" HeaderText="UUID" ReadOnly="True" SortExpression="UUID" />
         <asp:BoundField DataField="NodeOc" HeaderText="O.C." ReadOnly="True" SortExpression="NodeOc" />
         <asp:BoundField DataField="FechaTransaccion" HeaderText="Fecha" ReadOnly="True" SortExpression="FechaError" />
         <asp:BoundField DataField="ErrorValidacion" HeaderText="ErrorValidacion" ReadOnly="True" SortExpression="ErrorValidacion" />
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


   </ContentTemplate>
   <Triggers>
    <asp:AsyncPostBackTrigger controlid="Buscar" eventname="Click" />
    <asp:AsyncPostBackTrigger ControlID="dpEstatus" EventName="selectedindexchanged" />
    <asp:AsyncPostBackTrigger controlid="Buscar" eventname="Click" />
<%--    <asp:AsyncPostBackTrigger ControlID="gvFacturas" eventname="RowCommand"  />--%>
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