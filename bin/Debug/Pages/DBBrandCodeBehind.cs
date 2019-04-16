private bool loadRecordInUI(Int64 brandid)
{
 DBBrand oBrand =new DBBrand(brandid);
CreatedBydrp.DataSource=Users.SelectAll();;
 CreatedBydrp.DataBind();
 CreatedBydrp.SelectedValue=oBrand.CreatedBy;
CreatedIn=oBrand.CreatedIn;
Name=oBrand.Name;
ClientIDdrp.DataSource=Client.SelectAll();;
 ClientIDdrp.DataBind();
 ClientIDdrp.SelectedValue=oBrand.ClientID;
Descriptiontxb.Text=oBrand.Description;
oBrand.Dispose();
 oBrand=null;
}
private bool saveRecord()
{
 DBBrand oBrand =new DBBrand();
DBBrand.CreatedBy=Int64.Parse(CreatedBydrp.SelectedValue);
DBBrand.CreatedIn=DateTime.Parse(CreatedIntxb.Text);
DBBrand.Name=Nametxb.Text;
DBBrand.ClientID=Int64.Parse(ClientIDdrp.SelectedValue);
DBBrand.Description=Descriptiontxb.Text;
DBBrand.Logo=Logofil.HasFile ? Logo.fil.FileBytes : null;

 bool res=DBBrand.Insert();
 DBBrand.Dispose();
 return res;
}
private bool UpdateRecord(Int64 brandid)
{
 DBBrand oBrand =new DBBrand(brandid);
DBBrand.CreatedBy=Int64.Parse(CreatedBydrp.SelectedValue);
DBBrand.CreatedIn=DateTime.Parse(CreatedIntxb.Text);
DBBrand.Name=Nametxb.Text;
DBBrand.ClientID=Int64.Parse(ClientIDdrp.SelectedValue);
DBBrand.Description=Descriptiontxb.Text;
DBBrand.Logo=Logofil.HasFile ? Logo.fil.FileBytes : null;

 bool res=DBBrand.Update();
 DBBrand.Dispose();
 return res;
}
