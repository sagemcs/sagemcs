<%@ Page title="Edit Pre Registro" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeFile="EditPreRegistro.aspx.cs" Inherits="EditPreRegistro" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    Version Actualizacion 08-Abril-2019 p.m. By Luis Angel Garcia P
    <META HTTP-EQUIV="Cache-Control" CONTENT ="no-cache">
    <meta http-equiv="Expires" content="0" />
<meta http-equiv="Pragma" content="no-cache" />
    <script src ="../../Css/sweetalert.min.js" type="text/javascript"></script>
    <script>
        function alert(campo) {
            $(campo).show("slow").delay(2000).hide("slow")
            return true;
        }
    </script>
    <script>
        function Pregunta() { 
        swal({
            title: "¿Confirma cancelar esta Factura?",
            text: "Una vez cancelada, no podrá volver actualizar el status de la Factura",
            icon: "warning",
            buttons: true,
            dangerMode: true,
            })
        .then((willDelete) => {
            if (willDelete) {

            } else {
                return false;
            }
        });
        }
    </script>
    <script>
        function alertme(titulo,mesaje,Tipo) {
            swal(titulo, mesaje, Tipo)
        }
    </script>
        <script type="text/javascript">
        $(function () {
            // We can attach the `fileselect` event to all file inputs on the page
            $(document).on('change', ':file', function () {
                var input = $(this),
                    numFiles = input.get(0).files ? input.get(0).files.length : 1,
                    label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
                input.trigger('fileselect', [numFiles, label]);
            });

            // We can watch for our custom `fileselect` event like this
            $(document).ready(function () {
                $(':file').on('fileselect', function (event, numFiles, label) {

                    var input = $(this).parents('.input-group').find(':text'),
                        log = numFiles > 1 ? numFiles + ' files selected' : label;

                    if (input.length) {
                        input.val(log);
                    } else {
                        if (log) alert(log);
                    }
                });
            });
            return true;
        })
    </script>

   <div class="col-lg-12 col-sm-5 col-12" id="M1">
      <h3>Edición de Usuarios</h3>
    </div>


    <!-- Nav pills -->
<ul class="nav nav-pills">
  <li class="nav-item active">
    <a class="nav nav-pills nav-justified" data-toggle="pill" href="#home">Proveedores&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </a>
  &nbsp;</li>
  <li class="nav-item">
    <a class="nav nav-pills nav-justified" data-toggle="pill" href="#menu1">Usuarios T|SYS|</a>
  </li>
</ul>

