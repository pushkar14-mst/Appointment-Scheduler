using Microsoft.EntityFrameworkCore;
namespace server;

public class ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : DbContext(options)
{
    public DbSet<UserModel> Users
    { get; set; }
    public DbSet<AppointmentModel> Appointments
    { get; set; }
}
