using RapidOrder.Core.DTOs;

namespace RapidOrder.Api.Hubs
{
    public interface IMissionClient
    {
        Task MissionCreated(MissionCreatedDto mission);
        Task MissionUpdated(MissionCreatedDto mission); // reuse payload for simplicity
    }
}
