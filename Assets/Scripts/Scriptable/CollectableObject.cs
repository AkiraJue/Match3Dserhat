using System;
using UnityEngine;

[Serializable]
public class CollectableObject : ScriptableObject
{
    public ObjectType objectType; // Type of this object.
    public int objectCount=3; // There must be at least 3 for a match.
    public enum ObjectType // Object that we have.
    {
        BasketBall,
        Bear,
        Bee,
        Boat,
        Bucket,
        Bunny,
        Car,
        Dinosaur,
        Doll,
        Duck,
        Fish,
        Helicopter,
        Horse,
        Minibus,
        Plane,
        Robot,
        Rocket,
        RollerSkate,
        RugbyBall,
        Sheep,
        Truck,
        WaterGun,
        WoodToy
    }
}