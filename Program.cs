using System.Collections.Specialized;
using System.Web;
using JetBrains.Annotations;

namespace ADALReplacementTest;

[PublicAPI]
public class Program
{

    private const string AUTHORITY = "https://[server]/adfs/oauth2/";           // complete this
    private const string AUTHORIZE_ENDPOINT = "authorize";
    private const string TOKEN_ENDPOINT = "token";
    private const string CLIENT_ID = "[client_id]";                             // complete this
    private const string RESOURCE = "[resource]";                               // complete this
    private const string REDIRECT_URI = "[redirect_uri";                        // complete this
    private const string RESPONSE_TYPE = "code";
    private const string AUTHORIZATION_CODE_GRANT_TYPE = "authorization_code";
    private const string REFRESH_TOKEN_GRANT_TYPE = "refresh_token";
    private const string AUTH_METHOD = "FormsAuthentication";
    private const string PASSWORD_UPDATE_ENDPOINT = "[ADFS password update endpoint]";  // complete this

    private static string _cachedUserName = string.Empty;
    private static string _cachedPassword = string.Empty;

    static async Task<AuthorizationCodeResult> GetAuthorizationCodeAsync(string username, string password)
    {
        _cachedUserName = username;
        _cachedPassword = password;

        UriBuilder builder = new($"{AUTHORITY}{AUTHORIZE_ENDPOINT}");
        NameValueCollection query = HttpUtility.ParseQueryString(builder.Query);
        query["resource"] = RESOURCE;
        query["client_id"] = CLIENT_ID;
        query["response_type"] = RESPONSE_TYPE;
        query["redirect_uri"] = REDIRECT_URI;
        builder.Query = query.ToString();

        FormUrlEncodedContent formData = new(new[]
        {
            new KeyValuePair<string, string>("UserName", username),
            new KeyValuePair<string, string>("Password", password ),
            new KeyValuePair<string, string>("AuthMethod", AUTH_METHOD),
        });

        HttpClient client = new();
        HttpResponseMessage response = await client.PostAsync(builder.ToString(), formData);

        string responseString = await response.Content.ReadAsStringAsync();
        if (responseString.Contains("Update Password")) { return new AuthorizationCodeResult { Result = AuthorizationCodeStatuses.PasswordChangeRequired }; }

        int offset = REDIRECT_URI.Length + 6;       // may need to adjust this.

        string authCode = response.Headers.Location?.ToString()[offset..] ?? string.Empty;

        return new AuthorizationCodeResult {
            AuthorizationCode = authCode, 
            Result = string.IsNullOrEmpty(authCode) 
            ? AuthorizationCodeStatuses.Failed 
            : AuthorizationCodeStatuses.Success 
        };
    }

    static async Task<Tokens> GetTokensFromAuthorizationCodeAsync(AuthorizationCodeResult authCode)
    {
        string finalUrl = $"{AUTHORITY}{TOKEN_ENDPOINT}";
        FormUrlEncodedContent formData = new(new[]
        {
            new KeyValuePair<string, string>("client_id", CLIENT_ID),
            new KeyValuePair<string, string>("code", authCode.AuthorizationCode ),
            new KeyValuePair<string, string>("redirect_uri", REDIRECT_URI),
            new KeyValuePair<string, string>("grant_type", AUTHORIZATION_CODE_GRANT_TYPE)
        });

        HttpClient client = new();
        HttpResponseMessage response = await client.PostAsync(finalUrl, formData);

        string httpResponse = await response.Content.ReadAsStringAsync();

        JsonTokenResponse jtr = System.Text.Json.JsonSerializer.Deserialize<JsonTokenResponse>(httpResponse) ?? throw new InvalidOperationException();

        return jtr;
    }

    [PublicAPI]
    private static async Task<Tokens> RefreshToken(Tokens currentToken, bool forceTheWholeOrdeal = false)
    {

        if (forceTheWholeOrdeal || currentToken.RefreshExpiresInSeconds <= 60)
        {
            // refresh via the whole ordeal.
            return await RefreshViaTheWholeOrdeal();
        }
        else
        {
            // refresh via the refresh token.
            return await RefreshViaRefreshToken(currentToken);
        }
    }

    private static async Task<Tokens> RefreshViaTheWholeOrdeal()
    {
        AuthorizationCodeResult authCode = await GetAuthorizationCodeAsync(_cachedUserName, _cachedPassword); 
        return await GetTokensFromAuthorizationCodeAsync(authCode);
    }

    private static async Task<Tokens> RefreshViaRefreshToken(Tokens currentToken)
    {
        string finalUrl = $"{AUTHORITY}{TOKEN_ENDPOINT}";
        FormUrlEncodedContent formData = new(new[]
        {
            new KeyValuePair<string, string>("grant_type", REFRESH_TOKEN_GRANT_TYPE),
            new KeyValuePair<string, string>("client_id", CLIENT_ID),
            new KeyValuePair<string, string>("refresh_token", currentToken.RefreshToken )
        });

        HttpClient client = new();
        HttpResponseMessage response = await client.PostAsync(finalUrl, formData);

        string httpResponse = await response.Content.ReadAsStringAsync();

        JsonRefreshTokenResponse refreshTokenResponse = System.Text.Json.JsonSerializer.Deserialize<JsonRefreshTokenResponse>(httpResponse) ?? throw new InvalidOperationException();

        Tokens tr = refreshTokenResponse.ToTokens(currentToken);

        return tr;
    }

    private static async Task<PasswordChangeResult> UpdatePasswordAsync(string username, string oldPassword, string newPassword)
    {
        FormUrlEncodedContent formData = new(new[]
        {
            new KeyValuePair<string, string>("UserName", username), 
            new KeyValuePair<string, string>("OldPassword", oldPassword),
            new KeyValuePair<string, string>("NewPassword", newPassword),
            new KeyValuePair<string, string>("ConfirmNewPassword", newPassword),
            new KeyValuePair<string, string>("Submit", "Submit")
        });

        HttpClient client = new(new HttpClientHandler { AllowAutoRedirect = false });
        HttpResponseMessage response = await client.PostAsync(PASSWORD_UPDATE_ENDPOINT, formData);

        if (response.Headers.Location == null) { return PasswordChangeResult.UnknownError; }
        if (response.Headers.Location.ToString().Contains("status=1")) { return PasswordChangeResult.CollectingInput; }
        if (response.Headers.Location.ToString().Contains("status=2")) { return PasswordChangeResult.InvalidCredentials; }
        if (response.Headers.Location.ToString().Contains("status=3")) { return PasswordChangeResult.PasswordComplexityFailed; }
        if (response.Headers.Location.ToString().Contains("status=4")) { return PasswordChangeResult.UnknownError; }

        // if we're here, we had success.
        _cachedPassword = newPassword;
        _cachedUserName = username;

        return PasswordChangeResult.Success;
    }

    static async Task Main()
    {
        await Task.Delay(10);
    }

}