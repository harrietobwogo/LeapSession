using LeapSession.Model;
using LeapSession.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LeapSession.Dialogs
{
    public class GreetingDialog :ComponentDialog
    {
        #region Variables
        private readonly BotStateService _botStateService;
        #endregion

        public GreetingDialog(string dialogId,BotStateService botStateService): base(dialogId)
        {
            _botStateService = botStateService ?? throw new System.ArgumentNullException(nameof(botStateService));
            InitializeWaterFallDialog();
        }

        private void InitializeWaterFallDialog()
        {
            //create waterfall steps
            var waterfallSteps = new WaterfallStep[]
            {
               InitialStepAsync,
               FinalStepAsync
            };
            //Add named Dialogs
            AddDialog(new WaterfallDialog($"{nameof(GreetingDialog)}.mainFlow", waterfallSteps));
            AddDialog(new TextPrompt($"{nameof(GreetingDialog)}.name"));

            //Set the starting Dialog
            InitialDialogId = $"{nameof(GreetingDialog)}.mainFlow";
        }
        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            if (string.IsNullOrEmpty(userProfile.name))
            {
                return await stepContext.PromptAsync($"{nameof(GreetingDialog)}.name",
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("what is Your name?")
                    }, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(null, cancellationToken);
            }
        }
        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            if (string.IsNullOrEmpty(userProfile.name))
            {
                //set the name
               // userProfile= (string)stepContext.Result;

                //save any state changes tat might have occured during the turn
                await _botStateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);
            }
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format($"Hi {0}. How can I help you today?", userProfile.name)),cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
