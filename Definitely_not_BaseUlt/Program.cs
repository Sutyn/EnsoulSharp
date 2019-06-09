#region Heander 
using System;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using SharpDX;
#endregion

//Credits: Chewy and Moon! Thanks <3

namespace Definitely_not_BaseUlt
{
    class Program
    { 
        //Ashe, Draven, Ezreal and Jinx.
        private static readonly string[] BaseUltNames =
            {
                "EzrealTrueshotBarrage", "EnchantedCrystalArrow",
                "DravenDoubleShotMissile", "JinxR"
            };

        private static int lastNotification;
        private static int recallFinishTime;

        private static AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }
        static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad()
        {
            Game.OnUpdate += GameOnOnUpdate;
            AIBaseClient.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
            Chat.PrintChat("<font color=\"#7e62cc\">Script: </font>Definitely Not BaseUlt<font color=\"#000000\"> by Sutyn</font> - <font color=\"#00BFFF\">Loaded</font>");

        }

        private static void Obj_AI_Hero_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (!sender.IsValid)
            {
                return;
            }

            var hero = (AIHeroClient)sender;

            if (!hero.IsMe || args.SData.Name != "recall")
            {
                return;
            }

            recallFinishTime = Environment.TickCount + GetRecallTime(hero);
        }

        private static void GameOnOnUpdate(EventArgs args)
        {
            if (!Player.IsRecalling())
            {
                return;
            }

            foreach (var missile in ObjectManager.Get<MissileClient>().Where(x => BaseUltNames.Any(y => y.Equals(x.SData.Name)) && x.SpellCaster.IsEnemy))
            {
                var spellCaster = missile.SpellCaster as AIHeroClient;

                if (spellCaster.GetSpellDamage(Player, SpellSlot.R, DamageStage.Default) < Player.Health)
                {
                    continue;
                }

                // Not a baseult
                if (!PositionIsInFountain(missile.EndPosition))
                {
                    continue;
                }

                var objSpawnPoint = ObjectManager.Get<Obj_SpawnPoint>().FirstOrDefault(x => x.IsAlly);
                if (objSpawnPoint == null)
                {
                    continue;
                }

                var timeToFountain = missile.Position.Distance(objSpawnPoint.Position) / missile.SData.MissileSpeed* 1000;

                if (recallFinishTime < timeToFountain)
                {
                    continue;
                }

                Player.IssueOrder(GameObjectOrder.MoveTo, Player.Position);

                if (Environment.TickCount - lastNotification > 1000)
                {
                    lastNotification = Environment.TickCount;

                    break;
                }
            }
        }
        private static bool PositionIsInFountain(Vector3 position)
        {
            float fountainRange = 562500;
            var map = GameMapId.SummonersRift;

            if (map != null)
            {
                fountainRange = 1102500;
            }

            return ObjectManager.Get<Obj_SpawnPoint>()
                    .Any(x => x.Team == Player.Team && position.Distance(x.Position) < fountainRange);
        }

        public static int GetRecallTime(AIHeroClient obj)
        {
            return GetRecallTime(obj.Spellbook.GetSpell(SpellSlot.Recall).Name);
        }

        public static int GetRecallTime(string recallName)
        {
            var duration = 0;

            switch (recallName.ToLower())
            {
                case "recall":
                    duration = 8000;
                    break;
                case "recallimproved":
                    duration = 7000;
                    break;
                case "odinrecall":
                    duration = 4500;
                    break;
                case "odinrecallimproved":
                    duration = 4000;
                    break;
                case "superrecall":
                    duration = 4000;
                    break;
                case "superrecallimproved":
                    duration = 4000;
                    break;
            }
            return duration;
        }
    }
}
