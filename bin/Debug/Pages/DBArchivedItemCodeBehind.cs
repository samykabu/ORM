private bool loadRecordInUI(Int64 archiveditemid)
{
 DBArchivedItem oArchivedItem =new DBArchivedItem(archiveditemid);
ActionIDtxb.Text=oArchivedItem.ActionID;
PostedIn=oArchivedItem.PostedIn;
PostedBydrp.DataSource=Users.SelectAll();;
 PostedBydrp.DataBind();
 PostedBydrp.SelectedValue=oArchivedItem.PostedBy;
ArchivedItemTypetxb.Text=oArchivedItem.ArchivedItemType;
MIME=oArchivedItem.MIME;
ProjectIDdrp.DataSource=Project.SelectAll();;
 ProjectIDdrp.DataBind();
 ProjectIDdrp.SelectedValue=oArchivedItem.ProjectID;
FileName=oArchivedItem.FileName;
oArchivedItem.Dispose();
 oArchivedItem=null;
}
private bool saveRecord()
{
 DBArchivedItem oArchivedItem =new DBArchivedItem();
DBArchivedItem.ActionID=Int64.Parse(ActionIDtxb.Text);
DBArchivedItem.PostedIn=DateTime.Parse(PostedIntxb.Text);
DBArchivedItem.PostedBy=Int64.Parse(PostedBydrp.SelectedValue);
DBArchivedItem.ArchivedItemType=short.Parse(ArchivedItemTypetxb.Text);
DBArchivedItem.MIME=MIMEtxb.Text;
DBArchivedItem.ProjectID=Int64.Parse(ProjectIDdrp.SelectedValue);
DBArchivedItem.FileName=FileNametxb.Text;
DBArchivedItem.FileBody=FileBodyfil.HasFile ? FileBody.fil.FileBytes : null;

 bool res=DBArchivedItem.Insert();
 DBArchivedItem.Dispose();
 return res;
}
private bool UpdateRecord(Int64 archiveditemid)
{
 DBArchivedItem oArchivedItem =new DBArchivedItem(archiveditemid);
DBArchivedItem.ActionID=Int64.Parse(ActionIDtxb.Text);
DBArchivedItem.PostedIn=DateTime.Parse(PostedIntxb.Text);
DBArchivedItem.PostedBy=Int64.Parse(PostedBydrp.SelectedValue);
DBArchivedItem.ArchivedItemType=short.Parse(ArchivedItemTypetxb.Text);
DBArchivedItem.MIME=MIMEtxb.Text;
DBArchivedItem.ProjectID=Int64.Parse(ProjectIDdrp.SelectedValue);
DBArchivedItem.FileName=FileNametxb.Text;
DBArchivedItem.FileBody=FileBodyfil.HasFile ? FileBody.fil.FileBytes : null;

 bool res=DBArchivedItem.Update();
 DBArchivedItem.Dispose();
 return res;
}
