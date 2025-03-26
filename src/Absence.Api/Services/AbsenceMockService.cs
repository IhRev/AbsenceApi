using AbsenceApi.DTOs;

namespace AbsenceApi.Services;

public class AbsenceMockService
{
    private readonly List<AbsenceDTO> absences = [];
    private static int lastId = 0;

    public IEnumerable<AbsenceDTO> GetAll() => absences;
     
    public int Add(AbsenceDTO absence)
    {
        absence.Id = ++lastId;
        absences.Add(absence);
        return lastId; 
    }
}