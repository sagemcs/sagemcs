<%@ Page Title="Complementos de Pagos" Language="C#" AutoEventWireup="true" MasterPageFile="SiteEmpleado.Master" CodeFile="Pagos_Empleados.aspx.cs" Inherits="Pagos_Empleados" %>

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

        function isFloatNumber(e, t) {
            var n;
            var r;
            if (navigator.appName == "Microsoft Internet Explorer" || navigator.appName == "Netscape") {
                n = t.keyCode;
                r = 1;
                if (navigator.appName == "Netscape") {
                    n = t.charCode;
                    r = 0
                }
            } else {
                n = t.charCode;
                r = 0
            }
            if (r == 1) {
                if (!(n >= 48 && n <= 57 || n == 46)) {
                    t.returnValue = false
                }
            } else {
                if (!(n >= 48 && n <= 57 || n == 0 || n == 46)) {
                    t.preventDefault()
                }
            }
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

        <asp:UpdatePanel runat="server" id="UpdatePanel2">
        <ContentTemplate>
        <div class ="row">
        <div class="col-xs-4 col-sm-4 col-md-4">
            <div class="form-group row">
                <label class="col-form-label col-sm-4">Proveedor</label>
                <div class="col-sm-8">
                    <asp:DropDownList ID="SelProv" OnSelectedIndexChanged="SelProv_SelectedIndexChanged1" AutoPostBack="true" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server"></asp:DropDownList>
                </div>
            </div>
        </div>
        
        </div>
        </ContentTemplate>
        <Triggers>
        <asp:AsyncPostBackTrigger ControlID="SelProv" EventName="selectedindexchanged" />
        <%--<asp:AsyncPostBackTrigger controlid="Button2" eventname="Click" />
        <asp:AsyncPostBackTrigger ControlID="Button3" EventName="Click" />--%>       
        <asp:AsyncPostBackTrigger ControlID="GridView1"/>
        </Triggers>
        </asp:UpdatePanel>
        <br />

        <div class ="form-group row"  runat="server" ID="Datos">
        
        <div class ="row">
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

        </div>
        <br />
        <div class="col-lg-10 col-sm-10 col-xs-10" style="margin-top: 30px;">
          <div class="alert alert-block alert-danger" id="B1" style="display: none">
             <h3>¡Error!</h3>
             <asp:Label runat="server" ID="Mensajes"></asp:Label>
           </div>
        </div>
        
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        <ContentTemplate>
        <div class="col-lg-11 col-sm-11 col-xs-11">
            <label class="col-form-label col-lg-7 col-sm-3 col-xs-3"></label>
            <div class="form-group col-lg-2 col-sm-4 col-xs-4">
                <h3 style="color:forestgreen">Total Pago:</h3>
            </div>
            <div class="form-group col-lg-2 col-sm-4 col-xs-4">
                <h3 runat="server" id ="Tot" style="color:forestgreen">$ 0.00</h3>
            </div>
        </div>
         <asp:GridView ID="GridView1" CssClass="table table-bordered bs-table"
              margin-left="auto" margin-right="auto" runat="server"
              AutoGenerateColumns="False"
              BackColor="White" BorderColor="#E7E7FF"
              BorderStyle="None" BorderWidth="1px"
              CellPadding="3" GridLines="Horizontal"
              Height="34px"
              Width="99%">
              
              <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
              <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
              <Columns>

                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="15px" HeaderText="Seleccionar">
                  <ItemTemplate>
                    <asp:CheckBox ID="Check" CssClass="ChkBoxClass" Style="width: 20px; height: 20px;" runat="server" AutoPostBack="true" OnCheckedChanged="GridView2_RowCommand" />
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Fecha" HeaderText="Fecha" ReadOnly="False" Visible="true" SortExpression="Fecha" HeaderStyle-Width="25px" ControlStyle-Width="25px" />
                <asp:BoundField DataField="VendID" HeaderText="ID Proveedor" ReadOnly="False" Visible="true" SortExpression="VendID" />
                <asp:BoundField DataField="RFC" HeaderText="RFC" ReadOnly="True" SortExpression="RFC" />
                <asp:BoundField DataField="OC" HeaderText="OC" ReadOnly="True" SortExpression="OC" />
                <asp:BoundField DataField="Folio" HeaderText="Folio" ReadOnly="True" SortExpression="Folio" />
                <asp:BoundField DataField="Moneda" HeaderText="Moneda" ReadOnly="True" SortExpression="Moneda" />
                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="95px" HeaderText="Pago" ControlStyle-Width="90px">
                  <ItemTemplate>
                   <asp:TextBox ID="cant" type="text" AutoComplete="off" AutoCompleteType="Disabled" CssClass="form-control" runat="server" AutoPostBack="true" onkeypress="return isFloatNumber(this,event);" OnTextChanged="TextBox1_TextChanged" />
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Saldo" HeaderText="Pago" ReadOnly="True" visible="false" SortExpression="Saldo" />
                <asp:BoundField DataField="Total" HeaderText="Total" ReadOnly="True" SortExpression="Total" />
                <asp:BoundField DataField="Resto" HeaderText="Saldo Restante" ReadOnly="True" SortExpression="Resto" />
              </Columns>
              
              <EditRowStyle BackColor="#F7F6F3" ForeColor="#333333" />
              <FooterStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="True" />
              <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
              <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
              <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
              <SortedAscendingCellStyle BackColor="#E9E7E2" />
              <SortedAscendingHeaderStyle BackColor="#506C8C" />
              <SortedDescendingCellStyle BackColor="#FFFDF8" />
              <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
         </asp:GridView>
        </ContentTemplate>
         <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="GridView1" />
                </Triggers>
        </asp:UpdatePanel>
        
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

        <div class="alert alert-warning" id="AL" style="display: none; margin-top: 30px;">
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

        


</asp:Content>
