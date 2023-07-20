using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel
{
    readonly Rigidbody _rigidBody;

    public CharacterModel(
            Rigidbody rigidBody
           )
    {
        _rigidBody = rigidBody;
    }
    public void AddForceAtPosition(Vector3 force, Vector3 pos)
    {
        _rigidBody.AddForceAtPosition(force, pos);
    }
    public Vector3 GetRigidbodyVelocity()
    {
        return _rigidBody.velocity;
    }

    public float GetRigidbodyMass()
    {
        return _rigidBody.mass;
    }
    public Vector3 Position
    {
        get { return _rigidBody.position; }
        set { _rigidBody.position = value; }
    }
    public Vector3 GetLocalScale
    {
        get { return _rigidBody.transform.localScale; }
        set { _rigidBody.transform.localScale = value; }
    }
}
