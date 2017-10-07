﻿namespace FacialStuff.Animator
{
    using System.Collections.Generic;

    using JetBrains.Annotations;

    using Verse;
    using Verse.AI;

    public class PawnHeadRotator
    {
        public PawnHeadRotator(Pawn p)
        {
            this.pawn = p;
        }

        public readonly SimpleCurve MotionCurve =
            new SimpleCurve
                {
                    new CurvePoint(0f, 0f),
                    new CurvePoint(90f, 1.25f),
                    new CurvePoint(270f, -1.25f),
                    new CurvePoint(360f, 0f)
                };

        private Pawn pawn;

        private int nextRotation = -5000;
        private int nextRotationEnd = -5000;

        private RotationDirection rotationMod;

        private int headRotation = 0;

        private Thing target;

        public float CurrentMovement
        {
            get
            {
                return this.MotionCurve.Evaluate(this.headRotation);
            }

        }
        // Verse.AI.GenAI
        private bool EnemyIsNear([NotNull] Pawn p, float radius)
        {
            bool enemy = false;
            target = null;

            if (!p.Spawned)
            {
                return false;
            }

            List<IAttackTarget> potentialTargetsFor = p.Map.attackTargetsCache.GetPotentialTargetsFor(p);
            foreach (IAttackTarget attackTarget in potentialTargetsFor)
            {
                if (!attackTarget.ThreatDisabled())
                {
                    if (p.Position.InHorDistOf(((Thing)attackTarget).Position, radius))
                    {
                        enemy = true;
                        break;
                    }
                }
            }
            if (enemy)
            {
                Thing thing = (Thing)AttackTargetFinder.BestAttackTarget(
                    p,
                    TargetScanFlags.NeedReachable | TargetScanFlags.NeedThreat,
                    (Thing x) => x is Pawn,
                    0f,
                    radius,
                    default(IntVec3),
                    3.40282347E+38f,
                    true);

                if (thing != null)
                {
                    target = thing;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        // RimWorld.JobDriver_StandAndBeSociallyActive
        private void FindClosestTarget()
        {
            // Watch out for enemies
            if (EnemyIsNear(this.pawn, 40f))
            {
                return;
            }

            float rand = Rand.Value;

            // Look at each other
            if (rand > 0.7f)
            {
                IntVec3 position = this.pawn.Position;

                // 8 = 1 field; 24 = 2 fields;
                for (int i = 0; i < 8; i++)
                {
                    IntVec3 intVec = position + GenRadial.RadialPattern[i];
                    if (intVec.InBounds(this.pawn.Map))
                    {
                        Thing thing = intVec.GetThingList(this.pawn.Map).Find(x => x is Pawn);

                        if (thing != null && thing != this.pawn)
                        {
                            if (GenSight.LineOfSight(position, intVec, this.pawn.Map))
                            {
                                //  Log.Message(this.pawn + " will look at random pawn " + thing);
                                target = thing;
                            }
                        }
                    }
                }
            }

            Job job = this.pawn.CurJob;
            if (job != null && job.targetA.IsValid)
            {
                LocalTargetInfo targetA = this.pawn.CurJob.targetA;
                if (!targetA.HasThing)
                {
                    return;
                }
                Thing thing = targetA.Thing;
                if (this.pawn.Position.InHorDistOf(thing.Position, 5f))
                {
                    //  Log.Message(this.pawn + " will look at job thing " + thing);
                    this.target = thing;
                    return;
                }
            }
        }


        public void RotatorTick()
        {

            int tickManagerTicksGame = Find.TickManager.TicksGame;

            this.headRotation++;
            if (this.headRotation > 360)
            {
                this.headRotation = 0;
            }

            if (tickManagerTicksGame > this.nextRotationEnd)
            {
                // Stop tracking after a while
                this.target = null;

                // Set upnext blinking cycle
                this.SetNextRotation(tickManagerTicksGame);

                // if (GenAI.InDangerousCombat(this.pawn))
                // {
                //     this.rotationMod = RotationDirection.None;
                //     return;
                // }
                this.FaceHead();
            }
            else
            {
                if (tickManagerTicksGame % 10 == 0)
                {
                    this.TrackHead();
                }
            }

        }

        private void TrackHead()
        {
            if (this.target != null)
            {
                this.FaceHead();
            }
        }

        public void FaceHead()
        {
            bool random = false;
            if (this.target == null)
            {
                random = true;
                this.FindClosestTarget();
            }

            if (this.target != null)
            {
               // if (random)
               // {
               //     if (target is Pawn p && p.GetComp<CompFace>() != null)
               //     {
               //         // Log.Message(p + " look back at " + this.pawn);
               //         p.GetComp<CompFace>().HeadRotator.LookAtPawn(this.pawn);
               //     }
               // }
                float angle = (this.target.Position - this.pawn.Position).ToVector3().AngleFlat();
                Rot4 rot = PawnRotator.RotFromAngleBiased(angle);
                if (rot != this.pawn.Rotation.Opposite)
                {
                    int rotty = this.pawn.Rotation.AsInt - rot.AsInt;
                    switch (rotty)
                    {
                        case 0:
                            this.rotationMod = RotationDirection.None;
                            break;
                        case -1:
                            this.rotationMod = RotationDirection.Clockwise;
                            break;
                        case 1:
                            this.rotationMod = RotationDirection.Counterclockwise;
                            break;
                    }
                    //  Log.Message(this.pawn + " now watching " + target.GetThingList(this.pawn.Map));
                    return;
                }
            }

            // Make them smile.
            // if (this.pawn.pather.Moving)
            // {
            //     this.rotationMod = RotationDirection.None;
            //     return;
            // }
            this.rotationMod = RotationDirection.None;
        }

        public void LookAtPawn(Thing t)
        {
            this.target = t;
            this.FaceHead();
            this.SetNextRotation(Find.TickManager.TicksGame + 720);
            // Log.Message(this.pawn + " look at " + p);
        }

        private void SetNextRotation(int tickManagerTicksGame)
        {
            float blinkDuration = Rand.Range(120f, 240f);

            this.nextRotationEnd = (int)(tickManagerTicksGame + blinkDuration);
        }

        public Rot4 Rotation(Rot4 headFacing, bool renderBody)
        {
            Rot4 rot = headFacing;
            bool flag = false;
            if (renderBody)
            {
                flag = true;
            }
            else if (!rot.IsHorizontal)
            {
                flag = true;
            }

            if (flag)
            {
                rot.Rotate(this.rotationMod);
            }
            return rot;
        }
    }
}