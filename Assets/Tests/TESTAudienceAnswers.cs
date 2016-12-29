using System.Collections.Generic;

using UnityEngine;

namespace Assets.Tests
{

    using Assets.Scripts.Controllers;

    public class TESTAudienceAnswers : MonoBehaviour
    {
        public AudienceAnswerUIController AudienceAnswer;

        private void Start()
        {
            var sampleVotes = new Dictionary<string, int>();
            sampleVotes.Add("1", 1);
            sampleVotes.Add("2", 2);
            sampleVotes.Add("3", 3);
            sampleVotes.Add("4", 4);
            sampleVotes.Add("5", 5);
            sampleVotes.Add("6", 6);

            this.AudienceAnswer.SetVoteCount(sampleVotes, true);
        }
	
    }

}
