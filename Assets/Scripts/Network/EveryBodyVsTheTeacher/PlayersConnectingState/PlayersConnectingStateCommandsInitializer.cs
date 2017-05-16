namespace Network.EveryBodyVsTheTeacher.PlayersConnectingState
{
    using System;

    using Commands.EveryBodyVsTheTeacher.PlayersConnectingState;
    
    using Interfaces.Controllers;
    using Interfaces.Network.NetworkManager;
    
    public class PlayersConnectingStateCommandsInitializer
    {
        PlayersConnectingStateCommandsInitializer()
        {
        }

        public static void InitializeCommands(
            IClientNetworkManager networkManager,
            IMainPlayersContainerUIController mainPlayersContainerUiController,
            IAudiencePlayersContainerUIController audiencePlayersContainerUiController,
            Action onEveryBodyRequestedGameStart)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            if (mainPlayersContainerUiController == null)
            {
                throw new ArgumentNullException("mainPlayersContainerUiController");
            }

            if (audiencePlayersContainerUiController == null)
            {
                throw new ArgumentNullException("audiencePlayersContainerUiController");
            }

            if (onEveryBodyRequestedGameStart == null)
            {
                throw new ArgumentNullException("onEveryBodyRequestedGameStart");
            }

            var mainPlayerConnectedCommand = new MainPlayerConnectedCommand(mainPlayersContainerUiController);
            var mainPlayerDisconnectedCommand = new MainPlayerDisconnectedCommand(mainPlayersContainerUiController);
            var audiencePlayerConnected = new AudiencePlayerConnectedCommand(audiencePlayersContainerUiController);
            var audiencePlayerDisconnectedCommand = new AudiencePlayerDisconnectedCommand(audiencePlayersContainerUiController);
            var mainPlayerRequestedGameStartCommand = new MainPlayerRequestedGameStartCommand(mainPlayersContainerUiController);
            var everyBodyRequestedGameStartCommand = new EveryBodyRequestedGameStartCommand();
            
            everyBodyRequestedGameStartCommand.OnExecuted += (sender, args) => onEveryBodyRequestedGameStart();

            var commandsManager = networkManager.CommandsManager;
            var allCommands = new INetworkManagerCommand[]
                              {
                                  mainPlayerConnectedCommand,
                                  mainPlayerDisconnectedCommand,
                                  audiencePlayerConnected,
                                  audiencePlayerDisconnectedCommand,
                                  mainPlayerRequestedGameStartCommand,
                                  everyBodyRequestedGameStartCommand
                              };
            commandsManager.AddCommands(allCommands);
        }

        public static void CleanCommands(IClientNetworkManager networkManager)
        {
            if (networkManager == null)
            {
                throw new ArgumentNullException("networkManager");
            }

            networkManager.CommandsManager.RemoveCommand<MainPlayerConnectedCommand>();
            networkManager.CommandsManager.RemoveCommand<MainPlayerDisconnectedCommand>();
            networkManager.CommandsManager.RemoveCommand<AudiencePlayerConnectedCommand>();
            networkManager.CommandsManager.RemoveCommand<AudiencePlayerDisconnectedCommand>();
            networkManager.CommandsManager.RemoveCommand<MainPlayerRequestedGameStartCommand>();
            networkManager.CommandsManager.RemoveCommand<EveryBodyRequestedGameStartCommand>();
        }
    }
}
