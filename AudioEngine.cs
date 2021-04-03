using System;
using System.Diagnostics;
using SFML.Audio;
using System.Threading.Tasks;
using SFML.System;

namespace AudioEngine
{
    public class AudioPlayer
    {
        public Task MP3Loading;
        private string _directory;
        private SoundBuffer _buffer;
        private Sound _sound;
        public AudioPlayer(string fileName)
        {
            _fileName = fileName;
            _directory = Environment.CurrentDirectory;
            if(!fileName.Contains(".mp3"))
            {                
                _buffer = new SoundBuffer(fileName);
                _sound = new Sound();
                _sound.SoundBuffer = _buffer;                
            }
            else
            {
                LoadToBuffer(fileName);
            }
            
        }
        public void OpenBuffer()
        {
            _buffer = new SoundBuffer("buffer.ogg");
            _sound = new Sound();
            _sound.SoundBuffer = _buffer;   
        }
        private void LoadToBuffer(string fileName)
        {
            var pi = new ProcessStartInfo("/bin/bash")
            {
                Arguments = $"-y -i \"{fileName}\" buffer.ogg",
                WorkingDirectory = _directory,
                FileName = "ffmpeg",
                Verb = "OPEN",
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            Process p = new Process();
            p.StartInfo = pi;
            p.Start();
            MP3Loading = p.WaitForExitAsync();     
        }
        private string _fileName;
        public float Duration
        {
            get
            {
                if(_buffer != null) return _buffer.Duration.AsSeconds();
                else return 0;
            }
        }
        public float Position
        {
            get
            {
                return _sound.PlayingOffset.AsSeconds();
            }
            set
            {
                _sound.PlayingOffset = Time.FromSeconds(value);
            }
        }
        public float Volume
        {
            get
            {
                return _sound.Volume;
            }
            set
            {
                _sound.Volume = value;
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