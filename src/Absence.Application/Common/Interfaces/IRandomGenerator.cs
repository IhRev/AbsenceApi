namespace Absence.Application.Common.Interfaces;

public interface IRandomGenerator
{
    byte[] GenerateBytes(int size);
}