using LeapSession.Model;
using LeapSession.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LeapSession.Bots
{
    public class GreetingBot:ActivityHandler
    {
        private readonly BotStateService _botStateService;
        public GreetingBot(BotStateService botStateService)
        {
            _botStateService = botStateService ?? throw new System.ArgumentNullException(nameof(botStateService)) ;
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await GetName(turnContext, cancellationToken);
        }
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach(var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await GetName(turnContext, cancellationToken);
                }
            }
        }
        private async Task GetName(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _botStateService.UserProfileAccessor.GetAsync(turnContext, () => new UserProfile());
            ConversationData conversationData = await _botStateService.ConversationDataAccessor.GetAsync(turnContext, () => new ConversationData());

            if (!string.IsNullOrEmpty(userProfile.name))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(string.Format("Hi {0}. How can I help you today?", userProfile.name)), cancellationToken);
            }
            else
            {
                if (conversationData.PromptedUserForName)
                {
                   //set the nameto what the user provided
                    userProfile.name = turnContext.Activity.Text?.Trim();
                    //Acknowledge that we got their name.
                    await turnContext.SendActivityAsync(MessageFactory.Text(string.Format("Thanks {0}. How can i help you today?", userProfile.name)), cancellationToken);
                    //reset the flag to allow bot to go through the cycle again
                    conversationData.PromptedUserForName = false;


                }
                else
                {
                    //prompt user for their name
                    await turnContext.SendActivityAsync(MessageFactory.Text($"What is your name?"), cancellationToken);
                    //set the flag to true, so we don't prompt in the next turn
                    conversationData.PromptedUserForName = true;
                }
                //save any changes to state
                await _botStateService.UserProfileAccessor.SetAsync(turnContext, userProfile);
                await _botStateService.ConversationDataAccessor.SetAsync(turnContext, conversationData);

                await _botStateService._userState.SaveChangesAsync(turnContext);
                await _botStateService._conversationState.SaveChangesAsync(turnContext);


            }
        }



    }
}
