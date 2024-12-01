using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] TextAsset _versionFile;
    [SerializeField] GameObject _levelSelectMenu;
    [SerializeField] TMP_Text _versionText;

    // Start is called before the first frame update
    void Start()
    {
        _versionText.text = $"V {_versionFile.text}";
        Instantiate(_levelSelectMenu);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
