<%@ Page Language="C#" Title="Asignación Usuarios" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeFile="AsignacionUsuarios.aspx.cs" Inherits="AsignacionUsuarios" %>
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
    <script>
        function Pregunta(prove, usua) {

            swal({
                title: "Confirmación",
                text: "¿Desea vincular el Usuario?",
                icon: "warning",
                buttons: ["No", "Si"],
                confirmButtonColor: "#FF6600",
                closeModal: true,
                closeOnConfirm: false,
                dangerMode: true,
            })
                .then((willDelete) => {
                    if (willDelete) {
                        //e.preventDefault();
                        var Proce = "/Vincular_user";
                        var user = usua;
                        var prov = prove;
                        var url_list = NotificacionesWebService.get_path() + Proce;
                        $.ajax({
                            type: "POST",
                            "beforeSend": function (xhr) {
                                xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
                            },
                            url: url_list,
                            dataType: "json",
                            data: {
                                prov: prov,
                                user: user
                            },
                            success: function (respuesta) {
                                if (respuesta.success) {
                                      swal({
                                          title: "Notificaciones T|SYS|",
                                          text: respuesta.msg,
                                          icon: "success",
                                          buttons: ["OK"],
                                          confirmButtonColor: "#FFFFFF",
                                          closeModal: true,
                                          closeOnConfirm: false,
                                          dangerMode: true,
                                      })
                                      .then((willDelete) => {
                                          if (willDelete) {
                                              //VariableG();
                                              setTimeout("location.href='AsignacionUsuarios'", 1);
                                          }
                                          else {
                                              //VariableG();
                                              setTimeout("location.href='AsignacionUsuarios'", 1);
                                          }
                                      });
                                }
                                else {
                                  swal('Notificaciones T|SYS|', respuesta.msg, 'error');
                                }

                  },
                  error: function (respuesta) {
                      swal("Ocurrio un error al intentar vincular el usuario");
                      //VariableG();
                  }
              });
                    }
                    else {
                        //VariableG();
                    }
                });
            return true;
        }

        function Pregunta2(prove, valor, nuevo) {
            swal({
                title: "Confirmación",
                text: "¿Desea vincular el Usuario?",
                icon: "warning",
                buttons: ["No", "Si"],
                confirmButtonColor: "#FF6600",
                closeModal: true,
                closeOnConfirm: false,
                dangerMode: true,
            })
                .then((willDelete) => {
                    if (willDelete) {
                        var Proce = "/Actualizar_user";
                        var prov = prove;
                        var valo = valor;
                        var user = nuevo;
                        
                        var url_list = NotificacionesWebService.get_path() + Proce;
                        $.ajax({
                            type: "POST",
                            "beforeSend": function (xhr) {
                                xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
                            },
                            url: url_list,
                            dataType: "json",
                            data: {
                                prov: prov,
                                valo: valo,
                                user: user
                            },
                            success: function (respuesta) {
                                if (respuesta.success) {
                                    swal({
                                        title: "Notificaciones T|SYS|",
                                        text: respuesta.msg,
                                        icon: "success",
                                        buttons: ["OK"],
                                        confirmButtonColor: "#FFFFFF",
                                        closeModal: true,
                                        closeOnConfirm: false,
                                        dangerMode: true,
                                    })
                                        .then((willDelete) => {
                                            if (willDelete) {
                                                setTimeout("location.href='AsignacionUsuarios'", 1);
                                            }
                                            else {
                                                setTimeout("location.href='AsignacionUsuarios'", 1);
                                            }
                                        });
                                }
                                else {
                                    swal('Notificaciones T|SYS|', respuesta.msg, 'error');
                                }

                            },
                            error: function (respuesta) {
                                swal("Ocurrio un error al intentar vincular el proveedor");
                            }
                        });
                    }
                    else {
                    }
                });
            return true;
        }
    </script>
    <div class="col-lg-12 col-sm-5 col-12" id="M1">
      <h3>Administracion de Asignaciones Usuarios - Superior</h3>
    </div>


        <!-- Nav pills -->
        <ul class="nav nav-pills">
          <li class="nav-item active">
            <a class="nav nav-pills nav-justified" data-toggle="pill" href="#home">Asigancion</a>
          </li>
          <li class="nav-item">
            <a class="nav nav-pills nav-justified" data-toggle="pill" href="#menu1">Actualizacion</a>
          </li>
        </ul>

        <!-- Tab panes -->
        <div class="tab-content">
       <div class="tab-pane container active" id="home">
        <br />
       <asp:UpdatePanel runat="server" id="UpdatePanel3" updatemode="Conditional">
          <Triggers>
              <asp:AsyncPostBackTrigger controlid="Button1" eventname="Click" />
              <asp:AsyncPostBackTrigger controlid="Button3" eventname="Click" />
              <asp:AsyncPostBackTrigger ControlID="DropDownList2" EventName="selectedindexchanged" />
            </Triggers>
          
          <ContentTemplate>

       <br />
       <div class="row">
           <label class="col-form-label col-sm-2">Superior TSYS:</label>
           <div class="col-sm-4">
              <asp:DropDownList ID="DropDownList2" AutoPostBack="true" OnSelectedIndexChanged="SelProv_SelectedIndexChanged1"  class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server" />
              <br />
              <div class="alert alert-block alert-danger" id="AID7" style="display: none">
              <h5>¡Error!</h5>
              <asp:Label runat="server" ID="Label3"></asp:Label>
              </div>
          </div>
       </div>

       <div class="row" id="Cam2">
             <div class="col-xs-6 col-sm-6 col-md-6">
                    <div class="form-group row">
                        <label class="col-form-label col-sm-4">Correo :</label>
                        <div class="col-sm-8">
                            <asp:TextBox runat="server" type="text" ReadOnly="true"  AutoComplete = "off" AutoCompleteType="Disabled" MaxLength="80" ID="Nombre" class="form-control" TabIndex="2" />
                        </div>
                    </div>
              </div>
       </div>

       <div class="row">
            <label class="col-form-label col-sm-2"> Empleado:</label>
             <div class="col-sm-4">
               <asp:DropDownList ID="STipoProv" class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server" />
                <br />
                 <div class="alert alert-block alert-danger" id="AID3" style="display: none">
                <h5>¡Error!</h5>
                 <asp:Label runat="server" ID ="tplabel"></asp:Label>
              </div>
             </div>
            </div>
  
       <div id="Acc">
            <div class="col-xs-3 col-md-1">
                 <asp:Button Text="Agregar" runat="server" CssClass="btn btn-tsys" title="Agregar Usuario a la Lista" OnClick="Campos" ID="Button1" />
            </div>
            <div class="col-xs-3 col-md-1">
                 <asp:Button Text="Limpiar" runat="server" CssClass="btn btn-success" title="Agregar Usuario a la Lista" OnClick="Unnamed1_Clean" ID="Button3" />
            </div>
       </div>

       <br /> <br />
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
        <br /> <br />
       <asp:GridView ID="GridView3" CssClass="table table-bordered bs-table" 
        margin-left="auto" margin-right="auto" runat="server" 
        AutoGenerateColumns="False" 
        width="99%">

        <AlternatingRowStyle BackColor="White" />

        <HeaderStyle BackColor="#337ab7" Font-Bold="True" ForeColor="White" />
        <EditRowStyle BackColor="#ffffcc" />
        <EmptyDataRowStyle forecolor="Red" CssClass="table table-bordered" />

        <Columns>
