<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FacultadesPage.aspx.cs" Inherits="SISTEMA_UNIVERSITARIO.Pages.Administracion.Facultades.FacultadesPage" %>

<%@ Register Assembly="DevExpress.Web.Bootstrap.v21.1, Version=21.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.Bootstrap" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v21.1, Version=21.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mt-4">
        <button type="button" class="btn btn-primary" data-toggle="modal" data-target="#modalFacultades">
            Nueva Facultad
        </button>
    </div>
    <div class="mt-2">
        <div class="">
            <h1>FACULTADES</h1>
        </div>

        <dx:ASPxGridView ID="gvFacultades" runat="server" Theme="Material" Width="100%" AutoGenerateColumns="False" DataSourceID="gvFacus_DS" KeyFieldName="COD_FACULTAD">
            <SettingsBehavior EnableRowHotTrack="true" HeaderFilterMaxRowCount="30" />
            <Styles>
                <RowHotTrack BackColor="#F5F5DC"></RowHotTrack>
            </Styles>
            <SettingsSearchPanel Visible="true" />
            <%-- AREA DE COLUMNAS --%>
            <Settings ShowFilterRow="false" ShowGroupPanel="false"></Settings>
            <Columns>
                <dx:GridViewDataTextColumn FieldName="COD_FACULTAD" Visible="false" ReadOnly="True" VisibleIndex="0">
                    <EditFormSettings Visible="False"></EditFormSettings>
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="COD_SUCURSAL" Visible="false" ReadOnly="True" VisibleIndex="1">
                    <EditFormSettings Visible="False"></EditFormSettings>
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="NOMBREFACULTAD" Caption="NOMBRE FACULTAD" VisibleIndex="2"></dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="NOMBRESUCURSAL" Caption="NOMBRE SUCURSAL" VisibleIndex="3"></dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="ESTADO" HeaderStyle-HorizontalAlign="Center" CellStyle-HorizontalAlign="Center" ReadOnly="True" VisibleIndex="4"></dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="EDITAR" HeaderStyle-HorizontalAlign="Center" CellStyle-HorizontalAlign="Center" ReadOnly="True" VisibleIndex="5">
                    <DataItemTemplate>
                        <asp:LinkButton ID="btnEditar" OnClick="btnEditar_Click" CssClass="btn btn-primary" runat="server"><i class="fa fa-edit" style="color:white"></i></asp:LinkButton>
                    </DataItemTemplate>
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="CAMBIAR" HeaderStyle-HorizontalAlign="Center" CellStyle-HorizontalAlign="Center" ReadOnly="True" VisibleIndex="5">
                    <DataItemTemplate>
                        <asp:LinkButton ID="btnInactivar" OnClick="btnInactivar_Click" runat="server" CssClass='<%# Eval("ESTADO").ToString() == "ACTIVO"?"disableButton":"enableButton" %>'  ToolTip='<%# Eval("ESTADO").ToString() == "INACTIVO"?"Activar":"Inactivar"%>'><i style="color:white;" class="<%# Eval("ESTADO").ToString() == "ACTIVO" ? "fas fa-ban" : "fas fa-check" %>"></i></asp:LinkButton>
                    </DataItemTemplate>
                </dx:GridViewDataTextColumn>
            </Columns>
            <FormatConditions>
                <dx:GridViewFormatConditionHighlight Expression="[ESTADO] = 'ACTIVO'" FieldName="ESTADO" ShowInColumn="ESTADO" Format="GreenText"></dx:GridViewFormatConditionHighlight>
                <dx:GridViewFormatConditionHighlight Expression="[ESTADO] = 'INACTIVO'" FieldName="ESTADO" ShowInColumn="ESTADO" Format="RedText"></dx:GridViewFormatConditionHighlight>
            </FormatConditions>
        </dx:ASPxGridView>
        <asp:SqlDataSource runat="server" ID="gvFacus_DS" ConnectionString='<%$ ConnectionStrings:SISTEMAUNIVERSITARIOConnectionString %>' SelectCommand="SELECT TB_FACULTADES.COD_FACULTAD, TB_SUCURSALES.COD_SUCURSAL, TB_FACULTADES.NOMBRE AS NOMBREFACULTAD, TB_SUCURSALES.NOMBRE AS NOMBRESUCURSAL,
