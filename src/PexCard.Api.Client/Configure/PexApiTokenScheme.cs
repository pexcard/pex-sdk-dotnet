namespace PexCard.Api.Client
{
    /// <summary>
    /// The HTTP <c>Authorization</c> scheme the <see cref="PexApiClient"/> uses when calling the
    /// PEX API with a caller-supplied token.
    /// </summary>
    public enum PexApiTokenScheme
    {
        /// <summary>
        /// The legacy PEX opaque-token scheme (<c>Authorization: token &lt;externalToken&gt;</c>).
        /// This is the default and preserves existing behaviour.
        /// </summary>
        Token = 0,

        /// <summary>
        /// The OAuth 2.1 bearer scheme (<c>Authorization: Bearer &lt;access_token&gt;</c>). Use this
        /// when the token is an OAuth 2.1 access token issued by the PEX OAuth Server; the PEX API
        /// validates it as a bearer (resource-server) token.
        /// </summary>
        Bearer = 1,
    }
}
