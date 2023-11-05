using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueAudio : MonoBehaviour
{

    private AudioSource m_typingAudioSource;

    [Tooltip("Characters always sound the same way or sound completely random.")]
    [SerializeField] private bool m_MakePredictable;

    [Header("ScriptableObject")]
    [SerializeField] private DialogueAudioInfoSO m_DefaultAudioInfo;
    private DialogueAudioInfoSO m_CurrentAudioInfo;

    //Audio configuration
    private AudioClip[] m_dialogueTypingAudioClips;
    private int m_FrequencyLevel;
    private float m_MaxPitch;
    private float m_MinPitch;
    private bool m_stopAudio;

    void Awake()
    {
        m_typingAudioSource = this.gameObject.AddComponent<AudioSource>();

        m_CurrentAudioInfo = m_DefaultAudioInfo;

        //debug
    }

    private void Start()
    {
        SetAudioInfo("patata");

    }

    //Set all the audio configurations to the chosen charater audio info
    public void SetAudioInfo(string id)
    {
        m_dialogueTypingAudioClips = m_CurrentAudioInfo.dialogueTypingAudioClips;
        m_FrequencyLevel = m_CurrentAudioInfo.frequencyLevel;
        m_MaxPitch = m_CurrentAudioInfo.maxPitch;
        m_MinPitch = m_CurrentAudioInfo.minPitch;
        m_stopAudio = m_CurrentAudioInfo.stopAudio;
    }

    public void PlayDialogueSound(int characterDisplayCounter, char currentCharacter)
    {
        Debug.Log("characterDisplayCounter: " + characterDisplayCounter);
        Debug.Log("currentCharacter: " + currentCharacter);

        if (characterDisplayCounter % m_FrequencyLevel == 0)
        {
            if (m_stopAudio)
                m_typingAudioSource.Stop();

            if (m_MakePredictable)
            {
                //Characters always sound the same way or sound completely random.
                int hashCode = currentCharacter.GetHashCode();

                //sound clip
                int clipIndex = hashCode % m_dialogueTypingAudioClips.Length;

                //chose pitch
                int minPitch = (int)(m_MinPitch * 1000);
                int maxPitch = (int)(m_MaxPitch * 1000);
                int pitchRange = maxPitch - minPitch;
                if (pitchRange == 0)
                    m_typingAudioSource.pitch = minPitch;
                else
                {
                    int hashPitchInt = (hashCode % pitchRange) + minPitch;
                    float hasPitch = hashPitchInt / 1000f;
                    m_typingAudioSource.pitch = hasPitch;
                }

                //play audio clip
                m_typingAudioSource.PlayOneShot(m_dialogueTypingAudioClips[clipIndex]);

            }
            else
            {
                //sound clip
                int clipIndex = UnityEngine.Random.Range(0, m_dialogueTypingAudioClips.Length);
                //pitch
                m_typingAudioSource.pitch = UnityEngine.Random.Range(m_MinPitch, m_MaxPitch);
                //play audio clip
                m_typingAudioSource.PlayOneShot(m_dialogueTypingAudioClips[clipIndex]);
            }
        }
    }
}
