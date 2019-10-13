﻿// 
// This is the source code of Telegram for Windows Phone v. 3.x.x.
// It is licensed under GNU GPL v. 2 or later.
// You should have received a copy of the license in this archive (see LICENSE).
// 
// Copyright Evgeny Nadymov, 2013-present.
// 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Telegram.Api.Aggregator;
using Telegram.Api.Extensions;
using Telegram.Api.Helpers;
using Telegram.Api.Services.Cache.EventArgs;
using Telegram.Api.Services.Updates;
using Telegram.Api.TL;
using Action = System.Action;


namespace Telegram.Api.Services.Cache
{
    public class MockupCacheService : ICacheService
    {
        public ExceptionInfo LastSyncMessageException { get; private set; }

        public void SyncProxyData(TLProxyDataBase proxyData, Action<TLProxyDataBase> callback)
        {
            throw new NotImplementedException();
        }

        public void ClearLocalFileNames()
        {

        }

        public void CompressAsync(Action callback)
        {
            callback.SafeInvoke();
        }

        public void Commit()
        {

        }

        public bool TryCommit()
        {
            return true;
        }

        public void SaveSnapshot(string toDirectoryName)
        {

        }

        public void SaveTempSnapshot(string toDirectoryName)
        {

        }

        public void LoadSnapshot(string fromDirectoryName)
        {

        }

        public TLUserBase GetUser(TLInt id)
        {
            throw new NotImplementedException();
        }

        public TLUserBase GetUser(string username)
        {
            throw new NotImplementedException();
        }

        public TLUserBase GetUser(TLUserProfilePhoto photo)
        {
            throw new NotImplementedException();
        }

        public TLMessageBase GetMessage(TLInt id, TLInt channelId = null)
        {
            throw new NotImplementedException();
        }

        public TLMessageBase GetMessage(TLLong randomId)
        {
            throw new NotImplementedException();
        }

        public TLMessageBase GetMessage(TLWebPageBase webPage)
        {
            throw new NotImplementedException();
        }

        public TLDecryptedMessageBase GetDecryptedMessage(TLInt chatId, TLLong randomId)
        {
            throw new NotImplementedException();
        }

        public TLDialog GetDialog(TLMessageCommon message)
        {
            throw new NotImplementedException();
        }

        public TLDialog GetDialog(TLPeerBase peer)
        {
            throw new NotImplementedException();
        }

        public TLDialogBase GetEncryptedDialog(TLInt chatId)
        {
            throw new NotImplementedException();
        }

        public TLChat GetChat(TLChatPhoto chatPhoto)
        {
            throw new NotImplementedException();
        }

        public TLChannel GetChannel(string username)
        {
            throw new NotImplementedException();
        }

        public TLChannel GetChannel(TLChatPhoto channelPhoto)
        {
            throw new NotImplementedException();
        }

        public TLChatBase GetChat(TLInt id)
        {
            throw new NotImplementedException();
        }

        public TLBroadcastChat GetBroadcast(TLInt id)
        {
            throw new NotImplementedException();
        }

        public IList<TLMessageBase> GetMessages()
        {
            throw new NotImplementedException();
        }

        public IList<TLMessageBase> GetSendingMessages()
        {
            throw new NotImplementedException();
        }

        public IList<TLMessageBase> GetResendingMessages()
        {
            throw new NotImplementedException();
        }

        public void GetHistoryAsync(TLPeerBase peer, Action<IList<TLMessageBase>> callback, int limit = Constants.CachedMessagesCount)
        {
            callback.SafeInvoke(null);
        }

        public IList<TLMessageBase> GetHistory(TLPeerBase peer, int limit = Constants.CachedMessagesCount)
        {
            throw new NotImplementedException();
        }

        public IList<TLMessageBase> GetHistory(TLPeerBase peer, int maxId, int limit = Constants.CachedMessagesCount)
        {
            throw new NotImplementedException();
        }

        public IList<TLMessageBase> GetHistory(int dialogId)
        {
            throw new NotImplementedException();
        }

        public IList<TLDecryptedMessageBase> GetDecryptedHistory(int dialogId, int limit = Constants.CachedMessagesCount)
        {
            throw new NotImplementedException();
        }

        public IList<TLDecryptedMessageBase> GetDecryptedHistory(int dialogId, long randomId, int limit = Constants.CachedMessagesCount)
        {
            throw new NotImplementedException();
        }

        public IList<TLDecryptedMessageBase> GetUnreadDecryptedHistory(int dialogId)
        {
            throw new NotImplementedException();
        }

        public void GetDialogsAsync(Action<IList<TLDialogBase>> callback)
        {
            throw new NotImplementedException();
        }

        public IList<TLDialogBase> GetDialogs()
        {
            throw new NotImplementedException();
        }

        public void GetContactsAsync(Action<IList<TLUserBase>> callback)
        {
            throw new NotImplementedException();
        }

        public List<TLUserBase> GetContacts()
        {
            throw new NotImplementedException();
        }

        public List<TLUserBase> GetUsersForSearch(IList<TLDialogBase> nonCachedDialogs)
        {
            throw new NotImplementedException();
        }

        public List<TLUserBase> GetUsers()
        {
            throw new NotImplementedException();
        }

        public List<TLChatBase> GetChats()
        {
            throw new NotImplementedException();
        }

        public void GetChatsAsync(Action<IList<TLChatBase>> callback)
        {
            throw new NotImplementedException();
        }

        public void ClearAsync(Action callback = null)
        {
            throw new NotImplementedException();
        }

        public void SyncMessage(TLMessageBase message, Action<TLMessageBase> callback)
        {
            throw new NotImplementedException();
        }

        public void SyncMessage(TLMessageBase message, bool notifyNewDialog, bool notifyTopMessageUpdated, Action<TLMessageBase> callback)
        {
            throw new NotImplementedException();
        }

        public void SyncEditedMessage(TLMessageBase message, bool notifyNewDialog, bool notifyTopMessageUpdated, Action<TLMessageBase> callback)
        {
            throw new NotImplementedException();
        }

        public void SyncSendingMessage(TLMessageCommon message, TLMessageBase previousMessage, Action<TLMessageCommon> callback)
        {
            throw new NotImplementedException();
        }

        public void SyncSendingMessages(IList<TLMessage> messages, TLMessageBase previousMessage, Action<IList<TLMessage>> callback)
        {
            throw new NotImplementedException();
        }

        public void SyncSendingMessageId(TLLong randomId, TLInt id, Action<TLMessageCommon> callback)
        {
            throw new NotImplementedException();
        }

        public void SyncPeerMessages(TLPeerBase peer, TLMessagesBase messages, bool notifyNewDialog, bool notifyTopMessageUpdated,
            Action<TLMessagesBase> callback)
        {
            throw new NotImplementedException();
        }

        public void AddMessagesToContext(TLMessagesBase messages, Action<TLMessagesBase> callback)
        {
            throw new NotImplementedException();
        }

        public void SyncDialogs(Stopwatch stopwatch, TLDialogsBase dialogs, Action<TLDialogsBase> callback)
        {
            throw new NotImplementedException();
        }

        public void SyncChannelDialogs(TLDialogsBase dialogs, Action<TLDialogsBase> callback)
        {
            throw new NotImplementedException();
        }

        public void MergeMessagesAndChannels(TLDialogsBase dialogs)
        {
            throw new NotImplementedException();
        }

        public void SyncUser(TLUserBase user, Action<TLUserBase> callback)
        {
            throw new NotImplementedException();
        }

        public void SyncUser(TLUserFull userFull, Action<TLUserFull> callback)
        {
            throw new NotImplementedException();
        }

        public void SyncUsers(TLVector<TLUserBase> users, Action<TLVector<TLUserBase>> callback)
        {
            throw new NotImplementedException();
        }

        public void AddUsers(TLVector<TLUserBase> users, Action<TLVector<TLUserBase>> callback)
        {
            throw new NotImplementedException();
        }

        public void SyncUsersAndChats(TLVector<TLUserBase> users, TLVector<TLChatBase> chats, Action<WindowsPhone.Tuple<TLVector<TLUserBase>, TLVector<TLChatBase>>> callback)
        {
            throw new NotImplementedException();
        }

        public void SyncUserLink(TLLinkBase link, Action<TLLinkBase> callback)
        {
            throw new NotImplementedException();
        }

        public void SyncContacts(TLContactsBase contacts, Action<TLContactsBase> callback)
        {
            throw new NotImplementedException();
        }

        public void SyncContacts(TLImportedContacts contacts, Action<TLImportedContacts> callback)
        {
            throw new NotImplementedException();
        }

        public void ClearDialog(TLPeerBase peer)
        {
            throw new NotImplementedException();
        }

        public void DeleteDialog(TLDialogBase dialog)
        {
            throw new NotImplementedException();
        }

        public void DeleteMessages(TLVector<TLInt> ids)
        {
            throw new NotImplementedException();
        }

        public void DeleteChannelMessages(TLInt channelId, TLVector<TLInt> ids)
        {
            throw new NotImplementedException();
        }

        public void DeleteMessages(TLPeerBase peer, TLMessageBase lastItem, TLVector<TLInt> messages)
        {
            throw new NotImplementedException();
        }

        public void DeleteMessages(TLVector<TLLong> ids)
        {
            throw new NotImplementedException();
        }

        public void DeleteDecryptedMessages(TLVector<TLLong> ids)
        {
            throw new NotImplementedException();
        }

        public void ClearDecryptedHistoryAsync(TLInt chatId)
        {
            throw new NotImplementedException();
        }

        public void ClearBroadcastHistoryAsync(TLInt chatId)
        {
            throw new NotImplementedException();
        }

        public void SyncStatedMessage(TLStatedMessageBase statedMessage, Action<TLStatedMessageBase> callback)
        {
            throw new NotImplementedException();
        }

        public void SyncStatedMessages(TLStatedMessagesBase statedMessages, Action<TLStatedMessagesBase> callback)
        {
            throw new NotImplementedException();
        }

        private TLCdnConfig _cdnConfig;

        private readonly object _cdnConfigSyncRoot = new object();

        public void GetCdnConfigAsync(Action<TLCdnConfig> callback)
        {
            if (_cdnConfig == null)
            {
                _cdnConfig = TLUtils.OpenObjectFromMTProtoFile<TLCdnConfig>(_cdnConfigSyncRoot, Constants.CdnConfigFileName);
            }

            callback.SafeInvoke(_cdnConfig);
        }

        public TLCdnConfig GetCdnConfig()
        {
            if (_cdnConfig == null)
            {
                _cdnConfig = TLUtils.OpenObjectFromMTProtoFile<TLCdnConfig>(_cdnConfigSyncRoot, Constants.CdnConfigFileName);
            }

            return _cdnConfig;
        }

        public void SetCdnCofig(TLCdnConfig cdnConfig)
        {
            _cdnConfig = cdnConfig;

            TLUtils.SaveObjectToMTProtoFile(_cdnConfigSyncRoot, Constants.CdnConfigFileName, _cdnConfig);
        }

        private TLConfig _config;

        public TLConfig GetConfig()
        {
#if SILVERLIGHT || WIN_RT
            if (_config == null)
            {
                _config = SettingsHelper.GetValue(Constants.ConfigKey) as TLConfig;
            }
#endif
            return _config;
        }


        public void GetConfigAsync(Action<TLConfig> callback)
        {
#if SILVERLIGHT || WIN_RT
            if (_config == null)
            {
                _config = SettingsHelper.GetValue(Constants.ConfigKey) as TLConfig;
            }
#endif
            callback.SafeInvoke(_config);
        }

        public void SetConfig(TLConfig config)
        {
            _config = config;
#if SILVERLIGHT || WIN_RT
            SettingsHelper.SetValue(Constants.ConfigKey, config);
#endif
        }

        public void ClearConfigImportAsync()
        {
            throw new NotImplementedException();
        }

        public void SyncChat(TLMessagesChatFull messagesChatFull, Action<TLMessagesChatFull> callback)
        {
            throw new NotImplementedException();
        }

        public void AddChats(TLVector<TLChatBase> chats, Action<TLVector<TLChatBase>> callback)
        {
            throw new NotImplementedException();
        }

        public void SyncBroadcast(TLBroadcastChat broadcast, Action<TLBroadcastChat> callback)
        {
            throw new NotImplementedException();
        }

        public TLEncryptedChatBase GetEncryptedChat(TLInt id)
        {
            throw new NotImplementedException();
        }

        public void SyncEncryptedChat(TLEncryptedChatBase encryptedChat, Action<TLEncryptedChatBase> callback)
        {
            throw new NotImplementedException();
        }

        public void SyncDecryptedMessage(TLDecryptedMessageBase message, TLEncryptedChatBase peer, Action<TLDecryptedMessageBase> callback)
        {
            throw new NotImplementedException();
        }

        public void SyncDecryptedMessages(IList<WindowsPhone.Tuple<TLDecryptedMessageBase, TLObject>> tuples, TLEncryptedChatBase peer, Action<IList<WindowsPhone.Tuple<TLDecryptedMessageBase, TLObject>>> callback)
        {
            throw new NotImplementedException();
        }

        public void SyncSendingDecryptedMessage(TLInt chatId, TLInt date, TLLong randomId, Action<TLDecryptedMessageBase> callback)
        {
            throw new NotImplementedException();
        }

        public void Init()
        {
            throw new NotImplementedException();
        }

        public void SyncDifference(TLDifference difference, Action<TLDifference> result, IList<ExceptionInfo> exceptions)
        {
            throw new NotImplementedException();
        }

        public void SyncDifferenceWithoutUsersAndChats(TLDifference difference, Action<TLDifference> result, IList<ExceptionInfo> exceptions)
        {
            throw new NotImplementedException();
        }

        public void SyncStatuses(TLVector<TLContactStatusBase> contacts, Action<TLVector<TLContactStatusBase>> callback)
        {
            throw new NotImplementedException();
        }

        public void DeleteUser(TLInt id)
        {
            throw new NotImplementedException();
        }

