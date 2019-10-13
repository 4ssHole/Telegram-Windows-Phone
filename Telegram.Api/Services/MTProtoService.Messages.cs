﻿// 
// This is the source code of Telegram for Windows Phone v. 3.x.x.
// It is licensed under GNU GPL v. 2 or later.
// You should have received a copy of the license in this archive (see LICENSE).
// 
// Copyright Evgeny Nadymov, 2013-present.
// 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Telegram.Api.Extensions;
using Telegram.Api.Helpers;
using Telegram.Api.Services.Cache;
using Telegram.Api.TL;
using Telegram.Api.TL.Functions.Help;
using Telegram.Api.TL.Functions.Messages;

namespace Telegram.Api.Services
{
    public partial class MTProtoService
    {
        public void ToggleTopPeersAsync(TLBool enabled, Action<TLBool> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLToggleTopPeers{ Enabled = enabled };

            const string caption = "messages.toggleTopPeers";
            SendInformativeMessage<TLBool>(caption, obj,
                callback.SafeInvoke,
                faultCallback);
        }

        public void ClearAllDraftsAsync(Action<TLBool> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLClearAllDrafts();

            const string caption = "messages.clearAllDrafts";
            SendInformativeMessage<TLBool>(caption, obj,
                callback.SafeInvoke,
                faultCallback);
        }

        public void MarkDialogUnreadAsync(bool unread, TLInputDialogPeer peer, Action<TLBool> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLMarkDialogUnread{ Flags = new TLInt(0), Unread = unread, Peer = peer };

            const string caption = "messages.markDialogUnread";
            SendInformativeMessage<TLBool>(caption, obj,
                callback.SafeInvoke,
                faultCallback);
        }

        public void GetDialogUnreadMarksAsync(Action<TLVector<TLDialogPeerBase>> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetDialogUnreadMarks();

            const string caption = "messages.getDialogUnreadMarks";
            SendInformativeMessage<TLVector<TLDialogPeerBase>>(caption, obj,
                callback.SafeInvoke,
                faultCallback);
        }

        public void GetProxyDataAsync(Action<TLProxyDataBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetProxyData();

            const string caption = "messages.getProxyData";
            SendInformativeMessage<TLProxyDataBase>(caption, obj,
                result =>
                {
                    _cacheService.SyncProxyData(result, callback.SafeInvoke);
                },
                faultCallback);
        }

        public void SearchStickerSetsAsync(bool full, bool excludeFeatured, TLString q, TLInt hash, Action<TLFoundStickerSetsBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLSearchStickerSets { Flags = new TLInt(0), ExcludeFeatured = excludeFeatured, Q = q, Hash = hash };

            var results = new List<TLMessagesStickerSet>();
            var resultsSyncRoot = new object();

            const string caption = "messages.searchStickerSets";
            SendInformativeMessage<TLFoundStickerSetsBase>(caption, obj,
                result =>
                {
                    var foundStickerSets = result as TLFoundStickerSets;
                    if (foundStickerSets != null && full)
                    {
                        GetStickerSetsAsync(foundStickerSets, r => callback(r as TLFoundStickerSetsBase),
                            stickerSetResult =>
                            {
                                var messagesStickerSet = stickerSetResult as TLMessagesStickerSet;
                                if (messagesStickerSet != null)
                                {
                                    bool processStickerSets;
                                    lock (resultsSyncRoot)
                                    {
                                        results.Add(messagesStickerSet);
                                        processStickerSets = results.Count == foundStickerSets.Sets.Count;
                                    }

                                    if (processStickerSets)
                                    {
                                        ProcessStickerSets(foundStickerSets, results);
                                        foundStickerSets.MessagesStickerSets = new TLVector<TLMessagesStickerSet>(results);
                                        callback.SafeInvoke(foundStickerSets);
                                    }
                                }
                            },
                            faultCallback);
                    }
                    else
                    {
                        callback.SafeInvoke(result);
                    }
                },
                faultCallback.SafeInvoke);
        }

        public void ReportAsync(TLInputPeerBase peer, TLVector<TLInt> id, TLInputReportReasonBase reason, Action<TLBool> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLReport { Peer = peer, Id = id, Reason = reason };

            const string caption = "messages.report";
            SendInformativeMessage(caption, obj, callback, faultCallback);
        }

        public void GetStickersAsync(TLString emoticon, TLInt hash, Action<TLStickersBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetStickers { Emoticon = emoticon, Hash = hash };

            const string caption = "messages.getStickers";
            SendInformativeMessage(caption, obj, callback, faultCallback);
        }

        public void GetRecentLocationsAsync(TLInputPeerBase peer, TLInt limit, TLInt hash, Action<TLMessagesBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetRecentLocations { Peer = peer, Limit = limit, Hash = hash };

            const string caption = "messages.getRecentLocations";
            SendInformativeMessage(caption, obj, callback, faultCallback);
        }

        public void GetFavedStickersAsync(TLInt hash, Action<TLFavedStickersBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetFavedStickers { Hash = hash };

            const string caption = "messages.getFavedStickers";
            SendInformativeMessage(caption, obj, callback, faultCallback);
        }

        public void FaveStickerAsync(TLInputDocumentBase id, TLBool unfave, Action<TLBool> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLFaveSticker { Id = id, Unfave = unfave };

            const string caption = "messages.faveSticker";
            SendInformativeMessage(caption, obj, callback, faultCallback);
        }

        public void GetUnreadMentionsAsync(TLInputPeerBase peer, TLInt offsetId, TLInt addOffset, TLInt limit, TLInt maxId, TLInt minId, Action<TLMessagesBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetUnreadMentions { Peer = peer, OffsetId = offsetId, AddOffset = addOffset, Limit = limit, MinId = minId, MaxId = maxId };

            SendInformativeMessage<TLMessagesBase>("messages.getUnreadMentions", obj, callback.SafeInvoke, faultCallback);
        }

        public void ToggleDialogPinAsync(bool pinned, TLPeerBase peer, Action<TLBool> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLToggleDialogPin { Flags = new TLInt(0), Peer = PeerToInputPeer(peer), Pinned = pinned };

            const string caption = "messages.toggleDialogPin";
            SendInformativeMessage<TLBool>(caption, obj,
                result =>
                {
                    _cacheService.UpdateDialogPinned(peer, pinned);
                    callback.SafeInvoke(result);
                },
                faultCallback);
        }

        public void ReorderPinnedDialogsAsync(bool force, TLVector<TLInputDialogPeerBase> order, Action<TLBool> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLReorderPinnedDialogs { Flags = new TLInt(0), Order = order };
            if (force)
            {
                obj.SetForce();
            }

            const string caption = "messages.reorderPinnedDialogs";
            SendInformativeMessage<TLBool>(caption, obj, callback.SafeInvoke, faultCallback);
        }

        public void GetWebPageAsync(TLString url, TLInt hash, Action<TLWebPageBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetWebPage { Url = url, Hash = hash };

            const string caption = "messages.getWebPage";
            SendInformativeMessage<TLWebPageBase>(caption, obj, callback.SafeInvoke, faultCallback);
        }

        public void GetCommonChatsAsync(TLInputUserBase user, TLInt maxId, TLInt limit, Action<TLChatsBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetCommonChats { User = user, MaxId = maxId, Limit = limit };

            const string caption = "messages.getCommonChats";
            SendInformativeMessage<TLChatsBase>(caption, obj,
                result =>
                {
                    var chats = result as TLChats24;
                    if (chats != null)
                    {
                        _cacheService.SyncUsersAndChats(new TLVector<TLUserBase>(), chats.Chats, tuple => callback.SafeInvoke(result));
                    }
                },
                faultCallback);
        }

        private readonly Dictionary<string, TLArchivedStickers> _cachedArchivedStickers = new Dictionary<string, TLArchivedStickers>();

        public void GetAttachedStickersAsync(bool full, TLInputStickeredMediaBase media, Action<TLArchivedStickers> callback, Action<TLRPCError> faultCallback = null)
        {
            TLArchivedStickers cachedArchivedStickers;
            var key = string.Format("{0} {1}", media, full);
            if (_cachedArchivedStickers.TryGetValue(key, out cachedArchivedStickers))
            {
                callback.SafeInvoke(cachedArchivedStickers);
                return;
            }


            var obj = new TLGetAttachedStickers { Media = media };

            const string caption = "messages.getAttachedStickers";
            //SendInformativeMessage(caption, obj, callback, faultCallback);
            var results = new List<TLMessagesStickerSet>();
            var resultsSyncRoot = new object();
            SendInformativeMessage<TLVector<TLStickerSetCoveredBase>>(caption, obj,
                result =>
                {
                    var archivedStickers = new TLArchivedStickers();
                    archivedStickers.Count = new TLInt(result.Count);
                    archivedStickers.SetsCovered = result;

                    archivedStickers.Packs = new TLVector<TLStickerPack>();
                    archivedStickers.Documents = new TLVector<TLDocumentBase>();
                    archivedStickers.MessagesStickerSets = new TLVector<TLMessagesStickerSet>();

                    if (full)
                    {
                        GetStickerSetsAsync(archivedStickers, r => callback(r as TLArchivedStickers),
                            stickerSetResult =>
                            {
                                var messagesStickerSet = stickerSetResult as TLMessagesStickerSet;
                                if (messagesStickerSet != null)
                                {
                                    bool processStickerSets;
                                    lock (resultsSyncRoot)
                                    {
                                        results.Add(messagesStickerSet);
                                        processStickerSets = results.Count == archivedStickers.Sets.Count;
                                    }

                                    if (processStickerSets)
                                    {
                                        ProcessStickerSets(archivedStickers, results);
                                        archivedStickers.MessagesStickerSets = new TLVector<TLMessagesStickerSet>(results);
                                        _cachedArchivedStickers[key] = archivedStickers;
                                        callback.SafeInvoke(archivedStickers);
                                    }
                                }
                            },
                            faultCallback);
                    }
                    else
                    {
                        _cachedArchivedStickers[key] = archivedStickers;
                        callback.SafeInvoke(archivedStickers);
                    }
                });
        }

        public void GetRecentStickersAsync(bool attached, TLInt hash, Action<TLRecentStickersBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetRecentStickers { Flags = new TLInt(0), Hash = hash };
            if (attached)
            {
                obj.SetAttached();
            }

            const string caption = "messages.getRecentStickers";
            SendInformativeMessage(caption, obj, callback, faultCallback);
        }

        public void ClearRecentStickersAsync(bool attached, Action<TLBool> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLClearRecentStickers { Flags = new TLInt(0) };
            if (attached)
            {
                obj.SetAttached();
            }

            const string caption = "messages.clearRecentStickers";
            SendInformativeMessage(caption, obj, callback, faultCallback);
        }

        public void GetUnusedStickersAsync(TLInt limit, Action<TLVector<TLStickerSetCoveredBase>> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetUnusedStickers { Limit = limit };

            const string caption = "messages.getUnusedStickers";
            SendInformativeMessage(caption, obj, callback, faultCallback);
        }

        public void ReadFeaturedStickersAsync(TLVector<TLLong> id, Action<TLBool> callback, Action<TLRPCError> faultCallback = null)
        {
#if DEBUG
            callback.SafeInvoke(TLBool.True);
            return;
#endif

            var obj = new TLReadFeaturedStickers { Id = id };

            const string caption = "messages.readFeaturedStickers";
            SendInformativeMessage<TLBool>(caption, obj, callback.SafeInvoke, faultCallback.SafeInvoke);
        }

