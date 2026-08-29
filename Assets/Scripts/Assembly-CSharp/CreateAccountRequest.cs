using System;
using System.Collections;

public class CreateAccountRequest : BaseNetRequest
{
	public enum ServerLanguage { eEnglish = 1, ePortuguese = 2, eFrench = 4, eSpanish = 8 }

	protected override void CreateRequiredResultKeyList()
	{
		this.m_RequiredResultKeys.Add("authToken");
	}

	public BaseNetRequest.Message BuildRequestMessage(string aUserName, string aPassword, string aEmail, int aColor)
	{
		int num = (int)this.ConvertToServerLanguageCode(LocalizationManager.GetLanguageCode());
		BaseNetRequest.Message message = new BaseNetRequest.Message("/mobileas/api/json/account/create_account");
		message.AddParameter("appVersion", "pl-1.0");
		message.AddParameter("user", aUserName);
		message.AddParameter("pass", aPassword);
		message.AddParameter("email", aEmail);
		message.AddParameter("color", aColor.ToString());
		message.AddParameter("lang", num.ToString());
		return message;
	}

	private CreateAccountRequest.ServerLanguage ConvertToServerLanguageCode(string aLanguage)
	{
		switch (aLanguage)
		{
			case "fr":
				return CreateAccountRequest.ServerLanguage.eFrench;
			case "es":
				return CreateAccountRequest.ServerLanguage.eSpanish;
			case "pt":
				return CreateAccountRequest.ServerLanguage.ePortuguese;
		}
		return CreateAccountRequest.ServerLanguage.eEnglish;
	}

	protected override void OnFail(Hashtable aResult) {}

	protected override void OnSuccess(Hashtable aResult)
	{
		NetManager.Instance.UpdateAuthToken(aResult["authToken"] as string);
	}
}
