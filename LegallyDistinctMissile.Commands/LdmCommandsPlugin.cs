#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.API.Users;
using OpenMod.Core.Eventing;
using OpenMod.Core.Plugins;
using OpenMod.Core.Users;
using OpenMod.Core.Users.Events;
using OpenMod.Unturned.Users;
using Steamworks;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

[assembly: PluginMetadata("Ldm.Commands", Author = "Rube200", DisplayName = "LDM-Commands")]

namespace RG.LegallyDistinctMissile.Commands
{
    public class LdmCommandsPlugin : OpenModPluginBase
    {
        private readonly Dictionary<CSteamID, LdmComponent> m_LdmComponents;
        private readonly UnturnedUserProvider m_UnturnedUserProvider;


        public LdmCommandsPlugin(IServiceProvider serviceProvider, IUserManager userManager) : base(
            serviceProvider)
        {
            m_UnturnedUserProvider =
                userManager.UserProviders.First(prv => prv.GetType() == typeof(UnturnedUserProvider)) as
                    UnturnedUserProvider;
            m_LdmComponents = new Dictionary<CSteamID, LdmComponent>();
        }


        public override async Task LoadAsync()
        {
            await base.LoadAsync();
            foreach (var player in await m_UnturnedUserProvider.GetUsersAsync(KnownActorTypes.Player))
            {
                if (!(player is UnturnedUser unturnedUser))
                    continue;

                var go = unturnedUser.Player.gameObject;
                var ldmComponent = go.getOrAddComponent<LdmComponent>();
                m_LdmComponents[unturnedUser.SteamId] = ldmComponent;
            }
        }

        public override async Task UnloadAsync()
        {
            await base.UnloadAsync();
            foreach (var player in await m_UnturnedUserProvider.GetUsersAsync(KnownActorTypes.Player))
            {
                if (!(player is UnturnedUser unturnedUser))
                    continue;

                var go = unturnedUser.Player.gameObject;
                var ldmComponent = go.GetComponent<LdmComponent>();
                if (ldmComponent != null)
                    Object.Destroy(ldmComponent);
            }

            m_LdmComponents.Clear();
        }


        [EventListener]
        public Task OnUserConnectedAsync(object _, UserConnectedEvent userConnectedEvent)
        {
            if (!(userConnectedEvent.User is UnturnedUser unturnedUser))
                return Task.CompletedTask;

            var go = unturnedUser.Player.gameObject;
            var ldmComponent = go.getOrAddComponent<LdmComponent>();
            m_LdmComponents[unturnedUser.SteamId] = ldmComponent;
            return Task.CompletedTask;
        }

        [EventListener]
        public Task OnUserDisconnectedAsync(object _, UserDisconnectedEvent userDisconnectedEvent)
        {
            if (!(userDisconnectedEvent.User is UnturnedUser unturnedUser))
                return Task.CompletedTask;

            m_LdmComponents.Remove(unturnedUser.SteamId);
            return Task.CompletedTask;
        }


        public LdmComponent GetLdmComponent(CSteamID steamId)
        {
            return m_LdmComponents.TryGetValue(steamId, out var component) ? component : null;
        }
    }
}