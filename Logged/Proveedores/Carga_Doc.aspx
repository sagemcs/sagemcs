<%@ Page title="Subir Documentos" Language="C#" AutoEventWireup="true"  MasterPageFile="MenuPreP.master" CodeFile="Carga_Doc.aspx.cs" Inherits="Carga_Doc" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    Version 16-Octubre-2019 By Luis Angel Garcia P
    <script src ="../../Css/sweetalert.min.js" type="text/javascript"></script>
    <script src ="../../Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script src ="../../Scripts/custom.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            // We can attach the `fileselect` event to all file inputs on the page
            $(document).on('change', ':file', function () {
                var input = $(this),
                    numFiles = input.get(0).files ? input.get(0).files.length : 1,
                    label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
                input.trigger('fileselect', [numFiles, label]);
            });

            // We can watch for our custom `fileselect` event like this
            $(document).ready(function () {
                $(':file').on('fileselect', function (event, numFiles, label) {

                    var input = $(this).parents('.input-group').find(':text'),
                        log = numFiles > 1 ? numFiles + ' files selected' : label;

                    if (input.length) {
                        input.val(log);
                    } else {
                        if (log) alert(log);
                    }
                });
            });
            return true;
        })
    </script>

    <script>
        function alertme(titulo,mesaje,Tipo) {
            swal(titulo, mesaje, Tipo)
            unblockUI();
        }
    </script>

    <script>
        function alerte(campo) {
        $(campo).show("slow").delay(2000).hide("slow")
        }
    </script>

    <script>
        function Alerta(Doc) {
            alert(Doc);
            unblockUI();
        }
    </script>

    <div class="Titulo">

    <div class="col-lg-12 col-sm-12 col-12" id="M1">
        <h3>Carga de Documentos</h3>
    </div>


          <div class="col-lg-12 col-sm-12 col-12">
            <asp:DropDownList ID="UsuarioP" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server" Width="30%" AutoPostBack ="true" OnSelectedIndexChanged="UsuarioP_SelectedIndexChanged"></asp:DropDownList>
            <label class="col-form-label col-sm-2"></label>
        <br />
        <br />
    </div>
         <asp:Panel runat ="server" id="Info">


<%--            <asp:UpdatePanel runat="server" ID="Panel1">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnEnv" EventName="Click" />
                <asp:PostBackTrigger ControlID="GridView1"/>
            </Triggers> 
            
            <ContentTemplate>--%>

        <div class="row">
            <asp:GridView ID="GridView1" CssClass="table table-bordered bs-table"
                margin-left="auto" margin-right="auto" runat="server"
                AutoGenerateColumns="False"
                OnRowCreated="GridView1_RowCreated"
                OnRowDataBound="GridView1_RowDataBound"
                OnRowCommand="GridView1_RowCommand" 
                >

                <AlternatingRowStyle BackColor="White" />

                <HeaderStyle BackColor="#337ab7" Font-Bold="True" ForeColor="White" />
                <EditRowStyle BackColor="#ffffcc" />
                <EmptyDataRowStyle ForeColor="Red" CssClass="table table-bordered" />

                <Columns>
                    <asp:BoundField DataField="Nombre" HeaderText="Archivo" HeaderStyle-Width="120px" ReadOnly="True">
                        <HeaderStyle Width="120px"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="NombreFile" HeaderText="Archivo Recibido" HeaderStyle-Width="40px" ReadOnly="True">
                        <HeaderStyle Width="40px"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="Fecha" HeaderText="Fecha Subida" HeaderStyle-Width="80px" ReadOnly="True">
                        <HeaderStyle Width="80px"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="Fecha2" HeaderText="Fecha Aceptación" HeaderStyle-Width="80px" ReadOnly="True">
                        <HeaderStyle Width="80px"></HeaderStyle>
                    </asp:BoundField>
                    <asp:BoundField DataField="Status" HeaderText="Status" HeaderStyle-Width="78px" ReadOnly="True">
                        <HeaderStyle Width="78px"></HeaderStyle>
                    </asp:BoundField>
                  <asp:TemplateField  ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="30px">
                  <ItemTemplate>

                   <div class="input-group col-lg-12 col-sm-12 col-xs-12" id="Cajass" runat ="server">
                    <label class="input-group-btn">
                        <span class="btn btn-primary glyphicon glyphicon-cloud-upload" id="Btn" runat ="server">
                            <asp:FileUpload type="file" ID="BDoc" runat="server" Style="display: none;" accept="application/pdf" />
                        </span>
                    </label>
                    <asp:TextBox type="text" runat="server" class="form-control" ID="CDoc" ReadOnly="true" />
                </div>
                  </ItemTemplate> 
                     <HeaderStyle Width="30px" />
                     <ItemStyle HorizontalAlign="Center" />
                 </asp:TemplateField>

                </Columns>

                <FooterStyle BackColor="#507CD1" ForeColor="White" Font-Bold="True" />
                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle CssClass="table table-bordered" BackColor="#EFF3FB" />
                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />

                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                <SortedDescendingHeaderStyle BackColor="#4870BE" />
            </asp:GridView>
            <br />
        </div>

             <div class="row">
                 <label class="col-form-label col-lg-11 col-sm-11 col-xs-1"></label>
                 <div class="col-xs-3 col-md-1 col-lg-1">
                    <asp:Button Text="Enviar" runat="server" ID="btnEnv" OnClick="Nuevos" CssClass="btn btn-tsys cargar" title="Enviar Documentos" />
                 </div>
             </div>
