﻿using LeapSession.Helpers;
using LeapSession.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LeapSession.Bots
{
    public class DialogBot<T>:ActivityHandler where T:Dialog
    {
        #region variables
        protected readonly Dialog _dialog;
        protected readonly BotStateService _botStateService;
        protected readonly ILogger _logger;
        #endregion

        public DialogBot(BotStateService botStateService, T dialog, ILogger<DialogBot<T>> logger)
        {
            _botStateService = botStateService ?? throw new System.ArgumentNullException(nameof(botStateService));
            _dialog=dialog?? throw new System.ArgumentNullException(nameof(dialog));
            _logger=logger?? throw new System.ArgumentNullException(nameof(logger));

        }
        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);
            //save any state changes that might have occurred during the turn
            await _botStateService._userState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _botStateService._conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        } 
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext,CancellationToken cancellationToken)
        {
            _logger.LogInformation("Running dialog with Message.Activity");

            //Run the Dialog with the new Message Activity
            await _dialog.Run(turnContext, _botStateService.DialogStateAccessor, cancellationToken);
        }

    }

}
