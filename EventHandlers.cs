using Synapse;
using Synapse.Api;
using Synapse.Api.Events.SynapseEventArguments;
using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace CommonUtilities
{
    public class EventHandlers : MonoBehaviour
    {
        List<Player> tablica;
        public static System.Random rnd = new System.Random();
        private bool firedOnce = false;
        private bool firedOnce2 = false;
        private bool isBlackoutActive = false;
        public static Timer NukeActivation = new Timer(PluginClass.Config.NukeActivationDelay * 1000);
        public static Timer CassieDelay = new Timer(PluginClass.Config.CassieDelay * 1000);
        public static Timer BlackoutActivation = new Timer((rnd.Next(PluginClass.Config.minBlackout, PluginClass.Config.maxBlackout) + PluginClass.Config.blackoutDelay ) * 1000);
        public static Timer Generator = new Timer(1000);
        public static Timer CheckIfEscaped = new Timer(500);

        public EventHandlers()
        {
            if(PluginClass.Config.isEnabled)
            {
                if(PluginClass.Config.isNuke)
                    SynapseController.Server.Events.Round.RoundStartEvent += NukeTimer;
                if (PluginClass.Config.isBlackout)
                {
                    SynapseController.Server.Events.Round.RoundStartEvent += BlackoutTimer;
                    SynapseController.Server.Events.Player.PlayerGeneratorInteractEvent += RestoreGenerators;
                }
                if (PluginClass.Config.isCoin)
                    SynapseController.Server.Events.Map.DoorInteractEvent += BlockDoors;
                if (PluginClass.Config.isTarget)
                    SynapseController.Server.Events.Scp.Scp096.Scp096AddTargetEvent += ShowTargets;
                if (PluginClass.Config.canEscape)
                    SynapseController.Server.Events.Round.RoundStartEvent += ManageEscapes;
            }
        }

        private void ManageEscapes()
        {
            CheckIfEscaped.Elapsed += CheckIfEscaped_Elapsed;
            CheckIfEscaped.Enabled = true;
            CheckIfEscaped.AutoReset = true;
            CheckIfEscaped.Start();
        }

        private void CheckIfEscaped_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach(Player player in tablica)
            {
                    float x = player.Position.x;
                    float z = player.Position.z;
                    if (x >= 168.4 && x <= 171.9 && z >= 23.0 && z <= 25.0)
                        player.RoleID = 13;
            }
        }

        private void ShowTargets(Scp096AddTargetEventArgument ev)
        {
            if(ev.RageState != PlayableScps.Scp096PlayerState.Calming)
                ev.Player.GiveTextHint("You are target of SCP-096, run!", 5);
        }

        private void BlockDoors(DoorInteractEventArgs ev)
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

        private void RestoreGenerators(PlayerGeneratorInteractEventArgs ev)
        {
            Generator.Enabled = true;
            Generator.AutoReset = true;
            Generator.Elapsed += RestoreGenerators_Elapsed;
            Generator.Start();
        }

        private void RestoreGenerators_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Server.Get.Players.ForEach(p => Synapse.Api.Logger.Get.Send(p.Position, System.ConsoleColor.Red));

            if (Map.Get.HeavyController.ActiveGenerators == 3)
            {
                BlackoutActivation.Stop();
                Generator.Stop();
                Generator.AutoReset = false;
                Generator.Enabled = false;
                isBlackoutActive = false;
                if(!firedOnce2)
                {
                    Map.Get.Cassie(PluginClass.Config.cassieBlackoutFix);
                    firedOnce2 = true;
                }
                
            }
        }

        private void BlackoutTimer()
        {
            foreach(Player player in Server.Get.Players)
            {
                if (player.RoleID == 15)
                    tablica.Add(player);
            }
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
                isBlackoutActive = true;
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
            NukeActivation.Stop();
            NukeActivation.Enabled = false;
        }

        private static void CassieDelay_Elapsed(object sender, ElapsedEventArgs e)
        {
            Map.Get.Nuke.StartDetonation();
            if(!PluginClass.Config.canBeDisabled)
                Map.Get.Nuke.InsidePanel.Locked = true;
            else
                Map.Get.Nuke.InsidePanel.Locked = false;

            Server.Get.Players.ForEach(p => p.SendBroadcast(5, "Automatic Nuke has been activated", true));
            CassieDelay.Stop();
            CassieDelay.Enabled = false;
        }
    }
}
