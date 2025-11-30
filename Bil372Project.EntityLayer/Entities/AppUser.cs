namespace Bil372Project.EntityLayer.Entities;
public class AppUser
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // şimdilik düz string ileride hashe çeviririz
    public string Password { get; set; } = string.Empty;
    
    public string? PhoneNumber { get; set; }   
    public DateTime? BirthDate { get; set; }   
    public string? City { get; set; }          
    public string? Country { get; set; }       
    public string? Bio { get; set; }           
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


    // Navigation: 1 user -> n ölçüm
    public ICollection<UserMeasure> UserMeasures { get; set; } = new List<UserMeasure>();
}
