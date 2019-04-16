private bool loadRecordInUI(Int64 languageid)
{
 DBLanguage oLanguage =new DBLanguage(languageid);
Titletxb.Text=oLanguage.Title;
oLanguage.Dispose();
 oLanguage=null;
}
private bool saveRecord()
{
 DBLanguage oLanguage =new DBLanguage();
DBLanguage.Title=Titletxb.Text;

 bool res=DBLanguage.Insert();
 DBLanguage.Dispose();
 return res;
}
private bool UpdateRecord(Int64 languageid)
{
 DBLanguage oLanguage =new DBLanguage(languageid);
DBLanguage.Title=Titletxb.Text;

 bool res=DBLanguage.Update();
 DBLanguage.Dispose();
 return res;
}
