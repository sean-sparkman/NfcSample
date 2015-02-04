using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Proximity;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace NfcSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        long subscribedMessageId;
        long detectWriteableTagId;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!string.IsNullOrEmpty((string)e.Parameter))
            {
                // parsing the query string, not pretty stuff
                var parameters = ((string)e.Parameter).Split('&');
                var profileId = parameters.FirstOrDefault(p => p.StartsWith("ProfileId"));
                var id = int.Parse(profileId.Split('=')[1]);

                var profile = ProfileService.Get(id);
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    ProfileName.Text = profile.Name;

                    var image = new BitmapImage();
                    image.UriSource = new Uri(profile.ImageUri);
                    ProfilePicture.Source = image;
                });
            }

            ChooseProfile.ItemsSource = ProfileService.GetProfiles();

            var device = ProximityDevice.GetDefault();
            if (device != null)
            {
                 // Protocol:  Select which type of NFC Message you wish to receive
                // Handler:  Event handler that is trigger when an NFC Tag is detected of the protocol specified
                subscribedMessageId = device.SubscribeForMessage("Windows.SampleMessage", MessageReceivedHandler);

                // Listen for a writable tag.  This protocol is only available for listening.
                detectWriteableTagId = device.SubscribeForMessage("WriteableTag", WritableTagDetectedHandler);
            }
            else
            {
                var dialog = new MessageDialog("Sorry, your device does not have NFC, or it is disabled.");
                await dialog.ShowAsync();
            }
        }

        private async void MessageReceivedHandler(ProximityDevice device, ProximityMessage message)
        {
            device.StopSubscribingForMessage(subscribedMessageId);

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                // Data:  byte array stored in the NFC tag
                var dialog = new MessageDialog("NFC Data: " + message.DataAsString);
                await dialog.ShowAsync();
            });
        }

        private async void WritableTagDetectedHandler(ProximityDevice device, ProximityMessage message)
        {
            device.StopSubscribingForMessage(detectWriteableTagId);

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                WriteTag.IsEnabled = true;
            });
        }

        private async void WriteTag_Click(object sender, RoutedEventArgs e)
        {
            var profile = ChooseProfile.SelectedItem as Profile;
            if (profile != null)
            {
                var device = ProximityDevice.GetDefault();

                var url = System.Text.Encoding.Unicode.GetBytes("Page=Profile&ProfileId=" + profile.Id + "\tWindowsPhone\t{81238de8-a99f-453b-9ed4-b83f96fbc5c0}");

                // Windows protocol is used to write a custom type to write binary data.  It encapsulates NDEF manipulations.  
                device.PublishBinaryMessage("LaunchApp:WriteTag", url.AsBuffer(), MessageWritenHandler);
            }
            else
            {
                var dialog = new MessageDialog("Please choose a profile.");
                await dialog.ShowAsync();
            }
        }

        private async void MessageWritenHandler(ProximityDevice device, long messageId)
        {
            
        }
    }
}
