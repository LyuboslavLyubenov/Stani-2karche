using ObjectsPool = Utils.Unity.ObjectsPool;

namespace Assets.Tests.UI.EverybodyVsTheTeacher.AudiencePlayersContainerUI
{
    using System.Collections;

    using Interfaces.Controllers;

    using UnityEngine;
    using UnityEngine.UI;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject.Source.Usage;

    public class WhenPlayerHiddenAndHaveMoreThan3PlayersShowPreviousConnectedOnScreen : MonoBehaviour
    {
        [Inject]
        private IAudiencePlayersContainerUIController audiencePlayersContainerUIController;

        void Start()
        {
            this.StartCoroutine(this.StartTest());
        }

        IEnumerator ShowPlayers()
        {
            for (int i = 1; i < 6; i++)
            {
                yield return new WaitForSeconds(1f);
                this.audiencePlayersContainerUIController.ShowAudiencePlayer(i, "Player " + i);
            }

            yield return new WaitForSeconds(5f);
        }
        
        IEnumerator StartTest()
        {
            yield return StartCoroutine(this.ShowPlayers());

            this.audiencePlayersContainerUIController.HideAudiencePlayer(5);

            yield return new WaitForSeconds(1f);

            var poolObject = GameObject.FindObjectOfType<ObjectsPool>()
                .transform;

            for (int i = 0; i < poolObject.childCount; i++)
            {
                var audiencePlayerObject = poolObject.GetChild(i);

                if (!audiencePlayerObject.gameObject.activeSelf)
                {
                    continue;
                }

                var audiencePlayerUsername = audiencePlayerObject.GetComponentInChildren<Text>()
                    .text;

                if (audiencePlayerUsername == "Player 5")
                {
                    IntegrationTest.Fail();
                    yield break;
                }

                if (audiencePlayerUsername == "Player 2")
                {
                    IntegrationTest.Pass();
                    yield break;
                }
            }
        }
    }
}