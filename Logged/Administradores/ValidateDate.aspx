<%@ Page Language="C#" Title="Debug" AutoEventWireup="true"  MasterPageFile="~/Site.Master" CodeFile="ValidateDate.aspx.cs" Inherits="Logged_Administradores_ValidateDate" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script>
        function alert(campo) {
            $(campo).toggle();
        }
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
                });
            });
            return true;
        })
    </script>

    <script>
        function alerte(campo) {
         $(campo).show("slow").delay(2000).hide("slow")
        }
    </script>

    <div class="col-lg-12 col-sm-5 col-12" id="M1">
      <h3>Depuración Portal Proveedores</h3>
    </div>

<%--    <asp:UpdatePanel runat="server" id="UpdatePanel" updatemode="Conditional">
     <triggers>
     </triggers>
    <contenttemplate>--%>

        <div class="col-xs-10 md-6 col-lg-3">
        <h5><strong>Ejecurtar Sobre :</strong></h5>
        <asp:DropDownList ID="SolCont" class="selectpicker show-tick form-control" data-live-search="true" AutoPostBack ="false" data-style="btn-primary" runat="server" >
        <asp:ListItem Value="1">mas500_u_app</asp:ListItem>
        <asp:ListItem Value="2">PortalProveedoresProd</asp:ListItem>
        <asp:ListItem Value="3">Sage500_app</asp:ListItem>
        <asp:ListItem Value="4">PortalProveedoresVal</asp:ListItem>


<%--        <asp:ListItem Value="1">Sage500_app</asp:ListItem>
        <asp:ListItem Value="2">PortalProveedoresProd</asp:ListItem>
        <asp:ListItem Value="3">Sage500_Portal</asp:ListItem>
        <asp:ListItem Value="4">PortalProveedores</asp:ListItem>--%>
        </asp:DropDownList>
        </div>

        <div class="col-lg-4 col-sm-6 col-xs-10">
            <h5>Ingresa tu Archivo</h5>
            <div class="input-group col-lg-12 col-sm-12 col-xs-12">
                <label class="input-group-btn">
                    <span class="btn btn-primary">
                        Seleccionar&hellip; <asp:FileUpload type="file" ID="FileUpload" runat=server accept=".sql" style="display: none;" />
                    </span>
                </label>
                <asp:TextBox type="text" runat="server" class="form-control" id="PNotarial" readonly="true" />
            </div>
        </div>

        <div class="col-xs-12 md-6 col-lg-3" style="margin-top:31px;">
            <span class="col-lg-12 col-md-12 col-xs-12"></span>
          <div class="col-xs-4 col-sm-6 col-md-5">
            <asp:Button Text="Ejecutar" id="Run" runat="server" CssClass="btn btn-success" title="Ejecutar Consulta" OnClick="CargarDatos" />
          </div>

          <div class="col-xs-4 col-sm-6 col-md-6">
            <asp:Button ID="Button2" runat="server" Text="Limpiar" OnClick="Limpiar_Click1" CssClass="btn btn-danger" title="Limpiar Busqueda" />
          </div>

        </div>

                <div class="col-xs-10 md-11 col-lg-12">
            <br />
            <asp:TextBox runat="server" Id="Consulta" TextMode="MultiLine" style="display: none;" type="text" class="form-control"></asp:TextBox>
        </div>

        <div class="col-xs-12 md-10 col-lg-10">
        <br />
        <div class="alert alert-danger"  id="AL"  style="display: none">
         <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                <span aria-hidden="true">&times;</span>
         </button>
         <h5>¡Error!</h5>
           <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
        </div>
        </div>

        <div class="col-xs-12 md-10 col-lg-10">
        <br />
        <div  class="alert alert-warning" role="alert" id="AL2"  style="display: none">
         <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                <span aria-hidden="true">&times;</span>
         </button>
         <h5>¡Error!</h5>
           <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
        </div>
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
        
        <asp:GridView ID="Tabla" runat="server"
         CssClass="table table-bordered bs-table"
         margin-left="auto" margin-right="auto">

         <AlternatingRowStyle BackColor="White" />
         <HeaderStyle BackColor="#337ab7" Font-Bold="True" ForeColor="White" />
         <EditRowStyle BackColor="#ffffcc" />
         <EmptyDataRowStyle ForeColor="Red" CssClass="table table-bordered" />

         <FooterStyle BackColor="#507CD1" ForeColor="White" Font-Bold="True" />
         <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
         <RowStyle CssClass="table table-bordered" BackColor="#EFF3FB" />
         <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />

         <SortedAscendingCellStyle BackColor="#F5F7FB" />
         <SortedAscendingHeaderStyle BackColor="#6D95E1" />
         <SortedDescendingCellStyle BackColor="#E9EBEF" />
         <SortedDescendingHeaderStyle BackColor="#4870BE" />
        </asp:GridView>

<%--    </contenttemplate>
    </asp:UpdatePanel>--%>
</asp:Content>