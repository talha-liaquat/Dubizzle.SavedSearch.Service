using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Dubizzle.SavedSearch.Api
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly string _username;
        private readonly string _password;

        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IConfiguration configuration)
            : base(options, logger, encoder, clock)
        {
            if(configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var securityConfigKey = configuration.GetSection("BasicAuth");

            _username = securityConfigKey["Username"];
            _password = securityConfigKey["Password"];
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");


            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
            var username = credentials[0];
            var password = credentials[1];

            if (username != _username || password != _password)
                return AuthenticateResult.Fail("Invalid username or password");

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, string.Empty),
                new Claim(ClaimTypes.Name, string.Empty),
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
