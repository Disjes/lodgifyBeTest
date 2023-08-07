namespace Services.Models;

public class Contact
{
    public long Id { get; private set; }

    public string Email { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public bool IsAnonymized { get; set; }


    public Contact(int id)
    {
        this.Id = id;
        this.IsAnonymized = false;
    }
}