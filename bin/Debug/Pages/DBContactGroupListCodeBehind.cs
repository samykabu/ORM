private bool loadRecordInUI(Int64 contactgrouplistid)
{
 DBContactGroupList oContactGroupList =new DBContactGroupList(contactgrouplistid);
ContcatIDdrp.DataSource=Contact.SelectAll();;
 ContcatIDdrp.DataBind();
 ContcatIDdrp.SelectedValue=oContactGroupList.ContcatID;
ContactGroupIDdrp.DataSource=ContactGroup.SelectAll();;
 ContactGroupIDdrp.DataBind();
 ContactGroupIDdrp.SelectedValue=oContactGroupList.ContactGroupID;
oContactGroupList.Dispose();
 oContactGroupList=null;
}
private bool saveRecord()
{
 DBContactGroupList oContactGroupList =new DBContactGroupList();
DBContactGroupList.ContcatID=Int64.Parse(ContcatIDdrp.SelectedValue);
DBContactGroupList.ContactGroupID=Int64.Parse(ContactGroupIDdrp.SelectedValue);

 bool res=DBContactGroupList.Insert();
 DBContactGroupList.Dispose();
 return res;
}
private bool UpdateRecord(Int64 contactgrouplistid)
{
 DBContactGroupList oContactGroupList =new DBContactGroupList(contactgrouplistid);
DBContactGroupList.ContcatID=Int64.Parse(ContcatIDdrp.SelectedValue);
DBContactGroupList.ContactGroupID=Int64.Parse(ContactGroupIDdrp.SelectedValue);

 bool res=DBContactGroupList.Update();
 DBContactGroupList.Dispose();
 return res;
}
