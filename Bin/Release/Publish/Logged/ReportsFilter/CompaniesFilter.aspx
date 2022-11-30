<%@ Page Title="Compañias" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="CompaniesFilter.aspx.cs" Inherits="Logged_ReportsFilter_CompaniesFilter" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <!--Version 08-Abril-2019 By Luis Angel Garcia P-->
    <META HTTP-EQUIV="Cache-Control" CONTENT ="no-cache">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Pragma" content="no-cache" />
      <script src="/Scripts/dataTable-1.10.16/datatables.min.js"></script>
    <script src="/Scripts/dataTable-1.10.16/datatables.checkboxes.min.js"></script>
    <script src="/Scripts/data-tables-empresas.js"></script>
    <div class="Titulo">

        <div class="col-lg-12 col-sm-5 col-12" id="M1">
            <h3>Compañias</h3>
        </div>
        <div class="row" >
            <div class="col-md-12">
          
                <div class="row">
                    
                    <div class="col-md-5">
                        <div class="form-group">
                        <label>Nombre Compañia</label>  
                        <asp:TextBox ID="inputNombre" AutoComplete = "off" AutoCompleteType="Disabled" runat="server" CssClass="form-control"></asp:TextBox>   
                         
                        </div>

                    </div>

                    <div class="col-md-5">
                        <div class="form-group">
                            <label>R.F.C.</label>  
                            <asp:TextBox ID="inputRFC" AutoComplete = "off" AutoCompleteType="Disabled" runat="server" CssClass="form-control"></asp:TextBox>   
                         
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

                </div>
          
            </div>


        </div>
        <div class="row">
            <div class="col-md-12">
                <table id="list" class="datatable table table-bordered table-striped" width="100%">
                    <thead>
                    <tr>
                        <th></th>
                        <th>Nombre Compañia</th>
                        <th>RFC</th>
                        <th>Fecha de Registro</th>
                        <th>Estado</th>
                    </tr>
                    </thead>
                    <tbody>

                    </tbody>
                </table>
            </div>
        </div>
        <div class="row">
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