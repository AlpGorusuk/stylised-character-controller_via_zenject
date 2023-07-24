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
    public void AddRbForceAtPosition(Vector3 force, Vector3 pos)
    {
        _rigidBody.AddForceAtPosition(force, pos);
    }
    public void AddForce(Vector3 _force)
    {
        _rigidBody.AddForce(_force);
    }
    public void AddForce(Vector3 _force, ForceMode _mode)
    {
        _rigidBody.AddForce(_force, _mode);
    }
    public Vector3 GetGravitationalForce()
    {
        return Physics.gravity * _rigidBody.mass;
    }
    public Vector3 RigidbodyVelocity
    {
        get { return _rigidBody.velocity; }
        set { _rigidBody.velocity = value; }
    }
    public Vector3 RigidbodyAngularVelocity
    {
        get { return _rigidBody.angularVelocity; }
        set { _rigidBody.angularVelocity = value; }
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
