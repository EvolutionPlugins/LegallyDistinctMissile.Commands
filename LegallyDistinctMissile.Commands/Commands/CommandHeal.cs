#region

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;
using OpenMod.API.Users;
using OpenMod.Core.Commands;
using OpenMod.Core.Users;
using OpenMod.Unturned.Users;

#endregion

namespace RG.LegallyDistinctMissile.Commands.Commands
{
    [Command("heal", Priority = Priority.Normal)]
    [CommandDescription("Heals the target user")]
    [CommandSyntax("[player]")]
    public class CommandHeal : Command
    {
        private readonly IPermissionChecker m_PermissionChecker;
        private readonly IStringLocalizer m_StringLocalizer;
        private readonly UnturnedUserProvider m_UnturnedUserProvider;

        public CommandHeal(IPermissionChecker permissionChecker, IServiceProvider serviceProvider,
            IStringLocalizer stringLocalizer, IUserManager userManager) : base(serviceProvider)
        {
            m_PermissionChecker = permissionChecker;
            m_StringLocalizer = stringLocalizer;
            m_UnturnedUserProvider = m_UnturnedUserProvider =
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
                var otherPermission = await m_PermissionChecker.CheckPermissionAsync(Context.Actor, "heal.other") ==
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

            // ReSharper disable PossibleNullReferenceException
            targetData.Player.life.askHeal(100, true, true);
            targetData.Player.life.askDisinfect(100);
            targetData.Player.life.askDrink(100);
            targetData.Player.life.askEat(100);
            targetData.Player.life.askRest(100);
            // ReSharper restore PossibleNullReferenceException

            var message = other
                ? m_StringLocalizer["ldm_cmds:success:heal_other", targetData.DisplayName]
                : m_StringLocalizer["ldm_cmds:success:heal"];
            await PrintAsync(message);
            if (other)
                await targetData.PrintMessageAsync(m_StringLocalizer["ldm_cmds:success:healed_by",
                    Context.Actor.DisplayName]);
        }
    }
}