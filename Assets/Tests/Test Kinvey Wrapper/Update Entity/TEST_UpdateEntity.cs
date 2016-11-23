using UnityEngine;
using System.Collections;
using System.Linq;

public class TEST_UpdateEntity : MonoBehaviour
{
    void Start()
    {
        KinveyWrapper.Instance.LoginAsync("ivan", "ivan", (data) =>
            {
                var tableName = "Servers";
                var id = "58356df6f08321f70dc31bd3";

                KinveyWrapper
                    .Instance
                    .RetrieveEntityAsync<CreatedGameInfo_Serializable>(
                    tableName, 
                    id, 
                    (retrievedData) =>
                    {
                        var entity = retrievedData.First().Entity;
                        entity.HostUsername = "ivan promenen";

                        KinveyWrapper.Instance.UpdateEntityAsync<CreatedGameInfo_Serializable>(tableName, id, entity, () =>
                            {
                                Debug.Log("bravo");
                            }, Debug.LogException);
                            
                    }, Debug.LogException);
                
            }, Debug.LogException);
    }
}
