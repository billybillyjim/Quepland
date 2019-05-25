using System;

public class Message
{
    public string messageText { get; set; }
    public string color;
    public Message(string messageString)
    {
        messageText = messageString;
    }
    public Message(string messageString, string color)
    {
        messageText = messageString;
        this.color = color;
    }
}
