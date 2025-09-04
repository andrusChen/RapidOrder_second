using Microsoft.AspNetCore.SignalR;
using RapidOrder.Api.Hubs;
using RapidOrder.Core.DTOs;

namespace RapidOrder.Api.Services
{
    public class MissionNotifier
    {
        private readonly IHubContext<MissionHub, IMissionClient> _hub;
        public MissionNotifier(IHubContext<MissionHub, IMissionClient> hub) { _hub = hub; }

        public Task PushCreatedAsync(MissionCreatedDto dto) => _hub.Clients.All.MissionCreated(dto);
        public Task PushUpdatedAsync(MissionCreatedDto dto) => _hub.Clients.All.MissionUpdated(dto);
    }
}