        public void DeleteChat(TLInt id)
        {
            throw new NotImplementedException();
        }

        public void DeleteUserHistory(TLPeerChannel channel, TLPeerUser peer)
        {
            throw new NotImplementedException();
        }

        public void UpdateDialogPinned(TLPeerBase peer, bool pinned)
        {
            throw new NotImplementedException();
        }

        public void UpdatePinnedDialogs(TLVector<TLPeerBase> order)
        {
            throw new NotImplementedException();
        }

        public void UpdateChannelAvailableMessages(TLInt channelId, TLInt availableMinId)
        {
            throw new NotImplementedException();
        }

        public void UpdateDialogPromo(TLDialogBase dialogBase, bool promo)
        {
            throw new NotImplementedException();
        }

        public TLProxyDataBase GetProxyData()
        {
            throw new NotImplementedException();
        }

        public void UpdateProxyData(TLProxyDataBase proxyData)
        {
            throw new NotImplementedException();
        }
    }

    public class InMemoryCacheService : ICacheService
    {
        private readonly object _databaseSyncRoot = new object();

        private InMemoryDatabase _database;

        private Context<TLUserBase> UsersContext
        {
            get { return _database != null ? _database.UsersContext : null; }
        }

        private Context<TLChatBase> ChatsContext
        {
            get { return _database != null ? _database.ChatsContext : null; }
        }

        private Context<TLBroadcastChat> BroadcastsContext
        {
            get { return _database != null ? _database.BroadcastsContext : null; }
        }

        private Context<TLEncryptedChatBase> EncryptedChatsContext
        {
            get { return _database != null ? _database.EncryptedChatsContext : null; }
        }

        private Context<TLMessageBase> MessagesContext
        {
            get { return _database != null ? _database.MessagesContext : null; }
        }

        private Context<Context<TLMessageBase>> ChannelsContext
        {
            get { return _database != null ? _database.ChannelsContext : null; }
        }

        private Context<TLDecryptedMessageBase> DecryptedMessagesContext
        {
            get { return _database != null ? _database.DecryptedMessagesContext : null; }
        }

        private Context<TLMessageBase> RandomMessagesContext
        {
            get { return _database != null ? _database.RandomMessagesContext : null; }
        }

        private Context<TLDialogBase> DialogsContext
        {
            get { return _database != null ? _database.DialogsContext : null; }
        }

        public void Init()
        {
            var stopwatch = Stopwatch.StartNew();

            _database = new InMemoryDatabase(_eventAggregator);
            _database.Open();

            Debug.WriteLine("{0} {1}", stopwatch.Elapsed, "open database time");
        }

        private readonly ITelegramEventAggregator _eventAggregator;

        public static ICacheService Instance { get; protected set; }

        public InMemoryCacheService(ITelegramEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            Instance = this;
        }

        public IList<TLDialogBase> GetDialogs()
        {
            var result = new List<TLDialogBase>();

            if (_database == null) Init();

            if (DialogsContext == null)
            {

                return result;
            }
            var timer = Stopwatch.StartNew();

            IList<TLDialogBase> dialogs = new List<TLDialogBase>();

            try
            {
                dialogs = new List<TLDialogBase>(_database.Dialogs);
            }
            catch (Exception e)
            {
                TLUtils.WriteLine("DB ERROR:", LogSeverity.Error);
                TLUtils.WriteLine(e.ToString(), LogSeverity.Error);
            }

            TLUtils.WritePerformance(string.Format("GetCachedDialogs time ({0} from {1}): {2}", dialogs.Count, _database.CountRecords<TLDialog>(), timer.Elapsed));
            return dialogs.OrderByDescending(x => x.GetDateIndex()).ToList();
        }


        public void GetDialogsAsync(Action<IList<TLDialogBase>> callback)
        {
            Execute.BeginOnThreadPool(
                () =>
                {
                    var result = new List<TLDialogBase>();

                    if (_database == null) Init();

                    if (DialogsContext == null)
                    {
                        callback(result);
                        return;
                    }
                    var timer = Stopwatch.StartNew();

                    IList<TLDialogBase> dialogs = new List<TLDialogBase>();

                    try
                    {
                        dialogs = new List<TLDialogBase>(_database.Dialogs);
                    }
                    catch (Exception e)
                    {
                        TLUtils.WriteLine("DB ERROR:", LogSeverity.Error);
                        TLUtils.WriteLine(e.ToString(), LogSeverity.Error);
                    }

                    TLUtils.WritePerformance(string.Format("GetCachedDialogs time ({0} from {1}): {2}", dialogs.Count, _database.CountRecords<TLDialog>(), timer.Elapsed));
                    callback(dialogs.OrderByDescending(x => x.GetDateIndex()).ToList());
                });
        }

        public List<TLUserBase> GetUsers()
        {
            var result = new List<TLUserBase>();

            if (_database == null) Init();

            if (UsersContext == null)
            {
                return result;
            }
            var timer = Stopwatch.StartNew();

            var contacts = new List<TLUserBase>();

            try
            {
                contacts = _database.UsersContext.Values.ToList();

            }
            catch (Exception e)
            {
                TLUtils.WriteLine("DB ERROR:", LogSeverity.Error);
                TLUtils.WriteException(e);
            }

            TLUtils.WritePerformance(string.Format("GetCachedContacts time ({0} from {1}): {2}", contacts.Count, _database.CountRecords<TLUserBase>(), timer.Elapsed));
            return contacts;
        }

        public List<TLUserBase> GetContacts()
        {
            var result = new List<TLUserBase>();

            if (_database == null) Init();

            if (UsersContext == null)
            {
                return result;
            }
            var timer = Stopwatch.StartNew();

            var contacts = new List<TLUserBase>();

            try
            {
                contacts = _database.UsersContext.Values.Where(x => x != null && (x.IsContact || x.IsSelf)).ToList();
                //contacts = _database.UsersContext.Values.Where(x => x.Contact != null).ToList();

            }
            catch (Exception e)
            {
                TLUtils.WriteLine("DB ERROR:", LogSeverity.Error);
                TLUtils.WriteException(e);
            }

            TLUtils.WritePerformance(string.Format("GetCachedContacts time ({0} from {1}): {2}", contacts.Count, _database.CountRecords<TLUserBase>(), timer.Elapsed));
            return contacts;
        }

        public List<TLUserBase> GetUsersForSearch(IList<TLDialogBase> nonCachedDialogs)
        {
            var result = new List<TLUserBase>();

            if (_database == null) Init();

            if (UsersContext == null)
            {
                return result;
            }

            var contacts = new List<TLUserBase>();
            try
            {
                var usersCache = new Dictionary<long, long>();

                if (nonCachedDialogs != null)
                {
                    for (var i = 0; i < nonCachedDialogs.Count; i++)
                    {
                        var dialog = nonCachedDialogs[i] as TLDialog;
                        if (dialog != null)
                        {
                            var user = nonCachedDialogs[i].With as TLUserBase;
                            if (user != null)
                            {
                                if (!usersCache.ContainsKey(user.Index))
                                {
                                    usersCache[user.Index] = user.Index;
                                    contacts.Add(user);
                                }
                            }
                        }
                    }
                }

                var dialogs = new List<TLDialogBase>(_database.Dialogs);

                for (var i = 0; i < dialogs.Count; i++)
                {
                    var dialog = dialogs[i] as TLDialog;
                    if (dialog != null)
                    {
                        var user = dialogs[i].With as TLUserBase;
                        if (user != null)
                        {
                            if (!usersCache.ContainsKey(user.Index))
                            {
                                usersCache[user.Index] = user.Index;
                                contacts.Add(user);
                            }
                        }
                    }
                }

                var unsortedContacts = _database.UsersContext.Values.Where(x => x != null && x.IsContact).ToList();
                for (var i = 0; i < unsortedContacts.Count; i++)
                {
                    var user = unsortedContacts[i];
                    if (!usersCache.ContainsKey(user.Index))
                    {
                        usersCache[user.Index] = user.Index;
                        contacts.Add(user);
                    }
                }
            }
            catch (Exception e)
            {
                TLUtils.WriteLine("DB ERROR:", LogSeverity.Error);
                TLUtils.WriteException(e);
            }

            return contacts;
        }

        public void GetContactsAsync(Action<IList<TLUserBase>> callback)
        {
            Execute.BeginOnThreadPool(
                () =>
                {
                    var result = new List<TLUserBase>();

                    if (_database == null) Init();

                    if (UsersContext == null)
                    {
                        callback(result);
                        return;
                    }
                    var timer = Stopwatch.StartNew();

                    IList<TLUserBase> contacts = new List<TLUserBase>();

                    try
                    {
                        contacts = _database.UsersContext.Values.Where(x => x != null && x.IsContact).ToList();
                        //contacts = _database.UsersContext.Values.Where(x => x.Contact != null).ToList();

                    }
                    catch (Exception e)
                    {
                        TLUtils.WriteLine("DB ERROR:", LogSeverity.Error);
                        TLUtils.WriteException(e);
                    }

                    TLUtils.WritePerformance(string.Format("GetCachedContacts time ({0} from {1}): {2}", contacts.Count, _database.CountRecords<TLUserBase>(), timer.Elapsed));
                    callback(contacts);
                });
        }

        public List<TLChatBase> GetChats()
        {
            var result = new List<TLChatBase>();

            if (_database == null) Init();

            if (ChatsContext == null)
            {
                return result;
            }
            var timer = Stopwatch.StartNew();

            IList<TLChatBase> chats = new List<TLChatBase>();

            try
            {
                result = _database.ChatsContext.Values.ToList();

            }
            catch (Exception e)
            {
                TLUtils.WriteLine("DB ERROR:", LogSeverity.Error);
                TLUtils.WriteException(e);
            }

            TLUtils.WritePerformance(string.Format("GetCachedChats time ({0} from {1}): {2}", chats.Count, _database.CountRecords<TLChatBase>(), timer.Elapsed));

            return result;
        }

        public void GetChatsAsync(Action<IList<TLChatBase>> callback)
        {
            Execute.BeginOnThreadPool(
                () =>
                {
                    var result = new List<TLChatBase>();

                    if (_database == null) Init();

                    if (ChatsContext == null)
                    {
                        callback(result);
                        return;
                    }
                    var timer = Stopwatch.StartNew();

                    IList<TLChatBase> chats = new List<TLChatBase>();

                    try
                    {
                        chats = _database.ChatsContext.Values.ToList();

                    }
                    catch (Exception e)
                    {
                        TLUtils.WriteLine("DB ERROR:", LogSeverity.Error);
                        TLUtils.WriteException(e);
                    }

                    TLUtils.WritePerformance(string.Format("GetCachedChats time ({0} from {1}): {2}", chats.Count, _database.CountRecords<TLChatBase>(), timer.Elapsed));
                    callback(chats);
                });
        }

        public TLChatBase GetChat(TLInt id)
        {
            if (_database == null)
            {
                Init();
            }

            return ChatsContext[id.Value];
        }

        public TLBroadcastChat GetBroadcast(TLInt id)
        {
            if (_database == null)
            {
                Init();
            }

            return BroadcastsContext[id.Value];
        }

        public TLEncryptedChatBase GetEncryptedChat(TLInt id)
        {
            if (_database == null)
            {
                Init();
            }

            return EncryptedChatsContext[id.Value];
        }

        public TLUserBase GetUser(TLInt id)
        {
            if (_database == null)
            {
                Init();
            }

            return UsersContext[id.Value];
        }

        public TLUserBase GetUser(TLUserProfilePhoto photo)
        {
            var usersShapshort = new List<TLUserBase>(UsersContext.Values);

            return usersShapshort.FirstOrDefault(x => x.Photo == photo);
        }

        public TLUserBase GetUser(string username)
        {
            var usersShapshort = new List<TLUserBase>(UsersContext.Values);

            return usersShapshort.FirstOrDefault(x => x is IUserName && ((IUserName)x).UserName != null && string.Equals(((IUserName)x).UserName.ToString(), username, StringComparison.OrdinalIgnoreCase));
        }

        public TLMessageBase GetMessage(TLInt id, TLInt channelId = null)
        {
            if (channelId != null)
            {
                var channelContext = ChannelsContext[channelId.Value];
                if (channelContext != null)
                {
                    return channelContext[id.Value];
                }

                return null;
            }

            return MessagesContext[id.Value];
        }

        public TLMessageBase GetMessage(TLLong randomId)
        {
            return RandomMessagesContext[randomId.Value];
        }

        public TLMessageBase GetMessage(TLWebPageBase webPageBase)
        {
            var m = MessagesContext.Values.FirstOrDefault(x =>
            {
                var message = x as TLMessage;
                if (message != null)
                {
                    var webPageMedia = message.Media as TLMessageMediaWebPage;
                    if (webPageMedia != null)
                    {
                        var currentWebPage = webPageMedia.WebPage;
                        if (currentWebPage != null
                            && currentWebPage.Id.Value == webPageBase.Id.Value)
                        {
                            return true;
                        }
                    }
                }

                return false;
            });

            if (m != null) return m;

            foreach (var channelContext in ChannelsContext.Values)
            {
                foreach (var x in channelContext.Values)
                {
                    var message = x as TLMessage;
                    if (message != null)
                    {
                        var webPageMedia = message.Media as TLMessageMediaWebPage;
                        if (webPageMedia != null)
                        {
                            var currentWebPage = webPageMedia.WebPage;
                            if (currentWebPage != null
                                && currentWebPage.Id.Value == webPageBase.Id.Value)
                            {
                                m = message;
                                break;
                            }
                        }
                    }
                }
            }

            if (m != null) return m;

            m = RandomMessagesContext.Values.FirstOrDefault(x =>
            {
                var message = x as TLMessage;
                if (message != null)
                {
                    var webPageMedia = message.Media as TLMessageMediaWebPage;
                    if (webPageMedia != null)
                    {
                        var currentWebPage = webPageMedia.WebPage;
                        if (currentWebPage != null
                            && currentWebPage.Id.Value == webPageBase.Id.Value)
                        {
                            return true;
                        }
                    }
                }

                return false;
            });

            return m;
        }

