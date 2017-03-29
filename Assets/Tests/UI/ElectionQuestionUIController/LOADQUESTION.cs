using System.Collections;
using System.Collections.Generic;

using Assets.Scripts.Interfaces.Controllers;

using Interfaces;

using UnityEngine;

using Utils.Unity;

using Zenject.Source.Usage;

public class LOADQUESTION : ExtendedMonoBehaviour
{

    [Inject]
    private IElectionQuestionUIController questionUiController;

    [Inject]
    private ISimpleQuestion question;

    // Use this for initialization
    void Start()
    {
        this.CoroutineUtils.WaitForSeconds(0.5f,
            () =>
                {
                    this.questionUiController.LoadQuestion(this.question);
                });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
