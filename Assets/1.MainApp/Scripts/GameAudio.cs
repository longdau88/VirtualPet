using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MainApp;
namespace Game
{
    public class GameAudio : MonoBehaviour
    {
        public static GameAudio Instance { get; private set; }
        [Header("Pool Audio")]
        public List<AudioClip> audioWrongSoundFx;
        public List<AudioClip> audioCorrectSoundFx;
        [Header("Sound Fx")]
        [SerializeField] AudioClip soundClick;

        List<AudioClip> audioWrongVoiceOver;
        List<AudioClip> audioCorrectVoiceOver;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void PlaySoundClickButton(System.Action onDone = null, bool stopOther = false)
        {
            PlayClip(SourceType.SOUND_FX, soundClick, stopOther, onDone);
        }

        private AudioClip GetClip(ClipType clipType)
        {
            int ran = 0;
            switch (clipType)
            {
                case ClipType.CORRECT_SOUND_FX:
                    if (audioCorrectSoundFx.Count <= 0) return null;
                    ran = Random.Range(10, 100) % audioCorrectSoundFx.Count;
                    return audioCorrectSoundFx[ran];

                case ClipType.CORRECT_VOICE_OVER:

					//SetVoiceOverCorrectBySubject();

					if (audioCorrectVoiceOver.Count <= 0) return null;
                    ran = Random.Range(10, 100) % audioCorrectVoiceOver.Count;
                    return audioCorrectVoiceOver[ran];

                case ClipType.WRONG_SOUND_FX:
                    if (audioWrongSoundFx.Count <= 0) return null;
                    ran = Random.Range(10, 100) % audioWrongSoundFx.Count;
                    return audioWrongSoundFx[ran];

                case ClipType.WRONG_VOICE_OVER:

					//SetVoiceOverWrongBySubject();

                    if (audioWrongVoiceOver.Count <= 0) return null;
                    ran = Random.Range(10, 100) % audioWrongVoiceOver.Count;
                    return audioWrongVoiceOver[ran];
                default:
                    return null;
            }
        }

        public void PlayAudio(SourceType sourceType, ClipType clipType, bool stopOther = true, System.Action callBack = null, bool loop = false)
        {
            AudioSource audioSource = null;
            if (stopOther)
            {
                AudioController.Instance.StopAllAudio();
            }
            switch (sourceType)
            {
                case SourceType.SOUND_FX:
                    audioSource = AudioController.Instance.GetAudioSource(MainApp.AudioType.SOUND);
                    break;
                case SourceType.VOICE_OVER:
                    audioSource = AudioController.Instance.GetAudioSource(MainApp.AudioType.VOICE);
                    break;
            }
            
            audioSource.clip = GetClip(clipType);
            if (audioSource.clip == null)
            {
                callBack?.Invoke();
                return;
            }
            audioSource.Play();
            audioSource.loop = loop;
            if (callBack != null && !loop)
            {
                TweenControl.GetInstance().DelayCall(this.transform, audioSource.clip.length, () =>
                {
                    callBack?.Invoke();
                });
            }
        }

        public void PlayClip(SourceType sourceType, AudioClip audioClip, bool stopOther = true, System.Action callBack = null, bool loop = false)
        {
            if (audioClip == null)
            {
                callBack?.Invoke();
                return;
            }
				
			AudioSource audioSource = null;
            if (stopOther)
            {
                AudioController.Instance.StopAllAudio();
            }
            switch (sourceType)
            {
                case SourceType.SOUND_FX:
                    audioSource = AudioController.Instance.GetAudioSource(MainApp.AudioType.SOUND);
                    break;
                case SourceType.VOICE_OVER:
					AudioController.Instance.SetVolumeBGOnPlayVoice();
					audioSource = AudioController.Instance.GetAudioSource(MainApp.AudioType.VOICE);
                    break;
            }
            audioSource.clip = audioClip;
			try
			{
				audioSource.Play();
			}
			catch (System.Exception e)
			{
				Debug.Log("Can not play clip, because: " + e + "    " + audioClip.name + "  " + audioClip.loadType);
				throw;
			}
            
            audioSource.loop = loop;
            if (callBack != null && !loop)
            {
                TweenControl.GetInstance().DelayCallNew(this.transform, audioSource.clip.length, () =>
                {
					if (sourceType == SourceType.VOICE_OVER)
						AudioController.Instance.SetVolumBGOnPlayVoiceDone();
					callBack?.Invoke();
				});
            }
			else if (sourceType == SourceType.VOICE_OVER)
			{
				TweenControl.GetInstance().DelayCallNew(this.transform, audioSource.clip.length, () =>
				{
					AudioController.Instance.SetVolumBGOnPlayVoiceDone();
				});
			}
        }
    }

    public enum ClipType
    {
        CORRECT_SOUND_FX,
        CORRECT_VOICE_OVER,
        WRONG_SOUND_FX,
        WRONG_VOICE_OVER,
    }

    public enum SourceType
    {
        SOUND_FX,
        VOICE_OVER
    }

    public enum AudioName
    {
        //SOUND
        SOUND_BUTTON_CLICK,
        SOUND_SHOW_RESULT,

        //VOICE
        VOICE_FILL_IN_THE_BLANK,

    }
}

