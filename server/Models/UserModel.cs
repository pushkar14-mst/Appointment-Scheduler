namespace server;

public class UserModel
{
    required
    public string Id
    { get; set; }
    required
    public string Username
    { get; set; }
    required
    public string Password
    { get; set; }
    public List<AppointmentModel>? Appointments
    { get; set; }
}

public class AppointmentModel
{

    public AppointmentModel()
    {
        Id = Guid.NewGuid().ToString();
    }
    required
    public string Id
    { get; set; }
    required
    public string Title
    { get; set; }
    required
    public DateOnly Date
    { get; set; }
    required
    public string Time
    { get; set; }

    public string? Description
    { get; set; }
}