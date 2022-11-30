<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ErrorRedirect.aspx.cs" Inherits="Upsi" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Error</title>
    <link href="~/Css/errorpage.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <%--                <div>
                    <a runat="server" href="~/">
                    <img src="/Img/Lgtys.png" style="border:0px solid black;"/>
                    </a>
                </div>--%>
    <div class="navbar navbar-inverse navbar-fixed-top" id="Portada">
        <div class="container">
            <div class="navbar-header">
                <%-- <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
                    </button> --%>
                <a runat="server">
                    <img src="/Img/Lgtys.png" style="border: 0px solid black;" />
                </a>
            </div>
        </div>
    </div>
    <div class="ti1" id="M1">
        <h1>Ha ocurrido un error</h1>
    </div>

    <%--<b>La página ha dejado de trabajar</b>--%>
    <br>
    <h3>El servidor tuvo un error interno y no pudo completar su petición.</h3>
    <h3>Disculpe los inconvenientes causados.</h3>
</body>
</html>
