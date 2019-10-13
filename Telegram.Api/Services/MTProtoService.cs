﻿// 
// This is the source code of Telegram for Windows Phone v. 3.x.x.
// It is licensed under GNU GPL v. 2 or later.
// You should have received a copy of the license in this archive (see LICENSE).
// 
// Copyright Evgeny Nadymov, 2013-present.
// 
#define MTPROTO
//#define DEBUG_UPDATEDCOPTIONS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using Org.BouncyCastle.Bcpg;
using Telegram.Api.Services.DeviceInfo;
using Telegram.Api.TL.Functions.Channels;
using Telegram.Api.TL.Functions.Messages;
#if WIN_RT
using Windows.UI.Xaml;
#elif WINDOWS_PHONE
using System.Windows.Threading;
using Microsoft.Devices;
#endif
using Telegram.Api.Extensions;
using Telegram.Api.Helpers;
using Telegram.Api.Services.Cache;
using Telegram.Api.Services.Connection;
using Telegram.Api.Services.Updates;
using Telegram.Api.TL;
using Telegram.Api.TL.Functions.Auth;
using Telegram.Api.TL.Functions.Upload;
using Telegram.Api.Transport;
using Telegram.Logs;
using Environment = System.Environment;

namespace Telegram.Api.Services
{
    public class CountryEventArgs : EventArgs
    {
        public string Country { get; set; }
    }

    public partial class MTProtoService : ServiceBase, IMTProtoService, IDisposable
    {
        public event EventHandler ProxyDisabled;

