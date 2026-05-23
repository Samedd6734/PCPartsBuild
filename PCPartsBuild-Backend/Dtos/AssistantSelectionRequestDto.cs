using System;

namespace PCPartsAPI.Dtos
{
    public record AssistantSelectionRequestDto(Guid SessionId, int ComponentId);
}
