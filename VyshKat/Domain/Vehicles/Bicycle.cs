namespace VyshKat.Domain.Vehicles;
using VyshKat.Domain.Abstractions;

public class Bicycle : Vehicle, INoviceFriendly
{
    public int NoviceRating {get;}

    public Bicycle(string inventoryNumber, string name, int noviceRating)
        : base (inventoryNumber, name)
    {
        NoviceRating = noviceRating;
    }
}