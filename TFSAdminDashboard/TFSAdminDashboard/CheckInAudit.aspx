<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master"  CodeBehind="CheckInAudit.aspx.cs" Inherits="TFSAdminDashboard.CheckInAudit" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/bootstrap.css" rel="stylesheet" />
     <div style="padding: 2em 2em 2em 2em">
        <div class="form-inline ">
            <%--<asp:DropDownList ID="drpdwnBranches" ControlStyle-CssClass="form-control" runat="server"></asp:DropDownList>--%>
            <asp:Label ID="Label1" runat="server" Text="TFS Path" CssClass="relesetext"></asp:Label>
            <asp:TextBox ID="txbPath" Width="450" runat="server" CssClass="textBox"></asp:TextBox>
            <asp:Button ID="btnBrowse" runat="server" Text="Browse" OnClick="btnBrowse_Click" class="btn-success ui-button ui-widget ui-state-default ui-corner-all back spinnertric" />

            &nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnShowReport" runat="server" Text="Full Report" OnClick="btnShowReport_Click"  class="btn-success ui-button ui-widget ui-state-default ui-corner-all back spinnertric" /> &nbsp;&nbsp;&nbsp;
            <asp:Button ID="btncomparewithServiceManager" runat="server" Text="Compare ServiceMngr" OnClick="btncomparewithServiceManager_Click"  class="btn-success ui-button ui-widget ui-state-default ui-corner-all back spinnertric" />
        </div>
        <asp:GridView ID="grdShowReport" runat="server"></asp:GridView>
    </div>
    <!-- Bootstrap Modal Dialog -->
    <div class="modal fade" id="myModal" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <asp:UpdatePanel ID="upModal" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                            <h4 class="modal-title">
                                <asp:Label ID="lblModalTitle" runat="server" Text=""></asp:Label></h4>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="lblModalBody" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="modal-footer">
                            <button class="btn btn-info" data-dismiss="modal" aria-hidden="true">Close</button>
                        </div>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>



</asp:Content>