<!-- Tab panes -->
<div class="tab-content">
     <!-- Registro Provedores Tsys -->
  <div class="tab-pane container active" id="home">
      <asp:UpdatePanel runat="server" id="UpdatePanel" updatemode="Conditional">

          <Triggers>
              <asp:AsyncPostBackTrigger controlid="Button1" eventname="Click" />
              <asp:AsyncPostBackTrigger controlid="Button2" eventname="Click" />
              <%--<asp:AsyncPostBackTrigger ControlID ="BnEnv" EventName ="Click" />--%>
            </Triggers>
          
          <ContentTemplate>

            <div class="row" id="Cam">
             <div class="col-xs-6 col-sm-6 col-md-6">
                    <div class="form-group row">
                        <label class="col-form-label col-sm-4">Correo Electronico:</label>
                        <div class="col-sm-8">
                            <asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" MaxLength="80" ID="Email" class="form-control" TabIndex="1" />
                            <div class="alert alert-block alert-danger" id="AID" style="display: none">
                                <h5>¡Error!</h5>
                                <asp:Label runat="server" ID="CMail"></asp:Label>
                            </div>
                            <div class="alert alert-block alert-danger" id="AID1" style="display: none">
                                <h5>¡Error!</h5>
                                Email de Proveedor No Encontrado, Verificalo en SAGE
                            </div>
                            <div class="alert alert-block alert-danger" id="AID2" style="display: none">
                                <h5>¡Error!</h5>
                                <asp:Label runat="server" ID ="Lsage"></asp:Label>
                            </div>
                        </div>
                    </div>
              </div>
                <div class="btn-toolbar" role="toolbar">
                    <div class="col-xs-1 btn-group">
                        <asp:LinkButton runat="server" type="button" Class="btn btn-tsys" ID="BtnBusc" OnClick="Btn_Buscar">                                        
                          <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
                        </asp:LinkButton>
                    </div>
                </div>
          </div>
            
             <%-- ADD New 2022--%>
            <div class="form-group row">
            <label class="col-form-label col-sm-2">   Tipo de Proveedor:</label>
             <div class="col-sm-4">
               <asp:DropDownList ID="STipoProv" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server" >
                 <asp:ListItem Value=""> </asp:ListItem>
                 <asp:ListItem Value="n">Proveedor Con Adenda</asp:ListItem>
                 <asp:ListItem Value="s">Proveedor Sin Adenda</asp:ListItem>
                 <asp:ListItem Value="e">Proveedor Extranjero</asp:ListItem>                
                 
               </asp:DropDownList>
               <%--<asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" MaxLength="30" ID="TipoProv" class="form-control" ReadOnly="true" TabIndex="1" />--%>
               <div class="alert alert-block alert-danger" id="AID3" style="display: none">
                <h5>¡Error!</h5>
                 <asp:Label runat="server" ID ="tplabel"></asp:Label>
              </div>
             </div>
            </div>
            <%-- ADD New 2022--%>


            <div class="form-group row" id="Cam1">
                <label class="col-form-label col-sm-2">Razón Social:</label>
                <div class="col-sm-8">
                    <asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" name="Razon" ID="Razon" MaxLength="100" class="form-control" placeholder="" TabIndex="3" ReadOnly="true" />
                </div>
                <div class="col-sm-8">
                    <asp:DropDownList runat="server" ID="Cclientes" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary"  AutoPostBack="True" OnSelectedIndexChanged="Cclientes_SelectedIndexChanged"  />
                </div>
            </div>

            <div class="row" id="Cam2">
                <div class="col-xs-6 col-sm-6 col-md-6">
                    <div class="form-group row">
                        <label class="col-form-label col-sm-4">RFC:</label>
                        <div class="col-sm-8">
                            <asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" name="RFC" MaxLength="13" ID="RFC" class="form-control" placeholder="" TabIndex="4" ReadOnly="true" />
                            <div class="alert alert-block alert-danger" id="ARF1" style="display: none">
                                <h5>¡Error!</h5>
                                El Campo RFC debe de ser a 12 Caracteres.
                            </div>
                            <div class="alert alert-block alert-danger" id="ARF2" style="display: none">
                                <h5>¡Error!</h5>
                                El Campo RFC es Obligatorio.
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-xs-6 col-sm-6 col-md-6">
                    <div class="form-group row" id="RF">
                        <label class="col-form-label col-sm-2">ID SAGE :</label>
                        <div class="col-sm-6">
                            <asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" name="IDSAGE" ID="IDSAGE" class="form-control" MaxLength="12" placeholder="" TabIndex="2" ReadOnly="true" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-xs-8 col-sm-8 col-md-8">
                    <div class="form-group row">
                       <label class="col-form-label col-sm-3">Empresa Destino:</label>
                      <div class="col-sm-6">
                          <asp:DropDownList ID="EmpresaP" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server" ></asp:DropDownList>
                      </div>
                </div>
           </div>
           </div> 

            <div class="alert alert-block alert-danger" id="ATD" style="display: none">
                <h5>¡Error!</h5>
                Todos los Campos estan Vacios, Son Obligatorios
            </div>

            <div id="Acc">
            <div class="col-xs-3 col-md-1">
                    <asp:Button Text="Agregar" runat="server" CssClass="btn btn-tsys" title="Agregar Usuario a la Lista" OnClick="Campos" ID="Button1" />
                </div>
            <div class="col-xs-3 col-md-1">
                    <asp:Button Text="Limpiar" runat="server" CssClass="btn btn-success" title="Agregar Usuario a la Lista" OnClick="Unnamed1_Clean" />
                </div>
            </div>

              <br />
              <br />
              <br />        




      <%--<div class="row">
        <div class="col-xs-10 col-sm-10 col-md-10">
            <div class="form-group row">
                <label class="col-form-label col-sm-11"></label>
                <div class="col-sm-1">
                    <asp:Button Text="Guardar" runat="server" id="BnEnv" autoposback="True" CssClass="btn btn-primary" title="Guardar Datos" OnClick="btn_Enviar" />
                </div>
            </div>

        </div>
    </div>--%>

  </ContentTemplate>
  </asp:UpdatePanel>

  </div>

    <!-- Registro Ususarios Tsys -->
  <div class="tab-pane container fade" id="menu1">      
            
      <asp:UpdatePanel runat="server" ID="upGrid" updatemode="Conditional" >
          
          <ContentTemplate>
                <%-- Grid de usuarios--%>
            <div class="row">
             <asp:GridView ID="GridView2" CssClass="table table-bordered bs-table" 
                    margin-left="auto" margin-right="auto" runat="server" 
                    AutoGenerateColumns="False"                    
                    onrowcancelingedit="GridView2_RowCancelingEdit" 
                    OnRowCreated = "GridView2_RowCreated" 
                    onrowupdating="GridView2_RowUpdating" 
                    OnRowDataBound="GridView2_RowDataBound"
                    AllowCustomPaging="true"
                    width="99%">
                    <AlternatingRowStyle BackColor="White" />
                    <HeaderStyle BackColor="#337ab7" Font-Bold="True" ForeColor="White" />
                    <EditRowStyle BackColor="#ffffcc" />
                    <EmptyDataRowStyle forecolor="Red" CssClass="table table-bordered" />

                    <Columns>
                        <%--botones de acción sobre los registros...--%>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="80px">
                            <ItemTemplate>
                                <%--Botones de eliminar y editar cliente...--%>
                                <asp:Button ID="btnCancel" runat="server" Text="Editar Usuario" CssClass="btn btn-success" CommandName="Cancel"  OnClientClick="return confirm('¿Seguro que quiere Aprobar los datos del cliente?');"/>
                            </ItemTemplate>
                            <EditItemTemplate>
                            </EditItemTemplate>
                             
                            <HeaderStyle Width="80px"></HeaderStyle>

                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        </asp:TemplateField>

                        <asp:BoundField DataField="UserKey" ReadOnly="True" ItemStyle-Width="5%">                            
                        </asp:BoundField>
                        <asp:BoundField DataField="UserID" HeaderText="ID USUARIO" HeaderStyle-Width="50px" ReadOnly="True">
                            <HeaderStyle Width="50px"></HeaderStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="UserName" HeaderText="NOMBRE USUARIO" HeaderStyle-Width="80px" ReadOnly="True">
                            <HeaderStyle Width="75px"></HeaderStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="Email" HeaderText="Email" HeaderStyle-Width="140px" ReadOnly="True">
                            <HeaderStyle Width="75px"></HeaderStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="Rol" HeaderText="Rol" HeaderStyle-Width="140px" ReadOnly="True">
                            <HeaderStyle Width="75px"></HeaderStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="RoleKey" HeaderText="RoleKey" HeaderStyle-Width="140px" ReadOnly="True">
                            <HeaderStyle Width="75px"></HeaderStyle>
                        </asp:BoundField>

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

               <%--Custom Navigator--%>
                <div id="navigator">
                    <div class="col-xs-2 col-sm-2 col-md-2">
                        <asp:Button Text="Anterior" runat="server" CssClass="btn btn-tsys" title="Anterior" OnClick="btn_anterior_Click" ID="btn_anterior" />
                    </div>
                    <div class="col-xs-2 col-sm-2 col-md-2">
                        <asp:Label runat="server" Text="" ID="lbl_pages"></asp:Label>
                    </div>
                    <div class="col-xs-2 col-sm-2 col-md-2s">
                        <asp:Button Text="Siguiente" runat="server" CssClass="btn btn-tsys" title="Siguiente" OnClick="btn_siguiente_Click" ID="btn_siguiente" />              
                    </div>
                </div>             
         </div>       
          </ContentTemplate>
          <Triggers>
              <asp:AsyncPostBackTrigger ControlID ="GridView2" EventName ="PageIndexChanging" />
              <asp:AsyncPostBackTrigger ControlID ="btn_anterior" EventName ="Click" />
              <asp:AsyncPostBackTrigger ControlID ="btn_siguiente" EventName ="Click" />
              <asp:AsyncPostBackTrigger ControlID ="Button2" EventName ="Click" />
            </Triggers>
      </asp:UpdatePanel>
      

        <asp:UpdatePanel runat="server" id="UpdatePanel1" updatemode="Conditional">
        <Triggers>
            <%--<asp:AsyncPostBackTrigger ControlID ="BntTsys" EventName ="Click" />--%>
            <asp:AsyncPostBackTrigger ControlID ="GridView2" EventName ="RowCancelingEdit" />
        </Triggers>
        <ContentTemplate>

           <%-- NOmbre Usuario--%>
            <div class="form-group row" id="Cam4" >
                <label class="col-form-label col-sm-2">Nombre Usuario:</label>
                <div class="col-sm-6">
                    <table>
                        <tr>
                            <td>                    
                                <asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" name="Razon" ID="NombreT" MaxLength="90" class="form-control" placeholder="" TabIndex="3" />
                            </td>
                            <td>
                                <table>
                                    <tr style="max-height: 100%">
                                        <td>
                                            <asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" name="Razon" ID="TextBoxSearchName" MaxLength="90" class="form-control" placeholder="" TabIndex="3" style="margin-left: 2rem" />
                                        </td>
                                        <td>
                                            <asp:Button Text="Cargar" runat="server" CssClass="btn btn-tsys" title="Cargar usuario" OnClick="CharginUserName" ID="ButtonCharge" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <div class="alert alert-block alert-danger" id="AUT" style="display: none">
                        <h5>¡Error!</h5>
                        El Campo Nombre Usuario es Obligatorio.
                    </div>
                </div>
            </div>

            <%--Correo Electronico--%>
            <div class="" id="Cam5">
                <%--<div class="col-xs-10 col-sm-10 col-md-6">--%>
                    <div class="form-group row">
                        <label class="col-form-label col-sm-2">Correo Electrónico:</label>
                        <div class="col-sm-6">
                            <table>
                                <tr>
                                    <td>                    
                                        <asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" name="Email" MaxLength="100" ID="EmailT" class="form-control" placeholder="" TabIndex="4" />
                                    </td>
                                    <td>
                                        <table>
                                            <tr style="max-height: 100%">
                                                <td>
                                                    <asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" name="Razon" ID="TextBoxSearchEmail" MaxLength="90" class="form-control" placeholder="" TabIndex="3" style="margin-left: 2rem" />
                                                </td>
                                                <td>
                                                    <asp:Button Text="Cargar" runat="server" CssClass="btn btn-tsys" title="Cargar usuario" OnClick="CharginEmail" ID="Button3" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                    <div class="alert alert-block alert-danger" id="AET" style="display: none">
                        <h5>¡Error!</h5>
                        <asp:Label runat="server" ID="CMT"></asp:Label>
                    </div>
                <%--</div>--%>
            </div>

            <%--Rol Asignado--%>
            <div class="row" id="Cam7">
              <div class="col-xs-6 col-sm-6 col-md-6">
                  <div class="form-group row">
                      <label class="col-form-label col-sm-4">Rol Asignado:</label>
                      <div class="col-sm-4">
                          <asp:DropDownList ID="UsersR" class="selectpicker show-tick form-control" data-live-search="true" AutoPostBack="true" data-style="btn-primary" OnSelectedIndexChanged="dpEstatus_SelectedIndexChanged" runat="server">
                           <asp:ListItem Value="5">T|SYS| - Admin</asp:ListItem>
                           <asp:ListItem Value="7">T|SYS| - Validador</asp:ListItem>
                           <asp:ListItem Value="6">T|SYS| - Consultas</asp:ListItem>
                           <asp:ListItem Value="8">T|SYS| - Empleado</asp:ListItem>
                           <asp:ListItem Value="12">T|SYS| - Gerente</asp:ListItem>
                           <asp:ListItem Value="13">T|SYS| - Tesoreria</asp:ListItem>
                           <asp:ListItem Value="14">T|SYS| - Finanzas</asp:ListItem>
                           </asp:DropDownList>
                      </div>
                  </div>
              </div>
            </div>  
            

            <div class="panel table-bordered" title="Permisos"> 
                <h4>Permisos</h4>
                <div class="row"> 
                    <div class="col-xs-2 col-sm-2 col-md-2">
                        <asp:CheckBox  runat="server" ID="chkReembolso" Text="Reembolsos" />
                    </div>
                    <div class="col-xs-2 col-sm-2 col-md-2">
                        <asp:CheckBox  runat="server" ID="chkAnticipo" Text="Anticipos" />
                    </div>
                    <div class="col-xs-2 col-sm-2 col-md-2">
                        <asp:CheckBox  runat="server" ID="chkTarjeta"  Text="Tarjetas"/>
                    </div>
                    <div class="col-xs-2 col-sm-2 col-md-2">
                        <asp:CheckBox  runat="server" ID="chkMédicos" Text="Gastos Medicos" />
                    </div>    
                    <div class="col-xs-2 col-sm-2 col-md-2">                       
                        <asp:CheckBox  runat="server" ID="chkFacturas" Text="Facturas" />
                    </div>
               </div>
            </div>
          <%-- <%-- Permisos asignados--%>
           <%-- <div class="row" id="Cam8" runat="server">
              <div class="col-xs-8 col-sm-8 col-md-8">
                  <div class="form-group row">
                      <label class="col-form-label col-sm-4">Acceso Actividades:</label>
                      <div class="col-sm-4">
                        <asp:CheckBoxList ID="cblElemetos" runat="server">
                           <asp:ListItem Value="1">Carga de Facturas</asp:ListItem>
                           <asp:ListItem Value="2">Reembolso de Gastos</asp:ListItem>
                           <asp:ListItem Value="3">Anticipo de Gastos</asp:ListItem>
                           <asp:ListItem Value="4">Tarjeta Corporativa</asp:ListItem>
                           <asp:ListItem Value="5">Gastos Médicos Mayores</asp:ListItem>
                        </asp:CheckBoxList>
                      </div>
                  </div>
              </div>
            </div>--%>

           <div class ="col-xs-10">
              <div class="alert alert-block alert-danger" id="ATT" style="display: none">
                  <h5>¡Error!</h5>
                  Todos los Campos estan Vacios, Son Obligatorios
              </div>
            </div>

            <div id="Acc1" class="col-xs-12">
            <div class="col-xs-4 col-md-1">
                    <asp:Button Text="Guardar" runat="server" CssClass="btn btn-tsys" title="Guardar datos de usuario" OnClick="Unnamed2_Click" ID="Button2" />
                </div>
            <div class="col-xs-3 col-md-1">
                    <asp:Button Text="Limpiar" runat="server" CssClass="btn btn-success" title="Limpiar campos" OnClick="Unnamed2_Clean" />
                </div>
            </div>
              <br />
              <br />
              <br />

  </ContentTemplate>
  </asp:UpdatePanel>

          <div class="col-lg-6 col-sm-6 col-xs-10">
          <h4></h4>
          <div class="input-group col-lg-10 col-sm-10 col-xs-10">
              <label class="input-group-btn">
                  <span class="btn btn-primary" Style="display: none;">Seleccionar&hellip;
                      <asp:FileUpload type="file" ID="FileUpload1" runat="server" Style="display: none;" accept=".xls, .xlsx" />
                  </span>
              </label>
              <asp:TextBox type="text" runat="server" AutoComplete = "off" AutoCompleteType="Disabled" Style="display: none;" class="form-control" ID="ID" ReadOnly="true" />
              
          </div>
          <div class="col-sm-1">
          <br />
          <asp:Button Text="Cargar" ID="Button4" runat="server" CssClass="btn btn-danger" Style="display: none;" title="Cargar Datos" OnClick="Excel" />
          </div>         
      </div>
      <span class="col-lg-12"><br /><br /></span>
      <div class="row">
            <asp:GridView ID="GridView3" runat="server"
                CssClass="table table-bordered bs-table"
                AutoGenerateColumns="False"
                BackColor="White" BorderColor="#E7E7FF"
                BorderStyle="None" BorderWidth="1px"
                CellPadding="3" GridLines="Horizontal"
                Height="34px" Width="82%">

                <AlternatingRowStyle BackColor="#F7F7F7" />
                <Columns>
                    <asp:BoundField HeaderText="#" SortExpression="#" DataField="No" />
                    <asp:BoundField HeaderText="ID" SortExpression="ID SAGE" DataField="ID" />
                    <asp:BoundField HeaderText="Razon Social" SortExpression="Razón Social" DataField="Nombre" />
                    <asp:BoundField HeaderText="Company" SortExpression="Company" DataField="Company" />
                    <asp:BoundField HeaderText="Email" SortExpression="Email" DataField="Email" />
                    <asp:BoundField HeaderText="Notificacion" SortExpression="Envio de Correo" DataField="Notif" />
                    <asp:BoundField HeaderText="Anotaciones" SortExpression="Anotaciones" DataField="Anot" />
                </Columns>
                <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
                <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" />
                <PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Right" />
                <RowStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" />
                <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
                <SortedAscendingCellStyle BackColor="#F4F4FD" />
                <SortedAscendingHeaderStyle BackColor="#5A4C9D" />
                <SortedDescendingCellStyle BackColor="#D8D8F0" />
                <SortedDescendingHeaderStyle BackColor="#3E3277" />
            </asp:GridView>
            </div>

  </div>

</div> 

    
</asp:Content>
