using UnityEngine;
using System.Linq;

public class TEST_RetrieveServerInfo : ExtendedMonoBehaviour
{
    void Start()
    {
        CoroutineUtils.WaitForSeconds(1, TestRetrieveGameInfo);
    }

    void TestRetrieveGameInfo()
    {
        KinveyWrapper.Instance.LoginAsync("ivan", "ivan", (data) =>
            {
                KinveyWrapper.Instance.RetrieveEntityAsync<CreatedGameInfo_Serializable>("Servers", null, (retrieved) =>
                    {
                        retrieved.ToList().ForEach(e => Debug.LogFormat("{0} {1}", e.Entity.HostUsername, e.EntityDetails._id));
                    }, Debug.LogException);
            }, Debug.LogException);
    }
}
