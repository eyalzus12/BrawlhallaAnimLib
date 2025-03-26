using System.Collections.Generic;
using BrawlhallaAnimLib.Gfx;

namespace BrawlhallaAnimLib.Bones;

internal static class BoneDatabase
{
    static BoneDatabase()
    {
        // taken directly from the game
        Register1("a_WeaponCrateReady", 3);
        Register1("a_WeaponCratePickUp", 3);
        Register1("a_WeaponCrateImpact", 3);
        Register1("a_WeaponCrateForm", 3);
        Register1("a_WeaponCrateFall", 3);
        Register1("a_WeaponCrateDelivery", 3);
        Register1("a_WeaponCrateAirPickUp", 3);
        Register1("a_WeaponCrate", 3);
        Register1("a_AxePickupFX", 3);
        Register1("a_PistolPickupFX", 3);
        Register1("a_SwordPickupFX", 3);
        Register1("a_LancePickupFX", 3);
        Register1("a_OrbPickupFX", 3);
        Register1("a_HammerPickupFX", 3);
        Register1("a_SpearPickupFX", 3);
        Register1("a_KatarPickupFX", 3);
        Register1("a_SpearPickupFXBack", 3);
        Register1("a_BowPickupFX", 3);
        Register1("a_FistsPickupFX", 3);
        Register1("a_KatarPickupFXRear", 3);
        Register1("a_ScythePickupFX", 3);
        Register1("a_CannonPickupFX", 3);
        Register1("a_GreatswordPickupFX", 3);
        Register1("a_ChakramPickupFX", 3);
        Register1("a_WeaponCrateJumpLand01", 3);
        Register1("a_WeaponCrateJumpLand02", 3);
        Register1("a_WeaponCrateJumpLand03", 3);
        Register1("a_WeaponCrateJumpLand04", 3);
        Register1("a_WeaponCrateJumpLand05", 3);
        Register1("a_WeaponCrateJumpLand06", 3);
        Register1("a_WeaponCrateJumpLand07", 3);
        Register1("a_WeaponCrateJumpLand08", 3);
        Register1("a_WeaponCrateJumpLand09", 3);
        Register1("a_WeaponCrateJumpLand10", 3);
        Register1("a_WeaponCrateJumpLand11", 3);
        Register1("a_WeaponCrateJumpLand12", 3);
        Register1("a_WeaponCrateJumpLand13", 3);
        Register1("a_WeaponCrateJumpLand14", 3);
        Register1("a_WeaponCrateJumpLand15", 3);
        Register1("a_WeaponCrateJumpLand16", 3);
        Register1("a_WeaponCrateJumpLand17", 3);
        Register1("a_WeaponCrateJumpLand18", 3);
        Register1("a_WeaponCrateJumpLand19", 3);
        Register1("a_WeaponCrateJumpLand20", 3);
        Register1("a_WeaponCrateJumpLand21", 3);
        Register1("a_WeaponCrateJumpLand22", 3);
        Register1("a_WeaponCrateJumpLand23", 3);
        Register1("a_WeaponCrateJumpLand24", 3);
        Register1("a_WeaponCrateJumpLand25", 3);
        Register1("a_WeaponCrateJumpLand26", 3);
        Register1("a_WeaponCrateJumpLand27", 3);
        Register1("a_WeaponCrateJumpLand28", 3);
        Register1("a_WeaponCrateJumpLand29", 3);
        Register1("a_WeaponCrateJumpLand30", 3);
        Register1("a_WeaponCrateJumpLand31", 3);
        Register1("a_WeaponCrateJumpLand32", 3);
        Register1("a_WeaponCrateJumpLand33", 3);
        Register1("a_WeaponCrateJumpLand34", 3);
        Register1("a_WeaponCrateJumpLand35", 3);
        Register1("a_WeaponCrateJumpLand51", 3);
        Register1("a_WeaponCrateJumpLand52", 3);
        Register1("a_WeaponCrateJumpLand53", 3);
        Register1("a_WeaponCrateJumpLand54", 3);
        Register1("a_WeaponCrateJumpLand55", 3);
        Register1("a_WeaponCrateJumpLand56", 3);
        Register1("a_WeaponCrateJumpLand57", 3);
        Register1("a_WeaponCrateJumpLand58", 3);
        Register1("a_WeaponCrateJumpLand59", 3);
        Register1("a_WeaponCrateJumpLand60", 3);
        Register1("a_WeaponCrateReady01", 3);
        Register1("a_WeaponCrateReady02", 3);
        Register1("a_WeaponCrateReady03", 3);
        Register1("a_WeaponCrateReady04", 3);
        Register1("a_WeaponCrateReady05", 3);
        Register1("a_WeaponCrateReady06", 3);
        Register1("a_WeaponCrateReady07", 3);
        Register1("a_WeaponCrateReady08", 3);
        Register1("a_WeaponCrateReady09", 3);
        Register1("a_WeaponCrateReady10", 3);
        Register1("a_WeaponCrateReady11", 3);
        Register1("a_WeaponCrateReady12", 3);
        Register1("a_WeaponCrateReady13", 3);
        Register1("a_WeaponCrateReady14", 3);
        Register1("a_WeaponCrateReady15", 3);
        Register1("a_WeaponCrateReady16", 3);
        Register1("a_WeaponCrateReady17", 3);
        Register1("a_WeaponCrateReady18", 3);
        Register1("a_WeaponCrateReady19", 3);
        Register1("a_WeaponCrateReady20", 3);
        Register1("a_WeaponCrateReady21", 3);
        Register1("a_WeaponCrateReady22", 3);
        Register1("a_WeaponCrateReady23", 3);
        Register1("a_WeaponCrateReady24", 3);
        Register1("a_WeaponCrateForm01", 3);
        Register1("a_WeaponCrateForm02", 3);
        Register1("a_WeaponCrateForm03", 3);
        Register1("a_WeaponCrateForm04", 3);
        Register1("a_Helmet", 2);
        Register1("a_HelmetBack", 2);
        Register2("a_Torso1", 2, 8, false);
        Register1("a_Torso1R", 2);
        Register2("a_Torso1Back", 2, 8, false);
        Register1("a_Torso2", 2);
        Register1("a_Torso2Back", 2);
        Register2("a_Shoulder1", 2, 4, true, true);
        Register2("a_Shoulder1Right", 2, 4, true, true);
        Register2("a_Arm", 2, 3, true);
        Register2("a_Arm1", 2, 3, true);
        Register2("a_ArmRight", 2, 3, true);
        Register2("a_Arm1Right", 2, 3, true);
        Register2("a_Forearm", 2, 2, false, true);
        Register2("a_Forearm2", 2, 2, false, true);
        Register2("a_ForearmAway", 2, 2, false, true);
        Register2("a_ForearmAway2", 2, 2, false, true);
        Register2("a_ForearmRight", 2, 2, false, true);
        Register2("a_Forearm2Right", 2, 2, false, true);
        Register2("a_ForearmAwayRight", 2, 2, false, true);
        Register2("a_ForearmAway2Right", 2, 2, false, true);
        Register2("a_HandFist01a", 2, 1, true);
        Register2("a_HandFist01b", 2, 1, true);
        Register2("a_HandFist01c", 2, 1, true);
        Register2("a_HandFist01d", 2, 1, true);
        Register2("a_HandFist01e", 2, 1, false);
        Register2("a_HandFist01f", 2, 1, false);
        Register2("a_HandFist01g", 2, 1, false);
        Register2("a_HandFist01h", 2, 1, true);
        Register2("a_HandFist02a", 2, 1, false);
        Register2("a_HandFist02b", 2, 1, false);
        Register2("a_HandFist02d", 2, 1, true);
        Register2("a_HandFist02e", 2, 1, false);
        Register2("a_HandFist02f", 2, 1, false);
        Register2("a_HandFist03a", 2, 1, true);
        Register2("a_HandFist03b", 2, 1, true);
        Register2("a_HandFist03c", 2, 1, true);
        Register2("a_HandFist03d", 2, 1, true);
        Register2("a_HandFist04a", 2, 1, false);
        Register2("a_HandFist04aBlaster", 2, 1, false);
        Register2("a_HandFist05", 2, 1, true);
        Register2("a_HandFist06", 2, 1, false);
        Register2("a_HandFist07", 2, 1, false);
        Register2("a_HandFist08", 2, 1, true);
        Register2("a_HandFist09", 2, 1, false);
        Register2("a_HandFistPoint01", 2, 1, true);
        Register2("a_HandFistPoint01b", 2, 1, true);
        Register2("a_HandFistPoint02", 2, 1, false);
        Register2("a_HandOpen01b", 2, 1, true);
        Register2("a_HandOpen01a", 2, 1, true);
        Register2("a_HandOpen02a", 2, 1, false);
        Register2("a_HandOpen02b", 2, 1, false);
        Register2("a_HandOpen02c", 2, 1, false);
        Register2("a_HandOpen02d", 2, 1, false);
        Register2("a_HandOpen03", 2, 1, false);
        Register2("a_HandOpen03a", 2, 1, false);
        Register2("a_HandOpen03b", 2, 1, false);
        Register2("a_HandOpen04", 2, 1, true);
        Register2("a_HandOpen04Pinky", 2, 1, false);
        Register2("a_HandOpen04Pinky2", 2, 1, false);
        Register2("a_HandOpen04Pinky3", 2, 1, false);
        Register2("a_HandOpen04Pinky3a", 2, 1, false);
        Register2("a_HandOpen05", 2, 1, false);
        Register2("a_HandOpen05a", 2, 1, false);
        Register2("a_HandOpen05b", 2, 1, false);
        Register2("a_HandOpen05c", 2, 1, false);
        Register2("a_HandOpen05Back", 2, 1, true);
        Register2("a_HandOpen05Backc", 2, 1, false);
        Register2("a_HandOpen06", 2, 1, true);
        Register2("a_HandOpen06a", 2, 1, true);
        Register2("a_HandOpen06c", 2, 1, true);
        Register2("a_HandOpen07", 2, 1, false);
        Register2("a_HandOpen08", 2, 1, false);
        Register2("a_HandOpen09", 2, 1, false);
        Register2("a_HandOpen09c", 2, 1, false);
        Register2("a_HandOpen10", 2, 1, false);
        Register2("a_HandOpen11", 2, 1, true);
        Register2("a_HandOpen11a", 2, 1, true);
        Register2("a_HandOpen11b", 2, 1, false);
        Register2("a_HandOpen12", 2, 1, true);
        Register2("a_HandOpen13", 2, 1, false);
        Register2("a_HandOpen13b", 2, 1, false);
        Register2("a_HandOpen13Back", 2, 1, false);
        Register2("a_HandOpen13Side", 2, 1, false);
        Register2("a_HandOpen14a", 2, 1, true);
        Register2("a_HandOpen14b", 2, 1, false);
        Register2("a_HandOpen14c", 2, 1, false);
        Register2("a_HandOpen14d", 2, 1, false);
        Register2("a_HandOpen15a", 2, 1, false);
        Register2("a_HandOpen15b", 2, 1, true);
        Register2("a_HandOpen15c", 2, 1, false);
        Register2("a_HandOpen16a", 2, 1, false);
        Register2("a_HandOpen16b", 2, 1, true);
        Register2("a_HandOpen17a", 2, 1, false);
        Register2("a_HandOpen17b", 2, 1, false);
        Register2("a_HandSupport", 2, 1, false);
        Register2("a_HandSupport02", 2, 1, true);
        Register2("a_HandTrigger", 2, 1, true);
        Register2("a_HandTriggerBlaster", 2, 1, true);
        Register2("a_HandTriggerb", 2, 1, true);
        Register2("a_HandTriggerc", 2, 1, true);
        Register2("a_HandTriggerSpin", 2, 1, true);
        Register2("a_HandThumb", 2, 1, true);
        Register2("a_HandThumb02", 2, 1, false);
        Register2("a_HandPullString01", 2, 1, true);
        Register2("a_HandPullString02", 2, 1, false);
        Register2("a_HandPullString03", 2, 1, false);
        Register2("a_HandPullString03b", 2, 1, false);
        Register2("a_HandPullString03c", 2, 1, false);
        Register2("a_HandPullString03d", 2, 1, false);
        Register2("a_HandPullString04", 2, 1, true);
        Register2("a_HandFist01aKatar", 2, 1, true);
        Register2("a_HandFist04aKatar", 2, 1, true);
        Register2("a_HandFist08Katar", 2, 1, true);
        Register2("a_HandFist01cKatar", 2, 1, true);
        Register2("a_HandFist09Katar", 2, 1, true);
        Register2("a_HandFist07Sword", 2, 1, false);
        Register2("a_HandFist04aSword", 2, 1, false);
        Register2("a_HandFist03bSword", 2, 1, true);
        Register2("a_HandFist03aSword", 2, 1, true);
        Register2("a_HandFist02eSword", 2, 1, false);
        Register2("a_HandFist02dSword", 2, 1, true);
        Register2("a_HandFist02bSword", 2, 1, false);
        Register2("a_HandFist02aSword", 2, 1, false);
        Register2("a_HandFist01fSword", 2, 1, false);
        Register2("a_HandFist01eSword", 2, 1, false);
        Register2("a_HandFist01cSword", 2, 1, true);
        Register2("a_HandFist01bSword", 2, 1, true);
        Register2("a_HandFist01aSword", 2, 1, true);
        Register1("a_Waist1", 2);
        Register1("a_Waist1Back", 2);
        Register2("a_Leg1", 2, 5, true, true);
        Register2("a_Leg1Flip", 2, 5, true);
        Register2("a_ShinBack", 2, 6, true, true);
        Register2("a_ShinSide", 2, 6, true, true);
        Register2("a_ShinSideStraight", 2, 6, true, true);
        Register2("a_Shin", 2, 6, true, true);
        Register2("a_ShinFrontAngle", 2, 6, true, true);
        Register2("a_ShinSideBend", 2, 6, true, true);
        Register2("a_Foot1", 2, 7, true);
        Register2("a_Foot1Side", 2, 7, true);
        Register2("a_Foot1Bent", 2, 7, true);
        Register2("a_Leg1Right", 2, 5, true, true);
        Register2("a_Leg1FlipRight", 2, 5, true);
        Register2("a_ShinBackRight", 2, 6, true, true);
        Register2("a_ShinSideRight", 2, 6, true, true);
        Register2("a_ShinSideStraightRight", 2, 6, true, true);
        Register2("a_ShinRight", 2, 6, true, true);
        Register2("a_ShinFrontAngleRight", 2, 6, true, true);
        Register2("a_ShinSideBendRight", 2, 6, true, true);
        Register2("a_Foot1Right", 2, 7, true);
        Register2("a_Foot1SideRight", 2, 7, true);
        Register2("a_Foot1BentRight", 2, 7, true);
        Register2("a_Hair", 2, 17, false);
        Register2("a_HairBack", 2, 17, false);
        Register1("a_HairR", 2);
        Register1("a_HairRBack", 2);
        Register1("a_Ear", 2);
        Register1("a_EarExtra", 2);
        Register1("a_EarBack", 2);
        Register1("a_EarBackExtra", 2);
        Register1("a_Nose", 2);
        Register2("a_Jaw", 2, 13, false);
        Register1("a_JawR", 2);
        Register2("a_JawBack", 2, 13, false);
        Register2("a_Mouth", 2, 16, false);
        Register2("a_MouthSmile", 2, 16, false);
        Register2("a_MouthKO", 2, 16, false);
        Register2("a_MouthHit", 2, 16, false);
        Register2("a_MouthGrowl", 2, 16, false);
        Register2("a_MouthBack", 2, 16, false);
        Register2("a_MouthWarCry", 2, 16, false);
        Register2("a_MouthBlow", 2, 16, false);
        Register1("a_MouthR", 2);
        Register1("a_MouthRSmile", 2);
        Register1("a_MouthRKO", 2);
        Register1("a_MouthRHit", 2);
        Register1("a_MouthRGrowl", 2);
        Register1("a_MouthRBack", 2);
        Register1("a_MouthRWarCry", 2);
        Register1("a_MouthRBlow", 2);
        Register2("a_Eyes", 2, 14, false);
        Register2("a_EyesTurn", 2, 14, false);
        Register2("a_EyesKO", 2, 14, false);
        Register2("a_EyesHit", 2, 14, false);
        Register2("a_EyesDown", 2, 14, false);
        Register2("a_EyesAngry", 2, 14, false);
        Register1("a_EyesR", 2);
        Register1("a_EyesRTurn", 2);
        Register1("a_EyesRKO", 2);
        Register1("a_EyesRHit", 2);
        Register1("a_EyesRDown", 2);
        Register1("a_EyesRAngry", 2);
        Register1("a_Accent", 2);
        Register1("a_AccentTurn", 2);
        Register1("a_AccentKO", 2);
        Register1("a_AccentHit", 2);
        Register1("a_AccentDown", 2);
        Register1("a_AccentAngry", 2);
        Register1("a_AccentSpecial", 2);
        for (int i = 1; i < 86; ++i)
        {
            string name = i.ToString();
            if (name.Length < 2)
            {
                name = "0" + name;
            }
            Register1("a_Special" + name, 2);
        }
        Register1("a_WeaponHammer", 1);
        Register1("a_WeaponHammerShort", 1);
        Register1("a_WeaponSword", 1);
        Register1("a_WeaponSwordAttack", 1);
        Register1("a_WeaponSwordLand", 1);
        Register1("a_WeaponRocketLance", 1);
        Register1("a_WeaponRocketLanceOpen", 1);
        Register1("a_WeaponRocketLanceSpin1", 1);
        Register1("a_WeaponRocketLanceSpin2", 1);
        Register1("a_WeaponRocketLanceSpin3", 1);
        Register1("a_LanceBackOpen", 1);
        Register2("a_WeaponPistol", 1, 11, true);
        Register2("a_WeaponPistolRight", 1, 11, true);
        Register1("a_WeaponPistolSpin", 1);
        Register1("a_WeaponSpear", 1);
        Register1("a_WeaponSpearBend1", 1);
        Register1("a_WeaponSpearBend1Back", 1);
        Register1("a_WeaponSpearBend2", 1);
        Register1("a_WeaponSpearBend2Back", 1);
        Register1("a_WeaponSpearForeshortened", 1);
        Register1("a_WeaponSpearHead2", 1);
        Register1("a_WeaponSpearSpin", 1);
        Register1("a_WeaponSpearBuried", 1);
        Register1("a_WeaponSpearBuried2", 1);
        Register1("a_WeaponSpearBuried3", 1);
        Register1("a_SpearEndSegment", 1);
        Register1("a_SpearShaftSegement", 1);
        Register1("a_WeaponSpearHead3", 1);
        Register1("a_WeaponAxe", 1);
        Register1("a_WeaponAxeSide", 1);
        Register1("a_WeaponAxeSideAway", 1);
        Register1("a_WeaponBow", 1);
        Register1("a_WeaponBowGrip", 1);
        Register1("a_WeaponBowTop", 1);
        Register1("a_WeaponBowBottom", 1);
        Register1("a_WeaponBowAngled", 1);
        Register1("a_WeaponBowAngledAway", 1);
        Register1("a_WeaponScythe", 1);
        Register1("a_WeaponScytheToward1", 1);
        Register1("a_WeaponScytheAway1", 1);
        Register1("a_WeaponScytheHead", 1);
        Register1("a_WeaponScytheHeadToward1", 1);
        Register1("a_WeaponScytheHeadAway1", 1);
        Register1("a_WeaponScytheSpin", 1);
        Register1("a_WeaponCannon", 1);
        Register1("a_WeaponCannon2", 1);
        Register1("a_WeaponCannonAway", 1);
        Register1("a_WeaponCannonToward", 1);
        Register1("a_WeaponOrb", 1);
        Register1("a_WeaponOrbActive", 1);
        Register1("a_WeaponOrbSmear", 1);
        Register1("a_WeaponOrbSpin", 1);
        Register1("a_WeaponChakram", 1);
        Register1("a_WeaponChakramCombinedSpin", 1);
        Register1("a_WeaponChakramCombinedAngleToward", 1);
        Register1("a_WeaponChakramCombinedAngleTop", 1);
        Register1("a_WeaponChakramCombinedAngleBottom", 1);
        Register1("a_WeaponChakramCombinedAngleAway", 1);
        Register1("a_WeaponChakramCombined", 1);
        Register1("a_WeaponChakramAngleTowardRight", 1);
        Register1("a_WeaponChakramAngleToward", 1);
        Register1("a_WeaponChakramAngleTopRight", 1);
        Register1("a_WeaponChakramAngleTop", 1);
        Register1("a_WeaponChakramAngleBottomRight", 1);
        Register1("a_WeaponChakramAngleBottom", 1);
        Register1("a_WeaponChakramAngleAwayRight", 1);
        Register1("a_WeaponChakramAngleAway", 1);
        Register1("a_WeaponChakramSpinRight", 1);
        Register1("a_WeaponChakramSpin", 1);
        Register1("a_WeaponChakramRight", 1);
        Register1("a_WeaponSwordOverlay1", 1);
        Register1("a_WeaponSwordOverlay2", 1);
        Register1("a_WeaponSwordOverlay3", 1);
        Register1("a_WeaponSwordOverlay4", 1);
        Register1("a_WeaponSwordOverlay5", 1);
        Register1("a_WeaponSwordOverlay6", 1);
        Register1("a_WeaponSwordOverlay7", 1);
        Register1("a_WeaponSwordOverlay8", 1);
        Register1("a_WeaponSwordOverlay9", 1);
        Register1("a_WeaponSwordOverlay10", 1);
        Register1("a_WeaponSwordOverlay11", 1);
        Register1("a_WeaponSwordOverlay12", 1);
        Register1("a_WeaponSwordOverlay13", 1);
        Register2("a_WeaponKatarBladeUnder", 1, 12, true, true);
        Register2("a_WeaponKatarBladeUnderBuried", 1, 12, true, true);
        Register2("a_WeaponKatarBladeUnderTowards", 1, 12, true, true);
        Register2("a_WeaponKatarBladeTop", 1, 12, true, true);
        Register2("a_WeaponKatarBladeTopStrap", 1, 12, true);
        Register2("a_WeaponKatarBladeUnderRight", 1, 12, true, true);
        Register2("a_WeaponKatarBladeUnderBuriedRight", 1, 12, true, true);
        Register2("a_WeaponKatarBladeUnderTowardsRight", 1, 12, true, true);
        Register2("a_WeaponKatarBladeTopRight", 1, 12, true, true);
        Register2("a_WeaponKatarBladeTopStrapRight", 1, 12, true);
        Register2("a_WeaponKatarOverlayBladeUnder", 1, 12, true);
        Register2("a_WeaponKatarOverlayBladeUnderRight", 1, 12, true);
        Register2("a_WeaponKatarOverlayBladeUnderTowards", 1, 12, true);
        Register2("a_WeaponKatarOverlayBladeUnderTowardsRight", 1, 12, true);
        Register2("a_WeaponKatarOverlay2BladeUnder", 1, 12, true);
        Register2("a_WeaponKatarOverlay2BladeUnderRight", 1, 12, true);
        Register2("a_WeaponFists01", 1, 9, true);
        Register2("a_WeaponFists01Large", 1, 9, true);
        Register2("a_WeaponFists02", 1, 9, true);
        Register2("a_WeaponFists03", 1, 9, true);
        Register2("a_WeaponFists03Reversed", 1, 9, false);
        Register2("a_WeaponFists04", 1, 9, true);
        Register2("a_WeaponFists05", 1, 9, true);
        Register2("a_WeaponFists06", 1, 9, true);
        Register2("a_WeaponFists06Reversed", 1, 9, false);
        Register2("a_WeaponFists07", 1, 9, true);
        Register2("a_WeaponFists07Reversed", 1, 9, false);
        Register2("a_WeaponFistsAway", 1, 9, true);
        Register2("a_WeaponFistsAwayReversed", 1, 9, false);
        Register2("a_WeaponFistsOpen01a", 1, 9, true);
        Register2("a_WeaponFistsOpen01b", 1, 9, true);
        Register2("a_WeaponFistsOpen01c", 1, 9, true);
        Register2("a_WeaponFistsOpen02a", 1, 9, true);
        Register2("a_WeaponFistsOpen02b", 1, 9, true);
        Register2("a_WeaponFistsOpen03a", 1, 9, true);
        Register2("a_WeaponFistsOpen03b", 1, 9, true);
        Register2("a_WeaponFistsOpen03Reverseda", 1, 9, false);
        Register2("a_WeaponFistsOpen03Reversedb", 1, 9, false);
        Register2("a_WeaponFistsOpen04a", 1, 9, true);
        Register2("a_WeaponFistsOpen04b", 1, 9, true);
        Register2("a_WeaponFistsOpen04c", 1, 9, true);
        Register2("a_WeaponFistsOpen05a", 1, 9, true);
        Register2("a_WeaponFistsOpen05b", 1, 9, true);
        Register2("a_WeaponFistsOpen06a", 1, 9, true);
        Register2("a_WeaponFistsOpen06b", 1, 9, true);
        Register2("a_WeaponFistsOpen06Reverseda", 1, 9, false);
        Register2("a_WeaponFistsOpen06Reversedb", 1, 9, false);
        Register2("a_WeaponFistsOpen07a", 1, 9, true);
        Register2("a_WeaponFistsOpen08", 1, 9, true);
        Register2("a_WeaponFistsOpen09", 1, 9, true);
        Register2("a_WeaponFistsForearm", 1, 10, true);
        Register2("a_WeaponFistsForearmR", 1, 10, true);
        Register2("a_WeaponFistsForearmLarge", 1, 10, true);
        Register2("a_WeaponFistsForearm2", 1, 10, true);
        Register2("a_WeaponFistsForearmAway", 1, 10, true);
        Register2("a_WeaponFistsForearmRight", 1, 10, true);
        Register2("a_WeaponFistsForearmRightR", 1, 10, true);
        Register2("a_WeaponFistsForearmLargeRight", 1, 10, true);
        Register2("a_WeaponFistsForearm2Right", 1, 10, true);
        Register2("a_WeaponFistsForearmAwayRight", 1, 10, true);
        Register1("a_WeaponGreat", 1);
        Register1("a_WeaponGreatExtremeBladeAway", 1);
        Register1("a_WeaponGreatExtremeBladeToward", 1);
        Register1("a_WeaponGreatExtremeHandleAway", 1);
        Register1("a_WeaponGreatExtremeHandleToward", 1);
        Register1("a_WeaponGreatQuarterAway", 1);
        Register1("a_WeaponGreatQuarterToward", 1);
        Register1("a_WeaponGreatStabS3", 1);
        Register2("a_WeaponBootsBack", 1, 15, true);
        Register2("a_WeaponBootsBackRight", 1, 15, true);
        Register2("a_WeaponBootsFront", 1, 15, true);
        Register2("a_WeaponBootsFrontRight", 1, 15, true);
        Register2("a_WeaponBootsSide", 1, 15, true);
        Register2("a_WeaponBootsSideRight", 1, 15, true);
        Register2("a_WeaponBootsSideBent", 1, 15, true);
        Register2("a_WeaponBootsSideBentRight", 1, 15, true);
        Register2("a_WeaponBootsSideBottom", 1, 15, true);
        Register2("a_WeaponBootsSideBottomRight", 1, 15, true);
        Register2("a_WeaponBootsSideTop", 1, 15, true);
        Register2("a_WeaponBootsSideTopRight", 1, 15, true);
        Register2("a_WeaponBootsToeBack", 1, 15, true);
        Register2("a_WeaponBootsToeBackRight", 1, 15, true);
        Register2("a_WeaponBootsToeFront", 1, 15, true);
        Register2("a_WeaponBootsToeFrontRight", 1, 15, true);
        Register2("a_WeaponBootsToeSide", 1, 15, true);
        Register2("a_WeaponBootsToeSideRight", 1, 15, true);
        Register2("a_WeaponBootsToeSideBottom", 1, 15, true);
        Register2("a_WeaponBootsToeSideBottomRight", 1, 15, true);
        Register2("a_WeaponBootsToeSideTop", 1, 15, true);
        Register2("a_WeaponBootsToeSideTopRight", 1, 15, true);
        Register1("a_Flag1a", 4);
        Register1("a_Flag1b", 4);
        Register1("a_Flag1bLong", 4);
        Register1("a_Flag1c", 4);
        Register1("a_Flag2a", 4);
        Register1("a_Flag2b", 4);
        Register1("a_Flag2c", 4);
        Register1("a_Flag2cLong", 4);
        Register1("a_BotArmBack", 5);
        Register1("a_BotArmFront", 5);
        Register1("a_BotForearmBack", 5);
        Register1("a_BotForearmFront", 5);
        Register1("a_BotHead", 5);
        Register1("a_BotTail", 5);
        Register2("a_BotTorso", 5, 8, false, true);
        Register1("a_CompanionBone001", 6);
        Register1("a_CompanionBone002", 6);
        Register1("a_CompanionBone003", 6);
        Register1("a_CompanionBone004", 6);
        Register1("a_CompanionBone005", 6);
        Register1("a_CompanionBone006", 6);
        Register1("a_CompanionBone007", 6);
        Register1("a_CompanionBone008", 6);
        Register1("a_CompanionBone009", 6);
        Register1("a_GhostPupil04", 6);
        Register1("a_GhostPupil03", 6);
        Register1("a_GhostPupil02", 6);
        Register1("a_GhostPupil01", 6);
        Register1("a_GhostPanelJ01", 6);
        Register1("a_GhostPanelI02", 6);
        Register1("a_GhostPanelI01", 6);
        Register1("a_GhostPanelH03", 6);
        Register1("a_GhostPanelH02", 6);
        Register1("a_GhostPanelH01", 6);
        Register1("a_GhostPanelG02", 6);
        Register1("a_GhostPanelG01", 6);
        Register1("a_GhostPanelF01", 6);
        Register1("a_GhostPanelE01", 6);
        Register1("a_GhostPanelD02", 6);
        Register1("a_GhostPanelD01", 6);
        Register1("a_GhostPanelC01", 6);
        Register1("a_GhostPanelB01", 6);
        Register1("a_GhostPanelA02", 6);
        Register1("a_GhostPanelA01", 6);
        Register1("a_GhostJoint01", 6);
        Register1("a_GhostEye01", 6);
        Register1("a_GhostBall01", 6);
        Register1("a_GhostBackdrop01", 6);
        Register1("a_CapeIdle00", 2);
        Register1("a_CapeIdle01", 2);
        Register1("a_CapeIdle02", 2);
        Register1("a_CapeIdle03", 2);
        Register1("a_CapeIdle04", 2);
        Register1("a_CapeStretch", 2);
        Register1("a_CapeTurn01", 2);
        Register1("a_CapeTurn02", 2);
        Register1("a_CapeBillow01", 2);
        Register1("a_CapeBillow02", 2);
        Register1("a_CapeBillow03", 2);
        Register1("a_CapeBillow04", 2);
        Register1("a_CapeBillow05", 2);
        Register1("a_CapeBillow06", 2);
        Register1("a_CapeBillow07", 2);
        Register1("a_CapeDash01", 2);
        Register1("a_CapeDash02", 2);
        Register1("a_CapeBack", 2);
        Register1("a_CapeBackStretch", 2);
        Register1("a_CapeBackBillow01", 2);
        Register1("a_CapeBackBillow02", 2);
        Register1("a_CapeBackBillow03", 2);
        Register1("a_CapeBackBillow04", 2);
        Register1("a_CapeBackTurnFar", 2);
        Register1("a_CapeBackTurnNear", 2);
        Register1("a_KADSwoosh06b", 1);
        Register1("a_KAHDSwoosh05", 1);
        Register1("a_KAHSwoosh01", 1);
        Register1("a_KAHSwoosh02", 1);
        Register1("a_KANSwoosh01", 1);
        Register1("a_KANSwoosh09", 1);
        Register1("a_KGPSwoosh02", 1);
        Register1("a_SwooshKAS06", 1);
        Register1("a_SwooshSpAttackAirSide2", 1);
        Register1("a_SwooshSpAttackDownHit", 1);
        Register1("a_SwooshSpAttackSidea", 1);
        Register1("a_SwooshSpAttackSideb", 1);
        Register1("a_Swoosh1HR_AttackAirDown2b", 1);
        Register1("a_Swoosh1HR_AttackAirSidea", 1);
        Register1("a_Swoosh1HR_AttackAirUpb", 1);
        Register1("a_Swoosh1HR_AttackDown2", 1);
        Register1("a_Swoosh1HR_AttackDown2Hit", 1);
        Register1("a_Swoosh1HR_AttackSideb", 1);
        Register1("a_Swoosh1HR_Combo1a", 1);
        Register1("a_Swoosh1HR_Combo1b", 1);
        Register1("a_Swoosh1HR_Combo2", 1);
        Register1("a_Swoosh1HR_Combo3a", 1);
        Register1("a_Swoosh1HR_Combo3b", 1);
        Register1("a_Swoosh1HR_AttackAirSideb", 1);
        Register1("a_SwooshSpAttackAirDown", 1);
    }

