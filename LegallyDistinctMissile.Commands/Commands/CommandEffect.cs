#region

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using OpenMod.Core.Users;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using Command = OpenMod.Core.Commands.Command;

#endregion

namespace RG.LegallyDistinctMissile.Commands.Commands
{
    [Command("effect", Priority = Priority.Normal)]
    [CommandDescription("Triggers an effect.")]
    [CommandSyntax("<id> [x y z]")]
    public class CommandEffect : Command
    {
        private readonly IStringLocalizer m_StringLocalizer;

        public CommandEffect(IServiceProvider serviceProvider, IStringLocalizer stringLocalizer) : base(serviceProvider)
        {
            m_StringLocalizer = stringLocalizer;
        }

        protected override async Task OnExecuteAsync()
        {
            if (Context.Parameters.Length < 1 ||
                Context.Actor.Type == KnownActorTypes.Console && Context.Parameters.Length < 4
            ) //force console to pass position 
                throw new CommandWrongUsageException(Context);

            var effectId = await Context.Parameters.GetAsync<ushort>(0);
            if (Assets.find(EAssetType.EFFECT, effectId) == null)
                throw new UserFriendlyException(m_StringLocalizer["ldm_cmds:fail:effect_not_found",
                    effectId]);

            var unturnedUser = Context.Actor as UnturnedUser;
            var actorId = unturnedUser?.SteamId ?? CSteamID.Nil;
            Vector3 position;
            if (Context.Parameters.Length > 3)
            {
                var x = await Context.Parameters.GetAsync<float>(1);
                var y = await Context.Parameters.GetAsync<float>(2);
                var z = await Context.Parameters.GetAsync<float>(3);
                position = new Vector3(x, y, z);
            }
            else
            {
                // ReSharper disable once PossibleNullReferenceException
                position = unturnedUser.Player.transform.position;
            }

            EffectManager.sendEffect(effectId, actorId, position);
            await PrintAsync(m_StringLocalizer["ldm_cmds:success:effect_triggered", effectId]);
        }
    }
}