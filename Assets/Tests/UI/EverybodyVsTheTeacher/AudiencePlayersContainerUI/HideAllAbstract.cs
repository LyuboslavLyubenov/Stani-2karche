using ObjectsPool = Utils.Unity.ObjectsPool;

namespace Assets.Tests.UI.EverybodyVsTheTeacher.AudiencePlayersContainerUI
{

    using System.Collections;
    using System.Linq;

    using Assets.Scripts.Extensions;

    using Interfaces.Controllers;

    using UnityEngine;
    using UnityEngine.UI;

    using UnityTestTools.IntegrationTestsFramework.TestRunner;

    using Zenject;

    public class HideAllAbstract : MonoBehaviour
    {
        [Inject]
        protected IAudiencePlayersContainerUIController audiencePlayersContainerUIController;

        protected int NumberOfPlayersToShow;
        
        private IEnumerator StartTestCoroutine()
        {
            var audiencePlayersUsernames = Enumerable.Range(1, this.NumberOfPlayersToShow)
                .Select(n => "Player " + n)
                .ToArray();

            for (int i = 0; i < audiencePlayersUsernames.Length; i++)
            {
                var username = audiencePlayersUsernames[i];
                var connectionId = username.Split(' ')[1].ConvertTo<int>();
                this.audiencePlayersContainerUIController.ShowAudiencePlayer(connectionId, username);
                yield return new WaitForSeconds(1f);
            }

            yield return new WaitForSeconds(1f);

            this.audiencePlayersContainerUIController.HideAll();

            yield return null;

            var objectsPool = GameObject.FindObjectOfType<ObjectsPool>()
                .gameObject
                .transform;

            for (int i = 0; i < objectsPool.childCount; i++)
            {
                var audiencePlayerObject = objectsPool.GetChild(i).gameObject;

                if (!audiencePlayerObject.activeSelf)
                {
                    continue;
                }

                var audiencePlayerUsername = audiencePlayerObject.GetComponentInChildren<Text>().text;
                var audiencePlayerConnectionId = audiencePlayerUsername.Split(' ')[1]
                    .ConvertTo<int>();

                if (audiencePlayersUsernames.Contains(audiencePlayerUsername) ||
                    this.audiencePlayersContainerUIController.IsOnScreen(audiencePlayerConnectionId))
                {
                    IntegrationTest.Fail();
                    yield break;
                }
            }

            IntegrationTest.Pass();
        }

        protected void StartTest()
        {
            this.StartCoroutine(this.StartTestCoroutine());
        }
    }

}