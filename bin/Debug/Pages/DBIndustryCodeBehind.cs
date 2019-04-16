private bool loadRecordInUI(Int64 industryid)
{
 DBIndustry oIndustry =new DBIndustry(industryid);
Name=oIndustry.Name;
oIndustry.Dispose();
 oIndustry=null;
}
private bool saveRecord()
{
 DBIndustry oIndustry =new DBIndustry();
DBIndustry.Name=Nametxb.Text;

 bool res=DBIndustry.Insert();
 DBIndustry.Dispose();
 return res;
}
private bool UpdateRecord(Int64 industryid)
{
 DBIndustry oIndustry =new DBIndustry(industryid);
DBIndustry.Name=Nametxb.Text;

 bool res=DBIndustry.Update();
 DBIndustry.Dispose();
 return res;
}
