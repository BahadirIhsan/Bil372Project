namespace Bil372Project.EntityLayer.Entities;
public class AppUser
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // şimdilik düz string ileride hashe çeviririz
    public string Password { get; set; } = string.Empty;
    
    public string? PhoneNumber { get; set; }   // yeni
    public DateTime? BirthDate { get; set; }   // yeni
    public string? City { get; set; }          // yeni
    public string? Country { get; set; }       // yeni
    public string? Bio { get; set; }           // yeni

    // Navigation: 1 user -> n ölçüm
    public ICollection<UserMeasure> UserMeasures { get; set; } = new List<UserMeasure>();
}
