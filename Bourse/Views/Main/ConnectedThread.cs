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
    /// This thread runs during a connection with a remote device.
    /// It handles all incoming and outgoing transmissions.
    /// </summary>
    class ConnectedThread : Java.Lang.Thread
    {
        #region Overall

        /// <summary>
        /// Utilisation pour les logs
        /// </summary>
        protected string Tag => MethodBase.GetCurrentMethod().DeclaringType.FullName;

        #endregion

        /// <summary>
        /// Message échanger entre les deux devices
        /// </summary>
        public event EventHandler<BluetoothEventArgs> MessageHandler;

        public event EventHandler LostConnecion;


        private readonly BluetoothSocket socket;
        private readonly Stream inStream;
        private readonly Stream outStream;

        private ServiceStates state = ServiceStates.Connected;

        public ConnectedThread(BluetoothSocket socket)
        {
            this.socket = socket;
            Stream tmpIn = null;
            Stream tmpOut = null;

            // Get the BluetoothSocket input and output streams
            try
            {
                tmpIn = socket.InputStream;
                tmpOut = socket.OutputStream;
            }
            catch (Java.IO.IOException e)
            {
                //Log.Error(Tag, "temp sockets not created", e);
            }

            inStream = tmpIn;
            outStream = tmpOut;
        }

        public override void Run()
        {
            //Log.Info(Tag, "BEGIN mConnectedThread");
            byte[] buffer = new byte[1024];
            int index = 0;
            foreach (byte b in buffer)
            {
                buffer[index++] = 0;
            }
            int bytes;

            while (state == ServiceStates.Connected && socket.IsConnected)
            {
                try
                {
                    bytes = inStream.Read(buffer, 0, buffer.Length);
                    MessageHandler?.Invoke(this, new BluetoothEventArgs(BluetoothEventArgs.TypeMessages.Read, Encoding.ASCII.GetString(buffer, 0, bytes)));
                }
                catch (Java.IO.IOException e)
                {
                    LostConnecion?.Invoke(this, EventArgs.Empty);
                    break;
                }
                catch (Exception e)
                {
                    LostConnecion?.Invoke(this, EventArgs.Empty);
                    break;
                }
            }
        }

        /// <summary>
        /// Write to the connected OutStream.
        /// </summary>
        /// <param name='buffer'>
        /// The bytes to write
        /// </param>
        public void Write(string message)
        {
            try
            {
                if (socket.IsConnected)
                {
                    byte[] outbuffer = Encoding.ASCII.GetBytes(message);
                    outStream.Write(outbuffer, 0, outbuffer.Length);

                    // Share the sent message back to the UI Activity
                    MessageHandler?.Invoke(this, new BluetoothEventArgs(BluetoothEventArgs.TypeMessages.Write, message));
                }
            }
            catch (Java.IO.IOException e)
            {
                //Log.Error(Tag, "Exception during write", e);
            }
        }

        public void Cancel()
        {
            try
            {
                state = ServiceStates.Disconnected;
                socket.Close();
            }
            catch (Java.IO.IOException e)
            {
                //Log.Error(Tag, "close() of connect socket failed", e);
            }
        }
    }
}
