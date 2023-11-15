// ReSharper disable InconsistentNaming

using JetBrains.Annotations;

namespace ADALReplacementTest;
public class JsonRefreshTokenResponse
{
#pragma warning disable IDE1006 // Naming Styles
    public string? access_token { [UsedImplicitly] get; [UsedImplicitly]  set; }
    public string? token_type { [UsedImplicitly] get; [UsedImplicitly] set; }
    public int expires_in { [UsedImplicitly] get; [UsedImplicitly] set; }
    public string? id_token { [UsedImplicitly] get; [UsedImplicitly] set; }

    [UsedImplicitly]
    public Tokens ToTokens(Tokens inToken)
    {
        return new Tokens
        {
            AccessToken = access_token ?? string.Empty,
            TokenType = token_type ?? string.Empty,
            ExpiresInSeconds = (uint)expires_in,
            IdToken = id_token ?? string.Empty,
            Resource = inToken.Resource,
            RefreshExpiresInSeconds = inToken.RefreshExpiresInSeconds,
            RefreshToken = inToken.RefreshToken
        };
    }
}
#pragma warning restore IDE1006 // Naming Styles