using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bourse.Views.Main
{
    public class BluetoothEventArgs : EventArgs
    {
        public enum TypeMessages { DeviceName, State, Toast, Read, Write }

        public string Message { get; private set; }
        public TypeMessages Type { get; private set; }

        public BluetoothEventArgs(TypeMessages type, string message)
        {
            Message = message;
            Type = type;
        }
    }
}
