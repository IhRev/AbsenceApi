namespace Absence.Application.Common.Interfaces;

public interface IRequirePermission
{
    string Permission { get; }
    int OrganizationId { get; }
}