private bool loadRecordInUI(Int64 newsfeedid)
{
 DBNewsFeed oNewsFeed =new DBNewsFeed(newsfeedid);
PostedIntxb.Text=oNewsFeed.PostedIn;
CreatedBydrp.DataSource=Users.SelectAll();;
 CreatedBydrp.DataBind();
 CreatedBydrp.SelectedValue=oNewsFeed.CreatedBy;
AttachmentIDtxb.Text=oNewsFeed.AttachmentID;
ActionIDtxb.Text=oNewsFeed.ActionID;
Title=oNewsFeed.Title;
HasAttachment=oNewsFeed.HasAttachment;
FeedBody=oNewsFeed.FeedBody;
SendUsing=oNewsFeed.SendUsing;
ProjectID=oNewsFeed.ProjectID;
CreatedDate=oNewsFeed.CreatedDate;
Sataus=oNewsFeed.Sataus;
ContactIDtxb.Text=oNewsFeed.ContactID;
oNewsFeed.Dispose();
 oNewsFeed=null;
}
private bool saveRecord()
{
 DBNewsFeed oNewsFeed =new DBNewsFeed();
DBNewsFeed.PostedIn=DateTime.Parse(PostedIntxb.Text);
DBNewsFeed.CreatedBy=Int64.Parse(CreatedBydrp.SelectedValue);
DBNewsFeed.AttachmentID=Int64.Parse(AttachmentIDtxb.Text);
DBNewsFeed.ActionID=Int64.Parse(ActionIDtxb.Text);
DBNewsFeed.Title=Titletxb.Text;
DBNewsFeed.HasAttachment=bool.Parse(HasAttachmenttxb.Text);
DBNewsFeed.FeedBody=FeedBodytxb.Text;
DBNewsFeed.SendUsing=short.Parse(SendUsingtxb.Text);
DBNewsFeed.ProjectID=Int64.Parse(ProjectIDtxb.Text);
DBNewsFeed.CreatedDate=DateTime.Parse(CreatedDatetxb.Text);
DBNewsFeed.Sataus=Int32.Parse(Sataustxb.Text);
DBNewsFeed.ContactID=Int64.Parse(ContactIDtxb.Text);

 bool res=DBNewsFeed.Insert();
 DBNewsFeed.Dispose();
 return res;
}
private bool UpdateRecord(Int64 newsfeedid)
{
 DBNewsFeed oNewsFeed =new DBNewsFeed(newsfeedid);
DBNewsFeed.PostedIn=DateTime.Parse(PostedIntxb.Text);
DBNewsFeed.CreatedBy=Int64.Parse(CreatedBydrp.SelectedValue);
DBNewsFeed.AttachmentID=Int64.Parse(AttachmentIDtxb.Text);
DBNewsFeed.ActionID=Int64.Parse(ActionIDtxb.Text);
DBNewsFeed.Title=Titletxb.Text;
DBNewsFeed.HasAttachment=bool.Parse(HasAttachmenttxb.Text);
DBNewsFeed.FeedBody=FeedBodytxb.Text;
DBNewsFeed.SendUsing=short.Parse(SendUsingtxb.Text);
DBNewsFeed.ProjectID=Int64.Parse(ProjectIDtxb.Text);
DBNewsFeed.CreatedDate=DateTime.Parse(CreatedDatetxb.Text);
DBNewsFeed.Sataus=Int32.Parse(Sataustxb.Text);
DBNewsFeed.ContactID=Int64.Parse(ContactIDtxb.Text);

 bool res=DBNewsFeed.Update();
 DBNewsFeed.Dispose();
 return res;
}
