<%@ Page Title="Accesos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="DocumentosProveedoresFilter.aspx.cs" Inherits="Logged_ReportsFilter_DocumentosProveedoresFilter" %>
<script runat="server">
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        this.MasterPageFile = Tools.GetMasterPage();
    }
</script>
<asp:Content ID="CustomStyles" ContentPlaceHolderID="CustomStyles" runat="server">
    <!--Version 08-Abril-2019 By Luis Angel Garcia P-->
    <META HTTP-EQUIV="Cache-Control" CONTENT ="no-cache">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Pragma" content="no-cache" />
<%--    <link href="../../Css/reports-filter.css" rel="stylesheet" />--%>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
         <Services>
                <asp:ServiceReference Path="~/Servicios/DocumentosProveedoresWebService.asmx" /> 
         </Services>
         <Scripts>
            <asp:ScriptReference  Path="~/Scripts/dataTable-1.10.16/datatables.min.js" />
            <asp:ScriptReference   Path="~/Scripts/dataTable-1.10.16/datatables.checkboxes.min.js" />
            <asp:ScriptReference   Path="~/Scripts/crypto-js.js" />
            <asp:ScriptReference   Path="~/Scripts/sha256.js" />
            <asp:ScriptReference   Path="~/Scripts/custom.js" />
             <asp:ScriptReference   Path="~/Scripts/data-tables-documentos-proveedores.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
     
    <div class="Titulo">

        <div class="col-lg-12 col-sm-5 col-12" id="M1">
            <h3>Documentos de proveedores</h3>
        </div>
        <div class="row" >
            <div class="col-md-12">
          
                <div class="row">
                   
                   <div class="col-md-4">
                    
                        <div class="form-group">                     
                        <asp:TextBox ID="inputSocial" MaxLength="50"  placeholder="Razón Social" runat="server" CssClass="form-control filter" AutoComplete = "off" AutoCompleteType="Disabled" ToolTip="Razón social"></asp:TextBox>   
                        </div>
                    </div>
                   
                    <div class="col-md-2">
                        <div class="form-group">
                             <asp:DropDownList ID="comboEstado" runat="server"  CssClass="form-control select2 filter">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-md-2">
                        <div class="form-group pull-left">
                            <a href="#" Class="btn btn-primary buscar" title="Buscar" data-toggle="tooltip">
                               Buscar
                            </a>
                            <a href="#" class="btn btn-tsys limpiar" title="Limpiar filtro" data-toggle="tooltip">
                                Limpiar
                            </a>
          
                        </div>
                    </div>

                   <div class="col-md-1" style="visibility:hidden">
                        <div class="form-group">                            
                        <asp:TextBox ID="inputID" MaxLength="5"  placeholder="ID" runat="server" CssClass="form-control filter" ToolTip="ID"></asp:TextBox>   
                        </div>
                    </div>

                   <div class="col-md-1" style="visibility:hidden">
                        <div class="form-group">                            
                        <asp:TextBox ID="inputNombre" MaxLength="12"  placeholder="Nombre" runat="server" CssClass="form-control filter" ToolTip="Nombre"></asp:TextBox>   
                        </div>
                    </div>

                    <a href="../../Css/">../../Css/</a>
                </div>
          
            </div>


        </div>
        <div class="row">
            <div class="col-md-12">
                <table id="list" class="datatable table table-bordered table-striped" width="100%">
                    <thead>
                    <tr>             
                        <th></th>                     
                        <th>Razón social</th>                       
                        <th>Fecha de Aprobación</th>
                        <th>Descripción</th>
                        <th>Estado</th>
                    </tr>
                    </thead>
                    <tbody>

                    </tbody>
                </table>
            </div>
        </div>
        <div class="row" style="margin-top: 15px;">
            <div class="col-md-12">
                <div class="pull-right">
                    <button href="#" class="btn btn-primary generar" title="Generar reporte" data-toggle="tooltip">
                        Generar reporte
                    </button>
                </div>
            </div>
        </div>
    </div>

</asp:Content>


