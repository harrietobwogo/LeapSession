using LeapSession.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Connector.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LeapSession.Debugs
{
    public class BugReportDialog : ComponentDialog
    {
        private readonly BotStateService _botStsteService;

        public BugReportDialog(string dialogId, BotStateService botStateService): base(dialogId)
        {
            _botStsteService=botStateService??throw new System.ArgumentNullException(nameof(botStateService));
            InitializeWaterFallDialog();
        }
        private void InitializeWaterFallDialog()
        {
            //Create Waterfall steps
            var waterfallSteps = new WaterfallStep[]
            {
                DescriptionStepAsync,
                CallbackTimeStepAsync,
                PhoneNumberStepAsync,
                BugStepAsync,
                SummaryStepAsync
            };

            //Add Named Dialogs
            AddDialog(new WaterfallDialog($"{nameof(BugReportDialog)}.mainFlow", waterfallSteps));
            AddDialog(new TextPrompt($"{nameof(BugReportDialog)}.description"));
           // AddDialog(new DateTimePrompt($"{nameof(BugReportDialog)}.callbackTime", CallbackTimeValidatorAsync));
           // AddDialog(new TextPrompt($"{nameof(BugReportDialog)}.phoneNumber", PhoneNumberValidatorAsync));
            AddDialog(new ChoicePrompt($"{nameof(BugReportDialog)}.bug"));

            //set the starting Dialog
          //  IntialDialogId = $"{nameof(BugReportDialog)}.mainFlow";

            
        }
        //waterfall steps
        private async Task<DialogTurnResult> DescriptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
            {
                return await stepContext.PromptAsync($"{nameof(BugReportDialog)}.description",
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Enter a description for you Report")
                    }, cancellationToken);
                    
            }
        private async Task<DialogTurnResult> CallbackTimeStepAsync(WaterfallStepContext stepContext,CancellationToken cancellationToken)
        {
            stepContext.Values["description"] = (string)stepContext.Result;
            return await stepContext.PromptAsync($"{nameof(BugReportDialog)}.callbackTime",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Please enter callback time"),
                    RetryPrompt = MessageFactory.Text("The Value entered must be between the hours of 9am and 5pm"),
                }, cancellationToken);
        }
        private async Task<DialogTurnResult> PhoneNumberStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["CallbackTime"] = Convert.ToDateTime(((List<DateTimeResolution>)stepContext.Result).FirstOrDefault().Value);
            return await stepContext.PromptAsync($"{nameof(BugReportDialog)}.phoneNumber",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Please enter the phone number that we can call you back atcallback time"),
                    RetryPrompt = MessageFactory.Text("Please enter a valid phone number "),
                }, cancellationToken);
        }
        private async Task<DialogTurnResult> BugStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["phoneNumber"] = (string)stepContext.Result;
            return await stepContext.PromptAsync($"{nameof(BugReportDialog)}.bug",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Please enter the type of bug."),
                    Choices= ChoiceFactory.ToChoices(new List<string> { "Security", "Crash","Power","Performance","Usability","Serious Bug", "Other"}),
                 }, cancellationToken);
        }
        private async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["bug"] = (FoundChoice)stepContext.Result;
            //Get the current profile object from user state
            var userProfile = await _botStsteService.UserProfileAccessor.GetAsync(stepContext.Context, () => new Model.UserProfile(), cancellationToken);
            //save all of the data inside the user profile
            userProfile.Description = (string)stepContext.Values["description"];
            //userProfile.CallbackTime = (string)stepContext.Values["callbacktime"];
            userProfile.PhoneNumber = (string)stepContext.Values["phoneNumber"];
            userProfile.Bug = (string)stepContext.Values["bug"];
            //show the summary to the user
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Here is a summary of your bug report:"), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format("Description:{0}",userProfile.Description)), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format("Callback Time:{0}",userProfile.CallbackTime)), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format("Phone Number:{0}",userProfile.PhoneNumber)), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format("Bug:{0}",userProfile.Bug)), cancellationToken);
            //save data in userstate
            await _botStsteService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);
            //waterfallStep always finishes with the end of the waterfall or with another dialog here it is the end
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

    }

    }

