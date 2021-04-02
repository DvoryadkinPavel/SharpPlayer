using System.Transactions;
using System;
using System.Diagnostics;
using SFML.Audio;

namespace AudioEngine
{
    public class AudioPlayer
    {
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
                              
            }
            else
            {
                LoadToBuffer(fileName);
                _buffer = new SoundBuffer("buffer.ogg"); 
            }
            _sound = new Sound();
            _sound.SoundBuffer = _buffer;  
        }
        private void LoadToBuffer(string fileName)
        {
            string program = "ffmpeg";
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
                String error = p.StandardError.ReadToEnd();
                String output = p.StandardOutput.ReadToEnd() + pi.ToString();
                p.WaitForExit();     
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