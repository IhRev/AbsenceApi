using Absence.Application.UseCases.Events.DTOs;
using Absence.Domain.Entities;
using AutoMapper;

namespace Absence.Application.UseCases.Events.Mappings;

internal class EventMapping : Profile
{
    public EventMapping()
    {
        CreateMap<EventEntity, EventDTO>();
        CreateMap<EditEventDTO, EventEntity>();
        CreateMap<CreateEventDTO, EventEntity>();
    }
}