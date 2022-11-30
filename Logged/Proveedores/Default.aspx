<%@ Page Title="Home Page" Language="C#" MasterPageFile="MenuPreP.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%--<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">--%>
<%--        <br />

</asp:Content>--%>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    Version 16-Octubre-2019 By Luis Angel Garcia P
    <META HTTP-EQUIV="Cache-Control" CONTENT ="no-cache">
    <meta http-equiv="Expires" content="0" />
    <meta http-equiv="Pragma" content="no-cache" />
    <script src ="../../Css/sweetalert.min.js" type="text/javascript"></script>
    <script src ="../../Css/sweetalert2.all.min.js" type="text/javascript"></script>


    <script>
        function alertme(titulo,mesaje,Tipo) {
            swal(titulo, mesaje, Tipo)
        }
    </script>

    <script>
           function Varios() {
                    swal.mixin({
                        input: 'text',
                        confirmButtonText: 'Ok',
                        showCancelButton: true,
                        customClass: 'swal-wide'
                    }).queue([
                        {
                            title: 'Notificación de actualización de documentos 1',
                            text: 'Alguno de tus Documentos se encuentra en revisión, en los próximos días se te enviara un Email con el resultado de la aprobación te recomendamos estar pendiente',
                            icon: 'info',
                        },
                        {
                            title: 'Notificación de actualización de documentos 2',
                            text: 'Texto 5',
                            icon: 'error',
                        },
                        {
                            title: 'Question 3',
                            text: 'Chaining swal2 modals is easy3',
                            icon: 'warning',
                        }
                    ]).then((result) => {
                        if (result.value) {
                            const answers = JSON.stringify(result.value)
                            Swal.fire({
                                title: 'All done!',
                                customClass: 'swal-wide',
                                html: `Your answers:<pre><code>${answers}</code></pre>`,
                                confirmButtonText: 'Lovely!'
                            })
                        }
                    })
                }
    </script>

    <script>
        function Varies() {
            swal.mixin({
                confirmButtonText: 'Ok',
                showCancelButton: false,
                customClass: 'swal-wide'
            }).queue([
                { title: 'Notificación de actualización de documentos', text: 'Alguno de tus documentos se encuentra en revisión, en los próximos días se te enviara un Email con el resultado de la aprobación te recomendamos estar pendiente', icon: 'warning', }, { title: 'Notificación de actualización de documentos', text: ' Texto 2', icon: 'error', }, 
            ])
        }
        </script>
        
    <script>
        function Varies(Tez,teza,ok) {
            swal.mixin({
                confirmButtonText: 'Ok',
                showCancelButton: false,
                customClass: 'swal-wide'
            }).queue([
                {
                    Tez
                },
                {
                    teza
                },
                {
                    ok
                },                
            ])
        }
    </script>

    <script>
            function Varis(Texto) {
                Texto
            }
    </script>

     <asp:LoginView runat="server" ViewStateMode="Enabled">
     
           <LoggedInTemplate>
                <asp:ScriptManagerProxy ID="ScriptManagerProxy1"  runat="server">
                    <Services>
                        
           
                    </Services>
                    <Scripts>   
                        
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


       
        <div class="col-lg-12 col-sm-5 col-12">
            <br /><br /><br /><br /><br /><br /><br /><br /><br />
            <h4>Descarga el formato carta de no adeudo</h4>
            <asp:LinkButton runat="server" type="button" ToolTip ="Limpiar" Font-Size="X-Large" OnClick="DownloadFile" class="btn btn-tsys" ID="LinkButton3">                                        
            <span aria-hidden="true" class="glyphicon glyphicon-cloud-download"></span>
            </asp:LinkButton>
        </div>
        <%--<a href="../Contrarecibo.aspx">../Contrarecibo.aspx</a>--%>

        <div class="col-lg-12 col-sm-5 col-12">
            <br />
            <h4>Descarga constancia de situación fiscal</h4>
            <%--<asp:LinkButton runat="server" type="button" ToolTip ="Limpiar" Font-Size="X-Large" OnClick="DownloadCFiscal" class="btn btn-tsys" ID="LinkButton4">                                        
            <span aria-hidden="true" class="glyphicon glyphicon-cloud-download"></span>
            </asp:LinkButton>--%>
            <%--<asp:Button runat="server" type="submit" formaction="https://multiconsulting.com.mx/SAT10072022.pdf" class="btn btn-tsys" ID="LinkButton4" Font-Size="X-Large" ToolTip ="Limpiar">
            </asp:Button>--%>
            <%--<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="https://multiconsulting.com.mx/SAT10072022.pdf" Target="_blank">
                This will work.
            </asp:HyperLink>--%>
            <a href="https://multiconsulting.com.mx/SAT10072022.pdf" target="_blank" ToolTip ="Limpiar" Font-Size="X-Large">
                <input type="button" value="Descargar" class="btn btn-tsys"/>
            </a>
            <br /><br /><br /><br />
        </div>
    </div>
</asp:Content>
