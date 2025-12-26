using Android.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Bourse.Views.Main.BluetoothConnectionEventArgs;

namespace Bourse.Views.Main
{
    internal class ConnectThread : Java.Lang.Thread
    {
        #region Overall

        /// <summary>
        /// Utilisation pour les logs
        /// </summary>
        protected string Tag => MethodBase.GetCurrentMethod().DeclaringType.FullName;

        #endregion

        private readonly BluetoothSocket socket;
        private readonly string socketType;

        public event EventHandler<BluetoothConnectionEventArgs> Finish;

        public ConnectThread(BluetoothSocket socket, bool secure)
        {
            socketType = secure ? "Secure" : "Insecure";
            this.socket = socket;
        }

        public override void Run()
        {
            Name = $"ConnectThread_{socketType}";

            try
            {
                // This is a blocking call and will only return on a successful connection or an exception
                socket.Connect();
            }
            catch
            {
                Finish?.Invoke(this, new BluetoothConnectionEventArgs());
            }

            Finish?.Invoke(this, new BluetoothConnectionEventArgs(socket, BluetoothConnectionStatus.Connected));
        }

        public void Cancel()
        {
            try
            {
                socket.Close();
            }
            catch (Java.IO.IOException e)
            {
                //Log.Error(Tag, "close() of connect socket failed", e);
            }
        }
    }

}
