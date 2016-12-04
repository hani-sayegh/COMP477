﻿using System.Linq;
using UnityEngine;

public class DraggableLowerArm : Draggable
{
    private GameObject _upperArm;
    private GameObject _lowerArm;

    private GameObject[] _childObjects;
    private Vector3[]    _childObjectPositions;
    private Quaternion[] _childObjectRotations;

    // Joints
    private Vector3 _lowerArmHingeJointAnchor;

    protected override void Start()
    {
        base.Start();

        OriginalPosition = transform.localPosition;
        OriginalRotation = transform.localRotation;

        _childObjects =
                GetComponentsInChildren<Transform>().Select(trans => trans.gameObject).ToArray();
        _childObjectPositions =
                GetComponentsInChildren<Transform>().Select(trans => trans.localPosition).ToArray();
        _childObjectRotations =
                GetComponentsInChildren<Transform>().Select(trans => trans.localRotation).ToArray();

        _upperArm = GameObject.Find($"{SideString}UpperArm");
        _lowerArm = GameObject.Find($"{SideString}LowerArm");

        _lowerArmHingeJointAnchor = _lowerArm.GetComponent<HingeJoint>().connectedAnchor;
    }

    public override void DestroyHingeJoints()
    {
        Destroy(_lowerArm.GetComponent<HingeJoint>());
        Destroy(_lowerArm.GetComponent<Rigidbody>());
    }

    public override void AddRigidBodies()
    {
        _lowerArm.AddComponent<Rigidbody>();
    }

    protected override void UpdateChildComponents()
    {
        for (var index = 0; index < _childObjects.Length; index++)
        {
            _childObjects[index].transform.localPosition = _childObjectPositions[index];
            _childObjects[index].transform.localRotation = _childObjectRotations[index];
        }
        
        var lowerJoint = _lowerArm.AddComponent<HingeJoint>();
        lowerJoint.autoConfigureConnectedAnchor = false;
        lowerJoint.connectedBody = _upperArm.GetComponent<Rigidbody>();
        lowerJoint.connectedAnchor = _lowerArmHingeJointAnchor;
    }

    public override void ModifyRigidBodies(bool isKinematic)
    {
        _lowerArm.GetComponent<Rigidbody>().isKinematic = isKinematic;
    }

    public override void CreateDetachedConfiguration()
    {
        base.CreateDetachedConfiguration();

        HingeJoints[0].connectedBody = Rigidbodies[1];
        var anch = HingeJoints[0].anchor;
        anch.y = -1;
        HingeJoints[0].anchor = anch;

        var mouseDrag = ObjectChildren[1].gameObject.AddComponent<SimpleMouseDrag>();
        ObjectChildren[1].position = CalculateMousePosition();
        mouseDrag.Draggable = this;
        mouseDrag.Rigidbodies = Rigidbodies;
        mouseDrag.HingeJoints = HingeJoints;
    }
}
