using System;

public class UserNameEventArgs : EventArgs
{
    public string UserName
    {
        get;
        private set;
    }

    public UserNameEventArgs(string username)
        : base()
    {
        if (string.IsNullOrEmpty(username))
        {
            throw new ArgumentException("Name cannot be empty");
        }

        this.UserName = username;
    }
}
