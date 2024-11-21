using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChatReceiver
{
    public void ChatReceive(Chat_Message chat_Message);
}
