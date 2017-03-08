namespace Assets.Scripts.Controllers
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Assets.Scripts.Controllers.Lobby;
    using Assets.Scripts.DTOs;
    using Assets.Scripts.Interfaces;
    using Assets.Scripts.Notifications;
    using Assets.Scripts.Utils;
    
    using UnityEngine;

    public class SelectPlayerTypeRouter
    {
        private readonly BasicExamServerSelectPlayerTypeUIController basicExamServerSelectPlayerTypeUIController;
        private readonly EveryBodyVsTheTeacher.SelectPlayerTypeUIController everyBodyVsTheTeacherSelectPlayerTypeUiController;
        
        public SelectPlayerTypeRouter(
            BasicExamServerSelectPlayerTypeUIController basicExamServerSelectPlayerTypeUIController,
            EveryBodyVsTheTeacher.SelectPlayerTypeUIController everyBodyVsTheTeacherSelectPlayerTypeUiController)
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
            var methodName = "OnConnectingTo" + gameType;
            var method = this.GetType()
                .GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (method == null)
            {
                NotificationsesController.Instance.AddNotification(Color.red, "Неподържан вид игра", 10);
                return;
            }

            var dto = this.ParseJSONToSpecificGameTypeDTO(gameType, gameInfoJSON);

            method.Invoke(this, new[] { dto });
        }

        private object ParseJSONToSpecificGameTypeDTO(string gameType, string gameTypeJSON)
        {
            var typeName = gameType + "GameInfo_DTO";
            var type = Assembly.GetExecutingAssembly()
                .GetTypes()
                .First(t => t.Name == typeName);
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
            var gameInfo = (EveryBodyVsTheTeacherGameInfo_DTO)gameInfo_DTO;
            this.everyBodyVsTheTeacherSelectPlayerTypeUiController.gameObject.SetActive(true);
            ThreadUtils.Instance.RunOnMainThread(() => this.everyBodyVsTheTeacherSelectPlayerTypeUiController.Initialize(gameInfo));
        }

        public void Handle(string gameType, string gameTypeJSON)
        {
            RouteReceivedGameInfo(gameType, gameTypeJSON);
        }
    }
}