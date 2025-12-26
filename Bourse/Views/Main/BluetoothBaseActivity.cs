using Android.Bluetooth;
using Android.Content;
using Bourse.Common;
using Java.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bourse.Views.Main
{
    public abstract class BluetoothBaseActivity : BaseActivity
    {
        /// <summary>
        /// Etat du service
        /// </summary>
        public enum ServiceStates
        {
            [StringValue("None")]
            [CodeValue("")]
            Disconnected,
            [StringValue("listening for incoming connections")]
            [CodeValue("Listen")]
            Listen,
            [StringValue("initiating an outgoing connection")]
            [CodeValue("Connecting")]
            Connecting,
            [StringValue("now connected to a remote device")]
            [CodeValue("Connected")]
            Connected
        }

        private static ServiceStates ParseCodeState(string data)
        {
            ServiceStates result = ServiceStates.Disconnected;
            if (!string.IsNullOrEmpty(data))
            {
                foreach (ServiceStates item in Enum.GetValues(typeof(ServiceStates)))
                {
                    if (item.GetCodeValue() == data)
                    {
                        result = item;
                    }
                }
            }
            return result;
        }

        #region Propriétés

        public virtual UUID SecureKey { get; } = UUID.FromString("fa87c0d0-afac-11de-8a39-0800200c9a66");
        public virtual UUID InsecureKey { get; } = UUID.FromString("8ce255c0-200a-11e0-ac64-0800200c9a66");

        /// <summary>
        /// Connected device
        /// </summary>
        public BluetoothDevice ConnectedDevice => bluetoothService?.ConnectedDevice;

        #endregion

        #region Internal variable

        private BluetoothBroadcastReceiver _receiver;

        /// <summary>
        /// Adaptateur Bluetooth 
        /// </summary>
        private BluetoothAdapter _btAdapter;

        /// <summary>
        /// Service Bluetooth : connection aux devices et échanges d'informations
        /// </summary>
        private BluetoothService bluetoothService = null;

        /// <summary>
        /// Bonded device
        /// </summary>
        protected ICollection<BluetoothDevice> BluetoothBondedDevices => _btAdapter.BondedDevices;

        #endregion

        #region bluetooth message 

        /// <summary>
        /// Broadcast action bluetooth : remote device discovered
        /// </summary>
        /// <param name="device"></param>
        protected virtual void OnBluetoothDeviceFound(BluetoothDevice device)
        {
        }

        /// <summary>
        /// Broadcast action bluetooth : the local bluetooth adapter finished discovery
        /// </summary>
        protected virtual void OnBluetoothDiscoveryFinished()
        {
        }

        /// <summary>
        /// Broadcast action bluetooth : indicates the bluetooth scan mode of the local adapter has changed
        /// </summary>
        protected virtual void OnBluetoothScanModeChanged(BluetoothScanModeEventArgs scanmode)
        {
        }

        /// <summary>
        /// Bluetooth : Reception message
        /// </summary>
        protected abstract void OnBluetoothReceive(string message);

        /// <summary>
        /// Bluetooth : Sent message
        /// </summary>
        protected virtual void OnBluetoothSend(string message)
        {
        }

        /// <summary>
        /// Bluetooth : Bluetooth change status connexion
        /// </summary>
        protected virtual void OnBluetoothStatusChanged(BluetoothDevice device, ServiceStates state)
        {
        }

        #endregion

        #region life cycle

        /// <summary>
        /// OnCreate est la première méthode à appeler lorsqu’une activité est créée. 
        /// OnCreate est toujours remplacée pour effectuer toutes les initialisations de démarrage qui peuvent être requis par une activité telles que :
        /// - Création de vues
        /// - Initialiser des variables
        /// - Liaison de données statiques aux listes
        /// Aide Windows: https://docs.microsoft.com/fr-fr/xamarin/android/
        /// https://developer.xamarin.com/guides/android/application_fundamentals/activity_lifecycle/
        /// Icon : http://modernuiicons.com/ 
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _btAdapter = BluetoothAdapter.DefaultAdapter;

            _receiver = new BluetoothBroadcastReceiver();
            RegisterReceiver(_receiver, new IntentFilter(BluetoothDevice.ActionFound));
            RegisterReceiver(_receiver, new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished));
            RegisterReceiver(_receiver, new IntentFilter(BluetoothAdapter.ActionScanModeChanged));

            _receiver.BluetoothDeviceFound += (s, e) => OnBluetoothDeviceFound(e);
            _receiver.BluetoothDiscoveryFinished += (s, e) => OnBluetoothDiscoveryFinished();
            _receiver.BluetoothScanModeChanged += (s, e) => OnBluetoothScanModeChanged(e);

            bluetoothService = new BluetoothService(SecureKey, InsecureKey);
            bluetoothService.MessageHandler += (s, e) =>
            {
                RunOnUiThread(() =>
                {
                    switch (e.Type)
                    {
                        case BluetoothEventArgs.TypeMessages.Read:
                            OnBluetoothReceive(e.Message);
                            break;
                        case BluetoothEventArgs.TypeMessages.Write:
                            OnBluetoothSend(e.Message);
                            break;
                        case BluetoothEventArgs.TypeMessages.DeviceName:
                            Message(e.Message);
                            break;
                        case BluetoothEventArgs.TypeMessages.State:
                            OnBluetoothStatusChanged(bluetoothService?.ConnectedDevice, ParseCodeState(e.Message));
                            break;
                    }
                });
            };
        }

        /// <summary>
        /// Le système appelle OnResume lorsque l’activité est prête à commencer à interagir avec l’utilisateur. 
        /// Activités doivent substituer cette méthode pour effectuer des tâches telles que :
        /// - En identifiant des fréquences d’images(une tâche courante dans la construction de jeu)
        /// - Démarrage des animations
        /// - À l’écoute des mises à jour GPS
        /// - Afficher les alertes pertinentes ou les boîtes de dialogue
        /// - Des gestionnaires d’événements externes
        /// https://developer.xamarin.com/guides/android/application_fundamentals/activity_lifecycle/
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            if (bluetoothService != null && bluetoothService.State == ServiceStates.Disconnected)
            {
                bluetoothService.Start();
            }
        }

        /// <summary>
        /// OnDestroy est la dernière méthode appelée sur une instance d’activité avant qu’elle ne soit détruite et complètement supprimée de la mémoire. 
        /// Dans les situations extrêmes, Android peut arrêter le processus d’application qui héberge l’activité, ce qui entraînera l' OnDestroy invocation de. 
        /// La plupart des activités n’implémentent pas cette méthode, car la plupart des opérations de nettoyage et d’arrêt ont été effectuées dans les OnPause OnStop méthodes 
        /// OnDestroy méthode est généralement remplacée pour nettoyer les tâches de longue durée qui peuvent provoquer des fuites de ressources. 
        /// Il peut s’agir, par exemple, de threads d’arrière-plan démarrés dans OnCreate .
        /// Aucune méthode de cycle de vie n’est appelée une fois que l’activité a été détruite
        /// </summary>
        protected override void OnDestroy()
        {
            UnregisterReceiver(_receiver);

            base.OnDestroy();

            if (_btAdapter != null)
            {
                _btAdapter.CancelDiscovery();
            }

            if (bluetoothService != null)
            {
                bluetoothService.Stop();
            }
        }

        #endregion

        #region User Interface

        /// <summary>
        /// Connection à bluetooth : unsecure socket
        /// </summary>
        /// <param name="device"></param>
        protected void BluetoothConnectUnbond(BluetoothDevice device) => bluetoothService.Connect(device, false);

        /// <summary>
        /// Connection à bluetooth : secure socket
        /// </summary>
        /// <param name="device"></param>
        protected void BluetoothConnectBond(BluetoothDevice device) => bluetoothService.Connect(device, true);

        /// <summary>
        /// VRAI, si recherche des réseaux Bluetooth actif
        /// </summary>
        protected bool IsDiscovering => _btAdapter.IsDiscovering;

        /// <summary>
        /// Commencer la recherche des réseaux Bluetooth
        /// </summary>
        protected void StartDiscovery()
        {
            if (!IsDiscovering) _btAdapter.StartDiscovery();
        }

        /// <summary>
        /// Annule la recherche des réseaux Bluetooth
        /// </summary>
        protected void CancelDiscovery()
        {
            if (IsDiscovering) _btAdapter.CancelDiscovery();
        }

        /// <summary>
        /// Bluetooth : send a message to other device
        /// </summary>
        /// <param name="message"></param>
        protected virtual void SendBluetoothMessage(string message)
        {
            switch (bluetoothService.State)
            {
                case ServiceStates.Disconnected:
                    break;
                case ServiceStates.Listen:
                    break;
                case ServiceStates.Connecting:
                    break;
                case ServiceStates.Connected:
                    if (message.Length > 0)
                    {
                        bluetoothService.Write(message);
                    }
                    break;
            }
        }

        #endregion

    }

}
