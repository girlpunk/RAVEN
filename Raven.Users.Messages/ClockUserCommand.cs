using Silverback.Messaging.Messages;

namespace Raven.Users.Messages;

public class ClockUserCommand : ICommand<bool>
{
    public string cardValue { get; set; }
    public DateTime ReadTimestamp { get; set; }
    public KSUID.Ksuid Reader { get; set; }
}
