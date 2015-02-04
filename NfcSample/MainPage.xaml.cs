using System;
using System.Collections.Generic;
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
            if (e.Parameter != null)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    Parameters.Text = string.Format("Parameters:\n {0}", e.Parameter);
                });
            }

            var device = ProximityDevice.GetDefault();
            if (device != null)
            {
                var dialog = new MessageDialog("NFC Present");
                await dialog.ShowAsync();
                
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
                
                // Proximity messages contains the capacity of the NFC tag
                var capacity = System.BitConverter.ToInt32(message.Data.ToArray(), 0);
                var dialog = new MessageDialog("NFC Tag Capacity: " + capacity);
                await dialog.ShowAsync();
            });
        }

        private void WriteTag_Click(object sender, RoutedEventArgs e)
        {
            var device = ProximityDevice.GetDefault();

            var url = System.Text.Encoding.Unicode.GetBytes("Page=Profile&ProfileId=1\tWindowsPhone\t{81238de8-a99f-453b-9ed4-b83f96fbc5c0}");

            // Windows protocol is used to write a custom type to write binary data.  It encapsulates NDEF manipulations.  
            device.PublishBinaryMessage("LaunchApp:WriteTag", url.AsBuffer(), MessageWritenHandler);
        }

        private async void MessageWritenHandler(ProximityDevice device, long messageId)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var dialog = new MessageDialog("Message Id: " + messageId);
                await dialog.ShowAsync();
            });
        }
    }
}
