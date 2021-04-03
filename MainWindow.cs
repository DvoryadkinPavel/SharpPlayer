using System.Linq;
using System.Threading.Tasks;
using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;
using AudioEngine;
using System.Collections.Generic;

namespace SharpPlayer
{
    class MainWindow : Window
    {
        [UI] private Fixed _fixed = null;
        [UI] private Label _labelFileName = null;
        [UI] private Label _timePosition = null;
        [UI] private Button _buttonPlay = null;
        [UI] private Button _buttonPause = null;
        [UI] private Button _buttonStop = null;
        [UI] private Button _buttonOpen = null;
        [UI] private Scrollbar _scroll = null;
        [UI] private TreeView _treeView = null;
        [UI] private TreeStore treestore = null;
        private AudioPlayer _player;
        private List<string> _playlist;
        private int _currentIndex;
        private bool _isPlaying;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetObject("MainWindow").Handle)
        {
            builder.Autoconnect(this);

            DeleteEvent += Window_DeleteEvent;
            _buttonPlay.Clicked += ButtonPlay_Clicked;
            _buttonPause.Clicked += ButtonPause_Clicked;
            _buttonStop.Clicked += ButtonStop_Clicked;
            _buttonOpen.Clicked += ButtonOpen_Clicked;
            _scroll.ChangeValue += Scrollbar_Changed;
            new Task(()=>Scrollbar_Update()).Start();   
            _treeView = new TreeView();
            _treeView.RowActivated += OnRowActivated;
            TreeViewColumn listOfFiles = new TreeViewColumn();
            listOfFiles.Title = "Список воспроизведения";
            treestore = new TreeStore(typeof(string));  
            CellRendererText cell = new CellRendererText();
            listOfFiles.PackStart(cell, true);
            listOfFiles.AddAttribute(cell, "text", 0);
            treestore = new TreeStore(typeof(string));  
            _treeView.AppendColumn(listOfFiles);
            _treeView.Model = treestore;
            _treeView.MarginLeft = 10;
            _treeView.MarginTop = 110;
            _fixed.Add(_treeView);
            _fixed.ShowAll();
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
                _scroll.SetRange(0,_player.Duration);
                _player.Play();
                _isPlaying = true;
            }
            catch(Exception ex)
            {
                _labelFileName.Text = $"Не удалось загрузить файл : {ex.Message}";
            }
        }
        private void ButtonPlay_Clicked(object sender, EventArgs a)
        {
            if(_player != null) _player.Play();
            _isPlaying = true;
        }
        private void ButtonPause_Clicked(object sender, EventArgs a)
        {
            if(_player != null) _player.Pause();
            _isPlaying = true;
        }
        private void ButtonStop_Clicked(object sender, EventArgs a)
        {
            if(_player != null) _player.Stop();
            _isPlaying = false;
        }
        private void ButtonOpen_Clicked(object sender, EventArgs a)
        {
            Gtk.FileChooserDialog fcd = new Gtk.FileChooserDialog ("Выберете файл", null, Gtk.FileChooserAction.Open);
			fcd.AddButton (Gtk.Stock.Cancel, Gtk.ResponseType.Cancel);
			fcd.AddButton (Gtk.Stock.Open, Gtk.ResponseType.Ok);
			fcd.DefaultResponse = Gtk.ResponseType.Ok;
			fcd.SelectMultiple = true;
			Gtk.ResponseType response = (Gtk.ResponseType) fcd.Run ();
			if (response == Gtk.ResponseType.Ok) 
            {     
                _playlist = fcd.Filenames.ToList();
                if(_player != null) 
                {
                    _player.Stop();
                    _player = null;
                }
                new Task(() => LoadFile(_playlist.First())).Start();
                _currentIndex = 0;
                fcd.Dispose ();
                treestore.Clear();
                foreach(var item in _playlist)
                {
                    treestore.AppendValues(item); 
                }
            }
			

        }
        private void Scrollbar_Update()
        {
            
            while(true)
            {
                if((_player != null) && (_player.Duration != 0))
                {     
                    if(_isPlaying && _player.IsStopped && (_currentIndex < (_playlist.Count-1)))
                    {
                        _currentIndex++;
                        new Task(() => LoadFile(_playlist.ElementAt(_currentIndex))).Start();
                    }        
                    var position = _player.Position; 
                    _scroll.Adjustment.Value = position;                    
                    _timePosition.Text = $"{Minutes(position).ToString("00")}:{Seconds(position).ToString("00")}";                    
                }                
                System.Threading.Thread.Sleep(1000);
            }
        }
        private void Scrollbar_Changed(object sender, EventArgs a)
        {            
            if(_player != null)
            {
                _player.Position = (float)_scroll.Adjustment.Value;                    
            }
        }
        private int Minutes(double input)
        {
            return (int)(input/60);
        }
        private int Seconds(double input)
        {
            return (int)(input - (Minutes(input)*60));
        }
        void OnRowActivated (object sender, RowActivatedArgs args) 
        {
    
            TreeView view = (TreeView) sender;   
            _currentIndex = Convert.ToInt32(args.Args[0].ToString());
            if(_player != null)
            {
                _player.Stop();
                _player = null;
            }
            new Task(() => LoadFile(_playlist.ElementAt(_currentIndex))).Start();      
        }   
    }
}
