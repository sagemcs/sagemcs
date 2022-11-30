<%@ Page Language="C#" Title="Aprobar Usuarios" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeFile="Usuarios.aspx.cs" Inherits="Pagos" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    Version 25-Julio-2019 By Luis Angel Garcia P
    <META HTTP-EQUIV="Cache-Control" CONTENT ="no-cache">
    <meta http-equiv="Expires" content="0" />
<meta http-equiv="Pragma" content="no-cache" />
    <script src ="../../Css/sweetalert.min.js" type="text/javascript"></script>
    <link href ="../../Css/HojaProv.css" rel="stylesheet" type ="text/css" />
    <script src ="../../Scripts/jquery.blockui.min.js" type="text/javascript"></script>
    <script src ="../../Scripts/custom.js" type="text/javascript"></script>

    <script>
        function alertme(titulo, mesaje, Tipo) {
            swal(titulo, mesaje, Tipo)
             unblockUI();
        }
    </script>

    <div class="col-lg-12 col-sm-5 col-12" id="M1">
      <h3>Administracion de Usuarios</h3>
    </div>


        <!-- Nav pills -->
        <ul class="nav nav-pills">
          <li class="nav-item active">
            <a class="nav nav-pills nav-justified" data-toggle="pill" href="#home">Proveedores</a>
          </li>
          <li class="nav-item">
            <a class="nav nav-pills nav-justified" data-toggle="pill" href="#menu1">Usuarios T|SYS|</a>
          </li>
        </ul>

        <!-- Tab panes -->
        <div class="tab-content">
       <div class="tab-pane container active" id="home">
        <br />

       <asp:Panel runat="server" ID="DatosV" Width="90%" Height=" 300px">
               <div class ="col-xs-12">
                   <font size="5" color="#A4A4A4">No Se Encontraron Resultados</font>
               </div>
               <div class ="col-xs-12" id="Timg">
                   <center>     
                    <asp:Image runat="server" ID="PckT" Width="50%" Height="50%" ImageUrl="../../Img/TGris.png" />      
                   </center>
               
               </div>
          </asp:Panel>
       <asp:UpdatePanel runat="server" id="UpdatePanel" updatemode="Conditional">

          <Triggers>
              <asp:AsyncPostBackTrigger controlid="Button1" eventname="Click" />
            </Triggers>
          
          <ContentTemplate>
   
    <asp:GridView ID="GridView1" CssClass="table table-bordered bs-table" 
        margin-left="auto" margin-right="auto" runat="server" 
        AutoGenerateColumns="False" 
        onpageindexchanging="GridView1_PageIndexChanging" 
        onrowcancelingedit="GridView1_RowCancelingEdit" 
        onrowediting="GridView1_RowEditing"  
        OnRowCreated = "GridView1_RowCreated" 
        onrowupdating="GridView1_RowUpdating" 
        OnRowDataBound="GridView1_RowDataBound"
        width="99%">

        <AlternatingRowStyle BackColor="White" />

        <HeaderStyle BackColor="#337ab7" Font-Bold="True" ForeColor="White" />
        <EditRowStyle BackColor="#ffffcc" />
        <EmptyDataRowStyle forecolor="Red" CssClass="table table-bordered" />

        <Columns>


