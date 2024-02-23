namespace Talabat.Core.Entities.Order_Aggregate;

public class Address  //will not be mapped to a table
{
    public Address()
    {
        
    }
    public Address(string firstName, string lName, string street, string city, string country)
    {
        FirstName = firstName;
        LastName = lName;
        Street = street;
        City = city;
        Country = country;
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
}