<%--    </ContentTemplate>
    </asp:UpdatePanel>--%>



    </asp:Panel>



 

    <asp:Panel runat="server" id="Carga">


            <div class="col-lg-12 col-sm-12 col-12" >
            <div class="alert alert-block alert-danger" id="B1" style="display: none">
                <h3>Error!</h3>
                Debe Cargar Todos los Documentos, es Obligatorio
            </div>
            <div class="alert alert-block alert-danger" id="B2" style="display: none">
                <h3>Error!</h3>
                El Documento Acta Constitutiva, excede el Tamaño Permitido 15 Mb
            </div>
            <div class="alert alert-block alert-danger" id="B3" style="display: none">
                <h3>Error!</h3>
                El Documento Registro Federal del Contribuyente, excede el Tamaño Permitido 15 Mb
            </div>
            <div class="alert alert-block alert-danger" id="B4" style="display: none">
                <h3>Error!</h3>
                El Documento Poder Notarial, excede el Tamaño Permitido 15 Mb
            </div>
            <div class="alert alert-block alert-danger" id="B5" style="display: none">
                <h3>Error!</h3>
                El Documento Identificación Oficial, excede el Tamaño Permitido 15 Mb
            </div>
            <div class="alert alert-block alert-danger" id="B6" style="display: none">
                <h3>Error!</h3>
                El Documento Opinion de Cumplimiento de Obligaciones Fiscales, excede el Tamaño Permitido 15 Mb
            </div>
            <div class="alert alert-block alert-danger" id="B7" style="display: none">
                <h3>Error!</h3>
                El Documento Carta Instrucción, excede el Tamaño Permitido 15 Mb
            </div>
            <div class="alert alert-block alert-danger" id="B8" style="display: none">
                <h3>Error!</h3>
                El Documento Contrato y/o Carta de Prestación, excede el Tamaño Permitido 15 Mb
            </div>
            <div class="alert alert-block alert-danger" id="B11" style="display: none">
                <h3>Error!</h3>
                El Documento Carta de No Adeudo, excede el Tamaño Permitido 15 Mb
            </div>
            <div class="alert alert-block alert-danger" id="B12" style="display: none">
                <h3>Error!</h3>
                El Documento Terminos y Condiciones, excede el Tamaño Permitido 15 Mb
            </div>
            <div class="alert alert-block alert-danger" id="B9" style="display: none">
                <h3>Error!</h3>
                <asp:Label ID="Label4" runat="server" ></asp:Label>
            </div>
            </div>

            <div class="row">

            <div class="col-lg-6 col-sm-6 col-xs-10">
            <h4>Acta Constitutiva.</h4>
            <div class="input-group col-lg-10 col-sm-10 col-xs-10">
                <label class="input-group-btn">
                   <span class="btn btn-primary">
                        Seleccionar&hellip;<asp:FileUpload type="file" ID="FileUpload1" class="FliUp" runat=server style="display: none;" accept="application/pdf" />
                    </span>
                </label>
                <asp:TextBox type="text" runat="server" class="form-control" id="Acta" readonly="true" />
            </div>
                 <small class="form-text text-muted">Archivo Max 15 Mb.</small>
             </div>

            <div class="col-lg-6 col-sm-6 col-xs-10">
            <h4>Constancia de Situacion Fiscal.</h4>
            <div class="input-group col-lg-10 col-sm-10 col-xs-10">
                <label class="input-group-btn">
                    <span class="btn btn-primary">
                        Seleccionar&hellip; <asp:FileUpload type="file" ID="FileUpload2" runat=server style="display: none;" accept="application/pdf" />
                    </span>
                </label>
                <asp:TextBox type="text" runat="server" class="form-control" id="RFC" readonly="true" />
            </div>
                 <small class="form-text text-muted">Archivo Max 15 Mb.</small>
             </div>

            <div class="col-lg-6 col-sm-6 col-xs-10">
            <h4>Poder Notarial</h4>
            <div class="input-group col-lg-10 col-sm-10 col-xs-10">
                <label class="input-group-btn">
                    <span class="btn btn-primary">
                        Seleccionar&hellip; <asp:FileUpload type="file" ID="FileUpload3" runat=server style="display: none;" accept="application/pdf" />
                    </span>
                </label>
                <asp:TextBox type="text" runat="server" class="form-control" id="PNotarial" readonly="true" />
            </div>
                 <small class="form-text text-muted">Archivo Max 15 Mb.</small>
             </div>

            <div class="col-lg-6 col-sm-6 col-xs-10">
            <h4>Identificación Oficial</h4>
            <div class="input-group col-lg-10 col-sm-10 col-xs-10">
                <label class="input-group-btn">
                    <span class="btn btn-primary">
                        Seleccionar&hellip; <asp:FileUpload type="file" ID="FileUpload4" runat=server style="display: none;" accept="application/pdf" />
                    </span>
                </label>
                <asp:TextBox type="text" runat="server" class="form-control" id="ID" readonly="true" />
            </div>
                 <small class="form-text text-muted">Archivo Max 15 Mb.</small>
             </div>

            <div class="col-lg-6 col-sm-6 col-xs-10">
                <h4>Opinion de Cumplimiento de Obligaciones Fiscales</h4>
                <div class="input-group col-lg-10 col-sm-10 col-xs-10">
                    <label class="input-group-btn">
                        <span class="btn btn-primary">Seleccionar&hellip;
                            <asp:FileUpload type="file" ID="FileUpload5" runat="server" Style="display: none;" accept="application/pdf" />
                        </span>
                    </label>
                    <asp:TextBox type="text" runat="server" class="form-control" ID="SAT" ReadOnly="true" />
                </div>
                <small class="form-text text-muted">Archivo Max 15 Mb.</small>
            </div>

            <div class="col-lg-6 col-sm-6 col-xs-10">
                <h4>Carta Instrucción</h4>
                <div class="input-group col-lg-10 col-sm-10 col-xs-10">
                    <label class="input-group-btn">
                        s
                        <span class="btn btn-primary">Seleccionar&hellip;
                            <asp:FileUpload type="file" ID="FileUpload6" runat="server" Style="display: none;" accept="application/pdf" />
                        </span>
                    </label>
                    <asp:TextBox type="text" runat="server" class="form-control" ID="Carta" ReadOnly="true" />
                </div>
                <small class="form-text text-muted">Archivo Max 15 Mb.</small>
            </div>

            <div class="col-lg-6 col-sm-6 col-xs-10" id="CContrato">
            <h4>Contrato y/o Carta de Prestación</h4>
            <div class="input-group col-lg-10 col-sm-10 col-xs-10">
                <label class="input-group-btn">
                    <span class="btn btn-primary">
                        Seleccionar&hellip; <asp:FileUpload type="file" ID="FileUpload7" runat=server style="display: none;" accept="application/pdf" />
                    </span>
                </label>
                <asp:TextBox type="text" runat="server" class="form-control" id="Contrato" readonly="true" />
            </div>
                 <small class="form-text text-muted">Archivo Max 15 Mb.</small>
             </div>

            <div class="col-lg-6 col-sm-6 col-xs-10" id="CTerminos">
            <h4>Terminos Y Condiciones</h4>
            <div class="input-group col-lg-10 col-sm-10 col-xs-10">
                <label class="input-group-btn">
                    <span class="btn btn-primary">
                        Seleccionar&hellip; <asp:FileUpload type="file" ID="FileUpload8" runat=server style="display: none;" accept="application/pdf" />
                    </span>
                </label>
                <asp:TextBox type="text" runat="server" class="form-control" id="TextBox2" readonly="true" />
            </div>
                 <small class="form-text text-muted">Archivo Max 15 Mb.</small>
             </div>

            <div class="col-lg-6 col-sm-6 col-xs-10" id="CNoAdeudo">
            <h4>Carta de No Adeudo</h4>
            <div class="input-group col-lg-10 col-sm-10 col-xs-10">
                <label class="input-group-btn">
                    <span class="btn btn-primary">
                        Seleccionar&hellip; <asp:FileUpload type="file" ID="FileUpload9" runat=server style="display: none;" accept="application/pdf" />
                    </span>
                </label>
                <asp:TextBox type="text" runat="server" class="form-control" id="TextBox1" readonly="true" />
            </div>
                 <small class="form-text text-muted">Archivo Max 15 Mb.</small>
             </div>


            </div>

           <div class="row">
                    <label class="col-form-label col-lg-10 col-sm-10 col-xs-6"></label>
                    <div class="col-xs-7 col-md-7">
                        <asp:Button Text="Cargar" runat="server" ID="btnEnviar" AutoPostBack="False" CssClass="btn btn-tsys cargar" title="Cargar Documentos" OnClick="Btn_Buscar" />
                    </div>
                </div>




            </asp:Panel>




    </div>
</asp:Content>