namespace server;

public interface IJWTAuth
{
    public string JWTTokenAuth(string Id, string Username);
}