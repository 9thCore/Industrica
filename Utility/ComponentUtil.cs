namespace Industrica.Utility
{
    public static class ComponentUtil
    {
        public static void Setup<NewComponent, OldComponent>(NewComponent compNew, OldComponent compOld) where NewComponent : PlayerTool where OldComponent : PlayerTool
        {
            compNew.reloadMode = compOld.reloadMode;
            compNew.reloadSound = compOld.reloadSound;
            compNew.mainCollider = compOld.mainCollider;
            compNew.drawSound = compOld.drawSound;
            compNew.drawSoundUnderwater = compOld.drawSoundUnderwater;
            compNew.firstUseSound = compOld.firstUseSound;
            compNew.holsterSoundAboveWater = compOld.holsterSoundAboveWater;
            compNew.holsterSoundUnderwater = compOld.holsterSoundUnderwater;
            compNew.hitBleederSound = compOld.hitBleederSound;
            compNew.bleederDamage = compOld.bleederDamage;
            compNew.socket = compOld.socket;
            compNew.ikAimLeftArm = compOld.ikAimLeftArm;
            compNew.ikAimRightArm = compOld.ikAimRightArm;
            compNew.rightHandIKTarget = compOld.rightHandIKTarget;
            compNew.useLeftAimTargetOnPlayer = compOld.useLeftAimTargetOnPlayer;
            compNew.leftHandIKTarget = compOld.leftHandIKTarget;
            compNew.hasAnimations = compOld.hasAnimations;
            compNew.drawTime = compOld.drawTime;
            compNew.holsterTime = compOld.holsterTime;
            compNew.dropTime = compOld.dropTime;
            compNew.bashTime = compOld.bashTime;
            compNew.forceConfigureIK = compOld.forceConfigureIK;
            compNew.pickupable = compOld.pickupable;
            compNew.hasFirstUseAnimation = compOld.hasFirstUseAnimation;
            compNew.hasBashAnimation = compOld.hasBashAnimation;
            compNew.renderers = compOld.renderers;
            compNew.plugOrigin = compOld.plugOrigin;
        }
    }
}