        public TLDialog GetDialog(TLMessageCommon message)
        {
            TLPeerBase peer;
            if (message.ToId is TLPeerChat)
            {
                peer = message.ToId;
            }
            else
            {
                peer = message.Out.Value ? message.ToId : new TLPeerUser { Id = message.FromId };
            }
            return GetDialog(peer);
        }

        public TLDialog GetDialog(TLPeerBase peer)
        {
            return _database.GetDialog(peer) as TLDialog;

            //return _database.Dialogs.OfType<TLDialog>().FirstOrDefault(x => x.WithId == peer.Id.Value && x.IsChat == peer is TLPeerChat);
        }

        public TLDialogBase GetEncryptedDialog(TLInt chatId)
        {
            return _database.Dialogs.OfType<TLEncryptedDialog>().FirstOrDefault(x => x.Index == chatId.Value);
        }

        public TLChat GetChat(TLChatPhoto chatPhoto)
        {
            return _database.ChatsContext.Values.FirstOrDefault(x => x is TLChat && ((TLChat)x).Photo == chatPhoto) as TLChat;
        }

        public TLChannel GetChannel(string username)
        {
            var chatsSnapshort = new List<TLChatBase>(_database.ChatsContext.Values);

            return chatsSnapshort.FirstOrDefault(x => x is TLChannel && ((TLChannel)x).UserName != null && string.Equals(((TLChannel)x).UserName.ToString(), username, StringComparison.OrdinalIgnoreCase)) as TLChannel;
        }

        public TLChannel GetChannel(TLChatPhoto chatPhoto)
        {
            var chatsSnapshort = new List<TLChatBase>(_database.ChatsContext.Values);

            return chatsSnapshort.FirstOrDefault(x => x is TLChannel && ((TLChannel)x).Photo == chatPhoto) as TLChannel;
        }

        public IList<TLMessageBase> GetHistory(int dialogIndex)
        {
            var result = new List<TLMessageBase>();

            if (_database == null) Init();

            if (DecryptedMessagesContext == null || DialogsContext == null)
            {
                return result;
            }
            var timer = Stopwatch.StartNew();


            IList<TLMessageBase> msgs = new List<TLMessageBase>();
            try
            {
                var dialog = DialogsContext[dialogIndex] as TLDialog;

                if (dialog != null)
                {
                    msgs = dialog.Messages
                        .OfType<TLMessageCommon>()
                        //.Where(x =>

                            //x.FromId.Value == currentUserId.Value && x.ToId.Id.Value == peer.Id.Value           // to peer from current
                        //|| x.FromId.Value == peer.Id.Value && x.ToId.Id.Value == currentUserId.Value) // from peer to current

                            .Cast<TLMessageBase>()
                            .ToList();
                }

            }
            catch (Exception e)
            {
                TLUtils.WriteLine("DB ERROR:", LogSeverity.Error);
                TLUtils.WriteException(e);
            }

            //TLUtils.WritePerformance(string.Format("GetCachedHistory time ({0}): {1}", _database.CountRecords<TLMessageBase>(), timer.Elapsed));
            return msgs.Take(Constants.CachedMessagesCount).ToList();
        }

