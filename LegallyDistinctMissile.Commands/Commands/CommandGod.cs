#region

using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using LegallyDistinctMissile.Commands.Events;
using Microsoft.Extensions.Localization;
using OpenMod.API.Eventing;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;
using OpenMod.API.Users;
using OpenMod.Core.Commands;
using OpenMod.Core.Users;
using OpenMod.Unturned.Users;

#endregion

namespace LegallyDistinctMissile.Commands.Commands
{
    [Command("god", Priority = Priority.Normal)]
    [CommandDescription("Feel the power")]
    [CommandSyntax("[player]")]
    public class CommandGod : Command
    {
        private readonly IEventBus m_EventBus;
        private readonly IPermissionChecker m_PermissionChecker;
        private readonly LdmCommandsPlugin m_Plugin;
        private readonly IStringLocalizer m_StringLocalizer;
        private readonly UnturnedUserProvider m_UnturnedUserProvider;

        public CommandGod(IEventBus eventBus, LdmCommandsPlugin plugin, IPermissionChecker permissionChecker,
            IServiceProvider serviceProvider, IStringLocalizer stringLocalizer,
            IUserManager userManager) : base(serviceProvider)
        {
            m_EventBus = eventBus;
            m_Plugin = plugin;
            m_PermissionChecker = permissionChecker;
            m_StringLocalizer = stringLocalizer;
            m_UnturnedUserProvider =
                userManager.UserProviders.First(prv => prv.GetType() == typeof(UnturnedUserProvider)) as
                    UnturnedUserProvider;
        }

        protected override async Task OnExecuteAsync()
        {
            if (Context.Actor.Type == KnownActorTypes.Console && Context.Parameters.Length == 0)
                throw new CommandWrongUsageException(Context);

            var other = false;
            var targetData = (UnturnedUser) null;

            if (Context.Parameters.Length > 0)
            {
                var otherPermission = await m_PermissionChecker.CheckPermissionAsync(Context.Actor, "god.other") ==
                                      PermissionGrantResult.Grant;
                var target = await Context.Parameters.GetAsync<string>(0);
                targetData =
                    await m_UnturnedUserProvider.FindUserAsync(KnownActorTypes.Player, target, UserSearchMode.NameOrId)
                        as UnturnedUser;

                if (otherPermission)
                {
                    if (targetData == null)
                        throw new UserFriendlyException(
                            m_StringLocalizer["ldm_cmds:fail:player_not_found", target]);

                    if (!Context.Actor.Id.Equals(targetData.Id, StringComparison.OrdinalIgnoreCase))
                        other = true;
                }
            }

            if (!other)
                targetData = Context.Actor as UnturnedUser;

            // ReSharper disable once PossibleNullReferenceException
            var ldmComponent = m_Plugin.GetLdmComponent(targetData.SteamId);
            if (ldmComponent == null)
                throw new UserFriendlyException(m_StringLocalizer["ldm_cmds:fail:invalid_user", targetData.DisplayName]);

            string message;
            if (ldmComponent.IsGodModeActive)
            {
                await UniTask.SwitchToMainThread();
                ldmComponent.ActivateGodMode();
                message = other
                    ? m_StringLocalizer["ldm_cmds:success:god_other_enabled", targetData.DisplayName]
                    : m_StringLocalizer["ldm_cmds:success:god_enabled"];
            }
            else
            {
                ldmComponent.DesactivateGodMode();
                message = other
                    ? m_StringLocalizer["ldm_cmds:success:god_other_disabled", targetData.DisplayName]
                    : m_StringLocalizer["ldm_cmds:success:god_disabled"];
            }

            await PrintAsync(message);
            if (other)
                await targetData.PrintMessageAsync(m_StringLocalizer[
                    ldmComponent.IsGodModeActive
                        ? "ldm_cmds:success:god_enabled_by"
                        : "ldm_cmds:success:god_disabled_by"
                    , Context.Actor.DisplayName]);

            var getBalanceEvent = new ChangeGodModeEvent(targetData, ldmComponent.IsVanishModeActive);
            await m_EventBus.EmitAsync(m_Plugin, this, getBalanceEvent);
        }
    }
}