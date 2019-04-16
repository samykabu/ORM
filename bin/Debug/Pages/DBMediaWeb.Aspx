<asp:Label Text="Name" ID="Namelbl" runat="server" AssociatedControlID="Nametxb" /><asp:TextBox ID="Nametxb" runat="server" /><asp:RequiredFieldValidator ID="NameVal" runat="server" ControlToValidate="Nametxb" ErrorMessage="*" Text="*"/>
<asp:Label Text="Description" ID="Descriptionlbl" runat="server" AssociatedControlID="Descriptiontxb" /><asp:TextBox ID="Descriptiontxb" runat="server" />
<asp:Label Text="MediaDistributionTypeID" ID="MediaDistributionTypeIDlbl" runat="server" AssociatedControlID="MediaDistributionTypeIDdrp" /><asp:DropDownList ID="MediaDistributionTypeIDdrp" runat="server" /><asp:RequiredFieldValidator ID="MediaDistributionTypeIDVal" runat="server" ControlToValidate="MediaDistributionTypeIDdrp" ErrorMessage="*" Text="*"/>
<asp:Label Text="Logo" ID="Logolbl" runat="server" AssociatedControlID="Logofil" /><asp:FileUpload ID="Logofil" runat="server" />
<asp:Label Text="Country" ID="Countrylbl" runat="server" AssociatedControlID="Countrytxb" /><asp:TextBox ID="Countrytxb" runat="server" />
<asp:Label Text="MediatypeID" ID="MediatypeIDlbl" runat="server" AssociatedControlID="MediatypeIDdrp" /><asp:DropDownList ID="MediatypeIDdrp" runat="server" /><asp:RequiredFieldValidator ID="MediatypeIDVal" runat="server" ControlToValidate="MediatypeIDdrp" ErrorMessage="*" Text="*"/>
<asp:Label Text="Circulation" ID="Circulationlbl" runat="server" AssociatedControlID="Circulationtxb" /><asp:TextBox ID="Circulationtxb" runat="server" />
<asp:Label Text="Language" ID="Languagelbl" runat="server" AssociatedControlID="Languagedrp" /><asp:DropDownList ID="Languagedrp" runat="server" /><asp:RequiredFieldValidator ID="LanguageVal" runat="server" ControlToValidate="Languagedrp" ErrorMessage="*" Text="*"/>
<asp:Label Text="ReaderShip" ID="ReaderShiplbl" runat="server" AssociatedControlID="ReaderShiptxb" /><asp:TextBox ID="ReaderShiptxb" runat="server" />
<asp:Label Text="URL" ID="URLlbl" runat="server" AssociatedControlID="URLtxb" /><asp:TextBox ID="URLtxb" runat="server" />
