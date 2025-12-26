using Android.Bluetooth;
using Bourse.Common;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Bourse.Views.Main.BluetoothBaseActivity;

namespace Bourse.Views.Main
{
    /// <summary>
    /// <para>This class does all the work for setting up and managing Bluetooth connections with other devices. 
    /// It has :
    /// - a thread that listens for incoming connections, 
    /// - a thread for connecting with a device, 
    /// - a thread for performing data transmissions when connected.
    /// </para>
    /// <para>Note that this isn't a real Android service class; this is a wrapper which manages the various threads used to connect, send, and receive messages via BT.</para>
    /// </summary>
    internal class BluetoothService
    {
        #region Overall

        /// <summary>
        /// Utilisation pour les logs
        /// </summary>
        protected string Tag => MethodBase.GetCurrentMethod().DeclaringType.FullName;

        #endregion

        /// <summary>
        /// GUID pour identification du process bluetooth (connexion hors bond / insecure)
        /// </summary>
        private readonly UUID _insecureKey;

        /// <summary>
        /// GUID pour identification du process bluetooth (connexion bond / secure)
        /// </summary>
        private readonly UUID _secureKey;

        private readonly BluetoothAdapter btAdapter;

        /// <summary>
        /// Device connecté
        /// </summary>
        public BluetoothDevice ConnectedDevice => Socket?.RemoteDevice;


        /// <summary>
        /// Thread de connection: thread est nécessaire pour pouvoir annuler si déjà en cours (cf. Cancel)...
        /// </summary>
        private ConnectThread connectThread;

        /// <summary>
        /// Theard de partage : envoi et recoit les messages entre les devices
        /// </summary>
        private ConnectedThread connectedThread;

        /// <summary>
        /// Permet d'envoyer l'accord/confirmation de connexion au device partenaire
        /// </summary>
        private AcceptThread secureAcceptThread;

        /// <summary>
        /// Permet d'envoyer l'accord/confirmation de connexion au device partenaire
        /// </summary>
        private AcceptThread insecureAcceptThread;

        /// <summary>
        /// Message d'échange avec l'activité
        /// </summary>
        public event EventHandler<BluetoothEventArgs> MessageHandler;

        /// <summary>
        /// Etat courant de la connection Bluetooth
        /// </summary>
        public ServiceStates State { get; private set; }

