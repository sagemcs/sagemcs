<%@ Page Title="Tipo De Cambio" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="TipoC.aspx.cs" Inherits="Logged_Administradores_TipoC" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <meta HTTP-EQUIV="Cache-Control" CONTENT ="no-cache">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="Last-Modified" content="0">
    <meta http-equiv="Content-Type" content="text/html; charset=ISO-8859-1">
    <meta http-equiv="Cache-Control" content="no-cache, mustrevalidate">
    <script src ="../../Css/sweetalert.min.js" type="text/javascript"></script>
    <script src ="../../Scripts/jquery.blockui.min.js" type ="text/javascript"></script>
    <script src ="../../Scripts/custom.js" type ="text/javascript"></script>

    <script src ="../../Scripts/TipoC.js" type ="text/javascript"></script>--%>
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />


       <div class="col-lg-11 col-sm-11 col-xs-11">
        <label class="col-form-label col-lg-5 col-sm-3 col-xs-1"></label>
        <div class="form-group col-lg-6 col-sm-8 col-xs-10">
            <h1>Tipo de Cambio Banxico</h1>
        </div>
      </div>
    
<%--     <div class="col-xs-6 md-4 col-lg-3">
          <label>Seleccione la Fecha:</label>
        <asp:TextBox type="date" name="fecha" ID="txtdtp" AutoComplete = "off" AutoCompleteType="Disabled" min="1980-01-01" max="2050-12-31" step="1"  class="form-control"  runat="server"/>
      </div>--%>

    <div class="col-xs-11 col-sm-11 col-md-11">
        <div class="form-group row">
            <label class="col-form-label col-xs-12 col-sm-12 col-md-12"></label>
            <div class="col-xs-4 col-sm-2 col-md-1">
                <asp:Button ID="Buscar" runat="server" Text="Obtener" OnClick="Obtener" CssClass="btn btn-primary" title="Generar Busqueda" />
            </div>
        </div>
    </div>




</asp:Content>
