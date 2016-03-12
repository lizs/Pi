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
using socket4net;

namespace Pi.Framework
{
    public class LauncherArg<T> : LauncherArg where T : LauncherConfig
    {
        public T Config { get; private set; }

        public LauncherArg(T cfg, ILog logger, bool passiveLogicServiceEnabled = false)
            : base(logger, null, passiveLogicServiceEnabled)
        {
            Config = cfg;
        }
    }

    public class Launcher<T> : Launcher where T : LauncherConfig
    {
        protected readonly Mgr<Obj> Jobs = New<Mgr<Obj>>(ObjArg.Empty, false);
        public T Config { get; private set; }
        public Guid Id { get; private set; }

        protected override void OnInit(ObjArg arg)
        {
            base.OnInit(arg);
            var more = arg.As<LauncherArg<T>>();
            Config = more.Config;
            Id = more.Id;

            SpawnJobs();
        }

        protected override void OnDestroy()
        {
            GlobalVarPool.Ins.LogicService.Perform(() => Jobs.Destroy());
            base.OnDestroy();
        }

        protected override void OnStart()
        {
            base.OnStart();
            GlobalVarPool.Ins.LogicService.Perform(() => Jobs.Start());
        }

        protected virtual void SpawnJobs()
        {
        }
    }
}
