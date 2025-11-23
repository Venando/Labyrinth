using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterSaver : MonoBehaviour, ISavable
{
    [SerializeField] private CharacterController characterController;

    public string SaveKey => gameObject.name;

    public string CaptureState()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        var characterState = new CharacterState { 
            position = characterController.transform.position,
            rotation = characterController.transform.eulerAngles
        };

        return JsonUtility.ToJson(characterState);
    }

    public void RestoreState(string json)
    {
        if (string.IsNullOrEmpty(json)) return;

        var characterState = JsonUtility.FromJson<CharacterState>(json);
        
        if (characterState == null) return;

        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        characterController.transform.position = characterState.position;
        characterController.transform.eulerAngles = characterState.rotation;
    }

    [Serializable]
    public class CharacterState
    {
        public Vector3 position;
        public Vector3 rotation;
    }
}