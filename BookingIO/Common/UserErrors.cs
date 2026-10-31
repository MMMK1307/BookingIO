namespace BookingIO.Common;

public static class UserErrors {
    public const string LoginFailed = "Falha ao Realizar o Login";
    public const string SaveFailed = "Falha ao Salvar Usuário";
    public const string RoleFailed = "Falha em Alterar Permissão";
    private static Dictionary<string, string> _userErrors = new()
    {
        { "PasswordRequiresDigit", "Senha Requer ao menos um digito" },
        { "PasswordRequiresNonAlphanumeric", "Senha Requer ao menos um Simbolo" },
        { "PasswordTooShort", "Senha Requer ao menos 8 characteres" }
    };

    public static string Translate(string errorCode)
    {
        return _userErrors.GetValueOrDefault(errorCode, "");
    }
}
