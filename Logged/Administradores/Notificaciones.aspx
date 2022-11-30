<%@ Page Language="C#" Title="Administrador de Notificaciones del Sistemas" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeFile="Notificaciones.aspx.cs" Inherits="Logged_Administradores_Notificaciones" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    Version 08-Abril-2019 By Luis Angel Garcia P
    <meta http-equiv="Cache-Control" content="no-cache">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Pragma" content="no-cache" />

    <script src="../../Css/sweetalert.min.js" type="text/javascript"></script>
    <script src="../../Css/sweetalert2.all.min.js" type="text/javascript"></script>
    <script src="../../Scripts/custom.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery.blockui.min.js" type="text/javascript"></script>


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
        });
    </script>

    <script>
        function Varis(Texto) {
            Texto
        }
    </script>

    <script>

        function Pregunta(Userkey) {

            swal.fire({
                title: "¿Deseas Programar la Notificación?",
                text: "Una vez programada, esta aparecerá en la sección inicio de la pantalla de los proveedores durante el periodo indicado, podrás desactivarla desde la sección 'Todas mis Notificaciones'",
                icon: "warning",
                buttons: ["No", "Si"],
                confirmButtonText: 'Confirmar',
                cancelButtonText: 'Cancelar',
                confirmButtonColor: "#FF6600",
                customClass: 'swal-wide',
                closeModal: true,
                closeOnConfirm: false,
                showCancelButton: true,
                dangerMode: true,
            }).then((result) => {
                if (result.isConfirmed) {
                    Preguntas(Userkey)
                }
            })
            return true;
        }

        async function Preguntas(Userkey) {

            const { value: password } = await Swal.fire({
                title: 'Ingresa tu contraseña para confirmar',
                input: 'password',
                inputLabel: 'Password',
                inputPlaceholder: 'Enter your password',
                customClass: 'swal-wide',
                inputAttributes: {
                    maxlength: 20,
                    autocapitalize: 'off',
                    autocorrect: 'off'
                }
            })

            if (password) {
                Proceso("/Prog_Alerta", password, Userkey)
            }

            return true;
        }

        async function Proceso(Proce, password, Userkey) {

            var Userkey = Userkey;
            var Password = password;
            var Password = password;
            var Titulo = document.getElementById('<%= Email.ClientID %>').value;
            var e = document.getElementById('<%= DRol.ClientID %>');
            var Estilo = e.options[e.selectedIndex].text;
            var Estatus = document.getElementById('<%= Cclientes.ClientID %>').value;
            var Desde = document.getElementById('<%= F1.ClientID %>').value;
            var Hasta = document.getElementById('<%= F2.ClientID %>').value;
            var Mensaje = document.getElementById('<%= Nombre.ClientID %>').innerHTML;
            var Url = document.getElementById('<%= Imagen.ClientID %>').value;
            var url_list = NotificacionesWebService.get_path() + Proce;

            $.ajax({
                type: "POST",
                "beforeSend": function (xhr) {
                    xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
                },
                url: url_list,
                dataType: "json",
                data: {
                    Userkey: Userkey,
                    Password: Password,
                    Titulo: Titulo,
                    Estilo: Estilo,
                    Estatus: Estatus,
                    Desde: Desde,
                    Hasta: Hasta,
                    Mensaje: Mensaje,
                    Url: Url
                },
                success: function (respuesta) {
                    if (respuesta.success) {
                        swal.fire({ customClass: 'swal-wide', title: 'Notificaciones T|SYS|', text: respuesta.msg, icon: 'success' });
                    }
                    else {
                        swal.fire({ customClass: 'swal-wide', title: 'Notificaciones T|SYS|', text: respuesta.msg, icon: 'error' });
                    }

                },
                error: function (respuesta) {
                    swal.fire({ customClass: 'swal-wide', title: 'Notificaciones T|SYS|', text: 'Ocurrio un error al intentar actualizar la notificación', icon: 'error' });
                }
            });

            return false;
        }

        function Pregunta0(Userkey) {

                swal.fire({
                    title: "¿Deseas Confirmar los Cambios?",
                    text: "Esta actualización permitirá o negará el acceso a los proveedores en las páginas seleccionadas",
                    icon: "warning",
                    buttons: ["No", "Si"],
                    confirmButtonText: 'Confirmar',
                    cancelButtonText: 'Cancelar',
                    confirmButtonColor: "#FF6600",
                    customClass: 'swal-wide',
                    closeModal: true,
                    closeOnConfirm: false,
                    showCancelButton: true,
                    dangerMode: true,
                }).then((result) => {
                    if (result.isConfirmed) {
                        Preguntas0(Userkey)
                    }
                })
                return true;
            }

        async function Preguntas0(Userkey) {

                const { value: password } = await Swal.fire({
                    title: 'Ingresa tu contraseña para confirmar',
                    input: 'password',
                    inputLabel: 'Password',
                    inputPlaceholder: 'Enter your password',
                    customClass: 'swal-wide',
                    inputAttributes: {
                        maxlength: 20,
                        autocapitalize: 'off',
                        autocorrect: 'off'
                    }
                })

                if (password) {
                    Proceso0("/Suspender", password, Userkey)
                }

                return true;
            }

        async function Proceso0(Proce, password, Userkey) {

                var Userkey = Userkey;
                var Password = password;
                var cv = document.getElementById('<%= Radio1.ClientID %>');
                if (document.getElementById('<%= Radio1.ClientID %>').checked == true)
                {   
                    var In1 = 1;
                    var In2 = 0;
                    var Desde = "";
                    var Hasta = "";
                }
                else
                {
                    var In1 = 0;
                    var In2 = 1;
                    var Desde = document.getElementById('<%= TextBox1.ClientID %>').value;
                    var Hasta = document.getElementById('<%= TextBox2.ClientID %>').value;
                }

                if (document.getElementById('<%= RadioButton1.ClientID %>').checked == true)
                {
                    var In3 = 1;
                    var In4 = 0;
                    var Desde1 = "";
                    var Hasta1 = "";
                }
                else
                {
                    var In3 = 0;
                    var In4 = 1;
                    var Desde1 = document.getElementById('<%= TextBox4.ClientID %>').value;
                    var Hasta1 = document.getElementById('<%= TextBox5.ClientID %>').value;
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
                        Userkey: Userkey,
                        Password: Password,
                        In1: In1,
                        In2: In2,
                        Desde: Desde,
                        Hasta: Hasta,
                        In3: In3,
                        In4: In4,
                        Desde1: Desde1,
                        Hasta1: Hasta1
                    },
                    success: function (respuesta) {
                        if (respuesta.success) {
                            swal.fire({ customClass: 'swal-wide', title: 'Notificaciones T|SYS|', text: respuesta.msg, icon: 'success' });
                        }
                        else {
                            swal.fire({ customClass: 'swal-wide', title: 'Notificaciones T|SYS|', text: respuesta.msg, icon: 'error' });
                        }

                    },
                    error: function (respuesta) {
                        swal.fire({ customClass: 'swal-wide', title: 'Notificaciones T|SYS|', text: 'Ocurrio un error al intentar actualizar la notificación', icon: 'error' });
                    }
                });

                return false;
            }
    </script>

    <script>
        function alert(campo) {
            $(campo).show("slow").delay(3000).hide("slow")
            return true;
        }
    </script>
    <script>
        function alertame(titulo, mesaje, Tipo) {
            swal.fire(
                {
                    customClass: 'swal-wide',
                    title: titulo,
                    text: mesaje,
                    icon: Tipo,
                }
            )
        }
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
        <h3>Administración de Notificaciones</h3>
    </div>

    <!-- Nav pills -->
    <ul class="nav nav-pills">
        <li class="nav-item active">
            <a class="nav nav-pills nav-justified" data-toggle="pill" href="#home">Crear Notificacion</a>
        </li>
        <li class="nav-item">
            <a class="nav nav-pills nav-justified" data-toggle="pill" href="#menu1">Todas Mis Notificaciones</a>
        </li>
        <li class="nav-item">
            <a class="nav nav-pills nav-justified" data-toggle="pill" href="#menu2">Suspension de Actividades</a>
        </li>
    </ul>

    <!-- Tab panes -->
    <div class="tab-content">
        <div class="tab-pane container active" id="home">

            <br />
            <h4>Nueva Notificación</h4>
            <br />
            <asp:UpdatePanel runat="server" ID="UpdatePanel" UpdateMode="Conditional">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="LinkButton1" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="Button4" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="LinkButton3" EventName="Click" />
                </Triggers>
                <ContentTemplate>

                    <div class="col-sx-12 col-md-4">
                        <label>Titulo</label>
                        <asp:TextBox runat="server" type="text" AutoComplete="off" AutoCompleteType="Disabled" ID="Email" MaxLength="49" class="form-control" placeholder="" TabIndex="3" />
                    </div>

                    <div class="col-sx-12 col-md-3">
                        <label>Estilo de Alerta</label>
                        <asp:DropDownList runat="server" ID="DRol" class="selectpicker show-tick form-control" AutoPostBack="false" data-live-search="true" data-style="btn-primary">
                            <asp:ListItem Value="1">info</asp:ListItem>
                            <asp:ListItem Value="2">success</asp:ListItem>
                            <asp:ListItem Value="3">warning</asp:ListItem>
                            <asp:ListItem Value="4">error</asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="col-sx-12 col-md-3">
                        <label>Status</label>
                        <asp:DropDownList runat="server" ID="Cclientes" class="selectpicker show-tick form-control" AutoPostBack="false" data-live-search="true" data-style="btn-primary">
                            <asp:ListItem Value="1">Activo</asp:ListItem>
                            <asp:ListItem Value="2">Inactivo</asp:ListItem>
                        </asp:DropDownList>
                    </div>


                    <div class="col-xs-12 col-md-12">
                        <br />
                        <span aria-hidden="true">
                            <br />
                        </span>
                    </div>

                    <div class="col-xs-12 md-3 col-lg-2">
                        <label>Desde</label>
                        <asp:TextBox type="date" name="fecha" AutoComplete="off" AutoCompleteType="Disabled" ID="F1" min="1980-01-01" max="2050-12-31" step="1" class="form-control" runat="server" />
                    </div>

                    <div class="col-xs-12 md-3 col-lg-2">
                        <label>Hasta</label>
                        <asp:TextBox type="date" name="fecha" AutoComplete="off" AutoCompleteType="Disabled" ID="F2" min="1980-01-01" max="2050-12-31" step="1" class="form-control" runat="server" />
                    </div>

                    <div class="col-xs-12 md-3 col-lg-2">
                        <label>Url Imagen</label>
                        <asp:TextBox runat="server" type="text" AutoComplete="off" AutoCompleteType="Disabled" ID="Imagen" MaxLength="1200" class="form-control" placeholder="" TabIndex="3" />
                    </div>

                    <div class="col-xs-2 col-md-3">
                        <br />
                        <span aria-hidden="true"></span>
                        <asp:LinkButton runat="server" type="button" ToolTip="Vista Previa" Class="btn btn-success" OnClick="Buscar" ID="LinkButton1">                                        
                <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
                        </asp:LinkButton>

                        <asp:LinkButton runat="server" type="button" ToolTip="Limpiar" class="btn btn-primary" OnClick="Clean" ID="LinkButton3">                                        
        <span aria-hidden="true" class="glyphicon glyphicon-erase"></span>
                        </asp:LinkButton>
                    </div>

                    <div class="col-xs-2 col-md-3">
                        <br />
                        <span aria-hidden="true"></span>
                        <asp:Button runat="server" type="button" CssClass="btn btn-primary" OnClick="Save" Text="Guardar" ID="Button4"></asp:Button>
                    </div>

                    <div class="col-xs-12 col-md-12">
                        <br />
                        <span aria-hidden="true">
                            <br />
                        </span>
                    </div>

                    <%--Cuadro de Mensaje--%>
                    <div class="col-sx-12 col-md-7">
                        <label>Mensaje</label>
                        <asp:TextBox runat="server" type="text" TextMode="MultiLine" AutoComplete="off" AutoCompleteType="Disabled" ID="Nombre" MaxLength="120" class="form-control" placeholder="Ingrese un Texto..." TabIndex="3" />
                    </div>
                    <%--Cuadro de Mensaje--%>

                    <div class="col-xs-10 col-sm-9">
                        <br />
                        <div class="alert alert-block alert-warning" id="AL" style="display: none">
                            <h5>¡Error!</h5>
                            <asp:Label ID="Label4" runat="server" Text=""></asp:Label>
                        </div>
                    </div>

                    <asp:Panel runat="server" ID="DatosV" Width="90%" Height=" 300px">
                        <div class="col-xs-12" id="Timg">
                            <center>     
            <asp:Image runat="server" ID="PckT" Width="50%" Height="50%" ImageUrl="../../Img/TGris.png" />      
          </center>
                        </div>
                    </asp:Panel>

                </ContentTemplate>
            </asp:UpdatePanel>

        </div>

        <!-- Registro Ususarios Tsys -->
        <div class="tab-pane container fade" id="menu1">
            <br />
            <h4>Administración de Notificaciones</h4>

            <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                <Triggers>
                    <%--       <asp:AsyncPostBackTrigger controlid="btnEdit" eventname="Click" />
       <asp:AsyncPostBackTrigger controlid="btnUpdate" eventname="Click" />
       <asp:AsyncPostBackTrigger controlid="btnCancel" eventname="Click" />--%>
                    <asp:AsyncPostBackTrigger ControlID="Button3" EventName="Click" />
                </Triggers>
                <ContentTemplate>

                    <div class="row">
                        <asp:GridView ID="GridView2" CssClass="table table-bordered bs-table"
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
                                <%--botones de acción sobre los registros...--%>
                                <asp:BoundField DataField="Numero" HeaderText="Numero" HeaderStyle-Width="70px" ReadOnly="True">
                                    <HeaderStyle Width="75px"></HeaderStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="Titulo" HeaderText="Titulo" HeaderStyle-Width="70px" ReadOnly="True">
                                    <HeaderStyle Width="140px"></HeaderStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="Tipo" HeaderText="Tipo" HeaderStyle-Width="100px" ReadOnly="True">
                                    <HeaderStyle Width="78px"></HeaderStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="Desde" HeaderText="Desde" HeaderStyle-Width="100px" ReadOnly="True">
                                    <HeaderStyle Width="78px"></HeaderStyle>
                                </asp:BoundField>
                                <asp:BoundField DataField="Hasta" HeaderText="Hasta" HeaderStyle-Width="100px" ReadOnly="True">
                                    <HeaderStyle Width="78px"></HeaderStyle>
                                </asp:BoundField>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="90px" HeaderText="Status" ValidateRequestMode="Enabled">
                                    <ItemTemplate>
                                        <asp:DropDownList runat="server" class="selectpicker show-tick form-control" ID="Stat" Enabled="true">
                                            <asp:ListItem Value="1">Activo</asp:ListItem>
                                            <asp:ListItem Value="2">Inactivo</asp:ListItem>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                    <HeaderStyle Width="90px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>

                                <%--botones de acción sobre los registros...--%>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="80px">
                                    <ItemTemplate>
                                        <asp:Button ID="btnEdit" runat="server" Text="Editar" CssClass="btn btn-info" CommandName="Edit" />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:Button ID="btnUpdate" runat="server" Text="Guardar" CssClass="btn btn-success" CommandName="Update" />
                                        <asp:Button ID="btnCancel" runat="server" Text="Cancelar" CssClass="btn btn-danger" CommandName="Cancel" />
                                    </EditItemTemplate>
                                    <HeaderStyle Width="80px"></HeaderStyle>

                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
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

                    <div id="Acc2" class="col-xs-12 col-md-12">
                        <label class="col-xs-11 col-md-11"></label>
                        <div class="col-xs-3 col-md-1">
                            <asp:Button Text="Actualizar" runat="server" CssClass="btn btn-warning" title="Actualizar Lista" ID="Button3" OnClick="Guardar" />
                        </div>
                    </div>

                    <asp:Panel runat="server" ID="DatosV1" Width="90%" Height=" 300px">
                        <div class="col-xs-12" id="Timg">
                            <center>     
            <asp:Image runat="server" ID="Image1" Width="50%" Height="50%" ImageUrl="../../Img/TGris.png" />      
          </center>
                        </div>
                    </asp:Panel>

                </ContentTemplate>
            </asp:UpdatePanel>

        </div>

        <!-- Registro Ususarios Tsys -->
        <div class="tab-pane container fade" id="menu2">
            <br />
            <h4>Administración de Paginas</h4>

            <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                <Triggers>
