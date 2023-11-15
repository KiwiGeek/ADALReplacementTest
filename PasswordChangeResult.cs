namespace ADALReplacementTest;

public enum PasswordChangeResult
{
    Success = 0,
    CollectingInput = 1,
    InvalidCredentials = 2,
    PasswordComplexityFailed = 3,
    UnknownError = 4
}