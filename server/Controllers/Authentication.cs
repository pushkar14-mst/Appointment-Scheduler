
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using server;

public class Authentication(IAuthentication context) : IJWTAuth
{
    private readonly IAuthentication _context = context;

    public string JWTTokenAuth(string Id, string Username)
    {
        var key = _context.Key;
        var issuer = _context.Issuer;
        var tokenGenerator = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, Id),
                new Claim(ClaimTypes.Role, Username)
            ]),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)), SecurityAlgorithms.HmacSha256Signature),
            Issuer = issuer,
            Audience = issuer
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenGenerator);
        return tokenHandler.WriteToken(token);
    }
}
