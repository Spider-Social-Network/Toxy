﻿using System;
using System.Windows;
using System.Windows.Controls;
using Toxy.Managers;
using Toxy.ViewModels;
using Toxy.Windows;

namespace Toxy.Views
{
    /// <summary>
    /// Interaction logic for FriendControlVIew.xaml
    /// </summary>
    public partial class FriendControlView : UserControl
    {
        public FriendControlViewModel Context { get { return DataContext as FriendControlViewModel; } }

        public FriendControlView()
        {
            InitializeComponent();
        }

        private void CopyPubKey_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Clipboard.SetText(ProfileManager.Instance.Tox.GetFriendPublicKey(Context.ChatNumber).ToString());
        }

        private async void RemoveFriend_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var result = MessageBox.Show(string.Format("Are you sure you want to delete {0} from your friend list?", Context.Name), "Delete friend", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            //we don't want to keep this friend's avatar
            ProfileManager.Instance.AvatarManager.RemoveAvatar(Context.ChatNumber);

            if (!(MainWindow.Instance.ViewModel.CurrentFriendListView.RemoveObject(Context) && ProfileManager.Instance.Tox.DeleteFriend(Context.ChatNumber)))
            {
                Debugging.Write("Could not remove friend");
                return;
            }

            await ProfileManager.Instance.SaveAsync();
        }

        private void ClearScrollback_Click(object sender, RoutedEventArgs e)
        {
            Context.ConversationView.Messages.Clear();
        }

        private void OpenInWindow_Click(object sender, RoutedEventArgs e)
        {
            if (Context.ConversationView.Window != null)
            {
                if (Context.ConversationView.Window.WindowState == WindowState.Minimized)
                    Context.ConversationView.Window.WindowState = WindowState.Normal;
                else
                    Context.ConversationView.Window.Activate();

                return;
            }

            var window = new ConversationWindow(Context.ConversationView);

            Context.ConversationView.Window = window;
            MainWindow.Instance.AddChildWindow(window);

            window.Show();
        }
    }
}
