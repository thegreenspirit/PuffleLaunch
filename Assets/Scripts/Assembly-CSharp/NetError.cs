using System;
using System.Collections.Generic;

public static class NetError
{
	public static void CreateErrorCodeDictionary()
	{
		Utilities.AssertMsg(NetError.m_cErrorCodeDict == null, "Error Code Dictionary already created!");
		NetError.m_cErrorCodeDict = new Dictionary<int, string>();
		NetError.m_cErrorCodeDict[1] = "TXT_FailToConnect";
		NetError.m_cErrorCodeDict[2] = "TXT_NetworkError";
		NetError.m_cErrorCodeDict[3] = "TXT_NetworkError";
		NetError.m_cErrorCodeDict[4] = "TXT_ServerError";
		NetError.m_cErrorCodeDict[5] = "TXT_PasswordEmptyConfirm";
		NetError.m_cErrorCodeDict[6] = "TXT_PasswordMismatch";
		NetError.m_cErrorCodeDict[7] = "TXT_FailToConnect";
		NetError.m_cErrorCodeDict[-32070] = "TXT_ServerError";
		NetError.m_cErrorCodeDict[-32071] = "TXT_ServerError";
		NetError.m_cErrorCodeDict[-32072] = "TXT_ServerError";
		NetError.m_cErrorCodeDict[-32074] = "TXT_PlayerActive";
		NetError.m_cErrorCodeDict[-32076] = "TXT_ServerError";
		NetError.m_cErrorCodeDict[-32101] = "TXT_Error";
		NetError.m_cErrorCodeDict[-32102] = "TXT_ServerError";
		NetError.m_cErrorCodeDict[-32270] = "TXT_AccountNotFound";
		NetError.m_cErrorCodeDict[-32271] = "TXT_AuthorizationFailed";
		NetError.m_cErrorCodeDict[-32276] = "TXT_UserNameTaken";
		NetError.m_cErrorCodeDict[-32277] = "TXT_UserNameEmpty";
		NetError.m_cErrorCodeDict[-32278] = "TXT_UserNameTooShort";
		NetError.m_cErrorCodeDict[-32279] = "TXT_UserNameTooLong";
		NetError.m_cErrorCodeDict[-32280] = "TXT_UserNameTooManyNumbers";
		NetError.m_cErrorCodeDict[-32281] = "TXT_UserNameTooManySpaces";
		NetError.m_cErrorCodeDict[-32282] = "TXT_UserNameTooFewChars";
		NetError.m_cErrorCodeDict[-32283] = "TXT_UserNameWrongFormat";
		NetError.m_cErrorCodeDict[-32284] = "TXT_UserNameBannedWord";
		NetError.m_cErrorCodeDict[-32285] = "TXT_UserNameNotAllowed";
		NetError.m_cErrorCodeDict[-32286] = "TXT_PasswordEmpty";
		NetError.m_cErrorCodeDict[-32289] = "TXT_PasswordTooShort";
		NetError.m_cErrorCodeDict[-32290] = "TXT_PasswordTooLong";
		NetError.m_cErrorCodeDict[-32291] = "TXT_PasswordMatchesUserName";
		NetError.m_cErrorCodeDict[-32292] = "TXT_PasswordEasyToGuess";
		NetError.m_cErrorCodeDict[-32293] = "TXT_PasswordlsAfirstName";
		NetError.m_cErrorCodeDict[-32294] = "TXT_EmailEmpty";
		NetError.m_cErrorCodeDict[-32295] = "TXT_EmailWrongFormat";
		NetError.m_cErrorCodeDict[-32296] = "TXT_EmailWrongBannedDomain";
		NetError.m_cErrorCodeDict[-32297] = "TXT_EmailTooManyAccounts";
		NetError.m_cErrorCodeDict[-32299] = "TXT_EmailBadISP";
		NetError.m_cErrorCodeDict[-32298] = "TXT_ServerError";
		NetError.m_cErrorCodeDict[-32401] = "TXT_ReachedDailyLimit";
		NetError.m_cErrorCodeDict[-32602] = "TXT_ServerError";
		NetError.m_cErrorCodeDict[-32603] = "TXT_Error";
		NetError.m_cErrorCodeDict[-32301] = "TXT_ReceiptNotFound";
		NetError.m_cErrorCodeDict[-32302] = "TXT_InvalidGuestFlow";
		NetError.m_cErrorCodeDict[-32303] = "TXT_ReceiptMismatch";
		NetError.m_cErrorCodeDict[-32305] = "TXT_ReceiptRedeemedByAnother";
	}

