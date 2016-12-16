using System;
using UnityEngine;
using System.Timers;

public class AskPlayerQuestionResultRetriever : ExtendedMonoBehaviour
{
    const int SettingsReceiveTimeoutInSeconds = 5;

    static AskPlayerQuestionResultRetriever instance;

    public static AskPlayerQuestionResultRetriever Instance
    {
        get
        {
            if (instance == null)
            {
                var obj = new GameObject();
                obj.name = typeof(AskPlayerQuestionResultRetriever).Name;
                instance = obj.AddComponent<AskPlayerQuestionResultRetriever>();
            }

            return instance;
        }
    }

    public EventHandler<AskPlayerResponseEventArgs> OnReceivedAnswer = delegate
    {
    };

    public EventHandler<JokerSettingsEventArgs> OnReceivedSettings = delegate
    {
    };

    public EventHandler OnReceiveAnswerTimeout = delegate
    {
    };

    public EventHandler OnReceiveSettingsTimeout = delegate
    {
    };
    
    ClientNetworkManager networkManager;

    Timer timer;

    void Awake()
    {
        networkManager = GameObject.FindObjectOfType<ClientNetworkManager>();     
    }

    void _OnReceivedSettings(int timeToAnswerInSeconds)
    {
        timer.Stop();
        timer.Close();

        var responseCommand = new AskPlayerResponseCommand(_OnReceivedAnswer);
        networkManager.CommandsManager.AddCommand(responseCommand);

        timer = new Timer(SettingsReceiveTimeoutInSeconds * 1000);
        timer.AutoReset = false;
        timer.Elapsed += Timer_OnReceiveAnswerTimeout;

        OnReceivedSettings(this, new JokerSettingsEventArgs(timeToAnswerInSeconds));
    }

    void Timer_OnReceiveSettingsTimeout(object sender, ElapsedEventArgs args)
    {
        ThreadUtils.Instance.RunOnMainThread(() =>
            {
                timer.Close();
                networkManager.CommandsManager.RemoveCommand<HelpFromFriendJokerSettingsCommand>();
                OnReceiveSettingsTimeout(this, EventArgs.Empty);
            });
    }

    void _OnReceivedAnswer(string username, string answer)
    {
        OnReceivedAnswer(this, new AskPlayerResponseEventArgs(username, answer));
    }

    void Timer_OnReceiveAnswerTimeout(object sender, ElapsedEventArgs args)
    {
        ThreadUtils.Instance.RunOnMainThread(() =>
            {
                timer.Close();
                networkManager.CommandsManager.RemoveCommand<HelpFromFriendJokerSettingsCommand>();
                OnReceiveSettingsTimeout(this, EventArgs.Empty);
            });
    }

    public void Activate(int playerConnectionId)
    {
        var selected = NetworkCommandData.From<SelectedAskPlayerQuestionCommand>();
        selected.AddOption("PlayerConnectionId", playerConnectionId.ToString());

        networkManager.SendServerCommand(selected);

        var receivedSettingsCommand = new HelpFromFriendJokerSettingsCommand(_OnReceivedSettings);
        networkManager.CommandsManager.AddCommand(receivedSettingsCommand);

        timer = new Timer(SettingsReceiveTimeoutInSeconds * 1000);
        timer.AutoReset = false;
        timer.Elapsed += Timer_OnReceiveSettingsTimeout;
    }
}
