#region Header
using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;

using Color = System.Drawing.Color;
#endregion

namespace SutynLeona
{
    class Program
    {
        public class _comboleona
        {
            public static readonly MenuBool usQ = new MenuBool("useq", "Use Q");
            public static readonly MenuBool usW = new MenuBool("usew", "Use W");
            public static readonly MenuBool usE = new MenuBool("usee", "Use E");
            public static readonly MenuBool usR = new MenuBool("user", "Use R");
            public static readonly MenuBool exauts = new MenuBool("useex", "Use Exhaust", true);
        }

        public class _Misc
        {
            public static readonly MenuBool InterruptSpells = new MenuBool("Inter", "Interrupt Spells");
            public static readonly MenuBool eqinter = new MenuBool("EQInterrupt", "Use E and Q in Target");
            public static readonly MenuBool rinter = new MenuBool("RInterrupt", "Use R in Target");
            public static readonly MenuBool qgabs = new MenuBool("Qgab", "^- Auto-Q on GapClosers");
        }

        public class Draws
        {
            public static readonly MenuBool dq = new MenuBool("DQ", "Draw Q Range");
            public static readonly MenuBool dw = new MenuBool("DW", "Draw W Range");
            public static readonly MenuBool de = new MenuBool("DE", "Draw E Range");
            public static readonly MenuBool dr = new MenuBool("DR", "Draw R Range");
        }

        public const string ChampionName = "Leona";

        public static Menu Config;
        public static Spell Q, W, E, R;
        public static SpellSlot ExhaustSlot;
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnLoadGame;
        }

        private static void OnLoadGame()
        {
            if (_Player.CharacterName != ChampionName) return;
            Chat.PrintChat("<font color=\"#7e62cc\">Script: </font>Sutyn Leona<font color=\"#000000\"> by Sutyn</font> - <font color=\"#00BFFF\">Loaded</font>");

            #region Spells
            Q = new Spell(SpellSlot.Q, 120f);
            W = new Spell(SpellSlot.W, 450f);
            E = new Spell(SpellSlot.E, 875f);
            E.SetSkillshot(0.25f, 100f, 2000f, false, SkillshotType.Line);

            R = new Spell(SpellSlot.R, 1200f);
            R.SetSkillshot(1f, 300f, float.MaxValue, false, SkillshotType.Circle);

            ExhaustSlot = _Player.GetSpellSlot("SummonerExhaust");
            #endregion

            #region Menu
            var Config = new Menu("sutyn.leona", "Sutyn.Leon", true);
            //Combo
            var _combe = new Menu("combo", "Combo Settings");
            _combe.Add(_comboleona.usQ);
            _combe.Add(_comboleona.usW);
            _combe.Add(_comboleona.usE);
            _combe.Add(_comboleona.usR);
            _combe.Add(_comboleona.exauts);
            Config.Add(_combe);
            //Misc
            var _lmisc = new Menu("misc", "Misc Settings");
            _lmisc.Add(_Misc.eqinter);
            _lmisc.Add(_Misc.rinter);
            _lmisc.Add(_Misc.qgabs);
            Config.Add(_lmisc);
            //Draw
            var _drawleona = new Menu("draw", "Drawing Spells");
            _drawleona.Add(Draws.dq);
            _drawleona.Add(Draws.dw);
            _drawleona.Add(Draws.de);
            _drawleona.Add(Draws.dr);
            Config.Add(_drawleona);
            Config.Attach();
            #endregion

            #region Definit
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter.OnInterrupterSpell += OnInterruptableTarget;
            Gapcloser.OnGapcloser -= OnEnemyGapcloser;
            #endregion
        }

        private static void OnGameUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    break;
            }
        }

        private static void Combo()
        {
            var _qmenu = _comboleona.usQ.Enabled;
            var _wmenu = _comboleona.usW.Enabled;
            var _emenu = _comboleona.usE.Enabled;
            var _rmenu = _comboleona.usR.Enabled;

            var target = TargetSelector.GetTarget(E.Range);

            if (target != null)
            {
                if (_qmenu && Q.IsReady())
                {
                    if (_Player.Distance(target) < Q.Range)
                    {
                        Q.Cast();
                        _Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                    }
                }
                if (_emenu && E.IsReady())
                {
                    if (_Player.Distance(target) < E.Range)
                    {
                        E.CastIfHitchanceEquals(target, HitChance.High, true);
                    }
                }

                if (_wmenu && W.IsReady())
                {
                    if (_Player.Distance(target) < W.Range)
                        W.Cast();
                }

                if (_comboleona.exauts.Enabled)
                {
                    if (ExhaustSlot != SpellSlot.Unknown && _Player.Spellbook.CanUseSpell(ExhaustSlot) == SpellState.Ready)
                    {
                        _Player.Spellbook.CastSpell(ExhaustSlot, target);
                    }
                }

                if (_wmenu && W.IsReady())
                {
                    if (_Player.Distance(target) < W.Range)
                        W.Cast();
                }

                if (_rmenu && R.IsReady())
                {
                    if (_Player.Distance(target) > E.Range)
                    {
                        if (_Player.Distance(target) < R.Range)
                        {
                            R.CastIfHitchanceEquals(target, HitChance.Immobile);
                        }
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (_Player.IsDead || MenuGUI.IsChatOpen)
            {
                return;
            }
            if (Draws.dq.Enabled && Q.IsReady())
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, Q.Range, Color.Red);
            }

            if (Draws.dw.Enabled && W.IsReady())
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, W.Range, Color.Crimson);
            }
            if (Draws.de.Enabled && E.IsReady())
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, E.Range, Color.Crimson);
            }

            if (Draws.dr.Enabled && R.IsReady())
            {
                Render.Circle.DrawCircle(GameObjects.Player.Position, R.Range, Color.Crimson);
            }
        }

        private static void OnInterruptableTarget(AIHeroClient sender, Interrupter.InterruptSpellArgs args)
        {
            var intermenu = _Misc.InterruptSpells.Enabled;
            var eqintermenu = _Misc.eqinter.Enabled;
            var rintermenu = _Misc.rinter.Enabled;

            if (intermenu)
            {
                if (ObjectManager.Player.Distance(sender) < Q.Range && Q.IsReady())
                {
                    Q.Cast();
                    ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, sender);
                }

                if (ObjectManager.Player.Distance(sender) < E.Range && E.IsReady() && Q.IsReady())
                {
                    if (eqintermenu)
                    {
                        Q.Cast();
                        E.CastIfHitchanceEquals(sender, HitChance.High, true);
                        ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, sender);
                    }
                }
                else if (ObjectManager.Player.Distance(sender) > E.Range)
                {
                    if (rintermenu)
                    {
                        if (ObjectManager.Player.Distance(sender) < R.Range && R.IsReady())
                            R.CastIfHitchanceEquals(sender, HitChance.High, true);
                    }
                }
            }
        }

        private static void OnEnemyGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {
            if (_Misc.qgabs.Enabled)
            {
                if (_Player.Distance(sender) < _Player.AttackRange)
                {
                    Q.Cast();
                    ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, sender);
                }
            }
        }
    }
}
