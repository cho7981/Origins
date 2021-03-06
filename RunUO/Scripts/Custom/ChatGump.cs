
//////////////////////////////////////////////////////////////////////
// Automatically generated by Bradley's GumpStudio and roadmaster's 
// exporter.dll,  Special thanks goes to Daegon whose work the exporter
// was based off of, and Shadow wolf for his Template Idea.
//////////////////////////////////////////////////////////////////////
using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using System.Collections;
using Server.Targeting;
using System.Collections.Generic;

namespace Server.Gumps
{
    public class ChatGump : Gump
    {
        private Mobile m_Caller;
        private ChatSystem m_System;

        public static void Initialize()
        {
            CommandSystem.Register("Chat", AccessLevel.Player, new CommandEventHandler(Chat_OnCommand));
            CommandSystem.Register("ChatSquelch", AccessLevel.Counselor, new CommandEventHandler(Squelch_OnCommand));
        }

        [Usage("ChatSquelch")]
        [Description("Squelches a player in the chat system gump.")]
        public static void Squelch_OnCommand(CommandEventArgs e)
        {
            Mobile caller = e.Mobile;
            ChatSystem system = null;

            foreach (Item item in World.Items.Values)
            {
                if (item is ChatSystem)
                    system = item as ChatSystem;
            }

            caller.Target = new GetMobile(system);

        }

        private class GetMobile : Target
        {
            private ChatSystem m_System;

            public GetMobile(ChatSystem chat) : base(15, false, TargetFlags.None)
            {
                m_System = chat;
            }

            protected override void OnTarget(Mobile from, object targ)
            {
                if (targ is Mobile)
                    m_System.SquelchPlayer(((Mobile)targ));
            }
        }

        [Usage("Chat")]
        [Description("Makes a call to the chat system gump.")]
        public static void Chat_OnCommand(CommandEventArgs e)
        {
            Mobile caller = e.Mobile;
            ChatSystem system = null;
            string message = "";

            foreach (Item item in World.Items.Values)
            {
                if (item is ChatSystem)
                    system = item as ChatSystem;
            }

            if (system == null)
                system = new ChatSystem();

            if (e.Length >= 1)
            {
                foreach (string str in e.Arguments)
                {
                    message += str + " ";
                }

                if (!system.m_Players.ContainsKey(caller))
                {
                    caller.SendAsciiMessage("Usage: [chat");
                    return;
                }
                else
                {
                    foreach (Mobile m in system.m_Players.Keys)
                    {
                        m.SendAsciiMessage(0x49, String.Format("[{0}{1}]: {2}", (caller.AccessLevel > AccessLevel.Player ? "@" : ""), caller.Name, message));
                    }
                }
            }
            else
            {
                if (system.m_Players.ContainsKey(caller))
                {
                    caller.SendAsciiMessage("Usage: [chat <message>");
                    return;
                }
                else
                {
                    if (caller.HasGump(typeof(ChatGump)))
                        caller.CloseGump(typeof(ChatGump));

                    system.AddPlayer(caller);
                }
            }
        }

        public ChatGump(Mobile from, ChatSystem system) : base(50, 100)
        {
            m_Caller = from;
            m_System = system;

            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

            AddBackground(0, 0, 190, 330, 9200);
            AddButton(16, 269, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddLabel(40, 11, 0, @"UO: Origins Chat");

            string players = "";

            List<Mobile> toRemove = new List<Mobile>();

            foreach (Mobile m in m_System.m_Players.Keys)
            {
                if (m.Deleted || m == null)
                {
                    toRemove.Add(m);
                    continue;
                }

                bool visible = true;
                m_System.m_Players.TryGetValue(m,out visible);

                if (visible)
                    players += String.Format("{1} <BR>", players, (m == null ? "" : (m.AccessLevel > AccessLevel.Player ? "@" + m.Name : m.Name)));
                else if (from.AccessLevel > AccessLevel.Player)
                    players += String.Format("{1} <BR>", players, (m == null ? "" :  "#" + m.Name));
            }

            foreach (Mobile m in toRemove)
            {
                m_System.m_Players.Remove(m);
            }

            AddHtml(10, 50, 163, 188, players, (bool)true, (bool)true);

            AddLabel(10, 30, 0, @"Players Available");
            AddLabel(14, 244, 0, @"Type [chat <msg> to talk");


            bool isvisible = true;
            m_System.m_Players.TryGetValue(from, out isvisible);

            AddLabel(54, 270, 0, (isvisible ? @"Hide Name" : @"Show Name"));


            AddLabel(56, 298, 0, @"Quit Chat");
            AddButton(16, 296, 4017, 4019, 0, GumpButtonType.Reply, 0);

        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch(info.ButtonID)
            {
                case 0:
                {
                    from.SendAsciiMessage(0x49, "You can rejoin chat anytime by using the [chat command.");
                    m_System.RemovePlayer(from);
					break;
				}
                case 1:
                {
                    m_System.ToggleVisible(from);
                    m_System.UpdateGump();
                    break;
                }

            }
        }
    }
}