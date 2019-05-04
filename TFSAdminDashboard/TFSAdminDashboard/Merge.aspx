<%@ Page Title="Merge Branchess" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Merge.aspx.cs" Inherits="TFSAdminDashboard.Merge" %>



<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/bootstrap.css" rel="stylesheet" />
    <div style="padding: 2em 2em 2em 2em">
        <div style="text-align: center">
            <asp:Button ID="btnMergeBranch" runat="server" Text="Merge Selected Branches" OnClick="btnMergeBranch_Click" class="btn-success ui-button ui-widget ui-state-default ui-corner-all back spinnertric" />
            &nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnDeleteBranch" runat="server" Text="Delete Selected Branches" OnClick="btnDeleteBranch_Click" class="btn-success ui-button ui-widget ui-state-default ui-corner-all back spinnertric" />
        </div>
        <br />
        <div style="text-align: center">
            <asp:Label ID="lblSelectedBranches" runat="server" Visible="false" Text=""></asp:Label>
        </div>
        <br />
        <div class="entryPanel">
            <div class="row">
                <div class="col-sm-1"></div>
                <div class="form-group col-sm-3">

                    <asp:TreeView ID="TreeViewBranches" runat="server" ShowCheckBoxes="Leaf" SelectedNodeStyle-CssClass="selectedtree" CssClass="treeview"
                        ParentNodeStyle-CssClass="parenttree" RootNodeStyle-CssClass="rootnode" LeafNodeStyle-CssClass="childtree">
                    </asp:TreeView>

                </div>
            </div>
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


