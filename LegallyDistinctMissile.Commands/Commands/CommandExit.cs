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
    [Command("exit", Priority = Priority.Normal)]
    [CommandActor(typeof(UnturnedUser))]
    [CommandDescription("Exits the game without cooldown.")]
    public class CommandExit : Command
    {
        private readonly IStringLocalizer m_StringLocalizer;

        public CommandExit(IServiceProvider serviceProvider, IStringLocalizer stringLocalizer) : base(serviceProvider)
        {
            m_StringLocalizer = stringLocalizer;
        }

        protected override Task OnExecuteAsync()
        {
            var unturnedUser = (UnturnedUser) Context.Actor;
            Provider.kick(unturnedUser.SteamId, m_StringLocalizer["ldm_cmds:success:exit"]);
            return Task.CompletedTask;
        }
    }
}