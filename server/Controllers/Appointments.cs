using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
namespace server;
public class CreateAppointmentClass
{
    public string? Id { get; set; }
    public AppointmentModel? Appointment { get; set; }
}
public class UserLoginParams
{
    public string? Username { get; set; }
    public string? Password { get; set; }

}
public class UserResult
{
    public string? Id { get; set; }
    public string? Username { get; set; }
    public List<AppointmentModel>? Appointments { get; set; }
}
// define interface for JWTAuth method

public class Appointments : Controller

{
    private readonly ApplicationDBContext _context;
    private readonly IJWTAuth _jwtTokenMethod;

    public Appointments(ApplicationDBContext context, IJWTAuth jwtTokenMethod)
    {
        _context = context;
        _jwtTokenMethod = jwtTokenMethod;
    }
    [HttpGet]
    public string Home()
    {
        return "You are connected to the Appointments Controller";
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] UserModel user)
    {
        try
        {
            var userExists = _context.Users.Any(u => u.Username == user.Username);
            if (userExists)
            {
                return BadRequest("User already exists");
            }
            var encryptedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Password = encryptedPassword;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("User registered successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }
    [HttpPost]
    public IActionResult Login([FromBody] UserLoginParams user)
    {
        try
        {
            if (user == null)
            {
                return BadRequest("Invalid request body");
            }
            var userExists = _context.Users.FirstOrDefault(u => u.Username == user.Username);
            if (userExists == null)
            {
                return BadRequest("User not found");
            }
            var passwordMatch = BCrypt.Net.BCrypt.Verify(user.Password, userExists.Password);
            if (!passwordMatch)
            {
                return BadRequest("Invalid password");
            }
            // UserResult result = new UserResult
            // {
            //     Id = userExists.Id,
            //     Username = userExists.Username,
            //     Appointments = userExists.Appointments
            // };
            // return Ok(result);
            return new JsonResult(new { token = _jwtTokenMethod.JWTTokenAuth(userExists.Id, userExists.Username) });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CreateAppointment()
    {
        try
        {
            using StreamReader reader = new(Request.Body, Encoding.UTF8);
            string body = await reader.ReadToEndAsync();
            Console.WriteLine(body);
            CreateAppointmentClass? createAppointmentObj = JsonConvert.DeserializeObject<CreateAppointmentClass>(body);
            if (createAppointmentObj == null)
            {
                return BadRequest("Invalid request body");
            }

            var user = _context.Users.Find(createAppointmentObj.Id);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            AppointmentModel appointment = new AppointmentModel
            {
                Id = Guid.NewGuid().ToString(),
                Title = createAppointmentObj.Appointment!.Title,
                Date = createAppointmentObj.Appointment.Date,
                Time = createAppointmentObj.Appointment.Time,
                Description = createAppointmentObj.Appointment.Description
            };

            user.Appointments ??= new List<AppointmentModel>();
            user.Appointments.Add(appointment);

            await _context.SaveChangesAsync();
            Console.WriteLine("Appointment created successfully and saved to the database");
            return Ok("Appointment created successfully");
        }


        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult GetAllAppointments([FromQuery] string id)
    {
        try
        {
            Console.WriteLine(id);
            var appointments = _context.Users.Include(u => u.Appointments).FirstOrDefault(u => u.Id == id)?.Appointments;
            Console.WriteLine(appointments);
            if (appointments == null)
            {
                return Ok("No appointments found");
            }
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


}
