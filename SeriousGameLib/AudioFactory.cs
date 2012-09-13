using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace SeriousGameLib
{
    public static class AudioFactory
    {
        public  static bool IsMuted;
        private static Dictionary<string, object> _audioData;
        private static Dictionary<string, object> _audioInstances;
        public  static ContentManager Content;

        static AudioFactory()
        {
            IsMuted         = false;
            _audioData      = new Dictionary<string, object>();
            _audioInstances = new Dictionary<string, object>();
        }

        public static void AddSoundEffect(string name, string resourceName)
        {
            if (_audioData.ContainsKey(name)) return;// throw new Exception("The sound effect '" + name + "' has already been loaded. Try deleting it first, or use a unique name.");

            object data = Content.Load<object>(resourceName);
            _audioData.Add(name, data);
          
        }

        // NB.: When removing a sound effect it will automatically stop playing, too.
        public static void DeleteSoundEffect(string name)
        {
            
            if (_audioInstances.ContainsKey(name))
            {
                if (_audioData[name] is SoundEffectInstance)
                {
                    (_audioInstances[name] as SoundEffectInstance).Stop();
                    (_audioInstances[name] as SoundEffectInstance).Dispose();
                    _audioInstances.Remove(name);
                }
            }

            if (_audioData[name] is SoundEffect)
            {
                (_audioData[name] as SoundEffect).Dispose();
            }
            else if (_audioData[name] is Song)
            {
                (_audioData[name] as Song).Dispose();
                MediaPlayer.Stop();
            }

            if (_audioData.ContainsKey(name)) _audioData.Remove(name);
        }

        // Use this to only play a file once:
        public static void PlayOnce(string name, bool isLooped = false)
        {
            if (IsMuted) return;
            if (!_audioData.ContainsKey(name)) throw new Exception("The sound effect '" + name + "' cannot be played as it has not been loaded. Try loading it first, then play it.");

            if (_audioData[name] is SoundEffect)
            {
                if (!_audioInstances.ContainsKey(name) || (_audioInstances[name] as SoundEffectInstance).State == SoundState.Stopped)
                {
                    _audioInstances[name] = (_audioData[name] as SoundEffect).CreateInstance();
                    (_audioInstances[name] as SoundEffectInstance).IsLooped = isLooped;
                    (_audioInstances[name] as SoundEffectInstance).Play();
                }
            }
            else if (_audioData[name] is Song)
            {
                MediaPlayer.IsRepeating = isLooped;
                MediaPlayer.Play((Song)_audioData[name]);
            }
        }


        public static void PlayOnceChangePitch(string name, float Pitch, bool isLooped = false)
        {
            if (IsMuted) return;
            if (!_audioData.ContainsKey(name)) throw new Exception("The sound effect '" + name + "' cannot be played as it has not been loaded. Try loading it first, then play it.");

            if (_audioData[name] is SoundEffect)
            {
                if (!_audioInstances.ContainsKey(name) || (_audioInstances[name] as SoundEffectInstance).State == SoundState.Stopped)
                {
                    _audioInstances[name] = (_audioData[name] as SoundEffect).CreateInstance();
                    (_audioInstances[name] as SoundEffectInstance).IsLooped = isLooped;
                    (_audioInstances[name] as SoundEffectInstance).Play();
                    (_audioInstances[name] as SoundEffectInstance).Pitch = Pitch;
                }
            }
            else if (_audioData[name] is Song)
            {
                MediaPlayer.IsRepeating = isLooped;
                MediaPlayer.Play((Song)_audioData[name]);
            }
        }

        public static void DeleteAll()
        {
            _audioData.Clear();
            _audioInstances.Clear();
        }
    }
}
