<%@ Page Title="Accesos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Contrarecibo.aspx.cs" Inherits="Logged_Contrarecibo" %>
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

</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href ="../Css/tables.css" rel="stylesheet" type ="text/css" />
  <asp:ScriptManagerProxy  ID="ScriptManagerProxy1" runat="server">
      <Services>
        <asp:ServiceReference  Path="~/Servicios/FacturasWebService.asmx" />
              
    </Services>
         <Scripts>
            <asp:ScriptReference  Path="~/Scripts/bootstrap-datepicker/js/bootstrap-datepicker.js" />
            <asp:ScriptReference  Path="~/Scripts/dataTable-1.10.16/datatables.min.js" />
            <asp:ScriptReference  Path="~/Scripts/dataTable-1.10.16/datatables.checkboxes.min.js" />
            <asp:ScriptReference  Path="~/Scripts/crypto-js.js" />
            <asp:ScriptReference  Path="~/Scripts/sha256.js" />
            <asp:ScriptReference  Path="~/Scripts/custom.js" />
            <asp:ScriptReference  Path="~/Scripts/data-tables-contrarecibo-facturas.js" />
        </Scripts>
      
    </asp:ScriptManagerProxy>
    
    <div class="Titulo">

        <div class="col-lg-12 col-sm-5 col-12" id="M1">
            <h3>Generar contrarecibo</h3>
        </div>
        <div class="row" >
            <div class="col-md-12">
          
                <div class="row">
                    <div class="col-md-6">
                        <div class="form-group">
                           <label>Proveedor</label>
                            <asp:DropDownList ID="comboProveedores" runat="server" CssClass="form-control select filter">
                            </asp:DropDownList>
                           
                        </div>

                    </div>

                   <div class="col-md-3">
                        <div class="form-group">
                            <label> Fecha de Recepción</label>
                            <asp:TextBox type="date" name="fecha" ID="inputFecha" AutoComplete = "off" AutoCompleteType="Disabled" min="1980-01-01" max="2050-12-31" step="1"  class="form-control"  runat="server"/>
                        </div>
                    </div>

                   <div class="col-md-3">
                        <div class="form-group">
                            <label> Fecha de Aprobación</label>
                            <asp:TextBox type="date" name="fecha" ID="inputAproba" AutoComplete = "off" AutoCompleteType="Disabled" min="1980-01-01" max="2050-12-31" step="1"  class="form-control"  runat="server"/>
                        </div>
                    </div>

                    <div class="col-md-3" >
                        <br>
                        <div class="form-group pull-left">
                            <a href="#" Class="btn btn-primary buscar" title="Buscar" data-toggle="tooltip">
                               Buscar
                            </a>
                            <a href="#" class="btn btn-tsys limpiar" title="Limpiar filtro" data-toggle="tooltip">
                                Limpiar
                            </a>
                        </div>
                    </div>

                </div>
          
            </div>


        </div>
        <div class="row" style="margin-top: 15px;">
            <div class="col-md-12">
               <span  class="pull-right" style="color: green;">
                  Importe facturas seleccionadas $<strong id="importe-total"style="margin-left:3px;width:80px;font-size: 16px;">0.00</strong>

               </span> 
            </div>
        </div>
        <div class="row" style="margin-top: 15px;">
            <div class="col-md-12">
                <table id="list" class="datatable table table-bordered table-striped" style="width:100%">
                    <thead>
                    <tr>
                        <th></th>
                        <th style="width:80px;">Compañía</th>
                        <th>Folio Factura</th>
                        <th style="width:50px;">Serie Factura</th>                        
                        <th>Fecha Recepcion Factura</th>
                        <th>Fecha Aprobacion Factura</th>
                        <th>Proveedor</th>
                        <th style="width:120px;">Subtotal</th>
                        <th style="width:50px">Retenciones</th>
                        <th style="width:auto">Traslados</th>
                        <th style="width:130px;">Total<span></span></th>
                        <th>Crédito/Débitos</th>
                        <th style="width:auto">Importe</th>
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
                    <button href="#" class="btn btn-primary generar" title="Generar contrarecibo" data-toggle="tooltip">
                        Generar contrarecibo
                    </button>
                </div>
            </div>
        </div>
       
        <asp:HiddenField ID="facturas_seleccionadas" runat="server" />

       <div class="modal fade" id="modalDetallesNota" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span
                                    aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title"
                            id="myModalLabel">Notas</h4>
                    </div>
                    <div class="modal-body">

                            <div class="container-fluid">
                                <div class="row">
                                    <div id="modal-content-table" class="col-md-12">
                                          
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal"></i>Cerrar
                            </button>
                        </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
