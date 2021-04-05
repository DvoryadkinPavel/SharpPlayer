using System;
using System.Diagnostics;
using SFML.Audio;
using System.Threading.Tasks;
using SFML.System;

namespace AudioEngine
{
    public class AudioPlayer
    {
        /// <summary>
        /// Процесс конвертации MP3
        /// </summary>
        public Task MP3Loading;
        private string _directory;
        private SoundBuffer _buffer;
        private Sound _sound;
        /// <summary>
        /// Аудио проигрыватель
        /// </summary>
        /// <param name="fileName">имя файла</param>
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
        /// <summary>
        /// Открытие временного файла
        /// </summary>
        public void OpenBuffer()
        {
            _buffer = new SoundBuffer("buffer.wav");
            _sound = new Sound();
            _sound.SoundBuffer = _buffer;   
        }
        private void LoadToBuffer(string fileName)
        {
            var pi = new ProcessStartInfo("/bin/bash")
            {
                Arguments = $"-y -i \"{fileName}\" buffer.wav",
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
        /// <summary>
        /// Длительность в секундах
        /// </summary>
        public float Duration
        {
            get
            {
                if(_buffer != null) return _buffer.Duration.AsSeconds();
                else return 0;
            }
        }
        /// <summary>
        /// Текущая позиция в секундах
        /// </summary>
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
        /// <summary>
        /// Начать воспроизведение
        /// </summary>
        public void Play()
        {            
            if(!IsPlaying)
            {
                _sound.Play();
            }
        }
        /// <summary>
        /// Поставить на паузу
        /// </summary>
        public void Pause()
        {
            if(!IsPaused)
            {
                _sound.Pause();
            }
        }
        /// <summary>
        /// Остановить воспроизведение
        /// </summary>
        public void Stop()
        {
            _sound.Stop();
        }
        /// <summary>
        /// Воспроизводится ли
        /// </summary>
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
        /// <summary>
        /// На паузе ли
        /// </summary>
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
        /// <summary>
        /// Остановлено ли
        /// </summary>
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