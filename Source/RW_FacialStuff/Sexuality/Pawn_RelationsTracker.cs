﻿using RimWorld;
using UnityEngine;
using Verse;

namespace RW_FacialStuff.Sexuality
{
    public class Pawn_RelationsTrackerModded
    {
        private Pawn pawn;

        // RimWorld.Pawn_RelationsTracker
        public float AttractionToModded(Pawn otherPawn)
        {

            if (pawn.def != otherPawn.def || pawn == otherPawn)
            {
                return 0f;
            }
            float num = 1f;
            float num2 = 1f;
            float ageBiologicalYearsFloat = pawn.ageTracker.AgeBiologicalYearsFloat;
            float ageBiologicalYearsFloat2 = otherPawn.ageTracker.AgeBiologicalYearsFloat;
            if (pawn.gender == Gender.Male)
            {
                // if (pawn.RaceProps.Humanlike && pawn.story.traits.HasTrait(TraitDefOf.Gay))
                if (pawn.RaceProps.Humanlike && pawn.story.traits.DegreeOfTrait(TraitDef.Named("Gay")) == 1)
                {
                    if (otherPawn.gender == Gender.Female)
                    {
                        return 0f;
                    }
                }
                else if (pawn.story.traits.DegreeOfTrait(TraitDef.Named("Gay")) == 0)
                {
                    if (otherPawn.gender == Gender.Male)
                    {
                        return 0f;
                    }
                }
                num2 = GenMath.FlatHill(16f, 20f, ageBiologicalYearsFloat, ageBiologicalYearsFloat + 15f, ageBiologicalYearsFloat2);
            }
            else if (pawn.gender == Gender.Female)
            {
                if (pawn.RaceProps.Humanlike && pawn.story.traits.DegreeOfTrait(TraitDef.Named("Gay")) == 1)
                {
                    if (otherPawn.gender == Gender.Male)
                    {
                        return 0f;
                    }
                }
                else if (pawn.story.traits.DegreeOfTrait(TraitDef.Named("Gay")) == 0)
                {
                    if (otherPawn.gender == Gender.Female)
                    {
                        num = 0.15f;
                    }
                }

                if (ageBiologicalYearsFloat2 < ageBiologicalYearsFloat - 10f)
                {
                    return 0f;
                }
                if (ageBiologicalYearsFloat2 < ageBiologicalYearsFloat - 3f)
                {
                    num2 = Mathf.InverseLerp(ageBiologicalYearsFloat - 10f, ageBiologicalYearsFloat - 3f, ageBiologicalYearsFloat2) * 0.2f;
                }
                else
                {
                    num2 = GenMath.FlatHill(0.2f, ageBiologicalYearsFloat - 3f, ageBiologicalYearsFloat, ageBiologicalYearsFloat + 10f, ageBiologicalYearsFloat + 40f, 0.1f, ageBiologicalYearsFloat2);
                }
            }
            float num3 = 1f;
            num3 *= Mathf.Lerp(0.2f, 1f, otherPawn.health.capacities.GetEfficiency(PawnCapacityDefOf.Talking));
            num3 *= Mathf.Lerp(0.2f, 1f, otherPawn.health.capacities.GetEfficiency(PawnCapacityDefOf.Manipulation));
            num3 *= Mathf.Lerp(0.2f, 1f, otherPawn.health.capacities.GetEfficiency(PawnCapacityDefOf.Moving));
            float num4 = 1f;
            foreach (PawnRelationDef current in pawn.GetRelations(otherPawn))
            {
                num4 *= current.attractionFactor;
            }
            int num5 = 0;
            if (otherPawn.RaceProps.Humanlike)
            {
                num5 = otherPawn.story.traits.DegreeOfTrait(TraitDefOf.Beauty);
            }
            float num6 = 1f;
            if (num5 < 0)
            {
                num6 = 0.3f;
            }
            else if (num5 > 0)
            {
                num6 = 2.3f;
            }
            float num7 = Mathf.InverseLerp(15f, 18f, ageBiologicalYearsFloat);
            float num8 = Mathf.InverseLerp(15f, 18f, ageBiologicalYearsFloat2);
            return num * num2 * num3 * num4 * num7 * num8 * num6;
        }

    }
}