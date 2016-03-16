﻿#region MIT
//  /*The MIT License (MIT)
// 
//  Copyright 2016 lizs lizs4ever@163.com
//  
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//   * */
#endregion

using System;
using System.Linq;
using socket4net;
#if NET45
using System.Threading.Tasks;
#endif

namespace Pi.Framework
{
    public abstract class ClientSession : DispatchableSession
    {
        protected override sealed Player GetPlayer(long playerId)
        {
            return playerId == 0 ? PlayerMgr.Ins.FirstOrDefault() : PlayerMgr.Ins.Get(playerId);
        }
    }

    public abstract class ServerSession : DispatchableSession
    {
        protected override sealed Player GetPlayer(long playerId)
        {
            return PlayerMgr.Ins.Get(playerId);
        }
    }

    public abstract class DispatchableSession : socket4net.DispatchableSession
    {
        protected override void OnInit(ObjArg arg)
        {
            base.OnInit(arg);

            ReceiveBufSize = 10*1024;
            PackageMaxSize = 40*1024;

            DataParser = PiSerializer.Deserialize<DataProtocol>;
        }

        private static DataProtocol Pack(byte targetServer, long playerId, short ops, byte[] data, long objId,
            short cpId)
        {
            return new DataProtocol
            {
                ComponentId = cpId,
                Data = data,
                ObjId = objId,
                Ops = ops,
                PlayerId = playerId,
                TargetNode = targetServer,
            };
        }

        private static DataProtocol Pack<T>(byte targetServer, long playerId, short ops, T proto, long objId,
            short cpId)
        {
            return new DataProtocol
            {
                ComponentId = cpId,
                Data = PiSerializer.Serialize(proto),
                ObjId = objId,
                Ops = ops,
                PlayerId = playerId,
                TargetNode = targetServer,
            };
        }

        protected abstract Player GetPlayer(long playerId);

        protected virtual Task<NetResult> OnNonPlayerRequest(DataProtocol rq)
        {
            return Task.FromResult(NetResult.Failure);
        }

        protected virtual Task<bool> OnNonPlayerPush(DataProtocol rp)
        {
            return Task.FromResult(false);
        }
        
        public async override Task<NetResult> HandleRequest(IDataProtocol rq)
        {
            var proto = rq as DataProtocol;
            if (proto.Ops < 0)
            {
                return await OnNonPlayerRequest(proto);
            }

            var player = GetPlayer(proto.PlayerId);
            if (player == null)
                return false;

            using (new Flusher(player as IFlushable))
            {
                var entity = proto.ObjId != 0 ? player.Es.Get(proto.ObjId) : player;
                if (entity == null) return NetResult.Failure;

                if (proto.ComponentId == 0)
                    return await entity.OnRequest(proto.Ops, proto.Data);

                var cp = entity.GetComponent(proto.ComponentId);
                return cp == null
                    ? NetResult.Failure
                    : await cp.OnRequest(proto.Ops, proto.Data);
            }
        }

        public async override Task<bool> HandlePush(IDataProtocol ps)
        {
            var proto = ps as DataProtocol;
            if (proto.Ops < 0)
            {
                return await OnNonPlayerPush(proto);
            }

            var player = GetPlayer(proto.PlayerId);
            if (player == null)
                return false;

            using (new Flusher(player as IFlushable))
            {
                var entity = proto.ObjId != 0 ? player.Es.Get(proto.ObjId) : player;
                if (entity == null) return false;

                if (proto.ComponentId == 0)
                    return await entity.OnPush(proto.Ops, proto.Data);

                var cp = entity.GetComponent(proto.ComponentId);
                return cp != null && await cp.OnPush(proto.Ops, proto.Data);
            }
        }


        #region rpc

#if NET45
        public Task<NetResult> RequestAsync<T>(byte targetServer, long playerId, short ops, T proto, long objId,
            short cpId)
        {
            return RequestAsync(Pack(targetServer, playerId, ops, proto, objId, cpId));
        }

        public Task<NetResult> RequestAsync(byte targetServer, long playerId, short ops, byte[] data, long objId,
            short cpId)
        {
            return RequestAsync(Pack(targetServer, playerId, ops, data, objId, cpId));
        }

        public Task<NetResult> RequestWithoutDataAsync(byte targetServer, long playerId, short ops, long objId,
            short cpId)
        {
            return RequestAsync(Pack(targetServer, playerId, ops, null, objId, cpId));
        }
#endif

        public void RequestAsync<T>(byte targetServer, long playerId, short ops, T proto, long objId, short cpId,
            Action<bool, byte[]> cb)
        {
            RequestAsync(Pack(targetServer, playerId, ops, proto, objId, cpId), cb);
        }

        public void RequestAsync(byte targetServer, long playerId, short ops, byte[] data, long objId, short cpId,
            Action<bool, byte[]> cb)
        {
            RequestAsync(Pack(targetServer, playerId, ops, data, objId, cpId), cb);
        }

        public void RequestWithoutDataAsync(byte targetServer, long playerId, short ops, long objId, short cpId,
            Action<bool, byte[]> cb)
        {
            RequestAsync(Pack(targetServer, playerId, ops, null, objId, cpId), cb);
        }

        public void Push(byte targetServer, long playerId, short ops, byte[] data, long objId, short cpId)
        {
            Push(Pack(targetServer, playerId, ops, data, objId, cpId));
        }

        public void PushWithoutData(byte targetServer, long playerId, short ops, long objId, short cpId)
        {
            Push(Pack(targetServer, playerId, ops, null, objId, cpId));
        }

        public void Push<T>(byte targetServer, long playerId, short ops, T proto, long objId, short cpId)
        {
            Push(Pack(targetServer, playerId, ops, proto, objId, cpId));
        }
        #endregion
    }
}