<%--             <asp:BoundField DataField="ID" HeaderText="ID Proveedor" HeaderStyle-Width="50px" ReadOnly="True">
                  <HeaderStyle Width="50px"></HeaderStyle>
            </asp:BoundField>--%>
            <asp:BoundField DataField="Proveedor" HeaderText="Nombre Proveedor" HeaderStyle-Width="100px" ReadOnly="True">
                  <HeaderStyle Width="75px"></HeaderStyle>
            </asp:BoundField>
            <asp:BoundField DataField="Estatus" HeaderText="Estatus" HeaderStyle-Width="50px" ReadOnly="True">
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

      </ContentTemplate>
      </asp:UpdatePanel>
      <br />
       </div>  

        <!-- Registro Ususarios Tsys -->
      <div class="tab-pane container fade" id="menu1">
      <br />

    <asp:UpdatePanel runat="server" id="UpdatePanel1" updatemode="Conditional">
    <Triggers>
        <%--<asp:AsyncPostBackTrigger controlid="Button2" eventname="Click" />--%>
        <asp:AsyncPostBackTrigger ControlID="DropDownList1" EventName="selectedindexchanged" />
    </Triggers>          
    <ContentTemplate>

        <br />
        <div class="form-group row">
            <label class="col-form-label col-sm-2">Empleado:</label>
            <div class="col-sm-4">
                <asp:DropDownList ID="DropDownList1" AutoPostBack="true" OnSelectedIndexChanged="SelProv_SelectedIndexChanged2"  class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server" />
                <br />
                <div class="alert alert-block alert-danger" id="AID4" style="display: none">
                    <h5>¡Error!</h5>
                    <asp:Label runat="server" ID="Label1"></asp:Label>
                </div>
            </div>
        </div>
        <br />
        <div class="row" id="Cambios">
            <div class="form-group row">
                <div class="col-xs-3 col-sm-3 col-md-3">
                    <label class="col-form-label col-sm-7">Reasignar a Otro Empleado:</label>
                    <div class="col-sm-1">
                        <asp:CheckBox ID="CheckBox1" AutoPostBack="true" CssClass="ChkBoxClass" runat="server" OnCheckedChanged="CambioCheck" />
                    </div>
               </div>                
            </div>

            <div class="row" id="Cambio" runat="server" visible ="false">
            <div class="form-group row">
                <div class="col-xs-5 col-sm-5 col-md-5">
                    <div class="form-group row">
                        <label class="col-form-label col-sm-4">Usuario Actual :</label>
                        <div class="col-sm-8">
                            <asp:TextBox runat="server" type="text" ReadOnly="true" AutoComplete="off" AutoCompleteType="Disabled" MaxLength="80" ID="TextBox2" class="form-control" TabIndex="2" />
                        </div>
                    </div>
                </div>

                <div class="col-xs-5 col-sm-5 col-md-5">
                    <div class="form-group row">
                        <label class="col-form-label col-sm-4">Nuevo Usuario :</label>
                        <div class="col-sm-8">
                            <asp:DropDownList ID="DropDownList3" AutoPostBack="true" OnSelectedIndexChanged="SelProv_SelectedIndexChanged1"  class="selectpicker show-tick form-control" data-live-search="true" data-style="btn-primary" runat="server" />
                        </div>
                    </div>
                </div>
            </div>
            </div>
        </div>
        <br />
        <div class="form-group row">
            <div class="col-xs-3 col-sm-3 col-md-3">
                <label class="col-form-label col-sm-7">Sin Asignación:</label>
                <div class="col-sm-1">
                    <asp:CheckBox ID="CheckBox3" AutoPostBack="true" CssClass="ChkBoxClass" runat="server" OnCheckedChanged="CambioCheck2" />
                </div>
             </div>
        </div>
        <br />
        <div class="row" id="Div1" runat="server" visible ="false">
            <div class="form-group row">
                <div class="col-xs-5 col-sm-5 col-md-5">
                    <div class="form-group row">
                        <label class="col-form-label col-sm-4">Correo Electrónico :</label>
                        <div class="col-sm-8">
                            <asp:TextBox runat="server" ReadOnly="true" type="text" AutoComplete="off" AutoCompleteType="Disabled" name="Email" MaxLength="100" ID="Email" class="form-control" placeholder="" TabIndex="4" />
                        </div>
                    </div>
                </div> 
            </div>
        </div>
        <br />
        <div class="form-group row">
            <br />
            <div class="alert alert-block alert-danger" id="AID" style="display: none">
                <h5>¡Error!</h5>
                <asp:Label runat="server" ID="Label2"></asp:Label>
            </div>
        </div>

        <div id="Acc1">
            <div class="col-xs-3 col-md-1">
                <asp:Button Text="Limpiar" runat="server" CssClass="btn btn-success" title="Agregar Usuario a la Lista" OnClick="Unnamed2_Clean" ID="Button5" />
            </div>
            <div class="col-xs-3 col-md-1">
                <asp:Button Text="Guardar" runat="server" CssClass="btn btn-tsys" title="Agregar Usuario a la Lista" OnClick="Campos2" ID="Button4" />
            </div>
        </div>

        <br /><br /><br /><br />
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

        <%--OnClick="btn_EnviarTsys"--%>
      </ContentTemplate>
      </asp:UpdatePanel>

     </div>
     </div>

</asp:Content>
