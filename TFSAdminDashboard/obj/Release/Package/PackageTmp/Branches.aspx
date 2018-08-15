<%@ Page Title="Branches" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Branches.aspx.cs" Inherits="TFSAdminDashboard.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>

    

<div class="row">
  <div class="column" style="float:left" >
   <asp:TextBox ID="txbRelName" runat="server"></asp:TextBox>
<asp:Button ID="btnCreateBranch" runat="server" Text="Create Branches" OnClick="btnCreateBranch_Click" />
    <br />
<asp:CheckBoxList ID="cblistBranches" runat="server"></asp:CheckBoxList>
  </div>
<%--  <div class="column" style="float:left" >
    <h2>Column 2</h2>
    <p>Some text..</p>
  </div>
  <div class="column" style="float:left" >
    <h2>Column 3</h2>
    <p>Some text..</p>
  </div>--%>
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
