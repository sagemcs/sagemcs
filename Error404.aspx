﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Error404.aspx.cs" Inherits="Upsi" %>

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
        <h1>404 Página no encontrada</h1>
    </div>

    <%--<b>La página ha dejado de trabajar</b>--%>
    <br>
    <h3>La página que intentas solicitar no está en el servidor.</h3>
    <h3>Verifica la URL.</h3>
</body>
</html>