        /// <summary>
        /// Constructor. Prepares a new BluetoothChat session.
        /// </summary>
        /// <param name='handler'>
        /// A Handler to send messages back to the UI Activity.
        /// </param>
        public BluetoothService(UUID secureKey, UUID insecureKey)
        {
            _secureKey = secureKey;
            _insecureKey = insecureKey;
            btAdapter = BluetoothAdapter.DefaultAdapter;
            State = ServiceStates.Disconnected;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        void UpdateUserInterfaceTitle()
        {
            MessageHandler.Invoke(this, new BluetoothEventArgs(BluetoothEventArgs.TypeMessages.State, State.GetCodeValue()));
        }

        /// <summary>
        /// Start the chat service. 
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start()
        {
            if (connectThread != null)
            {
                connectThread.Cancel();
                connectThread = null;
            }

            if (connectedThread != null)
            {
                connectedThread.Cancel();
                connectedThread = null;
            }

            if (secureAcceptThread == null)
            {
                secureAcceptThread = new AcceptThread(btAdapter.ListenUsingRfcommWithServiceRecord("BluetoothServiceSecure", _secureKey));
                secureAcceptThread.OnAcquireConnection += (s, e) =>
                {
                    if (e.Socket == null || !e.Socket.IsConnected)
                    {
                        secureAcceptThread.Cancel();
                        secureAcceptThread = null;
                    }
                    else
                    {
                        Socket = e.Socket;
                        Connected();
                    }
                };
                secureAcceptThread.Start();
            }
            if (insecureAcceptThread == null)
            {
                insecureAcceptThread = new AcceptThread(btAdapter.ListenUsingInsecureRfcommWithServiceRecord("BluetoothServiceInsecure", _insecureKey));
                insecureAcceptThread.OnAcquireConnection += (s, e) =>
                {
                    if (e.Socket == null || !e.Socket.IsConnected)
                    {
                        insecureAcceptThread.Cancel();
                        insecureAcceptThread = null;
                    }
                    else
                    {
                        Socket = e.Socket;
                        Connected();
                    }
                };
                insecureAcceptThread.Start();
            }
            UpdateUserInterfaceTitle();
        }

        /// <summary>
        /// Start the ConnectThread to initiate a connection to a remote device.
        /// </summary>
        /// <param name='device'>
        /// The BluetoothDevice to connect.
        /// </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Connect(BluetoothDevice device, bool secure)
        {

            if (State == ServiceStates.Connecting && connectThread != null)
            {
                connectThread.Cancel();
                connectThread = null;
            }

            // Cancel any thread currently running a connection
            if (connectedThread != null)
            {
                connectedThread.Cancel();
                connectedThread = null;
            }

            // Start the thread to connect with the given device
            try
            {
                btAdapter.CancelDiscovery();
                Socket = secure ? device.CreateRfcommSocketToServiceRecord(_secureKey) : device.CreateInsecureRfcommSocketToServiceRecord(_insecureKey);
                State = ServiceStates.Connecting;
                connectThread = new ConnectThread(Socket, secure);
                connectThread.Finish += (s, e) =>
                {
                    switch (e.Status)
                    {
                        case BluetoothConnectionEventArgs.BluetoothConnectionStatus.Connected:
                            connectThread = null;
                            Connected();
                            break;
                        case BluetoothConnectionEventArgs.BluetoothConnectionStatus.Failed:
                            try
                            {
                                Socket.Close();
                            }
                            catch (Java.IO.IOException e2)
                            {
                                //Log.Error(Tag, $"unable to close() socket during connection failure.", e2);
                            }

                            ConnectionFailed();
                            break;
                    }
                };
                connectThread.Start();
            }
            catch (Java.IO.IOException e)
            {
                //Log.Error(Tag, "create() failed", e);
                Socket = null;
            }

            UpdateUserInterfaceTitle();
        }

        private BluetoothSocket Socket = null;

        /// <summary>
        /// Start the ConnectedThread to begin managing a Bluetooth connection
        /// </summary>
        /// <param name='socket'>
        /// The BluetoothSocket on which the connection was made.
        /// </param>
        /// <param name='device'>
        /// The BluetoothDevice that has been connected.
        /// </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Connected()
        {
            // Cancel the thread that completed the connection
            if (connectThread != null)
            {
                connectThread.Cancel();
                connectThread = null;
            }

            // Cancel any thread currently running a connection
            if (connectedThread != null)
            {
                connectedThread.Cancel();
                connectedThread = null;
            }

            if (secureAcceptThread != null)
            {
                secureAcceptThread.Cancel();
                secureAcceptThread = null;
            }

            if (insecureAcceptThread != null)
            {
                insecureAcceptThread.Cancel();
                insecureAcceptThread = null;
            }


            // Start the thread to manage the connection and perform transmissions
            State = ServiceStates.Connected;

            connectedThread = new ConnectedThread(Socket);
            connectedThread.LostConnecion += (s, e) =>
            {
                ConnectionLost();
            };
            connectedThread.MessageHandler += (s, e) =>
            {
                MessageHandler?.Invoke(s, e);
            };
            connectedThread.Start();

            MessageHandler?.Invoke(this, new BluetoothEventArgs(BluetoothEventArgs.TypeMessages.DeviceName, ConnectedDevice.Name));
            UpdateUserInterfaceTitle();
        }

        /// <summary>
        /// Stop all threads.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            if (connectThread != null)
            {
                connectThread.Cancel();
                connectThread = null;
            }

            if (connectedThread != null)
            {
                connectedThread.Cancel();
                connectedThread = null;
            }

            if (secureAcceptThread != null)
            {
                secureAcceptThread.Cancel();
                secureAcceptThread = null;
            }

            if (insecureAcceptThread != null)
            {
                insecureAcceptThread.Cancel();
                insecureAcceptThread = null;
            }

            State = ServiceStates.Disconnected;
            UpdateUserInterfaceTitle();
        }

        /// <summary>
        /// Write to the ConnectedThread in an unsynchronized manner
        /// </summary>
        /// <param name='out'>
        /// The bytes to write.
        /// </param>
        public void Write(string message)
        {
            // Create temporary object
            ConnectedThread r;
            // Synchronize a copy of the ConnectedThread
            lock (this)
            {
                if (State != ServiceStates.Connected)
                {
                    return;
                }
                r = connectedThread;
            }
            // Perform the write unsynchronized
            r.Write(message);
        }

        /// <summary>
        /// Indicate that the connection attempt failed and notify the UI Activity -> Start the service over to restart listening mode
        /// </summary>
        void ConnectionFailed()
        {
            State = ServiceStates.Listen;
            MessageHandler.Invoke(this, new BluetoothEventArgs(BluetoothEventArgs.TypeMessages.State, State.GetCodeValue()));
            Start();
        }

        /// <summary>
        /// Indicate that the connection was lost and notify the UI Activity.
        /// </summary>
        public void ConnectionLost()
        {
            State = ServiceStates.Disconnected;
            MessageHandler.Invoke(this, new BluetoothEventArgs(BluetoothEventArgs.TypeMessages.State, State.GetCodeValue()));
            Start();
        }
    }
}