<%--          <asp:TemplateField  ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="5px">
          <ItemTemplate>
             <asp:Button ID="Aprobar" Widht="5px" Text="Aprobar" CssClass="btn btn-info" title="Descargar Documento" runat="server" CommandName="Aprobar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"></asp:Button>
          </ItemTemplate> 
             <HeaderStyle Width="5px" />
             <ItemStyle HorizontalAlign="Center" />
         </asp:TemplateField>

          <asp:TemplateField  ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="5px">
          <ItemTemplate>
             <asp:Button ID="Rechazar" Widht="5px" Text="Rechazar" CssClass="btn btn-info" title="Descargar Documento" runat="server" CommandName="Rechazar" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"></asp:Button>
          </ItemTemplate> 
             <HeaderStyle Width="5px" />
             <ItemStyle HorizontalAlign="Center" />
         </asp:TemplateField>--%>


            <%--botones de acción sobre los registros...--%>
            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="90px">
             <ItemTemplate>
             <%--Botones de eliminar y editar cliente...--%>
                    <asp:Button ID="btnEdit" runat="server" Text="Acción" CssClass="btn btn-info" CommandName="Edit" />
                </ItemTemplate>
                <edititemtemplate>
                    <%--Botones de grabar y cancelar la edición de registro...--%>
                    <asp:Button ID="btnUpdate" runat="server" Text="Aceptar" CssClass="btn btn-tsys" CommandName="Update" />
                    <asp:Button ID="btnCancel" runat="server" Text="Rechazar" CssClass="btn btn-success" CommandName="Cancel" />
                </edititemtemplate>

            <HeaderStyle Width="90px"></HeaderStyle>

            <ItemStyle HorizontalAlign="Center"></ItemStyle>
            </asp:TemplateField>

            <asp:BoundField DataField="Fecha" HeaderText="Fecha" HeaderStyle-Width="60px" ReadOnly="True" SortExpression="C.B." >
                            <HeaderStyle Width="60px"></HeaderStyle>
            </asp:BoundField>
            <asp:BoundField DataField="UserID" HeaderText="User ID" HeaderStyle-Width="65px" SortExpression="Fecha" ReadOnly="True" >
                            <HeaderStyle Width="65px"></HeaderStyle>
            </asp:BoundField>
            <asp:BoundField DataField="UserName" HeaderText="User Name"  HeaderStyle-Width="140px" ReadOnly="True" SortExpression="Detalles" >
                            <HeaderStyle Width="140px"></HeaderStyle>
            </asp:BoundField>
            <asp:BoundField DataField="Empresa" HeaderText="Company" HeaderStyle-Width="78px" ReadOnly="True" SortExpression="Referencia" >
                            <HeaderStyle Width="78px"></HeaderStyle>
            </asp:BoundField>
            <asp:BoundField DataField="Estado" HeaderText="Status" HeaderStyle-Width="78px" ReadOnly="True" SortExpression="Referencia" >
                            <HeaderStyle Width="78px"></HeaderStyle>
            </asp:BoundField>
