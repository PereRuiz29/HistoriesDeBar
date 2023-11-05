using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DialogueAudioInfo", menuName = "ScriptableObject/DialogueAudioInfoS0", order = 1)]
public class DialogueAudioInfoSO : ScriptableObject
{
    [Tooltip("The name of the speaker")]
    public string id;

    public AudioClip[] dialogueTypingAudioClips;

    [Tooltip("Decide on the frequency of playing the dialogue sound in characters.")]
    [Range(1, 5)]
    public int frequencyLevel = 1;

    [Range(-3f, 3f)]
    public float maxPitch = 0.5f;
    [Range(-3f, 3f)]
    public float minPitch = -1f;

    [Tooltip("Stop the dialogue audio immediately before playing the next one to avoid overlap.")]
    public bool stopAudio;
}