<%--                   <asp:AsyncPostBackTrigger ControlID="Radio1" EventName="CheckedChanged" />
                   <asp:AsyncPostBackTrigger ControlID="Radio2" EventName="CheckedChanged" />
                    <asp:AsyncPostBackTrigger ControlID="RadioButton1" EventName="CheckedChanged" />
                   <asp:AsyncPostBackTrigger ControlID="RadioButton2" EventName="CheckedChanged" />--%>
                    <%--<asp:AsyncPostBackTrigger ControlID="DropDownList1" EventName="selectedindexchanged" />
                   <asp:AsyncPostBackTrigger controlid="btnEdit" eventname="Click" />
                   <asp:AsyncPostBackTrigger controlid="btnUpdate" eventname="Click" />
                   <asp:AsyncPostBackTrigger controlid="btnCancel" eventname="Click" />--%>
                    <asp:AsyncPostBackTrigger ControlID="Button1" EventName="Click" />
                </Triggers>
                <ContentTemplate>

                    <br />
                    <br />
                    <br />
                    <br />
                    <div class="col-sx-12 col-md-2">
                        <label>Carga de Facturas</label>
                    </div>
                    <div class="col-sx-6 col-md-2">
                        <asp:RadioButton ID="Radio1" Text="Inactivo" GroupName="RadioGroup1" runat="server" OnCheckedChanged="Group1_CheckedChanged" AutoPostBack="true" />
                    </div>
                    <div class="col-sx-6 col-md-2">
                        <asp:RadioButton ID="Radio2" Text="Activo" GroupName="RadioGroup1" runat="server" OnCheckedChanged="Group1_CheckedChanged" AutoPostBack="true" />
                    </div>

                    <div class="col-xs-12 col-md-3" runat="server" id="f5">
                        <label>Desde</label>
                        <asp:TextBox type="date" name="fecha" AutoComplete="off" AutoCompleteType="Disabled" ID="TextBox1" min="1980-01-01" max="2050-12-31" step="1" class="form-control" runat="server" />
                    </div>
                    <div class="col-xs-12 col-md-3" runat="server" id="f6">
                        <label>Hasta</label>
                        <asp:TextBox type="date" name="fecha" AutoComplete="off" AutoCompleteType="Disabled" ID="TextBox2" min="1980-01-01" max="2050-12-31" step="1" class="form-control" runat="server" />
                    </div>
