private bool loadRecordInUI(Int64 mediaid)
{
 DBMedia oMedia =new DBMedia(mediaid);
Name=oMedia.Name;
Descriptiontxb.Text=oMedia.Description;
MediaDistributionTypeIDdrp.DataSource=MediaDistributionType.SelectAll();;
 MediaDistributionTypeIDdrp.DataBind();
 MediaDistributionTypeIDdrp.SelectedValue=oMedia.MediaDistributionTypeID;
Countrytxb.Text=oMedia.Country;
MediatypeIDdrp.DataSource=MediaType.SelectAll();;
 MediatypeIDdrp.DataBind();
 MediatypeIDdrp.SelectedValue=oMedia.MediatypeID;
Circulationtxb.Text=oMedia.Circulation;
Languagedrp.DataSource=Language.SelectAll();;
 Languagedrp.DataBind();
 Languagedrp.SelectedValue=oMedia.Language;
ReaderShiptxb.Text=oMedia.ReaderShip;
URLtxb.Text=oMedia.URL;
oMedia.Dispose();
 oMedia=null;
}
private bool saveRecord()
{
 DBMedia oMedia =new DBMedia();
DBMedia.Name=Nametxb.Text;
DBMedia.Description=Descriptiontxb.Text;
DBMedia.MediaDistributionTypeID=Int64.Parse(MediaDistributionTypeIDdrp.SelectedValue);
DBMedia.Logo=Logofil.HasFile ? Logo.fil.FileBytes : null;
DBMedia.Country=Countrytxb.Text;
DBMedia.MediatypeID=Int64.Parse(MediatypeIDdrp.SelectedValue);
DBMedia.Circulation=Int64.Parse(Circulationtxb.Text);
DBMedia.Language=Int64.Parse(Languagedrp.SelectedValue);
DBMedia.ReaderShip=Int64.Parse(ReaderShiptxb.Text);
DBMedia.URL=URLtxb.Text;

 bool res=DBMedia.Insert();
 DBMedia.Dispose();
 return res;
}
private bool UpdateRecord(Int64 mediaid)
{
 DBMedia oMedia =new DBMedia(mediaid);
DBMedia.Name=Nametxb.Text;
DBMedia.Description=Descriptiontxb.Text;
DBMedia.MediaDistributionTypeID=Int64.Parse(MediaDistributionTypeIDdrp.SelectedValue);
DBMedia.Logo=Logofil.HasFile ? Logo.fil.FileBytes : null;
DBMedia.Country=Countrytxb.Text;
DBMedia.MediatypeID=Int64.Parse(MediatypeIDdrp.SelectedValue);
DBMedia.Circulation=Int64.Parse(Circulationtxb.Text);
DBMedia.Language=Int64.Parse(Languagedrp.SelectedValue);
DBMedia.ReaderShip=Int64.Parse(ReaderShiptxb.Text);
DBMedia.URL=URLtxb.Text;

 bool res=DBMedia.Update();
 DBMedia.Dispose();
 return res;
}
