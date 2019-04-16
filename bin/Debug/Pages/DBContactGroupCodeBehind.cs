private bool loadRecordInUI(Int64 contactgroupid)
{
 DBContactGroup oContactGroup =new DBContactGroup(contactgroupid);
IsClientInternal=oContactGroup.IsClientInternal;
ClientIDdrp.DataSource=Client.SelectAll();;
 ClientIDdrp.DataBind();
 ClientIDdrp.SelectedValue=oContactGroup.ClientID;
Title=oContactGroup.Title;
oContactGroup.Dispose();
 oContactGroup=null;
}
private bool saveRecord()
{
 DBContactGroup oContactGroup =new DBContactGroup();
DBContactGroup.IsClientInternal=bool.Parse(IsClientInternaltxb.Text);
DBContactGroup.ClientID=Int64.Parse(ClientIDdrp.SelectedValue);
DBContactGroup.Title=Titletxb.Text;

 bool res=DBContactGroup.Insert();
 DBContactGroup.Dispose();
 return res;
}
private bool UpdateRecord(Int64 contactgroupid)
{
 DBContactGroup oContactGroup =new DBContactGroup(contactgroupid);
DBContactGroup.IsClientInternal=bool.Parse(IsClientInternaltxb.Text);
DBContactGroup.ClientID=Int64.Parse(ClientIDdrp.SelectedValue);
DBContactGroup.Title=Titletxb.Text;

 bool res=DBContactGroup.Update();
 DBContactGroup.Dispose();
 return res;
}
