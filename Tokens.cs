using JetBrains.Annotations;
#if JWT
using System.IdentityModel.Tokens.Jwt;
#endif

namespace ADALReplacementTest;

public class Tokens
{

    private readonly DateTime _tokenTimeStamp = DateTime.Now;
    private readonly uint _expiresInFromJsonToken;
    private readonly uint _refreshExpiresInFromJsonToken;

    [PublicAPI]
    public required string AccessToken { get; init; }

    [PublicAPI]
    public required string TokenType { get; init; }

    [PublicAPI]
    public uint ExpiresInSeconds
    {
        get
        {
            DateTime tokenExpiresAt = _tokenTimeStamp.AddSeconds(_expiresInFromJsonToken);
            double expiresInSeconds = tokenExpiresAt.Subtract(DateTime.Now).TotalSeconds;
            return expiresInSeconds < 0
                ? 0
                : (uint)expiresInSeconds;
        }
        init => _expiresInFromJsonToken = value;
    }

    [PublicAPI]
    public DateTime ExpiresAt => _tokenTimeStamp.AddSeconds(_expiresInFromJsonToken);

    [PublicAPI]
    public required string Resource { get; init; }

    [PublicAPI]
    public required string RefreshToken { get; init; }

    [PublicAPI]
    public uint RefreshExpiresInSeconds
    {
        get
        {
            DateTime refreshTokenExpiresAt = _tokenTimeStamp.AddSeconds(_refreshExpiresInFromJsonToken);
            double refreshExpiresInSeconds = refreshTokenExpiresAt.Subtract(DateTime.Now).TotalSeconds;
            return refreshExpiresInSeconds < 0
                ? 0
                : (uint)refreshExpiresInSeconds;
        }
        init => _refreshExpiresInFromJsonToken = value;
    }

    [PublicAPI]
    public DateTime RefreshExpiresAt => _tokenTimeStamp.AddSeconds(_refreshExpiresInFromJsonToken);

    [PublicAPI]
    public required string IdToken { get; init; }


#if JWT
    [PublicAPI]
    public JwtSecurityToken Identity
    {
        get
        {
            JwtSecurityToken token = new (jwtEncodedString: IdToken);

            return token;
        }
    }
#endif

    [PublicAPI]
    public string BearerToken => $"{TokenType} {AccessToken}";
}