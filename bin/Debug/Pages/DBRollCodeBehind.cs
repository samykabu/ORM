private bool loadRecordInUI(Int64 rollid)
{
 DBRoll oRoll =new DBRoll(rollid);
Title=oRoll.Title;
Descriptiontxb.Text=oRoll.Description;
oRoll.Dispose();
 oRoll=null;
}
private bool saveRecord()
{
 DBRoll oRoll =new DBRoll();
DBRoll.Title=Titletxb.Text;
DBRoll.Description=Descriptiontxb.Text;

 bool res=DBRoll.Insert();
 DBRoll.Dispose();
 return res;
}
private bool UpdateRecord(Int64 rollid)
{
 DBRoll oRoll =new DBRoll(rollid);
DBRoll.Title=Titletxb.Text;
DBRoll.Description=Descriptiontxb.Text;

 bool res=DBRoll.Update();
 DBRoll.Dispose();
 return res;
}
