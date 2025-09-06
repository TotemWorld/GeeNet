using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

namespace GeeNet
{
    internal class TokenProvider
    {
        private readonly GeeNetOptions _options;
        private readonly GoogleCredential _googleCredential;
        public TokenProvider(IOptions<GeeNetOptions> options)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options), "GeeNetOptions cannot be null.");
            try
            {
                _googleCredential = GoogleCredential.FromFile(_options.GoogleServiceAccount).CreateScoped(_options.Scope);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create GoogleCredential from the specified service account file.", ex);

            }
        }
        public async Task<string> GetTokenAsync(string? userId = null)
        {
            var token = await _googleCredential.UnderlyingCredential.GetAccessTokenForRequestAsync(_options.Scope);
            return token;
        }
    }
}
