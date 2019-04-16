private bool loadRecordInUI(Int64 clipid)
{
 DBClip oClip =new DBClip(clipid);
Value=oClip.Value;
MediaIDdrp.DataSource=Media.SelectAll();;
 MediaIDdrp.DataBind();
 MediaIDdrp.SelectedValue=oClip.MediaID;
MediaSegmentID=oClip.MediaSegmentID;
ClipType=oClip.ClipType;
URLtxb.Text=oClip.URL;
IsCompanyMentioedtxb.Text=oClip.IsCompanyMentioed;
IndustryIDdrp.DataSource=Industry.SelectAll();;
 IndustryIDdrp.DataBind();
 IndustryIDdrp.SelectedValue=oClip.IndustryID;
PostedBy=oClip.PostedBy;
IsPicturetxb.Text=oClip.IsPicture;
Size=oClip.Size;
Descriptiontxb.Text=oClip.Description;
PagesCount=oClip.PagesCount;
InLastPagetxb.Text=oClip.InLastPage;
ActionIDdrp.DataSource=Actions.SelectAll();;
 ActionIDdrp.DataBind();
 ActionIDdrp.SelectedValue=oClip.ActionID;
PublishDate=oClip.PublishDate;
ReporterIDtxb.Text=oClip.ReporterID;
IsCompanytxb.Text=oClip.IsCompany;
PostDate=oClip.PostDate;
IsCoverPagetxb.Text=oClip.IsCoverPage;
PageNotxb.Text=oClip.PageNo;
Status=oClip.Status;
Subject=oClip.Subject;
IsColoredtxb.Text=oClip.IsColored;
oClip.Dispose();
 oClip=null;
}
private bool saveRecord()
{
 DBClip oClip =new DBClip();
DBClip.Value=Single.Parse(Valuetxb.Text);
DBClip.MediaID=Int64.Parse(MediaIDdrp.SelectedValue);
DBClip.ClipBody=ClipBodyfil.FileBytes;
DBClip.MediaSegmentID=Int64.Parse(MediaSegmentIDtxb.Text);
DBClip.ClipType=Int32.Parse(ClipTypetxb.Text);
DBClip.URL=URLtxb.Text;
DBClip.IsCompanyMentioed=bool.Parse(IsCompanyMentioedtxb.Text);
DBClip.IndustryID=Int64.Parse(IndustryIDdrp.SelectedValue);
DBClip.PostedBy=Int64.Parse(PostedBytxb.Text);
DBClip.IsPicture=bool.Parse(IsPicturetxb.Text);
DBClip.Size=Int64.Parse(Sizetxb.Text);
DBClip.Description=Descriptiontxb.Text;
DBClip.PagesCount=Int32.Parse(PagesCounttxb.Text);
DBClip.InLastPage=bool.Parse(InLastPagetxb.Text);
DBClip.ActionID=Int64.Parse(ActionIDdrp.SelectedValue);
DBClip.PublishDate=DateTime.Parse(PublishDatetxb.Text);
DBClip.ReporterID=Int64.Parse(ReporterIDtxb.Text);
DBClip.IsCompany=bool.Parse(IsCompanytxb.Text);
DBClip.PostDate=DateTime.Parse(PostDatetxb.Text);
DBClip.IsCoverPage=bool.Parse(IsCoverPagetxb.Text);
DBClip.PageNo=Int32.Parse(PageNotxb.Text);
DBClip.Status=Int32.Parse(Statustxb.Text);
DBClip.Subject=Subjecttxb.Text;
DBClip.IsColored=bool.Parse(IsColoredtxb.Text);

 bool res=DBClip.Insert();
 DBClip.Dispose();
 return res;
}
private bool UpdateRecord(Int64 clipid)
{
 DBClip oClip =new DBClip(clipid);
DBClip.Value=Single.Parse(Valuetxb.Text);
DBClip.MediaID=Int64.Parse(MediaIDdrp.SelectedValue);
DBClip.ClipBody=ClipBodyfil.FileBytes;
DBClip.MediaSegmentID=Int64.Parse(MediaSegmentIDtxb.Text);
DBClip.ClipType=Int32.Parse(ClipTypetxb.Text);
DBClip.URL=URLtxb.Text;
DBClip.IsCompanyMentioed=bool.Parse(IsCompanyMentioedtxb.Text);
DBClip.IndustryID=Int64.Parse(IndustryIDdrp.SelectedValue);
DBClip.PostedBy=Int64.Parse(PostedBytxb.Text);
DBClip.IsPicture=bool.Parse(IsPicturetxb.Text);
DBClip.Size=Int64.Parse(Sizetxb.Text);
DBClip.Description=Descriptiontxb.Text;
DBClip.PagesCount=Int32.Parse(PagesCounttxb.Text);
DBClip.InLastPage=bool.Parse(InLastPagetxb.Text);
DBClip.ActionID=Int64.Parse(ActionIDdrp.SelectedValue);
DBClip.PublishDate=DateTime.Parse(PublishDatetxb.Text);
DBClip.ReporterID=Int64.Parse(ReporterIDtxb.Text);
DBClip.IsCompany=bool.Parse(IsCompanytxb.Text);
DBClip.PostDate=DateTime.Parse(PostDatetxb.Text);
DBClip.IsCoverPage=bool.Parse(IsCoverPagetxb.Text);
DBClip.PageNo=Int32.Parse(PageNotxb.Text);
DBClip.Status=Int32.Parse(Statustxb.Text);
DBClip.Subject=Subjecttxb.Text;
DBClip.IsColored=bool.Parse(IsColoredtxb.Text);

 bool res=DBClip.Update();
 DBClip.Dispose();
 return res;
}
