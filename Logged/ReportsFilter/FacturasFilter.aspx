<%@ Page Title="Accesos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="FacturasFilter.aspx.cs" Inherits="Logged_ReportsFilter_FacturasFilter" %>
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
    <link href="../../Css/tables.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
         <Services>
                <asp:ServiceReference Path="~/Servicios/FacturasWebService.asmx" /> 
         </Services>
            <Scripts>
                <asp:ScriptReference  Path="~/Scripts/bootstrap-datepicker/js/bootstrap-datepicker.js" />
                <asp:ScriptReference  Path="~/Scripts/dataTable-1.10.16/datatables.min.js" />
                <asp:ScriptReference   Path="~/Scripts/dataTable-1.10.16/datatables.checkboxes.min.js" />
                <asp:ScriptReference   Path="~/Scripts/crypto-js.js" />
                <asp:ScriptReference   Path="~/Scripts/sha256.js" />
                <asp:ScriptReference   Path="~/Scripts/custom.js" />
                <asp:ScriptReference   Path="~/Scripts/data-tables-facturas.js" />
            </Scripts>
    </asp:ScriptManagerProxy>
  
    <div class="Titulo">

        <div class="col-lg-12 col-sm-5 col-12" id="M1">
            <h3>Facturas</h3>
        </div>
        <div class="row" >
            <div class="col-md-12">
          
                <div class="row">
                    <div class="col-md-3">
                    
                        <div class="form-group">
                        <label>Folio de Factura</label> 
                       <asp:TextBox ID="inputFolio" MaxLength="40" AutoComplete = "off" AutoCompleteType="Disabled" runat="server" CssClass="form-control filter" ToolTip="Folio"></asp:TextBox>   
                         
                            </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                             <label>Serie de Factura</label>
                        <asp:TextBox ID="inputSerie" MaxLength="25" AutoComplete = "off" AutoCompleteType="Disabled" runat="server" CssClass="form-control filter" ToolTip="Serie"></asp:TextBox>   
                         
                        </div>

                    </div>

                    <div class="col-md-3">
                        <div class="form-group">
                        <label> Fecha de Factura</label>
                        <asp:TextBox type="date" ID="inputFecha" AutoComplete = "off" AutoCompleteType="Disabled" min="1980-01-01" max="2050-12-31" CssClass="form-control filter"  runat="server"/>
                        </div>
                    </div>

                   <div class="col-md-3">
                        <div class="form-group">
                        <label> Fecha de Carga</label>
                        <asp:TextBox type="date" ID="inputFechaR" AutoComplete = "off" AutoCompleteType="Disabled" min="1980-01-01" max="2050-12-31" CssClass="form-control filter"  runat="server"/>
                        </div>
                    </div>
                    
                   <div class="col-md-3">
                        <div class="form-group">
                             <label> Total</label>
                            <asp:TextBox ID="inputTotal" AutoComplete = "off" AutoCompleteType="Disabled" runat="server" CssClass="form-control filter"  ToolTip="Total"></asp:TextBox>      
                        <span id="error_inputTotal" class="no-valid-message">Dato no válido</span>
                        </div>
                    </div>
                    
                    
                    <div class="col-md-3">
                        <div class="form-group">
                             <label>Proveedor</label>
                            <asp:DropDownList ID="comboProveedores" runat="server" CssClass="form-control select2 filter">
                            </asp:DropDownList>
                           
                        </div>

                    </div>

                   <div class="col-md-3">
                        <div class="form-group">
                             <label>Estado</label>
                            <asp:DropDownList ID="comboEstado" runat="server" CssClass="form-control select2 filter">
                            </asp:DropDownList>
                        </div>
                    </div>

                   <div class="col-md-3" >
                        <div class="form-group pull-left">
                            <br />
                            <a href="#" Class="btn btn-primary buscar" title="Buscar" onclick="Test(event, this)" data-toggle="tooltip">
                               Buscar
                            </a>
                            <a href="#" class="btn btn-tsys limpiar" title="Limpiar filtro" data-toggle="tooltip">
                                Limpiar
                            </a>
          
                        </div>
                    </div>

                </div>
          
            </div>
            
            <div class="col-md-3" style="visibility :hidden">
              <div class="form-group">
                <asp:TextBox ID="inputUUID" AutoComplete = "off" AutoCompleteType="Disabled" MaxLength="36"  placeholder="UUID" runat="server" CssClass="form-control filter" ToolTip="UUID"></asp:TextBox>   
              </div>
            </div>

        </div>
        <div class="row">
            <div class="col-md-12">
                <table id="list" class="datatable table table-bordered table-striped" width="100%">
                    <thead>
                    <tr>
                        <th></th>
                        <th>Proveedor</th>
                        <th>Folio de Factura</th>
                        <th>Serie de Factura</th>                        
                        <th>Fecha de Carga</th>
                        <th>Fecha de Factura</th>                        
                        <th>Subtotal</th>
                        <th>Traslados</th>
                        <th>Total</th>
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
