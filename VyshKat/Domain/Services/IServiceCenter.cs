namespace VyshKat.Domain.Services;
using VyshKat.Domain.Abstractions;

public interface IServiceCenter
{
    bool Approve(IInventoryItem item);
}