#region

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using Command = OpenMod.Core.Commands.Command;

#endregion

namespace RG.LegallyDistinctMissile.Commands.Commands
{
    [Command("heal", Priority = Priority.Normal)]
    [CommandActor(typeof(UnturnedUser))]
    [CommandDescription("Heals the target user")]
    [CommandSyntax("[amount]")]
    public class CommandMore : Command
    {
        private readonly IStringLocalizer m_StringLocalizer;

        public CommandMore(IServiceProvider serviceProvider, IStringLocalizer stringLocalizer) : base(serviceProvider)
        {
            m_StringLocalizer = stringLocalizer;
        }

        protected override async Task OnExecuteAsync()
        {
            var amount = (byte) 1;
            if (Context.Parameters.Length > 0)
                amount = await Context.Parameters.GetAsync<byte>(0);

            var unturnedUser = Context.Actor as UnturnedUser;
            // ReSharper disable once PossibleNullReferenceException
            var equipmentI = unturnedUser.Player.equipment;
            if (equipmentI.itemID == 0)
                throw new UserFriendlyException(m_StringLocalizer["ldm_cmds:fail:not_equipped"]);

            ItemTool.tryForceGiveItem(unturnedUser.Player, equipmentI.itemID, amount);
            await PrintAsync(m_StringLocalizer["ldm_cmds:success:more", amount, equipmentI.asset.itemName]);
        }
    }
}