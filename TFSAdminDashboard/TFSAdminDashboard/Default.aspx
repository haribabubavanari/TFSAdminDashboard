<%@ Page Title="Active Releases" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Default.aspx.cs" Inherits="TFSAdminDashboard.ActiveReleases" %>


<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/bootstrap.css" rel="stylesheet" />
    <br />
    <div style="padding: 2em 2em 2em 2em">

        <%--<asp:CheckBoxList ID="cblistActiveReleases" runat="server"  RepeatColumns="4" CssClass="checkboxlist"></asp:CheckBoxList>--%>
        <asp:GridView ID="gvActiveReleases" runat="server" AutoGenerateColumns="False" CssClass="Grid"
            OnRowDataBound="gvActiveReleases_OnRowDataBound" DataKeyNames="ActiveReleases" ShowHeader="True"
            RowStyle-VerticalAlign="Top" RowStyle-HorizontalAlign="Left" Width="1800px">
            <Columns>
                <asp:TemplateField ItemStyle-Width="30px">
                    <ItemTemplate>
                        <asp:Button ID="btnCollapse" Text="+" CssClass="collapseButton spinnertric" runat="server" OnClick="btnCollapse_Click" />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="left" DataField="ActiveReleases" HeaderText="Releases"></asp:BoundField>

                <asp:TemplateField>
                    <ItemTemplate>
                        <%--<img alt = "" style="cursor: pointer" src="images/plus.png" />--%>
                        <asp:Panel ID="Panel1" runat="server" Visible="false">

                            <asp:GridView ID="gvActiveBuilds" runat="server" AutoGenerateColumns="False" CssClass="ChildGrid"
                                OnRowDataBound="gvActiveBuilds_RowDataBound" RowStyle-VerticalAlign="Top" RowStyle-HorizontalAlign="Left" Width="1500px">
                                <Columns>
                                    <asp:TemplateField ItemStyle-Width="30px">
                                        <ItemTemplate>
                                            <asp:Button ID="btnNestedCollapse" Text="+" CssClass="collapseButton spinnertric" runat="server" OnClick="btnNestedCollapse_Click" />
                                        </ItemTemplate>
                                    </asp:TemplateField>


                                    <asp:BoundField ItemStyle-Width="30px" DataField="ID" HeaderText="ID" HeaderStyle-HorizontalAlign="Left" ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol" />
                                    <asp:BoundField DataField="Releases" HeaderText="Application" HeaderStyle-HorizontalAlign="Left" />
                                    <asp:TemplateField ItemStyle-Width="500px" HeaderText="Builds" HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:DropDownList ID="drpdwnBuilds" ControlStyle-CssClass="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpdwnBuilds_SelectedIndexChanged"></asp:DropDownList>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="" ItemStyle-Width="150px">
                                        <ItemTemplate>
                                            <asp:Button runat="server" ID="btnQueueNewBuild" Text="Queue New Build"
                                                ControlStyle-CssClass="btn-success ui-widget ui-state-default ui-corner-all spinnertric" OnClick="btnQueueNewBuild_Click" />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Logs" ItemStyle-Width="100px">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="linkBuildLog" Target="_blank" Font-Size="Small" ForeColor="#0645AD" runat="server" Visible="false" Text="Build."></asp:HyperLink>
                                            <asp:HyperLink ID="linkReleaseLog" Target="_blank" Font-Size="Small" ForeColor="#0645AD" runat="server" Visible="false" Text="Release."></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Panel ID="pnlRelease" runat="server" Visible="false">
                                                <asp:GridView ID="grdRelease" runat="server" AutoGenerateColumns="false" CssClass="Nested_ChildGrid" OnRowDataBound="grdRelease_RowDataBound" Width="600px">
                                                    <Columns>
                                                        <asp:BoundField ItemStyle-Width="150px" DataField="ReleaseID" HeaderText="Release ID" ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol" />
                                                        <asp:BoundField ItemStyle-Width="150px" DataField="EnvID" HeaderText="Environment ID" ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol" />
                                                        <asp:BoundField ItemStyle-Width="150px" DataField="Environments" HeaderText="Environments" />
                                                        <asp:BoundField ItemStyle-Width="250px" DataField="EnvStatus" HeaderText="Deployment Status" />
                                                        <%--<asp:ButtonField ButtonType="Button" CommandName="Update" ItemStyle-CssClass="paddy" 
                                    ControlStyle-CssClass="btn-success ui-widget ui-state-default ui-corner-all" HeaderText="Deploy" ShowHeader="False" Text="Deploy" />--%>
                                                        <asp:TemplateField HeaderText="Deploy">
                                                            <ItemTemplate>
                                                                <asp:Button runat="server" ID="btnDeploy" Text="Deploy" ItemStyle-CssClass="paddy"
                                                                    ControlStyle-CssClass="btn-success ui-widget ui-state-default ui-corner-all spinnertric " OnClick="btnDeploy_Click" class="spinnertric" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                    </Columns>
                                                </asp:GridView>
                                            </asp:Panel>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>

                        </asp:Panel>
                    </ItemTemplate>
                </asp:TemplateField>


            </Columns>
        </asp:GridView>


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

