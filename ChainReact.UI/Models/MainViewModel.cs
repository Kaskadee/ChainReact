using System;
using EmptyKeys.UserInterface.Mvvm;
using EmptyKeys.UserInterface.Input;

namespace ChainReact.UI.Models
{
	public class MainViewModel : ViewModelBase
	{
		private string _id;
		private string _username;

		public ICommand OkButtonCommand { get; set; }
		public ICommand CancelButtonCommand { get; set; }

		public string UserId { 
			get { return _id; } 
			set { SetProperty (ref _id, value); } 
		}

		public string Username { 
			get { return _username; } 
			set { SetProperty (ref _username, value); } 
		}

		public MainViewModel ()
		{
			OkButtonCommand = new RelayCommand (new Action<object> (OnOkCommand));
			CancelButtonCommand = new RelayCommand (new Action<object> (OnCancelCommand));
		}

		public void OnOkCommand(object obj) {

		}

		public void OnCancelCommand(object obj) {

		}
	}
}

