using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class MessageManager
{
    public event EventHandler StateChanged;
    private int maxMessages = 12;
    private List<Message> messages = new List<Message>();
    public void AddMessage(string newMessageString)
    {
        Message newMessage = new Message(newMessageString);
        messages.Add(newMessage);
        if (messages.Count >= maxMessages)
        {
            messages.Remove(messages[0]);
        }
        StateHasChanged();
    }
    public List<Message> GetMessages()
    {
        return messages;
    }
    private void StateHasChanged()
    {
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
