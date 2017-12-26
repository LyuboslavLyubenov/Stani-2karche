-debug
-target:library
-nowarn:0169
-langversion:4
-out:Temp/Assembly-CSharp.dll
-unsafe
-r:"C:/Program Files/Unity/Editor/Data/Managed/UnityEngine.dll"
-r:Library/ScriptAssemblies/Assembly-CSharp-firstpass.dll
-r:"C:/Program Files/Unity/Editor/Data/UnityExtensions/Unity/GUISystem/UnityEngine.UI.dll"
-r:"C:/Program Files/Unity/Editor/Data/UnityExtensions/Unity/Networking/UnityEngine.Networking.dll"
-r:"C:/Program Files/Unity/Editor/Data/UnityExtensions/Unity/TestRunner/UnityEngine.TestRunner.dll"
-r:"C:/Program Files/Unity/Editor/Data/UnityExtensions/Unity/TestRunner/net35/unity-custom/nunit.framework.dll"
-r:"C:/Program Files/Unity/Editor/Data/UnityExtensions/Unity/UnityAnalytics/UnityEngine.Analytics.dll"
-r:"C:/Program Files/Unity/Editor/Data/UnityExtensions/Unity/UnityHoloLens/RuntimeEditor/UnityEngine.HoloLens.dll"
-r:"C:/Program Files/Unity/Editor/Data/UnityExtensions/Unity/UnityVR/RuntimeEditor/UnityEngine.VR.dll"
-r:Assets/Libraries/CSharpJExcel.dll
-r:"C:/Program Files/Unity/Editor/Data/Managed/UnityEditor.dll"
-define:UNITY_5_3_OR_NEWER
-define:UNITY_5_4_OR_NEWER
-define:UNITY_5_5_OR_NEWER
-define:UNITY_5_6_OR_NEWER
-define:UNITY_5_6_4
-define:UNITY_5_6
-define:UNITY_5
-define:UNITY_ANALYTICS
-define:ENABLE_AUDIO
-define:ENABLE_CACHING
-define:ENABLE_CLOTH
-define:ENABLE_DUCK_TYPING
-define:ENABLE_GENERICS
-define:ENABLE_PVR_GI
-define:ENABLE_MICROPHONE
-define:ENABLE_MULTIPLE_DISPLAYS
-define:ENABLE_PHYSICS
-define:ENABLE_RUNTIME_NAVMESH_BUILDING
-define:ENABLE_SPRITERENDERER_FLIPPING
-define:ENABLE_SPRITES
-define:ENABLE_TERRAIN
-define:ENABLE_RAKNET
-define:ENABLE_UNET
-define:ENABLE_LZMA
-define:ENABLE_UNITYEVENTS
-define:ENABLE_WEBCAM
-define:ENABLE_WWW
-define:ENABLE_CLOUD_SERVICES_COLLAB
-define:ENABLE_CLOUD_SERVICES_ADS
-define:ENABLE_CLOUD_HUB
-define:ENABLE_CLOUD_PROJECT_ID
-define:ENABLE_CLOUD_SERVICES_UNET
-define:ENABLE_CLOUD_SERVICES_BUILD
-define:ENABLE_CLOUD_LICENSE
-define:ENABLE_EDITOR_METRICS
-define:ENABLE_EDITOR_METRICS_CACHING
-define:ENABLE_NATIVE_ARRAY
-define:INCLUDE_DYNAMIC_GI
-define:INCLUDE_GI
-define:PLATFORM_SUPPORTS_MONO
-define:RENDER_SOFTWARE_CURSOR
-define:INCLUDE_PUBNUB
-define:ENABLE_PLAYMODE_TESTS_RUNNER
-define:ENABLE_SCRIPTING_NEW_CSHARP_COMPILER
-define:ENABLE_VIDEO
-define:UNITY_STANDALONE_WIN
-define:UNITY_STANDALONE
-define:ENABLE_SUBSTANCE
-define:ENABLE_RUNTIME_GI
-define:ENABLE_MOVIES
-define:ENABLE_NETWORK
-define:ENABLE_CRUNCH_TEXTURE_COMPRESSION
-define:ENABLE_UNITYWEBREQUEST
-define:ENABLE_CLOUD_SERVICES
-define:ENABLE_CLOUD_SERVICES_ANALYTICS
-define:ENABLE_CLOUD_SERVICES_PURCHASING
-define:ENABLE_CLOUD_SERVICES_CRASH_REPORTING
-define:ENABLE_EVENT_QUEUE
-define:ENABLE_CLUSTERINPUT
-define:ENABLE_VR
-define:ENABLE_WEBSOCKET_HOST
-define:ENABLE_MONO
-define:NET_2_0
-define:ENABLE_PROFILER
-define:DEBUG
-define:TRACE
-define:UNITY_ASSERTIONS
-define:UNITY_EDITOR
-define:UNITY_EDITOR_64
-define:UNITY_EDITOR_WIN
-define:ENABLE_NATIVE_ARRAY_CHECKS
-define:UNITY_TEAM_LICENSE
"Assets/CielaSpike/Thread Ninja/Example/ExampleScript.cs"
"Assets/CielaSpike/Thread Ninja/Ninja.cs"
"Assets/CielaSpike/Thread Ninja/Task.cs"
"Assets/CielaSpike/Thread Ninja/TaskState.cs"
"Assets/CielaSpike/Thread Ninja/ThreadNinjaMonoBehaviourExtensions.cs"
Assets/PlayerPrefsEditor/PlayerPrefsUtility.cs
Assets/PlayerPrefsEditor/SimpleEncryption.cs
Assets/Reporter/MultiKeyDictionary.cs
Assets/Reporter/Reporter.cs
Assets/Reporter/ReporterGUI.cs
Assets/Reporter/ReporterMessageReceiver.cs
Assets/Scene/QuestionPanel/QuestionPanelRepeating.cs
Assets/Scripts/AnimationControllers/DestroyOnAnimationEnd.cs
Assets/Scripts/AnimationControllers/DisableAfterAnimationOver.cs
Assets/Scripts/AnimationControllers/FireSelectedAnswerEvent.cs
Assets/Scripts/AnimationControllers/InitializeChooseCategoryControllerAfterAnimationLoad.cs
Assets/Scripts/Commands/ActivateStateCommand.cs
Assets/Scripts/Commands/Client/AnswerTimeoutCommand.cs
Assets/Scripts/Commands/Client/AskAudienceVoteOneTimeCommand.cs
Assets/Scripts﻿namespace Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class NetworkCommandData
    {
        public const int CODE_Option_ClientConnectionId_All = -1;
        public const int CODE_Option_ClientConnectionId_Random = -2;
        public const int CODE_Option_ClientConnectionId_AI = -3;

        private const int MinCommandNameLength = 3;
        private const int MaxCommandNameLength = 100;

        private const int MinOptionsCount = 1;

        private const int MinOptionNameLength = 1;
        private const int MinOptionValueLength = 1;

        private const string NotEnoughSymbols = "{0} must be at least {1} symbols long";
        private const string TooMuchSymbols = "{0} must be less than {1} symbols long";
        private const string OptionAlreadyExists = "Already have option with name {0}";
        private const string CantFindOption = "Cant find option with name {0}";

        private readonly string commandName;
        private readonly Dictionary<string, string> commandOptions;

        public string Name
        {
            get
            {
                return this.commandName;
            }
        }

        /// <summary>
        /// Cannot set values from here
        /// </summary>
        public Dictionary<string, string> Options
        {
            get
            {
                return this.commandOptions.ToDictionary(co => co.Key, co => co.Value);
            }
        }

        public NetworkCommandData(string commandName, Dictionary<string, string> commandOptions)
        {
            ValidateCommandName(commandName);

            if (commandOptions == null)
            {
                throw new ArgumentNullException("commandOptions");
            }

            foreach (var option in commandOptions.ToList())
            {
                this.ValidateOption(option.Key, option.Value);
            }

            this.commandName = commandName;
            this.commandOptions = commandOptions;
        }

        public NetworkCommandData(string commandName)
            : this(commandName, new Dictionary<string, string>())
        {
        }

        public static NetworkCommandData From<T>()
        {
            var commandName = typeof(T).Name.Replace("Command", "");
            return new NetworkCommandData(commandName);
        }

        private static void ValidateCommandName(string commandName)
        {
            if (string.IsNullOrEmpty(commandName) || commandName.Trim().Length < MinCommandNameLength)
            {
                var exceptionMessage = string.Format(NotEnoughSymbols, "commandName", MinCommandNameLength);
                throw new ArgumentException(exceptionMessage);
            }

            if (commandName.Length > MaxCommandNameLength)
            {
                var exceptionMessage = string.Format(TooMuchSymbols, "commandName", MaxCommandNameLength);
                throw new ArgumentException(exceptionMessage);
            }
        }

        private void ValidateOption(string optionName, string optionValue)
        {
            if (string.IsNullOrEmpty(optionName) || optionName.Trim().Length < MinOptionNameLength)
            {
                var exceptionMessage = string.Format(NotEnoughSymbols, "optionName", MinOptionNameLength);
                throw new ArgumentException(exceptionMessage);
            } 

            if (string.IsNullOrEmpty(optionValue) || optionValue.Trim().Length < MinOptionValueLength)
            {
                var exceptionMessage = string.Format(NotEnoughSymbols, "optionValue", MinOptionValueLength);
                throw new ArgumentException(exceptionMessage);
            }
        }
       
        private static int FilterNameLength(string[] commandArgs)
        {
            int commandNameLength = -1;

            try
            {
                commandNameLength = int.Parse(commandArgs[0]);
            }catch 
            {

            }

            return 0;
        }
    }
}                                                                                                                                                                                                                                                                                                                                                                                                                                                     /NotificationFromServerCommand.cs
Assets/Scripts/Commands/Client/SendDeviceIdToServerCommand.cs
Assets/Scripts/Commands/Client/TimeToAnswerCommand.cs
Assets/Scripts/Commands/CommandsManager.cs
Assets/Scripts/Commands/CreatedGameInfoReceiver/ReceiveGameInfoRequest.cs
Assets/Scripts/Commands/CreatedGameInfoReceiver/ReceivedGameInfoCommand.cs
Assets/Scripts/Commands/CreatedGameInfoSender/SendGameInfoCommand.cs
Assets/Scripts/Commands/DummyCommand.cs
Assets/Scripts/Commands/DummyOneTimeCommand.cs
Assets/Scripts/Commands/EveryBodyVsTheTeacher/EnoughPlayersToStartGameCommand.cs
Assets/Scripts/Commands/EveryBodyVsTheTeacher/InCorrectAnswerCommand.cs
Assets/Scripts/Commands/EveryBodyVsTheTeacher/NotEnoughPlayersToStartGameCommand.cs
Assets/Scripts/Commands/EveryBodyVsTheTeacher/PlayersConnectingState/AudiencePlayerConnectedCommand.cs
Assets/Scripts/Commands/EveryBodyVsTheTeacher/PlayersConnectingState/AudiencePlayerDisconnectedCommand.cs
Assets/Scripts/Commands/EveryBodyVsTheTeacher/PlayersConnectingState/EveryBodyRequestedGameStartCommand.cs
Assets/Scripts/Commands/EveryBodyVsTheTeacher/PlayersConnectingState/MainPlayerConnectedCommand.cs
Assets/Scripts/Commands/EveryBodyVsTheTeacher/PlayersConnectingState/MainPlayerDisconnectedCommand.cs
Assets/Scripts/Commands/EveryBodyVsTheTeacher/PlayersConnectingState/MainPlayerRequestedGameStartCommand.cs
Assets/Scripts/Commands/EveryBodyVsTheTeacher/Presenter/States/Jokers/Kalitko/PlayerSelectedAgainstKalitkoJokerCommand.cs
Assets/Scripts/Commands/EveryBodyVsTheTeacher/Presenter/States/Jokers/Kalitko/PlayerSelectedForKalitkoJokerCommand.cs
Assets/Scripts/Commands/EveryBodyVsTheTeacher/PresenterConnectingCommand.cs
Assets/Scripts/Commands/EveryBodyVsTheTeacher/Shared/GameStartedCommand.cs
Assets/Scripts/Commands/EveryBodyVsTheTeacher/Shared/SwitchedToNextRoundCommand.cs
Assets/Scripts/Commands/EveryBodyVsTheTeacher/StartGameRequestCommand.cs
Assets/Scripts/Commands/EveryBodyVsTheTeacher/SurrenderCommand.cs
Assets/Scripts/Commands/GameData/GameDataGetQuestionAbstractCommand.cs
Assets/Scripts/Commands/GameData/GameDataGetQuestionRouterCommand.cs
Assets/Scripts/Commands/GameData/GameDataMarkCommand.cs
Assets/Scripts/Commands/GameData/GameDataNoMoreQuestionsCommand.cs
Assets/Scripts/Commands/GameData/GameDataQuestionCommand.cs
Assets/Scripts/Commands/GameData/GetCurrentQuestionCommand.cs
Assets/Scripts/Commands/GameData/GetNextQuestionCommand.cs
Assets/Scripts/Commands/GameData/LoadedGameDataCommand.cs
Assets/Scripts/Commands/Jokers/Add/AddAskAudienceJokerCommand.cs
Assets/Scripts/Commands/Jokers/Add/AddDisableRandomAnswersJokerCommand.cs
Assets/Scripts/Commands/Jokers/Add/AddHelpFromFriendJokerCommand.cs
Assets/Scripts/Commands/Jokers/Add/AddJokerAbstractCommand.cs
Assets/Scripts/Commands/Jokers/Add/AddRandomJokerCommand.cs
Assets/Scripts/Commands/Jokers/Add/EveryBodyVsTheTeacher/MainPlayer/AddAskAudienceJokerCommand.cs
Assets/Scripts/Commands/Jokers/Add/EveryBodyVsTheTeacher/MainPlayer/AddConsultWithTheTeacherJokerCommand.cs
Assets/Scripts/Commands/Jokers/Add/EveryBodyVsTheTeacher/Ma