<%@ Page Title="" Language="C#" MasterPageFile="~/Logged/Administradores/SiteVal.master" EnableEventValidation="false" AutoEventWireup="true" CodeFile="ValidadorGastosMedicosMenores.aspx.cs" Inherits="Logged_Administradores_ValidadorGastosMedicosMenores" %>

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
                        setTimeout("location.href='ValidadorGastosMedicosMenores'", 10);
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
    
    <div class="col-lg-12 col-sm-12 col-12" id="M1" >
       <h3>Validación de Gastos Médicos Menores</h3>
    </div>      
  
     <asp:Panel runat="server" id="upPackage">        
         <div class="col-lg-12 col-sm-12 col-12" id="M2">
            <h4>Paquete de Gastos Médicos Menores</h4>
        </div>
     <%--Fila del paquete--%>
    <div class="row">
            <%--Numero de paquete actual--%>
            <div class="col-lg-3 col-sm-3 col-xs-3">
                <h4>Paquete Actual:</h4>   
                <div class="col-lg-12 col-sm-12 col-xs-12">               
                    <asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" ID="tbx_no_paquete" MaxLength="120" class="form-control"  ReadOnly="true" BackColor="White"/>
                </div>
             </div>
            <%--Cantidad de reembolsos--%>
            <div class="col-lg-3 col-sm-3 col-xs-3">
                <h4>Gastos agregados:</h4>   
                <div class="col-lg-12 col-sm-12 col-xs-12">               
                    <asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" ID="tbx_cant_reembolsos" MaxLength="120" class="form-control"  ReadOnly="true" BackColor="White"/>
                </div>
             </div>
            <div class="col-lg-3 col-sm-3 col-xs-3">
                <h4>Crear / Cerrar Paquete:</h4>   
                <div class="col-lg-12 col-sm-12 col-xs-12">               
                    <asp:Button ID="btn_crear_paquete" runat="server"  Class="btn btn-tsys cargar" title="Crear Paquete" Text="Crear Paquete" OnClick="btn_crear_paquete_Click" />
                </div>
             </div> 
         </div>           
      </asp:Panel>   

    <br />
    <br />

    <div class="col-lg-12 col-sm-12 col-12" id="M3">
        <h4>Filtro de Gastos Médicos Menores</h4>
    </div>

    <asp:UpdatePanel runat="server" id="UpdatePanel2" updatemode="Conditional">
        <ContentTemplate>
    <%--Fila de filtros--%>
            <div class="row">

        <%-- Estado--%>     
        <div class="col-lg-2 col-sm-2 col-xs-2">
            <h4>Estado:</h4>      
            <div class="col-lg-12 col-sm-12 col-xs-12">
                <asp:DropDownList ID="drop_status" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drop_status_SelectedIndexChanged">                        
                    <asp:ListItem Value=0>Todos </asp:ListItem>
                    <asp:ListItem Value=1>Pendiente</asp:ListItem>
                    <asp:ListItem Value=2>Aprobado</asp:ListItem>
                    <asp:ListItem Value=3>Cancelado</asp:ListItem>
                </asp:DropDownList>  
            </div>
         </div>
        <%-- Empleados--%>     
        <div class="col-lg-2 col-sm-2 col-xs-2">
            <h4>Empleados:</h4>      
            <div class="col-lg-12 col-sm-12 col-xs-12">
                <asp:DropDownList ID="drop_empleados" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drop_empleados_SelectedIndexChanged">                        
                </asp:DropDownList>  
            </div>
         </div>
        <%--Fecha de Inicio--%>
        <div class="col-lg-2 col-sm-2 col-xs-2">
            <h4>Fecha de Inicio:</h4>
            <div class="col-lg-12 col-sm-12 col-xs-12">
                <asp:TextBox type="date" name="fecha" ID="tbx_fecha_inicio" AutoComplete = "off" AutoCompleteType="Disabled" min="1980-01-01" max="2050-12-31" step="1"  class="form-control"  runat="server" AutoPostBack="True" OnTextChanged="tbx_fecha_inicio_TextChanged" />
            </div>
        </div>

        <%--Fecha de Final--%>
        <div class="col-lg-2 col-sm-2 col-xs-2">
            <h4>Fecha de Fin:</h4>
            <div class="col-lg-12 col-sm-12 col-xs-12">
                <asp:TextBox type="date" name="fecha" ID="tbx_fecha_fin" AutoComplete = "off" AutoCompleteType="Disabled" min="1980-01-01" max="2050-12-31" step="1"  class="form-control"  runat="server" AutoPostBack="True" OnTextChanged="tbx_fecha_fin_TextChanged"/>
            </div>
        </div>
       <%-- Boton Filtrar--%>       
        <div class="col-lg-2 col-sm-2 col-xs-2">
            <h4>Borrar Filtros:</h4>
            <div class="col-xs-4 col-md-4 col-xs-3">
            <asp:Button ID="btn_filtrar" runat="server"  Class="btn btn-tsys cargar"  Text="Borrar Filtros" OnClick="btn_filtrar_Click" />
         </div>
        </div>

    </div>
        </ContentTemplate> 
        <Triggers>                                             
            <asp:AsyncPostBackTrigger ControlID="btn_filtrar" EventName="Click" />                          
        </Triggers>
      </asp:UpdatePanel>

    <br />

    <asp:UpdatePanel runat="server" id="UpdatePanel" updatemode="Always">
        <ContentTemplate>
    <asp:GridView 
        ID="gvGastos" 
        runat="server"            
        OnRowCommand="gvGastos_RowCommand"
        OnRowDataBound="gvGastos_RowDataBound"
        CssClass="table table-bordered bs-table"
        margin-left="auto" margin-right="auto"
        AutoGenerateColumns="False" 
        CellPadding="4"         
        ForeColor="#333333" GridLines="None" >

         <RowStyle BackColor="#F7F6F3" ForeColor="#333333" /> 
         <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        <Columns>
         <asp:BoundField DataField="MinorMedicalExpenseId" HeaderText="Id" ReadOnly="True" SortExpression="MinorMedicalExpenseId" />           
         <asp:BoundField DataField="PackageId" HeaderText="Paquete" ReadOnly="True" SortExpression="PackageId" />
         <asp:BoundField DataField="Date" HeaderText="Fecha del Gasto" ReadOnly="True" SortExpression="Date" DataFormatString="{0:dd/MM/yyyy}" />           
         <asp:BoundField DataField="Amount" HeaderText="Importe del Gasto" ReadOnly="True" SortExpression="Amount" DataFormatString="{0:c}"/>
         <asp:BoundField DataField="Status" HeaderText="Estado del Gasto" ReadOnly="True" SortExpression="Status" />
         <asp:TemplateField HeaderText="Aprobar">
           <ItemTemplate>
              <asp:Button ID="btnSelect" runat="server" CssClass="btn-success" CommandName="Select" Text="Aprobar" ></asp:Button>
           </ItemTemplate>             
         </asp:TemplateField>  
         <asp:TemplateField HeaderText="Denegar">
           <ItemTemplate>
              <asp:Button ID="btnDeny" runat="server" CssClass="btn-warning" CommandName="Deny" Text="Denegar" ></asp:Button>
           </ItemTemplate>
         </asp:TemplateField>  
         <asp:TemplateField HeaderText="Motivo">
            <ItemTemplate>
                <asp:TextBox ID="tbx_motivo" runat="server"  Width="100%" Text='<%# Bind("DeniedReason") %>' ></asp:TextBox>
            </ItemTemplate>
             <ItemStyle HorizontalAlign="Center" Width="25%" />
         </asp:TemplateField>        
          <asp:TemplateField HeaderText="">
           <ItemTemplate>
              <asp:Button ID="btnComent" runat="server" CssClass="btn-success"  CommandName="Coment" Text="Ver / Comentar" ></asp:Button>
           </ItemTemplate>             
         </asp:TemplateField>
            <asp:TemplateField HeaderText="Procesar">
            <ItemTemplate>
                <asp:Button ID="btn_Integrar" runat="server" CssClass="btn-success" CommandName="Integrate" Text="Integrar a Sage"></asp:Button>
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
        </ContentTemplate> 
        <Triggers>                                             
            <asp:AsyncPostBackTrigger ControlID="drop_empleados" EventName="Selectedindexchanged" />                          
            <asp:AsyncPostBackTrigger ControlID="drop_status" EventName="Selectedindexchanged" />                          
            <asp:AsyncPostBackTrigger ControlID="tbx_fecha_inicio" EventName="Textchanged" />                          
            <asp:AsyncPostBackTrigger ControlID="tbx_fecha_fin" EventName="Textchanged" />     
            <asp:AsyncPostBackTrigger ControlID="btn_filtrar" EventName="Click" />     
             <asp:AsyncPostBackTrigger ControlID="gvGastos" EventName="RowCommand" />  
        </Triggers>
      </asp:UpdatePanel>
        <asp:UpdateProgress ID="UpdateProgress2"  AssociatedUpdatePanelID="UpdatePanel" runat="server">
                <ProgressTemplate>               
                    <div style=" position:absolute; z-index:999; height:100%; top:0; left:0; width:100%; filter: alpha(opacity=60); opacity: 0.6;-moz-opacity: 0.8; background-color: Black; ">
                        <div style=" position:absolute;top:50%; left:40%">
                            <asp:Image ID="Image2" ImageUrl="/Img/aguarde.gif" runat="server" />
                         </div>                   
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
    <br />   
    <br />     
    
   
    <%--Errores en Alert--%>
    <div class="auto-style1">
        <div class="alert alert-block alert-danger" id="B1" style="display: none">           
            <h3>Error!</h3>
            Fecha de salida requerida, favor de verificar e intentar nuevamente.
        </div>
        <div class="alert alert-block alert-danger" id="B2" style="display: none">
            <h3>Error!</h3>
            Fecha de llegada requerida, favor de verificar e intentar nuevamente.
        </div>
        <div class="alert alert-block alert-danger" id="B3" style="display: none">
            <h3>Error!</h3>
            Fecha de llegada mayor que Fecha de salida, favor de verificar e intentar nuevamente.
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
        <div class="alert alert-block alert-danger" id="B20" style="display: none">
            <h3>Error!</h3>
            El paquete actual no contiene reembolsos, favor de verificar e intentar nuevamente.
        </div>
        <div class="alert alert-block alert-danger" id="B21" style="display: none">
            <h3>Error!</h3>
            Motivo de la cancelacion requerido, favor de verificar e intentar nuevamente.
        </div>
        <div class="alert alert-block alert-danger" id="B22" style="display: none">
            <h3>Error!</h3>
            Número de Paquete requerido, favor de verificar e intentar nuevamente.
        </div>
         <div class="alert alert-block alert-success" id="B23" style="display: none">
            <h3>Correcto!</h3>
            El reembolso se ha integrado satisfactoriamente en Sage
        </div>
        <div class="alert alert-block alert-danger" id="B24" style="display: none">
            <h3>Error!</h3>
            El reembolso no se ha podido integrar a Sage, favor de verificar e intentar nuevamente.
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