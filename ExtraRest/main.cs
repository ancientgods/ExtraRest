﻿using System;
using Terraria;
using TShockAPI;
using TerrariaApi.Server;
using System.Reflection;
using Rests;
using HttpServer;
using System.Collections.Generic;
using TShockAPI.DB;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace extrarest
{
    [ApiVersion(1, 20)]
    public class ExtraRest : TerrariaPlugin
    {
        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }
        public override string Author
        {
            get { return "Ancientgods"; }
        }
        public override string Name
        {
            get { return "ExtraRest"; }
        }

        public override string Description
        {
            get { return "Extra Rest Api commands"; }
        }

        public override void Initialize()
        {
            TShock.RestApi.Register(new RestCommand("/staff", Staff));
            TShock.RestApi.Register(new RestCommand("/whitelist", WhiteList));
        }

        public static RestObject WhiteList(RestRequestArgs args)
        {
            string[] whitelist = new[] { "" };
            try
            {
                using (StreamReader sr = new StreamReader(File.Open(Path.Combine(TShock.SavePath, "whitelist.txt"), FileMode.Open)))
                {
                    whitelist = sr.ReadToEnd().Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                }

            }
            catch
            {
                whitelist = new[] { "Whitelist not found!" };
            }
            return new RestObject()
            {
                    { "WhiteList", whitelist}
            };
        }

        public static RestObject Staff(RestRequestArgs args)
        {
            GroupManager groupmanager = new GroupManager(TShock.DB);
            UserManager usermanager = new UserManager(TShock.DB);

            List<string> groups = groupmanager.groups.FindAll(g => g.HasPermission("tshock.admin.kick")).Select(g => g.Name).ToList();

            List<User> users = usermanager.GetUsers().Where(u => groups.Contains(u.Group)).ToList();
            Dictionary<string, List<string>> RestGroups = new Dictionary<string, List<string>>();

            for (int i = 0; i < groups.Count; i++)
                RestGroups[groups[i]] = users.Where(u => u.Group == groups[i]).Select(u => u.Name).ToList();

            return new RestObject()
            {
                { "StaffList",  RestGroups },
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        public ExtraRest(Main game)
            : base(game)
        {
            Order = 1;
        }
    }
}