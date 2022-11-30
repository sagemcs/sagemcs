<%@ Page Title="Complementos de Pagos" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeFile="Pagos_Proveedores.aspx.cs" Inherits="Pagos_Proveedores" %>

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
    <br /><br /><br /><br /><br /><br /><br /><br />
        

            <!-- Nav pills -->
        <ul class="nav nav-pills">
          <li class="nav-item active">
            <a class="nav nav-pills nav-justified" data-toggle="pill" href="#home">Carga</a>
          </li>
          <li class="nav-item">
            <a class="nav nav-pills nav-justified" data-toggle="pill" href="#menu1">Consulta</a>
          </li>
        </ul>

        
        <div class="tab-content">

        <!-- Tab Carga -->
        <div class="tab-pane container active" id="home">

        <div class="col-lg-12 col-sm-12 col-12">
         <h3>Carga Complemento de Pago</h3>
        </div>
        <br /><br /><br /><br />

        <asp:UpdatePanel runat="server" id="UpdatePanel2">
        <ContentTemplate>
        </ContentTemplate>
        <Triggers>  
        <asp:AsyncPostBackTrigger ControlID="GridView1"/>
        <asp:AsyncPostBackTrigger ControlID="SelP" EventName="selectedindexchanged" />
        </Triggers>
        </asp:UpdatePanel>

        <div class ="form-group row"  runat="server" ID="Datos">
        
        <div class ="row">
