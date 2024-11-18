// RuLib 1.0.0
// Copyright (C) 2024, Adaruru

using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace RuLib.JsonI18n;

public class MailAttribute : ValidationAttribute
{
    public EmailValidationOptions Options { get; set; } = EmailValidationOptions.None;
    public bool AllowUppercase { get; set; } = false;
    public bool AllowSpecialCharacters { get; set; } = true;

    private string EmailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                                       @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";


    /// <summary>
    /// [RegularExpression(@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$", ErrorMessage = "請確認Email格式")]
    /// </summary>
    /// <param name="value"></param>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var email = value as string;

        if (!AllowUppercase)
        {
            EmailRegex =
        }

        if (!AllowUppercase)
        {
            EmailRegex = @"";
        }

        if (string.IsNullOrEmpty(email))
        {
            return ValidationResult.Success; // 空值可以通過（Required 應由其他驗證處理）
        }

        // 驗證基本格式
        if (!Regex.IsMatch(email, EmailRegex, RegexOptions.IgnoreCase))
        {
            return new ValidationResult(ErrorMessage ?? "請確認Email格式");
        }

        // 驗證大小寫限制
        if (!AllowUppercase)
        {

            return new ValidationResult(ErrorMessage ?? "電子郵件不可包含大寫字母");
        }

        // 驗證特殊字元限制
        if (!AllowSpecialCharacters)
        {

            return new ValidationResult(ErrorMessage ?? "電子郵件不可包含特殊字元");
        }

        return ValidationResult.Success;
    }
}
[Flags]
public enum EmailValidationOptions
{
    None = 0,                       // 無任何檢查
    AllowUpperCase = 1 << 0,        // 允許 @ 前有大寫字母
    RequireLocalPartStartWithAlpha = 1 << 1, // 本地部分必須以字母開頭
    DisallowConsecutiveDots = 1 << 2,  // 禁止連續的點
    AllowQuotedLocalPart = 1 << 3,   // 允許本地部分有引號
    RequireDomain = 1 << 4,         // 必須有域名部分
    AllowIpAddressDomain = 1 << 5,  // 允許使用 IP 地址作為域名
    RequireTld = 1 << 6,            // 必須有頂級域名（如 .com）
    RestrictDomainToLowerCase = 1 << 7, // 域名部分必須全小寫
    RestrictLength = 1 << 8         // 限制整體長度
}

public class  Test{
    [Mail(Options=)]
    public string Mail { get; set; }

}