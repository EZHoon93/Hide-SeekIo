using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    float _sfxValue;
    float _bgmValue;

    
    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }

            _audioSources[(int)Define.Sound.Bgm].loop = true;
        }
    }

    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {

            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }

    public void Play(string path, Define.Sound type = Define.Sound.Effect, float volume = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, volume);
    }

	public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float volume = 1.0f)
	{
        if (audioClip == null)
            return;

        var amountValue = type == Define.Sound.Bgm ? _bgmValue : _sfxValue;
        amountValue = amountValue * volume;

        if (type == Define.Sound.Bgm)
		{
			AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];
			if (audioSource.isPlaying)
				audioSource.Stop();

			audioSource.volume = amountValue;
			audioSource.clip = audioClip;
			audioSource.Play();
            audioSource.spatialBlend = 0;
        }
        else
		{
			AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
			audioSource.volume = amountValue;
			audioSource.PlayOneShot(audioClip);
            audioSource.spatialBlend = 0;
		}
        
	}

	AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
		if (path.Contains("Sounds/") == false)
			path = $"Sounds/{path}";

		AudioClip audioClip = null;

		if (type == Define.Sound.Bgm)
		{
			audioClip = Managers.Resource.Load<AudioClip>(path);
		}
		else
		{
			if (_audioClips.TryGetValue(path, out audioClip) == false)
			{
				audioClip = Managers.Resource.Load<AudioClip>(path);
				_audioClips.Add(path, audioClip);
			}
		}

		if (audioClip == null)
			Debug.Log($"AudioClip Missing ! {path}");

		return audioClip;
    }


    public void ChangeSfxValue(float value)
    {
        _sfxValue = value;
    }

    public void ChangeBgmValue(float value)
    {
        _bgmValue = value;
        _audioSources[(int)Define.Sound.Bgm].volume = value;
    }
}
