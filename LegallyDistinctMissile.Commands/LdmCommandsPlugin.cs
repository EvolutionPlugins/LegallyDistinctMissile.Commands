#region

using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using OpenMod.API.Users;
using OpenMod.Core.Plugins;
using OpenMod.Core.Users;
using OpenMod.Unturned.Plugins;
using OpenMod.Unturned.Users;
using Steamworks;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

[assembly: PluginMetadata("Ldm.Commands", Author = "Rube200", DisplayName = "LDM-Commands")]

namespace LegallyDistinctMissile.Commands
{
    public class LdmCommandsPlugin : OpenModUnturnedPlugin
    {
        internal readonly Dictionary<CSteamID, LdmComponent> LdmComponents;
        private readonly UnturnedUserProvider m_UnturnedUserProvider;


        public LdmCommandsPlugin(IServiceProvider serviceProvider, IUserManager userManager) : base(
            serviceProvider)
        {
            m_UnturnedUserProvider =
                userManager.UserProviders.First(prv => prv.GetType() == typeof(UnturnedUserProvider)) as
                    UnturnedUserProvider;
            LdmComponents = new Dictionary<CSteamID, LdmComponent>();
        }


        protected override async UniTask OnLoadAsync()
        {
            foreach (var player in await m_UnturnedUserProvider.GetUsersAsync(KnownActorTypes.Player))
            {
                if (!(player is UnturnedUser unturnedUser) || unturnedUser.Player == null)
                    continue;

                var go = unturnedUser.Player.gameObject;
                var ldmComponent = go.getOrAddComponent<LdmComponent>();
                LdmComponents[unturnedUser.SteamId] = ldmComponent;
            }
        }

        protected override async UniTask OnUnloadAsync()
        {
            foreach (var player in await m_UnturnedUserProvider.GetUsersAsync(KnownActorTypes.Player))
            {
                if (!(player is UnturnedUser unturnedUser) || unturnedUser.Player == null)
                    continue;

                var go = unturnedUser.Player.gameObject;
                var ldmComponent = go.GetComponent<LdmComponent>();
                if (ldmComponent != null)
                    Object.Destroy(ldmComponent);
            }

            LdmComponents.Clear();
        }

        public LdmComponent GetLdmComponent(CSteamID steamId)
        {
            return LdmComponents.TryGetValue(steamId, out var component) ? component : null;
        }
    }
}