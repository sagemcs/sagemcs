<%@ Page Title="" Language="C#" MasterPageFile="~/Logged/Administradores/SiteVal.master" AutoEventWireup="true" CodeFile="ComentariosValidador.aspx.cs" Inherits="Logged_Administradores_ComentariosValidador" %>


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


            <br />
            
            <br />
            <br />
            <br />
            <div class="col-lg-12 col-sm-12 col-12" id="m4">
                <h4>Comentarios</h4>
            </div>
             <br />
            <br />
            <%--Fila del Comentarios--%>
            <div  class="row">
            <%--Comentarios Lista--%>
             <div class="col-lg-8 col-sm-8 col-xs-8">
                <h4>Comentarios previos:</h4>   
                <div class="col-lg-12 col-sm-12 col-xs-12">               
                    <asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" ID="tbx_ListaComentarios" MaxLength="1000" class="form-control" ReadOnly="true" BackColor="White" TextMode="MultiLine"/>
                </div>
             </div>             
            </div>

            <div class="row">
            <%--Comentarios--%>
            <div class="col-lg-6 col-sm-6 col-xs-6">
                <h4>Comentarios del Gasto:</h4>   
                <div class="col-lg-12 col-sm-12 col-xs-12">               
                    <asp:TextBox runat="server" type="text" AutoComplete = "off" AutoCompleteType="Disabled" ID="tbx_coment" MaxLength="256" class="form-control" ReadOnly="false" BackColor="White" />
                </div>
             </div>
             <%--Guardar Comentario--%>
            <div class="col-lg-3 col-sm-3 col-xs-3">
                <h4>Guardar Comentario:</h4>   
                <div class="col-lg-12 col-sm-12 col-xs-12">               
                    <asp:Button ID="btn_GuardarComentario" runat="server"  Class="btn btn-tsys cargar" title="Guardar"  Text="Guardar Comentario" OnClick="btn_GuardarComentario_Click" />
                </div>
            </div> 
                 <%--Guardar Comentario--%>
            <div class="col-lg-3 col-sm-3 col-xs-3">
                <h4>Cancelar:</h4>   
                <div class="col-lg-12 col-sm-12 col-xs-12">               
                    <asp:Button ID="btn_cancel" runat="server"  Class="btn btn-tsys cargar" title="Cancelar"  Text="Cancelar" OnClick="btn_cancel_Click" />
                </div>
            </div> 
            </div>
    
   
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
        <div class="alert alert-block alert-danger" id="B23" style="display: none">
            <h3>Error!</h3>
            El comentario no puede estar vacío, favor de verificar e intentar nuevamente.
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

