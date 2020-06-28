#region

using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API.Prioritization;
using OpenMod.API.Users;
using OpenMod.Core.Commands;
using OpenMod.Core.Users;
using OpenMod.Unturned.Users;

#endregion

namespace RG.LegallyDistinctMissile.Commands.Commands
{
    [Command("broadcast", Priority = Priority.Normal)]
    [CommandDescription("Broadcasts a message")]
    [CommandSyntax("[color] <message>")]
    public class CommandBroadcast : Command
    {
        private readonly ILogger m_Logger;
        private readonly UnturnedUserProvider m_UnturnedUserProvider;

        public CommandBroadcast(ILogger logger, IServiceProvider serviceProvider,
            IUserManager userManager) : base(serviceProvider)
        {
            m_Logger = logger;
            m_UnturnedUserProvider =
                userManager.UserProviders.First(prv => prv.GetType() == typeof(UnturnedUserProvider)) as
                    UnturnedUserProvider;
        }

        protected override async Task OnExecuteAsync()
        {
            if (Context.Parameters.Length < 1)
                throw new CommandWrongUsageException(Context);

            var colorString = await Context.Parameters.GetAsync<string>(0);
            var color = Color.FromName(colorString);

            var index = 0;
            if (color.IsKnownColor) index = 1;

            var message = Context.Parameters.GetArgumentLine(index, Context.Parameters.Length - 1).Trim();
            if (string.IsNullOrWhiteSpace(message))
                throw new CommandWrongUsageException(Context);

            m_Logger.LogInformation(message);
            foreach (var user in await m_UnturnedUserProvider.GetUsersAsync(KnownActorTypes.Player))
                await user.PrintMessageAsync(message, color);
        }
    }
}