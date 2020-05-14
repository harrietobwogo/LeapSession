using LeapSession.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;

namespace LeapSession.Services
{
    public class BotStateService
    {//state variables
        public ConversationState _conversationState { get; }
        public UserState _userState { get; }
        //IDs
        public static string UserProfileId { get; } = $"{nameof(BotStateService)}.UserProfile";
        public static string ConversationDataId { get; } = $"{nameof(BotStateService) }.ConversationData";
        public static string DialogStateId { get; } = $"{nameof(BotStateService)}.DialogState";
        //Accessors
        public IStatePropertyAccessor<UserProfile> UserProfileAccessor { get; set; }
        public IStatePropertyAccessor<ConversationData> ConversationDataAccessor { get; set; }
        public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; set; }
        //Constructor for injection
     public BotStateService(ConversationState conversationState,UserState userState)
        {
            _conversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
                
            _userState = userState ?? throw new ArgumentNullException(nameof(userState));
            InitializeAccessors();
        }
//initialize the accessors  
        public void InitializeAccessors()
        {  
            UserProfileAccessor = _userState.CreateProperty<UserProfile>(UserProfileId);
            ConversationDataAccessor = _conversationState.CreateProperty<ConversationData>(ConversationDataId);
            DialogStateAccessor = _conversationState.CreateProperty<DialogState>(DialogStateId);
           
                
        }
    }
}
