using System.Linq;

using UnityEngine;

namespace Assets.Tests.Test_Kinvey_Wrapper.Update_Entity
{

    using Assets.Scripts;
    using Assets.Scripts.DTOs;
    using Assets.Scripts.Network;

    public class TEST_UpdateEntity : MonoBehaviour
    {
        private void Start()
        {
            var kinveyWrapper = new KinveyWrapper();

            kinveyWrapper.LoginAsync("ivan", "ivan", (data) =>
                {
                    var tableName = "Servers";
                    var id = "58356df6f08321f70dc31bd3";

                    kinveyWrapper
                        .RetrieveEntityAsync<CreatedGameInfo_DTO>(
                            tableName, 
                            id, 
                            (retrievedData) =>
                                {
                                    var entity = retrievedData.First().Entity;
                                    entity.HostUsername = "ivan promenen";

                                    kinveyWrapper.UpdateEntityAsync<CreatedGameInfo_DTO>(tableName, id, entity, () =>
                                        {
                                            Debug.Log("bravo");
                                        }, Debug.LogException);
                            
                                }, Debug.LogException);
                
                }, Debug.LogException);
        }
    }

}
