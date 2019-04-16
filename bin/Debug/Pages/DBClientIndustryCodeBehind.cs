private bool loadRecordInUI(Int64 clientindustryid)
{
 DBClientIndustry oClientIndustry =new DBClientIndustry(clientindustryid);
IndustryIDdrp.DataSource=Industry.SelectAll();;
 IndustryIDdrp.DataBind();
 IndustryIDdrp.SelectedValue=oClientIndustry.IndustryID;
ClientIDdrp.DataSource=Client.SelectAll();;
 ClientIDdrp.DataBind();
 ClientIDdrp.SelectedValue=oClientIndustry.ClientID;
oClientIndustry.Dispose();
 oClientIndustry=null;
}
private bool saveRecord()
{
 DBClientIndustry oClientIndustry =new DBClientIndustry();
DBClientIndustry.IndustryID=Int64.Parse(IndustryIDdrp.SelectedValue);
DBClientIndustry.ClientID=Int64.Parse(ClientIDdrp.SelectedValue);

 bool res=DBClientIndustry.Insert();
 DBClientIndustry.Dispose();
 return res;
}
private bool UpdateRecord(Int64 clientindustryid)
{
 DBClientIndustry oClientIndustry =new DBClientIndustry(clientindustryid);
DBClientIndustry.IndustryID=Int64.Parse(IndustryIDdrp.SelectedValue);
DBClientIndustry.ClientID=Int64.Parse(ClientIDdrp.SelectedValue);

 bool res=DBClientIndustry.Update();
 DBClientIndustry.Dispose();
 return res;
}
