<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Error.aspx.cs" Inherits="Logged_Error" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CustomStyles" Runat="Server">
    <link href ="~/Css/reports.css" rel="stylesheet" type ="text/css" />
    <link rel="stylesheet" href="~/Css/bootstrap.min.css" />
    <link rel="stylesheet" href="~/Css/bootstrap-theme.min.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <!--Version 10-Abril-2019 By Luis Angel-->
    <META HTTP-EQUIV="Cache-Control" CONTENT ="no-cache">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Pragma" content="no-cache" />
 <br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/>
    <asp:Table ID="Table1" runat="server" CssClass="datatable table table-bordered table-striped" Height="80%" Width="100%">
        <asp:TableRow runat="server" BackColor="#E3E3E3" Font-Size="18px">
   
            <asp:TableCell ID="ErrorLabel" runat="server"></asp:TableCell>
   
        </asp:TableRow>
    </asp:Table>
    <br/><br/>

</asp:Content>

