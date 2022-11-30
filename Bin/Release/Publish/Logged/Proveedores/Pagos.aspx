<%@ Page Title="Complementos de Pagos" Language="C#" AutoEventWireup="true" MasterPageFile="MenuPreP.Master" CodeFile="Pagos.aspx.cs" Inherits="Pagos" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    Version 16-Octubre-2019 By Luis Angel Garcia P
    <script src="../../Css/sweetalert.min.js" type="text/javascript"></script>
    <link href="../../Css/HojaProv.css" rel="stylesheet" type="text/css" />
    <script src ="../../Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script src ="../../Scripts/custom.js" type="text/javascript"></script>

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
        })
    </script>

    <script>
        function alertme(titulo, mesaje, Tipo) {
            swal(titulo, mesaje, Tipo);
            unblockUI();
        }
    </script>
    <script>
        function alerte(campo) {
            $(campo).show("slow").delay(2000).hide("slow");
            unblockUI();
        }
    </script>

    <script>
        function alert(campo) {
            $(campo).toggle();
            unblockUI();
        }
    </script>

    <script>
        function alertY(campo) {
            $(campo).style.display = 'none';
            unblockUI();
        }
    </script>
    <br />
    <br />
    <br />

    <%--<asp:Panel ID="Panel1" runat="server" Height="526px" Width="99%">--%>

    <div class="col-lg-11 col-sm-11 col-xs-11">
        <label class="col-form-label col-lg-5 col-sm-3 col-xs-1"></label>
        <div class="form-group col-lg-6 col-sm-8 col-xs-10">
            <br />
            <br />
            <h3>Carga Complemento de Pago</h3>
        </div>
    </div>

    <div class="col-lg-12 col-sm-12 col-xs-12">
        <div class="form-group col-lg-11 col-sm-11 col-xs-11">
            <label>Seleccione sus Archivos para Validar</label>
        </div>
    </div>

<%--    <asp:UpdatePanel runat="server" ID="UpdatePanel" UpdateMode="Conditional">
        <ContentTemplate>--%>
            <div class="col-lg-5 col-sm-5 col-12">
                <h4>Archivo XML.</h4>
                <div class="input-group">
                    <label class="input-group-btn">
                        <span class="btn btn-primary">Seleccionar&hellip;
                            <asp:FileUpload type="file" ID="FileUpload2" runat="server" Style="display: none;" accept="application/xml" />
<%--                            <ajaxToolkit:AsyncFileUpload  type="file" ID="FileUpload4" runat="server" PersistFile="true" Style="display: none;" />--%>
                        </span>
                    </label>
                    <asp:TextBox type="text" runat="server" class="form-control" ID="Cxml" ReadOnly="true" />
                </div>
            </div>

            <div class="col-lg-5 col-sm-5 col-12">
                <h4>Archivo PDF.</h4>
                <div class="input-group">
                    <label class="input-group-btn">
                        <span class="btn btn-primary">Seleccionar&hellip;
                            <asp:FileUpload type="file" ID="FileUpload1" runat="server" Style="display: none;" accept="application/pdf" />
                        </span>
                    </label>
                    <input type="text" class="form-control" readonly>
                </div>
            </div>

            <div class="col-lg-2 col-sm-2 col-12">
                <br />
                <br />
                <span>
                    <asp:Button Text="Cargar" runat="server" AutoPostBack="False" ID="Button3" href="#" OnClick="Unnamed1_Click" CssClass="btn btn-tsys cargar" />
                </span>
            </div>

            <div class="row">
            <div class="col-lg-10 col-sm-10 col-xs-12"></div>
            <br />
            </div>

                <div class="col-lg-10 col-sm-10 col-xs-10" style="margin-top:30px;">
                    <div class="alert alert-block alert-danger" id="B1" style="display: none">
                        <h3>¡Error!</h3>
                        <asp:Label runat="server" ID="Mensajes"></asp:Label>
                    </div>
                </div>

             <div class="alert alert-warning"  id="AL"  style="display:none; margin-top:30px;">
             <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                <span aria-hidden="true">&times;</span>
            </button>
            <h5>¡Error!</h5>
            <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
            </div>



            <asp:Panel ID="Panel2" runat="server" Height="90px" Width="90%">
                <br />
                <br />

                <div class="col-lg-3 col-sm-4 col-xs-8">
                    <label>
                        Version:
                        <asp:TextBox ID="txt10" CssClass="form-control" ReadOnly="true" runat="server" /></label>
                </div>

                <div class="col-lg-3 col-sm-4col-xs-8">
                    <label>
                        Folio:
                        <asp:TextBox ID="txt9" CssClass="form-control" ReadOnly="true" runat="server" /></label>
                </div>

                <div class="col-lg-3 col-sm-4 col-xs-8">
                    <label>
                        Serie:
                        <asp:TextBox ID="txt8" CssClass="form-control" ReadOnly="true" runat="server" /></label>
                </div>

                <div class="col-lg-3 col-sm-4 col-xs-8">
                    <label>
                        Fecha Timbrado:
                        <asp:TextBox ID="txt6" CssClass="form-control" ReadOnly="true" runat="server" /></label>
                </div>

                <div class="col-lg-3 col-sm-4 col-xs-8">
                    <label>
                        Fecha Pago:
                        <asp:TextBox ID="txt2" CssClass="form-control" ReadOnly="true" runat="server" /></label>
                </div>

                <div class="col-lg-3 col-sm-4 col-xs-8">
                    <label>
                        Forma de Pago:
                        <asp:TextBox ID="txt5" CssClass="form-control" ReadOnly="true" runat="server" /></label>
                </div>

                <div class="col-lg-3 col-sm-4 col-xs-8">
                    <label>
                        Moneda:
                        <asp:TextBox ID="txt3" CssClass="form-control" ReadOnly="true" runat="server" /></label>
                </div>

                <div class="col-lg-3 col-sm-4 col-xs-8">
                    <label>
                        Tasa de Cambio:
                        <asp:TextBox ID="txt4" CssClass="form-control" ReadOnly="true" runat="server" /></label>
                </div>

                <div class="col-lg-3 col-sm-4 col-xs-8">
                    <label>
                        TOTAL :
                        <asp:TextBox ID="txt1" CssClass="form-control" ReadOnly="true" runat="server" /></label>
                </div>

                <div class="col-lg-3 col-sm-4 col-xs-8">
                    <label>
                        UUID:
                        <asp:TextBox ID="txt7" CssClass="form-control" ReadOnly="true" runat="server" /></label>
                </div>

                <div class="col-lg-3 col-sm-4 col-xs-8">
                    <label>
                     N° Operacion:
                    <asp:TextBox ID="TextBox1" CssClass="form-control" ReadOnly="true" runat="server" /></label>
                </div>

                <div class="col-lg-3 col-sm-4 col-xs-8">
                    <label>
                        RFC Banco Emisor:
                        <asp:TextBox ID="TextBox2" CssClass="form-control" ReadOnly="true" runat="server" /></label>
                </div>

                <div class="col-lg-3 col-sm-4 col-xs-8">
                    <label>
                        Nombre Banco:
                        <asp:TextBox ID="TextBox3" CssClass="form-control" ReadOnly="true" runat="server" /></label>
                </div>

                <div class="col-lg-3 col-sm-4 col-xs-8">
                    <label>
                       N° Cuenta:
                        <asp:TextBox ID="TextBox4" CssClass="form-control" ReadOnly="true" runat="server" /></label>
                </div>

                <div class="col-lg-3 col-sm-4 col-xs-8">
                    <label>
                        RFC Cuenta Beneficiario:
                        <asp:TextBox ID="TextBox5" CssClass="form-control" ReadOnly="true" runat="server" /></label>
                </div>