<%--            <asp:TemplateField HeaderText="Estado">
                      <ItemTemplate>
                         <asp:image ID="imgestado" height="40" width="40" runat="server" ImageUrl="~/Img/Pend.png" />
                      </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="Anotaciones" HeaderStyle-Width="70px" ReadOnly="False" >
                            <HeaderStyle Width="70px"></HeaderStyle>
            </asp:BoundField>--%>
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

    </ContentTemplate>
    </asp:UpdatePanel>    
       <br />
         <asp:UpdatePanel runat="server" id="UpdatePanel3" updatemode="Conditional">
          <Triggers>
              <asp:AsyncPostBackTrigger controlid="Button1" eventname="Click" />
            </Triggers>
          
          <ContentTemplate>

       <div class="row">
            <div class="col-xs-11 col-sm-11 col-md-11">
                <div class="form-group row">
                    <label class="col-form-label col-sm-11"></label>
                    <div class="col-sm-11">
                      <asp:Button Text="Guardar" ID="Button1" runat="server" CssClass="btn btn-primary cargar" title="Guardar Datos" OnClick="btn_Enviar" />
                    </div>
                </div>
            </div>
        </div>

      </ContentTemplate>
      </asp:UpdatePanel>

       </div>  

        <!-- Registro Ususarios Tsys -->
      <div class="tab-pane container fade" id="menu1">
        <br />
          <asp:Panel runat="server" ID="DatosT" Width="90%" Height=" 300px">
              <div class="col-xs-12">
                  <font size="5" color="#A4A4A4">No Se Encontraron Resultados</font>
              </div>
              <div class="col-xs-12" id="Timg">
                  <center>     
                    <asp:Image runat="server" ID="Image1" Width="50%" Height="50%" ImageUrl="../../Img/TGris.png" />      
                   </center>

              </div>
          </asp:Panel>
       <asp:UpdatePanel runat="server" id="UpdatePanel1" updatemode="Conditional">

          <Triggers>
              <asp:AsyncPostBackTrigger controlid="Button2" eventname="Click" />
            </Triggers>
          
          <ContentTemplate>
       
   
    <asp:GridView ID="GridView2" CssClass="table table-bordered bs-table" 
        margin-left="auto" margin-right="auto" runat="server" 
        AutoGenerateColumns="False" 
        onpageindexchanging="GridView2_PageIndexChanging" 
        onrowcancelingedit="GridView2_RowCancelingEdit" 
        onrowediting="GridView2_RowEditing"  
        OnRowCreated = "GridView2_RowCreated" 
        onrowupdating="GridView2_RowUpdating" 
        OnRowDataBound="GridView2_RowDataBound"
        width="99%">

        <AlternatingRowStyle BackColor="White" />

        <HeaderStyle BackColor="#337ab7" Font-Bold="True" ForeColor="White" />
        <EditRowStyle BackColor="#ffffcc" />
        <EmptyDataRowStyle forecolor="Red" CssClass="table table-bordered" />

        <Columns>
            <%--botones de acción sobre los registros...--%>
            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="90px">
             <ItemTemplate>
             <%--Botones de eliminar y editar cliente...--%>
                    <asp:Button ID="btnEdit" runat="server" Text="Acción" CssClass="btn btn-info" CommandName="Edit" />
                </ItemTemplate>
                <edititemtemplate>
                    <%--Botones de grabar y cancelar la edición de registro...--%>
                    <asp:Button ID="btnUpdate" runat="server" Text="Aceptar" CssClass="btn btn-tsys" CommandName="Update" />
                    <asp:Button ID="btnCancel" runat="server" Text="Rechazar" CssClass="btn btn-success" CommandName="Cancel" />
                </edititemtemplate>

            <HeaderStyle Width="90px"></HeaderStyle>

            <ItemStyle HorizontalAlign="Center"></ItemStyle>
            </asp:TemplateField>

            <asp:BoundField DataField="Fecha" HeaderText="Fecha" HeaderStyle-Width="60px" ReadOnly="True" SortExpression="C.B." >
                            <HeaderStyle Width="60px"></HeaderStyle>
            </asp:BoundField>
            <asp:BoundField DataField="UserID" HeaderText="User ID" HeaderStyle-Width="65px" SortExpression="Fecha" ReadOnly="True" >
                            <HeaderStyle Width="65px"></HeaderStyle>
            </asp:BoundField>
            <asp:BoundField DataField="UserName" HeaderText="User Name"  HeaderStyle-Width="140px" ReadOnly="True" SortExpression="Detalles" >
                            <HeaderStyle Width="140px"></HeaderStyle>
            </asp:BoundField>
<%--            <asp:BoundField DataField="Empresa" HeaderText="Company" HeaderStyle-Width="78px" ReadOnly="True" SortExpression="Referencia" >
                            <HeaderStyle Width="78px"></HeaderStyle>
            </asp:BoundField>--%>
            <asp:BoundField DataField="Estado" HeaderText="Status" HeaderStyle-Width="78px" ReadOnly="True" SortExpression="Referencia" >
                            <HeaderStyle Width="78px"></HeaderStyle>
            </asp:BoundField>
<%--            <asp:BoundField HeaderText="Anotaciones" HeaderStyle-Width="70px" ReadOnly="False" >
                            <HeaderStyle Width="70px"></HeaderStyle>
            </asp:BoundField>--%>
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
      </ContentTemplate>
      </asp:UpdatePanel>
        <br />
      


       <asp:UpdatePanel runat="server" id="UpdatePanel2" updatemode="Conditional">
          <Triggers>
              <asp:AsyncPostBackTrigger controlid="Button2" eventname="Click" />
            </Triggers>
          
          <ContentTemplate>

                <div class="row">
            <div class="col-xs-11 col-sm-11 col-md-11">
                <div class="form-group row">
                    <label class="col-form-label col-sm-11"></label>
                    <div class="col-sm-11">
                        <asp:Button Text="Guardar" ID="Button2" runat="server" CssClass="btn btn-primary" title="Guardar Datos" OnClick="btn_EnviarTsys" />
                    </div>
                </div>
            </div>
      </div>


      </ContentTemplate>
      </asp:UpdatePanel>


       </div>


      </div>

</asp:Content>
