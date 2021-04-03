using System.Threading.Tasks;
using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using AudioEngine;

namespace SharpPlayer
{
    class MainWindow : Window
    {
        [UI] private Label _labelFileName = null;
        [UI] private Button _buttonPlay = null;
        [UI] private Button _buttonPause = null;
        [UI] private Button _buttonStop = null;
        [UI] private Button _buttonOpen = null;
        [UI] private Scrollbar _scroll = null;
        private AudioPlayer _player;
        private float _currentVolume;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetObject("MainWindow").Handle)
        {
            builder.Autoconnect(this);

            DeleteEvent += Window_DeleteEvent;
            _buttonPlay.Clicked += ButtonPlay_Clicked;
            _buttonPause.Clicked += ButtonPause_Clicked;
            _buttonStop.Clicked += ButtonStop_Clicked;
            _buttonOpen.Clicked += ButtonOpen_Clicked;
            _scroll.ValueChanged += Scrollbar_Changed;
            new Task(()=>Scrollbar_Update()).Start();
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }
        private void LoadFile(string fileName = "audio.wav")
        {
            try
            {
                _player = new AudioPlayer(fileName);                
                if(_player.MP3Loading != null) 
                {
                    _buttonOpen.Hide();
                    _buttonPause.Hide();
                    _buttonPlay.Hide();
                    _buttonStop.Hide();
                    _scroll.Hide();
                    _labelFileName.Text = "Конвертация MP3 в OGG";                    
                    _player.MP3Loading.Wait();
                    _player.OpenBuffer();
                    _buttonOpen.Show();
                    _buttonPause.Show();
                    _buttonPlay.Show();
                    _buttonStop.Show();
                    _scroll.Show();
                }
                _labelFileName.Text = fileName;
            }
            catch(Exception ex)
            {
                _labelFileName.Text = $"Не удалось загрузить файл : {ex.Message}";
            }
        }
        private void ButtonPlay_Clicked(object sender, EventArgs a)
        {
            if(_labelFileName.Text != "Выберете файл") _player.Play();
        }
        private void ButtonPause_Clicked(object sender, EventArgs a)
        {
            if(_labelFileName.Text != "Выберете файл") _player.Pause();
        }
        private void ButtonStop_Clicked(object sender, EventArgs a)
        {
            if(_labelFileName.Text != "Выберете файл") _player.Stop();
        }
        private void ButtonOpen_Clicked(object sender, EventArgs a)
        {
            Gtk.FileChooserDialog fcd = new Gtk.FileChooserDialog ("Open File", null, Gtk.FileChooserAction.Open);
			fcd.AddButton (Gtk.Stock.Cancel, Gtk.ResponseType.Cancel);
			fcd.AddButton (Gtk.Stock.Open, Gtk.ResponseType.Ok);
			fcd.DefaultResponse = Gtk.ResponseType.Ok;
			fcd.SelectMultiple = false;
			Gtk.ResponseType response = (Gtk.ResponseType) fcd.Run ();
			if (response == Gtk.ResponseType.Ok) 
            {           
                if(_player != null) 
                {
                    _player.Stop();
                    _player = null;
                }
                new Task(() => LoadFile(fcd.Filename)).Start();
                fcd.Dispose ();
            }
			

        }
        private void Scrollbar_Update()
        {
            
            while(true)
            {
                if((_player != null) && (_player.Duration != 0))
                {
                    _scroll.SetRange(0,_player.Duration);
                    _scroll.Adjustment.Value = _player.Position;                    
                }
                System.Threading.Thread.Sleep(500);
            }
        }
        private void Scrollbar_Changed(object sender, EventArgs a)
        {            
            if(_player != null)
            {
                _player.Position = (float)_scroll.Adjustment.Value;                    
            }
        }
    }
}
