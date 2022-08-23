using Synapse;
using Synapse.Api;
using System.Timers;

namespace CommonUtilities
{
    public class EventHandlers
    {
        public static System.Random rnd = new System.Random();
        private bool firedOnce = false;
        public static Timer NukeActivation = new Timer(PluginClass.Config.NukeActivationDelay * 1000);
        public static Timer CassieDelay = new Timer(PluginClass.Config.CassieDelay * 1000);
        public static Timer BlackoutActivation = new Timer((rnd.Next(PluginClass.Config.minBlackout, PluginClass.Config.maxBlackout) + PluginClass.Config.blackoutDelay ) * 1000);

        public EventHandlers()
        {
            if(PluginClass.Config.isEnabled)
            {
                if(PluginClass.Config.isNuke)
                    SynapseController.Server.Events.Round.RoundStartEvent += NukeTimer;
                if(PluginClass.Config.isBlackout)
                    SynapseController.Server.Events.Round.RoundStartEvent += BlackoutTimer;
                if (PluginClass.Config.isCoin)
                    SynapseController.Server.Events.Map.DoorInteractEvent += BlockDoors;
            }
        }

        private void BlockDoors(Synapse.Api.Events.SynapseEventArguments.DoorInteractEventArgs ev)
        {
            Door door;
            Timer blockTime = new Timer();
            blockTime.Enabled = true;
            blockTime.Interval = PluginClass.Config.coinBlockTime * 1000;
            blockTime.AutoReset = false;
            if (ev.Door.IsBreakable)
                if (ev.Player.ItemInHand.ID == 35 && ev.Door.Locked == false)
                {
                    door = ev.Door;
                    blockTime.Elapsed += (sender, e) => blockTime_Elapsed(sender, e, door);
                    ev.Player.ItemInHand.Destroy();
                    if (ev.Door.Open)
                    {
                        ev.Door.Open = true;
                        blockTime.Start();
                        ev.Door.Locked = true;
                    }
                    else if(!ev.Door.Open)
                    {
                        ev.Door.Open = true;
                        blockTime.Start();
                        ev.Door.Locked= true;
                    }
                }
        }

        public static void blockTime_Elapsed(object sender, ElapsedEventArgs e, Door door)
        {
            door.Locked = false;
        }

        private void BlackoutTimer()
        {
            BlackoutActivation.Elapsed += BlackoutTimer_Elapsed;
            BlackoutActivation.Enabled = true;
            BlackoutActivation.AutoReset = true;
            BlackoutActivation.Start();
        }

        private void BlackoutTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!firedOnce)
            {
                firedOnce = true;
                Map.Get.Cassie(PluginClass.Config.CassieBlackout);
            }
            
            BlackoutActivation.Interval = rnd.Next(PluginClass.Config.minBlackout, PluginClass.Config.maxBlackout) * 1000;

            foreach (var room in Map.Get.Rooms)
            {
                if (room.Zone == Synapse.Api.Enum.ZoneType.HCZ || room.Zone == Synapse.Api.Enum.ZoneType.LCZ)
                {
                    if (rnd.Next(1, 12) % 3 == 0)
                        room.LightsOut(PluginClass.Config.flickerDuration);
                    else if (rnd.Next(1, 10) % 2 == 0)
                    {
                        room.Doors.ForEach(d => d.Open = false);
                        room.LightsOut(rnd.Next(PluginClass.Config.minDurationBlackout, PluginClass.Config.maxDurationBlackout));
                    }
                }
            }
        }

        private void NukeTimer()
        {
            NukeActivation.Elapsed += NukeActivation_Elapsed;
            NukeActivation.Enabled = true;
            NukeActivation.AutoReset = false;
            NukeActivation.Start();
        }

        private static void NukeActivation_Elapsed(object sender, ElapsedEventArgs e)
        {
            Map.Get.Cassie(PluginClass.Config.CassieNukeMessage, false, true);
            CassieDelay.Elapsed += CassieDelay_Elapsed;
            CassieDelay.Enabled = true;
            CassieDelay.AutoReset = false;
            CassieDelay.Start();
        }

        private static void CassieDelay_Elapsed(object sender, ElapsedEventArgs e)
        {
            Map.Get.Nuke.StartDetonation();
            Map.Get.Nuke.InsidePanel.Locked = true;
            Server.Get.Players.ForEach(p => p.SendBroadcast(5, "Automatic Nuke has been activated", true));
        }
    }
}
