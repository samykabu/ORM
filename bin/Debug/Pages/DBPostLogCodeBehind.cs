private bool loadRecordInUI(Int64 postid)
{
 DBPostLog oPostLog =new DBPostLog(postid);
SourceType=oPostLog.SourceType;
IsGroup=oPostLog.IsGroup;
SourceID=oPostLog.SourceID;
PstedDatetxb.Text=oPostLog.PstedDate;
PostedToIDdrp.DataSource=Contact.SelectAll();;
 PostedToIDdrp.DataBind();
 PostedToIDdrp.SelectedValue=oPostLog.PostedToID;
Status=oPostLog.Status;
LastTriedDatetxb.Text=oPostLog.LastTriedDate;
oPostLog.Dispose();
 oPostLog=null;
}
private bool saveRecord()
{
 DBPostLog oPostLog =new DBPostLog();
DBPostLog.SourceType=bool.Parse(SourceTypetxb.Text);
DBPostLog.IsGroup=bool.Parse(IsGrouptxb.Text);
DBPostLog.SourceID=Int64.Parse(SourceIDtxb.Text);
DBPostLog.PstedDate=DateTime.Parse(PstedDatetxb.Text);
DBPostLog.PostedToID=Int64.Parse(PostedToIDdrp.SelectedValue);
DBPostLog.Status=Int32.Parse(Statustxb.Text);
DBPostLog.LastTriedDate=DateTime.Parse(LastTriedDatetxb.Text);

 bool res=DBPostLog.Insert();
 DBPostLog.Dispose();
 return res;
}
private bool UpdateRecord(Int64 postid)
{
 DBPostLog oPostLog =new DBPostLog(postid);
DBPostLog.SourceType=bool.Parse(SourceTypetxb.Text);
DBPostLog.IsGroup=bool.Parse(IsGrouptxb.Text);
DBPostLog.SourceID=Int64.Parse(SourceIDtxb.Text);
DBPostLog.PstedDate=DateTime.Parse(PstedDatetxb.Text);
DBPostLog.PostedToID=Int64.Parse(PostedToIDdrp.SelectedValue);
DBPostLog.Status=Int32.Parse(Statustxb.Text);
DBPostLog.LastTriedDate=DateTime.Parse(LastTriedDatetxb.Text);

 bool res=DBPostLog.Update();
 DBPostLog.Dispose();
 return res;
}
