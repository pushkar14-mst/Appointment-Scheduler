using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Cms;
namespace server;
public class CreateAppointmentClass
{
    public string? Id { get; set; }
    public AppointmentModel? Appointment { get; set; }
}
public class Appointments(ApplicationDBContext context) : Controller

{
    private readonly ApplicationDBContext _context = context;

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
