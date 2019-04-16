private bool loadRecordInUI(Int64 actionid)
{
 DBActions oActions =new DBActions(actionid);
Descriptiontxb.Text=oActions.Description;
Title=oActions.Title;
oActions.Dispose();
 oActions=null;
}
private bool saveRecord()
{
 DBActions oActions =new DBActions();
DBActions.Description=Descriptiontxb.Text;
DBActions.Title=Titletxb.Text;

 bool res=DBActions.Insert();
 DBActions.Dispose();
 return res;
}
private bool UpdateRecord(Int64 actionid)
{
 DBActions oActions =new DBActions(actionid);
DBActions.Description=Descriptiontxb.Text;
DBActions.Title=Titletxb.Text;

 bool res=DBActions.Update();
 DBActions.Dispose();
 return res;
}
