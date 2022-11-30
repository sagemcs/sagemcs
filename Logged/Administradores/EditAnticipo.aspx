<%@ Page Title="" Language="C#" MasterPageFile="~/Logged/Administradores/SiteEmpleado.master" AutoEventWireup="true" CodeFile="EditAnticipo.aspx.cs" Inherits="Logged_Administradores_EditAnticipo" %>
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
                        setTimeout("location.href='EditAnticipo'", 10);
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
        <h3>Captura de Anticipos de gastos</h3>
    </div>
    <asp:UpdatePanel runat="server" id="UpdatePanel3" updatemode="Always">
                    <ContentTemplate>
    <div class ="row" >
     <%--   Tipo de anticipo--%>
        <div class="col-lg-3 col-sm-3 col-xs-3">
            <h4>Tipo de Anticipo:</h4>                           
                <span class="dropdown-header">
                    <asp:DropDownList ID="drop_tipo_anticipo" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server" AutoPostBack="True" OnSelectedIndexChanged="drop_tipo_anticipo_SelectedIndexChanged">
                     <asp:ListItem Value=""> </asp:ListItem>
                     <asp:ListItem Value=1>Viaje</asp:ListItem>
                     <asp:ListItem Value=2>Compra Extraordinaria</asp:ListItem>                       
                    </asp:DropDownList>
                </span> 
        </div>    

     <%--Folio de Anticipo--%>        
        <div class="col-lg-6 col-sm-6 col-xs-6">
            <h4>Folio de Anticipo:</h4>            
            <div class="col-lg-8 col-sm-6 col-xs-6">
               <asp:TextBox runat="server" type="text" AutoComplete = "off" ReadOnly="true" AutoCompleteType="Disabled" ID="tbx_folio" MaxLength="500" class="form-control" BackColor="White"></asp:TextBox>
            </div>             
        </div>       
    </div>
   
    <br />

    <div class="row">
        <%--Importe del gasto--%>
        <div class="col-lg-4 col-sm-6 col-xs-5">
        <h4>Importe del Gasto:</h4>            
             <div class="col-lg-12 col-sm-12 col-xs-12">                
                  <asp:TextBox runat="server"  type="text" AutoComplete = "off" AutoCompleteType="Disabled" ID="tbx_importe" MaxLength="15" class="form-control" OnTextChanged="tbx_importe_TextChanged" AutoPostBack="true"></asp:TextBox>                      
              </div>        
       </div>

        <%--Jefe Inmediato--%>
        <div class="col-lg-3 col-sm-3 col-xs-3">
             <h4>Jefe Inmediato:</h4>
            <div class="col-lg-12 col-sm-12 col-xs-12">
                   <asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" ID="tbx_jefe_inmediato" MaxLength="15" class="form-control" ReadOnly="true" BackColor="White"></asp:TextBox>
                </div>        
        </div>    
    </div>

    <br />

    <div class ="row">
        <%--Fecha de Salida--%>
        <div class="col-lg-3 col-sm-3 col-xs-3">
            <h4>Fecha de Salida:</h4>
            <div class="col-lg-12 col-sm-12 col-xs-12">                 
                    <asp:TextBox type="date" name="fecha" ID="tbx_fecha_salida" AutoComplete = "off" AutoCompleteType="Disabled" min="1980-01-01" max="2050-12-31" step="1"  class="form-control"  runat="server" AutoPostBack="true" OnTextChanged="tbx_fecha_salida_TextChanged" />               
                
            </div>
        </div>

        <%--Fecha de Llegada--%>
        <div class="col-lg-3 col-sm-3 col-xs-3">
            <h4>Fecha de Llegada:</h4>
            <div class="col-lg-12 col-sm-12 col-xs-12">               
                    <asp:TextBox type="date" name="fecha" ID="tbx_fecha_llegada" AutoComplete = "off" AutoCompleteType="Disabled" min="1980-01-01" max="2050-12-31" step="1"  class="form-control"  runat="server" OnTextChanged="tbx_fecha_llegada_TextChanged" AutoPostBack="true"/>                
                
            </div>
        </div>

         <%--Fecha de Comprobacion del Gasto--%>
        <div class="col-lg-3 col-sm-3 col-xs-3">
            <h4>Fecha de Comprobación:</h4>            
            <div class="col-lg-12 col-sm-12 col-xs-12">
                <asp:TextBox type="text" name="fecha" ID="tbx_fecha_comprobacion" AutoComplete = "off" AutoCompleteType="Disabled" step="1"  class="form-control"  runat="server" ReadOnly="true" BackColor="White"/>
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
        </div>
                
    </div>   
    
    <br />   
   

    <br />
    <br />

    <div class="col-lg-10 col-sm-10 col-xs-12">
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
        <div class="alert alert-block alert-danger" id="B16" style="display: none">
            <h3>Error!</h3>
            El importe solo debe contener números, punto o coma, favor de verificar e intentar nuevamente.
        </div>
        <div class="alert alert-block alert-danger" id="B17" style="display: none">
            <h3>Error!</h3>
            Jefe Inmediato requerido, favor de verificar e intentar nuevamente.
        </div>
        <div class="alert alert-block alert-danger" id="B18" style="display: none">
            <h3>Error!</h3>
            El usuario cuenta con un anticipo vencido, favor de verificar e intentar nuevamente.
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
    </ContentTemplate> 
        <Triggers>       
            <asp:AsyncPostBackTrigger ControlID="tbx_importe" EventName="TextChanged" />    
            <asp:AsyncPostBackTrigger ControlID="tbx_fecha_llegada" EventName="TextChanged" />
            <asp:AsyncPostBackTrigger ControlID="tbx_fecha_salida" EventName="TextChanged" />
            <asp:AsyncPostBackTrigger ControlID="drop_tipo_anticipo" EventName="SelectedIndexChanged" />            
        </Triggers>
    </asp:UpdatePanel>
     <%--Boton Guardar / Cancelar--%>
    <div class="row">
        <div class="col-xs-3 col-md-3 col-xs-3">
            <asp:Button ID="btn_validar" runat="server" Class="btn btn-tsys cargar" title="Validar Información" Text="Validar Información" OnClick="btn_validar_Click" />
        </div>
        <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Always">
            <ContentTemplate>
                <div class="col-xs-3 col-md-3 col-xs-3">
                    <asp:Button ID="btn_Guardar" runat="server" Class="btn btn-tsys cargar" title="Guardar Cambios" Text="Guardar" OnClick="btn_Guardar_Click" Enabled="False" />
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="tbx_importe" EventName="TextChanged" />
                <asp:AsyncPostBackTrigger ControlID="tbx_fecha_llegada" EventName="TextChanged" />
                <asp:AsyncPostBackTrigger ControlID="tbx_fecha_salida" EventName="TextChanged" />
                <asp:AsyncPostBackTrigger ControlID="drop_tipo_anticipo" EventName="SelectedIndexChanged" />
            </Triggers>
        </asp:UpdatePanel>
        <div class="col-xs-3 col-md-3 col-xs-3">
            <asp:Button ID="btn_Cancelar" runat="server" Class="btn btn-tsys cargar" title="Cancelar" Text="Cancelar" OnClick="btn_Cancelar_Click" />
        </div>

    </div>


    <br />
    
    <br />



</asp:Content>
