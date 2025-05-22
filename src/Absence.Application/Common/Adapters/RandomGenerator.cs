using Absence.Domain.Interfaces;
using System.Security.Cryptography;

namespace Absence.Application.Common.Adapters;

internal class RandomGenerator : IRandomGenerator
{
    public byte[] GenerateBytes(int size) => 
        RandomNumberGenerator.GetBytes(size);
}