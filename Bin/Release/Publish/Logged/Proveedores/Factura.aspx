<%@ Page Title="Carga de Facturas" Language="C#" MasterPageFile="MenuPreP.master" AutoEventWireup="true" CodeFile="Factura.aspx.cs" Inherits="Factura" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
Version 16-Octubre-2019 By Luis Angel Garcia P
    <script src ="../Css/sweetalert.min.js" type="text/javascript"></script>
    <script src ="../../Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script src ="../../Scripts/custom.js" type="text/javascript"></script>
    <style>
            #Update1{
            position :fixed;
            top:40%;
            bottom :40%;
            left : 40%;
            right: 40%;
            z-index : 1001;
            overflow : hidden;
            margin :0;
            padding :0;
            background-color : #999;
            filter :alpha(opacity =50);
            opacity : 0.80;
            border: 1px solid gray;
            border:none;
            background-image :url("../../Img/aguarde.gif");
            background-repeat :no-repeat;
            background-size:cover;
            background-position:center;
            border-radius: 15px 15px 15px 15px;
            -moz-border-radius: 15px 15px 15px 15px;
            -webkit-border-radius: 15px 15px 15px 15px;}
        
            #backGround1{
            position :fixed;
            top:0px;
            bottom : 0px;
            left : 0px;
            right: 0px;
            overflow : hidden;
            margin :0;
            padding :0;
            background-color : #999999;
            filter :alpha(opacity =80);
            opacity : 0.80;
            z-index : 1000;}

            #Testdbd {
            width:50px;
            height:50px;
            background-color:red;}
        </style>

    <script type="text/javascript">
        $(function () {
            // We can attach the `fileselect` event to all file inputs on the page
            $(document).on('change', ':file', function () {
                var input = $(this);
                var caja = $(this).parents('.input-group').find(':text'),
                numFiles = input.get(0).files ? input.get(0).files.length : 1;
                label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
                //input.trigger('fileselect', [numFiles, label]);
                caja.val(label);
                //swal('Angel', label, 'success')
            });

            // We can watch for our custom `fileselect` event like this
            $(document).ready(function () {

                var ctrlKeyDown = false;
                $(document).on("keydown", keydown);
                $(document).on("keyup", keyup);

                function keydown(e) {

                    if ((e.which || e.keyCode) == 116 || ((e.which || e.keyCode) == 82 && ctrlKeyDown)) {
                        // Pressing F5 or Ctrl+R
                        //VariableG();
                        e.preventDefault();
                        setTimeout("location.href='Facturas'", 10);
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
                //$(':file').on('fileselect', function (event, numFiles, label) {

                //    var input = $(this).parents('.input-group').find(':text'),
                //    log = numFiles > 1 ? numFiles + ' files selected' : label;
                //    if (input.length) {
                //        input.val(log);
                //    } else {
                //        input.val(log);
                //        //if (log) alert(log);
                //    }

                //});

            });
        })
    </script>

    <script>
        function alertme(titulo,mesaje,Tipo) {
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
        function alertB(campo) {
            $(campo).toggle();
            unblockUI();
        }
    </script>

    <script >
        function hideshow() {
            document.getElementById('UpdatePross').style.display = "none";
            unblockUI();
        }
    </script>

    <script>
        function alert(campo) {
            $(campo).show("slow").delay(2000).hide("slow");
            unblockUI();
        }
    </script>

<%--    <asp:UpdatePanel runat="server" id="UpdatePanel" UpdateMode="Conditional" ChildrenAsTriggers="False" >
      
    <contenttemplate>--%>
   
    <div class="col-lg-12 col-sm-12 col-12" id="M1">
        <h3>Carga de Facturas</h3>
    </div>

    <div class ="form-group row">

        <%--<div class="row" id="Cam">--%>

        <div class="col-xs-3 col-sm-3 col-md-3">
            <div class="form-group row">
                <label class="col-form-label col-sm-4">Folio OC:</label>
                <div class="col-sm-8">
                    <asp:TextBox runat="server" type="text" AutoComplete="off" AutoCompleteType="Disabled" MaxLength="12" ID="NoOc" class="form-control" TabIndex="1" />
                </div>
            </div>
        </div>

        <div class="col-xs-1 col-sm-1 col-md-1">
            <div class="btn-toolbar" role="toolbar">
                <div class="col-xs-1 btn-group">
                    <asp:LinkButton runat="server" type="button" Class="btn btn-tsys" ID="LinkButton1" OnClick="Btn_Buscar">                                        
                          <span aria-hidden="true" class="glyphicon glyphicon-search"></span>
                    </asp:LinkButton>
                </div>
            </div>
        </div>

        <div class="col-xs-4 col-sm-4 col-md-4">
            <div class="form-group row">
                <label class="col-form-label col-sm-4">Folio Factura:</label>
                <div class="col-sm-8">
                    <asp:TextBox runat="server" type="text" AutoComplete="off" AutoCompleteType="Disabled" MaxLength="10" ID="FolioF" class="form-control" TabIndex="1" />
                </div>
            </div>
        </div>

       <%--</div>--%>

        <div class="col-xs-3 col-sm-3 col-md-3" style="display: none">
            <div class="form-group row">
                <label class="col-form-label col-sm-4">Proveedor</label>
                <div class="col-sm-8">
                    <asp:DropDownList ID="SelProv" AutoPostBack="true" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server"></asp:DropDownList>
                </div>
            </div>

        </div>


    </div>
    <br />
    <asp:UpdatePanel runat="server" id="UpdatePanel1">
   <ContentTemplate>
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

        <asp:TemplateField  ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="15px"  HeaderText="Seleccionar" >
          <ItemTemplate>
                <asp:CheckBox ID="Check" CssClass="ChkBoxClass" style="width: 20px; height: 20px;" runat ="server" AutoPostBack="true" OnCheckedChanged="GridView2_RowCommand" />
          </ItemTemplate>
         </asp:TemplateField>

         <asp:BoundField DataField="PoLine" HeaderText="No Partida" ReadOnly="False" Visible="true" SortExpression="PoLine" HeaderStyle-Width="25px" ControlStyle-Width="25px" />

         <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="65px" HeaderText="Cantidad" ControlStyle-Width="70px">
          <ItemTemplate>
                <asp:TextBox ID="cant" type="text" AutoComplete="off" AutoCompleteType="Disabled" CssClass="form-control" runat ="server" AutoPostBack="true" onkeypress="return isFloatNumber(this,event);"  OnTextChanged="TextBox1_TextChanged" />
          </ItemTemplate>
         </asp:TemplateField>
        
         
         <asp:BoundField DataField="Cantidad" HeaderText="Cantidad" ReadOnly="False" Visible="false" SortExpression="Cantidad" />
         <asp:BoundField DataField="Art" HeaderText="Articulo" ReadOnly="True" SortExpression="Art" />
         <asp:BoundField DataField="ShortDesc" HeaderText="Descripcion" ReadOnly="True" SortExpression="ShortDesc" />
         <asp:BoundField DataField="UnitCost" HeaderText="Precio Unitario" ReadOnly="True" SortExpression="UnitCost" />
         <asp:BoundField DataField="ExtAmt" HeaderText="Monto" ReadOnly="True" SortExpression="ExtAmt" />
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
<%--        <asp:AsyncPostBackTrigger ControlID="SelProv" EventName="selectedindexchanged" />
        <asp:AsyncPostBackTrigger controlid="Button2" eventname="Click" />
        <asp:AsyncPostBackTrigger ControlID="Button3" EventName="Click" />--%>       
        <asp:AsyncPostBackTrigger ControlID="GridView1"/>
    </Triggers>
   </asp:UpdatePanel>
    <br />
    <div class ="row">
        <%--    <div class="col-lg-4 col-sm-8 col-xs-10">
        <h4>XML Factura:</h4>
        <div class="input-group col-lg-12 col-sm-12 col-xs-12">
            <label class="input-group-btn">
                <span class="btn btn-primary">Seleccionar&hellip;
                    <asp:FileUpload  type="file" ID="FileUpload1" ClientIDMode="Static" runat="server" Style="display: none;" accept="application/xml"/> 
                </span>
            </label>
            <asp:TextBox type="text" runat="server" class="form-control" ID="Contrato" ReadOnly="true" />
        </div>
        <small class="form-text text-muted">Archivo Max 15 Mb.</small>
    </div>--%>

        <div class="col-lg-4 col-sm-8 col-xs-10">
            <h4>PDF Factura:</h4>
            <div class="input-group col-lg-12 col-sm-12 col-xs-12">
                <label class="input-group-btn">
                    <span class="btn btn-primary">Seleccionar&hellip;
                    <asp:FileUpload type="file" ID="FileUpload2" runat="server" Style="display: none;" accept="application/pdf" />
                        <%-- <ajaxToolkit:AsyncFileUpload ID="FileUpload2" runat="server" Style="display: none;" PersistFile="true" accept="application/pdf" />--%>
                    </span>
                </label>
                <asp:TextBox type="text" runat="server" class="form-control" ID="TextBox1" ReadOnly="true" />
            </div>
            <small class="form-text text-muted">Archivo Max 15 Mb.</small>
        </div>

        <div class="col-lg-4 col-sm-8 col-xs-10">
            <h4>PDF Anexo:</h4>
            <div class="input-group col-lg-12 col-sm-12 col-xs-12">
                <label class="input-group-btn">
                    <span class="btn btn-primary">Seleccionar&hellip;
                    <asp:FileUpload type="file" ID="FileUpload3" runat="server" Style="display: none;" accept="application/pdf" />
                        <%--<ajaxToolkit:AsyncFileUpload ID="FileUpload3" runat="server" Style="display: none;" PersistFile="true"  accept="application/pdf" />--%>
                    </span>
                </label>
                <asp:TextBox type="text" runat="server" class="form-control" ID="TextBox2" ReadOnly="true" />
            </div>
            <small class="form-text text-muted">Archivo Max 15 Mb.</small>
        </div>
    </div>

    <div>   
    <br />
     <div class="row">
      <label class="col-form-label col-lg-10 col-sm-10 col-xs-6"></label>
      <div class="col-xs-7 col-md-7">
        <asp:Button ID="btnSage" runat="server" OnClick="btnSage_Click" Class="btn btn-tsys cargar" title="Cargar Documentos" Text="Cargar" />
      </div>
     </div>  
    </div>

    <br />
    <br />

    <div class="col-lg-10 col-sm-10 col-xs-12">
        <div class="alert alert-block alert-danger" id="B1" style="display: none">           
            <h3>Error!</h3>
            Seleccione una Factura en XML para Cargar, vuelva a cargar los archivos.
        </div>
        <div class="alert alert-block alert-danger" id="B2" style="display: none">
            <h3>Error!</h3>
            Seleccione una Factura en PDF para Cargar, vuelva a cargar los archivos.
        </div>
        <div class="alert alert-block alert-danger" id="B3" style="display: none">
            <h3>Error!</h3>
            Seleccione un Anexo en PDF para Cargar, vuelva a cargar los archivos.
        </div>
        <div class="alert alert-block alert-danger" id="B4" style="display: none">            
            <h3>Error!</h3>
            Al intentar cargar los documentos en el servidor vuelva a intentarlo, en caso de persistir el problema comunicate con el área de sistemas.
        </div>
        <div class="alert alert-block alert-success" id="B5" style="display: none">
            <h3>Éxito!</h3>
            Factura Procesada.
        </div>
        <div class="alert alert-block alert-danger" id="B7" style="display: none">
            <h3>Error!</h3>
            El Archivo XML Factura no cuenta con formato XML, favor de verificar el archivo e intentar nuevamente.
        </div>
        <div class="alert alert-block alert-danger" id="B8" style="display: none">            
            <h3>Error!</h3>
            El Archivo PDF FACTURA no cuenta con formato PDF, favor de verificar el archivo e intentar nuevamente.
        </div>
        <div class="alert alert-block alert-danger" id="B9" style="display: none">
            <h3>Éxito!</h3>
            El Archivo PDF Anexo no cuenta con formato PDF, favor de verificar el archivo e intentar nuevamente.
        </div>
        <div class="alert alert-block alert-danger" id="B10" style="display: none">
            <h3>Error!</h3>
            No. de O.C Requerido, favor de verificar e intentar nuevamente.
        </div>
        <div class="alert alert-block alert-danger" id="B6" style="display: none">
            <h4>Error!</h4>
            <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                <span aria-hidden="true">&times;</span>
            </button>
            <asp:Label ID="Label4" runat="server" ></asp:Label>
        </div>
    </div>

      <%--<asp:Label ID="Label4" runat="server" ForeColor="#FF9900" BorderColor="White" BorderStyle="Solid"></asp:Label>--%>
    <br />
    <asp:GridView 
        ID="gvFacturas" 
        runat="server" 
        CssClass="table table-bordered bs-table"
        margin-left="auto" margin-right="auto"
        AutoGenerateColumns="False" 
        CellPadding="4" 
        OnRowCommand="GridView1_RowCommand" ForeColor="#333333" GridLines="None" >

         <RowStyle BackColor="#F7F6F3" ForeColor="#333333" /> 
         <AlternatingRowStyle BackColor="White" ForeColor="#284775" />

        <Columns>
         <asp:BoundField DataField="InvoiceKey" HeaderText="ID" ReadOnly="True" SortExpression="InvoiceKey" />
         <asp:BoundField DataField="UUID" HeaderText="No.Factura" ReadOnly="True" SortExpression="UUID" />
         <asp:BoundField DataField="FechaTransaccion" HeaderText="Fecha Transaccion" ReadOnly="True" SortExpression="FechaTransaccion" />
         <asp:TemplateField>
          <ItemTemplate>
            <asp:Button ID="Documento_1" runat="server" CommandName="Documento_1" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="XML" />
          </ItemTemplate> 
           </asp:TemplateField>
             <asp:TemplateField>
                 <ItemTemplate>
            <asp:Button ID="Documento_2" runat="server" CommandName="Documento_2" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="Comprobante PDF" />
          </ItemTemplate>
         </asp:TemplateField>
         <asp:TemplateField>
          <ItemTemplate>
            <asp:Button ID="Documento_3" runat="server" CommandName="Documento_3" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="Anexo PDF" />
          </ItemTemplate>
         </asp:TemplateField>
         </Columns>

        <EditRowStyle BackColor="#999999" />
        <FooterStyle BackColor="#5D7B9D" ForeColor="White" Font-Bold="True" /> 
        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" /> 
        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" /> 
        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" /> 
         <SortedAscendingCellStyle BackColor="#E9E7E2" />
         <SortedAscendingHeaderStyle BackColor="#506C8C" />
         <SortedDescendingCellStyle BackColor="#FFFDF8" />
         <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
    </asp:GridView>
    <br />
    <asp:GridView ID="gvValidacion" runat="server" 
       CssClass="table table-bordered bs-table"
       AutoGenerateColumns="False"
       BackColor="White" BorderColor="#E7E7FF"
       BorderStyle="None" BorderWidth="1px"
       CellPadding="3" GridLines="Horizontal"
       Height="34px" Width="90%">

        <AlternatingRowStyle BackColor="#F7F7F7" />
        <Columns>
         <asp:BoundField DataField="InvoiceKey" HeaderText="ID" ReadOnly="True" SortExpression="InvoiceKey" />
         <asp:BoundField DataField="folio" HeaderText="Folio Cmp." ReadOnly="True" SortExpression="folio" />
         <asp:BoundField DataField="UUID" HeaderText="UUID" ReadOnly="True" SortExpression="UUID" />
         <asp:BoundField DataField="NodeOc" HeaderText="O.C." ReadOnly="True" SortExpression="NodeOc" />
         <asp:BoundField DataField="FechaError" HeaderText="Fecha" ReadOnly="True" SortExpression="FechaError" />
         <asp:BoundField DataField="ErrorValidacion" HeaderText="Error de Validación" ReadOnly="True" SortExpression="ErrorValidacion" />
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
    

</asp:Content>

