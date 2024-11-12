// RuLib 1.0.0
// Copyright (C) 2024, Adaruru

using System.Web.Helpers;

namespace RuLib.Util;

public static class CheckUtil
{
    // 身份識別驗證

    public static bool ValidateID(string id, out string message)
    {
        message = string.Empty;
        if (string.IsNullOrEmpty(id) || id.Length != 10)
        {
            message = "身分證字號格式不正確";
            return false;
        }
        var idPattern = @"^[a-zA-Z]{1}[0-9]{9}$";
        bool isValid = System.Text.RegularExpressions.Regex.IsMatch(id, idPattern);
        if (!isValid) message = "無效的身分證字號";
        return isValid;
    }

    public static bool ValidatePassport(string passport, out string message)
    {
        message = string.Empty;
        if (string.IsNullOrEmpty(passport) || passport.Length < 6 || passport.Length > 9)
        {
            message = "護照號碼格式不正確";
            return false;
        }
        bool isValid = /* 護照驗證邏輯 */ true;
        if (!isValid) message = "無效的護照號碼";
        return isValid;
    }

    public static bool ValidateCompanyID(string companyID, out string message)
    {
        message = string.Empty;
        if (string.IsNullOrEmpty(companyID) || companyID.Length != 8)
        {
            message = "公司統編格式不正確";
            return false;
        }
        bool isValid = /* 公司統編驗證邏輯 */ true;
        if (!isValid) message = "無效的公司統編";
        return isValid;
    }

    // 聯絡資訊驗證

    public static bool ValidatePhoneNumber(string phoneNumber, out string message)
    {
        message = string.Empty;
        if (string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length != 10 || !phoneNumber.StartsWith("09"))
        {
            message = "手機號碼格式不正確";
            return false;
        }
        bool isValid = /* 手機號碼驗證邏輯 */ true;
        if (!isValid) message = "無效的手機號碼";
        return isValid;
    }

    public static bool ValidateEmail(string email, out string message)
    {
        message = string.Empty;
        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        bool isValid = System.Text.RegularExpressions.Regex.IsMatch(email, emailPattern);
        if (!isValid) message = "無效的電子郵件地址";
        return isValid;
    }

    public static bool ValidateAddress(string address, out string message)
    {
        message = string.IsNullOrEmpty(address) ? "地址不能為空" : string.Empty;
        return !string.IsNullOrEmpty(address);
    }

    /// <summary>
    /// 信用卡號驗證
    /// </summary>
    /// <param name="creditCardNumber"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static bool ValidateCreditCard(string creditCardNumber, out string message)
    {
        message = string.Empty;
        var cardPattern = @"^\d{16}$";
        bool isValid = System.Text.RegularExpressions.Regex.IsMatch(creditCardNumber, cardPattern);
        if (!isValid) message = "無效的信用卡號";
        return isValid;
    }

    /// <summary>
    /// 銀行帳號驗證
    /// </summary>
    /// <param name="bankAccount"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static bool ValidateBankAccount(string bankAccount, out string message)
    {
        message = string.IsNullOrEmpty(bankAccount) ? "銀行帳戶不能為空" : string.Empty;
        return !string.IsNullOrEmpty(bankAccount);
    }

    /// <summary>
    /// 通貨代碼驗證
    /// </summary>
    /// <param name="currencyCode"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static bool ValidateCurrency(string currencyCode, out string message)
    {
        message = string.Empty;
        bool isValid = currencyCode.Length == 3 && currencyCode.ToUpper() == currencyCode;
        if (!isValid) message = "無效的貨幣代碼";
        return isValid;
    }

    /// <summary>
    /// 日期驗證
    /// </summary>
    /// <param name="date"></param>
    /// <param name="message"></param>
    /// <returns></returns>

    public static bool ValidateDate(string date, out string message)
    {
        message = DateTime.TryParse(date, out _) ? string.Empty : "無效的日期格式";
        return string.IsNullOrEmpty(message);
    }

    /// <summary>
    /// url驗證
    /// </summary>
    /// <param name="url"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static bool ValidateURL(string url, out string message)
    {
        message = string.Empty;
        var urlPattern = @"^https?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$";
        bool isValid = System.Text.RegularExpressions.Regex.IsMatch(url, urlPattern);
        if (!isValid) message = "無效的 URL";
        return isValid;
    }

    /// <summary>
    /// IP地址驗證
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static bool ValidateIPAddress(string ipAddress, out string message)
    {
        message = System.Net.IPAddress.TryParse(ipAddress, out _) ? string.Empty : "無效的 IP 地址";
        return string.IsNullOrEmpty(message);
    }

    public static bool ValidatePostalCode(string postalCode, string country, out string message)
    {
        message = string.Empty;
        bool isValid = /* 郵遞區號檢核邏輯依據國家不同處理 */ true;
        if (!isValid) message = "無效的郵遞區號";
        return isValid;
    }



    public static bool ValidatePositiveInteger(string number, out string message)
    {
        message = string.Empty;
        bool isValid = int.TryParse(number, out int result) && result > 0;
        if (!isValid) message = "無效的正整數";
        return isValid;
    }

    public static bool ValidateDecimal(string decimalNumber, int precision, out string message)
    {
        message = string.Empty;
        bool isValid = decimal.TryParse(decimalNumber, out decimal result) && result >= 0;
        if (isValid && decimalNumber.Contains("."))
        {
            int actualPrecision = decimalNumber.Split('.')[1].Length;
            isValid = actualPrecision <= precision;
            if (!isValid) message = $"小數精度超過允許範圍 ({precision} 位)";
        }
        if (!isValid) message = "無效的小數格式";
        return isValid;
    }

    public static bool ValidateRange(int number, int min, int max, out string message)
    {
        message = string.Empty;
        bool isValid = number >= min && number <= max;
        if (!isValid) message = $"數值不在範圍內 ({min} 到 {max})";
        return isValid;
    }
}