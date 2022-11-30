<%@ Page Language="C#" Title="Asignación de Validadores" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeFile="AsignacionValidador.aspx.cs" Inherits="Logged_Administradores_AsignacionValidador" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    Version 08-Abril-2019 By Luis Angel Garcia P
    <meta http-equiv="Cache-Control" content="no-cache">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Pragma" content="no-cache" />
    <link href="../Css/SLoging.css" rel="stylesheet" type="text/css" />

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
        <h3>Asignación de Validadores</h3>
    </div>

    <!-- Nav pills -->
    <%--<ul class="nav nav-pills">
        <li class="nav-item active">
            <a class="nav nav-pills nav-justified" data-toggle="pill" href="#home">Proveedores</a>
        </li>
        <li class="nav-item">
            <a class="nav nav-pills nav-justified" data-toggle="pill" href="#home1">Internos</a>
        </li>
        <li class="nav-item">
            <a class="nav nav-pills nav-justified" data-toggle="pill" href="#menu1">Actualizar Correo</a>
        </li>
    </ul>--%>

    <!-- Tab panes -->
    <div class="tab-content">
        <div class="tab-pane container active" id="home">

            <br />
            <h4>Criterios de Búsqueda</h4>
            <asp:UpdatePanel runat="server" ID="UpdatePanel" UpdateMode="Conditional">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="LinkButton1" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="LinkButton2" EventName="Click" />
                    <%--<asp:AsyncPostBackTrigger ControlID="LinkButton3" EventName="Click" />--%>
                </Triggers>
                <ContentTemplate>

                    <div class="row" id="Cam">
                        <div class="col-xs-6 col-sm-6 col-md-6">
                            <div class="form-group row">
                                <label class="col-form-label col-sm-3">Usuario:</label>
                                <div class="col-sm-9">
                                    <asp:TextBox runat="server" type="text" AutoComplete="off" AutoCompleteType="Disabled" ID="Email" MaxLength="90" class="form-control" placeholder="ejemplo@tsys.com" TabIndex="3" />
                                </div>
                            </div>
                        </div>
                        <%--<a href="Administracion_Doc.aspx">Administracion_Doc.aspx</a>--%>
                        <div class="btn-toolbar" role="toolbar">
                            <div class="col-xs-1 btn-group">
                                <asp:LinkButton runat="server" type="button" Class="btn btn-tsys" ID="LinkButton1" OnClick="Buscar">                                        
                                <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>

                    <%--<div class="col-sx-12 col-md-4">
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

                    <div class="col-sx-12 col-md-2"><a href="../../Bin/">../../Bin/</a>
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
                    </div>--%>

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
                        <%--<div class="col-xs-12">
                            <font size="5" color="#A4A4A4">No Se Encontraron Resultados</font>
                        </div>--%>
                        <div class="col-xs-12" id="Timg">
                            <center>
                                <asp:Image runat="server" ID="PckT" Width="50%" Height="50%" ImageUrl="../../Img/TGris.png" />
                            </center>
                        </div>
                    </asp:Panel>

                    <div class="row">
                        <br />

                        <br />
                        <asp:GridView ID="gvValidadores" CssClass="table table-bordered bs-table"
                            margin-left="auto" margin-right="auto" runat="server"
                            AutoGenerateColumns="False"
                            OnRowDataBound="ValidadoresGridView_OnRowDataBound"
                            OnRowCommand="ValidadoresGridView_RowCommand"
                            Width="99%">

                            <AlternatingRowStyle BackColor="White" />

                            <HeaderStyle BackColor="#337ab7" Font-Bold="True" ForeColor="White" />
                            <EditRowStyle BackColor="#ffffcc" />
                            <EmptyDataRowStyle ForeColor="Red" CssClass="table table-bordered" />

                            <Columns>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="80px">
                                    <ItemTemplate>
                                        <asp:Button ID="btnCancel" CssClass="btn btn-danger" runat="server" CommandName="Eliminar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="Eliminar" />
                                        <%--<asp:Button ID="Editar" CssClass="btn btn-danger" runat="server" CommandName="Editar"  Text="Editar" />--%>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <%--<asp:Button ID="btnUpdate" runat="server" Text="Enviar" CssClass="btn btn-tsys" CommandName="Update" />
                                        <asp:Button ID="btnCancel" runat="server" Text="Cancelar" CssClass="btn btn-success" CommandName="Cancel" />--%>
                                    </EditItemTemplate>
                                    <HeaderStyle Width="80px"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                </asp:TemplateField>

                                <asp:BoundField DataField="UserKey" HeaderText="Id" HeaderStyle-Width="75px" ReadOnly="True">
                                    <HeaderStyle Width="75px"></HeaderStyle>
                                </asp:BoundField>

                                <asp:BoundField DataField="UserName" HeaderText="Usuario" HeaderStyle-Width="70px" ReadOnly="True">
                                    <HeaderStyle Width="50px"></HeaderStyle>
                                </asp:BoundField>

                                <asp:BoundField DataField="CorreoElec" HeaderText="Correo" HeaderStyle-Width="70px" ReadOnly="True">
                                    <HeaderStyle Width="70px"></HeaderStyle>
                                </asp:BoundField>

                                <asp:BoundField DataField="Role" HeaderText="Rol" HeaderStyle-Width="70px" ReadOnly="True">
                                    <HeaderStyle Width="70px"></HeaderStyle>
                                </asp:BoundField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="90px" HeaderText="Validador de CX" ValidateRequestMode="Enabled">
                                    <ItemTemplate>
                                        <asp:Label ID="lblValidadorCX" runat="server" Text='<%# Eval("UserValidadorCX") %>' Visible="false" />
                                        <asp:DropDownList runat="server" class="selectpicker show-tick form-control" ID="ValidadorCX">
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                    <HeaderStyle Width="90px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="90px" HeaderText="Gerente" ValidateRequestMode="Enabled">
                                    <ItemTemplate>
                                        <asp:Label ID="lblGerente" runat="server" Text='<%# Eval("UserGerente") %>' Visible="false" />
                                        <asp:DropDownList runat="server" class="selectpicker show-tick form-control" ID="Gerentes">
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                    <HeaderStyle Width="90px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="90px" HeaderText="Recursos Humanos" ValidateRequestMode="Enabled">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRH" runat="server" Text='<%# Eval("UserRH") %>' Visible="false" />
                                        <asp:DropDownList runat="server" class="selectpicker show-tick form-control" ID="RHs">
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                    <HeaderStyle Width="90px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="90px" HeaderText="Tesoreria" ValidateRequestMode="Enabled">
                                    <ItemTemplate>
                                        <asp:Label ID="lblTesoreria" runat="server" Text='<%# Eval("UserTesoreria") %>' Visible="false" />
                                        <asp:DropDownList runat="server" class="selectpicker show-tick form-control" ID="Tesorerias">
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                    <HeaderStyle Width="90px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>

                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="90px" HeaderText="Finanzas" ValidateRequestMode="Enabled">
                                    <ItemTemplate>
                                        <asp:Label ID="lblFinanzas" runat="server" Text='<%# Eval("UserFinanzas") %>' Visible="false" />
                                        <asp:DropDownList runat="server" class="selectpicker show-tick form-control" ID="Finanzass">
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                    <HeaderStyle Width="90px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>

                                <%--<asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="90px" HeaderText="Tipo" ValidateRequestMode="Enabled">
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
                                        <asp:Button ID="Eliminar" CssClass="btn btn-danger" runat="server" CommandName="Eliminar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="Eliminar" />
                                    </ItemTemplate>
                                </asp:TemplateField>--%>
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
                            <asp:Button runat="server" type="button" CssClass="btn btn-primary" Text="Guardar" OnClick="guardar" ID="LinkButton2"></asp:Button>
                            <br />
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
