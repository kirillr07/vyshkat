namespace VyshKat.Domain.Vehicles;
using VyshKat.Domain.Abstractions;

public class Scooter : Vehicle, INoviceFriendly
{
    public int NoviceRating {get;}

    public Scooter(string inventoryNumber, string name, int noviceRating)
        : base (inventoryNumber, name)
    {
        NoviceRating = noviceRating;
    }
}