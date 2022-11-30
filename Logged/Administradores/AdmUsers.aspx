<%@ Page Language="C#" Title="Administracion de Usuarios" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeFile="AdmUsers.aspx.cs" Inherits="Logged_Administradores_AdmUsers" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    Version 08-Abril-2019 By Luis Angel Garcia P
    <meta http-equiv="Cache-Control" content="no-cache">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Pragma" content="no-cache" />

    <script src="../../Css/sweetalert.min.js" type="text/javascript"></script>
    <script>
        function alert(campo) {
            $(campo).show("slow").delay(3000).hide("slow")
            return true;
        }
    </script>
    <script>
        function alertme(titulo, mesaje, Tipo) {
            swal(titulo, mesaje, Tipo)
        }
    </script>
    <script>
        function Update(Razon, Correo, Status) {
            $('#myModal').modal('show');
            $('#Razon').attr('value', Razon);
            $('#CorreoT').attr('value', Correo);
            $('#Menu').prop('selectedIndex', Status);
            return false;
        }
    </script>
    <script>
        $(document).ready(function () {

            $('#btn_send_comments').click(function (e) {
                Proceso(e, "/Actualizar_Datos")
                return false;
            });

            function Proceso(e, Proce) {
                e.preventDefault();

                var Razon = $('#Razon').val();
                var CorreoV = $('#CorreoT').val();
                if (document.getElementById('defaultCheck1').checked) {
                    Upd = '1';
                }
                else {
                    Upd = '0';
                }


                var url_list = NotificacionesWebService.get_path() + Proce;

                $.ajax({
                    type: "POST",
                    "beforeSend": function (xhr) {
                        xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
                    },
                    url: url_list,
                    dataType: "json",
                    data: {
                        Razon: Razon,
                        CorreoV2: CorreoV,
                        Estado: Upd
                    },
                    success: function (respuesta) {
                        $('#myModal').modal('hide');
                        if (respuesta.success) {
                            $('#myModal').modal('hide');
                            swal('Notificaciones T|SYS|', respuesta.msg, 'success');
                        }
                        else {
                            $('#myModal').modal('hide');
                            swal('Notificaciones T|SYS|', respuesta.msg, 'error');
                        }

                    },
                    error: function (respuesta) {
                        $('#myModal').modal('hide');
                        swal('Notificaciones T|SYS|', Upd, 'error');
                        //swal('Notificaciones T|SYS|', 'La actualización de datos no ha podido ser procesada', 'error');
                    }
                });

                return false;
            }
        });
    </script>

    <style>
        .data0 {
            font-style: normal;
            background-color: #0094ff;
        }

        .data1 {
            font-style: normal;
            background-color: #ff0000;
        }
    </style>

    <div class="col-lg-12 col-sm-12 col-12" id="M1">
        <br />
        <h3>Administración de Usuarios</h3>
    </div>

    <!-- Nav pills -->
    <ul class="nav nav-pills">
        <li class="nav-item active">
            <a class="nav nav-pills nav-justified" data-toggle="pill" href="#home">Proveedores</a>
        </li>
        <li class="nav-item">
            <a class="nav nav-pills nav-justified" data-toggle="pill" href="#home1">Internos</a>
        </li>
        <li class="nav-item">
            <a class="nav nav-pills nav-justified" data-toggle="pill" href="#menu1">Actualizar Correo</a>
        </li>
        <li class="nav-item">
            <a class="nav nav-pills nav-justified" data-toggle="pill" href="#menu2">Vencimiento del Token</a>
        </li>
    </ul>

    <!-- Tab panes -->
    <div class="tab-content">
        <div class="tab-pane container active" id="home">

            <br />
            <h4>Criterios de Búsqueda</h4>
            <br />
            <asp:UpdatePanel runat="server" ID="UpdatePanel" UpdateMode="Conditional">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="LinkButton1" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="LinkButton2" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="LinkButton3" EventName="Click" />
                </Triggers>
                <ContentTemplate>

                    <div class="col-sx-12 col-md-4">
                        <label>Correo Electronico</label>
                        <asp:TextBox runat="server" type="text" AutoComplete="off" AutoCompleteType="Disabled" ID="Email" MaxLength="90" class="form-control" placeholder="ejemplo@tsys.com" TabIndex="3" />
                    </div>

                    <div class="col-sx-12 col-md-4">
                        <label>Nombre / Razón Social</label>
                        <asp:TextBox runat="server" type="text" AutoComplete="off" AutoCompleteType="Disabled" ID="Nombre" MaxLength="120" class="form-control" placeholder="Nombre / Razón social" TabIndex="3" />
                    </div>

                    <div class="col-sx-12 col-md-3">
                        <div class="col-xs-10  col-md-8">
                            <label>Status</label>
                            <asp:DropDownList runat="server" ID="Cclientes" class="selectpicker show-tick form-control" AutoPostBack="true" data-live-search="true" data-style="btn-primary" OnSelectedIndexChanged="Cclientes_SelectedIndexChanged">
                                <asp:ListItem Value="1">Activo</asp:ListItem>
                                <asp:ListItem Value="2">Inactivo</asp:ListItem>
                                <asp:ListItem Value="3">Pendiente</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="col-xs-12 md-3 col-lg-2">
                        <br />
                        <label>Desde</label>
                        <asp:TextBox type="date" name="fecha" AutoComplete="off" AutoCompleteType="Disabled" ID="F1" min="1980-01-01" max="2050-12-31" step="1" class="form-control" runat="server" />
                    </div>

                    <div class="col-xs-12 md-3 col-lg-2">
                        <br />
                        <label>Hasta</label>
                        <asp:TextBox type="date" name="fecha" AutoComplete="off" AutoCompleteType="Disabled" ID="F2" min="1980-01-01" max="2050-12-31" step="1" class="form-control" runat="server" />
                    </div>

                    <div class="col-sx-12 col-md-2">
                        <br />
                        <div class="col-xs-10  col-md-12">
                            <label>Rol Asignado</label>
                            <asp:DropDownList runat="server" ID="DRol" class="selectpicker show-tick form-control" AutoPostBack="true" data-live-search="true" data-style="btn-primary" OnSelectedIndexChanged="Cclientes_SelectedIndexChanged">
                                <asp:ListItem Value="1">Seleccione Rol</asp:ListItem>
                                <asp:ListItem Value="2">Proveedor</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="col-sx-12 col-md-2">
                        <br />
                        <div class="col-xs-10  col-md-10">
                            <label>Tipo Prov</label>
                            <asp:DropDownList runat="server" ID="Tipo" class="selectpicker show-tick form-control" AutoPostBack="true" data-live-search="true" data-style="btn-primary" OnSelectedIndexChanged="Cclientes_SelectedIndexChanged">
                                <asp:ListItem Value="X">Seleccione Tipo</asp:ListItem>
                                <asp:ListItem Value="Con Adenda">Con Adenda</asp:ListItem>
                                <asp:ListItem Value="Sin Adenda">Sin Adenda</asp:ListItem>
                                <asp:ListItem Value="Extranjero">Extranjero</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>


                    <div class="col-xs-2 col-md-3">
                        <br />
                        <span aria-hidden="true">
                            <br />
                        </span>
                        <asp:LinkButton runat="server" type="button" Class="btn btn-tsys" OnClick="Buscar" ID="LinkButton1">                                        
                <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
                        </asp:LinkButton>

                        <asp:LinkButton runat="server" type="button" class="btn btn-primary" OnClick="Clean" ID="LinkButton3">                                        
        <span aria-hidden="true" class="glyphicon glyphicon-erase"></span>
                        </asp:LinkButton>
                        <br />
                        <br />
                        <br />
                        <br />
                    </div>

                    <div class="col-xs-10 col-sm-9">
                        <div class="alert alert-block alert-success" id="AL" style="display: none">
                            <h5>¡Exito!</h5>
                            <asp:Label ID="Label4" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                    <div class="col-xs-10 col-sm-9">
                        <div class="alert alert-block alert-danger" id="AL1" style="display: none">
                            <h5>¡Error!</h5>
                            <asp:Label ID="Labe5" runat="server" Text=""></asp:Label>
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
                        <br />

                        <br />
                        <asp:GridView ID="GridView1" CssClass="table table-bordered bs-table"
                            margin-left="auto" margin-right="auto" runat="server"
                            AutoGenerateColumns="False"
                            OnPageIndexChanging="GridView2_PageIndexChanging"
                            OnRowEditing="GridView2_RowEditing"
                            OnRowCancelingEdit="GridView2_RowCancelingEdit"
                            OnRowCreated="GridView2_RowCreated"
                            OnRowUpdating="GridView2_RowUpdating"
                            OnRowDataBound="GridView2_RowDataBound"
                            OnRowCommand="GridView2_RowCommand"
                            Width="99%">

                            <AlternatingRowStyle BackColor="White" />

                            <HeaderStyle BackColor="#337ab7" Font-Bold="True" ForeColor="White" />
                            <EditRowStyle BackColor="#ffffcc" />
                            <EmptyDataRowStyle ForeColor="Red" CssClass="table table-bordered" />

                            <Columns>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="80px">
                                    <ItemTemplate>
                                        <asp:Button ID="btnEdit" runat="server" Text="Resset" CssClass="btn btn-warning" CommandName="Edit" />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:Button ID="btnUpdate" runat="server" Text="Enviar" CssClass="btn btn-tsys" CommandName="Update" />
                                        <asp:Button ID="btnCancel" runat="server" Text="Cancelar" CssClass="btn btn-success" CommandName="Cancel" />
                                    </EditItemTemplate>
                                    <HeaderStyle Width="80px"></HeaderStyle>

                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                </asp:TemplateField>

                                <asp:BoundField DataField="Nombre" HeaderText="Nombre" HeaderStyle-Width="70px" ReadOnly="True">
                                    <HeaderStyle Width="50px"></HeaderStyle>
                                </asp:BoundField>

                                <asp:BoundField DataField="Correo" HeaderText="Correo" HeaderStyle-Width="75px" ReadOnly="True">
                                    <HeaderStyle Width="75px"></HeaderStyle>
                                </asp:BoundField>
                                <%--            <asp:BoundField DataField="Fecha" HeaderText="Company" HeaderStyle-Width="70px" ReadOnly="True">
                <HeaderStyle Width="70px"></HeaderStyle>
            </asp:BoundField>--%>
                                <asp:BoundField DataField="Creador" HeaderText="Rol" HeaderStyle-Width="70px" ReadOnly="True">
                                    <HeaderStyle Width="70px"></HeaderStyle>
                                </asp:BoundField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="90px" HeaderText="Status" ValidateRequestMode="Enabled">
                                    <ItemTemplate>
                                        <asp:DropDownList runat="server" class="selectpicker show-tick form-control" ID="Stat">
                                            <asp:ListItem Value="1">Activo</asp:ListItem>
                                            <asp:ListItem Value="2">Inactivo</asp:ListItem>
                                            <asp:ListItem Value="3">Pendiente</asp:ListItem>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                    <HeaderStyle Width="90px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="90px" HeaderText="Tipo" ValidateRequestMode="Enabled">
                                    <ItemTemplate>
                                        <asp:DropDownList runat="server" class="selectpicker show-tick form-control" ID="Tipo">
                                            <asp:ListItem Value="Con Adenda">Con Adenda</asp:ListItem>
                                            <asp:ListItem Value="Sin Adenda">Sin Adenda</asp:ListItem>
                                            <asp:ListItem Value="Extranjero">Extranjero</asp:ListItem>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                    <HeaderStyle Width="90px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>

                                <asp:BoundField DataField="Correo" HeaderText="Enviar A" HeaderStyle-Width="70px" ReadOnly="False">
                                    <HeaderStyle Width="70px"></HeaderStyle>
                                </asp:BoundField>

                                <asp:TemplateField HeaderStyle-Width="70px" ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:Button ID="Editar" CssClass="btn btn-danger" runat="server" CommandName="Editar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="Editar" />
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

                    <div class="col-xs-12 col-md-12">
                        <span class="col-xs-11 col-md-11"></span>
                        <div class="col-xs-11 col-md-1">
                            <asp:Button runat="server" type="button" CssClass="btn btn-primary" OnClick="Save" Text="Guardar" ID="LinkButton2"></asp:Button>
                            <br />
                        </div>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>

        </div>

        <!-- Tab Internos -->
        <div class="tab-pane container fade" id="home1">

            <br />
            <h4>Criterios de Búsqueda</h4>
            <br />
            <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="LinkButton4" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="LinkButton5" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="Button4" EventName="Click" />
                </Triggers>
                <ContentTemplate>

                    <div class="col-sx-12 col-md-4">
                        <label>Correo Electronico</label>
                        <asp:TextBox runat="server" type="text" AutoComplete="off" AutoCompleteType="Disabled" ID="Email1" MaxLength="90" class="form-control" placeholder="ejemplo@tsys.com" TabIndex="3" />
                    </div>

                    <div class="col-sx-12 col-md-4">
                        <label>Nombre / Razón Social</label>
                        <asp:TextBox runat="server" type="text" AutoComplete="off" AutoCompleteType="Disabled" ID="Nombre1" MaxLength="120" class="form-control" placeholder="Nombre / Razón social" TabIndex="3" />
                    </div>

                    <div class="col-sx-12 col-md-3">
                        <div class="col-xs-10  col-md-8">
                            <label>Status</label>
                            <asp:DropDownList runat="server" ID="Cclientes1" class="selectpicker show-tick form-control" AutoPostBack="true" data-live-search="true" data-style="btn-primary" OnSelectedIndexChanged="Cclientes_SelectedIndexChanged1">
                                <asp:ListItem Value="1">Activo</asp:ListItem>
                                <asp:ListItem Value="2">Inactivo</asp:ListItem>
                                <asp:ListItem Value="3">Pendiente</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="col-xs-12 md-3 col-lg-2">
                        <br />
                        <label>Desde</label>
                        <asp:TextBox type="date" name="fecha" AutoComplete="off" AutoCompleteType="Disabled" ID="F11" min="1980-01-01" max="2050-12-31" step="1" class="form-control" runat="server" />
                    </div>

                    <div class="col-xs-12 md-3 col-lg-2">
                        <br />
                        <label>Hasta</label>
                        <asp:TextBox type="date" name="fecha" AutoComplete="off" AutoCompleteType="Disabled" ID="F21" min="1980-01-01" max="2050-12-31" step="1" class="form-control" runat="server" />
                    </div>

                    <div class="col-sx-12 col-md-2">
                        <br />
                        <div class="col-xs-10  col-md-12">
                            <label>Rol Asignado</label>
                            <asp:DropDownList runat="server" ID="DRol1" class="selectpicker show-tick form-control" AutoPostBack="true" data-live-search="true" data-style="btn-primary" OnSelectedIndexChanged="Cclientes_SelectedIndexChanged1">
                                <asp:ListItem Value="1">Seleccione Rol</asp:ListItem>
                                <asp:ListItem Value="3">T|SYS| - Admin</asp:ListItem>
                                <asp:ListItem Value="4">T|SYS| - Validador</asp:ListItem>
                                <asp:ListItem Value="5">T|SYS| - Consultas</asp:ListItem>
                                <asp:ListItem Value="6">T|SYS| - Empleado</asp:ListItem>
                                <asp:ListItem Value="7">T|SYS| - Gerente</asp:ListItem>
                                <asp:ListItem Value="8">T|SYS| - Tesoreria</asp:ListItem>
                                <asp:ListItem Value="9">T|SYS| - Finanzas</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="col-xs-2 col-md-3">
                        <br />
                        <span aria-hidden="true">
                            <br />
                        </span>
                        <asp:LinkButton runat="server" type="button" Class="btn btn-tsys" OnClick="Buscar1" ID="LinkButton4">                                        
                <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
                        </asp:LinkButton>

                        <asp:LinkButton runat="server" type="button" class="btn btn-primary" OnClick="Clean1" ID="LinkButton5">                                        
        <span aria-hidden="true" class="glyphicon glyphicon-erase"></span>
                        </asp:LinkButton>
                        <br />
                        <br />
                        <br />
                        <br />
                    </div>

                    <div class="col-xs-10 col-sm-9">
                        <div class="alert alert-block alert-success" id="AL11" style="display: none">
                            <h5>¡Exito!</h5>
                            <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                    <div class="col-xs-10 col-sm-9">
                        <div class="alert alert-block alert-danger" id="AL111" style="display: none">
                            <h5>¡Error!</h5>
                            <asp:Label ID="Label3" runat="server" Text=""></asp:Label>
                        </div>
                    </div>

                    <asp:Panel runat="server" ID="DatosV1" Width="90%" Height=" 300px">
                        <div class="col-xs-12">
                            <font size="5" color="#A4A4A4">No Se Encontraron Resultados</font>
                        </div>
                        <div class="col-xs-12" id="Timg">
                            <center>
                                <asp:Image runat="server" ID="Image1" Width="50%" Height="50%" ImageUrl="../../Img/TGris.png" />
                            </center>
                        </div>
                    </asp:Panel>

                    <div class="row">
                        <br />

                        <br />
                        <asp:GridView ID="GridView5" CssClass="table table-bordered bs-table"
                            margin-left="auto" margin-right="auto" runat="server"
                            AutoGenerateColumns="False"
                            OnPageIndexChanging="GridView2_PageIndexChanging"
                            OnRowEditing="GridView5_RowEditing"
                            OnRowCancelingEdit="GridView5_RowCancelingEdit"
                            OnRowCreated="GridView2_RowCreated"
                            OnRowUpdating="GridView5_RowUpdating"
                            OnRowDataBound="GridView2_RowDataBound"
                            OnRowCommand="GridView5_RowCommand"
                            Width="99%">

                            <AlternatingRowStyle BackColor="White" />

                            <HeaderStyle BackColor="#337ab7" Font-Bold="True" ForeColor="White" />
                            <EditRowStyle BackColor="#ffffcc" />
                            <EmptyDataRowStyle ForeColor="Red" CssClass="table table-bordered" />

                            <Columns>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="80px">
                                    <ItemTemplate>
                                        <asp:Button ID="btnEdit" runat="server" Text="Resset" CssClass="btn btn-warning" CommandName="Edit" />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:Button ID="btnUpdate" runat="server" Text="Enviar" CssClass="btn btn-tsys" CommandName="Update" />
                                        <asp:Button ID="btnCancel" runat="server" Text="Cancelar" CssClass="btn btn-success" CommandName="Cancel" />
                                    </EditItemTemplate>
                                    <HeaderStyle Width="80px"></HeaderStyle>

                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                </asp:TemplateField>

                                <asp:BoundField DataField="Nombre" HeaderText="Nombre" HeaderStyle-Width="70px" ReadOnly="True">
                                    <HeaderStyle Width="50px"></HeaderStyle>
                                </asp:BoundField>

                                <asp:BoundField DataField="Correo" HeaderText="Correo" HeaderStyle-Width="75px" ReadOnly="True">
                                    <HeaderStyle Width="75px"></HeaderStyle>
                                </asp:BoundField>

                                <asp:BoundField DataField="Creador" HeaderText="Rol" HeaderStyle-Width="70px" ReadOnly="True">
                                    <HeaderStyle Width="70px"></HeaderStyle>
                                </asp:BoundField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="90px" HeaderText="Status" ValidateRequestMode="Enabled">
                                    <ItemTemplate>
                                        <asp:DropDownList runat="server" class="selectpicker show-tick form-control" ID="Stat1">
                                            <asp:ListItem Value="1">Activo</asp:ListItem>
                                            <asp:ListItem Value="2">Inactivo</asp:ListItem>
                                            <asp:ListItem Value="3">Pendiente</asp:ListItem>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                    <HeaderStyle Width="90px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>

                                <%--         <asp:TemplateField  ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="90px"  HeaderText="Tipo" ValidateRequestMode="Enabled">
          <ItemTemplate>
               <asp:DropDownList runat="server" class="selectpicker show-tick form-control" ID="Tipo1" >
                <asp:ListItem Value="Con Adenda">Con Adenda</asp:ListItem>
                <asp:ListItem Value="Sin Adenda">Sin Adenda</asp:ListItem>
                <asp:ListItem Value="Extranjero">Extranjero</asp:ListItem>
               </asp:DropDownList>
          </ItemTemplate> 
             <HeaderStyle Width="90px" />
             <ItemStyle HorizontalAlign="Center" />
         </asp:TemplateField>--%>

                                <asp:BoundField DataField="Correo" HeaderText="Enviar A" HeaderStyle-Width="70px" ReadOnly="False">
                                    <HeaderStyle Width="70px"></HeaderStyle>
                                </asp:BoundField>

                                <asp:TemplateField HeaderStyle-Width="70px" ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:Button ID="Editar" CssClass="btn btn-danger" runat="server" CommandName="Editar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="Editar" />
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

                    <div class="col-xs-12 col-md-12">
                        <span class="col-xs-11 col-md-11"></span>
                        <div class="col-xs-11 col-md-1">
                            <asp:Button runat="server" type="button" CssClass="btn btn-primary" OnClick="Save1" Text="Guardar" ID="Button4"></asp:Button>
                            <br />
                        </div>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>

        </div>

        <!-- Registro Ususarios Tsys -->
        <div class="tab-pane container fade" id="menu1">
            <br />
            <h4>Actualización de Datos</h4>

            <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="BtnBusc" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="Button1" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="Button2" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="Button3" EventName="Click" />
                </Triggers>
                <ContentTemplate>


                    <div class="row" id="Cam">
                        <div class="col-xs-6 col-sm-6 col-md-6">
                            <div class="form-group row">
                                <label class="col-form-label col-sm-4">Correo Electronico:</label>
                                <div class="col-sm-8">
                                    <asp:TextBox runat="server" type="text" AutoComplete="off" AutoCompleteType="Disabled" MaxLength="80" ID="EmailUp" class="form-control" TabIndex="1" />
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

                    <div class="form-group row" id="Cam2">
                        <label class="col-form-label col-sm-2">Nuevo Email:</label>
                        <div class="col-sm-4">
                            <asp:TextBox runat="server" type="text" AutoComplete="off" AutoCompleteType="Disabled" name="Nvo" ID="Nvoemail" MaxLength="100" class="form-control" placeholder="" TabIndex="3" ReadOnly="false" />
                        </div>
                    </div>

                    <div class="form-group row" id="Cam1" runat="server">
                        <label class="col-form-label col-sm-11">Se actualizarán los siguientes registros:</label>
                        <div class="col-sm-8">
                            <asp:GridView ID="GridView4" CssClass="table table-bordered bs-table"
                                margin-left="auto" margin-right="auto" runat="server"
                                AutoGenerateColumns="False"
                                Width="80%">

                                <AlternatingRowStyle BackColor="White" />
                                <HeaderStyle BackColor="#337ab7" Font-Bold="True" ForeColor="White" />
                                <EditRowStyle BackColor="#ffffcc" />
                                <EmptyDataRowStyle ForeColor="Red" CssClass="table table-bordered" />

                                <Columns>
                                    <asp:BoundField DataField="Nombre" HeaderText="Nombre" HeaderStyle-Width="50%" ReadOnly="True">
                                        <HeaderStyle Width="50%"></HeaderStyle>
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Compania" HeaderText="Compañia" HeaderStyle-Width="50%" ReadOnly="True">
                                        <HeaderStyle Width="50%"></HeaderStyle>
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
                        </div>
                    </div>

                    <div id="Acc" class="col-xs-12 col-md-12">
                        <div class="col-xs-3 col-md-1">
                            <asp:Button Text="Agregar" runat="server" CssClass="btn btn-tsys" title="Agregar Usuario a la Lista" ID="Button1" OnClick="Campos" />
                        </div>
                        <div class="col-xs-3 col-md-1">
                            <asp:Button Text="Limpiar" runat="server" CssClass="btn btn-success" title="Limpiar Campos" ID="Button2" OnClick="Limp" />
                        </div>

                    </div>

                    <div id="Acc1" class="col-xs-8 col-md-8">
                        <br />
                        <div class="alert alert-block alert-danger" id="AID2" style="display: none">
                            <h5>¡Error!</h5>
                            <asp:Label runat="server" ID="Lsage"></asp:Label>
                        </div>
                        <div class="alert alert-block alert-success" id="AID3" style="display: none">
                            <h5>¡Exito!</h5>
                            <asp:Label runat="server" ID="Label1"></asp:Label>
                        </div>
                    </div>

                    <div class="row">
                        <asp:GridView ID="GridView2" CssClass="table table-bordered bs-table"
                            margin-left="auto" margin-right="auto" runat="server"
                            OnRowCancelingEdit="GridView1_RowCancelingEdit"
                            AutoGenerateColumns="False"
                            Width="99%">

                            <AlternatingRowStyle BackColor="White" />

                            <HeaderStyle BackColor="#337ab7" Font-Bold="True" ForeColor="White" />
                            <EditRowStyle BackColor="#ffffcc" />
                            <EmptyDataRowStyle ForeColor="Red" CssClass="table table-bordered" />

                            <Columns>
                                <%--botones de acción sobre los registros...--%>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="80px">
                                    <ItemTemplate>
                                        <%--Botones de eliminar y editar cliente...--%>
                                        <asp:Button ID="btnCancel" runat="server" Text="Eliminar" CssClass="btn btn-danger" CommandName="Cancel" OnClientClick="return confirm('¿Seguro que quiere Eliminar los datos del cliente?');" />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                    </EditItemTemplate>

                                    <HeaderStyle Width="80px"></HeaderStyle>

                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Razon Social" HeaderText="Razon Social" HeaderStyle-Width="140px" ReadOnly="True">
                                    <HeaderStyle Width="75px"></HeaderStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="Email" HeaderText="Email" HeaderStyle-Width="70px" ReadOnly="True">
                                    <HeaderStyle Width="140px"></HeaderStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="NvoEmail" HeaderText="Nuevo Email" HeaderStyle-Width="100px" ReadOnly="True">
                                    <HeaderStyle Width="78px"></HeaderStyle>
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
                    </div>

                    <div id="Acc2" class="col-xs-12 col-md-12">
                        <label class="col-xs-11 col-md-11"></label>
                        <div class="col-xs-3 col-md-1">
                            <asp:Button Text="Guardar" runat="server" CssClass="btn btn-warning" title="Actualizar Usuarios" ID="Button3" OnClick="Guardar" />
                        </div>
                    </div>

                    <div class="row">
                        <asp:GridView ID="GridView3" runat="server"
                            CssClass="table table-bordered bs-table"
                            AutoGenerateColumns="False"
                            BackColor="White" BorderColor="#E7E7FF"
                            BorderStyle="None" BorderWidth="1px"
                            CellPadding="3" GridLines="Horizontal"
                            Height="34px" Width="90%">

                            <AlternatingRowStyle BackColor="#F7F7F7" />
                            <Columns>
                                <asp:BoundField HeaderText="Nombre" SortExpression="UUID" DataField="Nombre" />
                                <asp:BoundField HeaderText="Email" SortExpression="Folio" DataField="Email" />
                                <asp:BoundField HeaderText="Nuevo Email" SortExpression="Moneda" DataField="Nuevo" />
                                <asp:BoundField HeaderText="Error" SortExpression="Metodo" DataField="Error" />
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

                </ContentTemplate>
            </asp:UpdatePanel>

        </div>

        <!-- Establecer Fecha de vencimiento del token -->
        <div class="tab-pane container fade" id="menu2">
            <br />
            <h4>Establecer la expiración del Token</h4>
            <br />
            <asp:UpdatePanel runat="server" ID="UpdatePanel3" UpdateMode="Conditional">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="LinkButton1" EventName="Click" />
                </Triggers>
                <ContentTemplate>
                    <div class="row">
                        <div class="col-xs-6 col-sm-6 col-md-6">
                            <div class="form-group row">
                                <label class="col-form-label col-sm-3">Vencimiento:</label>
                                <div class="col-sm-9">
                                    <asp:Label ID="lblPeriodo" runat="server" Text='<%# Eval("PeriodoKey") %>' Visible="false" />
                                    <asp:DropDownList runat="server" class="selectpicker show-tick form-control" ID="Periodo">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xs-6 col-sm-6 col-md-6">
                            <div class="form-group row">
                                <label class="col-form-label col-sm-10"></label>
                                <div class="btn-toolbar" role="toolbar" aria-label="Toolbar with button groups">
                                    <div class="btn-group mr-2" role="group" aria-label="First group">
                                        <asp:Button Text="Guardar" runat="server" ID="BnEnv" autoposback="True" CssClass="btn btn-primary" title="Guardar Datos" OnClick="btn_Guardar"/>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span
                            aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title"
                        id="myModalLabel">Actualización de Datos</h4>
                </div>
                <div class="modal-body">

                    <div class="container-fluid">
                        <div class="row">
                            <div class="form-group">
                                Razón Social:
                                        <input id="Razon" maxlength="200" class="form-control" autocomplete="off" />
                            </div>
                            <div class="form-group">
                                Correo Electronico:
                                        <input id="CorreoT" maxlength="200" class="form-control" autocomplete="off" />
                            </div>
                            <div class="form-check">
                                <input class="form-check-input" type="checkbox" value="" name="defaultCheck1" id="defaultCheck1">
                                <label class="form-check-label" for="defaultCheck1">
                                    Restaurar Contraseña
                                </label>
                            </div>

                            <div class="col-sm-3">
                                <select class="form-control" id="Menu">
                                    <option value="Activo">Activo</option>
                                    <option value="Inactivo">Inactivo</option>
                                    <option value="Pendiente">Pendiente</option>
                                </select>
                            </div>


                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <button id="btn_No_Send" type="button" class="btn btn-danger pull-left" data-dismiss="modal">
                        Cancelar
                    </button>


                    <button id="btn_send_comments" class="btn btn-primary" title="Guardar Cambios" data-toggle="tooltip">
                        Guardar
                    </button>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
