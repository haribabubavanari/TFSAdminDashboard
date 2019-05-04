<%@ Page Title="Branches" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Branches.aspx.cs" Inherits="TFSAdminDashboard.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/bootstrap.css" rel="stylesheet" />
    <br />
    <div class="row" style="text-align: center; width: 100%; padding-left: 2em;">

        <div class="form-inline">
            <asp:Label ID="lblEnterReleaseNum" runat="server" Text="Enter Release Number" CssClass="relesetext" />
            <asp:TextBox ID="txbRelName" runat="server" CssClass="textBox"></asp:TextBox>
            &nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnCreateBranch" runat="server" Text="Create Branches" OnClick="btnCreateBranch_Click" class="btn-success ui-button .btn-success ui-widget ui-state-default ui-corner-all spinnertric" />
            <br />
        </div>

    </div>

    <div style="padding: 2em 2em 2em 2em">
        <div class="entryPanel panel-body ">
            <asp:CheckBoxList ID="cblistBranches" runat="server" RepeatLayout="Table" RepeatColumns="4" CssClass="checkboxlist" RepeatDirection="vertical"></asp:CheckBoxList>
        </div>
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
