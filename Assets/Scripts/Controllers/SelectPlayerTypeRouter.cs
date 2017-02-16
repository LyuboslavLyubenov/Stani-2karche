namespace Assets.Scripts.Controllers
{
    using System;
    using System.Reflection;
    
    using Assets.Scripts.DTOs;
    using Assets.Scripts.Network;
    using Assets.Scripts.Notifications;
    using Assets.Scripts.Utils;
    
    using UnityEngine;

    using Zenject;

    public class SelectPlayerTypeRouter
    {
        private readonly BasicExamServerSelectPlayerTypeUIController basicExamServerSelectPlayerTypeUIController;
        private readonly EveryBodyVsTheTeacher.ServerSelectPlayerTypeUIController everyBodyVsTheTeacherSelectPlayerTypeUiController;

        [Inject]
        private CreatedGameInfoReceiverService gameInfoReceiverService;
        
        public SelectPlayerTypeRouter(
            BasicExamServerSelectPlayerTypeUIController basicExamServerSelectPlayerTypeUIController,
            EveryBodyVsTheTeacher.ServerSelectPlayerTypeUIController everyBodyVsTheTeacherSelectPlayerTypeUiController)
        {
            if (basicExamServerSelectPlayerTypeUIController == null)
            {
                throw new ArgumentNullException("basicExamServerSelectPlayerTypeUIController");
            }

            if (everyBodyVsTheTeacherSelectPlayerTypeUiController == null)
            {
                throw new ArgumentNullException("everyBodyVsTheTeacherSelectPlayerTypeUiController");
            }
            
            this.basicExamServerSelectPlayerTypeUIController = basicExamServerSelectPlayerTypeUIController;
            this.everyBodyVsTheTeacherSelectPlayerTypeUiController = everyBodyVsTheTeacherSelectPlayerTypeUiController;
        }
        
        private void RouteReceivedGameInfo(string gameType, string gameInfoJSON)
        {
            var methodName = "OnConnectingTo " + gameType;
            var method = this.GetType()
                .GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (method == null)
            {
                NotificationsServiceController.Instance.AddNotification(Color.red, "Неподържан вид игра", 10);
                return;
            }

            var dto = this.ParseJSONToSpecificGameTypeDTO(gameType, gameInfoJSON);

            method.Invoke(this, new[] { dto });
        }

        private object ParseJSONToSpecificGameTypeDTO(string gameType, string gameTypeJSON)
        {
            var type = ServerGameTypeUtils.GetGameServerType(gameType);
            var dto = JsonUtility.FromJson(gameTypeJSON, type);
            return dto;
        }

        private void OnConnectingToBasicExam(object gameInfo_DTO)
        {
            var gameInfo = (BasicExamGameInfo_DTO)gameInfo_DTO;
            this.basicExamServerSelectPlayerTypeUIController.gameObject.SetActive(true);
            ThreadUtils.Instance.RunOnMainThread(() => this.basicExamServerSelectPlayerTypeUIController.Initialize(gameInfo));
            
        }

        private void OnConnectingToEveryBodyVsTheTeacher(object gameInfo_DTO)
        {
            var gameInfo = (EverybodyVsTheTeacherGameInfo_DTO)gameInfo_DTO;
            this.everyBodyVsTheTeacherSelectPlayerTypeUiController.gameObject.SetActive(true);
            ThreadUtils.Instance.RunOnMainThread(() => this.everyBodyVsTheTeacherSelectPlayerTypeUiController.Initialize(gameInfo));
        }

        public void Handle(string gameType, string gameTypeJSON)
        {
            RouteReceivedGameInfo(gameType, gameTypeJSON);
        }
    }
}