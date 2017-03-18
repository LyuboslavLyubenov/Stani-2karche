using KinveyWrapper = Network.KinveyWrapper;

namespace Tests.Test_Kinvey_Wrapper.Upload_and_retrieve_from_collection
{

    using System.Linq;

    using DTOs;
    using DTOs.KinveyDtoObjs;

    using UnityEngine;

    using Utils.Unity;

    public class TEST_RetrieveServerInfo : ExtendedMonoBehaviour
    {
        private void Start()
        {
            this.CoroutineUtils.WaitForSeconds(1, this.TestRetrieveGameInfo);
        }

        private void TestRetrieveGameInfo()
        {
            var kinveyWrapper = new KinveyWrapper();
            kinveyWrapper.LoginAsync("ivan", "ivan", (data) => this.OnReceivedLoginResponse(kinveyWrapper, data), Debug.LogException);
        }

        private void OnReceivedLoginResponse(KinveyWrapper kinveyWrapper, _UserReceivedData data)
        {
            kinveyWrapper.RetrieveEntityAsync<CreatedGameInfo_DTO>("Servers", null, (retrieved) =>
            {
                retrieved.ToList(
                    ).ForEach(e => Debug.LogFormat("{0} {1}", e.Entity.HostUsername, e.EntityDetails._id));
            }, Debug.LogException);
        }
    }

}
