private bool loadRecordInUI(Int64 clientid)
{
 DBClient oClient =new DBClient(clientid);
Addresstxb.Text=oClient.Address;
Teltxb.Text=oClient.Tel;
Name=oClient.Name;
Faxtxb.Text=oClient.Fax;
oClient.Dispose();
 oClient=null;
}
private bool saveRecord()
{
 DBClient oClient =new DBClient();
DBClient.Address=Addresstxb.Text;
DBClient.Tel=Teltxb.Text;
DBClient.Name=Nametxb.Text;
DBClient.Fax=Faxtxb.Text;
DBClient.Logo=Logofil.HasFile ? Logo.fil.FileBytes : null;

 bool res=DBClient.Insert();
 DBClient.Dispose();
 return res;
}
private bool UpdateRecord(Int64 clientid)
{
 DBClient oClient =new DBClient(clientid);
DBClient.Address=Addresstxb.Text;
DBClient.Tel=Teltxb.Text;
DBClient.Name=Nametxb.Text;
DBClient.Fax=Faxtxb.Text;
DBClient.Logo=Logofil.HasFile ? Logo.fil.FileBytes : null;

 bool res=DBClient.Update();
 DBClient.Dispose();
 return res;
}
