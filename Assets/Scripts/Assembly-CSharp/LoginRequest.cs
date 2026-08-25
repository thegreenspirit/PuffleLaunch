using System;
using System.Collections;

public class LoginRequest : BaseNetRequest
{
	protected override void CreateRequiredResultKeyList()
	{
		this.m_RequiredResultKeys.Add("authToken");
		this.m_RequiredResultKeys.Add("color");
	}

	public BaseNetRequest.Message BuildRequestMessage(string aUserName, string aPassword)
	{
		BaseNetRequest.Message message = new BaseNetRequest.Message("/mobileas/api/json/account/login");
		message.AddParameter("appVersion", "pl-1.0");
		message.AddParameter("user", aUserName);
		message.AddParameter("pass", aPassword);
		return message;
	}

	protected override void OnFail(Hashtable aResult) {}

	protected override void OnSuccess(Hashtable aResult)
	{
		ProfileManager.Instance.CurrentProfile.AuthToken = aResult["authToken"] as string;
	}
}