<%--        <div class="col-lg-5 col-sm-5 col-12">
            <h4>Archivo XML.</h4>
            <div class="input-group">
                <label class="input-group-btn">
                    <span class="btn btn-primary">Seleccionar&hellip;
                                <asp:FileUpload type="file" ID="FileUpload2" runat="server" Style="display: none;" accept="application/xml" />
                    </span>
                </label>
                <asp:TextBox type="text" runat="server" class="form-control" ID="Cxml" ReadOnly="true" />
            </div>
        </div>--%>

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

        <div class ="row">
        <div class="col-xs-12 col-md-3 col-lg-3">
              <label>Proveedor</label>
              <asp:DropDownList ID="SelP" runat="server" class="selectpicker show-tick form-control" OnSelectedIndexChanged="List_SelectProvs" AutoPostBack ="true"></asp:DropDownList>
            </div>
        </div>

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
            <div class="form-group col-lg-2 col-sm-4 col-xs-4" visible ="false">
                <h3 style="color:forestgreen; display:none">Total Pago:</h3>
            </div>
            <div class="form-group col-lg-2 col-sm-4 col-xs-4" visible ="false">
                <h3 runat="server" id ="Tot" style="color:forestgreen; display:none" >$ 0.00</h3>
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

        </div>
        
        <!-- Tab Consulta -->
        <div class="tab-pane container fade" id="menu1">
        <br />
        <asp:UpdatePanel runat="server" id="UpdatePanel" updatemode="Conditional">
            <ContentTemplate>
            
            <div class="col-lg-12 col-sm-12 col-12">
            <h3>Consulta de Pagos</h3>
            </div>
            <br /><br /><br />
            <div class="row">
              <br />
            <div class="col-xs-12 md-3 col-lg-3">
             <label>Folio</label>
            <asp:TextBox runat="server" type="text" MaxLength="20" ID="Folio" class="form-control" TabIndex="3" />
          </div>

            <div class="col-xs-12 md-3 col-lg-3">
              <label>Factura</label>
            <asp:TextBox runat="server" type="text" MaxLength="20" ID="Factura" class="form-control" TabIndex="4"  />
          </div>

            <div class="col-xs-12 md-3 col-lg-3">
              <label>Monto</label>
            <asp:TextBox runat="server" type="text" MaxLength="20" ID="Monto" class="form-control" TabIndex="2"  onkeypress="return numbersonly(event);"/>
          </div>

            <div class="col-xs-12 col-md-3 col-lg-3">
              <label>Estatus</label>
            <asp:DropDownList ID="List" runat="server" class="selectpicker show-tick form-control" OnSelectedIndexChanged="List_SelectedIndexChanged" AutoPostBack="True">
               <asp:ListItem Value="2">Aprobado</asp:ListItem>
               <asp:ListItem Value="1">Pendiente</asp:ListItem>
               <asp:ListItem Value="3">Rechazado</asp:ListItem>
               <asp:ListItem Value="4">Eliminado</asp:ListItem>
            </asp:DropDownList>
          </div>

            <div class="col-xs-11 md-4 col-lg-3">
            <asp:CheckBox runat="server" id="ChkFechas" Text="Filtrar Por Fechas" AutoPostBack="true" OnCheckedChanged="ChkFechas_CheckedChanged"/>
            <asp:DropDownList ID="LFechas" runat="server" class="selectpicker show-tick form-control">
               <asp:ListItem Value="Factura">Fecha de Recepcion</asp:ListItem>
               <asp:ListItem Value="Pago">Fecha de Pago</asp:ListItem>
            </asp:DropDownList>
           </div>

            <div class="col-xs-6 md-4 col-lg-3">
              <label>Desde</label>
            <asp:TextBox type="date" name="fecha" ID="F1" min="1980-01-01" max="2050-12-31" step="1"  class="form-control"  runat="server"/>
          </div>

            <div class="col-xs-6 md-4 col-lg-3">
              <label>Hasta</label>
            <asp:TextBox type="date" name="fecha" ID="F2" min="1980-01-01" max="2050-12-31" step="1"  class="form-control"  runat="server"/>
          </div>

            <div class="col-xs-12 col-md-3 col-lg-3">
              <label>Proveedor</label>
              <asp:DropDownList ID="SelProv" runat="server" class="selectpicker show-tick form-control" OnSelectedIndexChanged="List_SelectedIndexChanged" AutoPostBack ="true"></asp:DropDownList>
            </div>

            <br />
            <br />
            </div>
            <br />
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

            <asp:Panel runat="server" ID="Panel1" Width="90%" Height=" 300px">
              <div class="col-xs-12">
                  <font size="5" color="#A4A4A4">No Se Encontraron Resultados</font>
              </div>
              <div class="col-xs-12" id="Timg">
                  <center>     
                        <asp:Image runat="server" ID="Image1" Width="50%" Height="50%" ImageUrl="../../Img/TGris.png" />      
                       </center>

              </div>
            </asp:Panel>
            <br />
            <div class ="Row">
                <asp:GridView ID="GridView3" CssClass="table table-bordered bs-table" 
                    margin-left="auto" margin-right="auto" runat="server" 
                    AutoGenerateColumns="False" 
                    onpageindexchanging="GridView1_PageIndexChanging" 
                    onrowcancelingedit="GridView1_RowCancelingEdit" 
                    onrowediting="GridView1_RowEditing" 
                    OnRowCommand="GridView1_RowCommand" 
                    OnRowCreated = "GridView1_RowCreated" 
                    onrowupdating="GridView1_RowUpdating" 
                    OnRowDataBound="GridView1_RowDataBound"
                    width="99%">

                    <AlternatingRowStyle BackColor="White" />

                    <HeaderStyle BackColor="#337ab7" Font-Bold="True" ForeColor="White" />
                    <EditRowStyle BackColor="#ffffcc" />
                    <EmptyDataRowStyle forecolor="Red" CssClass="table table-bordered" />

                    <Columns>

                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="50px">
                            <ItemTemplate>
                                <asp:Button ID="Documento_2" CssClass="btn btn-default" runat="server" CommandName="Documento_2" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="PDF" />
                            </ItemTemplate>
                        <HeaderStyle Width="50px"></HeaderStyle>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        </asp:TemplateField>

                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="50px" Visible="false" >
                            <ItemTemplate>
                                <asp:Button ID="Documento_3" CssClass="btn btn-default" runat="server" CommandName="Documento_3" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="XML" />
                            </ItemTemplate>
                        <HeaderStyle Width="50px"></HeaderStyle>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        </asp:TemplateField>
  
                        <asp:BoundField DataField="Factura" HeaderText="Factura" HeaderStyle-Width="60px" ReadOnly="True" SortExpression="C.B." >
                                        <HeaderStyle Width="60px"></HeaderStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="Serie" HeaderText="Serie" HeaderStyle-Width="65px" SortExpression="Fecha" ReadOnly="True" >
                                        <HeaderStyle Width="65px"></HeaderStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="Folio" HeaderText="Folio"  HeaderStyle-Width="65px" ReadOnly="True" SortExpression="Detalles" >
                                        <HeaderStyle Width="65px"></HeaderStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="Total" HeaderText="Total" HeaderStyle-Width="78px" ReadOnly="True" SortExpression="Referencia" >
                                        <HeaderStyle Width="78px"></HeaderStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="Proveedor" HeaderText="Proveedor" HeaderStyle-Width="80px" SortExpression="Fecha" ReadOnly="True" >
                        <HeaderStyle Width="65px"></HeaderStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="Company" HeaderText="Empresa"  HeaderStyle-Width="120px" ReadOnly="True" SortExpression="Detalles" >
                                        <HeaderStyle Width="120px"></HeaderStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="FechaR" HeaderText="Fecha Recepción" HeaderStyle-Width="78px" ReadOnly="True" SortExpression="Referencia" >
                                        <HeaderStyle Width="78px"></HeaderStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="FechaP" HeaderText="Fecha Pago" HeaderStyle-Width="78px" ReadOnly="True" SortExpression="Referencia">
                            <HeaderStyle Width="78px"></HeaderStyle>
                        </asp:BoundField>
                        <asp:BoundField DataField="Status" HeaderText="Status" HeaderStyle-Width="78px" ReadOnly="True" SortExpression="Referencia">
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


      </ContentTemplate>
        <Triggers>
        <asp:AsyncPostBackTrigger controlid="Button1" eventname="Click" />
<%--        <asp:AsyncPostBackTrigger ControlID="SelProv" EventName="selectedindexchanged" />--%>
        <asp:PostBackTrigger ControlID="GridView3" />
        </Triggers>
        </asp:UpdatePanel>
        </div> 
            
        </div> 


</asp:Content>
