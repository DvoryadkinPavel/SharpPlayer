using System;
using SFML.Audio;

namespace AudioEngine
{
    public class AudioPlayer
    {
        private SoundBuffer _buffer;
        private Sound _sound;
        public AudioPlayer(string fileName)
        {
            _fileName = fileName;
            _buffer = new SoundBuffer(fileName);
            _sound = new Sound();
            _sound.SoundBuffer = _buffer;
        }
        private string _fileName;
        public string FileName
        {
            get
            {
                return _fileName;
            }
        }
        public void Play()
        {            
            if(!IsPlaying)
            {
                _sound.Play();
            }
        }
        public void Pause()
        {
            if(!IsPaused)
            {
                _sound.Pause();
            }
        }
        public void Stop()
        {
            _sound.Stop();
        }
        public bool IsPlaying
        {
            get
            {
                if(_sound.Status == SoundStatus.Playing)
                {
                    return true;
                }
                return false;
            }
        }
        public bool IsPaused
        {
            get
            {
                if(_sound.Status == SoundStatus.Paused)
                {
                    return true;
                }
                return false;
            }
        }
        public bool IsStopped
        {
            get
            {
                if(_sound.Status == SoundStatus.Stopped)
                {
                    return true;
                }
                return false;
            }
        }
    }
}