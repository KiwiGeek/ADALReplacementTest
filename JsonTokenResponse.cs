using JetBrains.Annotations;
// ReSharper disable InconsistentNaming

namespace ADALReplacementTest;

public class JsonTokenResponse
{
#pragma warning disable IDE1006 // Naming Styles
    public string? access_token { get; [UsedImplicitly] set; }
    public string? token_type { get; [UsedImplicitly] set; }
    public int expires_in { get; set; }
    public string? resource { get; [UsedImplicitly] set; }
    public string? refresh_token { get; [UsedImplicitly] set; }
    public int refresh_token_expires_in { get; set; }
    public string? id_token { get; [UsedImplicitly] set; }

    public static implicit operator Tokens(JsonTokenResponse jtr)
    {
        return new Tokens
        {
            AccessToken = jtr.access_token ?? string.Empty,
            TokenType = jtr.token_type ?? string.Empty,
            ExpiresInSeconds = (uint)jtr.expires_in,
            Resource = jtr.resource ?? string.Empty,
            RefreshToken = jtr.refresh_token ?? string.Empty,
            RefreshExpiresInSeconds = (uint)jtr.refresh_token_expires_in,
            IdToken = jtr.id_token ?? string.Empty
        };
    }
}

#pragma warning restore IDE1006 // Naming Styles