CASE WHEN TB_FACULTADES.ESTADO = 1 THEN 'ACTIVO' ELSE 'INACTIVO' END AS ESTADO
FROM TB_FACULTADES INNER JOIN TB_FACUXSUCURSAL ON TB_FACULTADES.COD_FACULTAD = TB_FACUXSUCURSAL.COD_FACULTAD
INNER JOIN TB_SUCURSALES ON TB_FACUXSUCURSAL.COD_SUCURSAL = TB_SUCURSALES.COD_SUCURSAL ORDER BY TB_FACULTADES.COD_FACULTAD DESC"></asp:SqlDataSource>
    </div>

    <%--    SECCION DE MODALES--%>

    <!-- Modal Incersion -->
    <div class="modal fade" id="modalFacultades" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="exampleModalLabel">NUEVA FACULTAD</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <dx:BootstrapTextBox ID="txtNombreFacultad" Caption="Nombre Facultad" Width="100%" ValidationSettings-RequiredField-IsRequired="true" NullText="Escriba aqui"
                        ValidationSettings-ValidationGroup="nuevoFacu" runat="server">
                    </dx:BootstrapTextBox>
                    <asp:RequiredFieldValidator ID="RequiredTxtFacu" ForeColor="Red" runat="server" ErrorMessage="Campo No Puede estar Vacio" ControlToValidate="txtNombreFacultad" ValidationGroup="nuevoFacu"></asp:RequiredFieldValidator>
                    <br />
                    <dx:BootstrapComboBox ID="cmbSucursales" NullText="Seleccione" runat="server" Caption="Sucursales" TextField="NOMBRE" ValueField="COD_SUCURSAL" DataSourceID="cmbSucursales_DS" Width="100%">
                    </dx:BootstrapComboBox>
                    <asp:RequiredFieldValidator ID="RequiredCmbSucu" ForeColor="Red" runat="server" ErrorMessage="Campo No Puede estar Vacio" ControlToValidate="cmbSucursales" ValidationGroup="nuevoFacu"></asp:RequiredFieldValidator>

                    <div class="row">
                    </div>
                    <div class="row">
                    </div>
                    <asp:SqlDataSource runat="server" ID="cmbSucursales_DS" ConnectionString='<%$ ConnectionStrings:SISTEMAUNIVERSITARIOConnectionString %>' SelectCommand="SELECT COD_SUCURSAL,NOMBRE,CASE WHEN ESTADO = 1 THEN 'ACTIVO' ELSE 'INACTIVO' END AS ESTADO FROM TB_SUCURSALES WHERE ESTADO = 1"></asp:SqlDataSource>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cerrar</button>
                    <dx:BootstrapButton ID="btnGuardarFacu" OnClick="btnGuardarFacu_Click" ValidationGroup="nuevoFacu" runat="server" SettingsBootstrap-RenderOption="Primary" AutoPostBack="false" Text="Guardar"></dx:BootstrapButton>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal Edicion -->
    <div class="modal fade" id="modalFacultadesEditar" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabelE" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="exampleModalLabelE">NUEVA FACULTAD</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <dx:BootstrapTextBox ID="txtNombreEdit" Caption="Nombre Facultad" Width="100%" ValidationSettings-RequiredField-IsRequired="true" NullText="Escriba aqui"
                        ValidationSettings-ValidationGroup="nuevoFacuE" runat="server">
                    </dx:BootstrapTextBox>
                    <asp:RequiredFieldValidator ID="ValidaEdicionFacultad" ForeColor="Red" runat="server" ErrorMessage="Campo No Puede estar Vacio" ControlToValidate="txtNombreEdit" ValidationGroup="nuevoFacu"></asp:RequiredFieldValidator>
                    <br />
                    <dx:BootstrapComboBox ID="cmbSucuEdit" NullText="Seleccione" runat="server" Caption="Sucursales" TextField="NOMBRE" ValueField="COD_SUCURSAL" DataSourceID="cmbSucursales_DS" Width="100%">
                    </dx:BootstrapComboBox>
                    <asp:RequiredFieldValidator ID="ValidaEdicionSucursal" ForeColor="Red" runat="server" ErrorMessage="Campo No Puede estar Vacio" ControlToValidate="cmbSucuEdit" ValidationGroup="nuevoFacuE"></asp:RequiredFieldValidator>

                    <div class="row">
                    </div>
                    <div class="row">
                    </div>
                    <asp:SqlDataSource runat="server" ID="cmbEditSucursales" ConnectionString='<%$ ConnectionStrings:SISTEMAUNIVERSITARIOConnectionString %>' SelectCommand="SELECT COD_SUCURSAL,NOMBRE,CASE WHEN ESTADO = 1 THEN 'ACTIVO' ELSE 'INACTIVO' END AS ESTADO FROM TB_SUCURSALES WHERE ESTADO = 1"></asp:SqlDataSource>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Cerrar</button>
                    <dx:BootstrapButton ID="btnEditarFacultades" OnClick="btnEditarFacultades_Click" ValidationGroup="nuevoFacuE" runat="server" SettingsBootstrap-RenderOption="Primary" AutoPostBack="false" Text="Guardar"></dx:BootstrapButton>
                </div>
            </div>
        </div>
    </div>
</asp:Content>


