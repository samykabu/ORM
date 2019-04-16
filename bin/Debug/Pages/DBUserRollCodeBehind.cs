private bool loadRecordInUI(Int64 userrollid)
{
 DBUserRoll oUserRoll =new DBUserRoll(userrollid);
UserIDdrp.DataSource=Users.SelectAll();;
 UserIDdrp.DataBind();
 UserIDdrp.SelectedValue=oUserRoll.UserID;
RollIDdrp.DataSource=Roll.SelectAll();;
 RollIDdrp.DataBind();
 RollIDdrp.SelectedValue=oUserRoll.RollID;
oUserRoll.Dispose();
 oUserRoll=null;
}
private bool saveRecord()
{
 DBUserRoll oUserRoll =new DBUserRoll();
DBUserRoll.UserID=Int64.Parse(UserIDdrp.SelectedValue);
DBUserRoll.RollID=Int64.Parse(RollIDdrp.SelectedValue);

 bool res=DBUserRoll.Insert();
 DBUserRoll.Dispose();
 return res;
}
private bool UpdateRecord(Int64 userrollid)
{
 DBUserRoll oUserRoll =new DBUserRoll(userrollid);
DBUserRoll.UserID=Int64.Parse(UserIDdrp.SelectedValue);
DBUserRoll.RollID=Int64.Parse(RollIDdrp.SelectedValue);

 bool res=DBUserRoll.Update();
 DBUserRoll.Dispose();
 return res;
}
