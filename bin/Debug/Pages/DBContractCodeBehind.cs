private bool loadRecordInUI(Int64 contractid)
{
 DBContract oContract =new DBContract(contractid);
Startdate=oContract.Startdate;
Title=oContract.Title;
ClientIDdrp.DataSource=Client.SelectAll();;
 ClientIDdrp.DataBind();
 ClientIDdrp.SelectedValue=oContract.ClientID;
Descriptiontxb.Text=oContract.Description;
EndDate=oContract.EndDate;
oContract.Dispose();
 oContract=null;
}
private bool saveRecord()
{
 DBContract oContract =new DBContract();
DBContract.Startdate=DateTime.Parse(Startdatetxb.Text);
DBContract.Title=Titletxb.Text;
DBContract.ClientID=Int64.Parse(ClientIDdrp.SelectedValue);
DBContract.Description=Descriptiontxb.Text;
DBContract.EndDate=DateTime.Parse(EndDatetxb.Text);

 bool res=DBContract.Insert();
 DBContract.Dispose();
 return res;
}
private bool UpdateRecord(Int64 contractid)
{
 DBContract oContract =new DBContract(contractid);
DBContract.Startdate=DateTime.Parse(Startdatetxb.Text);
DBContract.Title=Titletxb.Text;
DBContract.ClientID=Int64.Parse(ClientIDdrp.SelectedValue);
DBContract.Description=Descriptiontxb.Text;
DBContract.EndDate=DateTime.Parse(EndDatetxb.Text);

 bool res=DBContract.Update();
 DBContract.Dispose();
 return res;
}
