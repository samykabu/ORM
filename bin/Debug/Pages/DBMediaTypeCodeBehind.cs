private bool loadRecordInUI(Int64 mediatypeid)
{
 DBMediaType oMediaType =new DBMediaType(mediatypeid);
Title=oMediaType.Title;
oMediaType.Dispose();
 oMediaType=null;
}
private bool saveRecord()
{
 DBMediaType oMediaType =new DBMediaType();
DBMediaType.Title=Titletxb.Text;

 bool res=DBMediaType.Insert();
 DBMediaType.Dispose();
 return res;
}
private bool UpdateRecord(Int64 mediatypeid)
{
 DBMediaType oMediaType =new DBMediaType(mediatypeid);
DBMediaType.Title=Titletxb.Text;

 bool res=DBMediaType.Update();
 DBMediaType.Dispose();
 return res;
}