<%--                    <div class="col-sx-12 col-md-2">
                        <label>Redireccionar a:</label>
                        <asp:DropDownList runat="server" ID="DropDownList1" class="selectpicker show-tick form-control" AutoPostBack="true" data-live-search="true" data-style="btn-primary" OnSelectedIndexChanged="OnSelectedIndexChanged">
                            <asp:ListItem Value="1">Inicio</asp:ListItem>
                            <asp:ListItem Value="2">Contacto</asp:ListItem>
                            <asp:ListItem Value="3">Pagina en Blanco</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div runat="server"  class="col-sx-12 col-md-2" id="WF">
                        <label>Url Imagen</label>
                        <asp:TextBox runat="server" type="text" AutoComplete="off" AutoCompleteType="Disabled" ID="TextBox3" MaxLength="1200" class="form-control" placeholder="" TabIndex="3" />
                    </div>--%>


                    <div class="col-xs-12 col-md-12">
                        <br />
                        <span aria-hidden="true">
                            <br />
                        </span>
                    </div>

                    <div class="col-sx-12 col-md-2">
                        <label>Carga de Complementos de Pago</label>
                    </div>
                    <div class="col-sx-6 col-md-2">
                        <asp:RadioButton ID="RadioButton1" Text="Inactivo" GroupName="RadioGroup2"  runat="server" OnCheckedChanged="Group1_CheckedChanged" AutoPostBack="true" />
                    </div>
                    <div class="col-sx-6 col-md-2">
                        <asp:RadioButton ID="RadioButton2" Text="Activo" GroupName="RadioGroup2"  runat="server" OnCheckedChanged="Group1_CheckedChanged" AutoPostBack="true" />
                    </div>
                    <div class="col-xs-12 col-md-3" runat="server" id="f3">
                        <label>Desde</label>
                        <asp:TextBox type="date" name="fecha" AutoComplete="off" AutoCompleteType="Disabled" ID="TextBox4" min="1980-01-01" max="2050-12-31" step="1" class="form-control" runat="server" />
                    </div>
                    <div class="col-xs-12 col-md-3" runat="server" id="f4">
                        <label>Hasta</label>
                        <asp:TextBox type="date" name="fecha" AutoComplete="off" AutoCompleteType="Disabled" ID="TextBox5" min="1980-01-01" max="2050-12-31" step="1" class="form-control" runat="server" />
                    </div>
