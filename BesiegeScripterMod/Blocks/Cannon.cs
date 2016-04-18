﻿using System.Reflection;

namespace LenchScripterMod.Blocks
{
    public class Cannon : Block
    {
        private CanonBlock cb;
        private ArrowTurret turret;
        private ShrapnelCannon shrapnel;

        internal Cannon(BlockBehaviour bb) : base(bb)
        {
            cb = bb.GetComponent<CanonBlock>();
            var turret_field = cb.GetType().GetField("turret", BindingFlags.NonPublic | BindingFlags.Instance);
            var shrapnel_field = cb.GetType().GetField("shrapnel", BindingFlags.NonPublic | BindingFlags.Instance);
            turret = turret_field.GetValue(cb) as ArrowTurret;
            shrapnel = shrapnel_field.GetValue(cb) as ShrapnelCannon;
        }

        public override void action(string actionName)
        {
            actionName = actionName.ToUpper();
            if (actionName == "SHOOT")
            {
                Shoot();
                return;
            }
        }

        public void Shoot()
        {
            if (turret)
                cb.StartCoroutine_Auto(turret.Shoot());
            if (shrapnel)
                cb.StartCoroutine_Auto(shrapnel.Shoot());
        }

        internal static bool isCannon(BlockBehaviour bb)
        {
            return bb.GetComponent<CanonBlock>() != null;
        }
    }
}