	public static string GetErrorMsg(int aErrorCode)
	{
		string text;
		if (NetError.m_cErrorCodeDict.TryGetValue(aErrorCode, out text))
		{
			return LocalizationManager.Instance.GetString(text);
		}
		return "Error: " + aErrorCode.ToString();
	}

	public static string GetErrorMsgTextId(int aErrorCode)
	{
		string text;
		if (NetError.m_cErrorCodeDict.TryGetValue(aErrorCode, out text))
		{
			return text;
		}
		return "Error: " + aErrorCode.ToString();
	}

	public static bool IsUserNameRelatedError(int aErrorCode)
	{
		return (aErrorCode <= -32276 && aErrorCode >= -32285) || aErrorCode == -32270;
	}

	public static bool IsPasswordRelatedError(int aErrorCode)
	{
		return (aErrorCode <= -32286 && aErrorCode >= -32293) || aErrorCode == -32271;
	}

	public static bool IsPasswordMismatchError(int aErrorCode)
	{
		return aErrorCode == 6 || aErrorCode == 5;
	}

	public static bool IsEmailRelatedError(int aErrorCode)
	{
		return (aErrorCode <= -32294 && aErrorCode >= -32297) || aErrorCode == -32299;
	}

	private static Dictionary<int, string> m_cErrorCodeDict;

	public enum ClientError
	{
		eNone,
		eFailToConnectToServer,
		eServerTimeOut,
		eServerError,
		eMissingResultKey,
		ePasswordEmptyConfirm,
		ePasswordMismatch,
		eUnknown,
		eClientError_COUNT
	}

	public enum ServerError
	{
		eNone,
		ePlayerNotFound = -32070,
		ePlayerBanned = -32071,
		ePlayerBannedForever = -32072,
		ePlayerActive = -32074,
		eInvalidToken = -32076,
		eDataAccessError = -32101,
		eUnauthorizedAccess = -32102,
		eAccountNotFound = -32270,
		eAuthorizationFailed = -32271,
		eUserNameTaken = -32276,
		eUserNameEmpty = -32277,
		eUserNameTooShort = -32278,
		eUserNameTooLong = -32279,
		eUserNameTooManyNumbers = -32280,
		eUserNameTooManySpaces = -32281,
		eUserNameTooFewChars = -32282,
		eUserNameWrongFormat = -32283,
		eUserNameBannedWord = -32284,
		eUserNameNotAllowed = -32285,
		ePasswordEmpty = -32286,
		ePasswordTooShort = -32289,
		ePasswordTooLong = -32290,
		ePasswordMatchesUserName = -32291,
		ePasswordEasyToGuess = -32292,
		ePasswordIsAFirstName = -32293,
		eEmailEmpty = -32294,
		eEmailWrongFormat = -32295,
		eEmailWrongBannedDomain = -32296,
		eEmailTooManyAccounts = -32297,
		eEmailBadISP = -32299,
		eWrongPlayerColor,
		eReachedDailyLimit = -32401,
		eInvalidMissingParam = -32602,
		eInternalSystemError = -32603,
		eReceiptNotFound = -32301,
		eInvalidGuestFlow = -32302,
		eReceiptMismatch = -32303,
		eReceiptRedeemedByAnother = -32305
	}
}
