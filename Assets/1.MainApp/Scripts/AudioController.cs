using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace MainApp
{
    public class AudioController : MonoBehaviour
    {
        public static AudioController Instance { get; private set; }

        public Action<float> OnChangeVolumeVoice;
        public Action<float> OnChangeVolumeBg;

        [SerializeField]
        private List<AudioSource> audioSourceVoice;
        [SerializeField]
        private List<AudioSource> audioSourceSound;
        [SerializeField]
        private List<AudioSource> audioSourceBg;
		[SerializeField] List<AudioSource> audioSourceBgApp;

        private float _volumeVoice;
        private float _volumeSound;
        private float _volumeBgGame;
		private float _volumeBgApp;

        private float _maxVolumeSound = 0.5f;
        private float _maxVolumeBg = 0.3f;

		float defaultVolume;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        private void Start()
        {
            OnChangeVolumeVoice = VolumeVoice;
            OnChangeVolumeBg = (volume) =>
            {
                VolumeBG(volume);
                VolumeSound(volume);
            };
            InitVolume();
        }

		public float GetDefaultVolume()
		{
			if (PlayerPrefs.HasKey(StaticConfig.VOLUME_KEY))
			{
				return defaultVolume = PlayerPrefs.GetFloat(StaticConfig.VOLUME_KEY);
			}
			else
			{
				PlayerPrefs.SetFloat(StaticConfig.VOLUME_KEY, 100);
				return defaultVolume = 100;
			}
		}

		public void SaveDefaultVolume(float defaultVolume)
		{
			this.defaultVolume = defaultVolume;

			PlayerPrefs.SetFloat(StaticConfig.VOLUME_KEY, defaultVolume);
			PlayerPrefs.Save();
		}

        private void InitVolume()
        {
            float volumeVoice = GetDefaultVolume();//get from data
            float volumeBg = GetDefaultVolume();//get from data

            VolumeVoice(volumeVoice);
            VolumeBG(volumeBg);
            VolumeSound(volumeBg);
        }


        public float GetVolume(AudioType type)
        {
            float volume = 0;
            switch (type)
            {
                case AudioType.VOICE:
                    volume = _volumeVoice;
                    break;
                case AudioType.SOUND:
                    volume = _volumeSound;
                    break;
                case AudioType.BG:
                    volume = _volumeBgGame;
					break;
				case AudioType.BgApp:
					volume = _volumeBgApp;
                    break;
            }
            return volume;
        }

		public float GetDefaultVolumeToSlider()
		{
			return (float)defaultVolume / 100;
		}

        public float GetValueSliderVoice()
        {
            return ConvertVolumeToSlider(_volumeVoice, 1);
        }

        public float GetValueSliderBg()
        {
            return ConvertVolumeToSlider(_volumeBgGame, _maxVolumeBg);
        }

        private void SetVolumeDefault()
        {
			_volumeBgApp = _maxVolumeBg;
            _volumeBgGame = _maxVolumeBg;
            _volumeSound = _maxVolumeSound;
            _volumeVoice = 1;
            VolumeVoice(100);
            VolumeSound(100);
            VolumeBG(100);
        }

        private void VolumeVoice(float value)
        {
            float volume = ConvertToVolume(value, 1f);
            for (int i = 0; i < audioSourceVoice.Count; i++)
            {
                audioSourceVoice[i].volume = volume;
            }
            _volumeVoice = volume;
        }
        private void VolumeSound(float value)
        {
            float volume = ConvertToVolume(value, _maxVolumeSound);
            for (int i = 0; i < audioSourceSound.Count; i++)
            {
                audioSourceSound[i].volume = volume;
            }
            _volumeSound = volume;
        }
        private void VolumeBG(float value)
        {
            if (audioSourceBg == null)
            {
                _volumeBgGame = ConvertToVolume(value, _maxVolumeBg);
                return;
            }
            float volume = ConvertToVolume(value, _maxVolumeBg);
            for (int i = 0; i < audioSourceBg.Count; i++)
            {
                audioSourceBg[i].volume = volume;
            }
            _volumeBgGame = volume;

			if (audioSourceBgApp == null)
			{
				_volumeBgApp = ConvertToVolume(value, _maxVolumeBg);
				return;
			}
			for (int i = 0; i < audioSourceBgApp.Count; i++)
			{
				audioSourceBgApp[i].volume = volume;
			}
			_volumeBgApp = volume;
        }

		public void SetVolumeBGOnPlayVoice()
		{
			VolumeVoice(defaultVolume);
			VolumeBG(defaultVolume / 5);
		}

		public void SetVolumBGOnPlayVoiceDone()
		{
			VolumeVoice(defaultVolume);
			VolumeBG(defaultVolume);
		}

        private float ConvertToVolume(float value, float maxVolume)
        {
            return maxVolume * value / 100f;
        }
        private float ConvertVolumeToSlider(float volume, float maxVolume)
        {
            return volume * 100f / maxVolume;
        }


        public void StopAudio(AudioClip audioClip)
        {
            for (int i = 0; i < audioSourceSound.Count; i++)
            {
                if (audioSourceSound[i].isPlaying && ReferenceEquals(audioSourceSound[i].clip, audioClip))
                {
                    audioSourceSound[i].Stop();
                }
            }

            for (int i = 0; i < audioSourceVoice.Count; i++)
            {
                if (audioSourceVoice[i].isPlaying && ReferenceEquals(audioSourceVoice[i].clip, audioClip))
                {
                    audioSourceVoice[i].Stop();
                }
            }

            for (int i = 0; i < audioSourceBg.Count; i++)
            {
                if (audioSourceBg[i].isPlaying && ReferenceEquals(audioSourceBg[i].clip, audioClip))
                {
                    audioSourceBg[i].Stop();
                }
            }
        }
        public void StopAllAudio()
        {
            for (int i = 0; i < audioSourceSound.Count; i++)
            {
                if (audioSourceSound[i].isPlaying)
                {
                    audioSourceSound[i].Stop();
                }
            }

            for (int i = 0; i < audioSourceVoice.Count; i++)
            {
                if (audioSourceVoice[i].isPlaying)
                {
                    audioSourceVoice[i].Stop();
                }
            }

			SetVolumBGOnPlayVoiceDone();
		}

        public AudioSource GetAudioSource(AudioType audioNewType)
        {
            switch (audioNewType)
            {
                case AudioType.VOICE:
                    for (int i = 0; i < audioSourceVoice.Count; i++)
                    {
                        if (!audioSourceVoice[i].isPlaying)
                        {
                            return audioSourceVoice[i];
                        }
                    }
                    return audioSourceVoice[audioSourceVoice.Count - 1];
                case AudioType.SOUND:
                    for (int i = 0; i < audioSourceSound.Count; i++)
                    {
                        if (!audioSourceSound[i].isPlaying)
                        {
                            return audioSourceSound[i];
                        }
                    }
                    return audioSourceSound[audioSourceSound.Count - 1];
                case AudioType.BG:
                    for (int i = 0; i < audioSourceBg.Count; i++)
                    {
                        if (!audioSourceBg[i].isPlaying)
                        {
                            return audioSourceBg[i];
                        }
                    }
                    return audioSourceBg[audioSourceBg.Count - 1];
				case AudioType.BgApp:
					for (int i = 0; i < audioSourceBgApp.Count; i++)
					{
						if (!audioSourceBgApp[i].isPlaying)
							return audioSourceBgApp[i];
					}
					return audioSourceBgApp[audioSourceBgApp.Count - 1];
                default:
                    return null;
            }
        }
        public void PlaySoundBgGame(AudioClip audioClip, bool stopOther = true, bool loop = true)
        {
			PauseAudioBgApp();
            if (stopOther)
                StopAudioBgGame(false);
            AudioSource audiosource = GetAudioSource(AudioType.BG);
            audiosource.clip = audioClip;
            audiosource.loop = loop;
            audiosource.Play();
        }

        public void StopAudioBgGame(bool stopBgApp = true)
        {
            for (int i = 0; i < audioSourceBg.Count; i++)
            {
                if (audioSourceBg[i].isPlaying)
                {
                    audioSourceBg[i].Stop();
                }
            }

			if (stopBgApp) UnPauseAudioBgApp();
        }

        public void PauseAudioBgGame()
		{
			for (int i = 0; i < audioSourceBg.Count; i++)
			{
				if (audioSourceBg[i].isPlaying)
				{
					var temp = i;
					audioSourceBg[temp].Pause();
				}
			}
		}

		public void UnPauseAudioBgGame()
		{
			for (int i = 0; i < audioSourceBgApp.Count; i++)
			{
				if (!audioSourceBg[i].isPlaying && audioSourceBg[i].clip != null)
					audioSourceBg[i].UnPause();
			}
		}

		public void PlaySoundBgApp(AudioClip audioClip, bool stopOther = true, bool loop = true)
		{
			
			var audioSource = GetAudioSource(AudioType.BgApp);

			if (audioSource.isPlaying && audioSource.clip == audioClip) return;

			if (stopOther)
				StopAudioBgApp();

			audioSource.clip = audioClip;
			audioSource.loop = loop;
			audioSource.Play();
		}

		public void StopAudioBgApp()
		{
			for (int i = 0; i < audioSourceBgApp.Count; i++)
			{
				if (audioSourceBgApp[i].isPlaying)
				{
					audioSourceBgApp[i].Stop();
				}
			}
		}

		public void PauseAudioBgApp()
		{
			for (int i = 0; i < audioSourceBgApp.Count; i++)
			{
				if (audioSourceBgApp[i].isPlaying)
				{
					var temp = i;
					audioSourceBgApp[temp].Pause();
				}
			}
		}

		public void UnPauseAudioBgApp()
		{

			for (int i = 0; i < audioSourceBgApp.Count; i++)
			{
				if (!audioSourceBgApp[i].isPlaying && audioSourceBgApp[i].clip != null)
					audioSourceBgApp[i].UnPause();
			}
		}
    }
    public enum AudioType
    {
        VOICE,
        SOUND,
        BG,
		BgApp
    }
}