        protected void RaiseProxyDisabled()
        {
            var handler = ProxyDisabled;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public event EventHandler<CountryEventArgs> GotUserCountry;

        protected void RaiseGotUserCountry(string country)
        {
            var handler = GotUserCountry;
            if (handler != null)
            {
                handler(this, new CountryEventArgs { Country = country });
            }
        }

        public void SetInitState()
        {
            _updatesService.SetInitState();
        }

        public ITransport GetActiveTransport()
        {
            return _activeTransport;
        }

        public WindowsPhone.Tuple<int, int, int> GetCurrentPacketInfo()
        {
            return _activeTransport != null ? _activeTransport.GetCurrentPacketInfo() : null;
        }

        public string GetTransportInfo()
        {
            return _activeTransport != null ? _activeTransport.GetTransportInfo() : null;
        }

        public string Country
        {
            get { return _config != null ? _config.Country : null; }
        }

        private TLInt _currentUserId;

        public TLInt CurrentUserId
        {
            get
            {
                return _currentUserId;
            }
            set
            {
                _currentUserId = value;

            }
        }

        public long ClientTicksDelta { get { return _activeTransport.ClientTicksDelta; } }

        //private bool _isInitialized;

        /// <summary>
        /// Получен ли ключ авторизации
        /// </summary>
        //public bool IsInitialized
        //{
        //    get { return _isInitialized; }
        //    protected set
        //    {
        //        if (_isInitialized != value)
        //        {
        //            _isInitialized = value;
        //            NotifyOfPropertyChange(() => IsInitialized);
        //        }
        //    }
        //}

        public event EventHandler Initialized;

        protected virtual void RaiseInitialized()
        {
            var handler = Initialized;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler InitializationFailed;

        protected virtual void RaiseInitializationFailed()
        {
            var handler = InitializationFailed;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private readonly object _fileTransportRoot = new object();

        private readonly object _activeTransportRoot = new object();

        private ITransport _activeTransport;

        private readonly ITransportService _transportService;

        private readonly ICacheService _cacheService;

        private readonly IUpdatesService _updatesService;

        private readonly IConnectionService _connectionService;

        private readonly IDeviceInfoService _deviceInfo;

        private readonly Dictionary<long, HistoryItem> _history = new Dictionary<long, HistoryItem>();

#if DEBUG
        private readonly Dictionary<long, HistoryItem> _removedHistory = new Dictionary<long, HistoryItem>();
#endif

        public IList<HistoryItem> History
        {
            get { return _history.Values.ToList(); }
        }

        private TransportType _type = TransportType.Tcp;

        public TransportType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private readonly object _delayedItemsRoot = new object();

        private readonly List<DelayedItem> _delayedItems = new List<DelayedItem>();

        private Timer _timeoutsTimer;

        private Timer _deviceLockedTimer;

        private Timer _checkTransportTimer;

        public MTProtoService(IDeviceInfoService deviceInfo, IUpdatesService updatesService, ICacheService cacheService, ITransportService transportService, IConnectionService connectionService, IPublicConfigService publicConfigService)
        {
            var isBackground = deviceInfo != null && deviceInfo.IsBackground;

            _deviceInfo = deviceInfo;

            _sendingTimer = new Timer(CheckSendingMessages, this, Timeout.Infinite, Timeout.Infinite);
            _getConfigTimer = new Timer(CheckGetConfig, this, TimeSpan.FromSeconds(10.0), TimeSpan.FromSeconds(Constants.CheckGetConfigInterval));
            _timeoutsTimer = new Timer(CheckTimeouts, this, TimeSpan.FromSeconds(10.0), TimeSpan.FromSeconds(10.0));
            _deviceLockedTimer = new Timer(CheckDeviceLockedInternal, this, TimeSpan.FromSeconds(60.0), TimeSpan.FromSeconds(60.0));

            _publicConfigService = publicConfigService;

            _connectionService = connectionService;
            _connectionService.Initialize(this);
            _connectionService.ConnectionLost += OnConnectionLost;

            var sendStatusEvents = Observable.FromEventPattern<EventHandler<SendStatusEventArgs>, SendStatusEventArgs>(
                keh => { SendStatus += keh; },
                keh => { SendStatus -= keh; });

            _statusSubscription = sendStatusEvents
                .Throttle(TimeSpan.FromSeconds(Constants.UpdateStatusInterval))
                .Subscribe(e => UpdateStatusAsync(e.EventArgs.Offline, result => { }));

            _updatesService = updatesService;
            _updatesService.DCOptionsUpdated += OnDCOptionsUpdated;

            _cacheService = cacheService;

            if (_updatesService != null)
            {
                _updatesService.GetDifferenceAsync = GetDifferenceAsync;
                _updatesService.GetStateAsync = GetStateAsync;
                _updatesService.GetCurrentUserId = GetCurrentUserId;
                _updatesService.GetDHConfigAsync = GetDHConfigAsync;
                _updatesService.AcceptEncryptionAsync = AcceptEncryptionAsync;
                _updatesService.SendEncryptedServiceAsync = SendEncryptedServiceAsync;
                _updatesService.SetMessageOnTimeAsync = SetMessageOnTime;
                _updatesService.RemoveFromQueue = RemoveFromQueue;
                _updatesService.UpdateChannelAsync = UpdateChannelAsync;
                _updatesService.GetFullChatAsync = GetFullChatAsync;
                _updatesService.GetFullUserAsync = GetFullUserAsync;
                _updatesService.GetChannelMessagesAsync = GetMessagesAsync;
                _updatesService.GetPinnedDialogsAsync = GetPinnedDialogsAsync;
                _updatesService.GetMessagesAsync = GetMessagesAsync;
                _updatesService.GetPeerDialogsAsync = GetPeerDialogsAsync;
                _updatesService.GetPromoDialogAsync = GetPromoDialogAsync;
            }

            _transportService = transportService;
            _transportService.ConnectionLost += OnConnectionLost;
            _transportService.FileConnectionLost += OnFileConnectionLost;
            _transportService.SpecialConnectionLost += OnSpecialConnectionLost;
            _transportService.CheckConfig += OnCheckConfig;

            _activeTransport = GetTransport(Constants.FirstServerIpAddress, Constants.FirstServerPort, Type,
                new TransportSettings
                {
                    DcId = _activeTransport != null ? _activeTransport.DCId : Constants.FirstServerDCId,
                    Secret = _activeTransport != null ? _activeTransport.Secret : null,
                    AuthKey = _activeTransport != null ? _activeTransport.AuthKey : null,
                    Salt = _activeTransport != null ? _activeTransport.Salt : null,
                    SessionId = _activeTransport != null ? _activeTransport.SessionId : null,
                    MessageIdDict = _activeTransport != null ? _activeTransport.MessageIdDict : new Dictionary<long, long>(),
                    SequenceNumber = _activeTransport != null ? _activeTransport.SequenceNumber : 0,
                    ClientTicksDelta = _activeTransport != null ? _activeTransport.ClientTicksDelta : 0,
                    PacketReceivedHandler = OnPacketReceived
                });

#if DEBUG
            _checkTransportTimer = new Timer(CheckTransport, this, TimeSpan.FromSeconds(1.0), Timeout.InfiniteTimeSpan);
#endif

            Initialized += OnServiceInitialized;
            InitializationFailed += OnServiceInitializationFailed;

            //IsInitialized = true;
            if (!isBackground)
            {
                //Initialize();
            }

            Instance = this;
        }

        public event EventHandler<TransportCheckedEventArgs> TransportChecked;

        protected virtual void RaiseTransportChecked(TransportCheckedEventArgs e)
        {
            var handler = TransportChecked;
            if (handler != null) handler(this, e);
        }

        private void CheckTransport(object state)
        {
            //return;
            ITransport transport = null;

            lock (_activeTransportRoot)
            {
                transport = _activeTransport;
            }
            var transportId = transport.Id;
            var sessionId = transport.SessionId;
            var authKey = transport.AuthKey;
            var lastReceiveTime = transport.LastReceiveTime;
            int historyCount;
            string historyDescription;
            lock (_historyRoot)
            {
                historyCount = _history.Count;
                historyDescription = string.Join("\n", _history.Values.Select(x => x.Caption + " " + x.Hash));
            }
            var currentPacketLength = transport.PacketLength;
            var lastPacketLength = transport.LastPacketLength;

            RaiseTransportChecked(new TransportCheckedEventArgs
            {
                TransportId = transportId,
                SessionId = sessionId,
                AuthKey = authKey,
                LastReceiveTime = lastReceiveTime,
                HistoryCount = historyCount,
                HistoryDescription = historyDescription,
                NextPacketLength = currentPacketLength,
                LastPacketLength = lastPacketLength
            });

            _checkTransportTimer.Change(TimeSpan.FromSeconds(1.0), Timeout.InfiniteTimeSpan);
        }

        public static IMTProtoService Instance { get; protected set; }

        private void CheckTimeouts(object state)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                return;
            }
#endif

            const double timeout = Constants.TimeoutInterval;
            const double delayedTimeout = Constants.DelayedTimeoutInterval;
            const double nonEncryptedTimeout = Constants.NonEncryptedTimeoutInterval;

            var timedOutKeys = new List<long>();
            var timedOutValues = new List<HistoryItem>();
            var now = DateTime.Now;

            // history
            lock (_historyRoot)
            {
                foreach (var historyKeyValue in _history)
                {
                    var historyValue = historyKeyValue.Value;
                    if (historyValue.SendTime != default(DateTime)
                        && historyValue.SendTime.AddSeconds(timeout) < now)
                    {
                        timedOutKeys.Add(historyKeyValue.Key);
                        timedOutValues.Add(historyKeyValue.Value);
                    }
                }

                foreach (var key in timedOutKeys)
                {
                    _history.Remove(key);
                }
            }

            if (timedOutValues.Count > 0)
            {
                Execute.BeginOnThreadPool(() =>
                {
                    foreach (var item in timedOutValues)
                    {
                        try
                        {
                            item.FaultCallback.SafeInvoke(
                                new TLRPCError
                                {
                                    Code = new TLInt((int)ErrorCode.TIMEOUT),
                                    Message = new TLString("MTProtoService: operation timed out (" + timeout + "s)")
                                });
#if DEBUG
                            TLUtils.WriteLine(item.Caption + " time out", LogSeverity.Error);
#endif
                        }
                        catch (Exception ex)
                        {
                            TLUtils.WriteException("Timeout exception", ex);
                        }
                    }
                });
            }

            // delayed history
            var timedOutItems = new List<DelayedItem>();
            lock (_delayedItemsRoot)
            {
                foreach (var item in _delayedItems)
                {
                    if (item.SendTime != default(DateTime)
                        && item.SendTime.AddSeconds(delayedTimeout) < now)
                    {
                        timedOutItems.Add(item);
                    }
                }

                foreach (var item in timedOutItems)
                {
                    _delayedItems.Remove(item);
                }
            }

            if (timedOutItems.Count > 0)
            {
                Execute.BeginOnThreadPool(() =>
                {
                    foreach (var item in timedOutItems)
                    {
                        try
                        {
                            item.FaultCallback.SafeInvoke(
                                new TLRPCError
                                {
                                    Code = new TLInt((int)ErrorCode.TIMEOUT),
                                    Message = new TLString("MTProtoService: operation timed out (" + delayedTimeout + "s)")
                                });
#if DEBUG
                            TLUtils.WriteLine(item.Caption + " time out", LogSeverity.Error);
#endif
                        }
                        catch (Exception ex)
                        {
                            TLUtils.WriteException("Timeout exception", ex);
                        }
                    }
                });
            }

            // generating key
            if (_activeTransport != null)
            {
                var requests = _activeTransport.RemoveTimeOutRequests(nonEncryptedTimeout);

                if (requests.Count > 0)
                {
#if LOG_REGISTRATION
                    TLUtils.WriteLog("MTProtoService.CheckTimeouts clear history and replace transport");
#endif
                    ClearHistory("CheckTimeouts", false);

                    Execute.BeginOnThreadPool(() =>
                    {
                        foreach (var item in requests)
                        {
                            try
                            {
                                item.FaultCallback.SafeInvoke(
                                    new TLRPCError
                                    {
                                        Code = new TLInt((int)ErrorCode.TIMEOUT),
                                        Message = new TLString("MTProtoService: operation timed out (" + timeout + "s)")
                                    });
#if DEBUG
                                TLUtils.WriteLine(item.Caption + " time out", LogSeverity.Error);
#endif
                            }
                            catch (Exception ex)
                            {
                                TLUtils.WriteException("Timeout exception", ex);
                            }
                        }
                    });
                }
            }
        }

        private void OnConnectionLost(object sender, EventArgs e)
        {
            ResetConnection(null);
        }

        private void OnFileConnectionLost(object sender, EventArgs e)
        {
            ResetConnection(null);
        }

        private void OnSpecialConnectionLost(object sender, TransportEventArgs e)
        {
            var transport = e.Transport;
            if (transport != null)
            {
                var str = string.Format("Connection lost id={0} dc_id={1} ip={2} port={3}", transport.Id, transport.DCId, transport.Host, transport.Port);
                LogPublicConfig(str);
                Execute.ShowDebugMessage(str);
            }
        }

        public void GetConfigInformationAsync(Action<string> callback)
        {
            Execute.BeginOnThreadPool(() =>
            {
                var now = DateTime.Now;
                var currentTime = TLUtils.DateToUniversalTimeTLInt(ClientTicksDelta, now);

                var activeTransportString = _activeTransport != null ? _activeTransport.ToString() : null;

                var sb = new StringBuilder();
                sb.AppendLine("current_time utc0:");
                sb.AppendLine(string.Format("{0} {1}", currentTime, TLUtils.ToDateTime(currentTime).ToUniversalTime().ToString("HH:mm:ss.fff dd-MM-yyyy")));
                sb.AppendLine("config:");
                sb.AppendLine(_config.ToString());
                sb.AppendLine("active transport:");
                sb.AppendLine(activeTransportString);
                sb.AppendLine("dc_options:");
                foreach (var option in _config.DCOptions)
                {
                    sb.AppendLine(option.ToString());
                }

                callback(sb.ToString());
            });
        }

        public void GetTransportInformationAsync(Action<string> callback)
        {
            Execute.BeginOnThreadPool(() =>
            {
                var activeTransportString = _activeTransport != null ? _activeTransport.ToString() : null;

                var sb = new StringBuilder();
                sb.AppendLine("active transport:");
                sb.AppendLine(activeTransportString);
                sb.AppendLine("Date: " + TLUtils.ToDateTime(_config.Date));
                callback(sb.ToString());
            });
        }

        public void UpdateTransportInfoAsync(TLDCOption78 dcOption, TLString ipAddress, TLInt port, Action<bool> callback)
        {
            var args = new DCOptionsUpdatedEventArgs
            {
                Update = new TLUpdateDCOptions { DCOptions = new TLVector<TLDCOption> { dcOption } }
            };

            OnDCOptionsUpdated(this, args);

            ClearHistory("UpdateTransportInfoAsync", false);

            // continue listening on fault
            _activeTransport = GetTransport(ipAddress.ToString(), port.Value, Type,
                new TransportSettings
                {
                    DcId = dcOption.Id.Value,
                    Secret = TLUtils.ParseSecret(dcOption),
                    AuthKey = _activeTransport != null ? _activeTransport.AuthKey : null,
                    Salt = _activeTransport != null ? _activeTransport.Salt : null,
                    SessionId = _activeTransport != null ? _activeTransport.SessionId : null,
                    MessageIdDict = _activeTransport != null ? _activeTransport.MessageIdDict : new Dictionary<long, long>(),
                    SequenceNumber = _activeTransport != null ? _activeTransport.SequenceNumber : 0,
                    ClientTicksDelta = _activeTransport != null ? _activeTransport.ClientTicksDelta : 0,
                    PacketReceivedHandler = OnPacketReceived
                });

            callback.SafeInvoke(true);
        }


        private void OnDCOptionsUpdated(object sender, DCOptionsUpdatedEventArgs e)
        {
            var newOptions = e.Update.DCOptions;

            var dcOptionsInfo = new StringBuilder();
            dcOptionsInfo.AppendLine("TLUpdateDCOptions");
            foreach (var option in newOptions)
            {
                dcOptionsInfo.AppendLine(string.Format("DCId={0} {1}:{2}", option.Id, option.IpAddress, option.Port));
            }
            Execute.ShowDebugMessage(dcOptionsInfo.ToString());

            if (_config != null && _config.DCOptions != null)
            {
                var activeDCId = _config.DCOptions[_config.ActiveDCOptionIndex].Id.Value;

                foreach (var newOption in newOptions)
                {
                    var updated = false;
                    // 1) update ip address, port, hostname
                    foreach (var oldOption in _config.DCOptions)
                    {
                        if (newOption.Id.Value == oldOption.Id.Value
                            && newOption.IPv6.Value == oldOption.IPv6.Value
                            && newOption.Media.Value == oldOption.Media.Value
                            && newOption.TCPO.Value == oldOption.TCPO.Value
                            && newOption.Static.Value == oldOption.Static.Value)
                        {
                            // keep AuthKey, SessionId and Salt
                            var oldOption78 = oldOption as TLDCOption78;
                            var newOption78 = newOption as TLDCOption78;
                            if (oldOption78 != null
                                && newOption78 != null)
                            {
                                if (oldOption78.Secret != null && newOption78.Secret != null
                                    || oldOption78.Secret == null && newOption78.Secret == null)
                                {
                                    oldOption.Hostname = newOption.Hostname;
                                    oldOption.IpAddress = newOption.IpAddress;
                                    oldOption.Port = newOption.Port;
                                    oldOption78.Secret = newOption78.Secret;
                                    updated = true;

                                    if (!newOption.Media.Value && activeDCId == newOption78.Id.Value)
                                    {
                                        _config.ActiveDCOptionIndex = _config.DCOptions.IndexOf(oldOption);
                                    }
                                    break;
                                }
                            }
                            else
                            {
                                oldOption.Hostname = newOption.Hostname;
                                oldOption.IpAddress = newOption.IpAddress;
                                oldOption.Port = newOption.Port;
                                updated = true;

                                if (!newOption.Media.Value && activeDCId == newOption.Id.Value)
                                {
                                    _config.ActiveDCOptionIndex = _config.DCOptions.IndexOf(oldOption);
                                }
                                break;
                            }
                        }
                    }

                    // 2) append new DCOption
                    if (!updated)
                    {
                        // fix readonly array of dcOption
                        var list = _config.DCOptions.ToList();
                        list.Add(newOption);
                        _config.DCOptions.Items = list;

                        if (!newOption.Media.Value && activeDCId == newOption.Id.Value)
                        {
                            _config.ActiveDCOptionIndex = _config.DCOptions.IndexOf(newOption);
                        }
                    }
                }
                SaveConfig();
            }
        }

        private TLInt GetCurrentUserId()
        {
            return CurrentUserId;
        }

        private void OnPacketReceived(object sender, DataEventArgs e)
        {
            var transport = (ITransport)sender;
#if DEBUG
            bool byActiveTransport;
            lock (_activeTransportRoot)
            {
                byActiveTransport = transport == _activeTransport;
            }
            if (byActiveTransport)
            {
                var transportId = transport.Id;
                var sessionId = transport.SessionId;
                var authKey = transport.AuthKey;
                int historyCount;
                string historyDescription;
                lock (_historyRoot)
                {
                    historyCount = _history.Count;
                    historyDescription = string.Join("\n", _history.Values.Select(x => x.Caption + " " + x.Hash));
                }

                RaiseTransportChecked(new TransportCheckedEventArgs
                {
                    HistoryCount = historyCount,
                    HistoryDescription = historyDescription,

                    TransportId = transportId,
                    SessionId = sessionId,
                    AuthKey = authKey,
                    LastReceiveTime = e.LastReceiveTime,
                    NextPacketLength = e.NextPacketLength,
                    LastPacketLength = e.Data.Length
                });
            }
#endif

            var position = 0;
            var handled = false;

            if (transport.AuthKey == null)
            {
                try
                {

                    var message = TLObject.GetObject<TLNonEncryptedMessage>(e.Data, ref position);
                    var historyItem = transport.DequeueFirstNonEncryptedItem();
                    if (historyItem != null)
                    {
#if LOG_REGISTRATION
                        TLUtils.WriteLog(
                            string.Format("OnReceivedBytes by {0} AuthKey==null: invoke historyItem {1} with result {2} (data length={3})",
                                transport.Id, historyItem.Caption, message.Data.GetType(), e.Data.Length));
#endif
                        historyItem.Callback.SafeInvoke(message.Data);
                    }
                    else
                    {
#if LOG_REGISTRATION
                        TLUtils.WriteLog(
                            string.Format("OnReceivedBytes by {0} AuthKey==null: cannot find historyItem {1} with result {2} (data length={3})",
                                transport.Id, string.Empty, message.Data.GetType(), e.Data.Length));
#endif
                    }

                    handled = true;
                }
                catch (Exception ex)
                {
#if LOG_REGISTRATION

                    TLUtils.WriteLog(
                            string.Format("OnReceivedBytes by {0} AuthKey==null exception: cannot parse TLNonEncryptedMessage with History\n {1} \nand exception\n{2} (data length={3})",
                                transport.Id, transport.PrintNonEncryptedHistory(), ex, e.Data.Length));
#endif
                }

                if (!handled)
                {
#if LOG_REGISTRATION
                    TLUtils.WriteLog(
                            string.Format("OnReceivedBytes by {0} AuthKey==null !handled: invoke ReceiveBytesAsync with data length {1}",
                                transport.Id, e.Data.Length));
#endif
                    ReceiveBytesAsync(transport, e.Data);
                }
            }
            else
            {
#if LOG_REGISTRATION
                TLUtils.WriteLog(
                            string.Format("OnReceivedBytes by {0} AuthKey!=null: invoke ReceiveBytesAsync with data length {1}",
                                transport.Id, e.Data.Length));
#endif
                ReceiveBytesAsync(transport, e.Data);
            }
        }

        private void OnServiceInitializationFailed(object sender, EventArgs e)
        {
            Execute.BeginOnThreadPool(TimeSpan.FromSeconds(1.0), () =>
            {
#if LOG_REGISTRATION
                TLUtils.WriteLog("Service initialization failed");
#endif
                CancelDelayedItemsAsync();

                // если генерация ключа прошла успешно, но предыдущая попытка завершилась неудачно на методах help.getNearestDc, help.getConfig
                // обнуляем ключ, т.к. в противном случае resPQ будем пытаться расшифровать ключем

                lock (_activeTransportRoot)
                {
                    _activeTransport.AuthKey = null;
                }

                Initialize();
            });
        }

        private void CancelDelayedItemsAsync(bool force = false)
        {
            Execute.BeginOnThreadPool(() =>
            {
#if LOG_REGISTRATION
                TLUtils.WriteLog("Cancel delayed items");
#endif
                lock (_delayedItemsRoot)
                {
                    var canceledItems = new List<DelayedItem>();
                    foreach (var item in _delayedItems)
                    {
                        if (force
                            || (item.MaxAttempt.HasValue
                                && item.MaxAttempt <= item.CurrentAttempt))
                        {
                            item.AttemptFailed.SafeInvoke(item.CurrentAttempt);
                            canceledItems.Add(item);
                        }
                        item.CurrentAttempt++;
                    }

                    foreach (var canceledItem in canceledItems)
                    {
#if LOG_REGISTRATION
                        TLUtils.WriteLog("Cancel delayed item\n " + canceledItem);
#endif
                        _delayedItems.Remove(canceledItem);

                        if (canceledItem.FaultCallback != null)
                        {
                            canceledItem.FaultCallback(new TLRPCError { Code = new TLInt(404) });
                        }
                    }
                }
            });
        }

        private void OnServiceInitialized(object sender, EventArgs e)
        {
#if LOG_REGISTRATION
            TLUtils.WriteLog("Service initialized");
#endif
            if (Constants.IsLongPollEnabled)
            {
                StartLongPoll();
            }

            SendDelayedItemsAsync();
        }

        private void SendDelayedItemsAsync()
        {
            Execute.BeginOnThreadPool(() =>
            {
#if LOG_REGISTRATION
                TLUtils.WriteLog("Send delayed items (count=" + _delayedItems.Count + ")");
#endif
                lock (_delayedItemsRoot)
                {
                    if (_delayedItems.Count > 0)
                    {
                        foreach (var item in _delayedItems)
                        {
#if LOG_REGISTRATION
                            TLUtils.WriteLog("Dequeue and send delayed item \n" + item);
#endif
                            SendInformativeMessage(item.Caption, item.Object, item.Callback, item.FaultCallback, item.MaxAttempt, item.AttemptFailed);
                        }

                        _delayedItems.Clear();
                    }
                }

                lock (_delayedNonInformativeItemsRoot)
                {
                    if (_delayedNonInformativeItems.Count > 0)
                    {
                        foreach (var item in _delayedNonInformativeItems)
                        {
#if LOG_REGISTRATION
                            TLUtils.WriteLog("Dequeue and send delayed item \n" + item);
#endif
                            SendNonInformativeMessage(item.Caption, item.Object, item.Callback, item.FaultCallback);
                        }

                        _delayedNonInformativeItems.Clear();
                    }
                }
            });
        }

        private bool _initializeInvoked;

        public void StartInitialize()
        {
            if (_initializeInvoked) return;

            _initializeInvoked = true;

            Initialize();
        }

        public void Initialize()
        {
            Execute.BeginOnThreadPool(() =>
            {
                try
                {
                    TryReadConfig(
                        result =>
                        {
#if LOG_REGISTRATION
                            TLUtils.WriteLog("Read config with result: " + result);
#endif
                            if (!result)
                            {
#if LOG_REGISTRATION
                                TLUtils.WriteLog("TLUtils.LogRegistration=true");
                                TLUtils.IsLogEnabled = true;
#endif

                                var host = _activeTransport != null ? _activeTransport.Host : Constants.FirstServerIpAddress;  // !IMPORTANT host can be obtained through publig config
                                var port = _activeTransport != null ? _activeTransport.Port : Constants.FirstServerPort;       // !IMPORTANT port can be obtained through publig config
                                var dcId = _activeTransport != null ? _activeTransport.DCId : Constants.FirstServerDCId;       // !IMPORTANT dcId can be obtained through publig config
                                _activeTransport = GetTransport(host, port, Type,
                                    new TransportSettings
                                    {
                                        DcId = dcId,
                                        Secret = _activeTransport != null ? _activeTransport.Secret : null,
                                        AuthKey = _activeTransport != null ? _activeTransport.AuthKey : null,
                                        Salt = _activeTransport != null ? _activeTransport.Salt : null,
                                        SessionId = _activeTransport != null ? _activeTransport.SessionId : null,
                                        MessageIdDict = _activeTransport != null ? _activeTransport.MessageIdDict : new Dictionary<long, long>(),
                                        SequenceNumber = _activeTransport != null ? _activeTransport.SequenceNumber : 0,
                                        ClientTicksDelta = _activeTransport != null ? _activeTransport.ClientTicksDelta : 0,
                                        PacketReceivedHandler = OnPacketReceived
                                    });
#if LOG_REGISTRATION
                                TLUtils.WriteLog("Start generating auth key");
#endif
                                InitAsync(tuple =>
                                {
#if LOG_REGISTRATION
                                    TLUtils.WriteLog("Stop generating auth key");
                                    TLUtils.WriteLog("Start help.getNearestDc");
#endif
                                    lock (_activeTransportRoot)
                                    {
                                        _activeTransport.DCId = Constants.FirstServerDCId;
                                        _activeTransport.AuthKey = tuple.Item1;
                                        _activeTransport.Salt = tuple.Item2;
                                        _activeTransport.SessionId = tuple.Item3;
                                    }
                                    var authKeyId = TLUtils.GenerateLongAuthKeyId(tuple.Item1);

                                    lock (_authKeysRoot)
                                    {
                                        if (!_authKeys.ContainsKey(authKeyId))
                                        {
                                            _authKeys.Add(authKeyId, new AuthKeyItem { AuthKey = tuple.Item1, AutkKeyId = authKeyId });
                                        }
                                    }
                                    //IsInitialized = true;   // Важно, используется тут, чтобы OnPacketReceived не пытался рассматривать ответ как NonEncryptedMessage


                                    var timer = Stopwatch.StartNew();
                                    GetNearestDCAsync(nearestDC =>
                                    {
#if LOG_REGISTRATION
                                        TLUtils.WriteLog("Stop help.getNearestDc");
                                        TLUtils.WriteLog("Start help.getConfig");
#endif
                                        lock (_activeTransportRoot)
                                        {
                                            _activeTransport.DCId = nearestDC.ThisDC.Value;
                                        }
                                        var elapsed = timer.Elapsed;
                                        var timer2 = Stopwatch.StartNew();
                                        GetConfigAsync(
                                            config =>
                                            {
                                                var elapsed2 = timer2.Elapsed;
                                                var sb = new StringBuilder();
                                                sb.AppendLine("help.getNearestDc " + elapsed.ToString("g"));
                                                sb.AppendLine("help.getConfig " + elapsed2.ToString("g"));
                                                sb.AppendLine("auth time " + _authTimeElapsed.ToString("g"));
                                                Execute.ShowDebugMessage(sb.ToString());
#if LOG_REGISTRATION
                                                TLUtils.WriteLog("Stop help.getConfig");
#endif
                                                config.Country = nearestDC.Country.ToString();

                                                Execute.BeginOnThreadPool(() => RaiseGotUserCountry(config.Country));

                                                _config = TLConfig.Merge(_config, config);
                                                var dcOption = TLUtils.GetDCOption(config, new TLInt(_activeTransport.DCId));

                                                dcOption.AuthKey = _activeTransport.AuthKey;
                                                dcOption.Salt = _activeTransport.Salt;
                                                dcOption.SessionId = _activeTransport.SessionId;

                                                config.ActiveDCOptionIndex = config.DCOptions.IndexOf(dcOption);

                                                _cacheService.SetConfig(config);
                                                RaiseInitialized();
                                            },
                                            error => RaiseInitializationFailed());
                                    },
                                        error => RaiseInitializationFailed());
                                },
                                    error => RaiseInitializationFailed());
                            }
                            else
                            {
                                var configQ = _config;
                                var activeDCOPtionIndex = _config.ActiveDCOptionIndex;


                                var activeDCOption = configQ.DCOptions[activeDCOPtionIndex];
                                var getConfigRequired = activeDCOption.Id == null;
                                // fix to update from 0.1.2.1 to 0.1.2.4
                                // previously Id is not saved for first DC
                                _activeTransport = GetTransport(activeDCOption.IpAddress.ToString(), activeDCOption.Port.Value, Type,
                                    new TransportSettings
                                    {
                                        DcId = getConfigRequired ? 0 : activeDCOption.Id.Value,
                                        Secret = TLUtils.ParseSecret(activeDCOption),
                                        AuthKey = activeDCOption.AuthKey,
                                        Salt = activeDCOption.Salt,
                                        SessionId = TLLong.Random(),
                                        MessageIdDict = new Dictionary<long, long>(),
                                        SequenceNumber = 0,
                                        ClientTicksDelta = activeDCOption.ClientTicksDelta,
                                        PacketReceivedHandler = OnPacketReceived
                                    });

                                lock (_activeTransportRoot)
                                {
                                    _activeTransport.DCId = getConfigRequired ? 0 : activeDCOption.Id.Value;
                                    _activeTransport.AuthKey = activeDCOption.AuthKey;
                                    _activeTransport.Salt = activeDCOption.Salt;
                                    _activeTransport.SessionId = TLLong.Random();
                                    _activeTransport.ClientTicksDelta = activeDCOption.ClientTicksDelta;
                                }

                                //fix for version 0.1.3.12
                                if (activeDCOption.AuthKey == null)
                                {
                                    //clear config and try again 
                                    RaiseAuthorizationRequired(new AuthorizationRequiredEventArgs { MethodName = "Initialize activeDCOption.AuthKey==null", Error = null, AuthKeyId = 0 });
                                    _config = null;
                                    _cacheService.SetConfig(_config);

                                    RaiseInitializationFailed();

                                    return;
                                }

                                var authKeyId = TLUtils.GenerateLongAuthKeyId(activeDCOption.AuthKey);
                                //Log.Write("Use authKey=" + authKeyId);
                                lock (_authKeysRoot)
                                {
                                    if (!_authKeys.ContainsKey(authKeyId))
                                    {
                                        _authKeys.Add(authKeyId, new AuthKeyItem { AuthKey = activeDCOption.AuthKey, AutkKeyId = authKeyId });
                                    }
                                }

                                if (getConfigRequired)
                                {
                                    var timer = Stopwatch.StartNew();
                                    GetNearestDCAsync(nearestDC =>
                                    {
#if LOG_REGISTRATION
                                        TLUtils.WriteLog("Stop help.getNearestDc");
                                        TLUtils.WriteLog("Start help.getConfig");
#endif
                                        lock (_activeTransportRoot)
                                        {
                                            _activeTransport.DCId = nearestDC.ThisDC.Value;
                                        }
                                        var elapsed = timer.Elapsed;
                                        var timer2 = Stopwatch.StartNew();
                                        GetConfigAsync(
                                            config =>
                                            {
                                                var elapsed2 = timer2.Elapsed;
                                                var sb = new StringBuilder();
                                                sb.AppendLine("help.getNearestDc: " + elapsed);
                                                sb.AppendLine("help.getConfig: " + elapsed2);

                                                Execute.ShowDebugMessage(sb.ToString());
#if LOG_REGISTRATION
                                                TLUtils.WriteLog("Stop help.getConfig");
#endif
                                                config.Country = nearestDC.Country.ToString();
                                                _config = TLConfig.Merge(_config, config);
                                                var dcOption = TLUtils.GetDCOption(config, new TLInt(_activeTransport.DCId));

                                                dcOption.AuthKey = _activeTransport.AuthKey;
                                                dcOption.Salt = _activeTransport.Salt;
                                                dcOption.SessionId = _activeTransport.SessionId;

                                                config.ActiveDCOptionIndex = config.DCOptions.IndexOf(dcOption);
                                                _cacheService.SetConfig(config);
                                                RaiseInitialized();
                                            },
                                            error => RaiseInitializationFailed());
                                    },
                                        error => RaiseInitializationFailed());
                                }
                                else
                                {
                                    RaiseInitialized();
                                }

                            }
                        });
                }
                catch (Exception e)
                {
                    TLUtils.WriteException(e);
                    RaiseInitializationFailed();
                }
            });
        }

        private void TryReadConfig(Action<bool> callback)
        {
            _cacheService.GetConfigAsync(
                config =>
                {
                    _config = config;
                    if (_config == null)
                    {
                        callback(false);
                        return;
                    }

                    callback(true);
                });
        }

        private void SendAcknowledgments(TLTransportMessage response)
        {
            var ids = new TLVector<TLLong>();

            if (response.SeqNo.Value % 2 == 1)
            {
                ids.Items.Add(response.MessageId);
            }
            if (response.MessageData is TLContainer)
            {
                var container = (TLContainer)response.MessageData;
                foreach (var message in container.Messages)
                {
                    if (message.SeqNo.Value % 2 == 1)
                    {
                        ids.Items.Add(message.MessageId);
                    }
                }
            }

            if (ids.Items.Count > 0)
            {
                MessageAcknowledgments(ids);
            }
        }

        private void SendAcknowledgmentsByTransport(ITransport transport, TLTransportMessage response)
        {
            var ids = new TLVector<TLLong>();

            if (response.SeqNo.Value % 2 == 1)
            {
                ids.Items.Add(response.MessageId);
            }
            if (response.MessageData is TLContainer)
            {
                var container = (TLContainer)response.MessageData;
                foreach (var message in container.Messages)
                {
                    if (message.SeqNo.Value % 2 == 1)
                    {
                        ids.Items.Add(message.MessageId);
                    }
                }
            }

            if (ids.Items.Count > 0)
            {
                MessageAcknowledgmentsByTransport(transport, ids);
            }
        }

        public event EventHandler<AuthorizationRequiredEventArgs> AuthorizationRequired;

        public void RaiseAuthorizationRequired(AuthorizationRequiredEventArgs args)
        {
            var handler = AuthorizationRequired;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void ReceiveBytesAsync(ITransport transport, byte[] bytes)
        {
            try
            {
                if (bytes.Length == 4)
                {
                    var error = BitConverter.ToInt32(bytes, 0);
                    if (error == -404 || error == -444)
                    {
                        if (error == -444)
                        {
                            DisableMTProtoProxy();

                            RaiseProxyDisabled();
                        }

                        var message = string.Format("dc_id={0} error={1}", transport.DCId, error);
                        TLUtils.WriteException(new Exception(message));

                        ResetConnection(new Exception(message));
                        return;
                    }
                }

                var position = 0;
                var encryptedMessage = (TLEncryptedTransportMessage)new TLEncryptedTransportMessage().FromBytes(bytes, ref position);

                byte[] authKey2 = null;
                lock (_authKeysRoot)
                {
                    try
                    {
                        authKey2 = _authKeys[encryptedMessage.AuthKeyId.Value].AuthKey;
                    }
                    catch (Exception e)
                    {
                        var error = new StringBuilder();
                        error.AppendLine("bytes_length=" + bytes.Length);
                        error.AppendLine("authKey=" + encryptedMessage.AuthKeyId);
                        error.AppendLine("authKeys");
                        foreach (var authKey in _authKeys)
                        {
                            error.AppendLine(string.Format("{0} {1}", authKey.Key, authKey.Value.AutkKeyId));
                        }

                        TLUtils.WriteException(error.ToString(), e);
                    }
                }

                encryptedMessage.Decrypt(authKey2);

                #region Security checks

                if (encryptedMessage.Data == null)
                {
                    var message = "msgKey mismatch";

                    TLUtils.WriteException(new Exception(message));
                }

                if (encryptedMessage.Data.Length < 32)
                {
                    var message = string.Format("padding extension data={0} < 32", encryptedMessage.Data.Length);

                    TLUtils.WriteException(new Exception(message));
                }
                var messageDataLength = BitConverter.ToInt32(encryptedMessage.Data, 28);
                if (messageDataLength < 0 || messageDataLength % 4 != 0)
                {
                    var message = string.Format("incorrect length data={0} length={1}", encryptedMessage.Data.Length, messageDataLength);

                    TLUtils.WriteException(new Exception(message));
                }

                if (32 + messageDataLength > encryptedMessage.Data.Length)
                {
                    var message = string.Format("padding extension data={0} length={1}", encryptedMessage.Data.Length, messageDataLength);

                    TLUtils.WriteException(new Exception(message));
                }

                position = 0;
                var transportMessage = TLObject.GetObject<TLTransportMessage>(encryptedMessage.Data, ref position);
#if MTPROTO
                if (encryptedMessage.Data.Length - position < 12
                    || encryptedMessage.Data.Length - position > 1024)
                {
                    var message = string.Format("padding extension data={0} position={1} object={2}", encryptedMessage.Data.Length, position, transportMessage.MessageData);
                    TLUtils.WriteException(new Exception(message));
                }
#else
                if ((encryptedMessage.Data.Length - position) > 15)
                {
                    var message = string.Format("padding extension data={0} position={1} object={2}", encryptedMessage.Data.Length, position, transportMessage.MessageData);
                    TLUtils.WriteException(new Exception(message));
                }
#endif
                if (transportMessage.SessionId.Value != transport.SessionId.Value)
                {
                    var message = string.Format("Transport Session_id={0} is not equal to transport.session_id={1} transport_dc_id={2}", transportMessage.SessionId, transport.SessionId, transport.DCId);
                    TLUtils.WriteException(new Exception(message));
                }

                if (transport.MinMessageId != 0)
                {
                    if (transport.MinMessageId > transportMessage.MessageId.Value)
                    {
                        var message = string.Format("Message_id={0} seq_no={1} is higher than transport.min_message_id={2}", transportMessage.MessageId, transportMessage.SeqNo, transport.MinMessageId);
                        TLUtils.WriteException(new Exception(message));
                    }
                }

                if (transport.MessageIdDict.ContainsKey(transportMessage.MessageId.Value))
                {
                    var message = string.Format("Message_id={0} already exists", transportMessage.MessageId.Value);
                    TLUtils.WriteException(new Exception(message));
                }

                lock (transport.SyncRoot)
                {
                    transport.MessageIdDict[transportMessage.MessageId.Value] = transportMessage.MessageId.Value;
                }

                if ((transportMessage.MessageId.Value % 2) == 0)
                {
                    TLUtils.WriteException(new Exception("Incorrect message_id"));
                }

                #endregion

                // get acknowledgments
                foreach (var acknowledgment in TLUtils.FindInnerObjects<TLMessagesAcknowledgment>(transportMessage))
                {
                    var ids = acknowledgment.MessageIds.Items;
                    lock (_historyRoot)
                    {
                        foreach (var id in ids)
                        {
                            if (_history.ContainsKey(id.Value))
                            {
                                _history[id.Value].Status = RequestStatus.Confirmed;
                            }
                        }
                    }
                }
                // send acknowledgments
                SendAcknowledgments(transportMessage);

                // set client ticks delta
                bool updated = false;
                lock (_activeTransportRoot)
                {
                    if (transport.ClientTicksDelta == 0)
                    {
                        var serverTime = transportMessage.MessageId.Value;
                        var clientTime = transport.GenerateMessageId().Value;

                        var serverDateTime = Utils.UnixTimestampToDateTime(serverTime >> 32);
                        var clientDateTime = Utils.UnixTimestampToDateTime(clientTime >> 32);

                        var errorInfo = new StringBuilder();

                        errorInfo.AppendLine("Server time: " + serverDateTime);
                        errorInfo.AppendLine("Client time: " + clientDateTime);

                        transport.ClientTicksDelta = serverTime - clientTime;

                        if (transport.ClientTicksDelta == 0) transport.ClientTicksDelta = 1;
                        updated = true;
                    }
                }

                if (updated && _config != null)
                {
                    var dcOption = _config.DCOptions.FirstOrDefault(x => string.Equals(x.IpAddress.ToString(), transport.Host, StringComparison.OrdinalIgnoreCase));
                    if (dcOption != null)
                    {
                        dcOption.ClientTicksDelta = transport.ClientTicksDelta;
                        _cacheService.SetConfig(_config);
                    }
                }

                // updates
                _updatesService.ProcessTransportMessage(transportMessage);

                // bad messages
                foreach (var badMessage in TLUtils.FindInnerObjects<TLBadMessageNotification>(transportMessage))
                {

                    HistoryItem item = null;
                    lock (_historyRoot)
                    {
                        if (_history.ContainsKey(badMessage.BadMessageId.Value))
                        {
                            item = _history[badMessage.BadMessageId.Value];
                        }
                        else
                        {
                            Execute.ShowDebugMessage("TLBadMessageNotificaiton lost item id=" + badMessage.BadMessageId);
                        }
                    }

                    Logs.Log.Write(string.Format("{0} {1}", badMessage, item));

                    ProcessBadMessage(transportMessage, badMessage, item);
                }

                // bad server salts
                foreach (var badServerSalt in TLUtils.FindInnerObjects<TLBadServerSalt>(transportMessage))
                {

                    lock (_activeTransportRoot)
                    {
                        _activeTransport.Salt = badServerSalt.NewServerSalt;
                    }

                    // save config
                    if (_config != null && _config.DCOptions != null)
                    {
                        var activeDCOption = _config.DCOptions[_config.ActiveDCOptionIndex];
                        activeDCOption.Salt = badServerSalt.NewServerSalt;

                        SaveConfig();
                    }

                    HistoryItem item = null;
                    lock (_historyRoot)
                    {
                        if (_history.ContainsKey(badServerSalt.BadMessageId.Value))
                        {
                            item = _history[badServerSalt.BadMessageId.Value];
                        }
                        else
                        {
                            Execute.ShowDebugMessage("TLBadServerSalt lost item id=" + badServerSalt.BadMessageId);
                        }
                    }

                    Logs.Log.Write(string.Format("{0} {1}", badServerSalt, item));

                    ProcessBadServerSalt(transportMessage, badServerSalt, item);
                }

                // new session created
                foreach (var newSessionCreated in TLUtils.FindInnerObjects<TLNewSessionCreated>(transportMessage))
                {
                    TLUtils.WritePerformance(string.Format("NEW SESSION CREATED: {0} (old {1})", transportMessage.SessionId, _activeTransport.SessionId));
                    lock (_activeTransportRoot)
                    {
                        _activeTransport.SessionId = transportMessage.SessionId;
                        _activeTransport.Salt = newSessionCreated.ServerSalt;
                    }
                }

                foreach (var pong in TLUtils.FindInnerObjects<TLPong>(transportMessage))
                {
                    HistoryItem item;
                    lock (_historyRoot)
                    {
                        if (_history.ContainsKey(pong.MessageId.Value))
                        {
                            item = _history[pong.MessageId.Value];
                            _history.Remove(pong.MessageId.Value);
                        }
                        else
                        {
                            //Execute.ShowDebugMessage("TLPong lost item id=" + pong.MessageId);
                            continue;
                        }
                    }
#if DEBUG
                    NotifyOfPropertyChange(() => History);
#endif

                    if (item != null)
                    {
                        item.Callback.SafeInvoke(pong);
                    }
                }

                // rpcresults
                foreach (var result in TLUtils.FindInnerObjects<TLRPCResult>(transportMessage))
                {
                    HistoryItem historyItem = null;

                    lock (_historyRoot)
                    {
                        if (_history.ContainsKey(result.RequestMessageId.Value))
                        {
                            historyItem = _history[result.RequestMessageId.Value];
                            //#if !DEBUG
                            _history.Remove(result.RequestMessageId.Value);
                            //Debug.WriteLine("History remove msg_id={0} obj={1}", result.RequestMessageId, result.Object);
#if DEBUG
                            _removedHistory[result.RequestMessageId.Value] = new HistoryItem { Caption = historyItem.Caption };
#endif
                            //#endif
                        }
                        else
                        {
#if DEBUG
                            //Debug.WriteLine("History missing msg_id={0} obj={1}", result.RequestMessageId, result.Object);
                            if (_removedHistory.ContainsKey(result.RequestMessageId.Value))
                            {
                                var removedHistoryItem = _removedHistory[result.RequestMessageId.Value];

                                Execute.ShowDebugMessage(string.Format("TLRPCResult LostItem msg_id={0} caption={1} result={2}", result.RequestMessageId, removedHistoryItem != null ? removedHistoryItem.Caption : "null", result.Object));
                            }
                            else
                            {
                                HistoryItem removedHistoryItem = null;

                                Execute.ShowDebugMessage(string.Format("TLRPCResult LostItem msg_id={0} caption={1} result={2}", result.RequestMessageId, removedHistoryItem != null ? removedHistoryItem.Caption : "null", result.Object));
                            }
#endif
                            continue;
                        }
                    }
#if DEBUG
                    NotifyOfPropertyChange(() => History);
#endif

                    var error = result.Object as TLRPCError;
                    if (error != null)
                    {
                        string errorString;
                        var reqError = error as TLRPCReqError;
                        if (reqError != null)
                        {
                            errorString = string.Format("RPCReqError {1} {2} (query_id={0})", reqError.QueryId, reqError.Code, reqError.Message);
                        }
                        else
                        {
                            errorString = string.Format("RPCError {0} {1}", error.Code, error.Message);
                        }

                        Execute.ShowDebugMessage(historyItem + Environment.NewLine + errorString);
                        ProcessRPCError(error, historyItem, encryptedMessage.AuthKeyId.Value);
                        Debug.WriteLine(errorString + " msg_id=" + result.RequestMessageId.Value);
                        TLUtils.WriteLine(errorString);
                    }
                    else
                    {
                        var messageData = result.Object;
                        if (messageData is TLGzipPacked)
                        {
                            messageData = ((TLGzipPacked)messageData).Data;
                        }

                        if (messageData is TLSentMessageBase
                            || messageData is TLStatedMessageBase
                            || messageData is TLUpdatesBase
                            || messageData is TLSentEncryptedMessage
                            || messageData is TLSentEncryptedFile
                            || messageData is TLAffectedHistory
                            || messageData is TLAffectedMessages
                            || historyItem.Object is TLReadChannelHistory
                            || historyItem.Object is TLReadEncryptedHistory)
                        {
                            RemoveFromQueue(historyItem);
                        }

                        if (historyItem.Caption == "messages.getDialogs")
                        {
#if DEBUG_UPDATEDCOPTIONS
                            var dcOption = new TLDCOption
                            {
                                Hostname = new TLString(""),
                                Id = new TLInt(2),
                                IpAddress = new TLString("109.239.131.193"),
                                Port = new TLInt(80)
                            };
                            var dcOptions = new TLVector<TLDCOption> {dcOption};
                            var update = new TLUpdateDCOptions {DCOptions = dcOptions};
                            var updateShort = new TLUpdatesShort {Date = new TLInt(0), Update = update};

                            _updatesService.ProcessTransportMessage(new TLTransportMessage{MessageData = updateShort});
#endif
                        }
                        try
                        {
                            historyItem.Callback(messageData);
                        }
                        catch (Exception e)
                        {
#if LOG_REGISTRATION
                            TLUtils.WriteLog(e.ToString());
#endif
                            TLUtils.WriteException(e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                TLUtils.WriteException("ReceiveBytesAsync", e);

                ResetConnection(e);
            }
        }

        private void ResetConnectionByTransport(ITransport transport, Exception ex)
        {
            ClearHistoryByTransport(transport);

            lock (transport.SyncRoot)
            {
                // continue listening on fault
                transport = GetTransport(transport.Host, transport.Port, Type,
                    new TransportSettings
                    {
                        DcId = transport.DCId,
                        Secret = transport.Secret,
                        AuthKey = transport.AuthKey,
                        Salt = transport.Salt,
                        SessionId = transport.SessionId,
                        MessageIdDict = _activeTransport.MessageIdDict,
                        SequenceNumber = transport.SequenceNumber,
                        ClientTicksDelta = transport.ClientTicksDelta,
                        PacketReceivedHandler = OnPacketReceivedByTransport
                    });
            }
            // to bind authKey to current TCPTransport
            PingByTransportAsync(transport, TLLong.Random(), result => { });
        }

        private void ResetConnection(Exception ex)
        {
            ClearHistory("ResetConnection", false, false, ex);

            // continue listening on fault
            _activeTransport = GetTransport(_activeTransport.Host, _activeTransport.Port, Type,
                new TransportSettings
                {
                    DcId = _activeTransport != null ? _activeTransport.DCId : 0,
                    Secret = _activeTransport != null ? _activeTransport.Secret : null,
                    AuthKey = _activeTransport != null ? _activeTransport.AuthKey : null,
                    Salt = _activeTransport != null ? _activeTransport.Salt : null,
                    SessionId = _activeTransport != null ? _activeTransport.SessionId : null,
                    MessageIdDict = _activeTransport != null ? _activeTransport.MessageIdDict : new Dictionary<long, long>(),
                    SequenceNumber = _activeTransport != null ? _activeTransport.SequenceNumber : 0,
                    ClientTicksDelta = _activeTransport != null ? _activeTransport.ClientTicksDelta : 0,
                    PacketReceivedHandler = OnPacketReceived
                });

            // to bind authKey to current TCPTransport, get changes, etc...
            PingAsync(TLLong.Random(), pong => { });
        }

        public void Stop()
        {
            _transportService.CheckConfig -= OnCheckConfig;
            _transportService.ConnectionLost -= OnConnectionLost;
            _transportService.FileConnectionLost -= OnFileConnectionLost;
            _transportService.SpecialConnectionLost -= OnSpecialConnectionLost;
            _transportService.Close();

            var history = new List<HistoryItem>();
            lock (_historyRoot)
            {
                foreach (var keyValue in _history)
                {
                    history.Add(keyValue.Value);
                }
                _history.Clear();
            }

            foreach (var keyValue in history)
            {
                keyValue.FaultCallback.SafeInvoke(new TLRPCError { Code = new TLInt(404), Message = new TLString("MTProtoService.Stop") });
            }
        }

        public void ClearHistory(string caption, bool createNewSession, bool syncFaultCallbacks = false, Exception e = null)
        {
            var errorDebugString = string.Format("{0} clear history start {1}", DateTime.Now.ToString("HH:mm:ss.fff"), _history.Count);
            TLUtils.WriteLine(errorDebugString, LogSeverity.Error);

            _transportService.Close();
            if (createNewSession)
            {
                _activeTransport.SessionId = TLLong.Random();
                _activeTransport.MessageIdDict = new Dictionary<long, long>();
            }
            // сначала очищаем reqPQ, reqDHParams и setClientDHParams
            _activeTransport.ClearNonEncryptedHistory();

            //затем очищаем help.getNearestDc, help.getConfig и любые другие методы
            //иначе при вызове faultCallback для help.getNearestDc, help.getConfig и reqPQ снова бы завершился с ошибкой
            // при вызове ClearNonEncryptedHistory
            var history = new List<HistoryItem>();
            lock (_historyRoot)
            {
                foreach (var keyValue in _history)
                {
                    history.Add(keyValue.Value);
                }
                _history.Clear();
            }

            if (syncFaultCallbacks)
            {
                foreach (var keyValue in history)
                {
                    keyValue.FaultCallback.SafeInvoke(new TLRPCError { Code = new TLInt(404), Message = new TLString("MTProtoService.ClearHistory " + caption), Exception = e });
                }
            }
            else
            {
                Execute.BeginOnThreadPool(() =>
                {
                    foreach (var keyValue in history)
                    {
                        keyValue.FaultCallback.SafeInvoke(new TLRPCError { Code = new TLInt(404), Message = new TLString("MTProtoService.ClearHistory " + caption), Exception = e });
                    }
                });
            }
        }

        private void ResetConnection(ITransport transport, Exception ex)
        {
            Execute.ShowDebugMessage("ResetConnection dc_id=" + transport.DCId);

            ClearHistory("ResetConnection dc_id=" + transport.DCId, false, false, ex);
        }

        public void ClearHistory(string caption, ITransport transport, Exception e = null)
        {
            var errorDebugString = string.Format("{0} clear history start {1}", DateTime.Now.ToString("HH:mm:ss.fff"), _history.Count);
            TLUtils.WriteLine(errorDebugString, LogSeverity.Error);

            _transportService.CloseTransport(transport);
            // сначала очищаем reqPQ, reqDHParams и setClientDHParams
            transport.ClearNonEncryptedHistory();

            //затем очищаем help.getNearestDc, help.getConfig и любые другие методы
            //иначе при вызове faultCallback для help.getNearestDc, help.getConfig и reqPQ снова бы завершился с ошибкой
            // при вызове ClearNonEncryptedHistory
            var history = new List<HistoryItem>();
            lock (_historyRoot)
            {
                foreach (var keyValue in _history)
                {
                    if (keyValue.Value.DCId == transport.DCId)
                    {
                        history.Add(keyValue.Value);
                    }
                }
                foreach (var historyItem in history)
                {
                    _history.Remove(historyItem.Hash);
                }
            }

            Execute.BeginOnThreadPool(() =>
            {
                foreach (var keyValue in history)
                {
                    keyValue.FaultCallback.SafeInvoke(new TLRPCError { Code = new TLInt(404), Message = new TLString("MTProtoService.ClearHistory " + caption), Exception = e });
                }
            });
        }

        private void ProcessRPCError(TLRPCError error, HistoryItem historyItem, long keyId)
        {
            Log.Write(string.Format("RPCError {0} {1}", historyItem.Caption, error));

#if LOG_REGISTRATION
            TLUtils.WriteLog(string.Format("RPCError {0} {1}", historyItem.Caption, error));
#endif

            if (error.CodeEquals(ErrorCode.UNAUTHORIZED))
            {
                Execute.ShowDebugMessage(string.Format("RPCError {0} {1}", historyItem.Caption, error));

                if (historyItem != null
                    && historyItem.Caption != "account.updateStatus"
                    && historyItem.Caption != "account.registerDevice"
                    && historyItem.Caption != "auth.signIn")
                {
                    //Execute.ShowDebugMessage(string.Format("RPCError {0} {1} (auth required)", historyItem.Caption, error));
                    RaiseAuthorizationRequired(new AuthorizationRequiredEventArgs { Error = error, MethodName = historyItem.Caption, AuthKeyId = keyId });
                }
                else if (historyItem != null && historyItem.FaultCallback != null)
                {
                    historyItem.FaultCallback(error);
                }
            }
            else if (error.CodeEquals(ErrorCode.ERROR_SEE_OTHER)
                && (error.TypeStarsWith(ErrorType.NETWORK_MIGRATE)
                    || error.TypeStarsWith(ErrorType.PHONE_MIGRATE)
                //|| error.TypeStarsWith(ErrorType.FILE_MIGRATE)
                    ))
            {

                var serverNumber = Convert.ToInt32(
                    error.GetErrorTypeString()
                    .Replace(ErrorType.NETWORK_MIGRATE.ToString(), string.Empty)
                    .Replace(ErrorType.PHONE_MIGRATE.ToString(), string.Empty)
                    //.Replace(ErrorType.FILE_MIGRATE.ToString(), string.Empty)
                    .Replace("_", string.Empty));

                if (_config == null
                    || TLUtils.GetDCOption(_config, new TLInt(serverNumber)) == null)
                {
                    GetConfigAsync(config =>
                    {
                        // параметры предыдущего подключения не сохраняются, поэтому когда ответ приходит после
                        // подключения к следующему серверу, то не удается расшифровать старые сообщения, пришедшие с 
                        // задержкой с новой солью и authKey
                        _config = TLConfig.Merge(_config, config);
                        SaveConfig();
                        if (historyItem.Object.GetType() == typeof(TLSendCode))
                        {
                            var dcOption = TLUtils.GetDCOption(_config, new TLInt(serverNumber));

                            _activeTransport = GetTransport(dcOption.IpAddress.ToString(), dcOption.Port.Value, Type,
                                new TransportSettings
                                {
                                    DcId = dcOption.Id.Value,
                                    Secret = TLUtils.ParseSecret(dcOption),
                                    AuthKey = dcOption.AuthKey,
                                    Salt = dcOption.Salt,
                                    SessionId = TLLong.Random(),
                                    MessageIdDict = new Dictionary<long, long>(),
                                    SequenceNumber = 0,
                                    ClientTicksDelta = dcOption.ClientTicksDelta,
                                    PacketReceivedHandler = OnPacketReceived
                                });

                            //IsInitialized = false;
                            InitAsync(tuple =>
                            {
                                lock (_activeTransportRoot)
                                {
                                    _activeTransport.DCId = serverNumber;
                                    _activeTransport.AuthKey = tuple.Item1;
                                    _activeTransport.Salt = tuple.Item2;
                                    _activeTransport.SessionId = tuple.Item3;
                                }
                                var authKeyId = TLUtils.GenerateLongAuthKeyId(tuple.Item1);

                                lock (_authKeysRoot)
                                {
                                    if (!_authKeys.ContainsKey(authKeyId))
                                    {
                                        _authKeys.Add(authKeyId, new AuthKeyItem { AuthKey = tuple.Item1, AutkKeyId = authKeyId });
                                    }
                                }

                                dcOption.AuthKey = tuple.Item1;
                                dcOption.Salt = tuple.Item2;
                                dcOption.SessionId = tuple.Item3;

                                _config.ActiveDCOptionIndex = _config.DCOptions.IndexOf(dcOption);
                                _cacheService.SetConfig(_config);

                                //IsInitialized = true;
                                RaiseInitialized();

                                SendInformativeMessage(historyItem.Caption, historyItem.Object, historyItem.Callback, historyItem.FaultCallback);
                            },
                            er =>
                            {
                                //restore previous transport
                                var activeDCOption = _config.DCOptions[_config.ActiveDCOptionIndex];
                                _activeTransport = GetTransport(activeDCOption.IpAddress.ToString(), activeDCOption.Port.Value, Type,
                                new TransportSettings
                                {
                                    DcId = activeDCOption.Id.Value,
                                    Secret = TLUtils.ParseSecret(activeDCOption),
                                    AuthKey = activeDCOption.AuthKey,
                                    Salt = activeDCOption.Salt,
                                    SessionId = TLLong.Random(),
                                    MessageIdDict = new Dictionary<long, long>(),
                                    SequenceNumber = 0,
                                    ClientTicksDelta = activeDCOption.ClientTicksDelta,
                                    PacketReceivedHandler = OnPacketReceived
                                });
#if LOG_REGISTRATION
                                TLUtils.WriteLog(string.Format("RPCError restore transport {0} {1}:{2} item {3}", _activeTransport.Id, _activeTransport.Host, _activeTransport.Port, historyItem.Caption));
#endif

                                historyItem.FaultCallback.SafeInvoke(er);
                            });
                        }
                        else
                        {
                            MigrateAsync(serverNumber, auth => SendInformativeMessage(historyItem.Caption, historyItem.Object, historyItem.Callback, historyItem.FaultCallback));
                        }
                    });
                }
                else
                {
                    if (historyItem.Object.GetType() == typeof(TLSendCode)
                        || historyItem.Object.GetType() == typeof(TLGetFile))
                    {
                        var activeDCOption = TLUtils.GetDCOption(_config, new TLInt(serverNumber));

                        _activeTransport = GetTransport(activeDCOption.IpAddress.ToString(), activeDCOption.Port.Value, Type,
                            new TransportSettings
                            {
                                DcId = activeDCOption.Id.Value,
                                Secret = TLUtils.ParseSecret(activeDCOption),
                                AuthKey = activeDCOption.AuthKey,
                                Salt = activeDCOption.Salt,
                                SessionId = TLLong.Random(),
                                MessageIdDict = new Dictionary<long, long>(),
                                SequenceNumber = 0,
                                ClientTicksDelta = activeDCOption.ClientTicksDelta,
                                PacketReceivedHandler = OnPacketReceived
                            });

                        if (activeDCOption.AuthKey == null)
                        {
                            //IsInitialized = false;
                            InitAsync(tuple =>
                            {
                                lock (_activeTransportRoot)
                                {
                                    _activeTransport.DCId = serverNumber;
                                    _activeTransport.AuthKey = tuple.Item1;
                                    _activeTransport.Salt = tuple.Item2;
                                    _activeTransport.SessionId = tuple.Item3;
                                }

                                var authKeyId = TLUtils.GenerateLongAuthKeyId(tuple.Item1);

                                lock (_authKeysRoot)
                                {
                                    if (!_authKeys.ContainsKey(authKeyId))
                                    {
                                        _authKeys.Add(authKeyId, new AuthKeyItem { AuthKey = tuple.Item1, AutkKeyId = authKeyId });
                                    }
                                }

                                activeDCOption.AuthKey = tuple.Item1;
                                activeDCOption.Salt = tuple.Item2;
                                activeDCOption.SessionId = tuple.Item3;

                                _config.ActiveDCOptionIndex = _config.DCOptions.IndexOf(activeDCOption);
                                _cacheService.SetConfig(_config);

                                //IsInitialized = true;
                                RaiseInitialized();

                                SendInformativeMessage(historyItem.Caption, historyItem.Object, historyItem.Callback, historyItem.FaultCallback);
                            },
                            er =>
                            {
                                //restore previous transport
                                var activeDCOption2 = _config.DCOptions[_config.ActiveDCOptionIndex];
                                _activeTransport = GetTransport(activeDCOption2.IpAddress.ToString(), activeDCOption2.Port.Value, Type,
                                    new TransportSettings
                                    {
                                        DcId = activeDCOption2.Id.Value,
                                        Secret = TLUtils.ParseSecret(activeDCOption2),
                                        AuthKey = activeDCOption2.AuthKey,
                                        Salt = activeDCOption2.Salt,
                                        SessionId = TLLong.Random(),
                                        MessageIdDict = new Dictionary<long, long>(),
                                        SequenceNumber = 0,
                                        ClientTicksDelta = activeDCOption2.ClientTicksDelta,
                                        PacketReceivedHandler = OnPacketReceived
                                    });
#if LOG_REGISTRATION
                                TLUtils.WriteLog(string.Format("RPCError restore transport {0} {1}:{2} item {3}", _activeTransport.Id, _activeTransport.Host, _activeTransport.Port, historyItem.Caption));
#endif

                                historyItem.FaultCallback.SafeInvoke(er);
                            });
                        }
                        else
                        {
                            lock (_activeTransportRoot)
                            {
                                _activeTransport.AuthKey = activeDCOption.AuthKey;
                                _activeTransport.Salt = activeDCOption.Salt;
                                _activeTransport.SessionId = TLLong.Random();
                            }
                            var authKeyId = TLUtils.GenerateLongAuthKeyId(activeDCOption.AuthKey);

                            lock (_authKeysRoot)
                            {
                                if (!_authKeys.ContainsKey(authKeyId))
                                {
                                    _authKeys.Add(authKeyId, new AuthKeyItem { AuthKey = activeDCOption.AuthKey, AutkKeyId = authKeyId });
                                }
                            }

                            _config.ActiveDCOptionIndex = _config.DCOptions.IndexOf(activeDCOption);
                            _cacheService.SetConfig(_config);

                            //IsInitialized = true;
                            RaiseInitialized();

                            SendInformativeMessage(historyItem.Caption, historyItem.Object, historyItem.Callback, historyItem.FaultCallback);
                        }
                    }
                    else
                    {
                        MigrateAsync(serverNumber, auth => SendInformativeMessage(historyItem.Caption, historyItem.Object, historyItem.Callback, historyItem.FaultCallback));
                    }
                }
            }
            else if (error.CodeEquals(ErrorCode.ERROR_SEE_OTHER)
                && error.TypeStarsWith(ErrorType.USER_MIGRATE))
            {
                //return;

                var serverNumber = Convert.ToInt32(
                    error.GetErrorTypeString()
                    .Replace(ErrorType.USER_MIGRATE.ToString(), string.Empty)
                    .Replace("_", string.Empty));

                // фикс версии 0.1.3.13 когда первый конфиг для dc2 отличался от стартового dc2
                // можно убрать после
                if (_config.ActiveDCOptionIndex == 0 && serverNumber == 2)
                {
                    var activeDCOption = TLUtils.GetDCOption(_config, new TLInt(serverNumber));

                    _activeTransport = GetTransport(activeDCOption.IpAddress.ToString(), activeDCOption.Port.Value, Type,
                        new TransportSettings
                        {
                            DcId = activeDCOption.Id.Value,
                            Secret = TLUtils.ParseSecret(activeDCOption),
                            AuthKey = activeDCOption.AuthKey,
                            Salt = activeDCOption.Salt,
                            SessionId = TLLong.Random(),
                            MessageIdDict = new Dictionary<long, long>(),
                            SequenceNumber = 0,
                            ClientTicksDelta = activeDCOption.ClientTicksDelta,
                            PacketReceivedHandler = OnPacketReceived
                        });

                    var authKeyId = TLUtils.GenerateLongAuthKeyId(activeDCOption.AuthKey);

                    lock (_authKeysRoot)
                    {
                        if (!_authKeys.ContainsKey(authKeyId))
                        {
                            _authKeys.Add(authKeyId, new AuthKeyItem { AuthKey = activeDCOption.AuthKey, AutkKeyId = authKeyId });
                        }
                    }

                    _config.ActiveDCOptionIndex = _config.DCOptions.IndexOf(activeDCOption);
                    _cacheService.SetConfig(_config);

                    SendInformativeMessage(historyItem.Caption, historyItem.Object, historyItem.Callback, historyItem.FaultCallback);
                }
                // конец фикса

                //ITransport newTransport;
                //TLDCOption newActiveDCOption;
                //lock (_activeTransportRoot)
                //{
                //    newActiveDCOption = _config.DCOptions.First(x => x.IsValidIPv4Option(new TLInt(serverNumber)));

                //    var transportClientsTicksDelta = newActiveDCOption.ClientTicksDelta;
                //    bool isCreated;
                //    newTransport = _transportService.GetTransport(newActiveDCOption.IpAddress.ToString(), newActiveDCOption.Port.Value, Type, out isCreated);
                //    newTransport.ClientTicksDelta = transportClientsTicksDelta;
                //    newTransport.PacketReceived += OnPacketReceived;
                //}

                //if (newTransport.AuthKey == null)
                //{
                //    InitTransportAsync(newTransport,
                //        tuple =>
                //        {
                //            lock (newTransport.SyncRoot)
                //            {
                //                newTransport.AuthKey = tuple.Item1;
                //                newTransport.Salt = tuple.Item2;
                //                newTransport.SessionId = tuple.Item3;

                //                newTransport.IsInitializing = false;
                //            }
                //            var authKeyId = TLUtils.GenerateLongAuthKeyId(tuple.Item1);

                //            lock (_authKeysRoot)
                //            {
                //                if (!_authKeys.ContainsKey(authKeyId))
                //                {
                //                    _authKeys.Add(authKeyId, new AuthKeyItem {AuthKey = tuple.Item1, AutkKeyId = authKeyId});
                //                }
                //            }

                //            foreach (var dcOption in _config.DCOptions)
                //            {
                //                if (dcOption.Id.Value == newTransport.Id)
                //                {
                //                    dcOption.AuthKey = tuple.Item1;
                //                    dcOption.Salt = tuple.Item2;
                //                    dcOption.SessionId = tuple.Item3;
                //                }
                //            }

                //            _cacheService.SetConfig(_config);

                //            ExportImportAuthorizationAsync(
                //                newTransport,
                //                () =>
                //                {
                //                    lock (_activeTransportRoot)
                //                    {
                //                        _activeTransport = newTransport;
                //                    }

                //                    _config.ActiveDCOptionIndex = _config.DCOptions.IndexOf(newActiveDCOption);
                //                    SaveConfig();

                //                    SendInformativeMessage(historyItem.Caption, historyItem.Object, historyItem.Callback, historyItem.FaultCallback);
                //                },
                //                err =>
                //                {

                //                });
                //        },
                //        er =>
                //        {

                //        });
                //}
                //else
                //{
                //    ExportImportAuthorizationAsync(
                //        newTransport,
                //        () =>
                //        {
                //            lock (_activeTransportRoot)
                //            {
                //                _activeTransport = newTransport;
                //            }

                //            _config.ActiveDCOptionIndex = _config.DCOptions.IndexOf(newActiveDCOption);
                //            SaveConfig();

                //            SendInformativeMessage(historyItem.Caption, historyItem.Object, historyItem.Callback, historyItem.FaultCallback);
                //        },
                //        err =>
                //        {

                //        });
                //}

            }
            else if (historyItem.FaultCallback != null)
            {
                historyItem.FaultCallback(error);
            }
        }

        private void MigrateAsync(int serverNumber, Action<TLAuthorization> callback)
        {
            throw new NotImplementedException();

            //ExportAuthorizationAsync(new TLInt(serverNumber), 
            //    exportedAuthorization =>
            //    {
            //        var dcOption = _config.DCOptions.First(x => x.IsValidIPv4Option(new TLInt(serverNumber)));
            //        _activeTransport.SetAddress(dcOption.IpAddress.ToString(), dcOption.Port.Value);

            //        _isInitialized = false;
            //        NotifyOfPropertyChange(() => IsInitialized);

            //        _authHelper.InitAsync(tuple =>
            //        {
            //            ImportAuthorizationAsync(exportedAuthorization.Id, exportedAuthorization.Bytes, callback);
            //            _isInitialized = true;
            //            NotifyOfPropertyChange(() => IsInitialized);
            //            RaiseInitialized();
            //        });
            //    });
        }

        private void ProcessBadMessage(TLTransportMessage message, TLBadMessageNotification badMessage, HistoryItem historyItem)
        {
            if (historyItem == null) return;

            switch (badMessage.ErrorCode.Value)
            {
                case 16:    // слишком маленький msg_id
                case 17:    // слишком большой msg_id
                    var errorInfo = new StringBuilder();
                    errorInfo.AppendLine("2. CORRECT TIME DELTA for active transport " + _activeTransport.DCId);
                    errorInfo.AppendLine(historyItem.Caption);

                    lock (_historyRoot)
                    {
                        _history.Remove(historyItem.Hash);
                    }
#if DEBUG
                    NotifyOfPropertyChange(() => History);
#endif

                    var saveConfig = false;
                    lock (_activeTransportRoot)
                    {
                        var serverTime = message.MessageId.Value;
                        var clientTime = _activeTransport.GenerateMessageId().Value;

                        var serverDateTime = Utils.UnixTimestampToDateTime(serverTime >> 32);
                        var clientDateTime = Utils.UnixTimestampToDateTime(clientTime >> 32);

                        errorInfo.AppendLine("Server time: " + serverDateTime);
                        errorInfo.AppendLine("Client time: " + clientDateTime);

                        if (historyItem.ClientTicksDelta == _activeTransport.ClientTicksDelta)
                        {
                            saveConfig = true;
                            _activeTransport.ClientTicksDelta += serverTime - clientTime;
                            errorInfo.AppendLine("Set ticks delta: " + _activeTransport.ClientTicksDelta + "(" + (serverDateTime - clientDateTime).TotalSeconds + " seconds)");
                        }
                    }

                    TLUtils.WriteLine(errorInfo.ToString(), LogSeverity.Error);

                    if (saveConfig && _config != null)
                    {
                        var dcOption = _config.DCOptions.FirstOrDefault(x => string.Equals(x.IpAddress.ToString(), _activeTransport.Host, StringComparison.OrdinalIgnoreCase));
                        if (dcOption != null)
                        {
                            dcOption.ClientTicksDelta = _activeTransport.ClientTicksDelta;
                            _cacheService.SetConfig(_config);
                        }
                    }

                    // TODO: replace with SendInformativeMessage


                    var transportMessage = (TLContainerTransportMessage)historyItem.Message;
                    int sequenceNumber;
                    lock (_activeTransportRoot)
                    {
                        if (transportMessage.SeqNo.Value % 2 == 0)
                        {
                            sequenceNumber = 2 * _activeTransport.SequenceNumber;
                        }
                        else
                        {
                            sequenceNumber = 2 * _activeTransport.SequenceNumber + 1;
                            _activeTransport.SequenceNumber++;
                        }

                        transportMessage.SeqNo = new TLInt(sequenceNumber);
                        transportMessage.MessageId = _activeTransport.GenerateMessageId(false);
                    }

                    TLUtils.WriteLine("Corrected client time: " + TLUtils.MessageIdString(transportMessage.MessageId));
                    var authKey = _activeTransport.AuthKey;
                    var encryptedMessage = CreateTLEncryptedMessage(authKey, transportMessage);

                    lock (_historyRoot)
                    {
                        _history[historyItem.Hash] = historyItem;
                    }
                    var faultCallback = historyItem.FaultCallback;
                    lock (_activeTransportRoot)
                    {
                        if (_activeTransport.Closed)
                        {
                            _activeTransport = GetTransport(_activeTransport.Host, _activeTransport.Port, Type,
                                new TransportSettings
                                {
                                    DcId = _activeTransport.DCId,
                                    Secret = _activeTransport.Secret,
                                    AuthKey = _activeTransport.AuthKey,
                                    Salt = _activeTransport.Salt,
                                    SessionId = _activeTransport.SessionId,
                                    MessageIdDict = _activeTransport.MessageIdDict,
                                    SequenceNumber = _activeTransport.SequenceNumber,
                                    ClientTicksDelta = _activeTransport.ClientTicksDelta,
                                    PacketReceivedHandler = OnPacketReceived
                                });
                        }
                    }
                    //Debug.WriteLine(">>{0, -30} MsgId {1} SeqNo {2,-4} SessionId {3} BadMsgId {4}", string.Format("{0}: {1}", historyItem.Caption, "time"), transportMessage.MessageId.Value, transportMessage.SeqNo.Value, message.SessionId.Value, badMessage.BadMessageId.Value);

                    var captionString = string.Format("{0} {1} {2}", historyItem.Caption, message.SessionId, transportMessage.MessageId);
                    SendPacketAsync(_activeTransport, captionString, encryptedMessage,
                        result =>
                        {
                            Debug.WriteLine("@{0} {1} result {2}", string.Format("{0}: {1}", historyItem.Caption, "time"), transportMessage.MessageId.Value, result);

                        },//ReceiveBytesAsync(result, authKey),
                        error =>
                        {
                            lock (_historyRoot)
                            {
                                _history.Remove(historyItem.Hash);
                            }
#if DEBUG
                            NotifyOfPropertyChange(() => History);
#endif
                            faultCallback.SafeInvoke(new TLRPCError { Code = new TLInt(404) });
                        });

                    break;

                case 32:
                case 33:
                    TLUtils.WriteLine(string.Format("ErrorCode={0} INCORRECT MSGSEQNO, CREATE NEW SESSION {1}", badMessage.ErrorCode.Value, historyItem.Caption), LogSeverity.Error);
                    Execute.ShowDebugMessage(string.Format("ErrorCode={0} INCORRECT MSGSEQNO, CREATE NEW SESSION {1}", badMessage.ErrorCode.Value, historyItem.Caption));

                    var previousMessageId = historyItem.Hash;

                    // fix seqNo with creating new Session
                    lock (_activeTransportRoot)
                    {
                        _activeTransport.SessionId = TLLong.Random();
                        _activeTransport.SequenceNumber = 0;
                        transportMessage = (TLTransportMessage)historyItem.Message;
                        if (transportMessage.SeqNo.Value % 2 == 0)
                        {
                            sequenceNumber = 2 * _activeTransport.SequenceNumber;
                        }
                        else
                        {
                            sequenceNumber = 2 * _activeTransport.SequenceNumber + 1;
                            _activeTransport.SequenceNumber++;
                        }

                        transportMessage.SeqNo = new TLInt(sequenceNumber);
                        transportMessage.MessageId = _activeTransport.GenerateMessageId(true);
                    }
                    ((TLTransportMessage)transportMessage).SessionId = _activeTransport.SessionId;


                    // TODO: replace with SendInformativeMessage
                    TLUtils.WriteLine("Corrected client time: " + TLUtils.MessageIdString(transportMessage.MessageId));
                    authKey = _activeTransport.AuthKey;
                    encryptedMessage = CreateTLEncryptedMessage(authKey, transportMessage);

                    lock (_historyRoot)
                    {
                        _history.Remove(previousMessageId);
                        _history[historyItem.Hash] = historyItem;
                    }
                    faultCallback = historyItem.FaultCallback;
                    lock (_activeTransportRoot)
                    {
                        if (_activeTransport.Closed)
                        {
                            _activeTransport = GetTransport(_activeTransport.Host, _activeTransport.Port, Type,
                                new TransportSettings
                                {
                                    DcId = _activeTransport.DCId,
                                    Secret = _activeTransport.Secret,
                                    AuthKey = _activeTransport.AuthKey,
                                    Salt = _activeTransport.Salt,
                                    SessionId = _activeTransport.SessionId,
                                    MessageIdDict = _activeTransport.MessageIdDict,
                                    SequenceNumber = _activeTransport.SequenceNumber,
                                    ClientTicksDelta = _activeTransport.ClientTicksDelta,
                                    PacketReceivedHandler = OnPacketReceived
                                });
                        }
                    }
                    //Debug.WriteLine(">>{0, -30} MsgId {1} SeqNo {2,-4} SessionId {3} BadMsgId {4}", string.Format("{0}: {1}", historyItem.Caption, "seqNo"), transportMessage.MessageId.Value, transportMessage.SeqNo.Value, message.SessionId.Value, badMessage.BadMessageId.Value);

                    captionString = string.Format("{0} {1} {2}", historyItem.Caption, message.SessionId, transportMessage.MessageId);
                    SendPacketAsync(_activeTransport, captionString, encryptedMessage,
                        result =>
                        {
                            Debug.WriteLine("@{0} {1} result {2}", string.Format("{0}: {1}", historyItem.Caption, "seqNo"), transportMessage.MessageId.Value, result);

                        },//ReceiveBytesAsync(result, authKey)}, 
                        error => { if (faultCallback != null) faultCallback(null); });

                    break;
            }
        }



        private void ProcessBadServerSalt(TLTransportMessage message, TLBadServerSalt badServerSalt, HistoryItem historyItem)
        {
            if (historyItem == null)
            {
                return;
            }

            var transportMessage = (TLContainerTransportMessage)historyItem.Message;
            lock (_historyRoot)
            {
                _history.Remove(historyItem.Hash);
            }
#if DEBUG
            NotifyOfPropertyChange(() => History);
#endif

            TLUtils.WriteLine("CORRECT SERVER SALT:");
            ((TLTransportMessage)transportMessage).Salt = badServerSalt.NewServerSalt;
            //Salt = badServerSalt.NewServerSalt;
            TLUtils.WriteLine("New salt: " + _activeTransport.Salt);

            switch (badServerSalt.ErrorCode.Value)
            {
                case 16:
                case 17:
                    TLUtils.WriteLine("3. CORRECT TIME DELTA with Salt by activeTransport " + _activeTransport.DCId);

                    var saveConfig = false;
                    lock (_activeTransportRoot)
                    {
                        var serverTime = message.MessageId.Value;
                        TLUtils.WriteLine("Server time: " + TLUtils.MessageIdString(BitConverter.GetBytes(serverTime)));
                        var clientTime = _activeTransport.GenerateMessageId().Value;
                        TLUtils.WriteLine("Client time: " + TLUtils.MessageIdString(BitConverter.GetBytes(clientTime)));

                        if (historyItem.ClientTicksDelta == _activeTransport.ClientTicksDelta)
                        {
                            saveConfig = true;
                            _activeTransport.ClientTicksDelta += serverTime - clientTime;
                        }

                        transportMessage.MessageId = _activeTransport.GenerateMessageId(true);
                        TLUtils.WriteLine("Corrected client time: " + TLUtils.MessageIdString(transportMessage.MessageId));
                    }

                    if (saveConfig && _config != null)
                    {
                        var dcOption = _config.DCOptions.FirstOrDefault(x => string.Equals(x.IpAddress.ToString(), _activeTransport.Host, StringComparison.OrdinalIgnoreCase));
                        if (dcOption != null)
                        {
                            dcOption.ClientTicksDelta = _activeTransport.ClientTicksDelta;
                            _cacheService.SetConfig(_config);
                        }
                    }

                    break;
                case 48:
                    break;
            }

            if (transportMessage == null) return;

            var authKey = _activeTransport.AuthKey;
            var encryptedMessage = CreateTLEncryptedMessage(authKey, transportMessage);
            lock (_historyRoot)
            {
                _history[historyItem.Hash] = historyItem;
            }
            var faultCallback = historyItem.FaultCallback;
            lock (_activeTransportRoot)
            {
                if (_activeTransport.Closed)
                {
                    _activeTransport = GetTransport(_activeTransport.Host, _activeTransport.Port, Type,
                        new TransportSettings
                        {
                            DcId = _activeTransport.DCId,
                            Secret = _activeTransport.Secret,
                            AuthKey = _activeTransport.AuthKey,
                            Salt = _activeTransport.Salt,
                            SessionId = _activeTransport.SessionId,
                            MessageIdDict = _activeTransport.MessageIdDict,
                            SequenceNumber = _activeTransport.SequenceNumber,
                            ClientTicksDelta = _activeTransport.ClientTicksDelta,
                            PacketReceivedHandler = OnPacketReceived
                        });
                }
            }

            var captionString = string.Format("{0} {1} {2}", historyItem.Caption, message.SessionId, transportMessage.MessageId);
            SendPacketAsync(_activeTransport, captionString, encryptedMessage,
                result =>
                {
                    Debug.WriteLine("@{0} {1} result {2}", historyItem.Caption, transportMessage.MessageId.Value, result);

                },//ReceiveBytesAsync(result, authKey)}, 
                error => { if (faultCallback != null) faultCallback(new TLRPCError { Code = new TLInt(404), Message = new TLString("TCPTransport error") }); });
        }


        private readonly IDisposable _statusSubscription;

        public event EventHandler<SendStatusEventArgs> SendStatus;

        public void RaiseSendStatus(SendStatusEventArgs e)
        {
            var handler = SendStatus;
            if (handler != null) handler(this, e);
        }

        public void Dispose()
        {
            _statusSubscription.Dispose();
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    NotifyOfPropertyChange(() => Message);
                }
            }
        }

        private DispatcherTimer _messageScheduler;

        public void SetMessageOnTime(double seconds, string message)
        {
            //Logs.Log.Write(string.Format("MTProtoService.SetMessageOnTime sec={0}, message={1}", seconds, message));

            Execute.BeginOnUIThread(() =>
            {
                if (_messageScheduler == null)
                {
                    _messageScheduler = new DispatcherTimer();
                    _messageScheduler.Tick += MessageScheduler_Tick;
                }

                _messageScheduler.Stop();
                Message = message;
                _messageScheduler.Interval = TimeSpan.FromSeconds(seconds);
                _messageScheduler.Start();
            });
        }

#if WINDOWS_PHONE
        private void MessageScheduler_Tick(object sender, EventArgs e)
#elif WIN_RT
        private void MessageScheduler_Tick(object sender, object args)
#endif
        {
            Message = string.Empty;
            _messageScheduler.Stop();
        }

        private ITransport GetTransportWithProxyInternal(GetTransportWithProxyFunc getTransportAction, string host, int port, TransportType type, bool checkStatic, TransportSettings settings, TLProxyBase proxy)
        {
            bool isCreated;
            var staticHost = host;
            var staticPort = port;

            // looking for static dcOption with none empty config
            if (checkStatic)
            {
                var staticOption = GetStaticDCOption(settings.DcId);
                if (staticOption != null)
                {
                    staticHost = staticOption.IpAddress.ToString();
                    staticPort = staticOption.Port.Value;
                }
            }

            ITransport transport;
            lock (_activeTransportRoot)
            {
                transport = getTransportAction(host, port, staticHost, staticPort, type, TLUtils.GetProtocolDCId(settings.DcId, false, Constants.IsTestServer), settings.Secret, proxy, out isCreated);
            }
            if (isCreated)
            {
                transport.DCId = settings.DcId;
                transport.Secret = settings.Secret;
                transport.AuthKey = settings.AuthKey;
                transport.Salt = settings.Salt;
                transport.SessionId = settings.SessionId;
                transport.MessageIdDict = settings.MessageIdDict;
                transport.SequenceNumber = settings.SequenceNumber;
                transport.ClientTicksDelta = settings.ClientTicksDelta;
                transport.PacketReceived += settings.PacketReceivedHandler;
            }

            return transport;
        }

        private TLDCOption GetStaticDCOption(int dcId)
        {
            if (_config == null) return null;

            foreach (var item in _config.DCOptions.Items.Where(x => x.Static.Value))
            {
                if (item.Id.Value == dcId)
                {
                    return item;
                }
            }

            return null;
        }

        private ITransport GetTransportInternal(GetTransportFunc getTransportAction, string host, int port, TransportType type, bool checkStatic, TransportSettings settings)
        {
            bool isCreated;
            var staticHost = host;
            var staticPort = port;

            // looking for static dcOption with none empty config
            if (checkStatic)
            {
                var staticOption = GetStaticDCOption(settings.DcId);
                if (staticOption != null)
                {
                    staticHost = staticOption.IpAddress.ToString();
                    staticPort = staticOption.Port.Value;
                }
            }

            ITransport transport;
            lock (_activeTransportRoot)
            {
                transport = getTransportAction(host, port, staticHost, staticPort, type, TLUtils.GetProtocolDCId(settings.DcId, false, Constants.IsTestServer), settings.Secret, out isCreated);
            }
            if (isCreated)
            {
                transport.DCId = settings.DcId;
                transport.Secret = settings.Secret;
                transport.AuthKey = settings.AuthKey;
                transport.Salt = settings.Salt;
                transport.SessionId = settings.SessionId;
                transport.MessageIdDict = settings.MessageIdDict;
                transport.SequenceNumber = settings.SequenceNumber;
                transport.ClientTicksDelta = settings.ClientTicksDelta;
                transport.PacketReceived += settings.PacketReceivedHandler;
            }

            return transport;
        }

        private ITransport GetTransport(string host, int port, TransportType type, TransportSettings settings)
        {
            return GetTransportInternal(_transportService.GetTransport, host, port, type, true, settings);
        }

        private ITransport GetFileTransport(string host, int port, TransportType type, TransportSettings settings)
        {
            return GetTransportInternal(_transportService.GetFileTransport, host, port, type, true, settings);
        }

        private ITransport GetFileTransport2(string host, int port, int dcId, TransportType type, TransportSettings settings)
        {
            return GetTransportInternal(_transportService.GetFileTransport2, host, port, type, true, settings);
        }

        private ITransport GetSpecialTransport(string host, int port, TransportType type, TransportSettings settings)
        {
            return GetTransportInternal(_transportService.GetSpecialTransport, host, port, type, false, settings);
        }

        private ITransport GetSpecialTransportWithProxy(string host, int port, TransportType type, TLProxyBase proxy, TransportSettings settings)
        {
            return GetTransportWithProxyInternal(_transportService.GetSpecialTransport, host, port, type, true, settings, proxy);
        }
    }

    public class TransportSettings
    {
        public int DcId { get; set; }
        public byte[] Secret { get; set; }
        public byte[] AuthKey { get; set; }
        public TLLong Salt { get; set; }
        public TLLong SessionId { get; set; }
        public Dictionary<long, long> MessageIdDict { get; set; }
        public int SequenceNumber { get; set; }
        public long ClientTicksDelta { get; set; }
        public EventHandler<DataEventArgs> PacketReceivedHandler { get; set; }
    }

    public class AuthorizationRequiredEventArgs : EventArgs
    {
        public string MethodName { get; set; }

        public long AuthKeyId { get; set; }

        public TLRPCError Error { get; set; }
    }

    public class SendStatusEventArgs : EventArgs
    {
        public TLBool Offline { get; set; }

        public SendStatusEventArgs(TLBool offline)
        {
            Offline = offline;
        }
    }

    public class TransportCheckedEventArgs : EventArgs
    {
        public int TransportId { get; set; }
        public TLLong SessionId { get; set; }
        public byte[] AuthKey { get; set; }
        public DateTime? LastReceiveTime { get; set; }
        public int HistoryCount { get; set; }
        public int NextPacketLength { get; set; }
        public int LastPacketLength { get; set; }
        public string HistoryDescription { get; set; }
    }
}
