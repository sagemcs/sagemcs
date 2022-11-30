<%@ Page Title="Home Page" Language="C#" MasterPageFile="SiteEmpleado.master" AutoEventWireup="true" CodeFile="Documentacion_Empleado.aspx.cs" Inherits="Documentacion_Empleado" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    Version Actualizacion 08-Abril-2019 p.m. By Luis Angel Garcia P
    <meta http-equiv="Cache-Control" content="no-cache">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Pragma" content="no-cache" />

    <asp:LoginView runat="server" ViewStateMode="Enabled">
        <LoggedInTemplate>
            <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
                <Services>
                    <asp:ServiceReference Path="~/Servicios/NotificacionesWebService.asmx" />

                </Services>
                <Scripts>
                    <asp:ScriptReference Path="~/Scripts/custom.js" />
                    <asp:ScriptReference Path="~/Scripts/notificaciones.js" />
                </Scripts>
            </asp:ScriptManagerProxy>
        </LoggedInTemplate>
    </asp:LoginView>

    <script src="../../Css/sweetalert.min.js" type="text/javascript"></script>
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
        function alertme(titulo, mesaje, Tipo) {
            swal(titulo, mesaje, Tipo)
        }
    </script>
    <script>
        alert("Se ejecuta antes de cargar la pantalla.");
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

                    $('#BnEnv').click(function (e) {
                        Proceso(e, "/NOenviar_notificacion_de_rechazo_de_factura");
                        return false;
                    });

                    function Proceso(e, Proce) {
                        e.preventDefault();
                        var userKey = '';
                    }
                });
            });
            return true;
        })
    </script>

    <div class="col-lg-12 col-sm-5 col-12" id="M1">
        <h3>Documentación del Empleado</h3>
    </div>


    <!-- Nav pills -->
    <ul class="nav nav-pills">
        <li class="nav-item active">
            <%--    <a class="nav nav-pills nav-justified" data-toggle="pill" href="#home"></a>--%>
        </li>
        <li class="nav-item">
            <%--    <a class="nav nav-pills nav-justified" data-toggle="pill" href="#menu1"></a--%>
        </li>
    </ul>

    <!-- Tab panes -->
    <div class="tab-content">
        <div class="tab-pane container active" id="home">
            <asp:UpdatePanel runat="server" ID="UpdatePanel" UpdateMode="Conditional">

                <Triggers>
                    <%--<asp:AsyncPostBackTrigger controlid="Button1" eventname="Click" />--%>
                    <%--<asp:AsyncPostBackTrigger controlid="Button2" eventname="Click" />--%>
                    <asp:AsyncPostBackTrigger ControlID="BnEnv" EventName="Click" />
                </Triggers>

                <ContentTemplate>
                    <div class="row" id="Cam2">
                        <div class="col-xs-6 col-sm-6 col-md-6">
                            <div class="form-group row">
                                <label class="col-form-label col-sm-4">Nombre Completo:</label>
                                <div class="col-sm-8">
                                    <asp:TextBox runat="server" type="text" AutoComplete="off" AutoCompleteType="Disabled" MaxLength="80" ID="NombreC" class="form-control" TabIndex="1" />
                                    <div class="alert alert-block alert-danger" id="AID2" style="display: none">
                                        <h5>¡Error!</h5>
                                        <asp:Label runat="server" ID="Lsage"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-xs-6 col-sm-6 col-md-6">
                            <div class="form-group row">
                                <label class="col-form-label col-sm-4">Puesto:</label>
                                <div class="col-sm-8">
                                    <asp:TextBox runat="server" type="text" AutoComplete="off" AutoCompleteType="Disabled" MaxLength="80" ID="PuestoE" class="form-control" TabIndex="1" />
                                    <div class="alert alert-block alert-danger" id="AID3" style="display: none">
                                        <h5>¡Error!</h5>
                                        <asp:Label runat="server" ID="tplabel"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row" id="Cam2">
                        <div class="col-xs-6 col-sm-6 col-md-6">
                            <div class="form-group row">
                                <label class="col-form-label col-sm-4">Area:</label>
                                <div class="col-sm-8">
                                    <asp:TextBox runat="server" type="text" AutoComplete="off" AutoCompleteType="Disabled" MaxLength="30" ID="AreaE" class="form-control" TabIndex="1" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row" id="Cam2">
                        <div class="col-xs-6 col-sm-6 col-md-6">
                            <div class="form-group row">
                                <label class="col-form-label col-sm-4">Gefe Inmediato:</label>
                                <div class="col-sm-8">
                                    <asp:TextBox runat="server" type="text" AutoComplete="off" AutoCompleteType="Disabled" MaxLength="80" ID="GefeE" class="form-control" TabIndex="1" />
                                    <div class="alert alert-block alert-danger" id="ARF1" style="display: none">
                                        <h5>¡Error!</h5>
                                        El Campo RFC debe de ser a 12 Caracteres.Si
                                    </div>
                                    <div class="alert alert-block alert-danger" id="ARF2" style="display: none">
                                        <h5>¡Error!</h5>
                                        El Campo RFC es Obligatorio.
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-xs-6 col-sm-6 col-md-6">
                            <div class="form-group row">
                                <label class="col-form-label col-sm-4">Correo Gefe Inmediato:</label>
                                <div class="col-sm-8">
                                    <asp:TextBox runat="server" type="email" AutoComplete="off" AutoCompleteType="Disabled" MaxLength="50" ID="CorreoGefeE" class="form-control" TabIndex="1"/>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row" id="Cam2">
                        <div class="col-xs-6 col-sm-6 col-md-6">
                            <div class="form-group row">
                                <label class="col-form-label col-sm-12"></label>
                            </div>
                        </div>

                        <div class="col-xs-6 col-sm-6 col-md-6">
                            <div class="form-group row">
                                <label class="col-form-label col-sm-8"></label>
                                <div class="btn-toolbar" role="toolbar" aria-label="Toolbar with button groups">
                                    <div class="btn-group mr-2" role="group" aria-label="First group">
                                        <asp:Button Text="Guardar" runat="server" ID="BnEnv" autoposback="True" CssClass="btn btn-primary" title="Guardar Datos" OnClick="btn_EnviarTsy" />
                                    </div>
                                    <div class="btn-group mr-2" role="group" aria-label="Second group">
                                        <asp:Button Text="Editar" runat="server" ID="Button2" autoposback="True" CssClass="btn btn-danger" title="Editar Datos" OnClick="btn_Editar" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <!-- Registro Ususarios Tsys -->
        <div class="tab-pane container fade" id="menu1">
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                <Triggers>
                    <%--<asp:AsyncPostBackTrigger ControlID ="BntTsys" EventName ="Click" />--%>
                </Triggers>
                <ContentTemplate>
                    <div class="form-group row" id="Cam4">
                        <label class="col-form-label col-sm-2">Nombre Usuario:</label>
                        <div class="col-sm-4">
                            <asp:TextBox runat="server" type="text" AutoComplete="off" AutoCompleteType="Disabled" name="Razon" ID="NombreT" MaxLength="90" class="form-control" placeholder="" TabIndex="3" />
                            <div class="alert alert-block alert-danger" id="AUT" style="display: none">
                                <h5>¡Error!</h5>
                                El Campo Nombre Usuario es Obligatorio.
                            </div>
                        </div>
                    </div>

                    <div class="row" id="Cam5">
                        <div class="col-xs-10 col-sm-10 col-md-6">
                            <div class="form-group row">
                                <label class="col-form-label col-sm-4">Correo Electrónico:</label>
                                <div class="col-xs-10 col-sm-8 col-md-8">
                                    <asp:TextBox runat="server" type="text" AutoComplete="off" AutoCompleteType="Disabled" name="Email" MaxLength="100" ID="EmailT" class="form-control" placeholder="" TabIndex="4" />
                                    <div class="alert alert-block alert-danger" id="AET" style="display: none">
                                        <h5>¡Error!</h5>
                                        <asp:Label runat="server" ID="CMT"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-xs-10">
                        <div class="alert alert-block alert-danger" id="ATT" style="display: none">
                            <h5>¡Error!</h5>
                            Todos los Campos estan Vacios, Son Obligatorios
                        </div>
                    </div>

                    </div>           
                </ContentTemplate>
            </asp:UpdatePanel>

            <div class="col-lg-6 col-sm-6 col-xs-10">
                <h4></h4>
                <div class="input-group col-lg-10 col-sm-10 col-xs-10">
                    <label class="input-group-btn">
                        <span class="btn btn-primary" style="display: none;">Seleccionar&hellip;
                      <asp:FileUpload type="file" ID="FileUpload1" runat="server" Style="display: none;" accept=".xls, .xlsx" />
                        </span>
                    </label>
                    <asp:TextBox type="text" runat="server" AutoComplete="off" AutoCompleteType="Disabled" Style="display: none;" class="form-control" ID="ID" ReadOnly="true" />

                </div>
                <div class="col-sm-1">
                    <br />
                    <%--<asp:Button Text="Cargar" ID="Button4" runat="server" CssClass="btn btn-danger" Style="display: none;" title="Cargar Datos" OnClick="Excel" />--%>
                </div>
            </div>
            <span class="col-lg-12">
                <br />
                <br />
            </span>
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
