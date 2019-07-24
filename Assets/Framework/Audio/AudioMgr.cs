using Assets.Framework.Res;
using Assets.Framework.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Framework.Audio
{
    public class AudioMgr : Singleton<AudioMgr>
    {
        private AudioListener mAudioListener;
        private AudioSource mBGMSource = null;
        private AudioSource mEffectSource = null;
        public GameObject Root;
        public bool isPlayEffectMusic = true;
        public bool isPlayBGMusic = true;

        public override void Init()
        {
            Root = GameRoot.Instance.gameObject;
            mAudioListener = Root.GetComponent<AudioListener>();
            mBGMSource = Root.GetComponents<AudioSource>()[0];
            mEffectSource = Root.GetComponents<AudioSource>()[1];
            CheckAudioListener();
            CheckEffectSource();
            CheckBGMSource();
            if (PlayerPrefs.GetInt(StringMgr.isEffectOff) == 1)
            {
                CloseOrOpenEffectMusic();
            }
            if (PlayerPrefs.GetInt(StringMgr.isMusicOff) == 1)
            {
                CloseOrOpenBGMusic();
            }
        }

        private void CheckEffectSource()
        {
            if (mEffectSource == null)
                mEffectSource = Root.AddComponent<AudioSource>();
        }
        private void CheckBGMSource()
        {
            if (mBGMSource == null)
                mBGMSource = Root.AddComponent<AudioSource>();
        }
        private void CheckAudioListener()
        {
            if (mAudioListener == null)
                mAudioListener = Root.AddComponent<AudioListener>();
        }

        public void PlayBGM(string bgmName, bool loop = true)
        {
            AudioClip bgm = ResMgr.Instance.GetRes<AudioClip>(bgmName);
            mBGMSource.clip = bgm;
            mBGMSource.loop = loop;
            mBGMSource.Play();
            if (isPlayBGMusic)
            {
                BGMOn();
            }
            else
            {
                BGMOff();
            }

        }
        public void PlayEffectMusic(string effectName)
        {
            if (isPlayEffectMusic)
            {
                AudioClip effect = ResMgr.Instance.GetRes<AudioClip>(effectName);
                mEffectSource.PlayOneShot(effect);
            }
        }

        #region BGM operator
        private void BGMPause()
        {
            mBGMSource.Pause();
        }

        private void BGMStop()
        {
            mBGMSource.Stop();
        }

        private void BGMUnPause()
        {
            mBGMSource.UnPause();
        }

        private void BGMOn()
        {
            mBGMSource.UnPause();
            mBGMSource.mute = false;
        }

        private void BGMOff()
        {
            mBGMSource.Pause();
            mBGMSource.mute = true;
        }
        #endregion

        public void CloseOrOpenBGMusic()
        {
            isPlayBGMusic = !isPlayBGMusic;
            SetMusicPrefs(isPlayBGMusic);
            if (isPlayBGMusic)
            {
                BGMOn();
            }
            else
            {
                BGMOff();
            }
        }
        public void CloseOrOpenEffectMusic()
        {
            isPlayEffectMusic = !isPlayEffectMusic;
            SetEffectPrefs(isPlayEffectMusic);
        }

        private void SetMusicPrefs(bool isMusicPlay)
        {
            if (isMusicPlay)
            {
                PlayerPrefs.SetInt(StringMgr.isMusicOff, 0);
            }
            else
            {
                PlayerPrefs.SetInt(StringMgr.isMusicOff, 1);
            }
        }
        private void SetEffectPrefs(bool isEffectPlay)
        {
            if (isEffectPlay)
            {
                PlayerPrefs.SetInt(StringMgr.isEffectOff, 0);
            }
            else
            {
                PlayerPrefs.SetInt(StringMgr.isEffectOff, 1);
            }
        }



    }
}
