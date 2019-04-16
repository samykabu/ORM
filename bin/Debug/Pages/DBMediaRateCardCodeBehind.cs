private bool loadRecordInUI(Int64 mediaratecardid)
{
 DBMediaRateCard oMediaRateCard =new DBMediaRateCard(mediaratecardid);
MediaIDdrp.DataSource=Media.SelectAll();;
 MediaIDdrp.DataBind();
 MediaIDdrp.SelectedValue=oMediaRateCard.MediaID;
Title=oMediaRateCard.Title;
Value=oMediaRateCard.Value;
oMediaRateCard.Dispose();
 oMediaRateCard=null;
}
private bool saveRecord()
{
 DBMediaRateCard oMediaRateCard =new DBMediaRateCard();
DBMediaRateCard.MediaID=Int64.Parse(MediaIDdrp.SelectedValue);
DBMediaRateCard.Title=Titletxb.Text;
DBMediaRateCard.Value=double.Parse(Valuetxb.Text);

 bool res=DBMediaRateCard.Insert();
 DBMediaRateCard.Dispose();
 return res;
}
private bool UpdateRecord(Int64 mediaratecardid)
{
 DBMediaRateCard oMediaRateCard =new DBMediaRateCard(mediaratecardid);
DBMediaRateCard.MediaID=Int64.Parse(MediaIDdrp.SelectedValue);
DBMediaRateCard.Title=Titletxb.Text;
DBMediaRateCard.Value=double.Parse(Valuetxb.Text);

 bool res=DBMediaRateCard.Update();
 DBMediaRateCard.Dispose();
 return res;
}
