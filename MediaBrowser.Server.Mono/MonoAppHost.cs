﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Emby.Server.Core;
using Emby.Server.Core.Data;
using Emby.Server.Core.FFMpeg;
using MediaBrowser.IsoMounter;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.System;
using MediaBrowser.Server.Mono.Native;

namespace MediaBrowser.Server.Mono
{
    public class MonoAppHost : ApplicationHost
    {
        public MonoAppHost(ServerApplicationPaths applicationPaths, ILogManager logManager, StartupOptions options, IFileSystem fileSystem, IPowerManagement powerManagement, string releaseAssetFilename, IEnvironmentInfo environmentInfo, MediaBrowser.Controller.Drawing.IImageEncoder imageEncoder, ISystemEvents systemEvents, IMemoryStreamFactory memoryStreamFactory, MediaBrowser.Common.Net.INetworkManager networkManager, Action<string, string> certificateGenerator, Func<string> defaultUsernameFactory) : base(applicationPaths, logManager, options, fileSystem, powerManagement, releaseAssetFilename, environmentInfo, imageEncoder, systemEvents, memoryStreamFactory, networkManager, certificateGenerator, defaultUsernameFactory)
        {
        }

        public override bool CanSelfRestart
        {
            get
            {
                // A restart script must be provided
                return StartupOptions.ContainsOption("-restartpath");
            }
        }

        public override bool CanSelfUpdate
        {
            get
            {
                return false;
            }
        }

        protected override FFMpegInstallInfo GetFfmpegInstallInfo()
        {
            var info = new FFMpegInstallInfo();

            // Windows builds: http://ffmpeg.zeranoe.com/builds/
            // Linux builds: http://johnvansickle.com/ffmpeg/
            // OS X builds: http://ffmpegmac.net/
            // OS X x64: http://www.evermeet.cx/ffmpeg/

            var environment = (MonoEnvironmentInfo) EnvironmentInfo;

            if (environment.IsBsd)
            {

            }
            else if (environment.OperatingSystem == Model.System.OperatingSystem.Linux)
            {
                info.ArchiveType = "7z";
                info.Version = "20160215";
            }

            // No version available - user requirement
            info.DownloadUrls = new string[] { };

            return info;
        }

        protected override void RestartInternal()
        {
            MainClass.Restart(StartupOptions);
        }

        protected override List<Assembly> GetAssembliesWithPartsInternal()
        {
            var list = new List<Assembly>();

            list.Add(GetType().Assembly);
            list.AddRange(GetLinuxAssemblies());

            return list;
        }

        private IEnumerable<Assembly> GetLinuxAssemblies()
        {
            var list = new List<Assembly>();

            list.Add(typeof(LinuxIsoManager).Assembly);

            return list;
        }

        protected override void ShutdownInternal()
        {
            MainClass.Shutdown();
        }

        protected override void AuthorizeServer(int udpPort, int httpServerPort, int httpsServerPort, string applicationPath, string tempDirectory)
        {
            throw new NotImplementedException();
        }

        protected override IDbConnector GetDbConnector()
        {
            return new DbConnector(Logger);
        }

        protected override void ConfigureAutoRunInternal(bool autorun)
        {
            throw new NotImplementedException();
        }

        public override void LaunchUrl(string url)
        {
            throw new NotImplementedException();
        }

        protected override void EnableLoopbackInternal(string appName)
        {
        }

        public override bool SupportsRunningAsService
        {
            get
            {
                return false;
            }
        }

        public override bool SupportsAutoRunAtStartup
        {
            get { return false; }
        }

        public override bool IsRunningAsService
        {
            get
            {
                return false;
            }
        }
    }
}