<%--                    <div class="col-sx-12 col-md-2">
                        <label>Redireccionar a:</label>
                        <asp:DropDownList runat="server" ID="DropDownList2" class="selectpicker show-tick form-control" AutoPostBack="false" data-live-search="true" data-style="btn-primary">
                            <asp:ListItem Value="1">Inicio</asp:ListItem>
                            <asp:ListItem Value="2">Contacto</asp:ListItem>
                            <asp:ListItem Value="3">Pagina en Blanco</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-sx-12 col-md-2">
                        <label>Url Imagen</label>
                        <asp:TextBox runat="server" type="text" AutoComplete="off" AutoCompleteType="Disabled" ID="TextBox6" MaxLength="1200" class="form-control" placeholder="" TabIndex="3" />
                    </div>--%>

                    <div class="col-xs-12 col-md-12">
                        <br />
                        <span aria-hidden="true">
                            <br />
                        </span>
                    </div>

                    <div id="Acc2" class="col-xs-12 col-md-12">
                        <label class="col-xs-11 col-md-11"></label>
                        <div class="col-xs-3 col-md-1">
                            <asp:Button Text="Guardar" runat="server" CssClass="btn btn-warning" title="Guardar Cambios" ID="Button1" OnClick="Desactivar" />
                        </div>
                    </div>

                    <asp:Panel runat="server" ID="Panel1" Width="90%" Height=" 300px">
                        <div class="col-xs-12" id="Timg">
                            <center>     
            <asp:Image runat="server" ID="Image2" Width="50%" Height="50%" ImageUrl="../../Img/TGris.png" />      
          </center>
                        </div>
                    </asp:Panel>

                </ContentTemplate>
            </asp:UpdatePanel>

        </div>


    </div>

</asp:Content>
