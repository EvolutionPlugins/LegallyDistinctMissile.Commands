#region

using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.API.Prioritization;
using OpenMod.API.Users;
using OpenMod.Core.Commands;
using OpenMod.Core.Users;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using Command = OpenMod.Core.Commands.Command;

#endregion

namespace LegallyDistinctMissile.Commands.Commands
{
    [Command("broadcast", Priority = Priority.Normal)]
    [CommandDescription("Broadcasts a message")]
    [CommandSyntax("[color] <message>")]
    public class CommandBroadcast : Command
    {
        private readonly UnturnedUserProvider m_UnturnedUserProvider;

        public CommandBroadcast(IServiceProvider serviceProvider,
            IUserManager userManager) : base(serviceProvider)
        {
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
            if (color.IsKnownColor)
            {
                if (Context.Parameters.Length < 2)
                    throw new CommandWrongUsageException(Context);

                index = 1;
            }
            else
            {
                color = Color.White;
            }

            var message = Context.Parameters.GetArgumentLine(index).Trim();
            if (string.IsNullOrWhiteSpace(message))
                throw new CommandWrongUsageException(Context);

            CommandWindow.Log(message);
            foreach (var user in await m_UnturnedUserProvider.GetUsersAsync(KnownActorTypes.Player))
                await user.PrintMessageAsync(message, color);
        }
    }
}