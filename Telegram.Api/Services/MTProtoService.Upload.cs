﻿// 
// This is the source code of Telegram for Windows Phone v. 3.x.x.
// It is licensed under GNU GPL v. 2 or later.
// You should have received a copy of the license in this archive (see LICENSE).
// 
// Copyright Evgeny Nadymov, 2013-present.
// 
using System;
using System.Diagnostics;
using Telegram.Api.TL;
using Telegram.Api.TL.Functions.Upload;

namespace Telegram.Api.Services
{
	public partial class MTProtoService
	{
        public void SaveFilePartAsync(TLLong fileId, TLInt filePart, TLString bytes, Action<TLBool> callback, Action<TLRPCError> faultCallback = null)
        {
            var filePartValue = filePart.Value;
            var bytesLength = bytes.Data.Length;

            var obj = new TLSaveFilePart{ FileId = fileId, FilePart = filePart, Bytes = bytes };

            SendInformativeMessage("upload.saveFilePart" + " " + filePart.Value, obj, callback, faultCallback);
        }

        public void SaveBigFilePartAsync(TLLong fileId, TLInt filePart, TLInt fileTotalParts, TLString bytes, Action<TLBool> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLSaveBigFilePart { FileId = fileId, FilePart = filePart, FileTotalParts = fileTotalParts, Bytes = bytes };
            Debug.WriteLine(string.Format("upload.saveBigFilePart file_id={0} file_part={1} file_total_parts={2} bytes={3}", fileId, filePart, fileTotalParts, bytes.Data.Length));
            SendInformativeMessage("upload.saveBigFilePart " + filePart + " " + fileTotalParts, obj, callback, faultCallback);
        }

        public void GetFileAsync(TLInputFileLocationBase location, TLInt offset, TLInt limit, Action<TLFileBase> callback, Action<TLRPCError> faultCallback = null)
        {
            var obj = new TLGetFile { Location = location, Offset = offset, Limit = limit };

            SendInformativeMessage("upload.getFile", obj, callback, faultCallback);
        }
	}
}
