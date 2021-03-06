﻿using JakX.Server.Messages;
using JakX.Server.Messages.Lobby;
using JakX.Server.Messages.RTIME;
using Medius.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace JakX.Server.Medius
{
    public class MUIS : BaseMediusComponent
    {
        public override int Port => Program.Settings.MUISPort;
        public override PS2_RSA AuthKey => Program.GlobalAuthKey;

        public MUIS()
        {
            _sessionCipher = new PS2_RC4(Utils.FromString(Program.KEY), CipherContext.RC_CLIENT_SESSION);
        }

        protected override void Tick(ClientSocket client)
        {
            List<BaseMessage> recv = new List<BaseMessage>();
            List<BaseMessage> responses = new List<BaseMessage>();

            lock (_queue)
            {
                while (_queue.Count > 0)
                    recv.Add(_queue.Dequeue());
            }

            foreach (var msg in recv)
                HandleCommand(msg, client, ref responses);

            responses.Send(client);
        }

        protected override int HandleCommand(BaseMessage message, ClientSocket client, ref List<BaseMessage> responses)
        {
            // Log if id is set
            if (Program.Settings.IsLog(message.Id))
                Console.WriteLine($"MUIS {client}: {message}");

            // Update client echo
            client.ClientObject?.OnEcho(DateTime.UtcNow);

            // 
            switch (message.Id)
            {
                case RT_MSG_TYPE.RT_MSG_CLIENT_HELLO: //Connecting 1

                    responses.Add(new RT_MSG_SERVER_HELLO());

                    break;
                case RT_MSG_TYPE.RT_MSG_CLIENT_CRYPTKEY_PUBLIC:
                    {
                        responses.Add(new RT_MSG_SERVER_CRYPTKEY_PEER() { Key = Utils.FromString(Program.KEY) });
                        break;
                    }
                case RT_MSG_TYPE.RT_MSG_CLIENT_CONNECT_TCP:
                    {
                        responses.Add(new RT_MSG_SERVER_CONNECT_REQUIRE() { Contents = Utils.FromString("024802") });
                        break;
                    }
                case RT_MSG_TYPE.RT_MSG_CLIENT_CONNECT_READY_REQUIRE:
                    {
                        responses.Add(new RT_MSG_SERVER_CRYPTKEY_GAME() { Key = Utils.FromString(Program.KEY) });
                        responses.Add(new RT_MSG_SERVER_CONNECT_ACCEPT_TCP()
                        {
                            UNK_00 = 0,
                            UNK_01 = 0,
                            UNK_02 = 0,
                            UNK_03 = 0,
                            UNK_04 = 0,
                            UNK_05 = 0,
                            UNK_06 = 0x0001,
                            IP = (client.RemoteEndPoint as IPEndPoint)?.Address
                        });
                        break;
                    }
                case RT_MSG_TYPE.RT_MSG_CLIENT_CONNECT_READY_TCP:
                    {
                        responses.Add(new RT_MSG_SERVER_CONNECT_COMPLETE() { ARG1 = 0x0001 });
                        responses.Add(new RT_MSG_SERVER_ECHO());
                        break;
                    }
                case RT_MSG_TYPE.RT_MSG_SERVER_ECHO:
                    {

                        break;
                    }
                case RT_MSG_TYPE.RT_MSG_CLIENT_APP_TOSERVER:
                    {
                        var appMsg = (message as RT_MSG_CLIENT_APP_TOSERVER).AppMessage;

                        switch (appMsg.Id)
                        {
                            case MediusAppPacketIds.GetUniverseInformation:
                                {
                                    var msg = appMsg as MediusGetUniverseInformationRequest;

                                    // 
                                    responses.Add(new RT_MSG_SERVER_APP() { AppMessage = new MediusUniverseVariableSvoURLResponse() { Result = 1 } });

                                    // 
                                    responses.Add(new RT_MSG_SERVER_APP()
                                    {
                                        AppMessage = new MediusUniverseVariableInformationResponse()
                                        {
                                            MessageID = msg.MessageID,
                                            StatusCode = MediusCallbackStatus.MediusSuccess,
                                            InfoFilter = msg.InfoType,
                                            UniverseID = 1,
                                            ExtendedInfo = "",
                                            UniverseName = "Jak X Revival",
                                            UniverseDescription = "Jak X Revival",
                                            DNS = "jakx-prod.muis.pdonline.scea.com",
                                            // DNS = "randc-JakX.online.scee.com",
                                            Port = Program.AuthenticationServer.Port,
                                            EndOfList = true
                                        }
                                    });
                                    
                                    break;
                                }
                            default:
                                {
                                    Console.WriteLine($"UNHANDLED APP MESSAGE ID: {appMsg.Id}");
                                    break;
                                }
                        }
                        
                        break;
                    }
                case RT_MSG_TYPE.RT_MSG_CLIENT_ECHO:
                    {
                        responses.Add(new RT_MSG_CLIENT_ECHO() { Value = (message as RT_MSG_CLIENT_ECHO).Value });
                        break;
                    }
                case RT_MSG_TYPE.RT_MSG_CLIENT_DISCONNECT:
                case RT_MSG_TYPE.RT_MSG_CLIENT_DISCONNECT_WITH_REASON:
                    {
                        client.Disconnect();
                        break;
                    }
                default:
                    {
                        Console.WriteLine($"UNHANDLED MESSAGE ID: {message.Id}");

                        break;
                    }
            }

            return 0;
        }
    }
}
