<%@ Page Title="Consulta de Pagos" Language="C#" MasterPageFile="SiteEmpleado.Master" AutoEventWireup="true" CodeFile="Administracion_Pago_Empleado.aspx.cs" Inherits="Administracion_Pago_Empleado" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
   Version 16-Octubre-2019 By Luis Angel Garcia P
        <script src ="../../Css/sweetalert.min.js" type="text/javascript"></script>

   <script>
    function alertme(titulo,mesaje,Tipo) {
    swal(titulo, mesaje, Tipo)
    return true;
    }
    </script>

    <SCRIPT language="JavaScript">
  function numbersonly(e)
  {
    var unicode=e.charCode? e.charCode : e.keyCode
    if (unicode!=8 && unicode!=44)
    {
        if (unicode < 46 || unicode > 57) //if not a number
      { return false} //disable key press    
    }  
  }  
</SCRIPT>

  <asp:UpdatePanel runat="server" id="UpdatePanel" updatemode="Conditional">
  <ContentTemplate>
      
    <div class="col-lg-12 col-sm-12 col-12" id="M1">
        <h3>Consulta de Pagos</h3>
    </div>

      <div class="row">
          <br />
      <div class="col-xs-12 md-3 col-lg-3">
         <label>Folio</label>
        <asp:TextBox runat="server" type="text" MaxLength="20" ID="Folio" class="form-control" TabIndex="3" />
      </div>

      <div class="col-xs-12 md-3 col-lg-3">
          <label>Factura</label>
        <asp:TextBox runat="server" type="text" MaxLength="20" ID="Factura" class="form-control" TabIndex="4"  />
      </div>

      <div class="col-xs-12 md-3 col-lg-3">
          <label>Monto</label>
        <asp:TextBox runat="server" type="text" MaxLength="20" ID="Monto" class="form-control" TabIndex="2"  onkeypress="return numbersonly(event);"/>
      </div>

      <div class="col-xs-12 col-md-3 col-lg-3">
          <label>Estatus</label>
        <asp:DropDownList ID="List" runat="server" class="selectpicker show-tick form-control" OnSelectedIndexChanged="List_SelectedIndexChanged" AutoPostBack="True">
           <asp:ListItem Value="2">Aprobado</asp:ListItem>
           <asp:ListItem Value="1">Pendiente</asp:ListItem>
           <asp:ListItem Value="3">Rechazado</asp:ListItem>
           <asp:ListItem Value="4">Eliminado</asp:ListItem>
        </asp:DropDownList>
      </div>

      <div class="col-xs-11 md-4 col-lg-3">
        <asp:CheckBox runat="server" id="ChkFechas" Text="Filtrar Por Fechas" AutoPostBack="true" OnCheckedChanged="ChkFechas_CheckedChanged"/>
        <asp:DropDownList ID="LFechas" runat="server" class="selectpicker show-tick form-control">
           <asp:ListItem Value="Factura">Fecha de Recepcion</asp:ListItem>
           <asp:ListItem Value="Pago">Fecha de Pago</asp:ListItem>
        </asp:DropDownList>
       </div>

      <div class="col-xs-6 md-4 col-lg-3">
          <label>Desde</label>
        <asp:TextBox type="date" name="fecha" ID="F1" min="1980-01-01" max="2050-12-31" step="1"  class="form-control"  runat="server"/>
      </div>

      <div class="col-xs-6 md-4 col-lg-3">
          <label>Hasta</label>
        <asp:TextBox type="date" name="fecha" ID="F2" min="1980-01-01" max="2050-12-31" step="1"  class="form-control"  runat="server"/>
      </div>

    <div class="col-xs-12 col-md-3 col-lg-3">
      <label>Proveedor</label>
      <asp:DropDownList ID="SelProv" runat="server" class="selectpicker show-tick form-control" AutoPostBack ="true"></asp:DropDownList>
    </div>
        <br />
        <br />
      </div> 
      <div class="row">
              <div class="col-xs-3 col-sm-2 col-md-1">
                  <div class="form-group row">
                      <label class="col-form-label col-xs-11 col-sm-11 col-md-11"></label>
                      <div class="col-xs-4 col-sm-2 col-md-2">
                          <asp:Button Text="Buscar" ID="Button1" runat="server" CssClass="btn btn-primary" OnClick="Buscar" title="Generar Busqueda" />
                      </div>
                  </div>

              </div>

              <div class="col-xs-3 col-sm-2 col-md-1">
                  <div class="form-group row">
                      <label class="col-form-label col-xs-11 col-sm-11 col-md-11"></label>
                      <div class="col-xs-4 col-sm-2 col-md-2">
                          <asp:Button Text="Limpiar" ID="Button2" runat="server" CssClass="btn btn-tsys" OnClick="Limpia" title="Generar Busqueda" />
                      </div>
                  </div>

              </div>
          </div>


      <asp:Panel runat="server" ID="DatosV" Width="90%" Height=" 300px">
          <div class="col-xs-12">
              <font size="5" color="#A4A4A4">No Se Encontraron Resultados</font>
          </div>
          <div class="col-xs-12" id="Timg">
              <center>     
                    <asp:Image runat="server" ID="PckT" Width="50%" Height="50%" ImageUrl="../../Img/TGris.png" />      
                   </center>

          </div>
      </asp:Panel>

    <div class ="Row">

        <asp:GridView ID="GridView1" CssClass="table table-bordered bs-table" 
            margin-left="auto" margin-right="auto" runat="server" 
            AutoGenerateColumns="False" 
            onpageindexchanging="GridView1_PageIndexChanging" 
            onrowcancelingedit="GridView1_RowCancelingEdit" 
            onrowediting="GridView1_RowEditing" 
            OnRowCommand="GridView1_RowCommand" 
            OnRowCreated = "GridView1_RowCreated" 
            onrowupdating="GridView1_RowUpdating" 
            OnRowDataBound="GridView1_RowDataBound"
            width="99%">

            <AlternatingRowStyle BackColor="White" />

            <HeaderStyle BackColor="#337ab7" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#ffffcc" />
            <EmptyDataRowStyle forecolor="Red" CssClass="table table-bordered" />

            <Columns>

                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="50px">
                    <ItemTemplate>
                        <asp:Button ID="Documento_2" CssClass="btn btn-default" runat="server" CommandName="Documento_2" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="PDF" />
                    </ItemTemplate>
                <HeaderStyle Width="50px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:TemplateField>

                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="50px">
                    <ItemTemplate>
                        <asp:Button ID="Documento_3" CssClass="btn btn-default" runat="server" CommandName="Documento_3" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" Text="XML" />
                    </ItemTemplate>
                <HeaderStyle Width="50px"></HeaderStyle>
                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:TemplateField>
  
                <asp:BoundField DataField="Factura" HeaderText="Factura" HeaderStyle-Width="60px" ReadOnly="True" SortExpression="C.B." >
                                <HeaderStyle Width="60px"></HeaderStyle>
                </asp:BoundField>
                <asp:BoundField DataField="Serie" HeaderText="Serie" HeaderStyle-Width="65px" SortExpression="Fecha" ReadOnly="True" >
                                <HeaderStyle Width="65px"></HeaderStyle>
                </asp:BoundField>
                <asp:BoundField DataField="Folio" HeaderText="Folio"  HeaderStyle-Width="65px" ReadOnly="True" SortExpression="Detalles" >
                                <HeaderStyle Width="65px"></HeaderStyle>
                </asp:BoundField>
                <asp:BoundField DataField="Total" HeaderText="Total" HeaderStyle-Width="78px" ReadOnly="True" SortExpression="Referencia" >
                                <HeaderStyle Width="78px"></HeaderStyle>
                </asp:BoundField>
                <asp:BoundField DataField="Proveedor" HeaderText="Proveedor" HeaderStyle-Width="80px" SortExpression="Fecha" ReadOnly="True" >
                <HeaderStyle Width="65px"></HeaderStyle>
                </asp:BoundField>
                <asp:BoundField DataField="Company" HeaderText="Empresa"  HeaderStyle-Width="120px" ReadOnly="True" SortExpression="Detalles" >
                                <HeaderStyle Width="120px"></HeaderStyle>
                </asp:BoundField>
                <asp:BoundField DataField="FechaR" HeaderText="Fecha Recepción" HeaderStyle-Width="78px" ReadOnly="True" SortExpression="Referencia" >
                                <HeaderStyle Width="78px"></HeaderStyle>
                </asp:BoundField>
                <asp:BoundField DataField="FechaP" HeaderText="Fecha Pago" HeaderStyle-Width="78px" ReadOnly="True" SortExpression="Referencia">
                    <HeaderStyle Width="78px"></HeaderStyle>
                </asp:BoundField>
                <asp:BoundField DataField="Status" HeaderText="Status" HeaderStyle-Width="78px" ReadOnly="True" SortExpression="Referencia">
                    <HeaderStyle Width="78px"></HeaderStyle>
                </asp:BoundField>
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
    </div>


  </ContentTemplate>
   <Triggers>
    <asp:AsyncPostBackTrigger controlid="Button1" eventname="Click" />
    <asp:AsyncPostBackTrigger ControlID="SelProv" EventName="selectedindexchanged" />
    <asp:PostBackTrigger ControlID="GridView1" />
    </Triggers>
  </asp:UpdatePanel>


</asp:Content>