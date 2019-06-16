using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class MessageManager
{
    public event EventHandler StateChanged;
    private int maxMessages = 12;
    private List<Message> messages = new List<Message>();
    private string lastMessage;
    private int repeatMessageCount = 1;
    public void AddMessage(string newMessageString)
    {
        AddMessage(newMessageString, "black");
    }
    public void AddMessage(string newMessageString, string color)
    {
        Message newMessage = new Message(newMessageString, color);
        if (lastMessage == newMessageString)
        {
            repeatMessageCount++;
            messages.Last().messageText =  lastMessage + "(" + repeatMessageCount + ")";         
        }
        else
        {
            repeatMessageCount = 1;
            messages.Add(newMessage);
            if (messages.Count >= maxMessages)
            {
                messages.Remove(messages[0]);
            }
        }

        lastMessage = newMessage.messageText;
        StateHasChanged();
    }
    public List<Message> GetMessages()
    {
        return messages;
    }
    public List<Message> GetReversedMessages()
    {
        return messages.Reverse<Message>().ToList();
    }
    private void StateHasChanged()
    {
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
