﻿<%@ Page Title="Clone Build and Release Definitions" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Clone.aspx.cs" Inherits="TFSAdminDashboard._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Content/bootstrap.css" rel="stylesheet" />
    <div class="row" style="text-align: center; width: 100%; padding-left: 2em;">
        <br />
        <div class="form-inline ">
            <asp:Label ID="lblEnterReleaseNum" runat="server" Text="Enter Release Number" CssClass="relesetext" />

            <asp:TextBox ID="txbRelease" runat="server" CssClass="textBox"></asp:TextBox>
            &nbsp;&nbsp;&nbsp;
           <asp:Button ID="btnCloneBuilds" runat="server" Text="Clone Build and Releases" OnClick="btnCloneBuilds_Click" class="btn-success ui-widget ui-state-default ui-corner-all spinnertric" />
            <br />
        </div>

    </div>


    <div style="padding: 2em 2em 2em 2em">
        <div class="entryPanel">
            <asp:CheckBoxList ID="cblistBuilds" runat="server" RepeatColumns="4" CssClass="checkboxlist"></asp:CheckBoxList>
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
