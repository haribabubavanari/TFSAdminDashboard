<%@ Page Title="Merge Branchess" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Merge.aspx.cs" Inherits="TFSAdminDashboard.Contact" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>

    <div class="row">
  <div class="column" style="float:left" >
       <h3>MR Branches</h3>
 <asp:Button ID="btnMergeBranch" runat="server" Text="Merge Branches" OnClick="btnMergeBranch_Click"  />    <br />
<asp:CheckBoxList ID="cblistMergeBranch" runat="server"></asp:CheckBoxList>
  </div>
  <div class="column" style="float:left"  >
     <h3>HF Branches</h3>
      <asp:Button ID="btnHFMergeBranch" runat="server" Text="Merge HF Branches"  />    <br />
<asp:CheckBoxList ID="cblistHFMergeBranch" runat="server"></asp:CheckBoxList>
  </div>
  <div class="column" style="float:left" >
     <h3>Project Branches</h3>
      <asp:Button ID="btnPROJMergeBranch" runat="server" Text="Merge Branches" OnClick="btnMergeBranch_Click"  />    <br />
<asp:CheckBoxList ID="cblistProjMergeBranch" runat="server"></asp:CheckBoxList>
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
                        <h4 class="modal-title"><asp:Label ID="lblModalTitle" runat="server" Text=""></asp:Label></h4>
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
