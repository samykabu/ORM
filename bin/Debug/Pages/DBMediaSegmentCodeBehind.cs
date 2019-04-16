private bool loadRecordInUI(Int64 mediasegmentid)
{
 DBMediaSegment oMediaSegment =new DBMediaSegment(mediasegmentid);
MediaIDdrp.DataSource=Media.SelectAll();;
 MediaIDdrp.DataBind();
 MediaIDdrp.SelectedValue=oMediaSegment.MediaID;
Title=oMediaSegment.Title;
oMediaSegment.Dispose();
 oMediaSegment=null;
}
private bool saveRecord()
{
 DBMediaSegment oMediaSegment =new DBMediaSegment();
DBMediaSegment.MediaID=Int64.Parse(MediaIDdrp.SelectedValue);
DBMediaSegment.Title=Titletxb.Text;

 bool res=DBMediaSegment.Insert();
 DBMediaSegment.Dispose();
 return res;
}
private bool UpdateRecord(Int64 mediasegmentid)
{
 DBMediaSegment oMediaSegment =new DBMediaSegment(mediasegmentid);
DBMediaSegment.MediaID=Int64.Parse(MediaIDdrp.SelectedValue);
DBMediaSegment.Title=Titletxb.Text;

 bool res=DBMediaSegment.Update();
 DBMediaSegment.Dispose();
 return res;
}
