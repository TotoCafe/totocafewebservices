<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="deneme.aspx.cs" Inherits="totoCafeWebServices.deneme" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label Text="USERID" runat="server" />
            <asp:TextBox ID="tbUserID" runat="server"></asp:TextBox>
            <asp:Label Text="CompanyID" runat="server" />
            <asp:TextBox ID="tbCompanyID" runat="server"></asp:TextBox>
            <asp:Label Text="TableID" runat="server" /><asp:TextBox ID="tbTableID" runat="server"></asp:TextBox>
            <asp:Button Text="SEND" ID="btnResult" runat="server" OnClick="btnResult_Click" />
            <asp:Label Text="text" ID="lblStatus" runat="server" />
        </div>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Convert String to Date" />
        &nbsp;<asp:TextBox ID="tbStringToDate" runat="server"></asp:TextBox>
        <asp:GridView ID="GridView1" HeaderStyle-BackColor="#3AC0F2" HeaderStyle-ForeColor="White"
            runat="server" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField DataField="CategoryID" HeaderText="CategoryID" ItemStyle-Width="30" />
                <asp:BoundField DataField="CategoryName" HeaderText="CategoryName" ItemStyle-Width="150" />
                <asp:BoundField DataField="AvailabilityID" HeaderText="AvailabilityID" ItemStyle-Width="150" />
                <asp:BoundField DataField="CompanyID" HeaderText="CompanyID" ItemStyle-Width="150" />
            </Columns>
        </asp:GridView>

        <br />
        <br />

    </form>
</body>
</html>
