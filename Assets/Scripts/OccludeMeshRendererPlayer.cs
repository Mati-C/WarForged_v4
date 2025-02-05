﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccludeMeshRendererPlayer : MonoBehaviour
{
    public List<SkinnedMeshRenderer> _SkinMeshRenderPlayer = new List<SkinnedMeshRenderer>();
    Model_Player _player;
    public MeshRenderer _meshRenderPlayer;

    private void Awake()
    {
        _player = FindObjectOfType<Model_Player>();
        _SkinMeshRenderPlayer.AddRange(_player.GetComponentsInChildren<SkinnedMeshRenderer>());
        _meshRenderPlayer = _player.GetComponentInChildren<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider BoxCollisionWhithPlayer)
    {
        if (BoxCollisionWhithPlayer.gameObject.CompareTag("Player"))
        {
            foreach (var item in _SkinMeshRenderPlayer)
            {
                item.enabled = false;
            }
            _meshRenderPlayer.enabled = false;
        }
    }
    private void OnTriggerExit(Collider BoxCollisionWhithPlayer)
    {
        if (BoxCollisionWhithPlayer.gameObject.CompareTag("Player"))
        {
            foreach (var item in _SkinMeshRenderPlayer)
            {
                item.enabled = true;
            }
            _meshRenderPlayer.enabled = true;
        }
    }
}