<%--                <div class="col-lg-9 col-sm-8 col-xs-8">
                    <label>
                        CtaBeneficiario:
                        <asp:TextBox ID="TextBox6" CssClass="form-control" ReadOnly="true" runat="server" /></label>
                </div>--%>
            <br />
            <br />
            </asp:Panel>
            <br />
            <br />
            <br />
            <div class="row">
                <asp:GridView ID="GridView2" runat="server"
                    CssClass="table table-bordered bs-table"
                    AutoGenerateColumns="False"
                    BackColor="White" BorderColor="#E7E7FF"
                    BorderStyle="None" BorderWidth="1px"
                    CellPadding="3" GridLines="Horizontal"
                    Height="34px" Width="90%">

                    <AlternatingRowStyle BackColor="#F7F7F7" />
                    <Columns>
                        <asp:BoundField HeaderText="UUID" SortExpression="UUID" DataField="UUID" />
                        <asp:BoundField HeaderText="Folio" SortExpression="Folio" DataField="Folio" />
                        <asp:BoundField HeaderText="Moneda" SortExpression="Moneda" DataField="Moneda" />
                        <asp:BoundField HeaderText="Metodo Pago" SortExpression="Metodo" DataField="Metodo Pago" />
                        <asp:BoundField HeaderText="Parcialidad" SortExpression="Parcialidad" DataField="Parcialidad" />
                        <asp:BoundField HeaderText="SaldoAnt" SortExpression="SaldoAnt" DataField="SaldoAnt" />
                        <asp:BoundField HeaderText="Pago" SortExpression="Pago" DataField="Pago" />
                        <asp:BoundField HeaderText="NvoSaldo" SortExpression="NvoSaldo" DataField="NvoSaldo" />
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


        <div id="Barra" class="centrarCaja" style="display:none;">
        <h4 id="Prov" runat="server">Aplicar Movimiento Para: </h4>
        <asp:DropDownList ID="UsuarioP" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server" Width="30%"></asp:DropDownList>
        </div>

<%--    </asp:Panel>--%>
<%--            <div id="Acc">
                <div class="col-xs-1 col-md-1">
                    <asp:Button Text="Enviar" runat="server" CssClass="btn btn-tsys" title="Agregar Usuario a la Lista" ID="Button1" />
                </div>
            </div>--%>

<%--                        <div id="Acc">
                <div class="col-xs-1 col-md-1">
                    <asp:Button Text="Enviar" runat="server" CssClass="btn btn-tsys" OnClick="BtnEnviar" title="Agregar Usuario a la Lista" ID="Button2" />
                </div>
            </div>--%>
</asp:Content>
