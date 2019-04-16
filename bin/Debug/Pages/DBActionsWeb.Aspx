<asp:Label Text="Description" ID="Descriptionlbl" runat="server" AssociatedControlID="Descriptiontxb" /><asp:TextBox ID="Descriptiontxb" runat="server" />
<asp:Label Text="Title" ID="Titlelbl" runat="server" AssociatedControlID="Titletxb" /><asp:TextBox ID="Titletxb" runat="server" /><asp:RequiredFieldValidator ID="TitleVal" runat="server" ControlToValidate="Titletxb" ErrorMessage="*" Text="*"/>
