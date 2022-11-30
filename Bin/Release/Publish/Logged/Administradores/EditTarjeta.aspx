<%@ Page Title="" Language="C#" MasterPageFile="~/Logged/Administradores/SiteEmpleado.master" EnableEventValidation="false" AutoEventWireup="true" CodeFile="EditTarjeta.aspx.cs" Inherits="Logged_Administradores_EditTarjeta" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    Version 20-07-2022 By Rafael Boza
    <script src ="../Css/sweetalert.min.js" type="text/javascript"></script>
    <script src ="../../Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script src ="../../Scripts/custom.js" type="text/javascript"></script>   
    <script src ="../../Scripts/bootstrap-datepicker/js/bootstrap-datepicker.js"></script>
    <link rel="stylesheet" href="../../Scripts/bootstrap-datepicker/css/datepicker.css">
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
        .auto-style1 {
            left: 0px;
            top: 0px;
        }
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


                $('#STipoGasto').change(function () {

                    console.log('cambio');

                });

                function keydown(e) {

                    if ((e.which || e.keyCode) == 116 || ((e.which || e.keyCode) == 82 && ctrlKeyDown)) {
                        // Pressing F5 or Ctrl+R
                        //VariableG();
                        e.preventDefault();
                        setTimeout("location.href='EditTarjeta'", 10);
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

    <script>
        function show_policy() {
            alert('SIRVE');
            var tipo_gasto = $('#STipoGasto').val();
            $('#tbx_policy').attr('value', tipo_gasto);
            
        }
    </script>   
    <asp:MultiView ID="MultiView1" runat="server">
        
         <%--Vista de Articulos--%>
        <asp:View ID="View_Articulos" runat="server">
            <br />
            <br />
            <br />
            <br />
            <br />
            <br />
            <div class="col-lg-12 col-sm-12 col-12" id="M4">
                <h3>Insertar Articulos</h3>
            </div>
            <br />

            <%--Bloque Articulos al Grid--%>
            <div class="row">
                 <%--Tipo del gasto--%>
                <div class="col-lg-4 col-sm-6 col-xs-5">
                     <h4>Tipo del gasto:</h4>
                    <span class="dropdown-header">               
                        <asp:DropDownList ID="STipoGasto" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server" OnSelectedIndexChanged="STipoGasto_SelectedIndexChanged" AutoPostBack="true" >                           
                           <asp:ListItem Value=""> </asp:ListItem>
                           <asp:ListItem Value="1">Transporte Aéreo</asp:ListItem>
                           <asp:ListItem Value="2">Transporte Terrestre</asp:ListItem>
                           <asp:ListItem Value="3">Casetas</asp:ListItem>
                           <asp:ListItem Value="4">Gasolina</asp:ListItem>
                           <asp:ListItem Value="5">Estacionamiento</asp:ListItem>
                           <asp:ListItem Value="6">Alimentos</asp:ListItem>
                           <asp:ListItem Value="7">Hospedaje</asp:ListItem>
                           <asp:ListItem Value="8">Gastos Extraordinarios</asp:ListItem>
                        </asp:DropDownList>                             
                    </span>            
                </div>   
               </div>  
            <div class="row">
                <%--Articulos--%>
                <div class="col-lg-3 col-sm-3 col-xs-3">
                    <h4>Articulos:</h4>
                    <span class="dropdown-header">
                        <asp:DropDownList ID="drop_articulos" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server">
                        </asp:DropDownList>
                    </span>
                </div>
                <%--Importe del Articulo--%>
                <div class="col-lg-3 col-sm-3 col-xs-3">
                    <h4>Coste Unitario:</h4>
                    <div class="col-lg-12 col-sm-12 col-xs-12">
                        <asp:TextBox runat="server" type="text" AutoComplete="off" AutoCompleteType="Disabled" ID="tbx_importegasto" MaxLength="15" class="form-control"></asp:TextBox>
                    </div>
                </div>
                <%--Cantidad--%>
                <div class="col-lg-3 col-sm-3 col-xs-3">
                    <h4>Cantidad:</h4>
                    <div class="col-lg-12 col-sm-12 col-xs-12">
                        <asp:TextBox runat="server" type="text" AutoComplete="off" AutoCompleteType="Disabled" ID="tbx_cantidad" MaxLength="15" class="form-control"></asp:TextBox>
                    </div>
                </div>
                <%--Impuestos--%>
                <div class="col-lg-3 col-sm-3 col-xs-3">
                    <h4>Impuestos:</h4>
                    <span class="dropdown-header">
                        <asp:DropDownList ID="drop_taxes" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server">
                        </asp:DropDownList>
                    </span>
                </div>
               
            </div>

            <div class="row">
                <%--Agregar articulo--%>
                <div class="col-lg-2 col-sm-2 col-xs-2">
                    <h4>Finalizar:</h4>
                    <div class="col-lg-12 col-sm-12 col-xs-12">
                        <asp:Button ID="btn_additem" runat="server" Class="btn btn-tsys cargar" title="Agregar Artículo" Text="Guardar Artículo" OnClick="btn_additem_Click" CausesValidation="true" />
                    </div>
                </div>
                <%--Cancelar--%>
                <div class="col-lg-2 col-sm-2 col-xs-2">
                    <h4>Cancelar</h4>
                    <div class="col-lg-12 col-sm-12 col-xs-12">
                        <asp:Button ID="btn_cancelar_item" runat="server" Class="btn btn-tsys cargar" title="Cancelar" Text="Cancelar" OnClick="btn_cancelar_item_Click" />
                    </div>
                </div>
            </div>

        </asp:View>
        <%--Vista General--%>
        <asp:View ID="View_General" runat="server" OnActivate="View_General_Activate" OnDeactivate="View_General_Deactivate">
            <div class="col-lg-12 col-sm-12 col-12" id="M1" >
        <h3>Editar Tarjeta Corporativa</h3>
    </div>                 
   
            <br />

            <div class ="row">
                <%--   Tipo de moneda--%>
                <div class="col-lg-4 col-sm-4 col-xs-4">
                    <h4>Tipo de moneda:</h4>                           
                        <span class="dropdown-header">
                            <asp:DropDownList ID="drop_currency" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server">
                                <asp:ListItem Value=""> </asp:ListItem>
                                <asp:ListItem Value=1>Pesos</asp:ListItem>
                                <asp:ListItem Value=2>Dólares</asp:ListItem>
                                <asp:ListItem Value=3>Euros</asp:ListItem>   
                            </asp:DropDownList>
                        </span> 
                </div>  
                <%--Fecha del gasto--%>
                <div class="col-lg-4 col-sm-5 col-xs-5">
                    <h4>Fecha del gasto:</h4>
                    <div class="col-lg-12 col-sm-12 col-xs-12">
                        <asp:TextBox type="date" name="fecha" ID="tbx_fechagasto" AutoComplete = "off" AutoCompleteType="Disabled" min="1980-01-01" max="2050-12-31" step="1"  class="form-control"  runat="server" AutoPostBack="True" OnTextChanged="tbx_fechagasto_TextChanged"/>
                </div>
                </div>
               
                <%--Importe del gasto--%>
                <div class="col-lg-4 col-sm-6 col-xs-5">
                <h4>Importe del Gasto:</h4>  
                     <asp:UpdatePanel runat="server" id="upt" updatemode="Always">
                          <ContentTemplate>      
                         <div class="col-lg-12 col-sm-12 col-xs-12">
                               <asp:TextBox runat="server"  type="text" AutoComplete = "off" AutoCompleteType="Disabled" ID="tbx_importe" MaxLength="15" class="form-control" ReadOnly="True"></asp:TextBox>          
                          </div>    
                        </ContentTemplate> 
                        <Triggers>       
                            <asp:AsyncPostBackTrigger ControlID="GvItems" EventName="RowCommand" />                          
                        </Triggers>
                    </asp:UpdatePanel>      
               </div>
            </div>

            <div class="row">
                <%--Carga de XML--%>
                <div class="col-lg-5 col-sm-6 col-xs-5">
                    <h4>XML Factura:</h4>
                    <div class="input-group col-lg-12 col-sm-12 col-xs-12">
                        <label class="input-group-btn">
                            <span class="btn btn-primary">Seleccionar&hellip;
                                <asp:FileUpload  type="file" ID="fu_xml" ClientIDMode="Static" runat="server" Style="display: none;" accept="application/xml"/>                    
                            </span>
                        </label>
                        <asp:TextBox type="text" runat="server" class="form-control" ID="tbx_xml" ReadOnly="true" />
                    </div>
                    <small class="form-text text-muted">Archivo Max 15 Mb.</small>
                </div>
                <%--Carga de PDF--%>
                <div class="col-lg-5 col-sm-6 col-xs-5">
                        <h4>PDF Anexo:</h4>
                        <div class="input-group col-lg-12 col-sm-12 col-xs-12">
                            <label class="input-group-btn">
                                <span class="btn btn-primary">Seleccionar&hellip;
                                <asp:FileUpload type="file" ID="fu_pdf" runat="server" Style="display: none;" accept="application/pdf" />
                                </span>
                            </label>
                            <asp:TextBox type="text" runat="server" class="form-control" ID="tbx_pdf" ReadOnly="true" />
                        </div>
                        <small class="form-text text-muted">Archivo Max 15 Mb.</small>
                </div>
                <%--Carga de Voucher--%>
                <div class="col-lg-4 col-sm-4 col-xs-4">
                    <h4>Voucher Anexo:</h4>
                    <div class="input-group col-lg-12 col-sm-12 col-xs-12">
                        <label class="input-group-btn">
                            <span class="btn btn-primary">Seleccionar&hellip;
                            <asp:FileUpload type="file" ID="fu_voucher" runat="server" Style="display: none;" accept="application/pdf" />
                            </span>
                        </label>
                        <asp:TextBox type="text" runat="server" class="form-control" ID="tbx_voucher" ReadOnly="true" />
                    </div>
                    <small class="form-text text-muted">Archivo Max 15 Mb.</small>
               </div>
            </div>     
            
             <%--Motivo del gasto--%>
           <div class ="row">
               <div class="col-lg-8 col-sm-8 col-xs-8">
                    <h4>Motivo del Gasto:</h4>
                    <div class="col-lg-12 col-sm-12 col-xs-12">                    
                         <asp:TextBox type="text" name="motivo" ID="tbx_motivo" AutoComplete = "off" AutoCompleteType="Disabled" MaxLength="256"  class="form-control"  runat="server" EnableViewState="False" Height="68px" TextMode="MultiLine" />
                    </div>
                </div>
                 <%--Agregar impuestos o Articulos--%>
                <h3>Agregar Articulos</h3>
                <label class="col-form-label col-lg-4 col-sm-4 col-xs-4"></label>
                <div class="col-xs-4 col-md-4 col-xs-3">
                    <asp:Button ID="btn_new_article" runat="server"  Class="btn btn-primary" title="Añadir Articulos" Text="Agregar Articulo" OnClick="btn_new_article_Click"  />
                </div>   
            </div>
      
            <%--Grid Articulos--%>
            <asp:UpdatePanel runat="server" ID="upItems" UpdateMode="Conditional">
                <ContentTemplate>                        
                    <asp:GridView Caption="Articulos"
                        ID="GvItems"
                        runat="server"
                        CssClass="table table-bordered bs-table"
                        margin-left="auto" margin-right="auto"
                        AutoGenerateColumns="False"
                        OnRowCommand="GvItems_RowCommand"
                        OnRowDataBound="GvItems_RowDataBound"
                        CellPadding="4"
                        ForeColor="#333333" GridLines="None">

                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                        <Columns>
                            <asp:BoundField DataField="ItemKey" HeaderText="Id" ReadOnly="True" SortExpression="ItemKey" />
                            <asp:BoundField DataField="ItemId" HeaderText="Descripcion" ReadOnly="True" SortExpression="ItemId" />
                            <asp:BoundField DataField="UnitCost" HeaderText="Costo Unitario" ReadOnly="True" SortExpression="UnitCost" />
                            <asp:BoundField DataField="Qty" HeaderText="Cantidad" ReadOnly="True" SortExpression="Qty" />
                            <asp:BoundField DataField="Amount" HeaderText="Importe" ReadOnly="True" SortExpression="Amount" DataFormatString="{0:c}"/>
                            <asp:BoundField DataField="TipoGasto" HeaderText="Tipo de Gasto" ReadOnly="True" SortExpression="TipoGasto" />
                            <asp:BoundField DataField="STaxCodeID" HeaderText="Descripcion" ReadOnly="True" SortExpression="STaxCodeID" />                            
                            <asp:BoundField DataField="TaxAmount" HeaderText="Importe Impuesto" ReadOnly="True" SortExpression="Amount" DataFormatString="{0:c}"/>
                            <asp:CommandField ShowSelectButton="True" ControlStyle-CssClass="btn-success" ButtonType="Button" SelectText="Eliminar" ShowCancelButton="False">
                                <ControlStyle CssClass="btn-warning"></ControlStyle>
                            </asp:CommandField>
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

                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="GvItems" EventName="RowCommand"/>
                </Triggers>
            </asp:UpdatePanel>

            <%-- Grid Impuestos--%>

            <br />            
    <%--Boton Guardar--%>
     <div class="row">
         <div class="col-xs-3 col-md-3 col-xs-3">
            <asp:Button ID="btn_validar" runat="server"  Class="btn btn-tsys cargar" title="Validar Información" Text="Validar Información" OnClick="btn_validar_Click" />
        </div>        
         <div class="col-xs-4 col-md-4 col-xs-3">
            <asp:Button ID="btnSage" runat="server"  Class="btn btn-tsys cargar" title="Cargar Documentos" Text="Guardar" OnClick="btnSage_Click" Enabled="False" />
        </div>      
        <div class="col-xs-3 col-md-3 col-xs-3">
            <asp:Button ID="btn_Cancelar" runat="server"  Class="btn btn-tsys cargar" title="Cancelar" Text="Cancelar" OnClick="btn_Cancelar_Click" />
        </div>    

    </div>  
    <br />    
    
    <br />
        </asp:View>
        </asp:MultiView>

    
    <%--Labels Mensajes--%>
    <div class="auto-style1">
        <div class="alert alert-warning" id="B1" style="display: none">           
            <h3>Advertencia!</h3>
            Seleccione una Factura en XML para Cargar, vuelva a cargar los archivos.
        </div>
        <div class="alert alert-warning" id="B2" style="display: none">
            <h3>Advertencia!</h3>
            Seleccione una Factura en PDF para Cargar, vuelva a cargar los archivos.
        </div>
        <div class="alert alert-warning" id="B3" style="display: none">
            <h3>Advertencia!</h3>
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
        <div class="alert alert-block alert-danger" id="B11" style="display: none">
            <h3>Error!</h3>
            No coindice el monto declarado con el total de la factura, favor de verificar e intentar nuevamente.
        </div>
        <div class="alert alert-block alert-danger" id="B12" style="display: none">
            <h3>Error!</h3>
            Fecha del gasto requerida, favor de verificar e intentar nuevamente.
        </div>
        <div class="alert alert-block alert-danger" id="B13" style="display: none">
            <h3>Error!</h3>
            Tipo de gasto requerido, favor de verificar e intentar nuevamente.
        </div>
        <div class="alert alert-block alert-danger" id="B14" style="display: none">
            <h3>Error!</h3>
            Tipo de moneda requerido, favor de verificar e intentar nuevamente.
        </div>
        <div class="alert alert-block alert-danger" id="B15" style="display: none">
            <h3>Error!</h3>
            Importe del gasto requerido, favor de verificar e intentar nuevamente.
        </div>
        <div class="alert alert-block alert-danger" id="B16" style="display: none">
            <h3>Error!</h3>
            El importe solo debe contener números, punto o coma, favor de verificar e intentar nuevamente.
        </div>
         <div class="alert alert-block alert-danger" id="B30" style="display: none">
            <h3>Error!</h3>
            No coincide el importe total del gasto con los artículos e impuestos declarados, favor de verificar e intentar nuevamente.
        </div> 
        <br />
        <div class="alert alert-block alert-danger" id="B6" style="display: none">
            <h4>Error!</h4>
            <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                <span aria-hidden="true">&times;</span>
            </button>
            <asp:Label ID="Label4" runat="server" ></asp:Label>
        </div>
    </div>

    
</asp:Content>