        public TLDecryptedMessageBase GetDecryptedMessage(TLInt chatId, TLLong randomId)
        {
            TLDecryptedMessageBase result = null;

            if (_database == null) Init();

            if (MessagesContext == null || DialogsContext == null)
            {
                return result;
            }

            IList<TLDecryptedMessageBase> msgs = new List<TLDecryptedMessageBase>();
            try
            {
                var dialog = DialogsContext[chatId.Value] as TLEncryptedDialog;

                if (dialog != null)
                {
                    msgs = dialog.Messages.ToList();
                    foreach (var message in msgs)
                    {
                        if (message.RandomIndex == randomId.Value)
                        {
                            return message;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                TLUtils.WriteLine("DB ERROR:", LogSeverity.Error);
                TLUtils.WriteException(e);
            }

            return result;
        }

        public IList<TLDecryptedMessageBase> GetDecryptedHistory(int dialogIndex, int limit = Constants.CachedMessagesCount)
        {
            var result = new List<TLDecryptedMessageBase>();

            if (_database == null) Init();

            if (MessagesContext == null || DialogsContext == null)
            {
                return result;
            }

            IList<TLDecryptedMessageBase> msgs = new List<TLDecryptedMessageBase>();
            try
            {
                var dialog = DialogsContext[dialogIndex] as TLEncryptedDialog;

                if (dialog != null)
                {
                    msgs = dialog.Messages.ToList();
                }

            }
            catch (Exception e)
            {
                TLUtils.WriteLine("DB ERROR:", LogSeverity.Error);
                TLUtils.WriteException(e);
            }

            var returnedMessages = new List<TLDecryptedMessageBase>();
            var count = 0;
            for (var i = 0; i < msgs.Count && count < limit; i++)
            {
                returnedMessages.Add(msgs[i]);
                if (TLUtils.IsDisplayedDecryptedMessage(msgs[i]))
                {
                    count++;
                }
            }

            return returnedMessages;
        }

        public IList<TLDecryptedMessageBase> GetUnreadDecryptedHistory(int dialogIndex)
        {
            var result = new List<TLDecryptedMessageBase>();

            if (_database == null) Init();

            if (MessagesContext == null || DialogsContext == null)
            {
                return result;
            }

            IList<TLDecryptedMessageBase> msgs = new List<TLDecryptedMessageBase>();
            try
            {
                var dialog = DialogsContext[dialogIndex] as TLEncryptedDialog;

                if (dialog != null)
                {
                    msgs = dialog.Messages.ToList();
                }

            }
            catch (Exception e)
            {
                TLUtils.WriteLine("DB ERROR:", LogSeverity.Error);
                TLUtils.WriteException(e);
            }

            var returnedMessages = new List<TLDecryptedMessageBase>();
            for (var i = 0; i < msgs.Count; i++)
            {
                if (!msgs[i].Out.Value && msgs[i].Unread.Value)
                {
                    returnedMessages.Add(msgs[i]);
                }
            }

            return returnedMessages;
        }

        public IList<TLDecryptedMessageBase> GetDecryptedHistory(int dialogIndex, long randomId, int limit = Constants.CachedMessagesCount)
        {
            var result = new List<TLDecryptedMessageBase>();

            if (_database == null) Init();

            if (MessagesContext == null || DialogsContext == null)
            {
                return result;
            }

            IList<TLDecryptedMessageBase> msgs = new List<TLDecryptedMessageBase>();
            try
            {
                var dialog = DialogsContext[dialogIndex] as TLEncryptedDialog;

                if (dialog != null)
                {
                    msgs = dialog.Messages.ToList();
                }

            }
            catch (Exception e)
            {
                TLUtils.WriteLine("DB ERROR:", LogSeverity.Error);
                TLUtils.WriteException(e);
            }

            var skipCount = 0;
            if (randomId != 0)
            {
                skipCount = 1;
                for (var i = 0; i < msgs.Count; i++)
                {
                    if (msgs[i].RandomIndex != randomId)
                    {
                        skipCount++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            var returnedMessages = new List<TLDecryptedMessageBase>();
            var count = 0;
            for (var i = skipCount; i < msgs.Count && count < limit; i++)
            {
                returnedMessages.Add(msgs[i]);
                if (TLUtils.IsDisplayedDecryptedMessage(msgs[i]))
                {
                    count++;
                }
            }

            return returnedMessages;
        }

        public IList<TLMessageBase> GetHistory(TLPeerBase peer, int maxId, int limit = Constants.CachedMessagesCount)
        {
            var result = new List<TLMessageBase>();

            if (_database == null) Init();

            if (MessagesContext == null)
            {
                return result;
            }

            IList<TLMessageBase> msgs = new List<TLMessageBase>();
            try
            {
                var withId = peer.Id.Value;
                var dialogBase = _database.Dialogs.FirstOrDefault(x => x.WithId == withId && peer.GetType() == x.Peer.GetType());

                var dialog = dialogBase as TLDialog;
                if (dialog != null)
                {
                    msgs = dialog.Messages
                        .OfType<TLMessageCommon>()
                        .Cast<TLMessageBase>()
                        .ToList();
                }

                var broadcast = dialogBase as TLBroadcastDialog;
                if (broadcast != null)
                {
                    msgs = broadcast.Messages
                        .OfType<TLMessageCommon>()
                        .Cast<TLMessageBase>()
                        .ToList();
                }

            }
            catch (Exception e)
            {
                TLUtils.WriteLine("DB ERROR:", LogSeverity.Error);
                TLUtils.WriteException(e);
            }

            var count = 0;
            var startPosition = -1;
            var resultMsgs = new List<TLMessageBase>();
            for (var i = 0; i < msgs.Count && count < limit; i++)
            {
                var msg = msgs[i];
                if (startPosition == -1)
                {
                    if (msg.Index == 0 || msg.Index > maxId)
                    {
                        continue;
                    }

                    if (msg.Index == maxId)
                    {
                        startPosition = i;
                    }

                    if (msg.Index < maxId)
                    {
                        break;
                    }
                }

                resultMsgs.Add(msg);
                count++;
            }

            return resultMsgs;
        }

        public IList<TLMessageBase> GetHistory(TLPeerBase peer, int limit = Constants.CachedMessagesCount)
        {
            var result = new List<TLMessageBase>();

            if (_database == null) Init();

            if (MessagesContext == null)
            {
                return result;
            }
            var timer = Stopwatch.StartNew();


            IList<TLMessageBase> msgs = new List<TLMessageBase>();
            try
            {
                var withId = peer.Id.Value;
                var dialogBase = _database.Dialogs.FirstOrDefault(x => x.WithId == withId && peer.GetType() == x.Peer.GetType());

                var dialog = dialogBase as TLDialog;
                if (dialog != null)
                {
                    msgs = dialog.Messages
                        .OfType<TLMessageCommon>()
                        //.Where(x =>

                            //x.FromId.Value == currentUserId.Value && x.ToId.Id.Value == peer.Id.Value           // to peer from current
                        //|| x.FromId.Value == peer.Id.Value && x.ToId.Id.Value == currentUserId.Value) // from peer to current

                            .Cast<TLMessageBase>()
                            .ToList();
                }

                var broadcast = dialogBase as TLBroadcastDialog;
                if (broadcast != null)
                {
                    msgs = broadcast.Messages
                        .OfType<TLMessageCommon>()
                        //.Where(x =>

                            //x.FromId.Value == currentUserId.Value && x.ToId.Id.Value == peer.Id.Value           // to peer from current
                        //|| x.FromId.Value == peer.Id.Value && x.ToId.Id.Value == currentUserId.Value) // from peer to current

                            .Cast<TLMessageBase>()
                            .ToList();
                }

            }
            catch (Exception e)
            {
                TLUtils.WriteLine("DB ERROR:", LogSeverity.Error);
                TLUtils.WriteException(e);
            }

            // TLUtils.WritePerformance(string.Format("GetCachedHistory time ({0}): {1}", _database.CountRecords<TLMessageBase>(), timer.Elapsed));
            return msgs.Take(limit).ToList();
        }

        public void GetHistoryAsync(TLPeerBase peer, Action<IList<TLMessageBase>> callback, int limit = Constants.CachedMessagesCount)
        {
            Execute.BeginOnThreadPool(
                () =>
                {
                    var history = GetHistory(peer, limit);
                    callback.SafeInvoke(history);
                });
        }

        public void ClearAsync(Action callback = null)
        {
            Execute.BeginOnThreadPool(
                () =>
                {
                    lock (_databaseSyncRoot)
                    {
                        if (_database != null) _database.Clear();
                    }
                    callback.SafeInvoke();
                });
        }

        #region Messages

        private TLMessageBase GetCachedMessage(TLMessageBase message)
        {
            TLPeerChannel peerChannel;
            var isChannelMessage = TLUtils.IsChannelMessage(message, out peerChannel);
            if (isChannelMessage)
            {
                if (message.Index != 0 && ChannelsContext != null && ChannelsContext.ContainsKey(peerChannel.Id.Value))
                {
                    var channelContext = ChannelsContext[peerChannel.Id.Value];
                    if (channelContext != null)
                    {
                        return channelContext[message.Index];
                    }
                }

                return null;
            }

            if (message.Index != 0 && MessagesContext != null && MessagesContext.ContainsKey(message.Index))
            {
                return MessagesContext[message.Index];
            }

            if (message.RandomIndex != 0 && RandomMessagesContext != null && RandomMessagesContext.ContainsKey(message.RandomIndex))
            {
                return RandomMessagesContext[message.RandomIndex];
            }

            return null;
        }

        private TLDecryptedMessageBase GetCachedDecryptedMessage(TLLong randomId)
        {
            if (randomId != null && DecryptedMessagesContext != null && DecryptedMessagesContext.ContainsKey(randomId.Value))
            {
                return DecryptedMessagesContext[randomId.Value];
            }

            return null;
        }

        private TLDecryptedMessageBase GetCachedDecryptedMessage(TLDecryptedMessageBase message)
        {
            if (message.RandomId != null && DecryptedMessagesContext != null && DecryptedMessagesContext.ContainsKey(message.RandomIndex))
            {
                return DecryptedMessagesContext[message.RandomIndex];
            }

            //if (message.RandomIndex != 0 && RandomMessagesContext != null && RandomMessagesContext.ContainsKey(message.RandomIndex))
            //{
            //    return RandomMessagesContext[message.RandomIndex];
            //}


            return null;
        }

        public void SyncSendingMessages(IList<TLMessage> messages, TLMessageBase previousMessage, Action<IList<TLMessage>> callback)
        {
            if (messages == null)
            {
                callback(null);
                return;
            }

            var message73 = previousMessage as TLMessage73;
            if (message73 != null)
            {
                var mediaGroup = message73.Media as TLMessageMediaGroup;
                if (mediaGroup != null)
                {
                    previousMessage = mediaGroup.Group.LastOrDefault();
                }
            }

            var timer = Stopwatch.StartNew();

            var result = new List<TLMessage>();
            if (_database == null) Init();

            for (var i = 0; i < messages.Count; i++)
            {
                var message = messages[i];
                var cachedMessage = GetCachedMessage(message) as TLMessage;

                if (cachedMessage != null)
                {
                    _database.UpdateSendingMessage(message, cachedMessage);
                    result.Add(cachedMessage);
                }
                else
                {
                    var previousMsg = i == 0 ? previousMessage : messages[i - 1];
                    var isLastMsg = i == messages.Count - 1;
                    _database.AddSendingMessage(message, previousMsg, isLastMsg, isLastMsg);
                    result.Add(message);
                }
            }

            _database.Commit();

            TLUtils.WritePerformance("SyncSendingMessages time: " + timer.Elapsed);
            callback(result);
        }

        public void SyncSendingMessageId(TLLong randomId, TLInt id, Action<TLMessageCommon> callback)
        {
            var timer = Stopwatch.StartNew();

            TLMessage result = null;
            if (_database == null) Init();

            var cachedMessage = GetMessage(randomId) as TLMessage;
            if (cachedMessage != null)
            {
                cachedMessage.Id = id;
                _database.UpdateSendingMessageContext(cachedMessage);
                result = cachedMessage;

                // send at background task and GetDialogs was invoked before getDifference
                // remove duplicates
                var dialog = GetDialog(cachedMessage);
                if (dialog != null)
                {
                    lock (dialog.MessagesSyncRoot)
                    {
                        var count = 0;
                        for (int i = 0; i < dialog.Messages.Count; i++)
                        {
                            if (dialog.Messages[i].Index == id.Value)
                            {
                                count++;

                                if (count > 1)
                                {
                                    dialog.Messages.RemoveAt(i--);
                                }
                            }
                        }
                    }
                }
            }

            _database.Commit();

            TLUtils.WritePerformance("SyncSendingMessageId time: " + timer.Elapsed);
            callback(result);
        }

        public void SyncSendingMessage(TLMessageCommon message, TLMessageBase previousMessage, Action<TLMessageCommon> callback)
        {
            if (message == null)
            {
                callback(null);
                return;
            }

            var message73 = previousMessage as TLMessage73;
            if (message73 != null)
            {
                var mediaGroup = message73.Media as TLMessageMediaGroup;
                if (mediaGroup != null)
                {
                    previousMessage = mediaGroup.Group.LastOrDefault();
                }
            }

            var timer = Stopwatch.StartNew();

            var result = message;
            if (_database == null) Init();

            var cachedMessage = GetCachedMessage(message);

            if (cachedMessage != null)
            {
                _database.UpdateSendingMessage(message, cachedMessage);
                result = (TLMessage)cachedMessage;
            }
            else
            {
                _database.AddSendingMessage(message, previousMessage);

                // forwarding
                var messagesContainer = message.Reply as TLMessagesContainter;
                if (messagesContainer != null)
                {
                    var messages = messagesContainer.FwdMessages;
                    if (messages != null)
                    {
                        for (var i = 0; i < messages.Count; i++)
                        {
                            var fwdMessage = messages[i];
                            var previousMsg = i == 0 ? message : messages[i - 1];
                            var isLastMsg = i == messages.Count - 1;
                            _database.AddSendingMessage(fwdMessage, previousMsg, isLastMsg, isLastMsg);
                        }
                    }
                }
            }

            _database.Commit();

            TLUtils.WritePerformance("SyncSendingMessage time: " + timer.Elapsed);
            callback(result);
        }

        public void SyncSendingDecryptedMessage(TLInt chatId, TLInt date, TLLong randomId, Action<TLDecryptedMessageBase> callback)
        {
            TLDecryptedMessageBase result = null;
            if (_database == null) Init();

            if (DecryptedMessagesContext != null)
            {
                result = GetCachedDecryptedMessage(randomId);
            }

            if (result == null)
            {
                callback(null);
                return;
            }

            _database.UpdateSendingDecryptedMessage(chatId, date, result);

            _database.Commit();

            callback(result);
        }

        public void SyncDecryptedMessages(IList<WindowsPhone.Tuple<TLDecryptedMessageBase, TLObject>> tuples, TLEncryptedChatBase peer, Action<IList<WindowsPhone.Tuple<TLDecryptedMessageBase, TLObject>>> callback)
        {
            if (tuples == null)
            {
                callback(null);
                return;
            }

            var timer = Stopwatch.StartNew();

            var result = tuples;
            if (_database == null) Init();

            foreach (var tuple in tuples)
            {
                TLDecryptedMessageBase cachedMessage = null;

                if (DecryptedMessagesContext != null)
                {
                    cachedMessage = GetCachedDecryptedMessage(tuple.Item1);
                }

                if (cachedMessage != null)
                {
                    // update fields
                    if (tuple.Item1.GetType() == cachedMessage.GetType())
                    {
                        cachedMessage.Update(tuple.Item1);
                    }

                    tuple.Item1 = cachedMessage;
                }
                else
                {
                    // add object to cache
                    _database.AddDecryptedMessage(tuple.Item1, peer);
                }
            }

            _database.Commit();

            TLUtils.WritePerformance("Sync DecryptedMessage time: " + timer.Elapsed);
            callback(result);
        }

        public void SyncDecryptedMessage(TLDecryptedMessageBase message, TLEncryptedChatBase peer, Action<TLDecryptedMessageBase> callback)
        {
            if (message == null)
            {
                callback(null);
                return;
            }

            var timer = Stopwatch.StartNew();

            var result = message;
            if (_database == null) Init();

            TLDecryptedMessageBase cachedMessage = null;

            if (DecryptedMessagesContext != null)
            {
                cachedMessage = GetCachedDecryptedMessage(message);
            }

            if (cachedMessage != null)
            {
                // update fields
                if (message.GetType() == cachedMessage.GetType())
                {
                    cachedMessage.Update(message);
                }

                result = cachedMessage;
            }
            else
            {
                // add object to cache
                _database.AddDecryptedMessage(message, peer);
            }

            _database.Commit();

            TLUtils.WritePerformance("Sync DecryptedMessage time: " + timer.Elapsed);
            callback(result);
        }

        public ExceptionInfo LastSyncMessageException { get; set; }

        public void SyncMessage(TLMessageBase message, Action<TLMessageBase> callback)
        {
            SyncMessage(message, true, true, callback);
        }

        public void SyncEditedMessage(TLMessageBase message, bool notifyNewDialog, bool notifyTopMessageUpdated, Action<TLMessageBase> callback)
        {
            try
            {
                if (message == null)
                {
                    callback(null);
                    return;
                }

                var result = message;
                if (_database == null) Init();

                var cachedMessage = GetCachedMessage(message);

                if (cachedMessage != null)
                {
                    if (cachedMessage.RandomId != null)
                    {
                        _database.RemoveMessageFromContext(cachedMessage);

                        if (cachedMessage.Index != 0)
                        {
                            cachedMessage.RandomId = null;
                        }

                        _database.AddMessageToContext(cachedMessage);
                    }

                    if (message.GetType() == cachedMessage.GetType())
                    {
                        cachedMessage.Edit(message);
                    }
                    else
                    {
                        _database.RemoveMessageFromContext(cachedMessage);
                        _database.AddMessage(message, notifyNewDialog, notifyTopMessageUpdated);
                    }
                    result = cachedMessage;
                }

                _database.Commit();
                callback(result);
            }
            catch (Exception ex)
            {
                LastSyncMessageException = new ExceptionInfo
                {
                    Caption = "CacheService.SyncMessage",
                    Exception = ex,
                    Timestamp = DateTime.Now
                };

                TLUtils.WriteException("CacheService.SyncMessage", ex);
            }
        }

        public void SyncMessage(TLMessageBase message, bool notifyNewDialog, bool notifyTopMessageUpdated, Action<TLMessageBase> callback)
        {
            try
            {
                if (message == null)
                {
                    callback(null);
                    return;
                }

                var result = message;
                if (_database == null) Init();

                var cachedMessage = GetCachedMessage(message);

                if (cachedMessage != null)
                {
                    if (cachedMessage.RandomId != null)
                    {
                        _database.RemoveMessageFromContext(cachedMessage);

                        if (cachedMessage.Index != 0)
                        {
                            cachedMessage.RandomId = null;
                        }

                        _database.AddMessageToContext(cachedMessage);
                    }

                    if (message.GetType() == cachedMessage.GetType())
                    {
                        cachedMessage.Update(message);
                    }
                    else
                    {
                        _database.DeleteMessage(cachedMessage);
                        _database.AddMessage(message, notifyNewDialog, notifyTopMessageUpdated);
                    }
                    result = cachedMessage;
                }
                else
                {
                    try
                    {
                        _database.AddMessage(message, notifyNewDialog, notifyTopMessageUpdated);
                    }
                    catch (Exception ex)
                    {
                        LastSyncMessageException = new ExceptionInfo { Exception = ex, Timestamp = DateTime.Now };
                        Helpers.Execute.ShowDebugMessage("SyncMessage ex:\n" + ex);
                    }
                }

                _database.Commit();
                callback(result);
            }
            catch (Exception ex)
            {
                LastSyncMessageException = new ExceptionInfo
                {
                    Caption = "CacheService.SyncMessage",
                    Exception = ex,
                    Timestamp = DateTime.Now
                };

                TLUtils.WriteException("CacheService.SyncMessage", ex);
            }
        }

        public void SyncPeerMessages(TLPeerBase peer, TLMessagesBase messages, bool notifyNewDialog, bool notifyTopMessageUpdated, Action<TLMessagesBase> callback)
        {
            if (messages == null)
            {
                callback(new TLMessages());
                return;
            }

            var timer = Stopwatch.StartNew();

            var result = messages.GetEmptyObject();
            if (_database == null) Init();

            ProcessPeerReading(peer, messages);

            SyncChatsInternal(messages.Chats, result.Chats);
            SyncUsersInternal(messages.Users, result.Users);
            SyncMessagesInternal(peer, messages.Messages, result.Messages, notifyNewDialog, notifyTopMessageUpdated);

            _database.Commit();

            //TLUtils.WritePerformance("SyncPeerMessages time: " + timer.Elapsed);
            callback(result);
        }

        private void ProcessPeerReading(TLPeerBase peer, TLMessagesBase messages)
        {
            IReadMaxId readMaxId = null;
            if (peer is TLPeerUser)
            {
                readMaxId = GetUser(peer.Id) as IReadMaxId;
            }
            else if (peer is TLPeerChat)
            {
                readMaxId = GetChat(peer.Id) as IReadMaxId;
            }
            else if (peer is TLPeerChannel)
            {
                readMaxId = GetChat(peer.Id) as IReadMaxId;
            }

            if (readMaxId != null)
            {
                foreach (var message in messages.Messages)
                {
                    var messageCommon = message as TLMessageCommon;
                    if (messageCommon != null)
                    {
                        if (messageCommon.Out.Value
                            && readMaxId.ReadOutboxMaxId != null
                            && readMaxId.ReadOutboxMaxId.Value >= 0
                            && readMaxId.ReadOutboxMaxId.Value < messageCommon.Index)
                        {
                            messageCommon.SetUnreadSilent(TLBool.True);
                        }
                        else if (!messageCommon.Out.Value
                            && readMaxId.ReadInboxMaxId != null
                            && readMaxId.ReadInboxMaxId.Value >= 0
                            && readMaxId.ReadInboxMaxId.Value < messageCommon.Index)
                        {
                            messageCommon.SetUnreadSilent(TLBool.True);
                        }
                    }
                }
            }
        }

        public void AddMessagesToContext(TLMessagesBase messages, Action<TLMessagesBase> callback)
        {
            if (messages == null)
            {
                callback(new TLMessages());
                return;
            }

            var timer = Stopwatch.StartNew();

            var result = messages.GetEmptyObject();
            if (_database == null) Init();

            SyncChatsInternal(messages.Chats, result.Chats);
            SyncUsersInternal(messages.Users, result.Users);
            foreach (var message in messages.Messages)
            {
                if (GetCachedMessage(message) == null)
                {
                    _database.AddMessageToContext(message);
                }
            }

            _database.Commit();

            //TLUtils.WritePerformance("SyncPeerMessages time: " + timer.Elapsed);
            callback(result);
        }

        public void SyncStatuses(TLVector<TLContactStatusBase> contactStatuses, Action<TLVector<TLContactStatusBase>> callback)
        {
            if (contactStatuses == null)
            {
                callback(new TLVector<TLContactStatusBase>());
                return;
            }

            var timer = Stopwatch.StartNew();

            var result = contactStatuses;
            if (_database == null) Init();

            foreach (var contactStatus in contactStatuses)
            {
                var contactStatus19 = contactStatus as TLContactStatus19;
                if (contactStatus19 != null)
                {
                    var userId = contactStatus.UserId;
                    var user = GetUser(userId);
                    if (user != null)
                    {
                        user._status = contactStatus19.Status;
                    }
                }
            }

            _database.Commit();

            //TLUtils.WritePerformance("SyncPeerMessages time: " + timer.Elapsed);
            callback(result);
        }

        public void SyncDifference(TLDifference difference, Action<TLDifference> callback, IList<ExceptionInfo> exceptions)
        {
            if (difference == null)
            {
                callback(new TLDifference());
                return;
            }

            var timer = Stopwatch.StartNew();

            var result = (TLDifference)difference.GetEmptyObject();
            if (_database == null) Init();

            SyncChatsInternal(difference.Chats, result.Chats, exceptions);
            SyncUsersInternal(difference.Users, result.Users, exceptions);
            SyncMessagesInternal(null, difference.NewMessages, result.NewMessages, false, false, exceptions);
            SyncEncryptedMessagesInternal(difference.State.Qts, difference.NewEncryptedMessages, result.NewEncryptedMessages, exceptions);

            _database.Commit();

            //TLUtils.WritePerformance("Sync difference time: " + timer.Elapsed);
            callback(result);
        }

        public void SyncDifferenceWithoutUsersAndChats(TLDifference difference, Action<TLDifference> callback, IList<ExceptionInfo> exceptions)
        {
            if (difference == null)
            {
                callback(new TLDifference());
                return;
            }

            var timer = Stopwatch.StartNew();

            var result = (TLDifference)difference.GetEmptyObject();
            if (_database == null) Init();

            //SyncChatsInternal(difference.Chats, result.Chats, exceptions);
            //SyncUsersInternal(difference.Users, result.Users, exceptions);

            foreach (var messageBase in difference.NewMessages)
            {
                MTProtoService.ProcessSelfMessage(messageBase);
            }

            SyncMessagesInternal(null, difference.NewMessages, result.NewMessages, false, false, exceptions);
            SyncEncryptedMessagesInternal(difference.State.Qts, difference.NewEncryptedMessages, result.NewEncryptedMessages, exceptions);

            _database.Commit();

            //TLUtils.WritePerformance("Sync difference time: " + timer.Elapsed);
            callback(result);
        }

        private void SyncMessageInternal(TLPeerBase peer, TLMessageBase message, out TLMessageBase result)
        {
            TLMessageCommon cachedMessage = null;
            //if (MessagesContext != null)
            {
                cachedMessage = (TLMessageCommon)GetCachedMessage(message);
                //cachedMessage = (TLMessageCommon)MessagesContext[message.Index];
            }

            if (cachedMessage != null)
            {
                if (cachedMessage.RandomId != null)
                {
                    _database.RemoveMessageFromContext(cachedMessage);

                    cachedMessage.RandomId = null;

                    _database.AddMessageToContext(cachedMessage);

                }

                // update fields
                if (message.GetType() == cachedMessage.GetType())
                {
                    cachedMessage.Update(message);
                    //_database.Storage.Modify(cachedMessage);
                }
                // or replace object
                else
                {
                    _database.DeleteMessage(cachedMessage);
                    _database.AddMessage(message);
                }
                result = cachedMessage;
            }
            else
            {
                // add object to cache
                result = message;
                _database.AddMessage(message);
            }
        }

        private void SyncMessagesInternal(TLPeerBase peer, IEnumerable<TLMessageBase> messages, TLVector<TLMessageBase> result, bool notifyNewDialogs, bool notifyTopMessageUpdated, IList<ExceptionInfo> exceptions = null)
        {
            TLChannel channel = null;
            TLInt readInboxMaxId;
            if (peer is TLPeerChannel)
            {
                channel = GetChat(peer.Id) as TLChannel;
            }

            foreach (var message in messages)
            {
                try
                {
                    // for updates we have input message only and set peer to null by default
                    if (peer == null)
                    {
                        peer = TLUtils.GetPeerFromMessage(message);

                        if (peer is TLPeerChannel)
                        {
                            channel = GetChat(peer.Id) as TLChannel;
                            if (channel != null)
                            {
                                readInboxMaxId = channel.ReadInboxMaxId;
                                if (readInboxMaxId != null)
                                {
                                    var messageCommon = message as TLMessageCommon;
                                    if (messageCommon != null && !messageCommon.Out.Value &&
                                        messageCommon.Index > readInboxMaxId.Value)
                                    {
                                        messageCommon.SetUnreadSilent(TLBool.True);
                                    }
                                }
                            }
                        }
                    }

                    var cachedMessage = (TLMessageCommon)GetCachedMessage(message);

                    if (cachedMessage != null)
                    {
                        if (message.GetType() == cachedMessage.GetType())
                        {
                            cachedMessage.Update(message);
                        }
                        else
                        {
                            _database.DeleteMessage(cachedMessage);
                            _database.AddMessage(message);
                        }
                        result.Add(cachedMessage);
                    }
                    else
                    {
                        if (peer != null)
                        {
                            if (channel != null)
                            {
                                readInboxMaxId = channel.ReadInboxMaxId;
                                if (readInboxMaxId != null)
                                {
                                    var messageCommon = message as TLMessageCommon;
                                    if (messageCommon != null && !messageCommon.Out.Value &&
                                        messageCommon.Index > readInboxMaxId.Value)
                                    {
                                        messageCommon.SetUnreadSilent(TLBool.True);
                                    }
                                }
                            }
                        }

                        result.Add(message);
                        _database.AddMessage(message, notifyNewDialogs, notifyTopMessageUpdated);
                    }
                }
                catch (Exception ex)
                {
                    if (exceptions != null)
                    {
                        exceptions.Add(new ExceptionInfo
                        {
                            Caption = "UpdatesService.ProcessDifference Messages",
                            Exception = ex,
                            Timestamp = DateTime.Now
                        });
                    }

                    TLUtils.WriteException("UpdatesService.ProcessDifference Messages", ex);
                }
            }
        }

        #endregion

        #region Dialogs

        private void SyncDialogsInternal(Stopwatch stopwatch2, TLDialogsBase dialogs, TLDialogsBase result)
        {
            MergeMessagesAndChannels(dialogs);

            //Debug.WriteLine("messages.getDialogs sync dialogs merge messages and channels elapsed=" + stopwatch.Elapsed);

            foreach (TLDialog dialog in dialogs.Dialogs)
            {
                //Debug.WriteLine("messages.getDialogs sync dialogs start get cached elapsed=" + stopwatch.Elapsed);
                TLDialog cachedDialog = null;
                if (DialogsContext != null)
                {
                    cachedDialog = DialogsContext[dialog.Index] as TLDialog;
                }
                //Debug.WriteLine("messages.getDialogs sync dialogs stop get cached elapsed=" + stopwatch.Elapsed);

                if (cachedDialog != null)
                {
                    //Debug.WriteLine("messages.getDialogs sync dialogs start update cached elapsed=" + stopwatch.Elapsed);
                    var raiseTopMessageUpdated = cachedDialog.TopMessageId == null || cachedDialog.TopMessageId.Value != dialog.TopMessageId.Value;
                    cachedDialog.Update(dialog);
                    //Debug.WriteLine("messages.getDialogs sync dialogs stop update cached elapsed=" + stopwatch.Elapsed);
                    if (raiseTopMessageUpdated)
                    {
                        if (_eventAggregator != null)
                        {
                            _eventAggregator.Publish(new TopMessageUpdatedEventArgs(cachedDialog, cachedDialog.TopMessage));
                        }
                    }
                    result.Dialogs.Add(cachedDialog);
                }
                else
                {
                    result.Dialogs.Add(dialog);

                    // skip left and not promo dialogs
                    var peerChannel = dialog.Peer as TLPeerChannel;
                    if (peerChannel != null)
                    {
                        var channel = GetChat(peerChannel.Id) as TLChannel;
                        var dialog71 = dialog as TLDialog71;
                        if (channel != null && channel.Left.Value && dialog71 != null && !dialog71.IsPromo)
                        {
                            continue;
                        }
                    }

                    //Debug.WriteLine("messages.getDialogs sync dialogs start add none cached elapsed=" + stopwatch.Elapsed);
                    // add object to cache
                    _database.AddDialog(dialog);

                    //Debug.WriteLine("messages.getDialogs sync dialogs stop add none cached elapsed=" + stopwatch.Elapsed);
                }
            }

            //Debug.WriteLine("messages.getDialogs sync dialogs foreach elapsed=" + stopwatch.Elapsed);



            result.Messages = dialogs.Messages;
        }

        private void SyncChannelDialogsInternal(TLDialogsBase dialogs, TLDialogsBase result)
        {
            // set TopMessage properties
            var timer = Stopwatch.StartNew();
            MergeMessagesAndChannels(dialogs);
            //TLUtils.WritePerformance("Dialogs:: merge dialogs and messages " + timer.Elapsed);

            timer = Stopwatch.StartNew();
            foreach (TLDialog dialog in dialogs.Dialogs)
            {
                TLDialog cachedDialog = null;
                if (DialogsContext != null)
                {
                    cachedDialog = DialogsContext[dialog.Index] as TLDialog;
                }

                if (cachedDialog != null)
                {
                    var raiseTopMessageUpdated = cachedDialog.TopMessageId == null || cachedDialog.TopMessageId.Value != dialog.TopMessageId.Value;
                    cachedDialog.Update(dialog);
                    if (raiseTopMessageUpdated)
                    {
                        if (_eventAggregator != null)
                        {
                            _eventAggregator.Publish(new TopMessageUpdatedEventArgs(cachedDialog, cachedDialog.TopMessage));
                        }
                    }
                    result.Dialogs.Add(cachedDialog);
                }
                else
                {
                    // add object to cache
                    result.Dialogs.Add(dialog);
                    _database.AddDialog(dialog);
                }
            }
            //TLUtils.WritePerformance("Dialogs:: foreach dialogs " + timer.Elapsed);



            result.Messages = dialogs.Messages;
        }

        public void SyncDialogs(Stopwatch stopwatch, TLDialogsBase dialogs, Action<TLDialogsBase> callback)
        {
            if (dialogs == null)
            {
                callback(new TLDialogs());
                return;
            }


            var result = dialogs.GetEmptyObject();
            if (_database == null) Init();

            //Debug.WriteLine("messages.getDialogs after init elapsed=" + stopwatch.Elapsed);

            MergeReadMaxIdAndNotifySettings(dialogs);

            //Debug.WriteLine("messages.getDialogs merge notify settings elapsed=" + stopwatch.Elapsed);

            SyncChatsInternal(dialogs.Chats, result.Chats);

            //Debug.WriteLine("messages.getDialogs sync chats elapsed=" + stopwatch.Elapsed);

            SyncUsersInternal(dialogs.Users, result.Users);

            //Debug.WriteLine("messages.getDialogs sync users elapsed=" + stopwatch.Elapsed);

            SyncDialogsInternal(stopwatch, dialogs, result);

            //Debug.WriteLine("messages.getDialogs end sync dialogs elapsed=" + stopwatch.Elapsed);

            _database.Commit();

            //Debug.WriteLine("messages.getDialogs after commit elapsed=" + stopwatch.Elapsed);

            callback.SafeInvoke(result);
        }

        public void SyncProxyData(TLProxyDataBase proxyData, Action<TLProxyDataBase> callback)
        {
            var result = proxyData != null ? proxyData.GetEmptyObject() : null;

            var proxyDataPromo = proxyData as TLProxyDataPromo;
            if (proxyDataPromo != null)
            {
                SyncChatsInternal(proxyDataPromo.Chats, ((TLProxyDataPromo)result).Chats);
                SyncUsersInternal(proxyDataPromo.Users, ((TLProxyDataPromo)result).Users);
            }

            _database.UpdateProxyData(proxyData);

            _database.Commit();

            callback.SafeInvoke(result);
        }

        public void SyncChannelDialogs(TLDialogsBase dialogs, Action<TLDialogsBase> callback)
        {
            if (dialogs == null)
            {
                callback(new TLDialogs());
                return;
            }

            var result = dialogs.GetEmptyObject();
            if (_database == null) Init();

            MergeReadMaxIdAndNotifySettings(dialogs);

            // add or update chats, users and messages
            var timer = Stopwatch.StartNew();
            SyncChatsInternal(dialogs.Chats, result.Chats);
            //TLUtils.WritePerformance("Dialogs:: sync chats " + timer.Elapsed);

            timer = Stopwatch.StartNew();
            SyncUsersInternal(dialogs.Users, result.Users);
            //TLUtils.WritePerformance("Dialogs:: sync users " + timer.Elapsed);

            //SyncMessagesInternal(dialogs.Messages, result.Messages);
            timer = Stopwatch.StartNew();
            SyncChannelDialogsInternal(dialogs, result);
            //TLUtils.WritePerformance("Dialogs:: sync dialogs " + timer.Elapsed);

            _database.Commit();

            TLUtils.WritePerformance("SyncDialogs time: " + timer.Elapsed);
            callback.SafeInvoke(result);
        }

        private void MergeReadMaxIdAndNotifySettings(TLDialogsBase dialogs)
        {
            var chatsIndex = new Dictionary<int, TLChatBase>();
            foreach (var chat in dialogs.Chats)
            {
                chatsIndex[chat.Index] = chat;
            }

            var usersIndex = new Dictionary<int, TLUserBase>();
            foreach (var user in dialogs.Users)
            {
                usersIndex[user.Index] = user;
            }

            foreach (var dialog in dialogs.Dialogs)
            {
                if (dialog.NotifySettings != null)
                {
                    if (dialog.Peer is TLPeerChannel)
                    {
                        TLChatBase chat;
                        if (chatsIndex.TryGetValue(dialog.Index, out chat))
                        {
                            chat.NotifySettings = dialog.NotifySettings;
                        }
                    }
                    else if (dialog.Peer is TLPeerChat)
                    {
                        TLChatBase chat;
                        if (chatsIndex.TryGetValue(dialog.Index, out chat))
                        {
                            chat.NotifySettings = dialog.NotifySettings;
                        }
                    }
                    else if (dialog.Peer is TLPeerUser)
                    {
                        TLUserBase user;
                        if (usersIndex.TryGetValue(dialog.Index, out user))
                        {
                            user.NotifySettings = dialog.NotifySettings;
                        }
                    }
                }

                var dialog53 = dialog as IReadMaxId;
                if (dialog53 != null)
                {
                    if (dialog.Peer is TLPeerChannel)
                    {
                        TLChatBase chatBase;
                        if (chatsIndex.TryGetValue(dialog.Index, out chatBase))
                        {
                            var chat = chatBase as IReadMaxId;
                            if (chat != null)
                            {
                                chat.ReadInboxMaxId = dialog53.ReadInboxMaxId;
                                chat.ReadOutboxMaxId = dialog53.ReadOutboxMaxId;
                            }
                        }
                    }
                    else if (dialog.Peer is TLPeerChat)
                    {
                        TLChatBase chatBase;
                        if (chatsIndex.TryGetValue(dialog.Index, out chatBase))
                        {
                            var chat = chatBase as IReadMaxId;
                            if (chat != null)
                            {
                                chat.ReadInboxMaxId = dialog53.ReadInboxMaxId;
                                chat.ReadOutboxMaxId = dialog53.ReadOutboxMaxId;
                            }
                        }
                    }
                    else if (dialog.Peer is TLPeerUser)
                    {
                        TLUserBase userBase;
                        if (usersIndex.TryGetValue(dialog.Index, out userBase))
                        {
                            var user = userBase as IReadMaxId;
                            if (user != null)
                            {
                                user.ReadInboxMaxId = dialog53.ReadInboxMaxId;
                                user.ReadOutboxMaxId = dialog53.ReadOutboxMaxId;
                            }
                        }
                    }
                }
            }
        }

        public void MergeMessagesAndChannels(TLDialogsBase dialogs)
        {
            var dialogsCache = new Context<TLDialog>();
            var messagesCache = new Context<Context<TLMessageBase>>();

            try
            {
                foreach (var dialogBase in dialogs.Dialogs)
                {
                    var dialog = dialogBase as TLDialog;
                    if (dialog != null)
                    {
                        var peerId = dialog.Peer.Id.Value;
                        dialogsCache[peerId] = dialog;
                    }
                }

                foreach (var messageBase in dialogs.Messages)
                {
                    var message = messageBase as TLMessageCommon;
                    if (message != null)
                    {
                        var peerId = message.ToId is TLPeerUser && !message.Out.Value ? message.FromId.Value : message.ToId.Id.Value;
                        if (!message.Out.Value)
                        {
                            TLDialog dialog;
                            if (dialogsCache.TryGetValue(peerId, out dialog))
                            {
                                var dialogChannel = dialog as TLDialogChannel;
                                if (dialogChannel != null
                                    && dialogChannel.ReadInboxMaxId.Value < message.Index)
                                {
                                    message.SetUnreadSilent(TLBool.True);
                                }
                            }
                        }

                        Context<TLMessageBase> dialogContext;
                        if (!messagesCache.TryGetValue(peerId, out dialogContext))
                        {
                            dialogContext = new Context<TLMessageBase>();
                            messagesCache[peerId] = dialogContext;
                        }
                        dialogContext[message.Index] = message;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            try
            {

                foreach (var dialogCache in messagesCache.Values)
                {

                    foreach (var message in dialogCache.Values)
                    {
                        TLMessageCommon cachedMessage = null;
                        //if (MessagesContext != null)
                        {
                            cachedMessage = (TLMessageCommon)GetCachedMessage(message);
                            //cachedMessage = (TLMessageCommon)MessagesContext[message.Index];
                        }

                        if (cachedMessage != null)
                        {
                            // update fields
                            if (message.GetType() == cachedMessage.GetType())
                            {
                                cachedMessage.Update(message);
                                //_database.Storage.Modify(cachedMessage);
                            }
                            // or replace object
                            else
                            {
                                _database.AddMessageToContext(message);
                            }
                        }
                        else
                        {
                            // add object to cache
                            _database.AddMessageToContext(message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            try
            {

                foreach (var dialogBase in dialogs.Dialogs)
                {
                    var peer = dialogBase.Peer;
                    if (peer is TLPeerUser)
                    {
                        dialogBase._with = UsersContext[peer.Id.Value];
                    }
                    else if (peer is TLPeerChat)
                    {
                        dialogBase._with = ChatsContext[peer.Id.Value];
                    }
                    else if (peer is TLPeerChannel)
                    {
                        dialogBase._with = ChatsContext[peer.Id.Value];
                    }

                    var dialogFeed = dialogBase as TLDialogFeed;
                    if (dialogFeed != null)
                    {
                        var channels = new TLVector<TLChatBase>();
                        foreach (var channelId in dialogFeed.FeedOtherChannels)
                        {
                            var ch = ChatsContext[channelId.Value];
                            if (ch != null)
                            {
                                channels.Add(ch);
                            }
                        }
                        dialogFeed._with = channels;
                    }

                    var dialog = dialogBase as TLDialog;
                    if (dialog != null)
                    {
                        dialog._topMessage = messagesCache[peer.Id.Value][dialogBase.TopMessageId.Value];
                        dialog.Messages = new ObservableCollection<TLMessageBase> { dialog.TopMessage };
                    }

                    //var dialogChannel = dialogBase as TLDialogChannel;
                    //if (dialog != null)
                    //{
                    //    dialog._topMessage = messagesCache[peer.Id.Value][dialogBase.TopMessageId.Value];
                    //    dialog.Messages = new ObservableCollection<TLMessageBase> { dialog.TopMessage };
                    //}
                }
            }
            catch (Exception ex)
            {

            }



        }

        #endregion

        #region Users

        public void SyncUserLink(TLLinkBase link, Action<TLLinkBase> callback)
        {
            if (link == null)
            {
                callback(null);
                return;
            }

            var timer = Stopwatch.StartNew();

            TLUserBase result;
            if (_database == null) Init();

            SyncUserInternal(link.User, out result);
            link.User = result;

            _database.Commit();

            TLUtils.WritePerformance("SyncUser time: " + timer.Elapsed);
            callback(link);
        }

        public void SyncUser(TLUserFull userFull, Action<TLUserFull> callback)
        {
            if (userFull == null)
            {
                callback(null);
                return;
            }

            var timer = Stopwatch.StartNew();

            TLUserBase result;
            if (_database == null) Init();

            SyncUserInternal(userFull.ToUser(), out result);
            userFull.User = result;

            var dialog = GetDialog(new TLPeerUser { Id = userFull.User.Id });
            if (dialog != null)
            {
                dialog.NotifySettings = userFull.NotifySettings;
            }

            _database.Commit();

            //TLUtils.WritePerformance("SyncUserFull time: " + timer.Elapsed);

            callback.SafeInvoke(userFull);
        }


        public void SyncUser(TLUserBase user, Action<TLUserBase> callback)
        {
            if (user == null)
            {
                callback(null);
                return;
            }

            var timer = Stopwatch.StartNew();

            TLUserBase result;
            if (_database == null) Init();

            SyncUserInternal(user, out result);

            _database.Commit();

            TLUtils.WritePerformance("SyncUser time: " + timer.Elapsed);
            callback(result);
        }

        public void SyncUsers(TLVector<TLUserBase> users, Action<TLVector<TLUserBase>> callback)
        {
            if (users == null)
            {
                callback(new TLVector<TLUserBase>());
                return;
            }

            var timer = Stopwatch.StartNew();

            var result = new TLVector<TLUserBase>();
            if (_database == null) Init();

            SyncUsersInternal(users, result);

            _database.Commit();

            TLUtils.WritePerformance("SyncUsers time: " + timer.Elapsed);
            callback(result);
        }

        public void SyncUsersAndChats(TLVector<TLUserBase> users, TLVector<TLChatBase> chats, Action<WindowsPhone.Tuple<TLVector<TLUserBase>, TLVector<TLChatBase>>> callback)
        {
            if (users == null && chats == null)
            {
                callback(new WindowsPhone.Tuple<TLVector<TLUserBase>, TLVector<TLChatBase>>(null, null));
                return;
            }

            var timer = Stopwatch.StartNew();

            var usersResult = new TLVector<TLUserBase>();
            var chatsResult = new TLVector<TLChatBase>();
            if (_database == null) Init();

            SyncUsersInternal(users, usersResult);
            SyncChatsInternal(chats, chatsResult);

            _database.Commit();

            TLUtils.WritePerformance("SyncUsersAndChats time: " + timer.Elapsed);
            callback(new WindowsPhone.Tuple<TLVector<TLUserBase>, TLVector<TLChatBase>>(usersResult, chatsResult));
        }

        private void SyncUserInternal(TLUserBase user, out TLUserBase result)
        {
            TLUserBase cachedUser = null;
            if (UsersContext != null)
            {
                cachedUser = UsersContext[user.Index];
            }

            if (cachedUser != null)
            {
                var user45 = user as TLUser45;
                var isMinUser = user45 != null && user45.Min;

                // update fields
                if (user.GetType() == cachedUser.GetType())
                {
                    cachedUser.Update(user);
                    result = cachedUser;
                }
                else if (isMinUser)
                {
                    result = cachedUser;
                }
                // or replace object
                else
                {
                    _database.ReplaceUser(user.Index, user);
                    result = user;
                }
            }
            else
            {
                // add object to cache
                result = user;
                _database.AddUser(user);
            }
        }

        private void SyncUsersInternal(TLVector<TLUserBase> users, TLVector<TLUserBase> result, IList<ExceptionInfo> exceptions = null)
        {
            foreach (var user in users)
            {
                try
                {
                    TLUserBase cachedUser = null;
                    if (UsersContext != null)
                    {
                        cachedUser = UsersContext[user.Index];
                    }

                    if (cachedUser != null)
                    {
                        var user45 = user as TLUser45;
                        var isMinUser = user45 != null && user45.Min;

                        // update fields
                        if (user.GetType() == cachedUser.GetType())
                        {
                            cachedUser.Update(user);
                            result.Add(cachedUser);
                        }
                        else if (isMinUser)
                        {
                            result.Add(cachedUser);
                        }
                        // or replace object
                        else
                        {
                            _database.ReplaceUser(user.Index, user);
                            result.Add(user);
                        }
                    }
                    else
                    {
                        // add object to cache
                        result.Add(user);
                        _database.AddUser(user);
                    }
                }
                catch (Exception ex)
                {
                    if (exceptions != null)
                    {
                        exceptions.Add(new ExceptionInfo
                        {
                            Caption = "UpdatesService.ProcessDifference Users",
                            Exception = ex,
                            Timestamp = DateTime.Now
                        });
                    }

                    TLUtils.WriteException("UpdatesService.ProcessDifference Users", ex);
                }
            }
        }

        #endregion

        #region SecretChats

        private void SyncEncryptedChatInternal(TLEncryptedChatBase chat, out TLEncryptedChatBase result)
        {
            try
            {
                TLEncryptedChatBase cachedChat = null;
                if (EncryptedChatsContext != null)
                {
                    cachedChat = EncryptedChatsContext[chat.Index];
                }

                if (cachedChat != null)
                {
                    // update fields
                    if (chat.GetType() == cachedChat.GetType())
                    {
                        cachedChat.Update(chat);
                        result = cachedChat;
                    }
                    // or replace object
                    else
                    {
                        var chatWaiting = cachedChat as TLEncryptedChatWaiting;
                        if (chatWaiting != null)
                        {
                            var encryptedChat = chat as TLEncryptedChat;
                            if (encryptedChat != null)
                            {
                                chat.A = cachedChat.A;
                                chat.P = cachedChat.P;
                                chat.G = cachedChat.G;

                                if (!TLUtils.CheckGaAndGb(encryptedChat.GAorB.Data, chat.P.Data))
                                {
                                    result = chat;
                                    return;
                                }

                                var gbBytes = encryptedChat.GAorB.ToBytes();
                                var authKey = MTProtoService.GetAuthKey(chat.A.Data, gbBytes, chat.P.ToBytes());
                                chat.Key = TLString.FromBigEndianData(authKey);

                                var authKeyFingerprint = Utils.ComputeSHA1(authKey);
                                chat.KeyFingerprint = new TLLong(BitConverter.ToInt64(authKeyFingerprint, 12));
                            }
                            else
                            {
                                if (cachedChat.Key != null) chat.Key = cachedChat.Key;
                                if (cachedChat.KeyFingerprint != null) chat.KeyFingerprint = cachedChat.KeyFingerprint;
                            }
                        }
                        //chat.A = cachedChat.A;
                        //chat.P = cachedChat.P;
                        //chat.G = cachedChat.G;

                        //var encryptedChat = chat as TLEncryptedChat;
                        //if (encryptedChat != null)
                        //{
                        //    var gbBytes = encryptedChat.GAorB.ToBytes();
                        //    var authKey = MTProtoService.GetAuthKey(chat.A.Data, gbBytes, chat.P.ToBytes());
                        //    chat.Key = TLString.FromBigEndianData(authKey);

                        //    var authKeyFingerprint = Utils.ComputeSHA1(authKey);
                        //    chat.KeyFingerprint = new TLLong(BitConverter.ToInt64(authKeyFingerprint, 12));
                        //}
                        //else
                        //{
                        //    if (cachedChat.Key != null) chat.Key = cachedChat.Key;
                        //    if (cachedChat.KeyFingerprint != null) chat.KeyFingerprint = cachedChat.KeyFingerprint;
                        //}

                        //Helpers.Execute.ShowDebugMessage(string.Format("InMemoryCacheService.SyncEncryptedChatInternal {0}!={1}", cachedChat.GetType(), chat.GetType()));

                        _database.ReplaceEncryptedChat(chat.Index, chat);

                        result = chat;
                    }
                }
                else
                {
                    // add object to cache
                    result = chat;
                    _database.AddEncryptedChat(chat);
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
        }

        public void SyncEncryptedChat(TLEncryptedChatBase encryptedChat, Action<TLEncryptedChatBase> callback)
        {
            if (encryptedChat == null)
            {
                callback(null);
                return;
            }

            TLEncryptedChatBase chatResult;
            if (_database == null) Init();

            SyncEncryptedChatInternal(encryptedChat, out chatResult);

            _database.Commit();

            callback.SafeInvoke(chatResult);
        }

        public void SyncEncryptedMessagesInternal(TLInt qts, TLVector<TLEncryptedMessageBase> messages, TLVector<TLEncryptedMessageBase> result, IList<ExceptionInfo> exceptions = null)
        {
            foreach (var message in messages)
            {
                try
                {
                    var encryptedChatBase = GetEncryptedChat(message.ChatId);
                    var encryptedChat = encryptedChatBase as TLEncryptedChat;
                    if (encryptedChat == null)
                    {
                        result.Add(message);
                        Execute.ShowDebugMessage(string.Format("SyncEncryptedMessagesInternal skip decrypted message chatId={0} chat_type={1}", encryptedChatBase != null ? encryptedChatBase.Id.ToString() : "null", encryptedChatBase != null ? encryptedChatBase.GetType().ToString() : "null"));

                        continue;
                    }

                    bool commitChat;
                    var decryptedMessage = UpdatesService.GetDecryptedMessage(MTProtoService.Instance.CurrentUserId, encryptedChat, message, qts, out commitChat);
                    if (commitChat)
                    {
                        Commit();
                    }
                    if (decryptedMessage == null)
                    {
                        continue;
                    }

                    var seqNoMessage = decryptedMessage as ISeqNo;
                    var encryptedChat17 = encryptedChat as TLEncryptedChat17;
                    if (seqNoMessage != null
                        && encryptedChat17 != null)
                    {
                        var chatRawInSeqNo = encryptedChat17.RawInSeqNo.Value;
                        var messageRawInSeqNo = UpdatesService.GetRawInFromReceivedMessage(MTProtoService.Instance.CurrentUserId, encryptedChat17, seqNoMessage);

                        if (chatRawInSeqNo <= messageRawInSeqNo)
                        {
                            if (messageRawInSeqNo > chatRawInSeqNo)
                            {
                                Execute.ShowDebugMessage(string.Format("SyncEncryptedMessagesInternal decrypted message gap chatId={0} chatRawInSeqNo={1} messageRawInSeqNo={2}", encryptedChat17.Id, chatRawInSeqNo, messageRawInSeqNo));
                            }
                            encryptedChat17.RawInSeqNo = new TLInt(chatRawInSeqNo + 1);
                            SyncEncryptedChat(encryptedChat17, r => { });
                        }
                        else
                        {
                            Execute.ShowDebugMessage(string.Format("SyncEncryptedMessagesInternal skip old decrypted message chatId={0} chatRawInSeqNo={1} messageRawInSeqNo={2}", encryptedChat17.Id, chatRawInSeqNo, messageRawInSeqNo));
                            continue;
                        }
                    }

                    Execute.BeginOnThreadPool(() => _eventAggregator.Publish(decryptedMessage));

                    SyncDecryptedMessage(decryptedMessage, encryptedChat, cachedMessage =>
                    {
                        var decryptedMessageService = decryptedMessage as TLDecryptedMessageService;
                        if (decryptedMessageService != null)
                        {
                            var readMessagesAction = decryptedMessageService.Action as TLDecryptedMessageActionReadMessages;
                            if (readMessagesAction != null)
                            {
                                var items = GetDecryptedHistory(encryptedChat.Id.Value, 100);
                                foreach (var randomId in readMessagesAction.RandomIds)
                                {
                                    foreach (var item in items)
                                    {
                                        if (item.RandomId.Value == randomId.Value)
                                        {
                                            item.Status = MessageStatus.Read;
                                            if (item.TTL != null && item.TTL.Value > 0)
                                            {
                                                item.DeleteDate = new TLLong(DateTime.Now.Ticks + encryptedChat.MessageTTL.Value * TimeSpan.TicksPerSecond);
                                            }

                                            var m = item as TLDecryptedMessage17;
                                            if (m != null)
                                            {
                                                var decryptedMediaPhoto = m.Media as TLDecryptedMessageMediaPhoto;
                                                if (decryptedMediaPhoto != null)
                                                {
                                                    if (decryptedMediaPhoto.TTLParams == null)
                                                    {
                                                        var ttlParams = new TTLParams();
                                                        ttlParams.IsStarted = true;
                                                        ttlParams.Total = m.TTL.Value;
                                                        ttlParams.StartTime = DateTime.Now;
                                                        ttlParams.Out = m.Out.Value;

                                                        decryptedMediaPhoto._ttlParams = ttlParams;
                                                    }
                                                }

                                                var decryptedMediaVideo17 = m.Media as TLDecryptedMessageMediaVideo17;
                                                if (decryptedMediaVideo17 != null)
                                                {
                                                    if (decryptedMediaVideo17.TTLParams == null)
                                                    {
                                                        var ttlParams = new TTLParams();
                                                        ttlParams.IsStarted = true;
                                                        ttlParams.Total = m.TTL.Value;
                                                        ttlParams.StartTime = DateTime.Now;
                                                        ttlParams.Out = m.Out.Value;

                                                        decryptedMediaVideo17._ttlParams = ttlParams;
                                                    }
                                                }

                                                var decryptedMediaAudio17 = m.Media as TLDecryptedMessageMediaAudio17;
                                                if (decryptedMediaAudio17 != null)
                                                {
                                                    if (decryptedMediaAudio17.TTLParams == null)
                                                    {
                                                        var ttlParams = new TTLParams();
                                                        ttlParams.IsStarted = true;
                                                        ttlParams.Total = m.TTL.Value;
                                                        ttlParams.StartTime = DateTime.Now;
                                                        ttlParams.Out = m.Out.Value;

                                                        decryptedMediaAudio17._ttlParams = ttlParams;
                                                    }
                                                }

                                                var decryptedMediaDocument45 = m.Media as TLDecryptedMessageMediaDocument45;
                                                if (decryptedMediaDocument45 != null && (m.IsVoice() || m.IsVideo()))
                                                {
                                                    if (decryptedMediaDocument45.TTLParams == null)
                                                    {
                                                        var ttlParams = new TTLParams();
                                                        ttlParams.IsStarted = true;
                                                        ttlParams.Total = m.TTL.Value;
                                                        ttlParams.StartTime = DateTime.Now;
                                                        ttlParams.Out = m.Out.Value;

                                                        decryptedMediaDocument45._ttlParams = ttlParams;
                                                    }

                                                    var message45 = m as TLDecryptedMessage45;
                                                    if (message45 != null)
                                                    {
                                                        message45.SetListened();
                                                    }
                                                    decryptedMediaDocument45.NotListened = false;
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        var isDisplayedMessage = TLUtils.IsDisplayedDecryptedMessageInternal(decryptedMessage);
                        if (!isDisplayedMessage)
                        {
                            decryptedMessage.Unread = TLBool.False;
                        }

                        UpdatesService.ProcessPFS(MTProtoService.Instance.SendEncryptedServiceAsync, this, _eventAggregator, encryptedChat, decryptedMessageService);

                        UpdatesService.ProcessNewLayer(MTProtoService.Instance.SendEncryptedServiceAsync, this, _eventAggregator, encryptedChat, decryptedMessage);

                        if (decryptedMessageService != null)
                        {
                            var resendAction = decryptedMessageService.Action as TLDecryptedMessageActionResend;
                            if (resendAction != null)
                            {
                                Execute.ShowDebugMessage(string.Format("SyncEncryptedMessagesInternal TLDecryptedMessageActionResend start_seq_no={0} end_seq_no={1}", resendAction.StartSeqNo, resendAction.EndSeqNo));
                            }
                        }
                    });

                    result.Add(message);
                }
                catch (Exception ex)
                {
                    if (exceptions != null)
                    {
                        exceptions.Add(new ExceptionInfo
                        {
                            Caption = "UpdatesService.ProcessDifference EncryptedMessages",
                            Exception = ex,
                            Timestamp = DateTime.Now
                        });
                    }

                    TLUtils.WriteException("UpdatesService.ProcessDifference EncryptedMessages", ex);
                }
            }
        }
        #endregion

        #region Chats

        public void AddChats(TLVector<TLChatBase> chats, Action<TLVector<TLChatBase>> callback)
        {
            if (chats == null)
            {
                callback(null);
                return;
            }

            if (_database == null) Init();

            foreach (var chat in chats)
            {
                TLChatBase cachedChat = null;
                if (ChatsContext != null)
                {
                    cachedChat = ChatsContext[chat.Index];
                }

                if (cachedChat == null)
                {
                    _database.AddChat(chat);
                }
            }

            _database.Commit();

            callback.SafeInvoke(chats);
        }

        public void SyncChat(TLMessagesChatFull messagesChatFull, Action<TLMessagesChatFull> callback)
        {
            if (messagesChatFull == null)
            {
                callback(null);
                return;
            }

            var usersResult = new TLVector<TLUserBase>(messagesChatFull.Users.Count);
            var chatsResult = new TLVector<TLChatBase>(messagesChatFull.Chats.Count);
            var currentChat = messagesChatFull.Chats.First(x => x.Index == messagesChatFull.FullChat.Id.Value);
            TLChatBase chatResult;
            if (_database == null) Init();

            SyncUsersInternal(messagesChatFull.Users, usersResult);
            messagesChatFull.Users = usersResult;

            SyncChatsInternal(messagesChatFull.Chats, chatsResult);
            messagesChatFull.Chats = chatsResult;

            SyncChatInternal(messagesChatFull.FullChat.ToChat(currentChat), out chatResult);

            var channel = currentChat as TLChannel;
            var dialog = GetDialog(channel != null ? (TLPeerBase)new TLPeerChannel { Id = messagesChatFull.FullChat.Id } : new TLPeerChat { Id = messagesChatFull.FullChat.Id });
            if (dialog != null)
            {
                dialog.NotifySettings = messagesChatFull.FullChat.NotifySettings;
            }

            _database.Commit();

            //TLUtils.WritePerformance("SyncChatFull time: " + timer.Elapsed);

            callback.SafeInvoke(messagesChatFull);
        }

        public void SyncChats(TLVector<TLChatBase> chats, Action<TLVector<TLChatBase>> callback)
        {
            if (chats == null)
            {
                callback(new TLVector<TLChatBase>());
                return;
            }

            var timer = Stopwatch.StartNew();

            var result = new TLVector<TLChatBase>();
            if (_database == null) Init();

            SyncChatsInternal(chats, result);

            _database.Commit();

            TLUtils.WritePerformance("SyncChats time: " + timer.Elapsed);
            callback(result);
        }

        private void SyncChatsInternal(TLVector<TLChatBase> chats, TLVector<TLChatBase> result, IList<ExceptionInfo> exceptions = null)
        {
            foreach (var chat in chats)
            {
                try
                {
                    TLChatBase cachedChat = null;
                    if (ChatsContext != null)
                    {
                        cachedChat = ChatsContext[chat.Index];
                    }

                    if (cachedChat != null)
                    {
                        var channel49 = chat as TLChannel49;
                        var isMinChannel = channel49 != null && channel49.Min;

                        // update fields
                        if (chat.GetType() == cachedChat.GetType())
                        {
                            cachedChat.Update(chat);
                        }
                        else if (isMinChannel)
                        {

                        }
                        // or replace object
                        else
                        {
                            _database.ReplaceChat(chat.Index, chat);
                        }
                        result.Add(cachedChat);
                    }
                    else
                    {
                        // add object to cache
                        result.Add(chat);
                        _database.AddChat(chat);
                    }
                }
                catch (Exception ex)
                {
                    if (exceptions != null)
                    {
                        exceptions.Add(new ExceptionInfo
                        {
                            Caption = "UpdatesService.ProcessDifference Chats",
                            Exception = ex,
                            Timestamp = DateTime.Now
                        });
                    }

                    TLUtils.WriteException("UpdatesService.ProcessDifference Chats", ex);
                }
            }
        }

        private void SyncChatInternal(TLChatBase chat, out TLChatBase result)
        {
            TLChatBase cachedChat = null;
            if (ChatsContext != null)
            {
                cachedChat = ChatsContext[chat.Index];
            }

            if (cachedChat != null)
            {
                var channel49 = chat as TLChannel49;
                var isMinChannel = channel49 != null && channel49.Min;

                // update fields
                if (chat.GetType() == cachedChat.GetType())
                {
                    cachedChat.Update(chat);
                }
                else if (isMinChannel)
                {

                }
                // or replace object
                else
                {
                    _database.ReplaceChat(chat.Index, chat);
                }
                result = cachedChat;
            }
            else
            {
                // add object to cache
                result = chat;
                _database.AddChat(chat);
            }
        }
        #endregion

        #region Broadcasts

        public void SyncBroadcast(TLBroadcastChat broadcast, Action<TLBroadcastChat> callback)
        {
            if (broadcast == null)
            {
                callback(null);
                return;
            }

            var timer = Stopwatch.StartNew();

            TLBroadcastChat result;
            if (_database == null) Init();

            SyncBroadcastInternal(broadcast, out result);

            _database.Commit();

            TLUtils.WritePerformance("SyncBroadcast time: " + timer.Elapsed);
            callback(result);
        }

        private void SyncBroadcastInternal(TLBroadcastChat chat, out TLBroadcastChat result)
        {
            TLBroadcastChat cachedBroadcast = null;
            if (BroadcastsContext != null)
            {
                cachedBroadcast = BroadcastsContext[chat.Index];
            }

            if (cachedBroadcast != null)
            {
                // update fields
                if (chat.GetType() == cachedBroadcast.GetType())
                {
                    cachedBroadcast.Update(chat);
                }
                // or replace object
                else
                {
                    _database.ReplaceBroadcast(chat.Index, chat);
                }
                result = cachedBroadcast;
            }
            else
            {
                // add object to cache
                result = chat;
                _database.AddBroadcast(chat);
            }
        }
        #endregion

        #region Contacts

        public void AddUsers(TLVector<TLUserBase> users, Action<TLVector<TLUserBase>> callback)
        {
            if (users == null)
            {
                callback(null);
                return;
            }

            if (_database == null) Init();

            foreach (var user in users)
            {
                TLUserBase cachedUser = null;
                if (UsersContext != null)
                {
                    cachedUser = UsersContext[user.Index];
                }

                if (cachedUser == null)
                {
                    _database.AddUser(user);
                }
            }

            _database.Commit();

            callback.SafeInvoke(users);
        }

        public void SyncContacts(TLImportedContacts contacts, Action<TLImportedContacts> callback)
        {
            if (contacts == null)
            {
                callback(new TLImportedContacts69());
                return;
            }

            var timer = Stopwatch.StartNew();

            var result = contacts.GetEmptyObject();
            if (_database == null) Init();

            SyncContactsInternal(contacts, result);

            _database.Commit();

            TLUtils.WritePerformance("SyncImportedContacts time: " + timer.Elapsed);
            callback(result);
        }

        public void SyncStatedMessage(TLStatedMessageBase statedMessage, Action<TLStatedMessageBase> callback)
        {
            if (statedMessage == null)
            {
                callback(null);
                return;
            }

            var timer = Stopwatch.StartNew();

            var result = statedMessage.GetEmptyObject();
            if (_database == null) Init();

            SyncChatsInternal(statedMessage.Chats, result.Chats);
            SyncUsersInternal(statedMessage.Users, result.Users);
            TLMessageBase message;
            SyncMessageInternal(TLUtils.GetPeerFromMessage(statedMessage.Message), statedMessage.Message, out message);
            result.Message = message;

            var messageCommon = message as TLMessageCommon;
            if (messageCommon != null)
            {
                var dialog = GetDialog(messageCommon);
                if (dialog != null)
                {
                    var oldMessage = dialog.Messages.FirstOrDefault(x => x.Index == message.Index);
                    if (oldMessage != null)
                    {
                        dialog.Messages.Remove(oldMessage);
                        dialog.Messages.Insert(0, message);
                        dialog._topMessage = message;
                        dialog.TopMessageId = message.Id;
                        dialog.TopMessageRandomId = message.RandomId;
                        _eventAggregator.Publish(new TopMessageUpdatedEventArgs(dialog, message));
                    }
                }
            }

            _database.Commit();


            TLUtils.WritePerformance("SyncStatedMessage time: " + timer.Elapsed);
            callback(result);
        }

        public void SyncStatedMessages(TLStatedMessagesBase statedMessages, Action<TLStatedMessagesBase> callback)
        {
            if (statedMessages == null)
            {
                callback(null);
                return;
            }

            var timer = Stopwatch.StartNew();

            var result = statedMessages.GetEmptyObject();
            if (_database == null) Init();

            SyncChatsInternal(statedMessages.Chats, result.Chats);
            SyncUsersInternal(statedMessages.Users, result.Users);

            foreach (var m in statedMessages.Messages)
            {
                TLMessageBase message;
                SyncMessageInternal(TLUtils.GetPeerFromMessage(m), m, out message);
                result.Messages.Add(message);
            }


            _database.Commit();


            TLUtils.WritePerformance("SyncStatedMessages time: " + timer.Elapsed);
            callback(result);
        }

        public void UpdateDialogPinned(TLPeerBase peer, bool pinned)
        {
            if (_database == null) Init();

            _database.UpdateDialogPinned(peer, pinned);
            _database.Commit();
        }

        public void UpdatePinnedDialogs(TLVector<TLPeerBase> order)
        {
            if (_database == null) Init();


            _database.Commit();
        }

        public TLProxyDataBase GetProxyData()
        {
            if (_database == null) Init();

            return _database.ProxyData;
        }

        public void UpdateProxyData(TLProxyDataBase proxyData)
        {
            if (_database == null) Init();

            _database.UpdateProxyData(proxyData);
            _database.Commit();
        }

        public void UpdateChannelAvailableMessages(TLInt channelId, TLInt availableMinId)
        {
            if (_database == null) Init();

            _database.UpdateChannelAvailableMessages(new TLPeerChannel { Id = channelId }, availableMinId);

            _database.Commit();
        }

        public void UpdateDialogPromo(TLDialogBase dialogBase, bool promo)
        {
            if (_database == null) Init();

            _database.UpdateDialogPromo(dialogBase, promo);
            _database.Commit();
        }

        public void DeleteDialog(TLDialogBase dialog)
        {
            if (dialog != null)
            {
                _database.DeleteDialog(dialog);

                _database.Commit();
            }
        }

        public void ClearDialog(TLPeerBase peer)
        {
            if (peer != null)
            {
                _database.ClearDialog(peer);

                _database.Commit();
            }
        }

        public void DeleteUser(TLInt id)
        {
            _database.DeleteUser(id);
            _database.Commit();
        }

        public void DeleteChat(TLInt id)
        {
            _database.DeleteChat(id);
            _database.Commit();
        }

        public void DeleteMessages(TLVector<TLLong> randomIds)
        {
            if (randomIds == null || randomIds.Count == 0) return;

            foreach (var id in randomIds)
            {
                var message = _database.RandomMessagesContext[id.Value];
                if (message != null)
                {
                    var peer = TLUtils.GetPeerFromMessage(message);

                    if (peer != null)
                    {
                        _database.DeleteMessage(message);
                    }
                }
            }

            _database.Commit();
        }

        public void DeleteDecryptedMessages(TLVector<TLLong> randomIds)
        {
            foreach (var id in randomIds)
            {
                var message = _database.DecryptedMessagesContext[id.Value];
                if (message != null)
                {
                    var peer = TLUtils.GetPeerFromMessage(message);

                    if (peer != null)
                    {
                        _database.DeleteDecryptedMessage(message, peer);
                    }
                }
            }

            _database.Commit();
        }

        public void ClearDecryptedHistoryAsync(TLInt chatId)
        {
            _database.ClearDecryptedHistory(chatId);

            _database.Commit();
        }

        public void ClearBroadcastHistoryAsync(TLInt chatId)
        {
            _database.ClearBroadcastHistory(chatId);

            _database.Commit();
        }

        public void DeleteMessages(TLPeerBase peer, TLMessageBase lastItem, TLVector<TLInt> messages)
        {
            if (messages == null || messages.Count == 0) return;

            _database.DeleteMessages(peer, lastItem, messages);

            _database.Commit();
        }

        public void DeleteMessages(TLVector<TLInt> ids)
        {
            if (ids == null || ids.Count == 0) return;

            foreach (var id in ids)
            {
                var message = _database.MessagesContext[id.Value];
                if (message != null)
                {
                    _database.DeleteMessage(message);
                }
            }

            _database.Commit();
        }

        public void DeleteUserHistory(TLPeerChannel channel, TLPeerUser user)
        {
            if (channel == null || user == null) return;

            _database.DeleteUserHistory(channel, user);

            _database.Commit();
        }

        public void DeleteChannelMessages(TLInt channelId, TLVector<TLInt> ids)
        {
            if (ids == null || ids.Count == 0) return;

            var channelContext = _database.ChannelsContext[channelId.Value];
            if (channelContext != null)
            {
                var peer = new TLPeerChannel { Id = channelId };

                var messages = new List<TLMessageBase>();
                foreach (var id in ids)
                {
                    var message = channelContext[id.Value];
                    if (message != null)
                    {
                        messages.Add(message);
                    }
                }

                _database.DeleteMessages(messages, peer);
            }

            _database.Commit();
        }

        private void SyncContactsInternal(TLImportedContacts contacts, TLImportedContacts result)
        {
            var cache = contacts.Users.ToDictionary(x => x.Index);
            foreach (var importedContact in contacts.Imported)
            {
                if (cache.ContainsKey(importedContact.UserId.Value))
                {
                    cache[importedContact.UserId.Value].ClientId = importedContact.ClientId;
                }
            }


            foreach (var user in contacts.Users)
            {
                TLUserBase cachedUser = null;
                if (UsersContext != null)
                {
                    cachedUser = UsersContext[user.Index];
                }

                if (cachedUser != null)
                {
                    var user45 = user as TLUser45;
                    var isMinUser = user45 != null && user45.Min;

                    // update fields
                    if (user.GetType() == cachedUser.GetType())
                    {
                        cachedUser.Update(user);
                        result.Users.Add(cachedUser);
                    }
                    else if (isMinUser)
                    {
                        result.Users.Add(cachedUser);
                    }
                    // or replace object
                    else
                    {
                        _database.ReplaceUser(user.Index, user);
                        result.Users.Add(user);
                    }
                }
                else
                {
                    // add object to cache
                    result.Users.Add(user);
                    _database.AddUser(user);
                }

            }

            result.Imported = contacts.Imported;
        }

        public void SyncContacts(TLContactsBase contacts, Action<TLContactsBase> callback)
        {
            if (contacts == null)
            {
                callback(new TLContacts());
                return;
            }

            if (contacts is TLContactsNotModified)
            {
                callback(contacts);
                return;
            }

            var timer = Stopwatch.StartNew();

            var result = contacts.GetEmptyObject();
            if (_database == null) Init();

            SyncContactsInternal((TLContacts)contacts, (TLContacts)result);

            _database.Commit();

            TLUtils.WritePerformance("SyncContacts time: " + timer.Elapsed);
            callback(result);
        }

        private void SyncContactsInternal(TLContacts contacts, TLContacts result)
        {
            var contactsCache = new Dictionary<int, TLContact>();
            foreach (var contact in contacts.Contacts)
            {
                contactsCache[contact.UserId.Value] = contact;
            }

            foreach (var user in contacts.Users)
            {
                user.Contact = contactsCache[user.Index];

                TLUserBase cachedUser = null;
                if (UsersContext != null)
                {
                    cachedUser = UsersContext[user.Index];
                }

                if (cachedUser != null)
                {
                    var user45 = user as TLUser45;
                    var isMinUser = user45 != null && user45.Min;

                    // update fields
                    if (user.GetType() == cachedUser.GetType())
                    {
                        cachedUser.Update(user);
                        result.Users.Add(cachedUser);
                    }
                    else if (isMinUser)
                    {
                        result.Users.Add(cachedUser);
                    }
                    // or replace object
                    else
                    {
                        _database.ReplaceUser(user.Index, user);
                        result.Users.Add(user);
                    }
                }
                else
                {
                    // add object to cache
                    result.Users.Add(user);
                    _database.AddUser(user);
                }

            }

            result.Contacts = contacts.Contacts;
        }

        #endregion

        #region Config

        private TLCdnConfig _cdnConfig;

        private readonly object _cdnConfigSyncRoot = new object();

        public void GetCdnConfigAsync(Action<TLCdnConfig> callback)
        {
            if (_cdnConfig == null)
            {
                _cdnConfig = TLUtils.OpenObjectFromMTProtoFile<TLCdnConfig>(_cdnConfigSyncRoot, Constants.CdnConfigFileName);
            }

            callback.SafeInvoke(_cdnConfig);
        }

        public TLCdnConfig GetCdnConfig()
        {
            if (_cdnConfig == null)
            {
                _cdnConfig = TLUtils.OpenObjectFromMTProtoFile<TLCdnConfig>(_cdnConfigSyncRoot, Constants.CdnConfigFileName);
            }

            return _cdnConfig;
        }

        public void SetCdnCofig(TLCdnConfig cdnConfig)
        {
            _cdnConfig = cdnConfig;

            TLUtils.SaveObjectToMTProtoFile(_cdnConfigSyncRoot, Constants.CdnConfigFileName, _cdnConfig);
        }

        private TLConfig _config;

        public TLConfig GetConfig()
        {
#if SILVERLIGHT || WIN_RT
            if (_config == null)
            {
                _config = SettingsHelper.GetValue(Constants.ConfigKey) as TLConfig;
            }
#endif
            return _config;
        }


        public void GetConfigAsync(Action<TLConfig> callback)
        {
            GetConfig();

            callback.SafeInvoke(_config);
        }

        public void SetConfig(TLConfig config)
        {
            _config = config;
#if SILVERLIGHT || WIN_RT
            SettingsHelper.SetValue(Constants.ConfigKey, config);
#endif
        }

        public void ClearConfigImportAsync()
        {
            GetConfigAsync(config =>
            {
                foreach (var option in config.DCOptions)
                {
                    option.IsAuthorized = false;
                    //if (config.ThisDC.Value != option.Id.Value)
                    //{
                    //    option.IsAuthorized = false;
                    //}
                    //else
                    //{
                    //    option.IsAuthorized = true;
                    //}
                }

                SetConfig(config);
            });
        }

        #endregion

        public IList<TLMessageBase> GetSendingMessages()
        {
            return RandomMessagesContext.Values.ToList();
        }

        public IList<TLMessageBase> GetResendingMessages()
        {
            return _database.ResendingMessages;
        }

        public IList<TLMessageBase> GetMessages()
        {
            var result = new List<TLMessageBase>();
            foreach (var d in _database.Dialogs)
            {
                var dialog = d as TLDialog;
                if (dialog != null)
                {
                    foreach (var message in dialog.Messages)
                    {
                        result.Add(message);
                    }
                }
                else
                {

                    //var encryptedDialog = d as TLEncryptedDialog;
                    //if (encryptedDialog != null)
                    //{
                    //    foreach (var message in encryptedDialog.Messages)
                    //    {
                    //        result.Add(message);
                    //    }
                    //}
                }
            }

            return result;
        }

        public void Commit()
        {
            if (_database != null)
            {
                _database.Commit();
            }
        }

        public void CompressAsync(Action callback)
        {
            if (_database != null)
            {
                Execute.BeginOnThreadPool(() =>
                {
                    _database.Compress();

                    callback.SafeInvoke();
                });
            }
        }

        public void ClearLocalFileNames()
        {
            if (_database != null)
            {
                _database.ClearLocalFileNames();
            }
        }

        public bool TryCommit()
        {
            if (_database != null && _database.HasChanges)
            {
                _database.CommitInternal();
                //Helpers.Execute.ShowDebugMessage("TryCommit result=true");
                return true;
            }

            return false;
        }

        public void SaveSnapshot(string toDirectoryName)
        {
            if (_database != null)
            {
                _database.SaveSnapshot(toDirectoryName);
            }
        }

        public void SaveTempSnapshot(string toDirectoryName)
        {
            if (_database != null)
            {
                _database.SaveTempSnapshot(toDirectoryName);
            }
        }

        public void LoadSnapshot(string fromDirectoryName)
        {
            if (_database != null)
            {
                _database.LoadSnapshot(fromDirectoryName);
                _database.Open();
            }
        }
    }
}
