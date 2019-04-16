private bool loadRecordInUI(Int64 projectclipid)
{
 DBProjectClip oProjectClip =new DBProjectClip(projectclipid);
ProjectIDdrp.DataSource=Project.SelectAll();;
 ProjectIDdrp.DataBind();
 ProjectIDdrp.SelectedValue=oProjectClip.ProjectID;
ClipIDdrp.DataSource=Clip.SelectAll();;
 ClipIDdrp.DataBind();
 ClipIDdrp.SelectedValue=oProjectClip.ClipID;
AssignedDate=oProjectClip.AssignedDate;
oProjectClip.Dispose();
 oProjectClip=null;
}
private bool saveRecord()
{
 DBProjectClip oProjectClip =new DBProjectClip();
DBProjectClip.ProjectID=Int64.Parse(ProjectIDdrp.SelectedValue);
DBProjectClip.ClipID=Int64.Parse(ClipIDdrp.SelectedValue);
DBProjectClip.AssignedDate=DateTime.Parse(AssignedDatetxb.Text);

 bool res=DBProjectClip.Insert();
 DBProjectClip.Dispose();
 return res;
}
private bool UpdateRecord(Int64 projectclipid)
{
 DBProjectClip oProjectClip =new DBProjectClip(projectclipid);
DBProjectClip.ProjectID=Int64.Parse(ProjectIDdrp.SelectedValue);
DBProjectClip.ClipID=Int64.Parse(ClipIDdrp.SelectedValue);
DBProjectClip.AssignedDate=DateTime.Parse(AssignedDatetxb.Text);

 bool res=DBProjectClip.Update();
 DBProjectClip.Dispose();
 return res;
}
