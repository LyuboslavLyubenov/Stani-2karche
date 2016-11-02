using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

public class BasicExamClientOptionsUIController : MonoBehaviour
{
    public ServerNetworkManager NetworkManager;

    FieldUIController connectionIdField;
    FieldUIController usernameField;
    FieldUIController roleField;
    InputField reasonInputField;

    void Start()
    {
        connectionIdField = transform.Find("ConnectionIdField").GetComponent<FieldUIController>();
        usernameField = transform.Find("UsernameField").GetComponent<FieldUIController>();
        roleField = transform.Find("RoleField").GetComponent<FieldUIController>();
        reasonInputField = transform.Find("KickBanGroup/ReasonInputField").GetComponent<InputField>();

        var kickButton = transform.Find("KickBanGroup/KickButton").GetComponent<Button>();
        var banButton = transform.Find("KickBanGroup/BanButton").GetComponent<Button>();

        kickButton.onClick.AddListener(new UnityAction(OnKick));
        banButton.onClick.AddListener(new UnityAction(OnBan));
    }

    void OnKick()
    {
        var connectionId = int.Parse(connectionIdField.Value);
        var reason = reasonInputField.text;

        if (string.IsNullOrEmpty(reason))
        {
            NetworkManager.KickPlayer(connectionId);    
        }
        else
        {
            NetworkManager.KickPlayer(connectionId, reason);    
        }

        gameObject.SetActive(false);
    }

    void OnBan()
    {
        var connectionId = int.Parse(connectionIdField.Value);
        NetworkManager.BanPlayer(connectionId);

        gameObject.SetActive(false);
    }

    public void Set(ConnectedClientData clientData, BasicExamClientRole role)
    {
        var enumName = Enum.GetName(typeof(BasicExamClientRole), role);

        connectionIdField.Value = clientData.ConnectionId.ToString();
        usernameField.Value = clientData.Username;
        roleField.Value = LanguagesManager.Instance.GetValue(enumName);
    }
}