using System.Threading.Tasks;
using OpenMod.API.Eventing;
using OpenMod.Core.Users.Events;
using OpenMod.Unturned.Users;
using UnityEngine;

namespace LegallyDistinctMissile.Commands.Events
{
    public class EventListener : IEventListener<UserConnectedEvent>, IEventListener<UserDisconnectedEvent>
    {
        private readonly LdmCommandsPlugin m_Plugin;

        public EventListener(LdmCommandsPlugin plugin)
        {
            m_Plugin = plugin;
        }

        public Task HandleEventAsync(object emitter, UserConnectedEvent userConnectedEvent)
        {
            System.Console.WriteLine("connected");
            if (!(userConnectedEvent.User is UnturnedUser unturnedUser))
                return Task.CompletedTask;

            var go = unturnedUser.Player.gameObject;
            var ldmComponent = go.getOrAddComponent<LdmComponent>();
            m_Plugin.LdmComponents[unturnedUser.SteamId] = ldmComponent;
            return Task.CompletedTask;
        }

        public Task HandleEventAsync(object emitter, UserDisconnectedEvent userDisconnectedEvent)
        {
            System.Console.WriteLine("discconnected");
            if (!(userDisconnectedEvent.User is UnturnedUser unturnedUser))
                return Task.CompletedTask;

            m_Plugin.LdmComponents.Remove(unturnedUser.SteamId);
            return Task.CompletedTask;
        }
    }
}
