namespace CropDeal.DTO;

public class UpdateUserDto
{

    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Location { get; set; }
    public Guid? BankAccountId { get; set; }
}

