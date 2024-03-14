using MainApp.VirtualFriend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] Button btnClose;


    // Start is called before the first frame update
    void Start()
    {
        btnClose.onClick.RemoveAllListeners();
        btnClose.onClick.AddListener(OnClickClose);
    }

    public void InitResultGame()
    {

    }

    private void OnClickClose()
    {
        VirtualPetManager.Instance.HidePanelResultDialog();
    }
}
