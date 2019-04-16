private bool loadRecordInUI(Int64 contactid)
{
 DBContact oContact =new DBContact(contactid);
Mobile=oContact.Mobile;
FirstName=oContact.FirstName;
MNametxb.Text=oContact.MName;
Email=oContact.Email;
Phonetxb.Text=oContact.Phone;
MediaIDdrp.DataSource=Media.SelectAll();;
 MediaIDdrp.DataBind();
 MediaIDdrp.SelectedValue=oContact.MediaID;
Faxtxb.Text=oContact.Fax;
LName=oContact.LName;
Company=oContact.Company;
IsReporter=oContact.IsReporter;
oContact.Dispose();
 oContact=null;
}
private bool saveRecord()
{
 DBContact oContact =new DBContact();
DBContact.Mobile=Mobiletxb.Text;
DBContact.FirstName=FirstNametxb.Text;
DBContact.MName=MNametxb.Text;
DBContact.Email=Emailtxb.Text;
DBContact.Phone=Phonetxb.Text;
DBContact.MediaID=Int64.Parse(MediaIDdrp.SelectedValue);
DBContact.Fax=Faxtxb.Text;
DBContact.LName=LNametxb.Text;
DBContact.Company=Companytxb.Text;
DBContact.IsReporter=bool.Parse(IsReportertxb.Text);

 bool res=DBContact.Insert();
 DBContact.Dispose();
 return res;
}
private bool UpdateRecord(Int64 contactid)
{
 DBContact oContact =new DBContact(contactid);
DBContact.Mobile=Mobiletxb.Text;
DBContact.FirstName=FirstNametxb.Text;
DBContact.MName=MNametxb.Text;
DBContact.Email=Emailtxb.Text;
DBContact.Phone=Phonetxb.Text;
DBContact.MediaID=Int64.Parse(MediaIDdrp.SelectedValue);
DBContact.Fax=Faxtxb.Text;
DBContact.LName=LNametxb.Text;
DBContact.Company=Companytxb.Text;
DBContact.IsReporter=bool.Parse(IsReportertxb.Text);

 bool res=DBContact.Update();
 DBContact.Dispose();
 return res;
}
