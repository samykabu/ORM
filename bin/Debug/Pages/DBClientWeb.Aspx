<asp:Label Text="Address" ID="Addresslbl" runat="server" AssociatedControlID="Addresstxb" /><asp:TextBox ID="Addresstxb" runat="server" />
<asp:Label Text="Tel" ID="Tellbl" runat="server" AssociatedControlID="Teltxb" /><asp:TextBox ID="Teltxb" runat="server" />
<asp:Label Text="Name" ID="Namelbl" runat="server" AssociatedControlID="Nametxb" /><asp:TextBox ID="Nametxb" runat="server" /><asp:RequiredFieldValidator ID="NameVal" runat="server" ControlToValidate="Nametxb" ErrorMessage="*" Text="*"/>
<asp:Label Text="Fax" ID="Faxlbl" runat="server" AssociatedControlID="Faxtxb" /><asp:TextBox ID="Faxtxb" runat="server" />
<asp:Label Text="Logo" ID="Logolbl" runat="server" AssociatedControlID="Logofil" /><asp:FileUpload ID="Logofil" runat="server" />
