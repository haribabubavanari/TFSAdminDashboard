<%@ Page Title="Active Releases" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="ActiveReleases.aspx.cs" Inherits="TFSAdminDashboard.ActiveReleases" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
<script type="text/javascript">
    $("[src*=plus]").live("click", function () {
        $(this).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(this).next().html() + "</td></tr>")
        $(this).attr("src", "images/minus.png");
    });
    $("[src*=minus]").live("click", function () {
        $(this).attr("src", "images/plus.png");
        $(this).closest("tr").next().remove();
    });
</script>
 
      <h2><%: Title %>.</h2>



      <asp:CheckBoxList ID="cblistActiveReleases" runat="server" ></asp:CheckBoxList>

    <%--<asp:GridView ID="grdActiveReleases" runat="server"></asp:GridView>--%>

   <%-- <asp:DataGrid ID="ActiveReleases" runat="server" AutoGenerateColumns="false" CssClass="Grid" 
        DataKeyNames="ActiveReleases" ShowHeader="false">
        <Columns>
             <asp:TemplateField>
                <ItemTemplate>
                    <img alt = "" style="cursor: pointer" src="images/plus.png" />
                    <asp:Panel ID="pnlActiveReleases" runat="server" Style="display: none">

                    </asp:Panel>
                </ItemTemplate>
             </asp:TemplateField>
        </Columns>

    </asp:DataGrid>--%>
     <asp:GridView ID="gvActiveReleases" runat="server" AutoGenerateColumns="False" CssClass="Grid"
           OnRowDataBound ="gvActiveReleases_OnRowDataBound" DataKeyNames="ActiveReleases" ShowHeader="True">
        <Columns>
     <asp:TemplateField>
                <ItemTemplate>
                      <img alt = "" style="cursor: pointer" src="images/plus.png" />
                    <asp:Panel ID="Panel1" runat="server" Style="display: none">
                        
      <asp:GridView ID="gvActiveBuilds" runat="server" AutoGenerateColumns="False" CssClass="ChildGrid"
          OnRowDataBound="gvActiveBuilds_RowDataBound">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <img alt = "" style="cursor: pointer" src="images/plus.png" />
                    <asp:Panel ID="pnlOrders" runat="server" Style="display: none">
                        <asp:GridView ID="gvOrders" runat="server" AutoGenerateColumns="false" CssClass = "ChildGrid" >
                            <Columns>
                                <asp:BoundField ItemStyle-Width="150px" DataField="Environments" HeaderText="Environments" />
                                <asp:BoundField ItemStyle-Width="250px" DataField="EnvStatus" HeaderText="Deployment Status" />
                                <asp:ButtonField ButtonType="Button" CommandName="Update" HeaderText="Deploy" ShowHeader="False" Text="Deploy" />

            
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField ItemStyle-Width="150px" DataField="Releases" HeaderText="Releases" >

<ItemStyle Width="150px"></ItemStyle>
            </asp:BoundField>

            
            <asp:TemplateField HeaderText="Builds">
                <ItemTemplate>
                <asp:DropDownList ID="drpdwnBuilds" Width="200px" runat="server"></asp:DropDownList>
                    </ItemTemplate>
            </asp:TemplateField>

            
            <asp:ButtonField ButtonType="Button" CommandName="Update" HeaderText="Queue New Build" ShowHeader="True" Text="Queue New Build" />

            <asp:TemplateField HeaderText="Deployment Status">
                <ItemTemplate>
                    <asp:Label ID="lblDeploymentStatus" runat="server" Text="Succeeded in PREP"></asp:Label>
                     </ItemTemplate>
            </asp:TemplateField>
            
        </Columns>
    </asp:GridView>

                    </asp:Panel>
                       </ItemTemplate>
                         </asp:TemplateField>
        
             <asp:BoundField ItemStyle-Width="150px" DataField="ActiveReleases" HeaderText="Active Releases" >

<ItemStyle Width="150px"></ItemStyle>
            </asp:BoundField>    
        </Columns>
    </asp:GridView>


</asp:Content>

