namespace ADALReplacementTest;
internal class AuthorizationCodeResult
{
    public string AuthorizationCode { get; init; } = string.Empty;
    public AuthorizationCodeStatuses Result { get; init; } = AuthorizationCodeStatuses.Failed;
}
