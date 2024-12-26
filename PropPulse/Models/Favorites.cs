using PropPulse.Models;

public class Favorite
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AdId { get; set; }

    public User User { get; set; }
   
}
