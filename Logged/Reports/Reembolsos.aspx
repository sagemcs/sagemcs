<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Reembolsos.aspx.cs" Inherits="Logged_Reports_Reembolsos" %>
<script runat="server">
    protected void Page_PreInit(Object sender, EventArgs e)
    {
        this.MasterPageFile = Tools.GetMasterPage();
    }
</script>
<%@ Register assembly="CrystalDecisions.Web, Version=13.0.3500.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CustomStyles" Runat="Server">
    <!--Version 31-Mayo-2019 By Luis Angel-->
    <META HTTP-EQUIV="Cache-Control" CONTENT ="no-cache">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Pragma" content="no-cache" />
    <link href ="../../Css/reports.css" rel="stylesheet" type ="text/css" />
    <link rel="stylesheet" href="~/Css/bootstrap.min.css" />
    <link rel="stylesheet" href="~/Css/bootstrap-theme.min.css" />
    <script src="../../Scripts/jquery-3.3.1.min.js" type="text/javascript"></script>
    <script src="../../Scripts/print-report.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <br/> <br/> <br/> <br /> <br/>
    <div class="row">
        <div class="col-lg-3 col-sm-3 col-xs-3">
            <input id="btn_print" class="btn btn-primary" type="button" value="Imprimir" /> 
        </div>
        <div class="col-lg-3 col-sm-3 col-xs-3">
            <asp:Button runat="server" id="btn_back" class="btn btn-primary" type="button" Text="Regresar a Reembolsos" OnClick="btn_back_Click"  /> 
        </div>
    </div>
    <br/>
         <div id = "dvReport"  style="position: relative; left: 5%;" > 
            <CR:CrystalReportViewer ID="Reporte_Reembolsos" runat="server" AutoDataBind="true" BestFitPage="False" DisplayStatusbar="False" HasCrystalLogo="False" HasDrilldownTabs="False" HasToggleGroupTreeButton="False" HasToggleParameterPanelButton="False" ToolPanelView="None" ClientIDMode="Inherit" PrintMode="pdf" HasPrintButton="True" />
        </div>
<script>
        $(document).ready(function () {
            $('#Reporte_Facturas_toptoolbar_print').click(function (e) {
                e.preventDefault();
                Print();
            });

            $('#btn_print').click(function (e) {
                e.preventDefault();
                Print();
            });
        });
</script>
</asp:Content>