    public static Dictionary<string, ArtTypeEnum> ArtTypeDict { get; } = [];
    public static Dictionary<string, BoneType> BoneTypeDict { get; } = [];
    public static Dictionary<string, string> ForearmVariantDict { get; } = [];
    public static Dictionary<string, string> ShinVariantDict { get; } = [];
    public static Dictionary<string, string> KatarVariantDict { get; } = [];
    public static Dictionary<string, string> AsymSwapDict { get; } = [];

    private static void Register1(string name, uint artType)
    {
        ArtTypeDict[name] = (ArtTypeEnum)artType;
    }

    private static void Register2(string name, uint artType, int boneType, bool dir, bool hasRVar = false)
    {
        BoneTypeDict[name] = new((BoneTypeEnum)boneType, dir);
        if (hasRVar)
        {
            string rVar = name + "R";
            BoneTypeDict[rVar] = new((BoneTypeEnum)boneType, dir);
            if (boneType == 2)
                ForearmVariantDict[name] = rVar;
            else if (boneType == 6)
                ShinVariantDict[name] = rVar;
            else if (boneType == 12)
                KatarVariantDict[name] = rVar;
            Register1(rVar, artType);
        }
        if (name.EndsWith("Right"))
            AsymSwapDict[name] = name[..^"Right".Length];
        else if (name.EndsWith("Left"))
            AsymSwapDict[name] = name[..^"Left".Length];
        Register1(name, artType);
    }
}