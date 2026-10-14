namespace VyshKat.Domain.Abstractions;

public interface INoviceFriendly
{
    int NoviceRating {get;}
    bool IsSuitableForNovice => NoviceRating >= 6;
}