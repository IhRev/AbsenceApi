namespace Absence.Infrastructure.Identity;

public interface IRandomGenerator
{
    byte[] GenerateBytes(int size);
}