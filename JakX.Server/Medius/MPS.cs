﻿using JakX.Server.Messages;
using JakX.Server.Messages.DME;
using JakX.Server.Messages.Lobby;
using JakX.Server.Messages.MGCL;
using JakX.Server.Messages.RTIME;
using JakX.Server.Stream;
using Medius.Crypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace JakX.Server.Medius
{
    public class MPS : BaseMediusComponent
    {
        public override int Port => Program.Settings.MPSPort;
        public override PS2_RSA AuthKey => Program.DmeAuthKey;

        DateTime lastSend = DateTime.UtcNow;

        public MPS()
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

            // 
            var targetMsgs = client.ClientObject?.PullProxyMessages();
            if (targetMsgs != null && targetMsgs.Count > 0)
                responses.AddRange(targetMsgs);

            // 
            if ((DateTime.UtcNow - client.ClientObject?.UtcLastEcho)?.TotalSeconds > Program.Settings.ServerEchoInterval)
                Echo(client, ref responses);

            // 
            responses.Send(client);
        }

        protected override int HandleCommand(BaseMessage message, ClientSocket client, ref List<BaseMessage> responses)
        {
            // Log if id is set
            if (Program.Settings.IsLog(message.Id))
                Console.WriteLine($"MPS {client}: {message}");

            // Update client echo
            client.ClientObject?.OnEcho(DateTime.UtcNow);

            // 
            switch (message.Id)
            {
                case RT_MSG_TYPE.RT_MSG_CLIENT_HELLO:
                    {
                        responses.Add(new RT_MSG_SERVER_HELLO());

                        break;
                    }
                case RT_MSG_TYPE.RT_MSG_CLIENT_CRYPTKEY_PUBLIC:
                    {
                        responses.Add(new RT_MSG_SERVER_CRYPTKEY_PEER() { Key = Utils.FromString(Program.KEY) });
                        break;
                    }
                case RT_MSG_TYPE.RT_MSG_CLIENT_CONNECT_TCP:
                    {
                        var m00 = message as RT_MSG_CLIENT_CONNECT_TCP;
                        client.SetToken(m00.AccessToken);

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
                            UNK_02 = 0x01,
                            UNK_06 = 0x01,
                            IP = (client.RemoteEndPoint as IPEndPoint)?.Address
                        });
                        break;
                    }
                case RT_MSG_TYPE.RT_MSG_CLIENT_CONNECT_READY_TCP:
                    {
                        responses.Add(new RT_MSG_SERVER_CONNECT_COMPLETE() { ARG1 = 0x0001 });
                        break;
                    }
                case RT_MSG_TYPE.RT_MSG_SERVER_ECHO:
                    {
                        client.ClientObject?.OnEcho(DateTime.UtcNow);
                        break;
                    }
                case RT_MSG_TYPE.RT_MSG_CLIENT_APP_TOSERVER:
                    {
                        var appMsg = (message as RT_MSG_CLIENT_APP_TOSERVER).AppMessage;

                        switch (appMsg.Id)
                        {
                            case MediusAppPacketIds.MediusServerCreateGameWithAttributesResponse:
                                {
                                    var msg = appMsg as MediusServerCreateGameWithAttributesResponse;

                                    int gameId = int.Parse(msg.MessageID.Split('-')[0]);
                                    int accountId = int.Parse(msg.MessageID.Split('-')[1]);
                                    string msgId = msg.MessageID.Split('-')[2];
                                    var game = Program.GetGameById(gameId);
                                    var rClient = Program.GetClientByAccountId(accountId);
                                    game.DMEWorldId = msg.WorldID;

                                    rClient.AddLobbyMessage(new RT_MSG_SERVER_APP()
                                    {
                                        AppMessage = new MediusCreateGameResponse()
                                        {
                                            MessageID = msgId,
                                            StatusCode = MediusCallbackStatus.MediusSuccess,
                                            MediusWorldID = game.Id
                                        }
                                    });


                                    break;
                                }
                            case MediusAppPacketIds.MediusServerJoinGameResponse:
                                {
                                    var msg = appMsg as MediusServerJoinGameResponse;

                                    int gameId = int.Parse(msg.MessageID.Split('-')[0]);
                                    int accountId = int.Parse(msg.MessageID.Split('-')[1]);
                                    string msgId = msg.MessageID.Split('-')[2];
                                    var game = Program.GetGameById(gameId);
                                    var rClient = Program.GetClientByAccountId(accountId);

                                    game.OnPlayerJoined(rClient);
                                    rClient.AddLobbyMessage(new RT_MSG_SERVER_APP()
                                    {
                                        AppMessage = new MediusJoinGameResponse()
                                        {
                                            MessageID = msgId,
                                            StatusCode = MediusCallbackStatus.MediusSuccess,
                                            GameHostType = game.GameHostType,
                                            ConnectInfo = new NetConnectionInfo()
                                            {
                                                AccessKey = msg.AccessKey,
                                                SessionKey = rClient.SessionKey,
                                                WorldID = game.DMEWorldId,
                                                ServerKey = msg.pubKey,
                                                AddressList = new NetAddressList()
                                                {
                                                    AddressList = new NetAddress[MediusConstants.NET_ADDRESS_LIST_COUNT]
                                                        {
                                                            //new NetAddress() { Address = Program.SERVER_IP.ToString(), Port = (uint)Program.ProxyServer.Port, AddressType = NetAddressType.NetAddressTypeExternal},
#if FALSE
                                                            new NetAddress() { Address = (client.RemoteEndPoint as IPEndPoint)?.Address.ToString(), Port = (uint)(client.Client as DMEObject).Port, AddressType = NetAddressType.NetAddressTypeExternal},
#else                                                        
                                                            new NetAddress() { Address = (client.ClientObject as DMEObject).IP.ToString(), Port = (uint)(client.ClientObject as DMEObject).Port, AddressType = NetAddressType.NetAddressTypeExternal},
#endif
                                                            new NetAddress() { AddressType = NetAddressType.NetAddressNone},
                                                        }
                                                },
                                                Type = NetConnectionType.NetConnectionTypeClientServerTCPAuxUDP
                                            }
                                        }
                                    });
                                    break;
                                }

                            case MediusAppPacketIds.MediusServerReport:
                                {
                                    (client.ClientObject as DMEObject)?.OnWorldReport(appMsg as MediusServerReport);

                                    break;
                                }
                            case MediusAppPacketIds.MediusServerConnectNotification:
                                {
                                    var msg = appMsg as MediusServerConnectNotification;

                                    Program.GetGameById((int)msg.MediusWorldUID)?.OnMediusServerConnectNotification(msg);
                                    

                                    break;
                                }

                            case MediusAppPacketIds.MediusServerEndGameResponse:
                                {
                                    var msg = appMsg as MediusServerEndGameResponse;

                                    break;
                                }

                            default:
                                {
                                    Console.WriteLine($"MPS Unhandled App Message: {appMsg.Id} {appMsg}");
                                    break;
                                }
                        }
                        break;
                    }

                case RT_MSG_TYPE.RT_MSG_CLIENT_ECHO:
                    {
                        client.ClientObject?.OnEcho(DateTime.UtcNow);
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
                        Console.WriteLine($"MPS Unhandled Medius Command: {message.Id} {message}");
                        break;
                    }
            }

            return 0;
        }


        protected ClientSocket GetFreeDme()
        {
            try
            {
                return Clients.Where(x => x.ClientObject is DMEObject).MinBy(x => (x.ClientObject as DMEObject).CurrentWorlds);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e);
            }

            return null;
        }


        public void CreateGame(ClientSocket client, MediusCreateGameRequest request)
        {
            // Ensure the name is unique
            // If the host leaves then we unreserve the name
            if (Program.Games.Any(x => x.WorldStatus != MediusWorldStatus.WorldClosed && x.WorldStatus != MediusWorldStatus.WorldInactive && x.GameName == request.GameName && x.Host != null && x.Host.IsConnected))
            {
                client.ClientObject.AddLobbyMessage(new RT_MSG_SERVER_APP()
                {
                    AppMessage = new MediusCreateGameResponse()
                    {
                        MessageID = request.MessageID,
                        MediusWorldID = -1,
                        StatusCode = MediusCallbackStatus.MediusGameNameExists
                    }
                });
                return;
            }

            // 
            var dme = GetFreeDme()?.ClientObject as DMEObject;
            if (dme == null)
            {
                client.ClientObject.AddLobbyMessage(new RT_MSG_SERVER_APP()
                {
                    AppMessage = new MediusCreateGameResponse()
                    {
                        MessageID = request.MessageID,
                        MediusWorldID = -1,
                        StatusCode = MediusCallbackStatus.MediusDMEError
                    }
                });
                return;
            }

            var game = new Game(client.ClientObject, request, dme);
            Program.Games.Add(game);
            Console.WriteLine($"DEBUG: GAME ADDED");

            dme.AddProxyMessage(new RT_MSG_SERVER_APP()
            {
                AppMessage = new MediusServerCreateGameWithAttributesRequest()
                {
                    MessageID = $"{game.Id}-{client.ClientObject.ClientAccount.AccountId}-{request.MessageID}",
                    MediusWorldUID = (uint)game.Id,
                    Attributes = game.Attributes,
                    ApplicationID = Program.Settings.ApplicationId,
                    MaxClients = game.MaxPlayers
                }
            });
        }

        public void JoinGame(ClientSocket client, MediusJoinGameRequest request)
        {
            var game = Program.GetGameById(request.MediusWorldID);
            if (game == null)
            {
                client.ClientObject.AddLobbyMessage(new RT_MSG_SERVER_APP()
                {
                    AppMessage = new MediusJoinGameResponse()
                    {
                        MessageID = request.MessageID,
                        StatusCode = MediusCallbackStatus.MediusGameNotFound
                    }
                });
            }
            else if (game.GamePassword != null && game.GamePassword != string.Empty && game.GamePassword != request.GamePassword)
            {
                client.ClientObject.AddLobbyMessage(new RT_MSG_SERVER_APP()
                {
                    AppMessage = new MediusJoinGameResponse()
                    {
                        MessageID = request.MessageID,
                        StatusCode = MediusCallbackStatus.MediusInvalidPassword
                    }
                });
            }
            else
            {
                var dme = game.DMEServer;
                dme.AddProxyMessage(new RT_MSG_SERVER_APP()
                {
                    AppMessage = new MediusServerJoinGameRequest()
                    {
                        MessageID = $"{game.Id}-{client.ClientObject.ClientAccount.AccountId}-{request.MessageID}",
                        ConnectInfo = new NetConnectionInfo()
                        {
                            Type = NetConnectionType.NetConnectionTypeClientServerTCPAuxUDP,
                            WorldID = game.DMEWorldId,
                            SessionKey = request.SessionKey,
                            ServerKey = Program.GlobalAuthPublic
                        }
                    }
                });
            }
        }
    }
}
