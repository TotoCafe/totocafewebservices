<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="deneme.aspx.cs" Inherits="totoCafeWebServices.deneme" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label Text="USERID" runat="server" /> <asp:TextBox ID="tbUserID" runat="server"></asp:TextBox>
        <asp:Label Text="CompanyID" runat="server" />  <asp:TextBox ID="tbCompanyID" runat="server"></asp:TextBox>
        <asp:Label Text="TableID" runat="server" /><asp:TextBox ID="tbTableID" runat="server"></asp:TextBox>
        <asp:Button Text="SEND" ID="btnResult" runat="server" OnClick="btnResult_Click" />
        <asp:Label Text="text" ID="lblStatus" runat="server" />
    </div>
    </form>
</body>
</html>
