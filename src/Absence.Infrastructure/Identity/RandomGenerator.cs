using System.Security.Cryptography;

namespace Absence.Infrastructure.Identity;

internal class RandomGenerator : IRandomGenerator
{
    public byte[] GenerateBytes(int size) => 
        RandomNumberGenerator.GetBytes(size);
}