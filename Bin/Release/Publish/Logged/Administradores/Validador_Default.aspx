<%@ Page Title="" Language="C#" MasterPageFile="~/Logged/Administradores/SiteVal.master" AutoEventWireup="true" CodeFile="Validador_Default.aspx.cs" Inherits="Logged_Administradores_Validador_Default" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    Version 15-Enero-2019 By Luis Angel
    <META HTTP-EQUIV="Cache-Control" CONTENT ="no-cache">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Pragma" content="no-cache" />

     <asp:LoginView runat="server" ViewStateMode="Enabled">
     
           <LoggedInTemplate>
                <asp:ScriptManagerProxy ID="ScriptManagerProxy1"  runat="server">
                    <Services>
                        <asp:ServiceReference Path="~/Servicios/NotificacionesWebService.asmx" />
           
                    </Services>
                    <Scripts>   
                        <asp:ScriptReference   Path="~/Scripts/custom.js" />
                        <asp:ScriptReference  Path="~/Scripts/notificaciones.js" />
                    </Scripts>
                </asp:ScriptManagerProxy>
            </LoggedInTemplate>
          </asp:LoginView>
       <%-- <asp:Image ID="Image1" runat="server" Height="97px" Width="181px" ImageUrl="~/Img/TSYS.png" />--%>
      
    
    <div class="Titulo">

        <div class="col-lg-12 col-sm-5 col-12" id="M1">
            <h3></h3>
        </div>
       
        <div class="row" >
            <div class="col-md-12">
                  <asp:LoginView runat="server" ViewStateMode="Enabled">
                        <AnonymousTemplate>
                            <div class="center-block"><h1>Pagina de Inicio</h1></div>
                           
                        </AnonymousTemplate>
                        <LoggedInTemplate>
                                  <table class="datatable table table-bordered table-striped" style="margin: 0 auto;" width="80%">
                                 <thead>
                                <tr>
                                  <td style="text-align:center;background-color:rgba(227,227,227,1)"><h4>Notificaciones</h4></td>
                                </tr>
                                </thead>
                                <tbody  id="noticicaciones-container">

                                </tbody>
                            </table>

                        </LoggedInTemplate>
                    </asp:LoginView>

          
            </div>


        </div>

    </div>

</asp:Content>

