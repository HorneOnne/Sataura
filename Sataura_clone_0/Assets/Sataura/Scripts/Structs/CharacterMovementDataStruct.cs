namespace Sataura
{
    [System.Serializable]
    public struct CharacterMovementDataStruct
    {
        public float movementSpeed;
        public float movementForceInAir;
        public float airDragMultiplier;
        public float jumpForce;
        public float hangTime;
        public float jumpBufferLength;
        public float fallMultiplier;
        public float lowMultiplier;
        public float maxMovementSpeed;
        public float maxJumpVelocity;
        public float maxFallVelocity;

        public CharacterMovementDataStruct(CharacterMovementData characterMovementData)
        {
            movementSpeed = characterMovementData.movementSpeed;
            movementForceInAir = characterMovementData.movementForceInAir;
            airDragMultiplier = characterMovementData.airDragMultiplier;
            jumpForce = characterMovementData.jumpForce;
            hangTime = characterMovementData.hangTime;
            jumpBufferLength = characterMovementData.jumpBufferLength;
            fallMultiplier = characterMovementData.fallMultiplier;
            lowMultiplier = characterMovementData.lowMultiplier;
            maxMovementSpeed = characterMovementData.maxMovementSpeed;
            maxJumpVelocity = characterMovementData.maxJumpVelocity;
            maxFallVelocity = characterMovementData.maxFallVelocity;
        }
    }
}
