private bool loadRecordInUI(Int64 projectid)
{
 DBProject oProject =new DBProject(projectid);
CreatedBydrp.DataSource=Users.SelectAll();;
 CreatedBydrp.DataBind();
 CreatedBydrp.SelectedValue=oProject.CreatedBy;
Name=oProject.Name;
CreatedIn=oProject.CreatedIn;
BrandIDdrp.DataSource=Brand.SelectAll();;
 BrandIDdrp.DataBind();
 BrandIDdrp.SelectedValue=oProject.BrandID;
ContractIDdrp.DataSource=Contract.SelectAll();;
 ContractIDdrp.DataBind();
 ContractIDdrp.SelectedValue=oProject.ContractID;
oProject.Dispose();
 oProject=null;
}
private bool saveRecord()
{
 DBProject oProject =new DBProject();
DBProject.CreatedBy=Int64.Parse(CreatedBydrp.SelectedValue);
DBProject.Name=Nametxb.Text;
DBProject.CreatedIn=DateTime.Parse(CreatedIntxb.Text);
DBProject.BrandID=Int64.Parse(BrandIDdrp.SelectedValue);
DBProject.ContractID=Int64.Parse(ContractIDdrp.SelectedValue);

 bool res=DBProject.Insert();
 DBProject.Dispose();
 return res;
}
private bool UpdateRecord(Int64 projectid)
{
 DBProject oProject =new DBProject(projectid);
DBProject.CreatedBy=Int64.Parse(CreatedBydrp.SelectedValue);
DBProject.Name=Nametxb.Text;
DBProject.CreatedIn=DateTime.Parse(CreatedIntxb.Text);
DBProject.BrandID=Int64.Parse(BrandIDdrp.SelectedValue);
DBProject.ContractID=Int64.Parse(ContractIDdrp.SelectedValue);

 bool res=DBProject.Update();
 DBProject.Dispose();
 return res;
}
