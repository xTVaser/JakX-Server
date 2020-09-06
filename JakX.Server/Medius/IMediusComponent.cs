using JakX.Server.Messages;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace JakX.Server.Medius
{
    public interface IMediusComponent
    {
        int Port { get; }

        void Start();
        void Stop();

        void Tick();
    }
}
