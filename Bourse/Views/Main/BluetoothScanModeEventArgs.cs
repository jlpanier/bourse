using Android.Bluetooth;

namespace Bourse.Views.Main
{
    public class BluetoothScanModeEventArgs
    {
        public override string ToString() => $"{ScanMode} {ConnectionState} {DiscoverableDuration} {LocalName} {State}";

        /// <summary>
        /// the current scan mode.
        /// </summary>
        public ScanMode ScanMode { get; set; }

        /// <summary>
        /// current connection state.
        /// </summary>
        public string ConnectionState { get; set; }

        /// <summary>
        /// request a specific duration for discoverability in seconds.
        /// </summary>
        public string DiscoverableDuration { get; set; }

        /// <summary>
        /// request the local Bluetooth name.
        /// </summary>
        public string LocalName { get; set; }

        /// <summary>
        /// represents the previous connection state.
        /// </summary>
        public string PreviousConnectionState { get; set; }

        /// <summary>
        ///  the previous scan mode.
        /// </summary>
        public string PreviousScanMode { get; set; }

        /// <summary>
        /// the previous power state.
        /// </summary>
        public string PreviousState { get; set; }

        /// <summary>
        /// the current power state.
        /// </summary>
        public string State { get; set; }

    }

}
