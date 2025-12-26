using Android.Bluetooth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Bourse.Views.Main.BluetoothBaseActivity;

namespace Bourse.Views.Main
{
    /// <summary>
    /// This thread runs while listening for incoming connections. It behaves
    /// like a server-side client. It runs until a connection is accepted
    /// (or until cancelled).
    /// </summary>
    class AcceptThread : Java.Lang.Thread
    {
        #region Overall

        /// <summary>
        /// Utilisation pour les logs
        /// </summary>
        protected string Tag => MethodBase.GetCurrentMethod().DeclaringType.FullName;

        #endregion

        private ServiceStates state;

        // The local server socket
        private readonly BluetoothServerSocket serverSocket;

        public event EventHandler<BluetoothConnectionEventArgs> OnAcquireConnection;

        public AcceptThread(BluetoothServerSocket socket)
        {
            serverSocket = socket;
            state = ServiceStates.Listen;
        }

        public override void Run()
        {
            Name = $"AcceptThread";
            BluetoothSocket socket = null;

            while (state != ServiceStates.Connected)
            {
                try
                {
                    socket = serverSocket.Accept(); // Block until a connection is established.
                }
                catch (Java.IO.IOException e)
                {
                    //Log.Error(Tag, "accept() failed", e);
                    break;
                }
                if (socket != null)
                {
                    OnAcquireConnection?.Invoke(this, new BluetoothConnectionEventArgs(socket));
                }
            }
        }

        public void Cancel()
        {
            try
            {
                state = ServiceStates.Disconnected;
                serverSocket.Close();
            }
            catch (Java.IO.IOException e)
            {
                //Log.Error(Tag, "close() of server failed", e);
            }
        }
    }

}
