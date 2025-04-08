namespace Absence.Domain.Interfaces;

public interface IRandomGenerator
{
    byte[] GenerateBytes(int size);
}