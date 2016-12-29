using System.Linq;

using UnityEngine;

namespace Assets.Tests.Test_Kinvey_Wrapper.Upload_and_retrieve_from_collection
{

    using Assets.Scripts;
    using Assets.Scripts.DTOs;
    using Assets.Scripts.DTOs.KinveySerializableObj;
    using Assets.Scripts.Network;
    using Assets.Scripts.Utils;
    using Assets.Scripts.Utils.Unity;

    public class TEST_RetrieveServerInfo : ExtendedMonoBehaviour
    {
        void Start()
        {
            this.CoroutineUtils.WaitForSeconds(1, this.TestRetrieveGameInfo);
        }

        void TestRetrieveGameInfo()
        {
            var kinveyWrapper = new KinveyWrapper();
            kinveyWrapper.LoginAsync("ivan", "ivan", (data) => OnReceivedLoginResponse(kinveyWrapper, data), Debug.LogException);
        }

        void OnReceivedLoginResponse(KinveyWrapper kinveyWrapper, _UserReceivedData data)
        {
            kinveyWrapper.RetrieveEntityAsync<CreatedGameInfo_DTO>("Servers", null, (retrieved) =>
            {
                retrieved.ToList(
                    ).ForEach(e => Debug.LogFormat("{0} {1}", e.Entity.HostUsername, e.EntityDetails._id));
            }, Debug.LogException);
        }
    }

}
