private bool loadRecordInUI(Int64 mediadistributiontypeid)
{
 DBMediaDistributionType oMediaDistributionType =new DBMediaDistributionType(mediadistributiontypeid);
MediatypeIDdrp.DataSource=MediaType.SelectAll();;
 MediatypeIDdrp.DataBind();
 MediatypeIDdrp.SelectedValue=oMediaDistributionType.MediatypeID;
Title=oMediaDistributionType.Title;
oMediaDistributionType.Dispose();
 oMediaDistributionType=null;
}
private bool saveRecord()
{
 DBMediaDistributionType oMediaDistributionType =new DBMediaDistributionType();
DBMediaDistributionType.MediatypeID=Int64.Parse(MediatypeIDdrp.SelectedValue);
DBMediaDistributionType.Title=Titletxb.Text;

 bool res=DBMediaDistributionType.Insert();
 DBMediaDistributionType.Dispose();
 return res;
}
private bool UpdateRecord(Int64 mediadistributiontypeid)
{
 DBMediaDistributionType oMediaDistributionType =new DBMediaDistributionType(mediadistributiontypeid);
DBMediaDistributionType.MediatypeID=Int64.Parse(MediatypeIDdrp.SelectedValue);
DBMediaDistributionType.Title=Titletxb.Text;

 bool res=DBMediaDistributionType.Update();
 DBMediaDistributionType.Dispose();
 return res;
}