        public void GetAllDraftsAsync(Action<TLUpdatesBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetAllDrafts();

            const string caption = "messages.getAllDrafts";
            SendInformativeMessage<TLUpdatesBase>(caption, obj,
                result =>
                {
                    var multiPts = result as IMultiPts;
                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption);
                    }
                    else
                    {
                        _updatesService.ProcessUpdates(result, true);
                    }

                    callback.SafeInvoke(result);
                },
                faultCallback.SafeInvoke);
        }

        public void SaveDraftAsync(TLInputPeerBase peer, TLDraftMessageBase draft, Action<TLBool> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = draft.ToSaveDraftObject(peer);

            const string caption = "messages.saveDraft";
            SendInformativeMessage<TLBool>(caption, obj,
                result =>
                {
                    callback.SafeInvoke(result);
                },
                faultCallback.SafeInvoke);
        }

        public void GetInlineBotResultsAsync(TLInputUserBase bot, TLInputPeerBase peer, TLInputGeoPointBase geoPoint, TLString query, TLString offset, Action<TLBotResults> callback, Action<TLRPCError> faultCallback = null)
        {
            var key = string.Format("{0}_{1}_{2}_{3}_{4}", bot, peer, geoPoint, query, offset);

            TLBotResults botResults;
            if (TryGetCachedValue(_cache, key, out botResults, IsCacheTimeValid))
            {
                callback.SafeInvoke(botResults);
                return;
            }

            var obj = new TLGetInlineBotResults { Flags = new TLInt(0), Bot = bot, Peer = peer, GeoPoint = geoPoint, Query = query, Offset = offset };

            const string caption = "messages.getInlineBotResults";
            SendInformativeMessage<TLBotResults>(caption, obj,
                result =>
                {
                    SetCachedValue(ref _cache, key, result as ICachedObject, IsCacheTimeValid);
                    callback.SafeInvoke(result);
                },
                faultCallback);
        }

        public void SetInlineBotResultsAsync(TLBool gallery, TLBool pr, TLLong queryId, TLVector<TLInputBotInlineResult> results, TLInt cacheTime, TLString nextOffset, TLInlineBotSwitchPM switchPM, Action<TLBool> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLSetInlineBotResults { Flags = new TLInt(0), Gallery = gallery, Private = pr, QueryId = queryId, Results = results, CacheTime = cacheTime, NextOffset = nextOffset, SwitchPM = switchPM };

            const string caption = "messages.setInlineBotResults";
            SendInformativeMessage(caption, obj, callback, faultCallback);
        }

        public void SendInlineBotResultAsync(TLMessage45 message, Action<TLMessageCommon> callback, Action fastCallback, Action<TLRPCError> faultCallback = null)
        {
            var inputPeer = PeerToInputPeer(message.ToId);
            var obj = new TLSendInlineBotResult { Flags = new TLInt(0), Peer = inputPeer, ReplyToMsgId = message.ReplyToMsgId, RandomId = message.RandomId, QueryId = message.InlineBotResultQueryId, Id = message.InlineBotResultId };

            if (message.IsChannelMessage)
            {
                obj.SetChannelMessage();
            }

            var message48 = message as TLMessage48;
            if (message48 != null && message48.Silent)
            {
                obj.SetSilent();
            }

            const string caption = "messages.sendInlineBotResult";
            SendInlineBotResultAsyncInternal(obj,
                result =>
                {
                    var multiPts = result as IMultiPts;
                    var shortSentMessage = result as TLUpdatesShortSentMessage;
                    if (shortSentMessage != null)
                    {
                        message.Flags = shortSentMessage.Flags;
                        if (shortSentMessage.HasMedia)
                        {
                            message._media = shortSentMessage.Media;
                        }
                        if (shortSentMessage.HasEntities)
                        {
                            message.Entities = shortSentMessage.Entities;
                        }

                        Execute.BeginOnUIThread(() =>
                        {
                            message.Status = GetMessageStatus(_cacheService, message.ToId);
                            message.Date = shortSentMessage.Date;
                            if (shortSentMessage.Media is TLMessageMediaWebPage)
                            {
                                message.NotifyOfPropertyChange(() => message.Media);
                            }

#if DEBUG
                            message.Id = shortSentMessage.Id;
                            message.NotifyOfPropertyChange(() => message.Id);
                            message.NotifyOfPropertyChange(() => message.Date);
#endif
                        });

                        _updatesService.SetState(multiPts, caption);

                        message.Id = shortSentMessage.Id;
                        _cacheService.SyncSendingMessage(message, null, callback);
                        return;
                    }

                    var updates = result as TLUpdates;
                    if (updates != null)
                    {
                        foreach (var update in updates.Updates)
                        {
                            var updateNewMessage = update as TLUpdateNewMessage24;
                            if (updateNewMessage != null)
                            {
                                Execute.BeginOnUIThread(() =>
                                {
                                    // faster update web page with inline bots @imdb, @vid, @wiki
                                    var newMessage = updateNewMessage.Message as TLMessage45;
                                    if (newMessage != null)
                                    {
                                        var mediaWebPage = newMessage.Media as TLMessageMediaWebPage;
                                        if (mediaWebPage != null)
                                        {
                                            message.Media = newMessage.Media;
                                        }

                                        if (mediaWebPage == null)
                                        {
                                            //Execute.ShowDebugMessage(newMessage.Media.GetType().ToString());
                                        }
                                    }
                                });

                                var messageCommon = updateNewMessage.Message as TLMessageCommon;
                                if (messageCommon != null)
                                {
                                    messageCommon.RandomId = message.RandomId;
                                    message.Id = messageCommon.Id;
                                    message.Date = messageCommon.Date;
                                }
                            }
                        }

                        Execute.BeginOnUIThread(() =>
                        {
                            message.Status = GetMessageStatus(_cacheService, message.ToId);
                        });

                        if (multiPts != null)
                        {
                            _updatesService.SetState(multiPts, caption);
                        }
                        else
                        {
                            _updatesService.ProcessUpdates(updates);
                        }

                        callback.SafeInvoke(message);
                    }
                },
                fastCallback,
                faultCallback.SafeInvoke);
        }

        public void GetDocumentByHashAsync(TLString sha256, TLInt size, TLString mimeType, Action<TLDocumentBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetDocumentByHash { Sha256 = sha256, Size = size, MimeType = mimeType };

            const string caption = "messages.getDocumentByHash";
            SendInformativeMessage(caption, obj, callback, faultCallback);
        }

        public void SearchGifsAsync(TLString q, TLInt offset, Action<TLFoundGifs> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLSearchGifs { Q = q, Offset = offset };

            const string caption = "messages.searchGifs";
            SendInformativeMessage(caption, obj, callback, faultCallback);
        }

        public void GetSavedGifsAsync(TLInt hash, Action<TLSavedGifsBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetSavedGifs { Hash = hash };

            const string caption = "messages.getSavedGifs";
            SendInformativeMessage(caption, obj, callback, faultCallback);
        }

        public void SaveGifAsync(TLInputDocumentBase id, TLBool unsave, Action<TLBool> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLSaveGif { Id = id, Unsave = unsave };

            const string caption = "messages.saveGif";
            SendInformativeMessage(caption, obj, callback, faultCallback);
        }

        public void ReorderStickerSetsAsync(bool masks, TLVector<TLLong> order, Action<TLBool> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLReorderStickerSets { Flags = new TLInt(0), Order = order };
            if (masks)
            {
                obj.SetMasks();
            }

            const string caption = "messages.reorderStickerSets";
            SendInformativeMessage(caption, obj, callback, faultCallback);
        }

        public void ReportSpamAsync(TLInputPeerBase peer, Action<TLBool> callback, Action<TLRPCError> faultCallback = null)
        {
#if DEBUG
            Execute.BeginOnThreadPool(() => callback.SafeInvoke(TLBool.True));
            return;
#endif

            var obj = new TLReportSpam { Peer = peer };

            const string caption = "messages.reportSpam";
            SendInformativeMessage(caption, obj, callback, faultCallback);
        }

        public void GetWebPagePreviewAsync(TLString message, Action<TLMessageMediaBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetWebPagePreview { Flags = new TLInt(0), Message = message, Entities = null };

            const string caption = "messages.getWebPagePreview";
            SendInformativeMessage(caption, obj, callback, faultCallback);
        }

        public void GetFeaturedStickersAsync(bool full, TLInt hash, Action<TLFeaturedStickersBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetFeaturedStickers { Hash = hash };

            const string caption = "messages.getFeaturedStickers";

            var results = new List<TLMessagesStickerSet>();
            var resultsSyncRoot = new object();
            SendInformativeMessage<TLFeaturedStickersBase>(caption, obj,
                result =>
                {
                    var featuredStickers = result as TLFeaturedStickers;
                    if (featuredStickers != null && full)
                    {
                        GetStickerSetsAsync(featuredStickers, r => callback(r as TLFeaturedStickersBase),
                            stickerSetResult =>
                            {
                                var messagesStickerSet = stickerSetResult as TLMessagesStickerSet;
                                if (messagesStickerSet != null)
                                {
                                    bool processStickerSets;
                                    lock (resultsSyncRoot)
                                    {
                                        results.Add(messagesStickerSet);
                                        processStickerSets = results.Count == featuredStickers.Sets.Count;
                                    }

                                    if (processStickerSets)
                                    {
                                        ProcessStickerSets(featuredStickers, results);
                                        featuredStickers.MessagesStickerSets = new TLVector<TLMessagesStickerSet>(results);
                                        //Execute.ShowDebugMessage(caption + " elapsed=" + stopwatch.Elapsed);
                                        callback.SafeInvoke(featuredStickers);
                                    }
                                }
                            },
                            faultCallback);
                    }
                    else
                    {
                        callback.SafeInvoke(result);
                    }
                },
                faultCallback.SafeInvoke);
        }

        public void GetArchivedStickersAsync(bool full, TLLong offsetId, TLInt limit, Action<TLArchivedStickers> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetArchivedStickers { Flags = new TLInt(0), OffsetId = offsetId, Limit = limit };
            //obj.SetMasks();

            const string caption = "messages.getArchivedStickers";

            var results = new List<TLMessagesStickerSet>();
            var resultsSyncRoot = new object();
            SendInformativeMessage<TLArchivedStickers>(caption, obj,
                result =>
                {
                    if (full)
                    {
                        GetStickerSetsAsync(result, r => callback(r as TLArchivedStickers),
                            stickerSetResult =>
                            {
                                var messagesStickerSet = stickerSetResult as TLMessagesStickerSet;
                                if (messagesStickerSet != null)
                                {
                                    bool processStickerSets;
                                    lock (resultsSyncRoot)
                                    {
                                        results.Add(messagesStickerSet);
                                        processStickerSets = results.Count == result.Sets.Count;
                                    }

                                    if (processStickerSets)
                                    {
                                        ProcessStickerSets(result, results);
                                        result.MessagesStickerSets = new TLVector<TLMessagesStickerSet>(results);
                                        callback.SafeInvoke(result);
                                    }
                                }
                            },
                            faultCallback);
                    }
                    else
                    {
                        callback.SafeInvoke(result);
                    }
                });
        }

        public void GetMaskStickersAsync(TLString hash, Action<TLAllStickersBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetMaskStickers { Hash = TLUtils.ToTLInt(hash) ?? new TLInt(0) };

            const string caption = "messages.getMaskStickers";

            var results = new List<TLMessagesStickerSet>();
            var resultsSyncRoot = new object();
            SendInformativeMessage<TLAllStickersBase>(caption, obj,
                result =>
                {
                    var allStickers32 = result as TLAllStickers43;
                    if (allStickers32 != null)
                    {
                        GetStickerSetsAsync(allStickers32, r => callback(r as TLAllStickersBase),
                            stickerSetResult =>
                            {
                                var messagesStickerSet = stickerSetResult as TLMessagesStickerSet;
                                if (messagesStickerSet != null)
                                {
                                    bool processStickerSets;
                                    lock (resultsSyncRoot)
                                    {
                                        results.Add(messagesStickerSet);
                                        processStickerSets = results.Count == allStickers32.Sets.Count;
                                    }

                                    if (processStickerSets)
                                    {
                                        ProcessStickerSets(allStickers32, results);

                                        callback.SafeInvoke(allStickers32);
                                    }
                                }
                            },
                            faultCallback);
                    }
                    else
                    {
                        callback.SafeInvoke(result);
                    }
                });
        }

        public void GetAllStickersAsync(TLString hash, Action<TLAllStickersBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetAllStickers { Hash = TLUtils.ToTLInt(hash) ?? new TLInt(0) };

            const string caption = "messages.getAllStickers";

            var results = new List<TLMessagesStickerSet>();
            var resultsSyncRoot = new object();
            SendInformativeMessage<TLAllStickersBase>(caption, obj,
                result =>
                {
                    var allStickers32 = result as TLAllStickers43;
                    if (allStickers32 != null)
                    {
                        GetStickerSetsAsync(allStickers32, r => callback(r as TLAllStickersBase),
                            stickerSetResult =>
                            {
                                var messagesStickerSet = stickerSetResult as TLMessagesStickerSet;
                                if (messagesStickerSet != null)
                                {
                                    bool processStickerSets;
                                    lock (resultsSyncRoot)
                                    {
                                        results.Add(messagesStickerSet);
                                        processStickerSets = results.Count == allStickers32.Sets.Count;
                                    }

                                    if (processStickerSets)
                                    {
                                        ProcessStickerSets(allStickers32, results);

                                        callback.SafeInvoke(allStickers32);
                                    }
                                }
                            },
                            faultCallback);
                    }
                    else
                    {
                        callback.SafeInvoke(result);
                    }
                });
        }

        private static void ProcessStickerSets(IStickers stickers, List<TLMessagesStickerSet> results)
        {
            var documentsDict = new Dictionary<long, TLDocumentBase>();
            var packsDict = new Dictionary<string, TLStickerPack>();
            foreach (var result in results)
            {
                foreach (var pack in result.Packs)
                {
                    var emoticon = pack.Emoticon.ToString();
                    TLStickerPack currentPack;
                    if (packsDict.TryGetValue(emoticon, out currentPack))
                    {
                        var docDict = new Dictionary<long, long>();
                        foreach (var document in currentPack.Documents)
                        {
                            docDict[document.Value] = document.Value;
                        }
                        foreach (var document in pack.Documents)
                        {
                            if (!docDict.ContainsKey(document.Value))
                            {
                                docDict[document.Value] = document.Value;
                                currentPack.Documents.Add(document);
                            }
                        }
                    }
                    else
                    {
                        packsDict[emoticon] = pack;
                    }
                }

                foreach (var document in result.Documents)
                {
                    documentsDict[document.Id.Value] = document;
                }
            }
            stickers.Packs = new TLVector<TLStickerPack>();
            foreach (var pack in packsDict.Values)
            {
                stickers.Packs.Add(pack);
            }
            stickers.Documents = new TLVector<TLDocumentBase>();
            foreach (var document in documentsDict.Values)
            {
                stickers.Documents.Add(document);
            }
        }

        private void GetStickerSetsAsync(IStickers stickers, Action<IStickers> callback, Action<TLObject> getStickerSetCallback, Action<TLRPCError> faultCallback)
        {
            var sets = stickers.Sets;
            if (sets.Count == 0)
            {
                callback.SafeInvoke(stickers);
                return;
            }

            var container = new TLContainer { Messages = new List<TLContainerTransportMessage>() };
            var historyItems = new List<HistoryItem>();
            for (var i = 0; i < sets.Count; i++)
            {
                var set = sets[i];
                var obj = new TLGetStickerSet { Stickerset = new TLInputStickerSetId { Id = set.Id, AccessHash = set.AccessHash } };
                int sequenceNumber;
                TLLong messageId;
                lock (_activeTransportRoot)
                {
                    sequenceNumber = _activeTransport.SequenceNumber * 2 + 1;
                    _activeTransport.SequenceNumber++;
                    messageId = _activeTransport.GenerateMessageId(true);
                }

                var data = i > 0 ? (TLObject)new TLInvokeAfterMsg { MsgId = container.Messages[i - 1].MessageId, Object = obj } : obj;

                var transportMessage = new TLContainerTransportMessage
                {
                    MessageId = messageId,
                    SeqNo = new TLInt(sequenceNumber),
                    MessageData = data
                };

                var historyItem = new HistoryItem
                {
                    SendTime = DateTime.Now,
                    Caption = "stickers.containerGetStickerSetPart" + i,
                    Object = obj,
                    Message = transportMessage,
                    Callback = getStickerSetCallback,
                    AttemptFailed = null,
                    FaultCallback = faultCallback,
                    ClientTicksDelta = ClientTicksDelta,
                    Status = RequestStatus.Sent,
                };
                historyItems.Add(historyItem);

                container.Messages.Add(transportMessage);
            }


            lock (_historyRoot)
            {
                foreach (var historyItem in historyItems)
                {
                    _history[historyItem.Hash] = historyItem;
                }
            }
#if DEBUG
            NotifyOfPropertyChange(() => History);
#endif

            SendNonInformativeMessage<TLObject>("stickers.container", container, result => callback(null), faultCallback);
        }

        private Dictionary<string, TLMessagesStickerSet> _stickerSetCache;

        public void GetStickerSetAsync(TLInputStickerSetBase stickerset, Action<TLMessagesStickerSet> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetStickerSet { Stickerset = stickerset };

            var inputStickerSetShortName = stickerset as TLInputStickerSetShortName;
            TLMessagesStickerSet cachedValue;
            if (inputStickerSetShortName != null && _stickerSetCache != null && _stickerSetCache.TryGetValue(inputStickerSetShortName.ShortName.ToString(), out cachedValue))
            {
                callback.SafeInvoke(cachedValue);
                return;
            }

            const string caption = "messages.getStickerSet";
            SendInformativeMessage<TLMessagesStickerSet>(caption, obj,
                result =>
                {
                    _stickerSetCache = _stickerSetCache ?? new Dictionary<string, TLMessagesStickerSet>();
                    _stickerSetCache[result.Set.ShortName.ToString()] = result;

                    callback.SafeInvoke(result);
                },
                faultCallback);
        }

        public void InstallStickerSetAsync(TLInputStickerSetBase stickerset, TLBool archived, Action<TLStickerSetInstallResultBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLInstallStickerSet { Stickerset = stickerset, Archived = archived };

            const string caption = "messages.installStickerSet";

            var results = new List<TLMessagesStickerSet>();
            var resultsSyncRoot = new object();
            SendInformativeMessage<TLStickerSetInstallResultBase>(caption, obj,
                result =>
                {
                    var resultArchive = result as TLStickerSetInstallResultArchive;
                    if (resultArchive != null)
                    {
                        GetStickerSetsAsync(resultArchive, r => callback(r as TLStickerSetInstallResultArchive),
                            stickerSetResult =>
                            {
                                var messagesStickerSet = stickerSetResult as TLMessagesStickerSet;
                                if (messagesStickerSet != null)
                                {
                                    var set32 = messagesStickerSet.Set as TLStickerSet32;
                                    if (set32 != null)
                                    {
                                        set32.Installed = true;
                                        set32.Archived = true;
                                    }

                                    var set76 = messagesStickerSet.Set as TLStickerSet76;
                                    if (set76 != null)
                                    {
                                        set76.InstalledDate = TLUtils.DateToUniversalTimeTLInt(ClientTicksDelta, DateTime.Now);
                                    }

                                    bool processStickerSets;
                                    lock (resultsSyncRoot)
                                    {
                                        results.Add(messagesStickerSet);
                                        processStickerSets = results.Count == resultArchive.Sets.Count;
                                    }

                                    if (processStickerSets)
                                    {
                                        ProcessStickerSets(resultArchive, results);
                                        resultArchive.MessagesStickerSets = new TLVector<TLMessagesStickerSet>(results);
                                        callback.SafeInvoke(result);
                                    }
                                }
                            },
                            faultCallback);
                    }
                    else
                    {
                        callback.SafeInvoke(result);
                    }
                },
                faultCallback);
        }

        public void UninstallStickerSetAsync(TLInputStickerSetBase stickerset, Action<TLBool> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLUninstallStickerSet { Stickerset = stickerset };

            const string caption = "messages.uninstallStickerSet";
            SendInformativeMessage(caption, obj, callback, faultCallback);
        }

        private static MessageStatus GetMessageStatus(ICacheService cacheService, TLPeerBase peer)
        {
            var status = MessageStatus.Confirmed;
            if (peer is TLPeerUser)
            {
                var user = cacheService.GetUser(peer.Id);
                if (user != null)
                {
                    var botInfo = user.BotInfo as TLBotInfo;
                    if (botInfo != null)
                    {
                        status = MessageStatus.Read;
                    }
                    else if (user.IsSelf)
                    {
                        status = MessageStatus.Read;
                    }
                }
            }

            //if (peer is TLPeerChannel)
            //{
            //    status = MessageStatus.Read;
            //}

            return status;
        }

        public TLInputPeerBase PeerToInputPeer(TLPeerBase peer)
        {
            if (peer is TLPeerUser)
            {
                var cachedUser = _cacheService.GetUser(peer.Id);
                if (cachedUser != null)
                {
                    var userForeign = cachedUser as TLUserForeign;
                    var userRequest = cachedUser as TLUserRequest;
                    var user = cachedUser as TLUser;

                    if (userForeign != null)
                    {
                        return new TLInputPeerUser { UserId = userForeign.Id, AccessHash = userForeign.AccessHash };
                    }

                    if (userRequest != null)
                    {
                        return new TLInputPeerUser { UserId = userRequest.Id, AccessHash = userRequest.AccessHash };
                    }

                    if (user != null)
                    {
                        return user.ToInputPeer();
                    }

                    return new TLInputPeerUser { UserId = peer.Id, AccessHash = new TLLong(0) };
                }

                return new TLInputPeerUser { UserId = peer.Id, AccessHash = new TLLong(0) };
            }

            if (peer is TLPeerChannel)
            {
                var channel = _cacheService.GetChat(peer.Id) as TLChannel;
                if (channel != null)
                {
                    return new TLInputPeerChannel { ChatId = peer.Id, AccessHash = channel.AccessHash };
                }
            }

            if (peer is TLPeerChat)
            {
                return new TLInputPeerChat { ChatId = peer.Id };
            }

            return new TLInputPeerBroadcast { ChatId = peer.Id };
        }

        public void SendMessageAsync(TLMessage36 message, Action<TLMessageCommon> callback, Action fastCallback, Action<TLRPCError> faultCallback = null)
        {
            var inputPeer = PeerToInputPeer(message.ToId);
            var obj = new TLSendMessage { Peer = inputPeer, ReplyToMsgId = message.ReplyToMsgId, Message = message.Message, RandomId = message.RandomId };

            if (message.Entities != null)
            {
                obj.Entities = message.Entities;
            }

            if (message.NoWebpage)
            {
                obj.NoWebpage();
            }

            if (message.IsChannelMessage)
            {
                obj.SetChannelMessage();
            }

            var message48 = message as TLMessage48;
            if (message48 != null && message48.Silent)
            {
                obj.SetSilent();
            }

            obj.ClearDraft();

            const string caption = "messages.sendMessage";
            SendMessageAsyncInternal(obj,
                result =>
                {
#if DEBUG
                    var builder = new StringBuilder();
                    builder.Append(result.GetType());
                    var updates = result as TLUpdates;
                    var updatesShort = result as TLUpdatesShort;
                    if (updates != null)
                    {
                        foreach (var update in updates.Updates)
                        {
                            builder.Append(update);
                        }
                    }
                    else if (updatesShort != null)
                    {
                        builder.Append(updatesShort.Update);
                    }

                    Logs.Log.Write(string.Format("{0} result={1}", caption, builder.ToString()));
#endif

                    var multiPts = result as IMultiPts;

                    var shortSentMessage = result as TLUpdatesShortSentMessage;
                    if (shortSentMessage != null)
                    {
                        message.Flags = shortSentMessage.Flags;
                        if (shortSentMessage.HasMedia)
                        {
                            message._media = shortSentMessage.Media;
                        }
                        if (shortSentMessage.HasEntities)
                        {
                            message.Entities = shortSentMessage.Entities;
                        }

                        Execute.BeginOnUIThread(() =>
                        {
                            message.Status = GetMessageStatus(_cacheService, message.ToId);
                            message.Date = shortSentMessage.Date;
                            if (shortSentMessage.Media is TLMessageMediaWebPage)
                            {
                                message.NotifyOfPropertyChange(() => message.Media);
                            }

#if DEBUG
                            message.Id = shortSentMessage.Id;
                            message.NotifyOfPropertyChange(() => message.Id);
                            message.NotifyOfPropertyChange(() => message.Date);
#endif
                        });

                        _updatesService.SetState(multiPts, caption);

                        message.Id = shortSentMessage.Id;
                        _cacheService.SyncSendingMessage(message, null, callback);
                        return;
                    }

                    Execute.BeginOnUIThread(() =>
                    {
                        message.Status = GetMessageStatus(_cacheService, message.ToId);
                    });

                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption);
                    }
                    else
                    {
                        ProcessUpdates(result, new List<TLMessage25> { message });
                    }

                    callback.SafeInvoke(message);
                },
                fastCallback,
                faultCallback.SafeInvoke);
        }

        private void ProcessUpdates(TLUpdatesBase updatesBase, IList<TLMessage25> messages, bool notifyNewMessage = false)
        {
            var updates = updatesBase as TLUpdates;
            if (updates != null)
            {
                var messagesRandomIndex = new Dictionary<long, TLMessage25>();
                if (messages != null)
                {
                    for (var i = 0; i < messages.Count; i++)
                    {
                        if (messages[i].RandomIndex != 0)
                        {
                            messagesRandomIndex[messages[i].RandomIndex] = messages[i];
                        }
                    }
                }

                var updateNewMessageIndex = new Dictionary<long, TLUpdateNewMessage>();
                var updateNewChannelMessageIndex = new Dictionary<long, TLUpdateNewChannelMessage>();
                var updateMessageIdList = new List<TLUpdateMessageId>();
                for (var i = 0; i < updates.Updates.Count; i++)
                {
                    var updateNewMessage = updates.Updates[i] as TLUpdateNewMessage;
                    if (updateNewMessage != null)
                    {
                        ProcessSelfMessage(updateNewMessage.Message);

                        updateNewMessageIndex[updateNewMessage.Message.Index] = updateNewMessage;
                        continue;
                    }

                    var updateNewChannelMessage = updates.Updates[i] as TLUpdateNewChannelMessage;
                    if (updateNewChannelMessage != null)
                    {
                        //ProcessSelfMessage(updateNewChannelMessage.Message); // no need to channel messages

                        updateNewChannelMessageIndex[updateNewChannelMessage.Message.Index] = updateNewChannelMessage;
                        continue;
                    }

                    var updateMessageId = updates.Updates[i] as TLUpdateMessageId;
                    if (updateMessageId != null)
                    {
                        updateMessageIdList.Add(updateMessageId);
                        continue;
                    }
                }

                foreach (var updateMessageId in updateMessageIdList)
                {
                    TLUpdateNewMessage updateNewMessage;
                    if (updateNewMessageIndex.TryGetValue(updateMessageId.Id.Value, out updateNewMessage))
                    {
                        var cachedSendingMessage = _cacheService.GetMessage(updateMessageId.RandomId);
                        if (cachedSendingMessage != null)
                        {
                            updateNewMessage.Message.RandomId = updateMessageId.RandomId;
                        }
                    }

                    TLUpdateNewChannelMessage updateNewChannelMessage;
                    if (updateNewChannelMessageIndex.TryGetValue(updateMessageId.Id.Value, out updateNewChannelMessage))
                    {
                        var cachedSendingMessage = _cacheService.GetMessage(updateMessageId.RandomId);
                        if (cachedSendingMessage != null)
                        {
                            updateNewChannelMessage.Message.RandomId = updateMessageId.RandomId;
                        }
                    }

                    TLMessage25 message;
                    if (messagesRandomIndex.TryGetValue(updateMessageId.RandomId.Value, out message))
                    {
                        message.Id = updateMessageId.Id;
                        if (updateNewMessage != null)
                        {
                            var messageCommon = updateNewMessage.Message as TLMessageCommon;
                            if (messageCommon != null)
                            {
                                message.Date = messageCommon.Date;
                            }
                        }
                        else if (updateNewChannelMessage != null)
                        {
                            var messageCommon = updateNewChannelMessage.Message as TLMessageCommon;
                            if (messageCommon != null)
                            {
                                message.Date = messageCommon.Date;
                            }
                        }
                    }
                }
            }

            _updatesService.ProcessUpdates(updates, notifyNewMessage);
        }

        public static void ProcessSelfMessage(TLMessageBase messageBase)
        {
            var messageCommon = messageBase as TLMessageCommon;
            if (messageCommon != null
                && messageCommon.ToId is TLPeerUser
                && messageCommon.FromId != null
                && messageCommon.FromId.Value == messageCommon.ToId.Id.Value)
            {
                messageCommon.Out = TLBool.True;
                messageCommon.SetUnreadSilent(TLBool.False);
            }
        }

        public void GetBotCallbackAnswerAsync(TLInputPeerBase peer, TLInt messageId, TLString data, TLBool game, Action<TLBotCallbackAnswer> callback, Action<TLRPCError> faultCallback = null)
        {
            var key = string.Format("{0}_{1}_{2}_{3}", peer, messageId, data, game);

            TLBotCallbackAnswer58 botCallbackAnswer;
            if (TryGetCachedValue(_cache, key, out botCallbackAnswer, IsCacheTimeValid))
            {
                callback.SafeInvoke(botCallbackAnswer);
                return;
            }

            var obj = new TLGetBotCallbackAnswer { Peer = peer, MessageId = messageId, Data = data };
            if (game != null && game.Value)
            {
                obj.SetGame();
            }

            const string caption = "messages.getBotCallbackAnswer";
            SendInformativeMessage<TLBotCallbackAnswer>(caption, obj,
                result =>
                {
                    SetCachedValue(ref _cache, key, result as ICachedObject, IsCacheTimeValid);

                    callback.SafeInvoke(result);
                },
                faultCallback);
        }

        private Dictionary<string, Tuple<DateTime, WeakReference<ICachedObject>>> _cache;

        private static bool IsCacheTimeValid(DateTime cachedDate, ICachedObject cachedObject)
        {
            if (cachedObject != null && cachedDate.AddSeconds(cachedObject.CacheTime.Value) > DateTime.Now)
            {
                return true;
            }

            return false;
        }

        private static void SetCachedValue<T>(ref Dictionary<string, Tuple<DateTime, WeakReference<T>>> dict, string key, T result, Func<DateTime, T, bool> predicate)
            where T : class
        {
            var now = DateTime.Now;
            if (!predicate(now, result)) return;

            dict = dict ?? new Dictionary<string, Tuple<DateTime, WeakReference<T>>>();

            dict[key] = new Tuple<DateTime, WeakReference<T>>(now, new WeakReference<T>(result));
        }

        private static bool TryGetCachedValue<T1, T>(Dictionary<string, Tuple<DateTime, WeakReference<T>>> dict, string key, out T1 cachedValue, Func<DateTime, T, bool> predicate)
            where T : class
            where T1 : class
        {
            Tuple<DateTime, WeakReference<T>> tuple;
            cachedValue = null;

            T target;
            if (dict != null
                && dict.TryGetValue(key, out tuple)
                && tuple.Item2.TryGetTarget(out target)
                && predicate(tuple.Item1, target))
            {
                if (target is T1)
                {
                    cachedValue = target as T1;
                    return true;
                }
            }

            return false;
        }

        public void StartBotAsync(TLInputUserBase bot, TLString startParam, TLMessage25 message, Action<TLUpdatesBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLStartBot { Bot = bot, Peer = PeerToInputPeer(message.ToId), RandomId = message.RandomId, StartParam = startParam };

            const string caption = "messages.startBot";
            StartBotAsyncInternal(obj,
                result =>
                {
                    Execute.BeginOnUIThread(() =>
                    {
                        message.Status = GetMessageStatus(_cacheService, message.ToId);
                        message.Media.LastProgress = 0.0;
                        message.Media.DownloadingProgress = 0.0;
                    });

                    var multiPts = result as IMultiPts;
                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption);
                    }
                    else
                    {
                        ProcessUpdates(result, new List<TLMessage25> { message });
                    }

                    callback.SafeInvoke(result);
                },
                () =>
                {
                    //TLUtils.WriteLine(caption + " fast result " + message.RandomIndex, LogSeverity.Error);
                    //fastCallback();
                },
                faultCallback.SafeInvoke);
        }

        public void UploadMediaAsync(TLInputPeerBase inputPeer, TLInputMediaBase inputMedia, Action<TLMessageMediaBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLUploadMedia { Peer = inputPeer, Media = inputMedia };

            const string caption = "messages.uploadMedia";
            SendInformativeMessage(caption, obj, callback, faultCallback);
        }

        public void SendMultiMediaAsync(TLInputPeerBase inputPeer, TLVector<TLInputSingleMedia> inputMedia, TLMessage25 message, Action<TLUpdatesBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLSendMultiMedia { Flags = new TLInt(0), Peer = inputPeer, ReplyToMsgId = message.ReplyToMsgId, MultiMedia = inputMedia };

            var message48 = message as TLMessage48;
            if (message48 != null && message48.Silent)
            {
                obj.SetSilent();
            }

            const string caption = "messages.sendMultiMedia";
            SendMultiMediaAsyncInternal(obj,
                result =>
                {
                    Execute.BeginOnUIThread(() =>
                    {
                        message.Status = GetMessageStatus(_cacheService, message.ToId);
                        message.Media.LastProgress = 0.0;
                        message.Media.DownloadingProgress = 1.0;

                        var mediaGroup = message.Media as TLMessageMediaGroup;
                        if (mediaGroup != null)
                        {
                            foreach (var m in mediaGroup.Group.OfType<TLMessage>())
                            {
                                m.Status = GetMessageStatus(_cacheService, message.ToId);
                                m.Media.LastProgress = 0.0;
                                m.Media.DownloadingProgress = 1.0;
                            }
                        }
                    });

                    var multiPts = result as IMultiPts;
                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption);
                    }
                    else
                    {
                        var handled = false;
                        var message73 = message as TLMessage73;
                        if (message73 != null)
                        {
                            var mediaGroup = message73.Media as TLMessageMediaGroup;
                            if (mediaGroup != null)
                            {
                                var messages = new List<TLMessage25>();
                                foreach (var item in mediaGroup.Group)
                                {
                                    messages.Add((TLMessage25)item);
                                }

                                if (messages.Count > 0)
                                {
                                    handled = true;
                                    ProcessUpdates(result, messages);
                                    message.Id = messages[messages.Count - 1].Id;
                                    message.Date = messages[messages.Count - 1].Date;
                                    message.RandomId = new TLLong(0);
                                }
                            }
                        }

                        if (!handled)
                        {
                            ProcessUpdates(result, new List<TLMessage25> { message });
                        }
                    }

                    callback.SafeInvoke(result);
                },
                () =>
                {

                },
                faultCallback.SafeInvoke);
        }

        public void SendMediaAsync(TLInputPeerBase inputPeer, TLInputMediaBase inputMedia, TLMessage34 message, Action<TLUpdatesBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLSendMedia { Flags = new TLInt(0), Peer = inputPeer, ReplyToMsgId = message.ReplyToMsgId, Media = inputMedia, Message = message.Message, RandomId = message.RandomId, ReplyMarkup = null, Entities = message.Entities };

            if (message.IsChannelMessage)
            {
                obj.SetChannelMessage();
            }

            var message48 = message as TLMessage48;
            if (message48 != null && message48.Silent)
            {
                obj.SetSilent();
            }

            const string caption = "messages.sendMedia";
            SendMediaAsyncInternal(obj,
                result =>
                {
                    Execute.BeginOnUIThread(() =>
                    {
                        message.Status = GetMessageStatus(_cacheService, message.ToId);
                        message.Media.LastProgress = 0.0;
                        message.Media.DownloadingProgress = 1.0;
                    });

                    var multiPts = result as IMultiPts;
                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption);
                    }
                    else
                    {
                        ProcessUpdates(result, new List<TLMessage25> { message });
                    }

                    callback.SafeInvoke(result);
                },
                () =>
                {

                },
                faultCallback.SafeInvoke);
        }

        public void SendBroadcastAsync(TLVector<TLInputUserBase> contacts, TLInputMediaBase inputMedia, TLMessage25 message, Action<TLUpdatesBase> callback, Action fastCallback, Action<TLRPCError> faultCallback = null)
        {
            var randomId = new TLVector<TLLong>();
            for (var i = 0; i < contacts.Count; i++)
            {
                randomId.Add(TLLong.Random());
            }

            var obj = new TLSendBroadcast { Contacts = contacts, RandomId = randomId, Message = message.Message, Media = inputMedia };



            const string caption = "messages.sendBroadcast";
            SendInformativeMessage<TLUpdatesBase>(caption,
                obj,
                result =>
                {
                    var multiPts = result as IMultiPts;
                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption);
                    }
                    else
                    {
                        ProcessUpdates(result, new List<TLMessage25> { message });
                    }

                    var updates = result as TLUpdates;
                    if (updates != null)
                    {
                        var updateNewMessage = updates.Updates.FirstOrDefault(x => x is TLUpdateNewMessage) as TLUpdateNewMessage;
                        if (updateNewMessage != null)
                        {
                            var messageCommon = updateNewMessage.Message as TLMessageCommon;
                            if (messageCommon != null)
                            {
                                message.Date = new TLInt(messageCommon.DateIndex - 1); // Делаем бродкаст после всех чатов, в которые отправили, в списке диалогов
                            }
                        }
                    }

                    //message.Id = result.Id;
                    message.Status = MessageStatus.Confirmed;

                    callback.SafeInvoke(result);
                },
                faultCallback);
        }



        public void SendEncryptedAsync(TLInputEncryptedChat peer, TLLong randomId, TLString data, Action<TLSentEncryptedMessage> callback, Action fastCallback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLSendEncrypted { Peer = peer, RandomId = randomId, Data = data };

            SendEncryptedAsyncInternal(
                obj,
                result =>
                {
                    callback(result);
                },
                () =>
                {

                },
                faultCallback);
        }

        private object _sendEncryptedFileSyncRoot;

        public void SendEncryptedMultiMediaAsync(TLInputEncryptedChat peer, TLVector<TLLong> randomId, TLVector<TLString> data, TLVector<TLInputEncryptedFileBase> file, Action<TLVector<TLSentEncryptedFile>> callback, Action fastCallback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLVector<TLSendEncryptedFile>();
            for (var i = 0; i < randomId.Count; i++)
            {
                obj.Add(new TLSendEncryptedFile { Peer = peer, RandomId = randomId[i], Data = data[i], File = file[i] });
            }

            _sendEncryptedFileSyncRoot = _sendEncryptedFileSyncRoot ?? new object();
            var results = new TLVector<TLSentEncryptedFile>();

            SendEncryptedFileAsyncInternal(
                obj,
                result =>
                {
                    lock (_sendEncryptedFileSyncRoot)
                    {
                        results.Add(result);
                    }

                    if (results.Count == randomId.Count)
                    {
                        callback.SafeInvoke(results);
                    }
                },
                () =>
                {

                },
                faultCallback);
        }

        public void SendEncryptedFileAsync(TLInputEncryptedChat peer, TLLong randomId, TLString data, TLInputEncryptedFileBase file, Action<TLSentEncryptedFile> callback, Action fastCallback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLSendEncryptedFile { Peer = peer, RandomId = randomId, Data = data, File = file };

            SendEncryptedFileAsyncInternal(
                obj,
                callback,
                () =>
                {

                },
                faultCallback);
        }

        public void SendEncryptedServiceAsync(TLInputEncryptedChat peer, TLLong randomId, TLString data, Action<TLSentEncryptedMessage> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLSendEncryptedService { Peer = peer, RandomId = randomId, Data = data };

            SendEncryptedServiceAsyncInternal(
                obj,
                callback,
                () =>
                {

                },
                faultCallback);
        }

        public void ReadEncryptedHistoryAsync(TLInputEncryptedChat peer, TLInt maxDate, Action<TLBool> callback,
            Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLReadEncryptedHistory { Peer = peer, MaxDate = maxDate };

            ReadEncryptedHistoryAsyncInternal(obj, callback, () => { }, faultCallback);
        }

        public void SetEncryptedTypingAsync(TLInputEncryptedChat peer, TLBool typing, Action<TLBool> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLSetEncryptedTyping { Peer = peer, Typing = typing };

            SendInformativeMessage("messages.setEncryptedTyping", obj, callback, faultCallback);
        }

        public void SetTypingAsync(TLInputPeerBase peer, TLBool typing, Action<TLBool> callback, Action<TLRPCError> faultCallback = null)
        {
            var action = typing.Value ? (TLSendMessageActionBase)new TLSendMessageTypingAction() : new TLSendMessageCancelAction();
            var obj = new TLSetTyping { Peer = peer, Action = action };

            SendInformativeMessage("messages.setTyping", obj, callback, faultCallback);
        }

        public void SetTypingAsync(TLInputPeerBase peer, TLSendMessageActionBase action, Action<TLBool> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLSetTyping { Peer = peer, Action = action ?? new TLSendMessageTypingAction() };

            SendInformativeMessage("messages.setTyping", obj, callback, faultCallback);
        }

        public void GetMessagesAsync(TLVector<TLInputMessageBase> id, Action<TLMessagesBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetMessages { Id = id };

            SendInformativeMessage("messages.getMessages", obj, callback, faultCallback);
        }

        private static void ProcessPinnedId(IList<TLDialogBase> result)
        {
            var pinnedIndex = 0;
            foreach (var dialog in result)
            {
                var dialog53 = dialog as TLDialog53;
                if (dialog53 != null && dialog53.IsPinned)
                {
                    dialog53.PinnedId = new TLInt(pinnedIndex++);
                }
            }
        }

        private static void ProcessUnread(TLDialogsBase result)
        {
            var dialogsCache = new Dictionary<int, List<TLDialogBase>>();
            foreach (var dialogBase in result.Dialogs)
            {
                List<TLDialogBase> dialogs;
                if (dialogsCache.TryGetValue(dialogBase.TopMessageId.Value, out dialogs))
                {
                    dialogs.Add(dialogBase);
                }
                else
                {
                    dialogsCache[dialogBase.TopMessageId.Value] = new List<TLDialogBase> { dialogBase };
                }
            }

            foreach (var messageBase in result.Messages)
            {
                ProcessSelfMessage(messageBase);

                var messageCommon = messageBase as TLMessageCommon;
                if (messageCommon != null)
                {
                    List<TLDialogBase> dialogs;
                    if (dialogsCache.TryGetValue(messageBase.Index, out dialogs))
                    {
                        TLDialog53 dialog53 = null;
                        if (messageCommon.ToId is TLPeerChannel)
                        {
                            dialog53 =
                                dialogs.FirstOrDefault(
                                        x => x.Peer is TLPeerChannel && x.Peer.Id.Value == messageCommon.ToId.Id.Value) as
                                    TLDialog53;
                        }
                        else if (messageCommon.ToId is TLPeerChat)
                        {
                            dialog53 =
                                dialogs.FirstOrDefault(
                                    x => x.Peer is TLPeerChat && x.Peer.Id.Value == messageCommon.ToId.Id.Value) as TLDialog53;
                        }
                        else if (messageCommon.ToId is TLPeerUser)
                        {
                            var peer = messageCommon.Out.Value ? messageCommon.ToId : new TLPeerUser { Id = messageCommon.FromId };
                            dialog53 =
                                dialogs.FirstOrDefault(x => x.Peer is TLPeerUser && x.Peer.Id.Value == peer.Id.Value) as
                                    TLDialog53;
                        }
                        if (dialog53 != null)
                        {
                            if (messageCommon.Out.Value)
                            {
                                if (messageCommon.Index > dialog53.ReadOutboxMaxId.Value)
                                {
                                    messageCommon.SetUnreadSilent(TLBool.True);
                                }
                            }
                            else
                            {
                                if (messageCommon.Index > dialog53.ReadInboxMaxId.Value)
                                {
                                    messageCommon.SetUnreadSilent(TLBool.True);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void GetPeerDialogsAsync(TLInputPeerBase peer, Action<TLPeerDialogs> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetPeerDialogs { Peers = new TLVector<TLInputPeerBase> { peer } };

            SendInformativeMessage<TLPeerDialogs>("messages.getPeerDialogs", obj,
                result =>
                {
                    ProcessUnread(result.ToDialogs());

                    _cacheService.SyncDialogs(Stopwatch.StartNew(),
                        result.ToDialogs(),
                        r =>
                        {
                            callback.SafeInvoke(r.ToPeerDialogs(result.State));
                        });
                },
                faultCallback);
        }

        public void GetPromoDialogAsync(TLInputPeerBase peer, Action<TLPeerDialogs> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetPeerDialogs { Peers = new TLVector<TLInputPeerBase> { peer } };

            SendInformativeMessage<TLPeerDialogs>("messages.getPeerDialogs", obj,
                result =>
                {
                    var oldDialogs = _cacheService.GetDialogs();
                    var oldPromoDialogs = new List<TLDialogBase>();
                    foreach (var dialogBase in oldDialogs)
                    {
                        var dialog = dialogBase as TLDialog71;
                        if (dialog != null && dialog.IsPromo)
                        {
                            oldPromoDialogs.Add(dialog);
                        }
                    }

                    ProcessUnread(result.ToDialogs());

                    foreach (var dialogBase in oldPromoDialogs)
                    {
                        _cacheService.UpdateDialogPromo(dialogBase, false);
                    }

                    foreach (var dialogBase in result.Dialogs)
                    {
                        var channel = _cacheService.GetChat(dialogBase.Peer.Id) as TLChannel;
                        if (channel != null && channel.Left.Value)
                        {
                            _cacheService.UpdateDialogPromo(dialogBase, true);
                        }
                    }

                    _cacheService.SyncDialogs(Stopwatch.StartNew(),
                        result.ToDialogs(),
                        r =>
                        {
                            callback.SafeInvoke(r.ToPeerDialogs(result.State));
                        });
                },
                faultCallback);
        }

        public void GetPinnedDialogsAsync(Action<TLPeerDialogs> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetPinnedDialogs();

            const string caption = "messages.getPinnedDialogs";
            SendInformativeMessage<TLPeerDialogs>(caption, obj,
                result =>
                {
                    var oldDialogs = _cacheService.GetDialogs();
                    var oldPinnedDialogs = new List<TLDialogBase>();
                    foreach (var dialogBase in oldDialogs)
                    {
                        var dialog = dialogBase as TLDialog53;
                        if (dialog != null
                            && dialog.IsPinned
                            && result.Dialogs.FirstOrDefault(x => x.Index == dialog.Index) == null)
                        {
                            oldPinnedDialogs.Add(dialog);
                        }
                    }

                    ProcessPinnedId(result.Dialogs);

                    ProcessUnread(result.ToDialogs());

                    foreach (var dialogBase in oldPinnedDialogs)
                    {
                        _cacheService.UpdateDialogPinned(dialogBase.Peer, false);
                    }

                    _cacheService.SyncDialogs(Stopwatch.StartNew(),
                        result.ToDialogs(), r =>
                        {
                            callback.SafeInvoke(r.ToPeerDialogs(result.State));
                        });
                },
                faultCallback);
        }

        public void GetDialogsAsync(Stopwatch stopwatch, TLInt offsetDate, TLInt offsetId, TLInputPeerBase offsetPeer, TLInt limit, TLInt hash, Action<TLDialogsBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetDialogs { Flags = new TLInt(0), /*FeedId = null,*/ OffsetDate = offsetDate, OffsetId = offsetId, OffsetPeer = offsetPeer, Limit = limit, Hash = hash };

            if (offsetDate.Value != 0
                || offsetId.Value != 0
                || !(offsetPeer is TLInputPeerEmpty))
            {
                obj.ExcludePinned = true;
            }

            SendInformativeMessage<TLDialogsBase>("messages.getDialogs", obj,
                result =>
                {
                    if (offsetDate.Value == 0 && !obj.ExcludePinned)
                    {
                        ProcessPinnedId(result.Dialogs);
                    }

                    ProcessUnread(result);

                    _cacheService.SyncDialogs(stopwatch, result, callback);
                },
                faultCallback);
        }

        public void GetChannelDialogsAsync(TLInt offset, TLInt limit, Action<TLDialogsBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TL.Functions.Channels.TLGetDialogs { Offset = offset, Limit = limit };

            SendInformativeMessage<TLDialogsBase>("channels.getDialogs", obj, result =>
            {
                //return;
                var channelsCache = new Context<TLChannel>();
                foreach (var chatBase in result.Chats)
                {
                    var channel = chatBase as TLChannel;
                    if (channel != null)
                    {
                        channelsCache[channel.Index] = channel;
                    }
                }

                var dialogsCache = new Context<TLDialogChannel>();
                foreach (var dialogBase in result.Dialogs)
                {
                    var dialogChannel = dialogBase as TLDialogChannel;
                    if (dialogChannel != null)
                    {
                        var channelId = dialogChannel.Peer.Id.Value;
                        dialogsCache[channelId] = dialogChannel;
                        TLChannel channel;
                        if (channelsCache.TryGetValue(channelId, out channel))
                        {
                            channel.ReadInboxMaxId = dialogChannel.ReadInboxMaxId;
                            //channel.UnreadCount = dialogChannel.UnreadCount;
                            //channel.UnreadImportantCount = dialogChannel.UnreadImportantCount;
                            channel.NotifySettings = dialogChannel.NotifySettings;
                            //channel.Pts = dialogChannel.Pts;
                        }
                    }
                }

                //_cacheService.SyncChannelDialogs(result, callback);
                _cacheService.SyncUsersAndChats(result.Users, result.Chats, x =>
                {
                    _cacheService.MergeMessagesAndChannels(result);

                    callback.SafeInvoke(result);
                });
            }, faultCallback);
        }

        private void GetChannelHistoryAsyncInternal(bool sync, TLPeerBase peer, TLMessagesBase result, Action<TLMessagesBase> callback)
        {
            if (sync)
            {
                _cacheService.SyncPeerMessages(peer, result, false, false, callback);
            }
            else
            {
                _cacheService.AddChats(result.Chats, results => { });
                _cacheService.AddUsers(result.Users, results => { });
                callback(result);
            }
        }

        private void GetHistoryAsyncInternal(bool sync, TLPeerBase peer, TLMessagesBase result, Action<TLMessagesBase> callback)
        {
            if (sync)
            {
                _cacheService.SyncPeerMessages(peer, result, false, true, callback);
            }
            else
            {
                _cacheService.AddChats(result.Chats, results => { });
                _cacheService.AddUsers(result.Users, results => { });
                callback(result);
            }
        }

        public void GetHistoryAsync(Stopwatch timer, TLInputPeerBase inputPeer, TLPeerBase peer, bool sync, TLInt offsetDate, TLInt offset, TLInt maxId, TLInt limit, Action<TLMessagesBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetHistory { Peer = inputPeer, AddOffset = offset, OffsetId = maxId, OffsetDate = offsetDate, Limit = limit, MaxId = new TLInt(int.MaxValue), MinId = new TLInt(0), Hash = new TLInt(0) };

            //Telegram.Api.Helpers.Execute.ShowDebugMessage(string.Format("messages.getHistory offset_date=[{0} {1}], offset={2} max_id={3} limit={4}", offsetDate, TLUtils.ToDateTime(offsetDate).ToString("dd-MM-yyyy HH:mm:ss.f"), offset, maxId, limit));
            //Debug.WriteLine("UpdateItems start request elapsed=" + (timer != null? timer.Elapsed.ToString() : null));

            SendInformativeMessage<TLMessagesBase>("messages.getHistory", obj,
                result =>
                {
                    var peerChannel = inputPeer as TLInputPeerChannel;
                    if (peerChannel != null)
                    {
                        var channel = result.Chats.FirstOrDefault(x => x.Index == peerChannel.ChatId.Value) as TLChannel;
                        if (channel != null && channel.Left.Value)
                        {
                            sync = false;
                        }
                    }
                    //Debug.WriteLine("UpdateItems stop request elapsed=" + (timer != null ? timer.Elapsed.ToString() : null));

                    foreach (var message in result.Messages)
                    {
                        ProcessSelfMessage(message);
                    }
                    var replyId = new TLVector<TLInputMessageBase>();
                    var waitingList = new List<TLMessage25>();
                    //for (var i = 0; i < result.Messages.Count; i++)
                    //{
                    //    var message25 = result.Messages[i] as TLMessage25;
                    //    if (message25 != null 
                    //        && message25.ReplyToMsgId != null 
                    //        && message25.ReplyToMsgId.Value > 0)
                    //    {
                    //        var cachedReply = _cacheService.GetMessage(message25.ReplyToMsgId);
                    //        if (cachedReply != null)
                    //        {
                    //            message25.Reply = cachedReply;
                    //        }
                    //        else
                    //        {
                    //            replyId.Add(message25.ReplyToMsgId);
                    //            waitingList.Add(message25);
                    //        }
                    //    }
                    //}

                    if (replyId.Count > 0)
                    {
                        //Debug.WriteLine("UpdateItems start GetMessages elapsed=" + (timer != null ? timer.Elapsed.ToString() : null));

                        GetMessagesAsync(
                            replyId,
                            messagesResult =>
                            {
                                //Debug.WriteLine("UpdateItems stop GetMessages elapsed=" + (timer != null ? timer.Elapsed.ToString() : null));

                                _cacheService.AddChats(result.Chats, results => { });
                                _cacheService.AddUsers(result.Users, results => { });

                                for (var i = 0; i < messagesResult.Messages.Count; i++)
                                {
                                    for (var j = 0; j < waitingList.Count; j++)
                                    {
                                        var messageToReply = messagesResult.Messages[i] as TLMessageCommon;
                                        if (messageToReply != null
                                            && messageToReply.Index == waitingList[j].Index)
                                        {
                                            waitingList[j].Reply = messageToReply;
                                        }
                                    }
                                }

                                var inputChannelPeer = inputPeer as TLInputPeerChannel;
                                if (inputChannelPeer != null)
                                {
                                    var channel = _cacheService.GetChat(inputChannelPeer.ChatId) as TLChannel;
                                    if (channel != null)
                                    {
                                        var maxIndex = channel.ReadInboxMaxId != null ? channel.ReadInboxMaxId.Value : 0;
                                        foreach (var messageBase in messagesResult.Messages)
                                        {
                                            var messageCommon = messageBase as TLMessageCommon;
                                            if (messageCommon != null
                                                && !messageCommon.Out.Value
                                                && messageCommon.Index > maxIndex)
                                            {
                                                messageCommon.SetUnread(TLBool.True);
                                            }
                                        }
                                    }
                                }

                                //Debug.WriteLine("UpdateItems stop GetMessages GetHistoryAsyncInternal elapsed=" + (timer != null ? timer.Elapsed.ToString() : null));

                                GetHistoryAsyncInternal(sync, peer, result, callback);
                            },
                            faultCallback);
                    }
                    else
                    {
                        //Debug.WriteLine("UpdateItems GetHistoryAsyncInternal elapsed=" + (timer != null ? timer.Elapsed.ToString() : null));

                        GetHistoryAsyncInternal(sync, peer, result, callback);
                    }
                },
                faultCallback);
        }


        public void GetChannelHistoryAsync(string debugInfo, TLInputPeerBase inputPeer, TLPeerBase peer, bool sync, TLInt offset, TLInt maxId, TLInt limit, Action<TLMessagesBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetHistory { Peer = inputPeer, AddOffset = offset, OffsetId = maxId, OffsetDate = new TLInt(0), Limit = limit, MaxId = new TLInt(int.MaxValue), MinId = new TLInt(0), Hash = new TLInt(0) };

            TLUtils.WriteLine(string.Format("{0} {1} messages.getHistory peer={2} offset={3} max_id={4} limit={5}", string.Empty, debugInfo, inputPeer, offset, maxId, limit), LogSeverity.Error);
            SendInformativeMessage<TLMessagesBase>("messages.getHistory", obj,
                result =>
                {
                    var peerChannel = inputPeer as TLInputPeerChannel;
                    if (peerChannel != null)
                    {
                        var channel = result.Chats.FirstOrDefault(x => x.Index == peerChannel.ChatId.Value) as TLChannel;
                        if (channel != null && channel.Left.Value)
                        {
                            sync = false;
                        }
                    }

                    var replyId = new TLVector<TLInputMessageBase>();
                    var waitingList = new List<TLMessage25>();
                    //for (var i = 0; i < result.Messages.Count; i++)
                    //{
                    //    var message25 = result.Messages[i] as TLMessage25;
                    //    if (message25 != null 
                    //        && message25.ReplyToMsgId != null 
                    //        && message25.ReplyToMsgId.Value > 0)
                    //    {
                    //        var cachedReply = _cacheService.GetMessage(message25.ReplyToMsgId);
                    //        if (cachedReply != null)
                    //        {
                    //            message25.Reply = cachedReply;
                    //        }
                    //        else
                    //        {
                    //            replyId.Add(message25.ReplyToMsgId);
                    //            waitingList.Add(message25);
                    //        }
                    //    }
                    //}

                    if (replyId.Count > 0)
                    {
                        GetMessagesAsync(
                            replyId,
                            messagesResult =>
                            {
                                _cacheService.AddChats(result.Chats, results => { });
                                _cacheService.AddUsers(result.Users, results => { });

                                for (var i = 0; i < messagesResult.Messages.Count; i++)
                                {
                                    for (var j = 0; j < waitingList.Count; j++)
                                    {
                                        var messageToReply = messagesResult.Messages[i] as TLMessageCommon;
                                        if (messageToReply != null
                                            && messageToReply.Index == waitingList[j].Index)
                                        {
                                            waitingList[j].Reply = messageToReply;
                                        }
                                    }
                                }

                                var inputChannelPeer = inputPeer as TLInputPeerChannel;
                                if (inputChannelPeer != null)
                                {
                                    var channel = _cacheService.GetChat(inputChannelPeer.ChatId) as TLChannel;
                                    if (channel != null)
                                    {
                                        var maxIndex = channel.ReadInboxMaxId != null ? channel.ReadInboxMaxId.Value : 0;
                                        foreach (var messageBase in messagesResult.Messages)
                                        {
                                            var messageCommon = messageBase as TLMessageCommon;
                                            if (messageCommon != null
                                                && !messageCommon.Out.Value
                                                && messageCommon.Index > maxIndex)
                                            {
                                                messageCommon.SetUnread(TLBool.True);
                                            }
                                        }
                                    }
                                }

                                GetChannelHistoryAsyncInternal(sync, peer, result, callback);
                            },
                            faultCallback);
                    }
                    else
                    {
                        GetChannelHistoryAsyncInternal(sync, peer, result, callback);
                    }
                },
                faultCallback);
        }

        public void SearchAsync(TLInputPeerBase peer, TLString query, TLInputUserBase fromId, TLInputMessagesFilterBase filter, TLInt minDate, TLInt maxDate, TLInt addOffset, TLInt offsetId, TLInt limit, TLInt hash, Action<TLMessagesBase> callback, Action<TLRPCError> faultCallback = null)
        {
            Debug.WriteLine("messages.search query={0} from_id={7} filter={1} minDate={2} maxDate={3} offset={4} maxId={5} limit={6}", query, filter, maxDate, maxDate, addOffset, offsetId, limit, fromId);

            var obj = new TLSearch { Flags = new TLInt(0), Peer = peer, Query = query, FromId = fromId, Filter = filter, MinDate = minDate, MaxDate = maxDate, AddOffset = addOffset, OffsetId = offsetId, Limit = limit, MinId = new TLInt(0), MaxId = new TLInt(0), Hash = hash };

            SendInformativeMessage<TLMessagesBase>("messages.search", obj, result =>
            {
                callback.SafeInvoke(result);
            }, faultCallback);
        }

        public void SearchGlobalAsync(TLString query, TLInt offsetDate, TLInputPeerBase offsetPeer, TLInt offsetId, TLInt limit, Action<TLMessagesBase> callback, Action<TLRPCError> faultCallback = null)
        {
            TLUtils.WriteLine(string.Format("{0} messages.searchGlobal query={1} offset_date={2} offset_peer={3} offset_id={4} limit={5}", DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture), query, offsetDate, offsetPeer, offsetId, limit), LogSeverity.Error);

            var obj = new TLSearchGlobal { Query = query, OffsetDate = offsetDate, OffsetPeer = offsetPeer, OffsetId = offsetId, Limit = limit };

            SendInformativeMessage<TLMessagesBase>("messages.searchGlobal", obj, result =>
            {
                TLUtils.WriteLine(string.Format("{0} messages.searchGlobal result={1}", DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture), result.Messages.Count), LogSeverity.Error);
                callback.SafeInvoke(result);
            }, faultCallback);
        }

        public void ReadHistoryAsync(TLInputPeerBase peer, TLInt maxId, TLInt offset, Action<TLAffectedMessages> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLReadHistory { Peer = peer, MaxId = maxId };

            const string caption = "messages.readHistory";
            ReadHistoryAsyncInternal(obj,
                result =>
                {
                    var multiPts = result as IMultiPts;
                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption);
                    }
                    else
                    {
                        _updatesService.SetState(null, result.Pts, null, null, null, caption);
                    }

                    callback.SafeInvoke(result);
                },
                () => { },
                faultCallback.SafeInvoke);
        }

        public void ReadMentionsAsync(TLInputPeerBase peer, Action<TLAffectedHistory24> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLReadMentions { Peer = peer };

            const string caption = "messages.readMentions";
            ReadMentionsAsyncInternal(obj,
                result =>
                {
                    var multiPts = result as IMultiPts;
                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption);
                    }
                    else
                    {
                        _updatesService.SetState(null, result.Pts, null, null, null, caption);
                    }

                    callback.SafeInvoke(result);
                },
                () => { },
                faultCallback.SafeInvoke);
        }

        public void ReadMessageContentsAsync(TLVector<TLInt> id, Action<TLAffectedMessages> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLReadMessageContents { Id = id };

            const string caption = "messages.readMessageContents";
            ReadMessageContentsAsyncInternal(obj,
                result =>
                {
                    var multiPts = result as IMultiPts;
                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption);
                    }
                    else
                    {
                        _updatesService.SetState(null, result.Pts, null, null, null, caption);
                    }

                    callback.SafeInvoke(result);
                },
                () => { },
                faultCallback.SafeInvoke);
        }

        public void DeleteHistoryAsync(bool justClear, TLInputPeerBase peer, TLInt offset, Action<TLAffectedHistory> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLDeleteHistory { Flags = new TLInt(0), Peer = peer, MaxId = new TLInt(int.MaxValue) };

            if (justClear)
            {
                obj.SetJustClear();
            }

            const string caption = "messages.deleteHistory";
            SendInformativeMessage<TLAffectedHistory>(caption, obj,
                result =>
                {
                    var multiPts = result as IMultiPts;
                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption);
                    }
                    else
                    {
                        _updatesService.SetState(result.Seq, result.Pts, null, null, null, caption);
                    }

                    callback(result);
                },
                faultCallback);
        }

        public void DeleteMessagesAsync(bool revoke, TLVector<TLInt> id, Action<TLAffectedMessages> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLDeleteMessages { Id = id, Revoke = revoke };

            const string caption = "messages.deleteMessages";
            SendInformativeMessage<TLAffectedMessages>(caption, obj,
                result =>
                {
                    var multiPts = result as IMultiPts;
                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption);
                    }
                    else
                    {
                        _updatesService.SetState(null, result.Pts, null, null, null, caption);
                    }

                    callback(result);
                },
                faultCallback);
        }

        public void RestoreMessagesAsync(TLVector<TLInt> id, Action<TLVector<TLInt>> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLRestoreMessages { Id = id };

            SendInformativeMessage("messages.restoreMessages", obj, callback, faultCallback);
        }

        public void ReceivedMessagesAsync(TLInt maxId, Action<TLVector<TLReceivedNotifyMessage>> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLReceivedMessages { MaxId = maxId };

            SendInformativeMessage("messages.receivedMessages", obj, callback, faultCallback);
        }

        public void ForwardMessageAsync(TLInputPeerBase peer, TLInt fwdMessageId, TLMessage25 message, Action<TLUpdatesBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLForwardMessage { Peer = peer, Id = fwdMessageId, RandomId = message.RandomId };

            const string caption = "messages.forwardMessage";
            ForwardMessageAsyncInternal(obj,
                result =>
                {
                    Execute.BeginOnUIThread(() =>
                    {
                        message.Status = MessageStatus.Confirmed;
                        message.Media.LastProgress = 0.0;
                        message.Media.DownloadingProgress = 0.0;
                    });

                    var multiPts = result as IMultiPts;
                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption);
                    }
                    else
                    {
                        ProcessUpdates(result, new List<TLMessage25> { message });
                    }

                    callback.SafeInvoke(result);
                },
                () =>
                {

                },
                faultCallback);
        }

        private readonly object _forwardMessagesWithCommentSyncRoot = new object();

        public void ForwardMessagesAsync(TLMessage25 commentMessage, TLInputPeerBase toPeer, TLVector<TLInt> id, IList<TLMessage25> messages, bool withMyScore, Action<TLUpdatesBase[]> callback, Action<TLRPCError> faultCallback = null)
        {
            var randomId = new TLVector<TLLong>();
            foreach (var message in messages)
            {
                randomId.Add(message.RandomId);
            }

            TLInputPeerBase fromPeer = null;
            var message48 = messages.FirstOrDefault() as TLMessage48;
            if (message48 != null)
            {
                fromPeer = message48.FwdFromChannelPeer;
            }

            if (fromPeer == null)
            {
                var message40 = messages.FirstOrDefault() as TLMessage40;
                if (message40 != null)
                {
                    fromPeer = message40.FwdFromChannelPeer ?? PeerToInputPeer(message40.FwdFromPeer);
                }
            }

            var obj1 = commentMessage != null && !TLString.IsNullOrEmpty(commentMessage.Message) ? new TLSendMessage { Flags = new TLInt(0), Peer = toPeer, Message = commentMessage.Message, RandomId = commentMessage.RandomId } : null;
            var obj2 = new TLForwardMessages { ToPeer = toPeer, Id = id, RandomIds = randomId, FromPeer = fromPeer, Flags = new TLInt(0) };

            if (message48 != null && message48.IsChannelMessage)
            {
                obj2.SetChannelMessage();
            }

            if (message48 != null && message48.Silent)
            {
                obj2.SetSilent();
            }

            if (withMyScore)
            {
                obj2.SetWithMyScore();
            }

            var grouped = messages.OfType<TLMessage73>().FirstOrDefault(x => x.GroupedId != null) != null;
            if (grouped)
            {
                obj2.SetGrouped();
            }

            const string caption1 = "messages.sendMessage";
            const string caption2 = "messages.forwardMessages";

            var results = new TLUpdatesBase[2];

            ForwardMessagesWithCommentAsyncInternal(obj1, obj2,
                result =>
                {
                    bool invokeCallback;

                    var multiPts = result as IMultiPts;
                    var shortSentMessage = result as TLUpdatesShortSentMessage;
                    if (shortSentMessage != null)
                    {
                        commentMessage.Flags = shortSentMessage.Flags;
                        if (shortSentMessage.HasMedia)
                        {
                            commentMessage._media = shortSentMessage.Media;
                        }
                        if (shortSentMessage.HasEntities)
                        {
                            var commentMessage70 = commentMessage as TLMessage70;
                            if (commentMessage70 != null)
                            {
                                commentMessage70.Entities = shortSentMessage.Entities;
                            }
                        }

                        Execute.BeginOnUIThread(() =>
                        {
                            commentMessage.Status = GetMessageStatus(_cacheService, commentMessage.ToId);
                            commentMessage.Date = shortSentMessage.Date;
                            if (shortSentMessage.Media is TLMessageMediaWebPage)
                            {
                                commentMessage.NotifyOfPropertyChange(() => commentMessage.Media);
                            }

#if DEBUG
                            commentMessage.Id = shortSentMessage.Id;
                            commentMessage.NotifyOfPropertyChange(() => commentMessage.Id);
                            commentMessage.NotifyOfPropertyChange(() => commentMessage.Date);
#endif
                        });

                        _updatesService.SetState(multiPts, caption1);

                        commentMessage.Id = shortSentMessage.Id;
                        _cacheService.SyncSendingMessage(commentMessage, null,
                            cachedMessage =>
                            {
                                lock (_forwardMessagesWithCommentSyncRoot)
                                {
                                    results[0] = result;
                                    invokeCallback = obj1 == null || (results[0] != null && results[1] != null);
                                }

                                if (invokeCallback)
                                {
                                    callback.SafeInvoke(results);
                                }
                            });
                        return;
                    }

                    Execute.BeginOnUIThread(() =>
                    {
                        commentMessage.Status = GetMessageStatus(_cacheService, commentMessage.ToId);
                    });

                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption1);
                    }
                    else
                    {
                        ProcessUpdates(result, new List<TLMessage25> { commentMessage });
                    }

                    lock (_forwardMessagesWithCommentSyncRoot)
                    {
                        results[0] = result;
                        invokeCallback = obj1 == null || (results[0] != null && results[1] != null);
                    }

                    if (invokeCallback)
                    {
                        callback.SafeInvoke(results);
                    }
                },
                result =>
                {
                    Execute.BeginOnUIThread(() =>
                    {
                        for (var i = 0; i < messages.Count; i++)
                        {
                            messages[i].Status = MessageStatus.Confirmed;
                            messages[i].Media.LastProgress = 0.0;
                            messages[i].Media.DownloadingProgress = 0.0;
                        }
                    });

                    var multiPts = result as IMultiPts;
                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption2);
                    }
                    else
                    {
                        ProcessUpdates(result, messages);
                    }

                    bool invokeCallback;
                    lock (_forwardMessagesWithCommentSyncRoot)
                    {
                        results[1] = result;
                        invokeCallback = obj1 == null || (results[0] != null && results[1] != null);
                    }

                    if (invokeCallback)
                    {
                        callback.SafeInvoke(results);
                    }
                },
                () =>
                {

                },
                faultCallback.SafeInvoke);
        }

        public void ForwardMessagesAsync(TLInputPeerBase toPeer, TLVector<TLInt> id, IList<TLMessage25> messages, bool withMyScore, Action<TLUpdatesBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var randomId = new TLVector<TLLong>();
            foreach (var message in messages)
            {
                randomId.Add(message.RandomId);
            }

            TLInputPeerBase fromPeer = null;
            var message48 = messages.FirstOrDefault() as TLMessage48;
            if (message48 != null)
            {
                fromPeer = message48.FwdFromChannelPeer;
            }

            if (fromPeer == null)
            {
                var message40 = messages.FirstOrDefault() as TLMessage40;
                if (message40 != null)
                {
                    fromPeer = message40.FwdFromChannelPeer ?? PeerToInputPeer(message40.FwdFromPeer);
                }
            }

            var obj = new TLForwardMessages { ToPeer = toPeer, Id = id, RandomIds = randomId, FromPeer = fromPeer, Flags = new TLInt(0) };

            if (message48 != null && message48.IsChannelMessage)
            {
                obj.SetChannelMessage();
            }

            if (message48 != null && message48.Silent)
            {
                obj.SetSilent();
            }

            if (withMyScore)
            {
                obj.SetWithMyScore();
            }

            const string caption = "messages.forwardMessages";

            //Execute.ShowDebugMessage(string.Format(caption + " to_peer={0} from_peer={1} id={2} flags={3}", toPeer, fromPeer, string.Join(",", id), TLForwardMessages.ForwardMessagesFlagsString(obj.Flags)));
            //Execute.ShowDebugMessage(caption + string.Format("id={0} random_id={1} from_peer={2} to_peer={3}", obj.Id.FirstOrDefault(), obj.RandomIds.FirstOrDefault(), obj.FromPeer, obj.ToPeer));

            ForwardMessagesAsyncInternal(obj,
                result =>
                {
                    Execute.BeginOnUIThread(() =>
                    {
                        for (var i = 0; i < messages.Count; i++)
                        {
                            messages[i].Status = MessageStatus.Confirmed;
                            messages[i].Media.LastProgress = 0.0;
                            messages[i].Media.DownloadingProgress = 0.0;
                        }
                    });

                    var multiPts = result as IMultiPts;
                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption);
                    }
                    else
                    {
                        ProcessUpdates(result, messages);
                    }

                    callback.SafeInvoke(result);
                },
                () =>
                {

                },
                faultCallback.SafeInvoke);
        }

        public void GetChatsAsync(TLVector<TLInt> id, Action<TLChatsBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetChats { Id = id };

            SendInformativeMessage("messages.getChats", obj, callback, faultCallback);
        }

        public void GetFullChatAsync(TLInt chatId, Action<TLMessagesChatFull> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetFullChat { ChatId = chatId };

            SendInformativeMessage<TLMessagesChatFull>(
                "messages.getFullChat", obj,
                messagesChatFull =>
                {
                    _cacheService.SyncChat(messagesChatFull, result => callback.SafeInvoke(messagesChatFull));
                },
                faultCallback);
        }

        public void EditChatTitleAsync(TLInt chatId, TLString title, Action<TLUpdatesBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLEditChatTitle { ChatId = chatId, Title = title };

            const string caption = "messages.editChatTitle";
            SendInformativeMessage<TLUpdatesBase>(caption,
                obj,
                result =>
                {
                    var multiPts = result as IMultiPts;
                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption);
                    }
                    else
                    {
                        ProcessUpdates(result, null);
                    }

                    callback.SafeInvoke(result);
                },
                faultCallback);
        }

        public void EditChatPhotoAsync(TLInt chatId, TLInputChatPhotoBase photo, Action<TLUpdatesBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLEditChatPhoto { ChatId = chatId, Photo = photo };

            const string caption = "messages.editChatPhoto";
            SendInformativeMessage<TLUpdatesBase>(caption,
                obj,
                result =>
                {
                    var multiPts = result as IMultiPts;
                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption);
                    }
                    else
                    {
                        ProcessUpdates(result, null, true);
                    }

                    callback.SafeInvoke(result);
                },
                faultCallback);
        }

        public void AddChatUserAsync(TLInt chatId, TLInputUserBase userId, TLInt fwdLimit, Action<TLUpdatesBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLAddChatUser { ChatId = chatId, UserId = userId, FwdLimit = fwdLimit };

            const string caption = "messages.addChatUser";
            SendInformativeMessage<TLUpdatesBase>(caption,
                obj,
                result =>
                {
                    var multiPts = result as IMultiPts;
                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption);
                    }
                    else
                    {
                        ProcessUpdates(result, null);
                    }

                    callback.SafeInvoke(result);
                },
                faultCallback);
        }

        public void DeleteChatUserAsync(TLInt chatId, TLInputUserBase userId, Action<TLUpdatesBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLDeleteChatUser { ChatId = chatId, UserId = userId };

            const string caption = "messages.deleteChatUser";
            SendInformativeMessage<TLUpdatesBase>(caption,
                obj,
                result =>
                {
                    var multiPts = result as IMultiPts;
                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption);
                    }
                    else
                    {
                        ProcessUpdates(result, null);
                    }

                    callback.SafeInvoke(result);
                },
                faultCallback);
        }

        public void CreateChatAsync(TLVector<TLInputUserBase> users, TLString title, Action<TLUpdatesBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLCreateChat { Users = users, Title = title };

            const string caption = "messages.createChat";
            SendInformativeMessage<TLUpdatesBase>(caption,
                obj,
                result =>
                {
                    var multiPts = result as IMultiPts;
                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption);
                    }
                    else
                    {
                        ProcessUpdates(result, null);
                    }

                    callback.SafeInvoke(result);
                },
                faultCallback);
        }

        public void ExportChatInviteAsync(TLInt chatId, Action<TLExportedChatInvite> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLExportChatInvite { ChatId = chatId };

            SendInformativeMessage("messages.exportChatInvite", obj, callback, faultCallback);
        }

        public void CheckChatInviteAsync(TLString hash, Action<TLChatInviteBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLCheckChatInvite { Hash = hash };

            SendInformativeMessage<TLChatInviteBase>("messages.checkChatInvite", obj,
                result =>
                {
                    var chatInvite = result as TLChatInvite54;
                    if (chatInvite != null)
                    {
                        _cacheService.SyncUsers(chatInvite.Participants, participants =>
                        {
                            chatInvite.Participants = participants;
                            callback.SafeInvoke(result);
                        });
                    }
                    else
                    {
                        callback.SafeInvoke(result);
                    }
                }
                , faultCallback);
        }

        public void ImportChatInviteAsync(TLString hash, Action<TLUpdatesBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLImportChatInvite { Hash = hash };

            const string caption = "messages.importChatInvite";
            SendInformativeMessage<TLUpdatesBase>(caption,
                obj,
                result =>
                {
                    var updates = result as TLUpdates;
                    if (updates != null)
                    {
                        _cacheService.SyncUsersAndChats(updates.Users, updates.Chats, tuple => { });
                    }

                    var multiPts = result as IMultiPts;
                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption);
                    }
                    else
                    {
                        ProcessUpdates(result, null);
                    }

                    callback.SafeInvoke(result);
                },
                faultCallback);
        }

        public void SendActionsAsync(List<TLObject> actions, Action<TLObject, TLObject> callback, Action<TLRPCError> faultCallback = null)
        {
            var container = new TLContainer { Messages = new List<TLContainerTransportMessage>() };
            var historyItems = new List<HistoryItem>();
            for (var i = 0; i < actions.Count; i++)
            {
                var obj = actions[i];
                int sequenceNumber;
                TLLong messageId;
                lock (_activeTransportRoot)
                {
                    sequenceNumber = _activeTransport.SequenceNumber * 2 + 1;
                    _activeTransport.SequenceNumber++;
                    messageId = _activeTransport.GenerateMessageId(true);
                }

                var data = i > 0 ? new TLInvokeAfterMsg { MsgId = container.Messages[i - 1].MessageId, Object = obj } : obj;
                var invokeWithoutUpdates = new TLInvokeWithoutUpdates { Object = data };

                var transportMessage = new TLContainerTransportMessage
                {
                    MessageId = messageId,
                    SeqNo = new TLInt(sequenceNumber),
                    MessageData = invokeWithoutUpdates
                };

                var historyItem = new HistoryItem
                {
                    SendTime = DateTime.Now,
                    Caption = "messages.containerPart" + i,
                    Object = obj,
                    Message = transportMessage,
                    Callback = result => callback(obj, result),
                    AttemptFailed = null,
                    FaultCallback = faultCallback,
                    ClientTicksDelta = ClientTicksDelta,
                    Status = RequestStatus.Sent,
                };
                historyItems.Add(historyItem);

                container.Messages.Add(transportMessage);
            }


            lock (_historyRoot)
            {
                foreach (var historyItem in historyItems)
                {
                    _history[historyItem.Hash] = historyItem;
                }
            }
#if DEBUG
            NotifyOfPropertyChange(() => History);
#endif

            SendNonInformativeMessage<TLObject>("messages.container", container, result => callback(null, result), faultCallback);
        }

        public void ToggleChatAdminsAsync(TLInt chatId, TLBool enabled, Action<TLUpdatesBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLToggleChatAdmins { ChatId = chatId, Enabled = enabled };

            const string caption = "messages.toggleChatAdmins";
            SendInformativeMessage<TLUpdatesBase>(caption, obj,
                result =>
                {
                    var multiPts = result as IMultiPts;
                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption);
                    }
                    else
                    {
                        ProcessUpdates(result, null);
                    }

                    callback.SafeInvoke(result);
                },
                faultCallback);
        }

        public void EditChatAdminAsync(TLInt chatId, TLInputUserBase userId, TLBool isAdmin, Action<TLBool> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLEditChatAdmin { ChatId = chatId, UserId = userId, IsAdmin = isAdmin };

            SendInformativeMessage("messages.editChatAdmin", obj, callback, faultCallback);
        }

        public void DeactivateChatAsync(TLInt chatId, TLBool enabled, Action<TLUpdatesBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLDeactivateChat { ChatId = chatId, Enabled = enabled };

            const string caption = "messages.deactivateChat";
            SendInformativeMessage<TLUpdatesBase>(caption, obj,
                result =>
                {
                    var multiPts = result as IMultiPts;
                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption);
                    }
                    else
                    {
                        ProcessUpdates(result, null);
                    }

                    callback.SafeInvoke(result);
                },
                faultCallback);
        }

        public void HideReportSpamAsync(TLInputPeerBase peer, Action<TLBool> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLHideReportSpam { Peer = peer };

            const string caption = "messages.hideReportSpam";
            SendInformativeMessage<TLBool>(caption, obj, callback.SafeInvoke, faultCallback);
        }

        public void GetPeerSettingsAsync(TLInputPeerBase peer, Action<TLPeerSettings> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetPeerSettings { Peer = peer };

            const string caption = "messages.getPeerSettings";
            SendInformativeMessage<TLPeerSettings>(caption, obj, callback.SafeInvoke, faultCallback);
        }

        public void MigrateChatAsync(TLInt chatId, Action<TLUpdatesBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLMigrateChat { ChatId = chatId };

            const string caption = "messages.migrateChat";
            SendInformativeMessage<TLUpdatesBase>(caption, obj,
                result =>
                {
                    var multiPts = result as IMultiPts;
                    if (multiPts != null)
                    {
                        _updatesService.SetState(multiPts, caption);
                    }
                    else
                    {
                        ProcessUpdates(result, null);
                    }

                    callback.SafeInvoke(result);
                },
                faultCallback);
        }

        public int SendingMessages
        {
            get
            {
                var result = 0;
                lock (_historyRoot)
                {
                    foreach (var historyItem in _history.Values)
                    {
                        if (historyItem.Caption.StartsWith("messages.containerPart"))
                        {
                            result++;
                            break;
                        }
                    }
                }

                return result;
            }
        }
    }
}
