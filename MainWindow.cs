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
        private AudioPlayer _player;


        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetObject("MainWindow").Handle)
        {
            builder.Autoconnect(this);

            DeleteEvent += Window_DeleteEvent;
            _buttonPlay.Clicked += ButtonPlay_Clicked;
            _buttonPause.Clicked += ButtonPause_Clicked;
            _buttonStop.Clicked += ButtonStop_Clicked;
            _buttonOpen.Clicked += ButtonOpen_Clicked;
            _labelFileName.Text = "Выберете файл";
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }
        private void LoadFile(string fileName = "audio.wav")
        {
            _player = new AudioPlayer(fileName);
            _labelFileName.Text = fileName;
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
                if(_labelFileName.Text != "Выберете файл") _player.Stop();
				LoadFile(fcd.Filename);
			fcd.Dispose ();

        }
    }
}
