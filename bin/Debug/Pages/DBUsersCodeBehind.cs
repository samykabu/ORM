private bool loadRecordInUI(Int64 userid)
{
 DBUsers oUsers =new DBUsers(userid);
Name=oUsers.Name;
LoginID=oUsers.LoginID;
LastLogintxb.Text=oUsers.LastLogin;
Password=oUsers.Password;
oUsers.Dispose();
 oUsers=null;
}
private bool saveRecord()
{
 DBUsers oUsers =new DBUsers();
DBUsers.Name=Nametxb.Text;
DBUsers.LoginID=LoginIDtxb.Text;
DBUsers.LastLogin=DateTime.Parse(LastLogintxb.Text);
DBUsers.Password=Passwordtxb.Text;

 bool res=DBUsers.Insert();
 DBUsers.Dispose();
 return res;
}
private bool UpdateRecord(Int64 userid)
{
 DBUsers oUsers =new DBUsers(userid);
DBUsers.Name=Nametxb.Text;
DBUsers.LoginID=LoginIDtxb.Text;
DBUsers.LastLogin=DateTime.Parse(LastLogintxb.Text);
DBUsers.Password=Passwordtxb.Text;

 bool res=DBUsers.Update();
 DBUsers.Dispose();
 return res;
}
