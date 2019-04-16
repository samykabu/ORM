private bool loadRecordInUI(Int64 eventid)
{
 DBEvent oEvent =new DBEvent(eventid);
Message=oEvent.Message;
ActionIDdrp.DataSource=Actions.SelectAll();;
 ActionIDdrp.DataBind();
 ActionIDdrp.SelectedValue=oEvent.ActionID;
Title=oEvent.Title;
Eventdate=oEvent.Eventdate;
CreatedIn=oEvent.CreatedIn;
ProjectIDdrp.DataSource=Project.SelectAll();;
 ProjectIDdrp.DataBind();
 ProjectIDdrp.SelectedValue=oEvent.ProjectID;
PostedBydrp.DataSource=Users.SelectAll();;
 PostedBydrp.DataBind();
 PostedBydrp.SelectedValue=oEvent.PostedBy;
oEvent.Dispose();
 oEvent=null;
}
private bool saveRecord()
{
 DBEvent oEvent =new DBEvent();
DBEvent.Message=Messagetxb.Text;
DBEvent.ActionID=Int64.Parse(ActionIDdrp.SelectedValue);
DBEvent.Title=Titletxb.Text;
DBEvent.Eventdate=DateTime.Parse(Eventdatetxb.Text);
DBEvent.CreatedIn=DateTime.Parse(CreatedIntxb.Text);
DBEvent.ProjectID=Int64.Parse(ProjectIDdrp.SelectedValue);
DBEvent.PostedBy=Int64.Parse(PostedBydrp.SelectedValue);

 bool res=DBEvent.Insert();
 DBEvent.Dispose();
 return res;
}
private bool UpdateRecord(Int64 eventid)
{
 DBEvent oEvent =new DBEvent(eventid);
DBEvent.Message=Messagetxb.Text;
DBEvent.ActionID=Int64.Parse(ActionIDdrp.SelectedValue);
DBEvent.Title=Titletxb.Text;
DBEvent.Eventdate=DateTime.Parse(Eventdatetxb.Text);
DBEvent.CreatedIn=DateTime.Parse(CreatedIntxb.Text);
DBEvent.ProjectID=Int64.Parse(ProjectIDdrp.SelectedValue);
DBEvent.PostedBy=Int64.Parse(PostedBydrp.SelectedValue);

 bool res=DBEvent.Update();
 DBEvent.Dispose();
 return res;
}
