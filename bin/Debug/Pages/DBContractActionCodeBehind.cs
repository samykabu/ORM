private bool loadRecordInUI(Int64 actionid,Int64 contractid)
{
 DBContractAction oContractAction =new DBContractAction(actionid,contractid);
ContractCount=oContractAction.ContractCount;
DueIntxb.Text=oContractAction.DueIn;
StartIntxb.Text=oContractAction.StartIn;
ContractActionID=oContractAction.ContractActionID;
CurrentCount=oContractAction.CurrentCount;
oContractAction.Dispose();
 oContractAction=null;
}
private bool saveRecord()
{
 DBContractAction oContractAction =new DBContractAction();
DBContractAction.ContractCount=Int32.Parse(ContractCounttxb.Text);
DBContractAction.DueIn=DateTime.Parse(DueIntxb.Text);
DBContractAction.StartIn=DateTime.Parse(StartIntxb.Text);
DBContractAction.ContractActionID=Int64.Parse(ContractActionIDtxb.Text);
DBContractAction.CurrentCount=Int32.Parse(CurrentCounttxb.Text);

 bool res=DBContractAction.Insert();
 DBContractAction.Dispose();
 return res;
}
private bool UpdateRecord(Int64 actionid,Int64 contractid)
{
 DBContractAction oContractAction =new DBContractAction(actionid,contractid);
DBContractAction.ContractCount=Int32.Parse(ContractCounttxb.Text);
DBContractAction.DueIn=DateTime.Parse(DueIntxb.Text);
DBContractAction.StartIn=DateTime.Parse(StartIntxb.Text);
DBContractAction.ContractActionID=Int64.Parse(ContractActionIDtxb.Text);
DBContractAction.CurrentCount=Int32.Parse(CurrentCounttxb.Text);

 bool res=DBContractAction.Update();
 DBContractAction.Dispose();
 return res;
}
